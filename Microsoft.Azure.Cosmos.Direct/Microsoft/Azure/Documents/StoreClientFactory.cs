using System;
using System.Net;
using System.Net.Security;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.FaultInjection;
using Microsoft.Azure.Documents.Rntbd;
using Microsoft.Azure.Documents.Telemetry;

namespace Microsoft.Azure.Documents;

internal sealed class StoreClientFactory : IStoreClientFactory, IDisposable
{
	private bool isDisposed;

	private readonly Protocol protocol;

	private readonly RetryWithConfiguration retryWithConfiguration;

	private readonly bool disableRetryWithRetryPolicy;

	private TransportClient transportClient;

	private TransportClient fallbackTransportClient;

	private ConnectionStateListener connectionStateListener;

	public StoreClientFactory(Protocol protocol, int requestTimeoutInSeconds, int maxConcurrentConnectionOpenRequests, UserAgentContainer userAgent = null, ICommunicationEventSource eventSource = null, string overrideHostNameInCertificate = null, int openTimeoutInSeconds = 0, int idleTimeoutInSeconds = -1, int timerPoolGranularityInSeconds = 0, int maxRntbdChannels = 65535, int rntbdPartitionCount = 1, int maxRequestsPerRntbdChannel = 30, PortReuseMode rntbdPortReuseMode = PortReuseMode.ReuseUnicastPort, int rntbdPortPoolReuseThreshold = 256, int rntbdPortPoolBindAttempts = 5, int receiveHangDetectionTimeSeconds = 65, int sendHangDetectionTimeSeconds = 10, bool disableRetryWithRetryPolicy = false, RetryWithConfiguration retryWithConfiguration = null, RntbdConstants.CallerId callerId = RntbdConstants.CallerId.Anonymous, bool enableTcpConnectionEndpointRediscovery = false, IAddressResolver addressResolver = null, TimeSpan localRegionOpenTimeout = default(TimeSpan), bool enableChannelMultiplexing = false, int rntbdMaxConcurrentOpeningConnectionCount = 65535, MemoryStreamPool memoryStreamPool = null, RemoteCertificateValidationCallback remoteCertificateValidationCallback = null, Func<string, Task<IPAddress>> dnsResolutionFunction = null, DistributedTracingOptions distributedTracingOptions = null, IChaosInterceptor chaosInterceptor = null)
	{
		if (idleTimeoutInSeconds > 0 && idleTimeoutInSeconds < 600)
		{
			throw new ArgumentOutOfRangeException("idleTimeoutInSeconds");
		}
		switch (protocol)
		{
		case Protocol.Https:
			if (eventSource == null)
			{
				throw new ArgumentOutOfRangeException("eventSource");
			}
			transportClient = new HttpTransportClient(requestTimeoutInSeconds, eventSource, userAgent, idleTimeoutInSeconds);
			break;
		case Protocol.Tcp:
			if (maxRntbdChannels <= 0)
			{
				throw new ArgumentOutOfRangeException("maxRntbdChannels");
			}
			if (rntbdPartitionCount < 1 || rntbdPartitionCount > 8)
			{
				throw new ArgumentOutOfRangeException("rntbdPartitionCount");
			}
			if (maxRequestsPerRntbdChannel <= 0)
			{
				throw new ArgumentOutOfRangeException("maxRequestsPerRntbdChannel");
			}
			if (maxRntbdChannels > 65535)
			{
				DefaultTrace.TraceWarning("The value of {0} is unreasonably large. Received: {1}. Use {2} to represent \"effectively infinite\".", "maxRntbdChannels", maxRntbdChannels, ushort.MaxValue);
			}
			if (maxRequestsPerRntbdChannel < 6)
			{
				DefaultTrace.TraceWarning("The value of {0} is unreasonably small. Received: {1}. Small values of {0} can cause a large number of RNTBD channels to be opened to the same back-end. Reasonable values are between {2} and {3}", "maxRequestsPerRntbdChannel", maxRequestsPerRntbdChannel, 6, 256);
			}
			if (maxRequestsPerRntbdChannel > 256)
			{
				DefaultTrace.TraceWarning("The value of {0} is unreasonably large. Received: {1}. Large values of {0} can cause significant head-of-line blocking over RNTBD channels. Reasonable values are between {2} and {3}", "maxRequestsPerRntbdChannel", maxRequestsPerRntbdChannel, 6, 256);
			}
			if (checked(maxRntbdChannels * maxRequestsPerRntbdChannel) < 512)
			{
				DefaultTrace.TraceWarning("The number of simultaneous requests allowed per backend is unreasonably small. Received {0} = {1}, {2} = {3}. Reasonable values are at least {4}", "maxRntbdChannels", maxRntbdChannels, "maxRequestsPerRntbdChannel", maxRequestsPerRntbdChannel, 512);
			}
			ValidatePortPoolReuseThreshold(ref rntbdPortPoolReuseThreshold);
			ValidatePortPoolBindAttempts(ref rntbdPortPoolBindAttempts);
			if (rntbdPortPoolBindAttempts > rntbdPortPoolReuseThreshold)
			{
				DefaultTrace.TraceWarning("Raising the value of {0} from {1} to {2} to match the value of {3}", "rntbdPortPoolReuseThreshold", rntbdPortPoolReuseThreshold, rntbdPortPoolBindAttempts + 1, "rntbdPortPoolBindAttempts");
				rntbdPortPoolReuseThreshold = rntbdPortPoolBindAttempts;
			}
			if (receiveHangDetectionTimeSeconds < 65)
			{
				DefaultTrace.TraceWarning("The value of {0} is too small. Received {1}. Adjusting to {2}", "receiveHangDetectionTimeSeconds", receiveHangDetectionTimeSeconds, 65);
				receiveHangDetectionTimeSeconds = 65;
			}
			if (receiveHangDetectionTimeSeconds > 180)
			{
				DefaultTrace.TraceWarning("The value of {0} is too large. Received {1}. Adjusting to {2}", "receiveHangDetectionTimeSeconds", receiveHangDetectionTimeSeconds, 180);
				receiveHangDetectionTimeSeconds = 180;
			}
			if (sendHangDetectionTimeSeconds < 2)
			{
				DefaultTrace.TraceWarning("The value of {0} is too small. Received {1}. Adjusting to {2}", "sendHangDetectionTimeSeconds", sendHangDetectionTimeSeconds, 2);
				sendHangDetectionTimeSeconds = 2;
			}
			if (sendHangDetectionTimeSeconds > 60)
			{
				DefaultTrace.TraceWarning("The value of {0} is too large. Received {1}. Adjusting to {2}", "sendHangDetectionTimeSeconds", sendHangDetectionTimeSeconds, 60);
				sendHangDetectionTimeSeconds = 60;
			}
			if (enableTcpConnectionEndpointRediscovery && addressResolver != null)
			{
				connectionStateListener = new ConnectionStateListener(addressResolver);
			}
			ValidateRntbdMaxConcurrentOpeningConnectionCount(ref rntbdMaxConcurrentOpeningConnectionCount);
			transportClient = new Microsoft.Azure.Documents.Rntbd.TransportClient(new Microsoft.Azure.Documents.Rntbd.TransportClient.Options(TimeSpan.FromSeconds(requestTimeoutInSeconds))
			{
				MaxChannels = maxRntbdChannels,
				PartitionCount = rntbdPartitionCount,
				MaxRequestsPerChannel = maxRequestsPerRntbdChannel,
				PortReuseMode = rntbdPortReuseMode,
				PortPoolReuseThreshold = rntbdPortPoolReuseThreshold,
				PortPoolBindAttempts = rntbdPortPoolBindAttempts,
				ReceiveHangDetectionTime = TimeSpan.FromSeconds(receiveHangDetectionTimeSeconds),
				SendHangDetectionTime = TimeSpan.FromSeconds(sendHangDetectionTimeSeconds),
				UserAgent = userAgent,
				CertificateHostNameOverride = overrideHostNameInCertificate,
				OpenTimeout = TimeSpan.FromSeconds(openTimeoutInSeconds),
				LocalRegionOpenTimeout = localRegionOpenTimeout,
				TimerPoolResolution = TimeSpan.FromSeconds(timerPoolGranularityInSeconds),
				IdleTimeout = TimeSpan.FromSeconds(idleTimeoutInSeconds),
				CallerId = callerId,
				ConnectionStateListener = connectionStateListener,
				EnableChannelMultiplexing = enableChannelMultiplexing,
				MaxConcurrentOpeningConnectionCount = rntbdMaxConcurrentOpeningConnectionCount,
				MemoryStreamPool = memoryStreamPool,
				RemoteCertificateValidationCallback = remoteCertificateValidationCallback,
				DnsResolutionFunction = dnsResolutionFunction,
				DistributedTracingOptions = distributedTracingOptions
			}, chaosInterceptor);
			fallbackTransportClient = new Microsoft.Azure.Documents.Rntbd.TransportClient(new Microsoft.Azure.Documents.Rntbd.TransportClient.Options(TimeSpan.FromSeconds(requestTimeoutInSeconds))
			{
				MaxChannels = maxRntbdChannels,
				PartitionCount = rntbdPartitionCount,
				MaxRequestsPerChannel = 1,
				PortReuseMode = rntbdPortReuseMode,
				PortPoolReuseThreshold = rntbdPortPoolReuseThreshold,
				PortPoolBindAttempts = rntbdPortPoolBindAttempts,
				ReceiveHangDetectionTime = TimeSpan.FromSeconds(receiveHangDetectionTimeSeconds),
				SendHangDetectionTime = TimeSpan.FromSeconds(sendHangDetectionTimeSeconds),
				UserAgent = userAgent,
				CertificateHostNameOverride = overrideHostNameInCertificate,
				OpenTimeout = TimeSpan.FromSeconds(openTimeoutInSeconds),
				LocalRegionOpenTimeout = localRegionOpenTimeout,
				TimerPoolResolution = TimeSpan.FromSeconds(timerPoolGranularityInSeconds),
				IdleTimeout = TimeSpan.FromSeconds(idleTimeoutInSeconds),
				CallerId = callerId,
				ConnectionStateListener = connectionStateListener,
				EnableChannelMultiplexing = enableChannelMultiplexing,
				MaxConcurrentOpeningConnectionCount = rntbdMaxConcurrentOpeningConnectionCount,
				MemoryStreamPool = memoryStreamPool,
				RemoteCertificateValidationCallback = remoteCertificateValidationCallback,
				DnsResolutionFunction = dnsResolutionFunction,
				DistributedTracingOptions = distributedTracingOptions
			}, chaosInterceptor);
			break;
		default:
			throw new ArgumentOutOfRangeException("protocol", protocol, "Invalid protocol value");
		}
		this.protocol = protocol;
		this.retryWithConfiguration = retryWithConfiguration;
		this.disableRetryWithRetryPolicy = disableRetryWithRetryPolicy;
	}

	private StoreClientFactory(Protocol protocol, RetryWithConfiguration retryWithConfiguration, TransportClient transportClient, TransportClient fallbackTransportClient, ConnectionStateListener connectionStateListener)
	{
		this.protocol = protocol;
		this.retryWithConfiguration = retryWithConfiguration;
		this.transportClient = transportClient;
		this.fallbackTransportClient = fallbackTransportClient;
		this.connectionStateListener = connectionStateListener;
	}

	internal StoreClientFactory Clone()
	{
		return new StoreClientFactory(protocol, retryWithConfiguration, transportClient, fallbackTransportClient, connectionStateListener);
	}

	internal void WithTransportInterceptor(Func<TransportClient, TransportClient> transportClientHandlerFactory)
	{
		if (transportClientHandlerFactory == null)
		{
			throw new ArgumentNullException("transportClientHandlerFactory");
		}
		transportClient = transportClientHandlerFactory(transportClient);
		fallbackTransportClient = transportClientHandlerFactory(fallbackTransportClient);
	}

	public StoreClient CreateStoreClient(IAddressResolver addressResolver, ISessionContainer sessionContainer, IServiceConfigurationReader serviceConfigurationReader, IAuthorizationTokenProvider authorizationTokenProvider, bool enableRequestDiagnostics = false, bool enableReadRequestsFallback = false, bool useFallbackClient = true, bool useMultipleWriteLocations = false, bool detectClientConnectivityIssues = false, bool enableReplicaValidation = false)
	{
		ThrowIfDisposed();
		RetryWithConfiguration retryWithConfiguration;
		if (useFallbackClient && fallbackTransportClient != null)
		{
			DefaultTrace.TraceInformation("Using fallback TransportClient");
			Protocol num = protocol;
			TransportClient obj = fallbackTransportClient;
			bool num2 = disableRetryWithRetryPolicy;
			retryWithConfiguration = this.retryWithConfiguration;
			return new StoreClient(addressResolver, sessionContainer, serviceConfigurationReader, authorizationTokenProvider, num, obj, enableRequestDiagnostics, enableReadRequestsFallback, useMultipleWriteLocations, detectClientConnectivityIssues, num2, enableReplicaValidation, retryWithConfiguration);
		}
		Protocol num3 = protocol;
		TransportClient obj2 = transportClient;
		bool num4 = disableRetryWithRetryPolicy;
		retryWithConfiguration = this.retryWithConfiguration;
		return new StoreClient(addressResolver, sessionContainer, serviceConfigurationReader, authorizationTokenProvider, num3, obj2, enableRequestDiagnostics, enableReadRequestsFallback, useMultipleWriteLocations, detectClientConnectivityIssues, num4, enableReplicaValidation, retryWithConfiguration);
	}

	public void Dispose()
	{
		if (!isDisposed)
		{
			if (transportClient != null)
			{
				transportClient.Dispose();
				transportClient = null;
			}
			if (fallbackTransportClient != null)
			{
				fallbackTransportClient.Dispose();
				fallbackTransportClient = null;
			}
			isDisposed = true;
		}
	}

	private void ThrowIfDisposed()
	{
		if (isDisposed)
		{
			throw new ObjectDisposedException("StoreClientFactory");
		}
	}

	private static void ValidatePortPoolReuseThreshold(ref int rntbdPortPoolReuseThreshold)
	{
		if (rntbdPortPoolReuseThreshold < 32)
		{
			DefaultTrace.TraceWarning("The value of {0} is too small. Received {1}. Adjusting to {2}", "rntbdPortPoolReuseThreshold", rntbdPortPoolReuseThreshold, 32);
			rntbdPortPoolReuseThreshold = 32;
		}
		else if (rntbdPortPoolReuseThreshold > 2048)
		{
			DefaultTrace.TraceWarning("The value of {0} is too large. Received {1}. Adjusting to {2}", "rntbdPortPoolReuseThreshold", rntbdPortPoolReuseThreshold, 2048);
			rntbdPortPoolReuseThreshold = 2048;
		}
	}

	private static void ValidatePortPoolBindAttempts(ref int rntbdPortPoolBindAttempts)
	{
		if (rntbdPortPoolBindAttempts < 3)
		{
			DefaultTrace.TraceWarning("The value of {0} is too small. Received {1}. Adjusting to {2}", "rntbdPortPoolBindAttempts", rntbdPortPoolBindAttempts, 3);
			rntbdPortPoolBindAttempts = 3;
		}
		else if (rntbdPortPoolBindAttempts > 32)
		{
			DefaultTrace.TraceWarning("The value of {0} is too large. Received {1}. Adjusting to {2}", "rntbdPortPoolBindAttempts", rntbdPortPoolBindAttempts, 32);
			rntbdPortPoolBindAttempts = 32;
		}
	}

	private static void ValidateRntbdMaxConcurrentOpeningConnectionCount(ref int rntbdMaxConcurrentOpeningConnectionCount)
	{
		int num = 65535;
		string environmentVariable = Environment.GetEnvironmentVariable("AZURE_COSMOS_TCP_MAX_CONCURRENT_OPENING_CONNECTION_UPPER_LIMIT");
		if (!string.IsNullOrEmpty(environmentVariable))
		{
			int result = 0;
			if (int.TryParse(environmentVariable, out result))
			{
				if (result <= 0)
				{
					throw new ArgumentException("RntbdMaxConcurrentOpeningConnectionUpperLimitConfig should be larger than 0");
				}
				num = result;
			}
		}
		if (rntbdMaxConcurrentOpeningConnectionCount > num)
		{
			DefaultTrace.TraceWarning("The value of {0} is too large. Received {1}. Adjusting to {2}", "rntbdMaxConcurrentOpeningConnectionCount", rntbdMaxConcurrentOpeningConnectionCount, num);
			rntbdMaxConcurrentOpeningConnectionCount = num;
		}
	}
}
