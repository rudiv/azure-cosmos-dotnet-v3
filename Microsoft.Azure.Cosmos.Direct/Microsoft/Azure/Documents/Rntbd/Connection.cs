using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Rntbd;

namespace Microsoft.Azure.Documents.Rntbd;

internal sealed class Connection : IDisposable
{
	public sealed class ResponseMetadata : IDisposable
	{
		private bool disposed;

		private BufferProvider.DisposableBuffer header;

		private BufferProvider.DisposableBuffer metadata;

		public ArraySegment<byte> Header => header.Buffer;

		public ArraySegment<byte> Metadata => metadata.Buffer;

		public ResponseMetadata(BufferProvider.DisposableBuffer header, BufferProvider.DisposableBuffer metadata)
		{
			this.header = header;
			this.metadata = metadata;
			disposed = false;
		}

		public void Dispose()
		{
			if (!disposed)
			{
				header.Dispose();
				metadata.Dispose();
				disposed = true;
			}
		}
	}

	private const int ResponseLengthByteLimit = int.MaxValue;

	private const SslProtocols TlsProtocols = SslProtocols.Tls12;

	private const uint TcpKeepAliveIntervalSocketOptionEnumValue = 17u;

	private const uint TcpKeepAliveTimeSocketOptionEnumValue = 3u;

	private const uint DefaultSocketOptionTcpKeepAliveInterval = 1u;

	private const uint DefaultSocketOptionTcpKeepAliveTime = 30u;

	private static readonly uint SocketOptionTcpKeepAliveInterval = GetUInt32FromEnvironmentVariableOrDefault("AZURE_COSMOS_TCP_KEEPALIVE_INTERVAL_SECONDS", 1u, 100u, 1u);

	private static readonly uint SocketOptionTcpKeepAliveTime = GetUInt32FromEnvironmentVariableOrDefault("AZURE_COSMOS_TCP_KEEPALIVE_TIME_SECONDS", 1u, 100u, 30u);

	private static readonly Lazy<ConcurrentPrng> rng = new Lazy<ConcurrentPrng>(LazyThreadSafetyMode.ExecutionAndPublication);

	private static readonly Lazy<byte[]> keepAliveConfiguration = new Lazy<byte[]>(GetWindowsKeepAliveConfiguration, LazyThreadSafetyMode.ExecutionAndPublication);

	private static readonly Lazy<bool> isKeepAliveCustomizationSupported = new Lazy<bool>(IsKeepAliveCustomizationSupported, LazyThreadSafetyMode.ExecutionAndPublication);

	private static readonly TimeSpan recentReceiveWindow = TimeSpan.FromSeconds(1.0);

	private readonly Guid connectionCorrelationId;

	private readonly Uri serverUri;

	private readonly string hostNameCertificateOverride;

	private readonly MemoryStreamPool memoryStreamPool;

	private readonly RemoteCertificateValidationCallback remoteCertificateValidationCallback;

	private bool disposed;

	private TcpClient tcpClient;

	private UserPortPool portPool;

	private IPEndPoint localEndPoint;

	private IPEndPoint remoteEndPoint;

	private readonly TimeSpan idleConnectionTimeout;

	private readonly TimeSpan idleConnectionClosureTimeout;

	private readonly Func<string, Task<IPAddress>> dnsResolutionFunction;

	private readonly SemaphoreSlim writeSemaphore = new SemaphoreSlim(1);

	private Stream stream;

	private RntbdStreamReader streamReader;

	private readonly object timestampLock = new object();

	private DateTime lastSendAttemptTime;

	private DateTime lastSendTime;

	private DateTime lastReceiveTime;

	private long numberOfSendsSinceLastReceive;

	private DateTime firstSendSinceLastReceive;

	private readonly object nameLock = new object();

	private string name;

	private static int numberOfOpenTcpConnections;

	private readonly ConnectionHealthChecker healthChecker;

	public static int NumberOfOpenTcpConnections => numberOfOpenTcpConnections;

	public BufferProvider BufferProvider { get; }

	public Uri ServerUri => serverUri;

	public bool Healthy
	{
		get
		{
			ThrowIfDisposed();
			if (tcpClient == null)
			{
				return false;
			}
			SnapshotConnectionTimestamps(out var lastSendAttempt, out var lastSend, out var lastReceive, out var dateTime, out var num);
			return healthChecker.IsHealthy(DateTime.UtcNow, lastSendAttempt, lastSend, lastReceive, dateTime, num, tcpClient.Client);
		}
	}

	public bool Disposed => disposed;

	public Guid ConnectionCorrelationId => connectionCorrelationId;

	internal TimeSpan TestIdleConnectionClosureTimeout => idleConnectionClosureTimeout;

	public Connection(Uri serverUri, string hostNameCertificateOverride, TimeSpan receiveHangDetectionTime, TimeSpan sendHangDetectionTime, TimeSpan idleTimeout, MemoryStreamPool memoryStreamPool, RemoteCertificateValidationCallback remoteCertificateValidationCallback, Func<string, Task<IPAddress>> dnsResolutionFunction)
	{
		connectionCorrelationId = Guid.NewGuid();
		this.serverUri = serverUri;
		this.hostNameCertificateOverride = hostNameCertificateOverride;
		BufferProvider = new BufferProvider();
		this.dnsResolutionFunction = dnsResolutionFunction ?? new Func<string, Task<IPAddress>>(ResolveHostIncludingIPv6AddressesAsync);
		lastSendAttemptTime = DateTime.MinValue;
		lastSendTime = DateTime.MinValue;
		lastReceiveTime = DateTime.MinValue;
		if (idleTimeout > TimeSpan.Zero)
		{
			idleConnectionTimeout = idleTimeout;
			idleConnectionClosureTimeout = idleConnectionTimeout + TimeSpan.FromTicks(2 * (sendHangDetectionTime.Ticks + receiveHangDetectionTime.Ticks));
		}
		name = string.Format(CultureInfo.InvariantCulture, "<not connected> -> {0}", this.serverUri);
		this.memoryStreamPool = memoryStreamPool;
		this.remoteCertificateValidationCallback = remoteCertificateValidationCallback;
		healthChecker = new ConnectionHealthChecker(sendHangDetectionTime, receiveHangDetectionTime, idleTimeout);
	}

	public async Task OpenAsync(ChannelOpenArguments args)
	{
		ThrowIfDisposed();
		await OpenSocketAsync(args);
		await NegotiateSslAsync(args);
	}

	public async Task WriteRequestAsync(ChannelCommonArguments args, TransportSerialization.SerializedRequest messagePayload, TransportRequestStats transportRequestStats)
	{
		ThrowIfDisposed();
		if (transportRequestStats != null)
		{
			SnapshotConnectionTimestamps(out var lastSendAttempt, out var lastSend, out var lastReceive, out var _, out var _);
			transportRequestStats.ConnectionLastSendAttemptTime = lastSendAttempt;
			transportRequestStats.ConnectionLastSendTime = lastSend;
			transportRequestStats.ConnectionLastReceiveTime = lastReceive;
		}
		args.SetTimeoutCode(TransportErrorCode.SendLockTimeout);
		await writeSemaphore.WaitAsync();
		try
		{
			args.SetTimeoutCode(TransportErrorCode.SendTimeout);
			args.SetPayloadSent();
			UpdateLastSendAttemptTime();
			await messagePayload.CopyToStreamAsync(stream);
		}
		finally
		{
			writeSemaphore.Release();
		}
		UpdateLastSendTime();
	}

	[SuppressMessage("", "AvoidMultiLineComments", Justification = "Multi line business logic")]
	public async Task<ResponseMetadata> ReadResponseMetadataAsync(ChannelCommonArguments args)
	{
		ThrowIfDisposed();
		Trace.CorrelationManager.ActivityId = args.ActivityId;
		int metadataHeaderLength = 24;
		BufferProvider.DisposableBuffer header = BufferProvider.GetBuffer(metadataHeaderLength);
		await ReadPayloadAsync(header.Buffer.Array, metadataHeaderLength, "header", args);
		uint num = BitConverter.ToUInt32(header.Buffer.Array, 0);
		if (num > int.MaxValue)
		{
			header.Dispose();
			DefaultTrace.TraceCritical("[RNTBD Connection {0}] RNTBD header length says {1} but expected at most {2} bytes. Connection: {3}", connectionCorrelationId, num, int.MaxValue, this);
			throw TransportExceptions.GetInternalServerErrorException(serverUri, string.Format(CultureInfo.CurrentUICulture, RMResources.ServerResponseHeaderTooLargeError, num, this));
		}
		if (num < metadataHeaderLength)
		{
			DefaultTrace.TraceCritical("[RNTBD Connection {0}] Invalid RNTBD header length {1} bytes. Expected at least {2} bytes. Connection: {3}", connectionCorrelationId, num, metadataHeaderLength, this);
			throw TransportExceptions.GetInternalServerErrorException(serverUri, string.Format(CultureInfo.CurrentUICulture, RMResources.ServerResponseInvalidHeaderLengthError, metadataHeaderLength, num, this));
		}
		int num2 = (int)num - metadataHeaderLength;
		BufferProvider.DisposableBuffer metadata = BufferProvider.GetBuffer(num2);
		await ReadPayloadAsync(metadata.Buffer.Array, num2, "metadata", args);
		return new ResponseMetadata(header, metadata);
	}

	public async Task<MemoryStream> ReadResponseBodyAsync(ChannelCommonArguments args)
	{
		ThrowIfDisposed();
		Trace.CorrelationManager.ActivityId = args.ActivityId;
		using BufferProvider.DisposableBuffer bodyLengthHeader = BufferProvider.GetBuffer(4);
		await ReadPayloadAsync(bodyLengthHeader.Buffer.Array, 4, "body length header", args);
		uint num = BitConverter.ToUInt32(bodyLengthHeader.Buffer.Array, 0);
		if (num > int.MaxValue)
		{
			DefaultTrace.TraceCritical("[RNTBD Connection {0}] Invalid RNTBD response body length {1} bytes. Connection: {2}", connectionCorrelationId, num, this);
			throw TransportExceptions.GetInternalServerErrorException(serverUri, string.Format(CultureInfo.CurrentUICulture, RMResources.ServerResponseBodyTooLargeError, num, this));
		}
		MemoryStream memoryStream = null;
		if (memoryStreamPool?.TryGetMemoryStream((int)num, out memoryStream) ?? false)
		{
			await ReadPayloadAsync(memoryStream, (int)num, "body", args);
			memoryStream.Position = 0L;
			return memoryStream;
		}
		byte[] body = new byte[num];
		await ReadPayloadAsync(body, (int)num, "body", args);
		return StreamExtension.CreateExportableMemoryStream(body);
	}

	public override string ToString()
	{
		lock (nameLock)
		{
			return name;
		}
	}

	public void Dispose()
	{
		ThrowIfDisposed();
		disposed = true;
		string connectionTimestampsText = GetConnectionTimestampsText();
		if (tcpClient != null)
		{
			DefaultTrace.TraceInformation("[RNTBD Connection {0}] Disposing RNTBD connection {1} -> {2} to server {3}. {4}", connectionCorrelationId, localEndPoint, remoteEndPoint, serverUri, connectionTimestampsText);
			string text = string.Format(CultureInfo.InvariantCulture, "<disconnected> {0} -> {1}", localEndPoint, remoteEndPoint);
			lock (nameLock)
			{
				name = text;
			}
		}
		else
		{
			DefaultTrace.TraceInformation("[RNTBD Connection {0}] Disposing unused RNTBD connection to server {1}. {2}", connectionCorrelationId, serverUri, connectionTimestampsText);
		}
		if (tcpClient != null)
		{
			if (portPool != null)
			{
				portPool.RemoveReference(localEndPoint.AddressFamily, checked((ushort)localEndPoint.Port));
			}
			tcpClient.Close();
			Interlocked.Decrement(ref numberOfOpenTcpConnections);
			tcpClient = null;
			stream.Close();
			streamReader?.Dispose();
			TransportClient.GetTransportPerformanceCounters().IncrementRntbdConnectionClosedCount();
		}
	}

	public bool IsActive(out TimeSpan timeToIdle)
	{
		ThrowIfDisposed();
		SnapshotConnectionTimestamps(out var _, out var _, out var lastReceive, out var _, out var _);
		DateTime utcNow = DateTime.UtcNow;
		if (utcNow - lastReceive > idleConnectionTimeout)
		{
			timeToIdle = idleConnectionClosureTimeout;
			return false;
		}
		timeToIdle = lastReceive + idleConnectionClosureTimeout - utcNow;
		return true;
	}

	internal void NotifyConnectionStatus(bool isCompleted, bool isReadRequest = false)
	{
		healthChecker.UpdateTransitTimeoutCounters(isCompleted, isReadRequest);
	}

	internal void TestSetLastReceiveTime(DateTime lrt)
	{
		lock (timestampLock)
		{
			lastReceiveTime = lrt;
		}
	}

	private static uint GetUInt32FromEnvironmentVariableOrDefault(string name, uint minValue, uint maxValue, uint defaultValue)
	{
		string environmentVariable = Environment.GetEnvironmentVariable(name);
		if (string.IsNullOrEmpty(environmentVariable) || !uint.TryParse(environmentVariable, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
		{
			return defaultValue;
		}
		if (result > maxValue || result < minValue)
		{
			throw new ArgumentOutOfRangeException(name, $"Value for environment variable '{name}' is outside expected range of {minValue} - {maxValue}.");
		}
		return result;
	}

	private void ThrowIfDisposed()
	{
		if (disposed)
		{
			throw new ObjectDisposedException(string.Format("{0}:{1}", "Connection", serverUri));
		}
	}

	private async Task OpenSocketAsync(ChannelOpenArguments args)
	{
		if (this.tcpClient != null)
		{
			throw new InvalidOperationException("Attempting to call Connection.OpenSocketAsync on an " + $"already initialized connection {this}");
		}
		TcpClient tcpClient = null;
		TransportErrorCode errorCode = TransportErrorCode.Unknown;
		try
		{
			errorCode = TransportErrorCode.DnsResolutionFailed;
			args.CommonArguments.SetTimeoutCode(TransportErrorCode.DnsResolutionTimeout);
			IPAddress iPAddress = await dnsResolutionFunction(serverUri.DnsSafeHost);
			errorCode = TransportErrorCode.ConnectFailed;
			args.CommonArguments.SetTimeoutCode(TransportErrorCode.ConnectTimeout);
			UpdateLastSendAttemptTime();
			DefaultTrace.TraceInformation("[RNTBD Connection {0}] Port reuse mode: {1}. Connection: {2}", connectionCorrelationId, args.PortReuseMode, this);
			switch (args.PortReuseMode)
			{
			case PortReuseMode.ReuseUnicastPort:
				tcpClient = await ConnectUnicastPortAsync(serverUri, iPAddress, connectionCorrelationId);
				break;
			case PortReuseMode.PrivatePortPool:
			{
				Tuple<TcpClient, bool> tuple = await ConnectUserPortAsync(serverUri, iPAddress, args.PortPool, ToString(), connectionCorrelationId);
				tcpClient = tuple.Item1;
				if (tuple.Item2)
				{
					portPool = args.PortPool;
					break;
				}
				DefaultTrace.TraceInformation("[RNTBD Connection {0}] PrivatePortPool: Configured but actually not used. Connection: {1}", connectionCorrelationId, this);
				break;
			}
			default:
				throw new ArgumentException($"Unsupported port reuse policy {args.PortReuseMode.ToString()}");
			}
			UpdateLastSendTime();
			UpdateLastReceiveTime();
			args.OpenTimeline.RecordConnectFinishTime();
			DefaultTrace.TraceInformation("[RNTBD Connection {0}] RNTBD connection established {1} -> {2}", connectionCorrelationId, tcpClient.Client.LocalEndPoint, tcpClient.Client.RemoteEndPoint);
			TransportClient.GetTransportPerformanceCounters().IncrementRntbdConnectionEstablishedCount();
			string text = string.Format(CultureInfo.InvariantCulture, "{0} -> {1}", tcpClient.Client.LocalEndPoint, tcpClient.Client.RemoteEndPoint);
			lock (nameLock)
			{
				name = text;
			}
		}
		catch (Exception ex)
		{
			tcpClient?.Close();
			DefaultTrace.TraceInformation("[RNTBD Connection {0}] Connection.OpenSocketAsync failed. Converting to TransportException. Connection: {1}. Inner exception: {2}", connectionCorrelationId, this, ex);
			throw new TransportException(errorCode, ex, args.CommonArguments.ActivityId, serverUri, ToString(), args.CommonArguments.UserPayload, args.CommonArguments.PayloadSent);
		}
		localEndPoint = (IPEndPoint)tcpClient.Client.LocalEndPoint;
		remoteEndPoint = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
		this.tcpClient = tcpClient;
		stream = tcpClient.GetStream();
		Interlocked.Increment(ref numberOfOpenTcpConnections);
		this.tcpClient.Client.Blocking = false;
	}

	private async Task NegotiateSslAsync(ChannelOpenArguments args)
	{
		string targetHost = hostNameCertificateOverride ?? serverUri.DnsSafeHost;
		SslStream sslStream = new SslStream(stream, leaveInnerStreamOpen: false, remoteCertificateValidationCallback);
		try
		{
			args.CommonArguments.SetTimeoutCode(TransportErrorCode.SslNegotiationTimeout);
			UpdateLastSendAttemptTime();
			await sslStream.AuthenticateAsClientAsync(targetHost, null, SslProtocols.Tls12, checkCertificateRevocation: false);
			UpdateLastSendTime();
			UpdateLastReceiveTime();
			args.OpenTimeline.RecordSslHandshakeFinishTime();
			stream = sslStream;
			streamReader = new RntbdStreamReader(stream);
			DefaultTrace.TraceInformation("[RNTBD Connection {0}] SSL handshake complete {1} -> {2}", connectionCorrelationId, localEndPoint, remoteEndPoint);
		}
		catch (Exception ex)
		{
			DefaultTrace.TraceInformation("[RNTBD Connection {0}] Connection.NegotiateSslAsync failed. Converting to TransportException. Connection: {1}. Inner exception: {2}", connectionCorrelationId, this, ex);
			throw new TransportException(TransportErrorCode.SslNegotiationFailed, ex, args.CommonArguments.ActivityId, serverUri, ToString(), args.CommonArguments.UserPayload, args.CommonArguments.PayloadSent);
		}
	}

	private async Task ReadPayloadAsync(byte[] payload, int length, string type, ChannelCommonArguments args)
	{
		int read;
		for (int bytesRead = 0; bytesRead < length; bytesRead += read)
		{
			read = 0;
			try
			{
				read = await streamReader.ReadAsync(payload, bytesRead, length - bytesRead);
			}
			catch (IOException e)
			{
				TraceAndThrowReceiveFailedException(e, type, args);
			}
			if (read == 0)
			{
				TraceAndThrowEndOfStream(type, args);
			}
			UpdateLastReceiveTime();
		}
	}

	private async Task ReadPayloadAsync(MemoryStream payload, int length, string type, ChannelCommonArguments args)
	{
		int read;
		for (int bytesRead = 0; bytesRead < length; bytesRead += read)
		{
			read = 0;
			try
			{
				read = await streamReader.ReadAsync(payload, length - bytesRead);
			}
			catch (IOException e)
			{
				TraceAndThrowReceiveFailedException(e, type, args);
			}
			if (read == 0)
			{
				TraceAndThrowEndOfStream(type, args);
			}
			UpdateLastReceiveTime();
		}
	}

	private void TraceAndThrowReceiveFailedException(IOException e, string type, ChannelCommonArguments args)
	{
		DefaultTrace.TraceError("[RNTBD Connection {0}] Hit IOException {1} with HResult {2} while reading {3} on connection {4}. {5}", connectionCorrelationId, e.Message, e.HResult, type, this, GetConnectionTimestampsText());
		throw new TransportException(TransportErrorCode.ReceiveFailed, e, args.ActivityId, serverUri, ToString(), args.UserPayload, payloadSent: true);
	}

	private void TraceAndThrowEndOfStream(string type, ChannelCommonArguments args)
	{
		DefaultTrace.TraceError("[RNTBD Connection {0}] Reached end of stream. Read 0 bytes while reading {1} on connection {2}. {3}", connectionCorrelationId, type, this, GetConnectionTimestampsText());
		throw new TransportException(TransportErrorCode.ReceiveStreamClosed, null, args.ActivityId, serverUri, ToString(), args.UserPayload, payloadSent: true);
	}

	private void SnapshotConnectionTimestamps(out DateTime lastSendAttempt, out DateTime lastSend, out DateTime lastReceive, out DateTime? firstSendSinceLastReceive, out long numberOfSendsSinceLastReceive)
	{
		lock (timestampLock)
		{
			lastSendAttempt = lastSendAttemptTime;
			lastSend = lastSendTime;
			lastReceive = lastReceiveTime;
			firstSendSinceLastReceive = ((lastReceiveTime < this.firstSendSinceLastReceive) ? new DateTime?(this.firstSendSinceLastReceive) : null);
			numberOfSendsSinceLastReceive = this.numberOfSendsSinceLastReceive;
		}
	}

	private string GetConnectionTimestampsText()
	{
		SnapshotConnectionTimestamps(out var lastSendAttempt, out var lastSend, out var lastReceive, out var dateTime, out var num);
		return string.Format(CultureInfo.InvariantCulture, "Last send attempt time: {0:o}. Last send time: {1:o}. Last receive time: {2:o}. First sends since last receieve: {3:o}. # of sends since last receive: {4}", lastSendAttempt, lastSend, lastReceive, dateTime, num);
	}

	private void UpdateLastSendAttemptTime()
	{
		lock (timestampLock)
		{
			lastSendAttemptTime = DateTime.UtcNow;
		}
	}

	private void UpdateLastSendTime()
	{
		lock (timestampLock)
		{
			lastSendTime = DateTime.UtcNow;
			if (numberOfSendsSinceLastReceive++ == 0L)
			{
				firstSendSinceLastReceive = lastSendTime;
			}
		}
	}

	private void UpdateLastReceiveTime()
	{
		lock (timestampLock)
		{
			numberOfSendsSinceLastReceive = 0L;
			lastReceiveTime = DateTime.UtcNow;
		}
	}

	private static async Task<TcpClient> ConnectUnicastPortAsync(Uri serverUri, IPAddress resolvedAddress, Guid connectionCorrelationId)
	{
		TcpClient tcpClient = new TcpClient(resolvedAddress.AddressFamily);
		SetCommonSocketOptions(tcpClient.Client, connectionCorrelationId);
		SetReuseUnicastPort(tcpClient.Client, connectionCorrelationId);
		DefaultTrace.TraceInformation("[RNTBD Connection {0}] {1} connecting to {2} (address {3})", connectionCorrelationId, "ConnectUnicastPortAsync", serverUri, resolvedAddress);
		await tcpClient.ConnectAsync(resolvedAddress, serverUri.Port);
		return tcpClient;
	}

	private static async Task<Tuple<TcpClient, bool>> ConnectReuseAddrAsync(Uri serverUri, IPAddress address, ushort candidatePort, Guid connectionCorrelationId)
	{
		TcpClient candidateClient = new TcpClient(address.AddressFamily);
		TcpClient item = null;
		try
		{
			SetCommonSocketOptions(candidateClient.Client, connectionCorrelationId);
			candidateClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, optionValue: true);
			EndPoint endPoint = address.AddressFamily switch
			{
				AddressFamily.InterNetwork => new IPEndPoint(IPAddress.Any, candidatePort), 
				AddressFamily.InterNetworkV6 => new IPEndPoint(IPAddress.IPv6Any, candidatePort), 
				_ => throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Address family {0} not supported", address.AddressFamily)), 
			};
			DefaultTrace.TraceInformation("[RNTBD Connection {0}] RNTBD: {1} binding local endpoint {2}", connectionCorrelationId, "ConnectReuseAddrAsync", endPoint);
			try
			{
				candidateClient.Client.Bind(endPoint);
			}
			catch (SocketException ex)
			{
				if (ex.SocketErrorCode == SocketError.AccessDenied)
				{
					return Tuple.Create<TcpClient, bool>(null, item2: false);
				}
				throw;
			}
			DefaultTrace.TraceInformation("[RNTBD Connection {0}] {1} connecting to {2} (address {3})", connectionCorrelationId, "ConnectReuseAddrAsync", serverUri, address);
			try
			{
				await candidateClient.ConnectAsync(address, serverUri.Port);
			}
			catch (SocketException ex2)
			{
				if (ex2.SocketErrorCode == SocketError.AddressAlreadyInUse)
				{
					return Tuple.Create<TcpClient, bool>(null, item2: true);
				}
				throw;
			}
			item = candidateClient;
			candidateClient = null;
		}
		finally
		{
			candidateClient?.Close();
		}
		return Tuple.Create(item, item2: true);
	}

	private static async Task<Tuple<TcpClient, bool>> ConnectUserPortAsync(Uri serverUri, IPAddress address, UserPortPool portPool, string connectionName, Guid connectionCorrelationId)
	{
		ushort[] candidatePorts = portPool.GetCandidatePorts(address.AddressFamily);
		checked
		{
			if (candidatePorts != null)
			{
				ushort[] array = candidatePorts;
				foreach (ushort candidatePort in array)
				{
					Tuple<TcpClient, bool> obj = await ConnectReuseAddrAsync(serverUri, address, candidatePort, connectionCorrelationId);
					TcpClient item = obj.Item1;
					bool item2 = obj.Item2;
					if (item != null)
					{
						ushort port = (ushort)((IPEndPoint)item.Client.LocalEndPoint).Port;
						portPool.AddReference(address.AddressFamily, port);
						return Tuple.Create(item, item2: true);
					}
					if (!item2)
					{
						portPool.MarkUnusable(address.AddressFamily, candidatePort);
					}
				}
				DefaultTrace.TraceInformation("[RNTBD Connection {0}] PrivatePortPool: All {1} candidate ports have been tried but none connects. Connection: {2}", connectionCorrelationId, candidatePorts.Length, connectionName);
			}
			TcpClient item3 = (await ConnectReuseAddrAsync(serverUri, address, 0, connectionCorrelationId)).Item1;
			if (item3 != null)
			{
				portPool.AddReference(address.AddressFamily, (ushort)((IPEndPoint)item3.Client.LocalEndPoint).Port);
				return Tuple.Create(item3, item2: true);
			}
			DefaultTrace.TraceInformation("[RNTBD Connection {0}] PrivatePortPool: Not enough reusable ports in the system or pool. Have to connect unicast port. Pool status: {1}. Connection: {2}", connectionCorrelationId, portPool.DumpStatus(), connectionName);
			return Tuple.Create(await ConnectUnicastPortAsync(serverUri, address, connectionCorrelationId), item2: false);
		}
	}

	private static Task<IPAddress> ResolveHostIncludingIPv6AddressesAsync(string hostName)
	{
		return ResolveHostAsync(hostName, includeIPv6Addresses: true);
	}

	internal static async Task<IPAddress> ResolveHostAsync(string hostName, bool includeIPv6Addresses)
	{
		IPAddress[] array = await Dns.GetHostAddressesAsync(hostName);
		int num = array.Length;
		if (!includeIPv6Addresses)
		{
			num = 0;
			IPAddress[] array2 = array;
			foreach (IPAddress iPAddress in array2)
			{
				if (iPAddress.AddressFamily == AddressFamily.InterNetwork)
				{
					array[num++] = iPAddress;
				}
			}
		}
		int num2 = 0;
		if (num > 1)
		{
			num2 = rng.Value.Next(num);
		}
		if (num == 0)
		{
			throw new ArgumentOutOfRangeException($"DNS Resolve resulted in no internet addresses. includeIPv6Addresses: {includeIPv6Addresses}");
		}
		return array[num2];
	}

	private static void SetCommonSocketOptions(Socket clientSocket, Guid connectionCorrelationId)
	{
		clientSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug, optionValue: true);
		EnableTcpKeepAlive(clientSocket, connectionCorrelationId);
	}

	private static void EnableTcpKeepAlive(Socket clientSocket, Guid connectionCorrelationId)
	{
		clientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, optionValue: true);
		if (Environment.OSVersion.Platform == PlatformID.Win32NT)
		{
			try
			{
				clientSocket.IOControl(IOControlCode.KeepAliveValues, keepAliveConfiguration.Value, null);
				return;
			}
			catch (Exception ex)
			{
				DefaultTrace.TraceWarning("[RNTBD Connection {0}] IOControl(KeepAliveValues) failed: {1}", connectionCorrelationId, ex);
				return;
			}
		}
		SetKeepAliveSocketOptions(clientSocket);
	}

	private static void SetKeepAliveSocketOptions(Socket clientSocket)
	{
		if (isKeepAliveCustomizationSupported.Value)
		{
			clientSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.BlockSource, SocketOptionTcpKeepAliveInterval);
			clientSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TypeOfService, SocketOptionTcpKeepAliveTime);
		}
	}

	private static bool IsKeepAliveCustomizationSupported()
	{
		try
		{
			using Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
			socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.BlockSource, SocketOptionTcpKeepAliveInterval);
			socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TypeOfService, SocketOptionTcpKeepAliveTime);
			return true;
		}
		catch
		{
			return false;
		}
	}

	private static byte[] GetWindowsKeepAliveConfiguration()
	{
		byte[] array = new byte[12];
		BitConverter.GetBytes(1u).CopyTo(array, 0);
		BitConverter.GetBytes(30000u).CopyTo(array, 4);
		BitConverter.GetBytes(1000u).CopyTo(array, 8);
		return array;
	}

	private static void SetReuseUnicastPort(Socket clientSocket, Guid connectionCorrelationId)
	{
		if (Environment.OSVersion.Platform == PlatformID.Win32NT)
		{
			try
			{
				clientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseUnicastPort, optionValue: true);
			}
			catch (Exception ex)
			{
				DefaultTrace.TraceWarning("[RNTBD Connection {0}] SetSocketOption(Socket, ReuseUnicastPort) failed: {1}", connectionCorrelationId, ex);
			}
		}
	}
}
