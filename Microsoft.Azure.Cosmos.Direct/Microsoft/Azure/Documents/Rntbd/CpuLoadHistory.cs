using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace Microsoft.Azure.Documents.Rntbd;

internal sealed class CpuLoadHistory
{
	private readonly TimeSpan monitoringInterval;

	private readonly Lazy<bool> cpuOverload;

	private readonly Lazy<string> cpuloadHistory;

	internal ReadOnlyCollection<CpuLoad> CpuLoad { get; }

	internal DateTime LastTimestamp => CpuLoad.Last().Timestamp;

	public bool IsCpuOverloaded => cpuOverload.Value;

	public CpuLoadHistory(ReadOnlyCollection<CpuLoad> cpuLoad, TimeSpan monitoringInterval)
	{
		if (cpuLoad == null)
		{
			throw new ArgumentNullException("cpuLoad");
		}
		CpuLoad = cpuLoad;
		cpuloadHistory = new Lazy<string>(delegate
		{
			ReadOnlyCollection<CpuLoad> cpuLoad2 = CpuLoad;
			return (cpuLoad2 != null && cpuLoad2.Count == 0) ? "empty" : string.Join(", ", CpuLoad);
		});
		if (monitoringInterval <= TimeSpan.Zero)
		{
			throw new ArgumentOutOfRangeException("monitoringInterval", monitoringInterval, string.Format("{0} must be strictly positive", "monitoringInterval"));
		}
		this.monitoringInterval = monitoringInterval;
		cpuOverload = new Lazy<bool>(GetCpuOverload, LazyThreadSafetyMode.ExecutionAndPublication);
	}

	public override string ToString()
	{
		return cpuloadHistory.Value;
	}

	private bool GetCpuOverload()
	{
		for (int i = 0; i < CpuLoad.Count; i++)
		{
			if ((double)CpuLoad[i].Value > 90.0)
			{
				return true;
			}
		}
		for (int j = 0; j < CpuLoad.Count - 1; j++)
		{
			double totalMilliseconds = CpuLoad[j + 1].Timestamp.Subtract(CpuLoad[j].Timestamp).TotalMilliseconds;
			TimeSpan timeSpan = monitoringInterval;
			if (totalMilliseconds > 1.5 * timeSpan.TotalMilliseconds)
			{
				return true;
			}
		}
		return false;
	}
}
