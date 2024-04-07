using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents;

internal sealed class PooledTimer
{
	private long beginTicks;

	private TimeSpan timeoutPeriod;

	private TimerPool timerPool;

	private readonly TaskCompletionSource<object> tcs;

	private readonly object memberLock;

	private bool timerStarted;

	public long TimeoutTicks => beginTicks + Timeout.Ticks;

	public TimeSpan Timeout
	{
		get
		{
			return timeoutPeriod;
		}
		set
		{
			timeoutPeriod = value;
		}
	}

	public TimeSpan MinSupportedTimeout => timerPool.MinSupportedTimeout;

	public PooledTimer(int timeout, TimerPool timerPool)
		: this(TimeSpan.FromSeconds(timeout), timerPool)
	{
	}

	public PooledTimer(TimeSpan timeout, TimerPool timerPool)
	{
		timeoutPeriod = timeout;
		tcs = new TaskCompletionSource<object>();
		this.timerPool = timerPool;
		memberLock = new object();
	}

	public Task StartTimerAsync()
	{
		lock (memberLock)
		{
			if (timerStarted)
			{
				throw new InvalidOperationException("Timer Already Started");
			}
			beginTicks = timerPool.SubscribeForTimeouts(this);
			timerStarted = true;
			return tcs.Task;
		}
	}

	public bool CancelTimer()
	{
		return tcs.TrySetCanceled();
	}

	internal bool FireTimeout()
	{
		return tcs.TrySetResult(null);
	}
}
