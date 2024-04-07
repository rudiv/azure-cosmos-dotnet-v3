using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Core.Trace;

namespace Microsoft.Azure.Documents.Rntbd;

internal sealed class CpuMonitor : IDisposable
{
	internal const int DefaultRefreshIntervalInSeconds = 10;

	private const int HistoryLength = 6;

	private static TimeSpan refreshInterval = TimeSpan.FromSeconds(10.0);

	private bool disposed;

	private readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

	private CancellationTokenSource cancellation;

	private CpuLoadHistory currentReading;

	private Task periodicTask;

	internal static void OverrideRefreshInterval(TimeSpan newRefreshInterval)
	{
		refreshInterval = newRefreshInterval;
	}

	public void Start()
	{
		ThrowIfDisposed();
		rwLock.EnterWriteLock();
		try
		{
			ThrowIfDisposed();
			if (periodicTask != null)
			{
				throw new InvalidOperationException("CpuMonitor already started");
			}
			cancellation = new CancellationTokenSource();
			CancellationToken cancellationToken = cancellation.Token;
			periodicTask = Task.Factory.StartNew(() => RefreshLoopAsync(cancellationToken), cancellationToken, TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach, TaskScheduler.Default).Unwrap();
			periodicTask.ContinueWith(delegate(Task t)
			{
				DefaultTrace.TraceError("The CPU monitor refresh task failed. Exception: {0}", t.Exception);
			}, TaskContinuationOptions.OnlyOnFaulted);
			periodicTask.ContinueWith(delegate(Task t)
			{
				DefaultTrace.TraceInformation("The CPU monitor refresh task stopped. Status: {0}", t.Status);
			}, TaskContinuationOptions.NotOnFaulted);
		}
		finally
		{
			rwLock.ExitWriteLock();
		}
		DefaultTrace.TraceInformation("CpuMonitor started");
	}

	private void StopCoreUnderWriteLock(ref CancellationTokenSource cancel, ref Task backgroundTask)
	{
		if (periodicTask == null)
		{
			throw new InvalidOperationException("CpuMonitor not started or has been stopped or disposed already.");
		}
		cancel = cancellation;
		backgroundTask = periodicTask;
		cancellation = null;
		currentReading = null;
		periodicTask = null;
	}

	private static void StopCoreAfterReleasingWriteLock(CancellationTokenSource cancel, Task backgroundTask)
	{
		cancel.Cancel();
		try
		{
			backgroundTask.Wait();
		}
		catch (AggregateException)
		{
		}
		cancel.Dispose();
	}

	public void Stop()
	{
		ThrowIfDisposed();
		CancellationTokenSource cancel = null;
		Task backgroundTask = null;
		rwLock.EnterWriteLock();
		try
		{
			ThrowIfDisposed();
			StopCoreUnderWriteLock(ref cancel, ref backgroundTask);
		}
		finally
		{
			rwLock.ExitWriteLock();
		}
		StopCoreAfterReleasingWriteLock(cancel, backgroundTask);
		DefaultTrace.TraceInformation("CpuMonitor stopped");
	}

	public CpuLoadHistory GetCpuLoad()
	{
		ThrowIfDisposed();
		rwLock.EnterReadLock();
		try
		{
			if (periodicTask == null)
			{
				throw new InvalidOperationException("CpuMonitor was not started or has been stopped or disposed already.");
			}
			return currentReading;
		}
		finally
		{
			rwLock.ExitReadLock();
		}
	}

	public void Dispose()
	{
		Interlocked.MemoryBarrier();
		if (disposed)
		{
			return;
		}
		CancellationTokenSource cancel = null;
		Task backgroundTask = null;
		rwLock.EnterWriteLock();
		try
		{
			Interlocked.MemoryBarrier();
			if (disposed)
			{
				return;
			}
			if (periodicTask != null)
			{
				StopCoreUnderWriteLock(ref cancel, ref backgroundTask);
			}
		}
		finally
		{
			MarkDisposed();
			rwLock.ExitWriteLock();
		}
		if (backgroundTask != null)
		{
			StopCoreAfterReleasingWriteLock(cancel, backgroundTask);
		}
		rwLock.Dispose();
	}

	private void MarkDisposed()
	{
		disposed = true;
		Interlocked.MemoryBarrier();
	}

	private void ThrowIfDisposed()
	{
		Interlocked.MemoryBarrier();
		if (disposed)
		{
			throw new ObjectDisposedException("CpuMonitor");
		}
	}

	private async Task RefreshLoopAsync(CancellationToken cancellationToken)
	{
		Trace.CorrelationManager.ActivityId = Guid.NewGuid();
		SystemUtilizationReaderBase cpuReader = SystemUtilizationReaderBase.SingletonInstance;
		CpuLoad[] buffer = new CpuLoad[6];
		int clockHand = 0;
		TaskCompletionSource<object> cancellationCompletion = new TaskCompletionSource<object>();
		cancellationToken.Register(delegate
		{
			cancellationCompletion.SetCanceled();
		});
		Task[] refreshTasks = new Task[2] { null, cancellationCompletion.Task };
		DateTime nextExpiration = DateTime.UtcNow;
		while (!cancellationToken.IsCancellationRequested)
		{
			DateTime utcNow = DateTime.UtcNow;
			float systemWideCpuUsage = cpuReader.GetSystemWideCpuUsage();
			if (!float.IsNaN(systemWideCpuUsage))
			{
				List<CpuLoad> list = new List<CpuLoad>(buffer.Length);
				CpuLoadHistory cpuLoadHistory = new CpuLoadHistory(new ReadOnlyCollection<CpuLoad>(list), refreshInterval);
				buffer[clockHand] = new CpuLoad(utcNow, systemWideCpuUsage);
				clockHand = (clockHand + 1) % buffer.Length;
				for (int i = 0; i < buffer.Length; i++)
				{
					int num = (clockHand + i) % buffer.Length;
					if (buffer[num].Timestamp != DateTime.MinValue)
					{
						list.Add(buffer[num]);
					}
				}
				rwLock.EnterWriteLock();
				try
				{
					if (cancellationToken.IsCancellationRequested)
					{
						break;
					}
					currentReading = cpuLoadHistory;
				}
				finally
				{
					rwLock.ExitWriteLock();
				}
			}
			for (utcNow = DateTime.UtcNow; nextExpiration <= utcNow; nextExpiration += refreshInterval)
			{
			}
			TimeSpan timeSpan = nextExpiration - DateTime.UtcNow;
			if (timeSpan > TimeSpan.Zero)
			{
				refreshTasks[0] = Task.Delay(timeSpan);
				if (await Task.WhenAny(refreshTasks) == refreshTasks[1])
				{
					break;
				}
			}
		}
	}
}
