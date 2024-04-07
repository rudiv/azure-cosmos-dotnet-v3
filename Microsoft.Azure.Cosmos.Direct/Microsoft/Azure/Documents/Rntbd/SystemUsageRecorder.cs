using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Azure.Documents.Rntbd;

internal sealed class SystemUsageRecorder
{
	internal readonly string identifier;

	private readonly int historyLength;

	internal readonly TimeSpan refreshInterval;

	private readonly Queue<SystemUsageLoad> historyQueue;

	private TimeSpan nextTimeStamp;

	public SystemUsageHistory Data { get; private set; }

	internal SystemUsageRecorder(string identifier, int historyLength, TimeSpan refreshInterval)
	{
		if (string.IsNullOrEmpty(identifier))
		{
			throw new ArgumentException("Identifier can not be null.");
		}
		this.identifier = identifier;
		if (historyLength <= 0)
		{
			throw new ArgumentOutOfRangeException("historyLength can not be less than or equal to zero.");
		}
		this.historyLength = historyLength;
		if (refreshInterval <= TimeSpan.Zero)
		{
			throw new ArgumentException("refreshInterval timespan can not be zero.");
		}
		this.refreshInterval = refreshInterval;
		historyQueue = new Queue<SystemUsageLoad>(this.historyLength);
	}

	internal void RecordUsage(SystemUsageLoad systemUsageLoad, Stopwatch watch)
	{
		nextTimeStamp = watch.Elapsed.Add(refreshInterval);
		Data = new SystemUsageHistory(Collect(systemUsageLoad), refreshInterval);
	}

	private ReadOnlyCollection<SystemUsageLoad> Collect(SystemUsageLoad loadData)
	{
		if (historyQueue.Count == historyLength)
		{
			historyQueue.Dequeue();
		}
		historyQueue.Enqueue(loadData);
		return new ReadOnlyCollection<SystemUsageLoad>(historyQueue.ToList());
	}

	internal bool IsEligibleForRecording(Stopwatch watch)
	{
		return TimeSpan.Compare(watch.Elapsed, nextTimeStamp) >= 0;
	}
}
