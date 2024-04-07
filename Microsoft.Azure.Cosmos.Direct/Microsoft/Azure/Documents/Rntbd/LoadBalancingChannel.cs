using System;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.FaultInjection;

namespace Microsoft.Azure.Documents.Rntbd;

internal sealed class LoadBalancingChannel : IChannel, IDisposable
{
	private readonly Uri serverUri;

	private readonly LoadBalancingPartition singlePartition;

	private readonly LoadBalancingPartition[] partitions;

	private bool disposed;

	public bool Healthy
	{
		get
		{
			ThrowIfDisposed();
			return true;
		}
	}

	public LoadBalancingChannel(Uri serverUri, ChannelProperties channelProperties, bool localRegionRequest, IChaosInterceptor chaosInterceptor = null)
	{
		this.serverUri = serverUri;
		if (channelProperties.PartitionCount < 1 || channelProperties.PartitionCount > 8)
		{
			throw new ArgumentOutOfRangeException("PartitionCount", channelProperties.PartitionCount, "The partition count must be between 1 and 8");
		}
		if (channelProperties.PartitionCount > 1)
		{
			ChannelProperties channelProperties2 = new ChannelProperties(channelProperties.UserAgent, channelProperties.CertificateHostNameOverride, channelProperties.ConnectionStateListener, channelProperties.RequestTimerPool, channelProperties.RequestTimeout, channelProperties.OpenTimeout, channelProperties.LocalRegionOpenTimeout, channelProperties.PortReuseMode, channelProperties.UserPortPool, MathUtils.CeilingMultiple(channelProperties.MaxChannels, channelProperties.PartitionCount) / channelProperties.PartitionCount, 1, channelProperties.MaxRequestsPerChannel, channelProperties.MaxConcurrentOpeningConnectionCount, channelProperties.ReceiveHangDetectionTime, channelProperties.SendHangDetectionTime, channelProperties.IdleTimeout, channelProperties.IdleTimerPool, channelProperties.CallerId, channelProperties.EnableChannelMultiplexing, channelProperties.MemoryStreamPool, channelProperties.RemoteCertificateValidationCallback, channelProperties.DnsResolutionFunction);
			partitions = new LoadBalancingPartition[channelProperties.PartitionCount];
			for (int i = 0; i < partitions.Length; i++)
			{
				partitions[i] = new LoadBalancingPartition(serverUri, channelProperties2, localRegionRequest, null, chaosInterceptor);
			}
		}
		else
		{
			singlePartition = new LoadBalancingPartition(serverUri, channelProperties, localRegionRequest, null, chaosInterceptor);
		}
	}

	public Task<StoreResponse> RequestAsync(DocumentServiceRequest request, TransportAddressUri physicalAddress, ResourceOperation resourceOperation, Guid activityId, TransportRequestStats transportRequestStats)
	{
		ThrowIfDisposed();
		if (singlePartition != null)
		{
			return singlePartition.RequestAsync(request, physicalAddress, resourceOperation, activityId, transportRequestStats);
		}
		return GetLoadBalancedPartition(activityId).RequestAsync(request, physicalAddress, resourceOperation, activityId, transportRequestStats);
	}

	public Task OpenChannelAsync(Guid activityId)
	{
		ThrowIfDisposed();
		if (singlePartition != null)
		{
			return singlePartition.OpenChannelAsync(activityId);
		}
		return GetLoadBalancedPartition(activityId).OpenChannelAsync(activityId);
	}

	private LoadBalancingPartition GetLoadBalancedPartition(Guid activityId)
	{
		int hashCode = activityId.GetHashCode();
		return partitions[(hashCode & 0x8FFFFFFFu) % partitions.Length];
	}

	public void Close()
	{
		((IDisposable)this).Dispose();
	}

	void IDisposable.Dispose()
	{
		ThrowIfDisposed();
		disposed = true;
		if (singlePartition != null)
		{
			singlePartition.Dispose();
		}
		if (partitions != null)
		{
			for (int i = 0; i < partitions.Length; i++)
			{
				partitions[i].Dispose();
			}
		}
	}

	private void ThrowIfDisposed()
	{
		if (disposed)
		{
			throw new ObjectDisposedException(string.Format("{0}:{1}", "LoadBalancingChannel", serverUri));
		}
	}
}
