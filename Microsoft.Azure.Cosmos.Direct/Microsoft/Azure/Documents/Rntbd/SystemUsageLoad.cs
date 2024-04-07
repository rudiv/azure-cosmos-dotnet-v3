using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Azure.Documents.Rntbd;

internal struct SystemUsageLoad
{
	public readonly DateTime Timestamp;

	public readonly float? CpuUsage;

	public readonly long? MemoryAvailable;

	public readonly ThreadInformation ThreadInfo;

	public readonly int? NumberOfOpenTcpConnections;

	public SystemUsageLoad(DateTime timestamp, ThreadInformation threadInfo, float? cpuUsage = null, long? memoryAvailable = null, int? numberOfOpenTcpConnection = 0)
	{
		Timestamp = timestamp;
		CpuUsage = cpuUsage;
		MemoryAvailable = memoryAvailable;
		ThreadInfo = threadInfo ?? throw new ArgumentNullException("Thread Information can not be null");
		NumberOfOpenTcpConnections = numberOfOpenTcpConnection;
	}

	public void AppendJsonString(StringBuilder stringBuilder)
	{
		stringBuilder.Append("{\"dateUtc\":\"");
		DateTime timestamp = Timestamp;
		stringBuilder.Append(timestamp.ToString("O", CultureInfo.InvariantCulture));
		stringBuilder.Append("\",\"cpu\":");
		stringBuilder.Append((CpuUsage.HasValue && !float.IsNaN(CpuUsage.Value)) ? CpuUsage.Value.ToString("F3", CultureInfo.InvariantCulture) : "\"no info\"");
		stringBuilder.Append(",\"memory\":");
		stringBuilder.Append(MemoryAvailable.HasValue ? MemoryAvailable.Value.ToString("F3", CultureInfo.InvariantCulture) : "\"no info\"");
		stringBuilder.Append(",\"threadInfo\":");
		if (ThreadInfo != null)
		{
			ThreadInfo.AppendJsonString(stringBuilder);
		}
		else
		{
			stringBuilder.Append("\"no info\"");
		}
		stringBuilder.Append(",\"numberOfOpenTcpConnection\":");
		stringBuilder.Append(NumberOfOpenTcpConnections.HasValue ? NumberOfOpenTcpConnections.Value.ToString(CultureInfo.InvariantCulture) : "\"no info\"");
		stringBuilder.Append("}");
	}

	public override string ToString()
	{
		return string.Format(CultureInfo.InvariantCulture, "({0:O} => CpuUsage :{1:F3}, MemoryAvailable :{2:F3} {3:F3}, NumberOfOpenTcpConnection : {4} )", Timestamp, CpuUsage, MemoryAvailable, ThreadInfo.ToString(), NumberOfOpenTcpConnections);
	}
}
