using System;
using System.Net;
using System.Net.Security;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Rntbd;

internal sealed class ChannelProperties
{
	public UserAgentContainer UserAgent { get; private set; }

	public string CertificateHostNameOverride { get; private set; }

	public IConnectionStateListener ConnectionStateListener { get; private set; }

	public TimerPool RequestTimerPool { get; private set; }

	public TimerPool IdleTimerPool { get; private set; }

	public TimeSpan RequestTimeout { get; private set; }

	public TimeSpan OpenTimeout { get; private set; }

	public TimeSpan LocalRegionOpenTimeout { get; private set; }

	public PortReuseMode PortReuseMode { get; private set; }

	public int MaxChannels { get; private set; }

	public int PartitionCount { get; private set; }

	public int MaxRequestsPerChannel { get; private set; }

	public TimeSpan ReceiveHangDetectionTime { get; private set; }

	public TimeSpan SendHangDetectionTime { get; private set; }

	public TimeSpan IdleTimeout { get; private set; }

	public UserPortPool UserPortPool { get; private set; }

	public RntbdConstants.CallerId CallerId { get; private set; }

	public bool EnableChannelMultiplexing { get; private set; }

	public int MaxConcurrentOpeningConnectionCount { get; private set; }

	public MemoryStreamPool MemoryStreamPool { get; private set; }

	public RemoteCertificateValidationCallback RemoteCertificateValidationCallback { get; private set; }

	public Func<string, Task<IPAddress>> DnsResolutionFunction { get; private set; }

	public ChannelProperties(UserAgentContainer userAgent, string certificateHostNameOverride, IConnectionStateListener connectionStateListener, TimerPool requestTimerPool, TimeSpan requestTimeout, TimeSpan openTimeout, TimeSpan localRegionOpenTimeout, PortReuseMode portReuseMode, UserPortPool userPortPool, int maxChannels, int partitionCount, int maxRequestsPerChannel, int maxConcurrentOpeningConnectionCount, TimeSpan receiveHangDetectionTime, TimeSpan sendHangDetectionTime, TimeSpan idleTimeout, TimerPool idleTimerPool, RntbdConstants.CallerId callerId, bool enableChannelMultiplexing, MemoryStreamPool memoryStreamPool, RemoteCertificateValidationCallback remoteCertificateValidationCallback, Func<string, Task<IPAddress>> dnsResolutionFunction)
	{
		UserAgent = userAgent;
		CertificateHostNameOverride = certificateHostNameOverride;
		ConnectionStateListener = connectionStateListener;
		RequestTimerPool = requestTimerPool;
		RequestTimeout = requestTimeout;
		OpenTimeout = openTimeout;
		LocalRegionOpenTimeout = localRegionOpenTimeout;
		PortReuseMode = portReuseMode;
		UserPortPool = userPortPool;
		MaxChannels = maxChannels;
		PartitionCount = partitionCount;
		MaxRequestsPerChannel = maxRequestsPerChannel;
		ReceiveHangDetectionTime = receiveHangDetectionTime;
		SendHangDetectionTime = sendHangDetectionTime;
		IdleTimeout = idleTimeout;
		IdleTimerPool = idleTimerPool;
		CallerId = callerId;
		EnableChannelMultiplexing = enableChannelMultiplexing;
		MaxConcurrentOpeningConnectionCount = maxConcurrentOpeningConnectionCount;
		MemoryStreamPool = memoryStreamPool;
		RemoteCertificateValidationCallback = remoteCertificateValidationCallback;
		DnsResolutionFunction = dnsResolutionFunction;
	}
}
