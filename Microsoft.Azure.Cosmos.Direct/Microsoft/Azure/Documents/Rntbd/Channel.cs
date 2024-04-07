using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.FaultInjection;

namespace Microsoft.Azure.Documents.Rntbd;

internal sealed class Channel : IChannel, IDisposable
{
	private enum State
	{
		New,
		WaitingToOpen,
		Opening,
		Open,
		Closed
	}

	private readonly Dispatcher dispatcher;

	private readonly TimerPool timerPool;

	private readonly int requestTimeoutSeconds;

	private readonly Uri serverUri;

	private readonly bool localRegionRequest;

	private bool disposed;

	private readonly ReaderWriterLockSlim stateLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

	private State state;

	private Task initializationTask;

	private volatile bool isInitializationComplete;

	private ChannelOpenArguments openArguments;

	private readonly SemaphoreSlim openingSlim;

	private readonly IChaosInterceptor chaosInterceptor;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public bool Healthy
	{
		get
		{
			ThrowIfDisposed();
			Dispatcher dispatcher = null;
			stateLock.EnterReadLock();
			try
			{
				switch (state)
				{
				case State.Open:
					dispatcher = this.dispatcher;
					break;
				case State.WaitingToOpen:
				case State.Opening:
					return true;
				case State.Closed:
					return false;
				case State.New:
					return false;
				default:
					return false;
				}
			}
			finally
			{
				stateLock.ExitReadLock();
			}
			return dispatcher.Healthy;
		}
	}

	private Guid ConnectionCorrelationId => dispatcher.ConnectionCorrelationId;

	internal bool TestIsIdle => dispatcher.TestIsIdle;

	internal event Action TestOnInitializeComplete;

	internal event Action TestOnConnectionClosed
	{
		add
		{
			dispatcher.TestOnConnectionClosed += value;
		}
		remove
		{
			dispatcher.TestOnConnectionClosed -= value;
		}
	}

	public Channel(Guid activityId, Uri serverUri, ChannelProperties channelProperties, bool localRegionRequest, SemaphoreSlim openingSlim, IChaosInterceptor chaosInterceptor = null, Func<Guid, Guid, Uri, Channel, Task> onChannelOpen = null)
	{
		dispatcher = new Dispatcher(serverUri, channelProperties.UserAgent, channelProperties.ConnectionStateListener, channelProperties.CertificateHostNameOverride, channelProperties.ReceiveHangDetectionTime, channelProperties.SendHangDetectionTime, channelProperties.IdleTimerPool, channelProperties.IdleTimeout, channelProperties.EnableChannelMultiplexing, channelProperties.MemoryStreamPool, channelProperties.RemoteCertificateValidationCallback, channelProperties.DnsResolutionFunction, chaosInterceptor);
		timerPool = channelProperties.RequestTimerPool;
		requestTimeoutSeconds = (int)channelProperties.RequestTimeout.TotalSeconds;
		this.serverUri = serverUri;
		this.localRegionRequest = localRegionRequest;
		this.chaosInterceptor = chaosInterceptor;
		TimeSpan openTimeout = (localRegionRequest ? channelProperties.LocalRegionOpenTimeout : channelProperties.OpenTimeout);
		openArguments = new ChannelOpenArguments(activityId, new ChannelOpenTimeline(), openTimeout, channelProperties.PortReuseMode, channelProperties.UserPortPool, channelProperties.CallerId);
		this.openingSlim = openingSlim;
		Initialize(activityId, onChannelOpen);
	}

	public void InjectFaultInjectionConnectionError(TransportException transportException)
	{
		if (!disposed)
		{
			dispatcher.InjectFaultInjectionConnectionError(transportException);
		}
	}

	public Uri GetServerUri()
	{
		ThrowIfDisposed();
		return serverUri;
	}

	private void Initialize(Guid activityId, Func<Guid, Guid, Uri, Channel, Task> onChannelOpen = null)
	{
		ThrowIfDisposed();
		stateLock.EnterWriteLock();
		try
		{
			state = State.WaitingToOpen;
			initializationTask = Task.Run(async delegate
			{
				Trace.CorrelationManager.ActivityId = openArguments.CommonArguments.ActivityId;
				await InitializeAsync(activityId, onChannelOpen);
				isInitializationComplete = true;
				this.TestOnInitializeComplete?.Invoke();
			});
		}
		finally
		{
			stateLock.ExitWriteLock();
		}
	}

	public async Task<StoreResponse> RequestAsync(DocumentServiceRequest request, TransportAddressUri physicalAddress, ResourceOperation resourceOperation, Guid activityId, TransportRequestStats transportRequestStats)
	{
		ThrowIfDisposed();
		if (!isInitializationComplete)
		{
			transportRequestStats.RequestWaitingForConnectionInitialization = true;
			DefaultTrace.TraceInformation("[RNTBD Channel {0}] Awaiting RNTBD channel initialization. Request URI: {1}", ConnectionCorrelationId, physicalAddress);
			await initializationTask;
		}
		else
		{
			transportRequestStats.RequestWaitingForConnectionInitialization = false;
		}
		transportRequestStats.RecordState(TransportRequestStats.RequestStage.Pipelined);
		using ChannelCallArguments callArguments = ((chaosInterceptor == null) ? new ChannelCallArguments(activityId) : new ChannelCallArguments(activityId, request.OperationType, request.ResourceType, request.RequestContext.ResolvedCollectionRid, request.Headers, request.RequestContext.LocationEndpointToRoute));
		try
		{
			callArguments.PreparedCall = dispatcher.PrepareCall(request, physicalAddress, resourceOperation, activityId, transportRequestStats);
		}
		catch (DocumentClientException ex)
		{
			ex.Headers.Add("x-ms-request-validation-failure", "1");
			throw;
		}
		catch (Exception ex2)
		{
			DefaultTrace.TraceError("[RNTBD Channel {0}] Failed to serialize request. Assuming malformed request payload: {1}", ConnectionCorrelationId, ex2);
			throw new BadRequestException(ex2)
			{
				Headers = { { "x-ms-request-validation-failure", "1" } }
			};
		}
		PooledTimer timer = timerPool.GetPooledTimer(requestTimeoutSeconds);
		Task[] tasks = new Task[2]
		{
			timer.StartTimerAsync(),
			null
		};
		Task<StoreResponse> dispatcherCall = dispatcher.CallAsync(callArguments, transportRequestStats);
		TransportClient.GetTransportPerformanceCounters().LogRntbdBytesSentCount(resourceOperation.resourceType, resourceOperation.operationType, callArguments.PreparedCall?.SerializedRequest.RequestSize);
		tasks[1] = dispatcherCall;
		Task task = await Task.WhenAny(tasks);
		if (task == tasks[0])
		{
			callArguments.CommonArguments.SnapshotCallState(out var timeoutCode, out var payloadSent);
			dispatcher.CancelCallAndNotifyConnectionOnTimeoutEvent(callArguments.PreparedCall, request.IsReadOnlyRequest);
			HandleTaskTimeout(tasks[1], activityId, ConnectionCorrelationId);
			Exception innerException = task.Exception?.InnerException;
			DefaultTrace.TraceWarning("[RNTBD Channel {0}] RNTBD call timed out on channel {1}. Error: {2}", ConnectionCorrelationId, this, timeoutCode);
			throw new TransportException(timeoutCode, innerException, activityId, physicalAddress.Uri, ToString(), callArguments.CommonArguments.UserPayload, payloadSent);
		}
		timer.CancelTimer();
		dispatcher.NotifyConnectionOnSuccessEvent();
		if (task.IsFaulted)
		{
			await task;
		}
		physicalAddress.SetConnected();
		StoreResponse result = dispatcherCall.Result;
		TransportClient.GetTransportPerformanceCounters().LogRntbdBytesReceivedCount(resourceOperation.resourceType, resourceOperation.operationType, result?.ResponseBody?.Length);
		return result;
	}

	public Task OpenChannelAsync(Guid activityId)
	{
		if (initializationTask == null)
		{
			throw new InvalidOperationException("Channal Initialization Task Can't be null.");
		}
		return initializationTask;
	}

	public override string ToString()
	{
		return dispatcher.ToString();
	}

	public void Close()
	{
		((IDisposable)this).Dispose();
	}

	void IDisposable.Dispose()
	{
		chaosInterceptor?.OnChannelDispose(ConnectionCorrelationId);
		ThrowIfDisposed();
		disposed = true;
		DefaultTrace.TraceInformation("[RNTBD Channel {0}] Disposing RNTBD Channel {1}", ConnectionCorrelationId, this);
		Task task = null;
		stateLock.EnterWriteLock();
		try
		{
			if (state != State.Closed)
			{
				task = initializationTask;
			}
			state = State.Closed;
		}
		finally
		{
			stateLock.ExitWriteLock();
		}
		if (task != null)
		{
			try
			{
				task.Wait();
			}
			catch (Exception ex)
			{
				DefaultTrace.TraceWarning("[RNTBD Channel {0}] {1} initialization failed. Consuming the task exception in {2}. Server URI: {3}. Exception: {4}", ConnectionCorrelationId, "Channel", "Dispose", serverUri, ex.Message);
			}
		}
		dispatcher.Dispose();
		stateLock.Dispose();
	}

	private void ThrowIfDisposed()
	{
		if (disposed)
		{
			throw new ObjectDisposedException("Channel");
		}
	}

	private async Task InitializeAsync(Guid activityId, Func<Guid, Guid, Uri, Channel, Task> onChannelOpen = null)
	{
		bool slimAcquired = false;
		try
		{
			if (chaosInterceptor != null)
			{
				await (onChannelOpen?.Invoke(activityId, ConnectionCorrelationId, serverUri, this));
			}
			openArguments.CommonArguments.SetTimeoutCode(TransportErrorCode.ChannelWaitingToOpenTimeout);
			slimAcquired = await openingSlim.WaitAsync(openArguments.OpenTimeout).ConfigureAwait(continueOnCapturedContext: false);
			if (!slimAcquired)
			{
				openArguments.CommonArguments.SnapshotCallState(out var timeoutCode, out var payloadSent);
				DefaultTrace.TraceWarning("[RNTBD Channel {0}] RNTBD waiting to open timed out on channel {1}. Error: {2}", ConnectionCorrelationId, this, timeoutCode);
				throw new TransportException(timeoutCode, null, openArguments.CommonArguments.ActivityId, serverUri, ToString(), openArguments.CommonArguments.UserPayload, payloadSent);
			}
			openArguments.CommonArguments.SetTimeoutCode(TransportErrorCode.ChannelOpenTimeout);
			state = State.Opening;
			PooledTimer timer = timerPool.GetPooledTimer(openArguments.OpenTimeout);
			Task[] tasks = new Task[2];
			if (localRegionRequest && openArguments.OpenTimeout < timer.MinSupportedTimeout)
			{
				tasks[0] = Task.Delay(openArguments.OpenTimeout);
			}
			else
			{
				tasks[0] = timer.StartTimerAsync();
			}
			tasks[1] = dispatcher.OpenAsync(openArguments);
			Task task = await Task.WhenAny(tasks);
			if (task == tasks[0])
			{
				openArguments.CommonArguments.SnapshotCallState(out var timeoutCode2, out var payloadSent2);
				HandleTaskTimeout(tasks[1], openArguments.CommonArguments.ActivityId, ConnectionCorrelationId);
				Exception innerException = task.Exception?.InnerException;
				DefaultTrace.TraceWarning("[RNTBD Channel {0}] RNTBD open timed out on channel {1}. Error: {2}", ConnectionCorrelationId, this, timeoutCode2);
				throw new TransportException(timeoutCode2, innerException, openArguments.CommonArguments.ActivityId, serverUri, ToString(), openArguments.CommonArguments.UserPayload, payloadSent2);
			}
			timer.CancelTimer();
			if (task.IsFaulted)
			{
				await task;
			}
			FinishInitialization(State.Open);
		}
		catch (DocumentClientException ex)
		{
			FinishInitialization(State.Closed);
			ex.Headers.Set("x-ms-activity-id", openArguments.CommonArguments.ActivityId.ToString());
			DefaultTrace.TraceWarning("[RNTBD Channel {0}] Channel.InitializeAsync failed. Channel: {1}. DocumentClientException: {2}", ConnectionCorrelationId, this, ex);
			throw;
		}
		catch (TransportException ex2)
		{
			FinishInitialization(State.Closed);
			DefaultTrace.TraceWarning("[RNTBD Channel {0}] Channel.InitializeAsync failed. Channel: {1}. TransportException: {2}", ConnectionCorrelationId, this, ex2);
			throw;
		}
		catch (Exception ex3)
		{
			FinishInitialization(State.Closed);
			DefaultTrace.TraceWarning("[RNTBD Channel {0}] Channel.InitializeAsync failed. Wrapping exception in TransportException. Channel: {1}. Inner exception: {2}", ConnectionCorrelationId, this, ex3);
			throw new TransportException(TransportErrorCode.ChannelOpenFailed, ex3, openArguments.CommonArguments.ActivityId, serverUri, ToString(), openArguments.CommonArguments.UserPayload, openArguments.CommonArguments.PayloadSent);
		}
		finally
		{
			openArguments.OpenTimeline.WriteTrace();
			openArguments = null;
			if (slimAcquired)
			{
				openingSlim.Release();
			}
		}
	}

	private void FinishInitialization(State nextState)
	{
		Task task = null;
		stateLock.EnterWriteLock();
		try
		{
			if (state != State.Closed)
			{
				state = nextState;
				task = initializationTask;
			}
		}
		finally
		{
			stateLock.ExitWriteLock();
		}
		if (nextState == State.Closed)
		{
			task?.ContinueWith(delegate(Task completedTask)
			{
				DefaultTrace.TraceWarning("[RNTBD Channel {0}] {1} initialization failed. Consuming the task exception asynchronously. Server URI: {2}. Exception: {3}", ConnectionCorrelationId, "Channel", serverUri, completedTask.Exception.InnerException?.Message);
			}, TaskContinuationOptions.OnlyOnFaulted);
		}
	}

	private static void HandleTaskTimeout(Task runawayTask, Guid activityId, Guid connectionCorrelationId)
	{
		runawayTask.ContinueWith(delegate(Task task)
		{
			Trace.CorrelationManager.ActivityId = activityId;
			Exception innerException = task.Exception.InnerException;
			DefaultTrace.TraceInformation("[RNTBD Channel {0}] Timed out task completed. Activity ID = {1}. HRESULT = {2:X}. Exception: {3}", connectionCorrelationId, activityId, innerException.HResult, innerException);
		}, TaskContinuationOptions.OnlyOnFaulted);
	}
}
