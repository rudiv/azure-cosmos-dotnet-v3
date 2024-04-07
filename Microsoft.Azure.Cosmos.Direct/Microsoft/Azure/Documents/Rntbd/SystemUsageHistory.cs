using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.Azure.Documents.Rntbd;

internal class SystemUsageHistory
{
	private readonly TimeSpan monitoringInterval;

	private readonly Lazy<string> loadHistory;

	private readonly Lazy<bool?> cpuHigh;

	private readonly Lazy<bool?> cpuThreadStarvation;

	internal ReadOnlyCollection<SystemUsageLoad> Values { get; }

	internal DateTime LastTimestamp { get; }

	public bool? IsCpuHigh => cpuHigh.Value;

	public bool? IsCpuThreadStarvation => cpuThreadStarvation.Value;

	public SystemUsageHistory(ReadOnlyCollection<SystemUsageLoad> data, TimeSpan monitoringInterval)
	{
		Values = data ?? throw new ArgumentNullException("data");
		if (Values.Count > 0)
		{
			LastTimestamp = Values.Last().Timestamp;
		}
		else
		{
			LastTimestamp = DateTime.MinValue;
		}
		loadHistory = new Lazy<string>(delegate
		{
			if (Values == null || Values.Count == 0)
			{
				return "{\"systemHistory\":\"Empty\"}";
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("{\"systemHistory\":[");
			foreach (SystemUsageLoad value in Values)
			{
				value.AppendJsonString(stringBuilder);
				stringBuilder.Append(",");
			}
			stringBuilder.Length--;
			stringBuilder.Append("]}");
			return stringBuilder.ToString();
		});
		if (monitoringInterval <= TimeSpan.Zero)
		{
			throw new ArgumentOutOfRangeException("monitoringInterval", monitoringInterval, string.Format("{0} must be strictly positive", "monitoringInterval"));
		}
		this.monitoringInterval = monitoringInterval;
		cpuHigh = new Lazy<bool?>(GetCpuHigh, LazyThreadSafetyMode.ExecutionAndPublication);
		cpuThreadStarvation = new Lazy<bool?>(GetCpuThreadStarvation, LazyThreadSafetyMode.ExecutionAndPublication);
	}

	public override string ToString()
	{
		return loadHistory.Value;
	}

	public void AppendJsonString(StringBuilder stringBuilder)
	{
		stringBuilder.Append(ToString());
	}

	private bool? GetCpuHigh()
	{
		if (Values.Count == 0)
		{
			return null;
		}
		bool? result = null;
		foreach (SystemUsageLoad value in Values)
		{
			if (value.CpuUsage.HasValue)
			{
				if ((double)value.CpuUsage.Value > 90.0)
				{
					return true;
				}
				result = false;
			}
		}
		return result;
	}

	private bool? GetCpuThreadStarvation()
	{
		if (Values.Count == 0)
		{
			return null;
		}
		bool? result = null;
		foreach (SystemUsageLoad value in Values)
		{
			if (value.ThreadInfo != null && value.ThreadInfo.IsThreadStarving.HasValue)
			{
				if (value.ThreadInfo.IsThreadStarving.Value)
				{
					return true;
				}
				result = false;
			}
		}
		return result;
	}
}
