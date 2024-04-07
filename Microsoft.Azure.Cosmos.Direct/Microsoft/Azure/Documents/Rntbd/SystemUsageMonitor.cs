using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Core.Trace;

namespace Microsoft.Azure.Documents.Rntbd;

internal sealed class SystemUsageMonitor : IDisposable
{
	private readonly SystemUtilizationReaderBase systemUtilizationReader = SystemUtilizationReaderBase.SingletonInstance;

	private readonly IDictionary<string, SystemUsageRecorder> recorders = new Dictionary<string, SystemUsageRecorder>();

	private readonly Stopwatch watch = new Stopwatch();

	private int pollDelayInMilliSeconds;

	private CancellationTokenSource cancellation;

	private Task periodicTask { get; set; }

	private bool disposed { get; set; }

	internal int PollDelayInMs => pollDelayInMilliSeconds;

	public bool IsRunning()
	{
		return periodicTask.Status == TaskStatus.Running;
	}

	internal bool TryGetBackgroundTaskException(out AggregateException aggregateException)
	{
		aggregateException = periodicTask?.Exception;
		return aggregateException != null;
	}

	public static SystemUsageMonitor CreateAndStart(IReadOnlyList<SystemUsageRecorder> usageRecorders)
	{
		SystemUsageMonitor systemUsageMonitor = new SystemUsageMonitor(usageRecorders);
		systemUsageMonitor.Start();
		return systemUsageMonitor;
	}

	private SystemUsageMonitor(IReadOnlyList<SystemUsageRecorder> recorders)
	{
		if (recorders.Count == 0)
		{
			throw new ArgumentException("No Recorders are configured so nothing to process");
		}
		int timeInterval = 0;
		foreach (SystemUsageRecorder recorder in recorders)
		{
			this.recorders.Add(recorder.identifier, recorder);
			TimeSpan refreshInterval = recorder.refreshInterval;
			timeInterval = GCD((int)refreshInterval.TotalMilliseconds, timeInterval);
		}
		pollDelayInMilliSeconds = timeInterval;
	}

	private int GCD(int timeInterval1, int timeInterval2)
	{
		if (timeInterval2 != 0)
		{
			return GCD(timeInterval2, timeInterval1 % timeInterval2);
		}
		return timeInterval1;
	}

	private void Start()
	{
		ThrowIfDisposed();
		if (periodicTask != null)
		{
			throw new InvalidOperationException("SystemUsageMonitor already started");
		}
		cancellation = new CancellationTokenSource();
		periodicTask = Task.Run(delegate
		{
			RefreshLoopAsync(cancellation.Token);
		}, cancellation.Token);
		periodicTask.ContinueWith(delegate(Task t)
		{
			DefaultTrace.TraceError("The CPU and Memory usage monitoring refresh task failed. Exception: {0}", t.Exception);
		}, TaskContinuationOptions.OnlyOnFaulted);
		periodicTask.ContinueWith(delegate(Task t)
		{
			DefaultTrace.TraceWarning("The CPU and Memory usage monitoring refresh task stopped. Status: {0}", t.Status);
		}, TaskContinuationOptions.NotOnFaulted);
		DefaultTrace.TraceInformation("SystemUsageMonitor started");
	}

	public void Stop()
	{
		ThrowIfDisposed();
		if (periodicTask == null)
		{
			throw new InvalidOperationException("SystemUsageMonitor not running");
		}
		CancellationTokenSource cancellationTokenSource = cancellation;
		Task task = periodicTask;
		watch.Stop();
		cancellation = null;
		periodicTask = null;
		cancellationTokenSource.Cancel();
		try
		{
			task.Wait();
		}
		catch (AggregateException)
		{
		}
		cancellationTokenSource.Dispose();
		DefaultTrace.TraceInformation("SystemUsageMonitor stopped");
	}

	public SystemUsageRecorder GetRecorder(string recorderKey)
	{
		ThrowIfDisposed();
		if (periodicTask == null)
		{
			DefaultTrace.TraceError("SystemUsageMonitor is not started");
			throw new InvalidOperationException("SystemUsageMonitor was not started");
		}
		if (!recorders.TryGetValue(recorderKey, out var value))
		{
			throw new ArgumentException("Recorder Identifier not present i.e. " + recorderKey);
		}
		return value;
	}

	public void Dispose()
	{
		ThrowIfDisposed();
		if (periodicTask != null)
		{
			Stop();
		}
		disposed = true;
	}

	private void ThrowIfDisposed()
	{
		if (disposed)
		{
			throw new ObjectDisposedException("SystemUsageMonitor");
		}
	}

	private void RefreshLoopAsync(CancellationToken cancellationToken)
	{
		while (!cancellationToken.IsCancellationRequested)
		{
			if (!watch.IsRunning)
			{
				watch.Start();
			}
			SystemUsageLoad? systemUsageLoad = null;
			foreach (SystemUsageRecorder value in recorders.Values)
			{
				if (value.IsEligibleForRecording(watch))
				{
					if (!systemUsageLoad.HasValue)
					{
						DateTime utcNow = DateTime.UtcNow;
						systemUsageLoad = new SystemUsageLoad(utcNow, ThreadInformation.Get(), systemUtilizationReader.GetSystemWideCpuUsage(), systemUtilizationReader.GetSystemWideMemoryAvailabilty(), Connection.NumberOfOpenTcpConnections);
					}
					value.RecordUsage(systemUsageLoad.Value, watch);
				}
			}
			Thread.Sleep(pollDelayInMilliSeconds);
		}
	}
}
