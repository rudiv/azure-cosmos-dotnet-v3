using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents;

internal sealed class TransportAddressHealthState
{
	public enum HealthStatus
	{
		Connected = 100,
		Unknown = 200,
		UnhealthyPending = 300,
		Unhealthy = 400
	}

	private readonly DateTime? lastUnknownTimestamp;

	private readonly DateTime? lastUnhealthyPendingTimestamp;

	private readonly DateTime? lastUnhealthyTimestamp;

	private readonly HealthStatus healthStatus;

	private readonly string healthStatusDiagnosticString;

	private readonly IReadOnlyList<string> healthStatusDiagnosticEnumerable;

	public TransportAddressHealthState(Uri transportUri, HealthStatus healthStatus, DateTime? lastUnknownTimestamp, DateTime? lastUnhealthyPendingTimestamp, DateTime? lastUnhealthyTimestamp)
	{
		if ((object)transportUri == null)
		{
			throw new ArgumentNullException("transportUri", "Argument transportUri can not be null");
		}
		this.healthStatus = healthStatus;
		this.lastUnknownTimestamp = lastUnknownTimestamp;
		this.lastUnhealthyPendingTimestamp = lastUnhealthyPendingTimestamp;
		this.lastUnhealthyTimestamp = lastUnhealthyTimestamp;
		healthStatusDiagnosticString = $"(port: {transportUri.Port} | status: {healthStatus} | lkt: {GetLastKnownTimestampByHealthStatus(healthStatus)})";
		List<string> list = new List<string> { healthStatusDiagnosticString };
		healthStatusDiagnosticEnumerable = list.AsReadOnly();
	}

	public HealthStatus GetHealthStatus()
	{
		return healthStatus;
	}

	public string GetHealthStatusDiagnosticString()
	{
		return healthStatusDiagnosticString;
	}

	public IEnumerable<string> GetHealthStatusDiagnosticsAsReadOnlyEnumerable()
	{
		return healthStatusDiagnosticEnumerable;
	}

	internal DateTime? GetLastKnownTimestampByHealthStatus(HealthStatus healthStatus)
	{
		return healthStatus switch
		{
			HealthStatus.Unhealthy => lastUnhealthyTimestamp, 
			HealthStatus.UnhealthyPending => lastUnhealthyPendingTimestamp, 
			HealthStatus.Unknown => lastUnknownTimestamp, 
			HealthStatus.Connected => DateTime.UtcNow, 
			_ => throw new ArgumentException($"Unsupported Health Status: {healthStatus}"), 
		};
	}
}
