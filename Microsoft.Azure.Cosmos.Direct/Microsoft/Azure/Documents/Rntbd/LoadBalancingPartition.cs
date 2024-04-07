using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.FaultInjection;

namespace Microsoft.Azure.Documents.Rntbd;

internal sealed class LoadBalancingPartition : IDisposable
{
	private sealed class SequenceGenerator
	{
		private int current;

		public uint Next()
		{
			return (uint)(2147483648u + Interlocked.Increment(ref current));
		}
	}

	private readonly Uri serverUri;

	private readonly ChannelProperties channelProperties;

	private readonly bool localRegionRequest;

	private readonly int maxCapacity;

	private int requestsPending;

	private readonly SequenceGenerator sequenceGenerator = new SequenceGenerator();

	private readonly ReaderWriterLockSlim capacityLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

	private int capacity;

	private readonly List<LbChannelState> openChannels = new List<LbChannelState>();

	private readonly SemaphoreSlim concurrentOpeningChannelSlim;

	private readonly IChaosInterceptor chaosInterceptor;

	private readonly Func<Guid, Uri, ChannelProperties, bool, SemaphoreSlim, IChaosInterceptor, Func<Guid, Guid, Uri, Channel, Task>, IChannel> channelFactory;

	public LoadBalancingPartition(Uri serverUri, ChannelProperties channelProperties, bool localRegionRequest, Func<Guid, Uri, ChannelProperties, bool, SemaphoreSlim, IChaosInterceptor, Func<Guid, Guid, Uri, Channel, Task>, IChannel> channelFactory = null, IChaosInterceptor chaosInterceptor = null)
	{
		this.serverUri = serverUri;
		this.channelProperties = channelProperties;
		this.localRegionRequest = localRegionRequest;
		maxCapacity = checked(channelProperties.MaxChannels * channelProperties.MaxRequestsPerChannel);
		concurrentOpeningChannelSlim = new SemaphoreSlim(channelProperties.MaxConcurrentOpeningConnectionCount, channelProperties.MaxConcurrentOpeningConnectionCount);
		this.channelFactory = ((channelFactory != null) ? channelFactory : new Func<Guid, Uri, ChannelProperties, bool, SemaphoreSlim, IChaosInterceptor, Func<Guid, Guid, Uri, Channel, Task>, IChannel>(CreateAndInitializeChannel));
		this.chaosInterceptor = chaosInterceptor;
	}

	public async Task<StoreResponse> RequestAsync(DocumentServiceRequest request, TransportAddressUri physicalAddress, ResourceOperation resourceOperation, Guid activityId, TransportRequestStats transportRequestStats)
	{
		int num = Interlocked.Increment(ref requestsPending);
		transportRequestStats.NumberOfInflightRequestsToEndpoint = num;
		try
		{
			if (num > maxCapacity)
			{
				throw new RequestRateTooLargeException($"All connections to {serverUri} are fully utilized. Increase the maximum number of connections or the maximum number of requests per connection", SubStatusCodes.ClientTcpChannelFull);
			}
			transportRequestStats.RecordState(TransportRequestStats.RequestStage.ChannelAcquisitionStarted);
			while (true)
			{
				LbChannelState channelState = null;
				bool flag = false;
				uint num2 = sequenceGenerator.Next();
				capacityLock.EnterReadLock();
				try
				{
					transportRequestStats.NumberOfOpenConnectionsToEndpoint = openChannels.Count;
					if (num <= capacity)
					{
						int index = (int)(num2 % openChannels.Count);
						LbChannelState lbChannelState = openChannels[index];
						if (lbChannelState.Enter())
						{
							channelState = lbChannelState;
						}
					}
					else
					{
						flag = true;
					}
				}
				finally
				{
					capacityLock.ExitReadLock();
				}
				if (channelState != null)
				{
					try
					{
						if (channelState.DeepHealthy)
						{
							return await channelState.Channel.RequestAsync(request, physicalAddress, resourceOperation, activityId, transportRequestStats);
						}
						capacityLock.EnterWriteLock();
						try
						{
							if (openChannels.Remove(channelState))
							{
								capacity -= channelProperties.MaxRequestsPerChannel;
							}
						}
						finally
						{
							capacityLock.ExitWriteLock();
						}
					}
					finally
					{
						if (channelState.Exit() && !channelState.ShallowHealthy)
						{
							channelState.Dispose();
							DefaultTrace.TraceInformation("Closed unhealthy channel {0}", channelState.Channel);
						}
					}
				}
				else
				{
					if (!flag)
					{
						continue;
					}
					int num3 = MathUtils.CeilingMultiple(num, channelProperties.MaxRequestsPerChannel) / channelProperties.MaxRequestsPerChannel;
					int num4 = 0;
					capacityLock.EnterWriteLock();
					try
					{
						if (openChannels.Count < num3)
						{
							num4 = num3 - openChannels.Count;
						}
						Func<Guid, Guid, Uri, Channel, Task> onChannelOpen = async delegate(Guid createId, Guid connectionCorrelationId, Uri serverUri, Channel createdChannel)
						{
							if (chaosInterceptor != null)
							{
								await chaosInterceptor.OnChannelOpenAsync(createId, connectionCorrelationId, serverUri, request, createdChannel);
							}
						};
						while (openChannels.Count < num3)
						{
							OpenChannelAndIncrementCapacity(activityId, onChannelOpen);
						}
					}
					finally
					{
						capacityLock.ExitWriteLock();
					}
					if (num4 > 0)
					{
						DefaultTrace.TraceInformation("Opened {0} channels to server {1}", num4, serverUri);
					}
				}
			}
		}
		finally
		{
			Interlocked.Decrement(ref requestsPending);
		}
	}

	internal Task OpenChannelAsync(Guid activityId)
	{
		IChannel channel = null;
		capacityLock.EnterUpgradeableReadLock();
		try
		{
			if (capacity >= maxCapacity)
			{
				throw new InvalidOperationException($"Failed to open channels to server {serverUri} because the current channel capacity {capacity} has exceeded the maaximum channel capacity limit: {maxCapacity}");
			}
			foreach (LbChannelState openChannel in openChannels)
			{
				if (openChannel.DeepHealthy)
				{
					return Task.FromResult(0);
				}
			}
			capacityLock.EnterWriteLock();
			try
			{
				channel = OpenChannelAndIncrementCapacity(activityId);
			}
			finally
			{
				capacityLock.ExitWriteLock();
			}
		}
		finally
		{
			capacityLock.ExitUpgradeableReadLock();
		}
		if (channel == null)
		{
			throw new InvalidOperationException($"Could not open a channel to server {serverUri} because a channel instance didn't get created.");
		}
		return channel.OpenChannelAsync(activityId);
	}

	public void Dispose()
	{
		capacityLock.EnterWriteLock();
		try
		{
			foreach (LbChannelState openChannel in openChannels)
			{
				openChannel.Dispose();
			}
		}
		finally
		{
			capacityLock.ExitWriteLock();
		}
		try
		{
			capacityLock.Dispose();
		}
		catch (SynchronizationLockException)
		{
		}
	}

	private IChannel OpenChannelAndIncrementCapacity(Guid activityId, Func<Guid, Guid, Uri, Channel, Task> onChannelOpen = null)
	{
		IChannel channel = channelFactory(activityId, serverUri, channelProperties, localRegionRequest, concurrentOpeningChannelSlim, chaosInterceptor, onChannelOpen);
		if (channel == null)
		{
			throw new ArgumentNullException("newChannel", "Channel can't be null.");
		}
		openChannels.Add(new LbChannelState(channel, channelProperties.MaxRequestsPerChannel));
		capacity += channelProperties.MaxRequestsPerChannel;
		return channel;
	}

	private static IChannel CreateAndInitializeChannel(Guid activityId, Uri serverUri, ChannelProperties channelProperties, bool localRegionRequest, SemaphoreSlim concurrentOpeningChannelSlim, IChaosInterceptor chaosInterceptor = null, Func<Guid, Guid, Uri, Channel, Task> onChannelOpen = null)
	{
		return new Channel(activityId, serverUri, channelProperties, localRegionRequest, concurrentOpeningChannelSlim, chaosInterceptor, onChannelOpen);
	}
}
