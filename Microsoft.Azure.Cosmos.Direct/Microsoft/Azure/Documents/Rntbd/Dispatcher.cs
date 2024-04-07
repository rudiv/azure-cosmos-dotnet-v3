using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Rntbd;
using Microsoft.Azure.Documents.FaultInjection;

namespace Microsoft.Azure.Documents.Rntbd;

using System.Text.Json;

internal sealed class Dispatcher : IDisposable
{
	public sealed class PrepareCallResult : IDisposable
	{
		private bool disposed;

		public uint RequestId { get; private set; }

		public TransportSerialization.SerializedRequest SerializedRequest { get; }

		public Uri Uri { get; private set; }

		public PrepareCallResult(uint requestId, Uri uri, TransportSerialization.SerializedRequest serializedRequest)
		{
			RequestId = requestId;
			Uri = uri;
			SerializedRequest = serializedRequest;
		}

		public void Dispose()
		{
			if (!disposed)
			{
				SerializedRequest.Dispose();
				disposed = true;
			}
		}
	}

	internal sealed class CallInfo : IDisposable
	{
		private enum State
		{
			New,
			Sent,
			SendFailed
		}

		private readonly TaskCompletionSource<StoreResponse> completion = new TaskCompletionSource<StoreResponse>();

		private readonly SemaphoreSlim sendComplete = new SemaphoreSlim(0);

		private readonly Guid connectionCorrelationId;

		private readonly Guid activityId;

		private readonly Uri uri;

		private readonly TaskScheduler scheduler;

		private bool disposed;

		private readonly object stateLock = new object();

		private State state;

		public TransportRequestStats TransportRequestStats { get; }

		public CallInfo(Guid connectionCorrelationId, Guid activityId, Uri uri, TaskScheduler scheduler, TransportRequestStats transportRequestStats)
		{
			this.connectionCorrelationId = connectionCorrelationId;
			this.activityId = activityId;
			this.uri = uri;
			this.scheduler = scheduler;
			TransportRequestStats = transportRequestStats;
		}

		public Task<StoreResponse> ReadResponseAsync(ChannelCallArguments args)
		{
			ThrowIfDisposed();
			CompleteSend(State.Sent);
			args.CommonArguments.SetTimeoutCode(TransportErrorCode.ReceiveTimeout);
			return completion.Task;
		}

		public void SendFailed()
		{
			ThrowIfDisposed();
			CompleteSend(State.SendFailed);
		}

		public void SetResponse(Connection.ResponseMetadata responseMd, TransportSerialization.RntbdHeader responseHeader, MemoryStream responseBody, string serverVersion, byte[] metadata, int metadataLength)
		{
			ThrowIfDisposed();
			RunAsynchronously(delegate
			{
				Trace.CorrelationManager.ActivityId = activityId;
				try
				{
					BytesDeserializer rntbdHeaderReader = new BytesDeserializer(metadata, metadataLength);
					StoreResponse result = TransportSerialization.MakeStoreResponse(responseHeader.Status, responseHeader.ActivityId, responseBody, serverVersion, ref rntbdHeaderReader);
					completion.SetResult(result);
				}
				catch (Exception exception)
				{
					completion.SetException(exception);
					responseBody?.Dispose();
				}
				finally
				{
					responseMd.Dispose();
				}
			});
		}

		public void SetConnectionBrokenException(Exception inner, string sourceDescription)
		{
			ThrowIfDisposed();
			RunAsynchronously(async delegate
			{
				Trace.CorrelationManager.ActivityId = activityId;
				await sendComplete.WaitAsync();
				lock (stateLock)
				{
					if (state != State.Sent)
					{
						return;
					}
				}
				completion.SetException(new TransportException(TransportErrorCode.ConnectionBroken, inner, activityId, uri, sourceDescription, userPayload: true, payloadSent: true));
			});
		}

		public void Cancel()
		{
			ThrowIfDisposed();
			RunAsynchronously(delegate
			{
				Trace.CorrelationManager.ActivityId = activityId;
				completion.SetCanceled();
			});
		}

		public void Dispose()
		{
			ThrowIfDisposed();
			disposed = true;
			sendComplete.Dispose();
		}

		private void ThrowIfDisposed()
		{
			if (disposed)
			{
				throw new ObjectDisposedException("CallInfo");
			}
		}

		private void RunAsynchronously(Action action)
		{
			Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.DenyChildAttach, scheduler).ContinueWith(delegate(Task failedTask, object connectionIdObject)
			{
				DefaultTrace.TraceError("[RNTBD Dispatcher.CallInfo {0}] Unexpected: Rntbd asynchronous completion call failed. Consuming the task exception asynchronously. Exception: {1}", connectionIdObject, failedTask.Exception?.InnerException);
			}, connectionCorrelationId, TaskContinuationOptions.OnlyOnFaulted);
		}

		private void RunAsynchronously(Func<Task> action)
		{
			Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.DenyChildAttach, scheduler).Unwrap().ContinueWith(delegate(Task failedTask, object connectionIdObject)
			{
				DefaultTrace.TraceError("[RNTBD Dispatcher.CallInfo {0}] Unexpected: Rntbd asynchronous completion call failed. Consuming the task exception asynchronously. Exception: {1}", connectionIdObject, failedTask.Exception?.InnerException);
			}, connectionCorrelationId, TaskContinuationOptions.OnlyOnFaulted);
		}

		private void CompleteSend(State newState)
		{
			lock (stateLock)
			{
				if (state != 0)
				{
					throw new InvalidOperationException("Send may only complete once");
				}
				state = newState;
				sendComplete.Release();
			}
		}
	}

	private readonly Connection connection;

	private readonly UserAgentContainer userAgent;

	private readonly Uri serverUri;

	private readonly IConnectionStateListener connectionStateListener;

	private readonly CancellationTokenSource cancellation = new CancellationTokenSource();

	private readonly TimerPool idleTimerPool;

	private readonly bool enableChannelMultiplexing;

	private bool disposed;

	private ServerProperties serverProperties;

	private int nextRequestId;

	private readonly object callLock = new object();

	private Task receiveTask;

	private readonly Dictionary<uint, CallInfo> calls = new Dictionary<uint, CallInfo>();

	private bool callsAllowed = true;

	private readonly object connectionLock = new object();

	private PooledTimer idleTimer;

	private Task idleTimerTask;

	private readonly IChaosInterceptor chaosInterceptor;

	private TransportException faultInjectionTransportException;

	private bool isFaultInjectionedConnectionError;

	public Guid ConnectionCorrelationId => connection.ConnectionCorrelationId;

	internal bool TestIsIdle
	{
		get
		{
			lock (connectionLock)
			{
				if (connection.Disposed)
				{
					return true;
				}
				TimeSpan timeToIdle;
				return !connection.IsActive(out timeToIdle);
			}
		}
	}

	public bool Healthy
	{
		get
		{
			ThrowIfDisposed();
			if (cancellation.IsCancellationRequested)
			{
				return false;
			}
			lock (callLock)
			{
				if (!callsAllowed)
				{
					return false;
				}
			}
			bool flag;
			try
			{
				flag = connection.Healthy;
			}
			catch (ObjectDisposedException)
			{
				DefaultTrace.TraceWarning("[RNTBD Dispatcher {0}] {1} ObjectDisposedException from Connection.Healthy", ConnectionCorrelationId, this);
				flag = false;
			}
			if (flag)
			{
				return true;
			}
			lock (callLock)
			{
				callsAllowed = false;
			}
			return false;
		}
	}

	internal event Action TestOnConnectionClosed;

	public Dispatcher(Uri serverUri, UserAgentContainer userAgent, IConnectionStateListener connectionStateListener, string hostNameCertificateOverride, TimeSpan receiveHangDetectionTime, TimeSpan sendHangDetectionTime, TimerPool idleTimerPool, TimeSpan idleTimeout, bool enableChannelMultiplexing, MemoryStreamPool memoryStreamPool, RemoteCertificateValidationCallback remoteCertificateValidationCallback, Func<string, Task<IPAddress>> dnsResolutionFunction, IChaosInterceptor chaosInterceptor)
	{
		connection = new Connection(serverUri, hostNameCertificateOverride, receiveHangDetectionTime, sendHangDetectionTime, idleTimeout, memoryStreamPool, remoteCertificateValidationCallback, dnsResolutionFunction);
		this.userAgent = userAgent;
		this.connectionStateListener = connectionStateListener;
		this.serverUri = serverUri;
		this.idleTimerPool = idleTimerPool;
		this.enableChannelMultiplexing = enableChannelMultiplexing;
		this.chaosInterceptor = chaosInterceptor;
	}

	public async Task OpenAsync(ChannelOpenArguments args)
	{
		ThrowIfDisposed();
		try
		{
			await connection.OpenAsync(args);
			await NegotiateRntbdContextAsync(args);
			lock (callLock)
			{
				receiveTask = Task.Factory.StartNew((Func<Task>)async delegate
				{
					await ReceiveLoopAsync();
				}, cancellation.Token, TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach, TaskScheduler.Default).Unwrap();
				receiveTask.ContinueWith(delegate(Task completedTask)
				{
					DefaultTrace.TraceWarning("[RNTBD Dispatcher {0}] Dispatcher.ReceiveLoopAsync failed. Consuming the task exception asynchronously. Dispatcher: {1}. Exception: {2}", ConnectionCorrelationId, this, completedTask.Exception?.InnerException);
				}, default(CancellationToken), TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default);
			}
			if (idleTimerPool != null)
			{
				StartIdleTimer();
			}
		}
		catch (DocumentClientException)
		{
			DisallowInitialCalls();
			throw;
		}
		catch (TransportException)
		{
			DisallowInitialCalls();
			throw;
		}
	}

	public void InjectFaultInjectionConnectionError(TransportException transportException)
	{
		if (!disposed)
		{
			isFaultInjectionedConnectionError = true;
			faultInjectionTransportException = transportException;
		}
	}

	public PrepareCallResult PrepareCall(DocumentServiceRequest request, TransportAddressUri physicalAddress, ResourceOperation resourceOperation, Guid activityId, TransportRequestStats transportRequestStats)
	{
		uint requestId = (uint)Interlocked.Increment(ref nextRequestId);
		string transportRequestIDOverride = requestId.ToString(CultureInfo.InvariantCulture);
		int headerSize;
		int? bodySize;
		TransportSerialization.SerializedRequest serializedRequest = TransportSerialization.BuildRequest(request, physicalAddress.PathAndQuery, resourceOperation, activityId, connection.BufferProvider, transportRequestIDOverride, out headerSize, out bodySize);
		transportRequestStats.RequestBodySizeInBytes = bodySize;
		transportRequestStats.RequestSizeInBytes = serializedRequest.RequestSize;
		return new PrepareCallResult(requestId, physicalAddress.Uri, serializedRequest);
	}

	public async Task<StoreResponse> CallAsync(ChannelCallArguments args, TransportRequestStats transportRequestStats)
	{
		ThrowIfDisposed();
		using CallInfo callInfo = new CallInfo(ConnectionCorrelationId, args.CommonArguments.ActivityId, args.PreparedCall.Uri, TaskScheduler.Current, transportRequestStats);
		uint requestId = args.PreparedCall.RequestId;
		lock (callLock)
		{
			transportRequestStats.NumberOfInflightRequestsInConnection = calls.Count;
			if (!callsAllowed)
			{
				throw new TransportException(TransportErrorCode.ChannelMultiplexerClosed, null, args.CommonArguments.ActivityId, args.PreparedCall.Uri, ToString(), args.CommonArguments.UserPayload, args.CommonArguments.PayloadSent);
			}
			calls.Add(requestId, callInfo);
		}
		try
		{
			try
			{
				if (chaosInterceptor != null)
				{
					await chaosInterceptor.OnBeforeConnectionWriteAsync(args);
					var (flag, result) = await chaosInterceptor.OnRequestCallAsync(args);
					if (flag)
					{
						transportRequestStats.RecordState(TransportRequestStats.RequestStage.Sent);
						return result;
					}
				}
				await connection.WriteRequestAsync(args.CommonArguments, args.PreparedCall.SerializedRequest, transportRequestStats);
				transportRequestStats.RecordState(TransportRequestStats.RequestStage.Sent);
				if (chaosInterceptor != null)
				{
					await chaosInterceptor.OnAfterConnectionWriteAsync(args);
				}
			}
			catch (Exception innerException)
			{
				callInfo.SendFailed();
				throw new TransportException(TransportErrorCode.SendFailed, innerException, args.CommonArguments.ActivityId, args.PreparedCall.Uri, ToString(), args.CommonArguments.UserPayload, args.CommonArguments.PayloadSent);
			}
			return await callInfo.ReadResponseAsync(args);
		}
		catch (DocumentClientException)
		{
			DisallowRuntimeCalls();
			throw;
		}
		catch (TransportException)
		{
			DisallowRuntimeCalls();
			throw;
		}
		finally
		{
			RemoveCall(requestId);
		}
	}

	public void CancelCallAndNotifyConnectionOnTimeoutEvent(PrepareCallResult preparedCall, bool isReadOnly)
	{
		ThrowIfDisposed();
		connection.NotifyConnectionStatus(isCompleted: true, isReadOnly);
		RemoveCall(preparedCall.RequestId)?.Cancel();
	}

	public void NotifyConnectionOnSuccessEvent()
	{
		ThrowIfDisposed();
		connection.NotifyConnectionStatus(isCompleted: true);
	}

	public override string ToString()
	{
		return connection.ToString();
	}

	public void Dispose()
	{
		ThrowIfDisposed();
		disposed = true;
		DefaultTrace.TraceInformation("[RNTBD Dispatcher {0}] Disposing RNTBD Dispatcher {1}", ConnectionCorrelationId, this);
		Task t = null;
		lock (connectionLock)
		{
			StartConnectionShutdown();
			t = StopIdleTimer();
		}
		WaitTask(t, "idle timer");
		Task t2 = null;
		lock (connectionLock)
		{
			t2 = CloseConnection();
		}
		WaitTask(t2, "receive loop");
		DefaultTrace.TraceInformation("[RNTBD Dispatcher {0}] RNTBD Dispatcher {1} is disposed", ConnectionCorrelationId, this);
	}

	private void StartIdleTimer()
	{
		DefaultTrace.TraceInformation("[RNTBD Dispatcher {0}] RNTBD idle connection monitor: Timer is starting...", ConnectionCorrelationId);
		TimeSpan timeToIdle = TimeSpan.MinValue;
		bool flag = false;
		try
		{
			lock (connectionLock)
			{
				if (!connection.IsActive(out timeToIdle))
				{
					DefaultTrace.TraceCritical("[RNTBD Dispatcher {0}][{1}] New connection already idle.", ConnectionCorrelationId, this);
				}
				else
				{
					ScheduleIdleTimer(timeToIdle);
					flag = true;
				}
			}
		}
		finally
		{
			if (flag)
			{
				DefaultTrace.TraceInformation("[RNTBD Dispatcher {0}] RNTBD idle connection monitor {1}: Timer is scheduled to fire {2} seconds later at {3}.", ConnectionCorrelationId, this, timeToIdle.TotalSeconds, DateTime.UtcNow + timeToIdle);
			}
			else
			{
				DefaultTrace.TraceInformation("[RNTBD Dispatcher {0}] RNTBD idle connection monitor {1}: Timer is not scheduled.", ConnectionCorrelationId, this);
			}
		}
	}

	private void OnIdleTimer(Task precedentTask)
	{
		Task t = null;
		lock (connectionLock)
		{
			if (cancellation.IsCancellationRequested)
			{
				return;
			}
			TimeSpan timeToIdle;
			bool flag = connection.IsActive(out timeToIdle);
			if (flag)
			{
				ScheduleIdleTimer(timeToIdle);
				return;
			}
			lock (callLock)
			{
				if (calls.Count > 0)
				{
					DefaultTrace.TraceCritical("[RNTBD Dispatcher {0}][{1}] Looks idle but still has {2} pending requests", ConnectionCorrelationId, this, calls.Count);
					flag = true;
				}
				else
				{
					callsAllowed = false;
				}
			}
			if (flag)
			{
				ScheduleIdleTimer(timeToIdle);
				return;
			}
			idleTimer = null;
			idleTimerTask = null;
			StartConnectionShutdown();
			t = CloseConnection();
		}
		WaitTask(t, "receive loop");
	}

	private void ScheduleIdleTimer(TimeSpan timeToIdle)
	{
		idleTimer = idleTimerPool.GetPooledTimer((int)timeToIdle.TotalSeconds);
		idleTimerTask = idleTimer.StartTimerAsync().ContinueWith(OnIdleTimer, TaskContinuationOptions.OnlyOnRanToCompletion);
		idleTimerTask.ContinueWith(delegate(Task failedTask)
		{
			DefaultTrace.TraceWarning("[RNTBD Dispatcher {0}][{1}] Idle timer callback failed: {2}", ConnectionCorrelationId, this, failedTask.Exception?.InnerException);
		}, TaskContinuationOptions.OnlyOnFaulted);
	}

	private void StartConnectionShutdown()
	{
		if (cancellation.IsCancellationRequested)
		{
			return;
		}
		try
		{
			lock (callLock)
			{
				callsAllowed = false;
			}
			cancellation.Cancel();
		}
		catch (AggregateException ex)
		{
			DefaultTrace.TraceWarning("[RNTBD Dispatcher {0}][{1}] Registered cancellation callbacks failed: {2}", ConnectionCorrelationId, this, ex);
		}
	}

	private Task StopIdleTimer()
	{
		Task result = null;
		if (idleTimer != null)
		{
			if (idleTimer.CancelTimer())
			{
				idleTimer = null;
				idleTimerTask = null;
			}
			else
			{
				result = idleTimerTask;
			}
		}
		return result;
	}

	private Task CloseConnection()
	{
		Task result = null;
		if (!connection.Disposed)
		{
			lock (callLock)
			{
				result = receiveTask;
			}
			connection.Dispose();
			this.TestOnConnectionClosed?.Invoke();
		}
		return result;
	}

	private void WaitTask(Task t, string description)
	{
		if (t == null)
		{
			return;
		}
		try
		{
			t.Wait();
		}
		catch (Exception ex)
		{
			DefaultTrace.TraceWarning("[RNTBD Dispatcher {0}][{1}] Parallel task failed: {2}. Exception: {3}", ConnectionCorrelationId, this, description, ex);
		}
	}

	private void ThrowIfDisposed()
	{
		if (disposed)
		{
			throw new ObjectDisposedException(string.Format("{0}:{1}", "Dispatcher", serverUri));
		}
	}

	private async Task NegotiateRntbdContextAsync(ChannelOpenArguments args)
	{
		byte[] buffer = TransportSerialization.BuildContextRequest(args.CommonArguments.ActivityId, userAgent, args.CallerId, enableChannelMultiplexing);
		await connection.WriteRequestAsync(args.CommonArguments, new TransportSerialization.SerializedRequest(new BufferProvider.DisposableBuffer(buffer), null), null);
		using Connection.ResponseMetadata responseMd = await connection.ReadResponseMetadataAsync(args.CommonArguments);
		StatusCodes status = (StatusCodes)BitConverter.ToUInt32(responseMd.Header.Array, 4);
		byte[] array = new byte[16];
		Buffer.BlockCopy(responseMd.Header.Array, 8, array, 0, 16);
		Guid activityId = new Guid(array);
		Trace.CorrelationManager.ActivityId = activityId;
		BytesDeserializer reader = new BytesDeserializer(responseMd.Metadata.Array, responseMd.Metadata.Count);
		RntbdConstants.ConnectionContextResponse response = new RntbdConstants.ConnectionContextResponse();
		response.ParseFrom(ref reader);
		string stringFromBytes = BytesSerializer.GetStringFromBytes(response.serverAgent.value.valueBytes);
		string stringFromBytes2 = BytesSerializer.GetStringFromBytes(response.serverVersion.value.valueBytes);
		serverProperties = new ServerProperties(stringFromBytes, stringFromBytes2);
		if ((uint)status < 200u || (uint)status >= 400u)
		{
			using (MemoryStream stream = await connection.ReadResponseBodyAsync(new ChannelCommonArguments(activityId, TransportErrorCode.TransportNegotiationTimeout, args.CommonArguments.UserPayload)))
			{
				Error error = JsonSerializer.Deserialize<Error>(stream);
				DocumentClientException ex = new DocumentClientException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, error.ToString()), null, (HttpStatusCode)status, connection.ServerUri);
				if (response.clientVersion.isPresent)
				{
					ex.Headers.Add("RequiredClientVersion", BytesSerializer.GetStringFromBytes(response.clientVersion.value.valueBytes));
				}
				if (response.protocolVersion.isPresent)
				{
					ex.Headers.Add("RequiredProtocolVersion", response.protocolVersion.value.valueULong.ToString());
				}
				if (response.serverAgent.isPresent)
				{
					ex.Headers.Add("ServerAgent", BytesSerializer.GetStringFromBytes(response.serverAgent.value.valueBytes));
				}
				if (response.serverVersion.isPresent)
				{
					ex.Headers.Add("x-ms-serviceversion", BytesSerializer.GetStringFromBytes(response.serverVersion.value.valueBytes));
				}
				throw ex;
			}
		}
		NotifyConnectionOnSuccessEvent();
		args.OpenTimeline.RecordRntbdHandshakeFinishTime();
	}

	private async Task ReceiveLoopAsync()
	{
		CancellationToken cancellationToken = cancellation.Token;
		ChannelCommonArguments args = new ChannelCommonArguments(Guid.Empty, TransportErrorCode.ReceiveTimeout, userPayload: true);
		Connection.ResponseMetadata responseMd = null;
		try
		{
			bool hasTransportErrors = false;
			while (!hasTransportErrors && !cancellationToken.IsCancellationRequested)
			{
				if (isFaultInjectionedConnectionError)
				{
					throw faultInjectionTransportException;
				}
				args.ActivityId = Guid.Empty;
				responseMd = await connection.ReadResponseMetadataAsync(args);
				ArraySegment<byte> metadata = responseMd.Metadata;
				TransportSerialization.RntbdHeader header = TransportSerialization.DecodeRntbdHeader(responseMd.Header.Array);
				args.ActivityId = header.ActivityId;
				BytesDeserializer rntbdHeaderReader = new BytesDeserializer(metadata.Array, metadata.Count);
				MemoryStream responseBody = null;
				if (HeadersTransportSerialization.TryParseMandatoryResponseHeaders(ref rntbdHeaderReader, out var payloadPresent, out var transportRequestId))
				{
					if (payloadPresent)
					{
						responseBody = await connection.ReadResponseBodyAsync(args);
					}
					DispatchRntbdResponse(responseMd, header, responseBody, metadata.Array, metadata.Count, transportRequestId);
				}
				else
				{
					hasTransportErrors = true;
					DispatchChannelFailureException(TransportExceptions.GetInternalServerErrorException(serverUri, RMResources.MissingRequiredHeader));
				}
				responseMd = null;
			}
			DispatchCancellation();
		}
		catch (OperationCanceledException)
		{
			responseMd?.Dispose();
			DispatchCancellation();
		}
		catch (ObjectDisposedException)
		{
			responseMd?.Dispose();
			DispatchCancellation();
		}
		catch (Exception ex3)
		{
			responseMd?.Dispose();
			DispatchChannelFailureException(ex3);
		}
	}

	private Dictionary<uint, CallInfo> StopCalls()
	{
		lock (callLock)
		{
			Dictionary<uint, CallInfo> result = new Dictionary<uint, CallInfo>(calls);
			calls.Clear();
			callsAllowed = false;
			return result;
		}
	}

	private void DispatchRntbdResponse(Connection.ResponseMetadata responseMd, TransportSerialization.RntbdHeader responseHeader, MemoryStream responseBody, byte[] metadata, int metadataLength, uint transportRequestId)
	{
		CallInfo callInfo = RemoveCall(transportRequestId);
		if (callInfo != null)
		{
			callInfo.TransportRequestStats.RecordState(TransportRequestStats.RequestStage.Received);
			callInfo.TransportRequestStats.ResponseMetadataSizeInBytes = responseMd.Metadata.Count;
			callInfo.TransportRequestStats.ResponseBodySizeInBytes = responseBody?.Length;
			callInfo.SetResponse(responseMd, responseHeader, responseBody, serverProperties.Version, metadata, metadataLength);
		}
		else
		{
			responseBody?.Dispose();
			responseMd.Dispose();
		}
	}

	private void DispatchChannelFailureException(Exception ex)
	{
		Dictionary<uint, CallInfo> dictionary = StopCalls();
		foreach (KeyValuePair<uint, CallInfo> item in dictionary)
		{
			item.Value.SetConnectionBrokenException(ex, ToString());
		}
		if (dictionary.Count > 0 || this.connectionStateListener == null)
		{
			return;
		}
		if (!(ex is TransportException { ErrorCode: var errorCode } ex2))
		{
			DefaultTrace.TraceWarning("[RNTBD Dispatcher {0}] Not a TransportException. Will not raise the connection state change event: {1}", ConnectionCorrelationId, ex);
			return;
		}
		ConnectionEvent connectionEvent;
		switch (errorCode)
		{
		case TransportErrorCode.ReceiveStreamClosed:
			connectionEvent = ConnectionEvent.ReadEof;
			break;
		case TransportErrorCode.ReceiveFailed:
			connectionEvent = ConnectionEvent.ReadFailure;
			break;
		default:
			DefaultTrace.TraceWarning("[RNTBD Dispatcher {0}] Will not raise the connection state change event for TransportException error code {1}. Exception: {2}", ConnectionCorrelationId, ex2.ErrorCode.ToString(), ex2.Message);
			return;
		}
		IConnectionStateListener connectionStateListener = this.connectionStateListener;
		ServerKey serverKey = new ServerKey(serverUri);
		DateTime exceptionTime = ex2.Timestamp;
		Task.Run(delegate
		{
			connectionStateListener.OnConnectionEvent(connectionEvent, exceptionTime, serverKey);
		}).ContinueWith(delegate(Task failedTask, object connectionIdObject)
		{
			DefaultTrace.TraceError("[RNTBD Dispatcher {0}] OnConnectionEvent callback failed: {1}", connectionIdObject, failedTask.Exception?.InnerException);
		}, ConnectionCorrelationId, TaskContinuationOptions.OnlyOnFaulted);
	}

	private void DispatchCancellation()
	{
		foreach (KeyValuePair<uint, CallInfo> item in StopCalls())
		{
			item.Value.Cancel();
		}
	}

	private CallInfo RemoveCall(uint requestId)
	{
		CallInfo value = null;
		lock (callLock)
		{
			calls.TryGetValue(requestId, out value);
			calls.Remove(requestId);
			return value;
		}
	}

	private void DisallowInitialCalls()
	{
		lock (callLock)
		{
			callsAllowed = false;
		}
	}

	private void DisallowRuntimeCalls()
	{
		lock (callLock)
		{
			callsAllowed = false;
		}
	}
}
