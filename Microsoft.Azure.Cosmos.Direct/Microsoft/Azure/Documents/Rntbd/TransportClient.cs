using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.FaultInjection;
using Microsoft.Azure.Documents.Telemetry;

namespace Microsoft.Azure.Documents.Rntbd;

internal sealed class TransportClient : Microsoft.Azure.Documents.TransportClient, IDisposable
{
	private enum TransportResponseStatusCode
	{
		Success = 0,
		DocumentClientException = -1,
		UnknownException = -2
	}

	public sealed class Options
	{
		private UserAgentContainer userAgent;

		private TimeSpan openTimeout = TimeSpan.Zero;

		private TimeSpan localRegionOpenTimeout = TimeSpan.Zero;

		private TimeSpan timerPoolResolution = TimeSpan.Zero;

		public TimeSpan RequestTimeout { get; private set; }

		public int MaxChannels { get; set; }

		public int PartitionCount { get; set; }

		public int MaxRequestsPerChannel { get; set; }

		public TimeSpan ReceiveHangDetectionTime { get; set; }

		public TimeSpan SendHangDetectionTime { get; set; }

		public TimeSpan IdleTimeout { get; set; }

		public RntbdConstants.CallerId CallerId { get; set; }

		public bool EnableChannelMultiplexing { get; set; }

		public MemoryStreamPool MemoryStreamPool { get; set; }

		public UserAgentContainer UserAgent
		{
			get
			{
				if (userAgent != null)
				{
					return userAgent;
				}
				userAgent = new UserAgentContainer();
				return userAgent;
			}
			set
			{
				userAgent = value;
			}
		}

		public string CertificateHostNameOverride { get; set; }

		public IConnectionStateListener ConnectionStateListener { get; set; }

		public TimeSpan OpenTimeout
		{
			get
			{
				if (openTimeout > TimeSpan.Zero)
				{
					return openTimeout;
				}
				return RequestTimeout;
			}
			set
			{
				openTimeout = value;
			}
		}

		public TimeSpan LocalRegionOpenTimeout
		{
			get
			{
				if (localRegionOpenTimeout > TimeSpan.Zero)
				{
					return localRegionOpenTimeout;
				}
				return OpenTimeout;
			}
			set
			{
				localRegionOpenTimeout = value;
			}
		}

		public PortReuseMode PortReuseMode { get; set; }

		public int PortPoolReuseThreshold { get; internal set; }

		public int PortPoolBindAttempts { get; internal set; }

		public TimeSpan TimerPoolResolution
		{
			get
			{
				return GetTimerPoolResolutionSeconds(timerPoolResolution, RequestTimeout, openTimeout);
			}
			set
			{
				timerPoolResolution = value;
			}
		}

		public int MaxConcurrentOpeningConnectionCount { get; set; }

		public RemoteCertificateValidationCallback RemoteCertificateValidationCallback { get; internal set; }

		public Func<string, Task<IPAddress>> DnsResolutionFunction { get; internal set; }

		public DistributedTracingOptions DistributedTracingOptions { get; set; }

		public Options(TimeSpan requestTimeout)
		{
			RequestTimeout = requestTimeout;
			MaxChannels = 65535;
			PartitionCount = 1;
			MaxRequestsPerChannel = 30;
			PortReuseMode = PortReuseMode.ReuseUnicastPort;
			PortPoolReuseThreshold = 256;
			PortPoolBindAttempts = 5;
			ReceiveHangDetectionTime = TimeSpan.FromSeconds(65.0);
			SendHangDetectionTime = TimeSpan.FromSeconds(10.0);
			IdleTimeout = TimeSpan.FromSeconds(1800.0);
			CallerId = RntbdConstants.CallerId.Anonymous;
			EnableChannelMultiplexing = false;
			MaxConcurrentOpeningConnectionCount = 65535;
			DnsResolutionFunction = null;
			DistributedTracingOptions = null;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Rntbd.TransportClient.Options");
			stringBuilder.Append("  OpenTimeout: ");
			stringBuilder.AppendLine(OpenTimeout.ToString("c"));
			stringBuilder.Append("  RequestTimeout: ");
			stringBuilder.AppendLine(RequestTimeout.ToString("c"));
			stringBuilder.Append("  TimerPoolResolution: ");
			stringBuilder.AppendLine(TimerPoolResolution.ToString("c"));
			stringBuilder.Append("  MaxChannels: ");
			stringBuilder.AppendLine(MaxChannels.ToString(CultureInfo.InvariantCulture));
			stringBuilder.Append("  PartitionCount: ");
			stringBuilder.AppendLine(PartitionCount.ToString(CultureInfo.InvariantCulture));
			stringBuilder.Append("  MaxRequestsPerChannel: ");
			stringBuilder.AppendLine(MaxRequestsPerChannel.ToString(CultureInfo.InvariantCulture));
			stringBuilder.Append("  ReceiveHangDetectionTime: ");
			stringBuilder.AppendLine(ReceiveHangDetectionTime.ToString("c"));
			stringBuilder.Append("  SendHangDetectionTime: ");
			stringBuilder.AppendLine(SendHangDetectionTime.ToString("c"));
			stringBuilder.Append("  IdleTimeout: ");
			stringBuilder.AppendLine(IdleTimeout.ToString("c"));
			stringBuilder.Append("  UserAgent: ");
			stringBuilder.Append(UserAgent.UserAgent);
			stringBuilder.Append(" Suffix: ");
			stringBuilder.AppendLine(UserAgent.Suffix);
			stringBuilder.Append("  CertificateHostNameOverride: ");
			stringBuilder.AppendLine(CertificateHostNameOverride);
			stringBuilder.Append("  LocalRegionTimeout: ");
			stringBuilder.AppendLine(LocalRegionOpenTimeout.ToString("c"));
			stringBuilder.Append("  EnableChannelMultiplexing: ");
			stringBuilder.AppendLine(EnableChannelMultiplexing.ToString());
			stringBuilder.Append("  MaxConcurrentOpeningConnectionCount: ");
			stringBuilder.AppendLine(MaxConcurrentOpeningConnectionCount.ToString(CultureInfo.InvariantCulture));
			stringBuilder.Append("  Use_RecyclableMemoryStream: ");
			stringBuilder.AppendLine((MemoryStreamPool != null) ? bool.TrueString : bool.FalseString);
			stringBuilder.Append("  Use_CustomDnsResolution: ");
			stringBuilder.AppendLine((DnsResolutionFunction != null) ? bool.TrueString : bool.FalseString);
			stringBuilder.Append("  IsDistributedTracingEnabled: ");
			stringBuilder.AppendLine(DistributedTracingOptions?.IsDistributedTracingEnabled.ToString());
			return stringBuilder.ToString();
		}

		private static TimeSpan GetTimerPoolResolutionSeconds(TimeSpan timerPoolResolution, TimeSpan requestTimeout, TimeSpan openTimeout)
		{
			if (timerPoolResolution > TimeSpan.Zero && timerPoolResolution < openTimeout && timerPoolResolution < requestTimeout)
			{
				return timerPoolResolution;
			}
			if (openTimeout > TimeSpan.Zero && requestTimeout > TimeSpan.Zero)
			{
				if (!(openTimeout < requestTimeout))
				{
					return requestTimeout;
				}
				return openTimeout;
			}
			if (!(openTimeout > TimeSpan.Zero))
			{
				return requestTimeout;
			}
			return openTimeout;
		}
	}

	private static TransportPerformanceCounters transportPerformanceCounters = new TransportPerformanceCounters();

	private readonly TimerPool TimerPool;

	private readonly TimerPool IdleTimerPool;

	private readonly ChannelDictionary channelDictionary;

	private bool disposed;

	private readonly DistributedTracingOptions DistributedTracingOptions;

	private readonly object disableRntbdChannelLock = new object();

	private bool disableRntbdChannel;

	public event Action OnDisableRntbdChannel;

	public TransportClient(Options clientOptions, IChaosInterceptor chaosInterceptor = null)
	{
		if (clientOptions == null)
		{
			throw new ArgumentNullException("clientOptions");
		}
		LogClientOptions(clientOptions);
		UserPortPool userPortPool = null;
		if (clientOptions.PortReuseMode == PortReuseMode.PrivatePortPool)
		{
			userPortPool = new UserPortPool(clientOptions.PortPoolReuseThreshold, clientOptions.PortPoolBindAttempts);
		}
		TimerPool = new TimerPool((int)clientOptions.TimerPoolResolution.TotalSeconds);
		if (clientOptions.IdleTimeout > TimeSpan.Zero)
		{
			IdleTimerPool = new TimerPool(30);
		}
		else
		{
			IdleTimerPool = null;
		}
		DistributedTracingOptions = clientOptions.DistributedTracingOptions;
		channelDictionary = new ChannelDictionary(new ChannelProperties(clientOptions.UserAgent, clientOptions.CertificateHostNameOverride, clientOptions.ConnectionStateListener, TimerPool, clientOptions.RequestTimeout, clientOptions.OpenTimeout, clientOptions.LocalRegionOpenTimeout, clientOptions.PortReuseMode, userPortPool, clientOptions.MaxChannels, clientOptions.PartitionCount, clientOptions.MaxRequestsPerChannel, clientOptions.MaxConcurrentOpeningConnectionCount, clientOptions.ReceiveHangDetectionTime, clientOptions.SendHangDetectionTime, clientOptions.IdleTimeout, IdleTimerPool, clientOptions.CallerId, clientOptions.EnableChannelMultiplexing, clientOptions.MemoryStreamPool, clientOptions.RemoteCertificateValidationCallback, clientOptions.DnsResolutionFunction), chaosInterceptor);
	}

	internal override Task<StoreResponse> InvokeStoreAsync(Uri physicalAddress, ResourceOperation resourceOperation, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(new TransportAddressUri(physicalAddress), resourceOperation, request);
	}

	internal override async Task<StoreResponse> InvokeStoreAsync(TransportAddressUri physicalAddress, ResourceOperation resourceOperation, DocumentServiceRequest request)
	{
		ThrowIfDisposed();
		Guid activityId = Trace.CorrelationManager.ActivityId;
		if (!request.IsBodySeekableClonableAndCountable)
		{
			throw new InternalServerErrorException();
		}
		StoreResponse storeResponse = null;
		TransportRequestStats transportRequestStats = new TransportRequestStats();
		string operation = "Unknown operation";
		DateTime requestStartTime = DateTime.UtcNow;
		int transportResponseStatusCode = 0;
		using OpenTelemetryRecorder recorder = OpenTelemetryRecorderFactory.CreateRecorder(DistributedTracingOptions, request);
		try
		{
			IncrementCounters();
			operation = "GetChannel";
			bool localRegionRequest = !request.RequestContext.IsRetry && request.RequestContext.LocalRegionRequest;
			IChannel channel = channelDictionary.GetChannel(physicalAddress.Uri, localRegionRequest);
			GetTransportPerformanceCounters().IncrementRntbdRequestCount(resourceOperation.resourceType, resourceOperation.operationType);
			operation = "RequestAsync";
			storeResponse = await channel.RequestAsync(request, physicalAddress, resourceOperation, activityId, transportRequestStats);
			transportRequestStats.RecordState(TransportRequestStats.RequestStage.Completed);
			storeResponse.TransportRequestStats = transportRequestStats;
		}
		catch (TransportException ex)
		{
			transportRequestStats.RecordState(TransportRequestStats.RequestStage.Failed);
			transportResponseStatusCode = (int)ex.ErrorCode;
			ex.RequestStartTime = requestStartTime;
			ex.RequestEndTime = DateTime.UtcNow;
			ex.OperationType = resourceOperation.operationType;
			ex.ResourceType = resourceOperation.resourceType;
			GetTransportPerformanceCounters().IncrementRntbdResponseCount(resourceOperation.resourceType, resourceOperation.operationType, (int)ex.ErrorCode);
			DefaultTrace.TraceInformation("{0} failed: RID: {1}, Resource Type: {2}, Op: {3}, Address: {4}, Exception: {5}", operation, request.ResourceAddress, request.ResourceType, resourceOperation, physicalAddress, ex);
			if (request.IsReadOnlyRequest)
			{
				DefaultTrace.TraceInformation("Converting to Gone (read-only request)");
				GoneException goneException = TransportExceptions.GetGoneException(physicalAddress.Uri, activityId, ex, transportRequestStats);
				recorder?.Record(physicalAddress.Uri, goneException);
				throw goneException;
			}
			if (!ex.UserRequestSent)
			{
				DefaultTrace.TraceInformation("Converting to Gone (write request, not sent)");
				GoneException goneException2 = TransportExceptions.GetGoneException(physicalAddress.Uri, activityId, ex, transportRequestStats);
				recorder?.Record(physicalAddress.Uri, goneException2);
				throw goneException2;
			}
			if (TransportException.IsTimeout(ex.ErrorCode))
			{
				DefaultTrace.TraceInformation("Converting to RequestTimeout");
				RequestTimeoutException requestTimeoutException = TransportExceptions.GetRequestTimeoutException(physicalAddress.Uri, activityId, ex, transportRequestStats);
				recorder?.Record(physicalAddress.Uri, requestTimeoutException);
				throw requestTimeoutException;
			}
			DefaultTrace.TraceInformation("Converting to ServiceUnavailable");
			ServiceUnavailableException serviceUnavailableException = TransportExceptions.GetServiceUnavailableException(physicalAddress.Uri, activityId, ex, transportRequestStats);
			recorder?.Record(physicalAddress.Uri, serviceUnavailableException);
			throw serviceUnavailableException;
		}
		catch (DocumentClientException ex2)
		{
			transportResponseStatusCode = -1;
			DefaultTrace.TraceInformation("{0} failed: RID: {1}, Resource Type: {2}, Op: {3}, Address: {4}, Exception: {5}", operation, request.ResourceAddress, request.ResourceType, resourceOperation, physicalAddress, ex2);
			transportRequestStats.RecordState(TransportRequestStats.RequestStage.Failed);
			ex2.TransportRequestStats = transportRequestStats;
			recorder?.Record(physicalAddress.Uri, ex2);
			throw;
		}
		catch (Exception ex3)
		{
			transportResponseStatusCode = -2;
			DefaultTrace.TraceInformation("{0} failed: RID: {1}, Resource Type: {2}, Op: {3}, Address: {4}, Exception: {5}", operation, request.ResourceAddress, request.ResourceType, resourceOperation, physicalAddress, ex3);
			recorder?.Record(physicalAddress.Uri, ex3);
			throw;
		}
		finally
		{
			DecrementCounters();
			GetTransportPerformanceCounters().IncrementRntbdResponseCount(resourceOperation.resourceType, resourceOperation.operationType, transportResponseStatusCode);
			RaiseProtocolDowngradeRequest(storeResponse);
		}
		try
		{
			Microsoft.Azure.Documents.TransportClient.ThrowServerException(request.ResourceAddress, storeResponse, physicalAddress.Uri, activityId, request);
		}
		catch (DocumentClientException exception)
		{
			recorder?.Record(physicalAddress.Uri, exception);
			throw;
		}
		recorder?.Record(physicalAddress.Uri, null, storeResponse);
		return storeResponse;
	}

	public override void Dispose()
	{
		ThrowIfDisposed();
		disposed = true;
		channelDictionary.Dispose();
		if (IdleTimerPool != null)
		{
			IdleTimerPool.Dispose();
		}
		TimerPool.Dispose();
		base.Dispose();
		DefaultTrace.TraceInformation("Rntbd.TransportClient disposed.");
	}

	private void ThrowIfDisposed()
	{
		if (disposed)
		{
			throw new ObjectDisposedException("TransportClient");
		}
	}

	private static void LogClientOptions(Options clientOptions)
	{
		DefaultTrace.TraceInformation("Creating RNTBD TransportClient with options {0}", clientOptions.ToString());
	}

	private static void IncrementCounters()
	{
	}

	private static void DecrementCounters()
	{
	}

	internal override Task OpenConnectionAsync(Uri physicalAddress)
	{
		return channelDictionary.GetChannel(physicalAddress, localRegionRequest: false).OpenChannelAsync(Trace.CorrelationManager.ActivityId);
	}

	private void RaiseProtocolDowngradeRequest(StoreResponse storeResponse)
	{
		if (storeResponse == null)
		{
			return;
		}
		string value = null;
		if (!storeResponse.TryGetHeaderValue("x-ms-disable-rntbd-channel", out value) || !string.Equals(value, "true"))
		{
			return;
		}
		bool flag = false;
		lock (disableRntbdChannelLock)
		{
			if (disableRntbdChannel)
			{
				return;
			}
			disableRntbdChannel = true;
			flag = true;
		}
		if (flag)
		{
			Task.Factory.StartNewOnCurrentTaskSchedulerAsync(delegate
			{
				this.OnDisableRntbdChannel?.Invoke();
			}).ContinueWith(delegate(Task failedTask)
			{
				DefaultTrace.TraceError("RNTBD channel callback failed: {0}", failedTask.Exception);
			}, default(CancellationToken), TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Current);
		}
	}

	internal static void SetTransportPerformanceCounters(TransportPerformanceCounters transportPerformanceCounters)
	{
		if (transportPerformanceCounters == null)
		{
			throw new ArgumentNullException("transportPerformanceCounters");
		}
		TransportClient.transportPerformanceCounters = transportPerformanceCounters;
	}

	internal static TransportPerformanceCounters GetTransportPerformanceCounters()
	{
		return transportPerformanceCounters;
	}
}
