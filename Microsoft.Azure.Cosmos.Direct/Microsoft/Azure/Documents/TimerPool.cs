using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Azure.Cosmos.Core.Trace;

namespace Microsoft.Azure.Documents;

internal sealed class TimerPool : IDisposable
{
	[ThreadStatic]
	private static Random PooledTimerBucketSelector;

	private readonly Timer timer;

	private readonly ConcurrentDictionary<int, ConcurrentQueue<PooledTimer>>[] pooledTimersByTimeout;

	private readonly TimeSpan minSupportedTimeout;

	private readonly object timerConcurrencyLock;

	private bool isRunning;

	private bool isDisposed;

	public TimeSpan MinSupportedTimeout => minSupportedTimeout;

	internal ConcurrentDictionary<int, ConcurrentQueue<PooledTimer>>[] PooledTimersByTimeout => pooledTimersByTimeout;

	public TimerPool(int minSupportedTimerDelayInSeconds, int maxBucketsForPools = -1)
	{
		timerConcurrencyLock = new object();
		minSupportedTimeout = TimeSpan.FromSeconds((minSupportedTimerDelayInSeconds <= 0) ? 1 : minSupportedTimerDelayInSeconds);
		maxBucketsForPools = ((maxBucketsForPools > 0) ? maxBucketsForPools : Environment.ProcessorCount);
		pooledTimersByTimeout = new ConcurrentDictionary<int, ConcurrentQueue<PooledTimer>>[maxBucketsForPools];
		for (int i = 0; i < maxBucketsForPools; i++)
		{
			pooledTimersByTimeout[i] = new ConcurrentDictionary<int, ConcurrentQueue<PooledTimer>>();
		}
		TimerCallback callback = OnTimer;
		timer = new Timer(callback, null, TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(minSupportedTimerDelayInSeconds));
		DefaultTrace.TraceInformation("TimerPool Created with minSupportedTimerDelayInSeconds = {0}", minSupportedTimerDelayInSeconds);
	}

	public void Dispose()
	{
		if (!isDisposed)
		{
			DisposeAllPooledTimers();
			isDisposed = true;
		}
	}

	private void ThrowIfDisposed()
	{
		if (isDisposed)
		{
			throw new ObjectDisposedException("TimerPool");
		}
	}

	private void DisposeAllPooledTimers()
	{
		DefaultTrace.TraceInformation("TimerPool Disposing");
		ConcurrentDictionary<int, ConcurrentQueue<PooledTimer>>[] array = pooledTimersByTimeout;
		for (int i = 0; i < array.Length; i++)
		{
			foreach (KeyValuePair<int, ConcurrentQueue<PooledTimer>> item in array[i])
			{
				ConcurrentQueue<PooledTimer> value = item.Value;
				PooledTimer result;
				while (value.TryDequeue(out result))
				{
					result.CancelTimer();
				}
			}
		}
		timer.Dispose();
		DefaultTrace.TraceInformation("TimePool Disposed");
	}

	private void OnTimer(object stateInfo)
	{
		lock (timerConcurrencyLock)
		{
			if (isRunning)
			{
				return;
			}
			isRunning = true;
		}
		try
		{
			long ticks = DateTime.UtcNow.Ticks;
			ConcurrentDictionary<int, ConcurrentQueue<PooledTimer>>[] array = pooledTimersByTimeout;
			for (int i = 0; i < array.Length; i++)
			{
				foreach (KeyValuePair<int, ConcurrentQueue<PooledTimer>> item in array[i])
				{
					ConcurrentQueue<PooledTimer> value = item.Value;
					int count = item.Value.Count;
					long num = 0L;
					for (int j = 0; j < count; j++)
					{
						if (value.TryPeek(out var result))
						{
							if (ticks < result.TimeoutTicks)
							{
								break;
							}
							if (result.TimeoutTicks < num)
							{
								DefaultTrace.TraceCritical("LastTicks: {0}, PooledTimer.Ticks: {1}", num, result.TimeoutTicks);
							}
							result.FireTimeout();
							num = result.TimeoutTicks;
							if (value.TryDequeue(out var result2) && result2 != result)
							{
								DefaultTrace.TraceCritical("Timer objects peeked and dequeued are not equal");
								value.Enqueue(result2);
							}
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			DefaultTrace.TraceCritical("Hit exception ex: {0}\n, stack: {1}", ex.Message, ex.StackTrace);
		}
		finally
		{
			lock (timerConcurrencyLock)
			{
				isRunning = false;
			}
		}
	}

	public PooledTimer GetPooledTimer(int timeoutInSeconds)
	{
		ThrowIfDisposed();
		return new PooledTimer(timeoutInSeconds, this);
	}

	public PooledTimer GetPooledTimer(TimeSpan timeout)
	{
		ThrowIfDisposed();
		return new PooledTimer(timeout, this);
	}

	public long SubscribeForTimeouts(PooledTimer pooledTimer)
	{
		ThrowIfDisposed();
		if (pooledTimer.Timeout < minSupportedTimeout)
		{
			object[] obj = new object[2]
			{
				pooledTimer.Timeout.TotalSeconds,
				null
			};
			TimeSpan timeSpan = minSupportedTimeout;
			obj[1] = timeSpan.TotalSeconds;
			DefaultTrace.TraceWarning("Timer timeoutinSeconds {0} is less than minSupportedTimeoutInSeconds {1}, will use the minsupported value", obj);
			pooledTimer.Timeout = minSupportedTimeout;
		}
		if (PooledTimerBucketSelector == null)
		{
			PooledTimerBucketSelector = new Random();
		}
		int num = PooledTimerBucketSelector.Next(pooledTimersByTimeout.Length);
		if (!pooledTimersByTimeout[num].TryGetValue((int)pooledTimer.Timeout.TotalSeconds, out var value))
		{
			value = pooledTimersByTimeout[num].GetOrAdd((int)pooledTimer.Timeout.TotalSeconds, (int _) => new ConcurrentQueue<PooledTimer>());
		}
		lock (value)
		{
			value.Enqueue(pooledTimer);
			return DateTime.UtcNow.Ticks;
		}
	}
}
