using System;
using System.Threading;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Rntbd;

namespace Microsoft.Azure.Documents;

internal sealed class TransportAddressUri : IEquatable<TransportAddressUri>
{
	private static readonly TimeSpan idleTimeInMinutes = TimeSpan.FromMinutes(1.0);

	private readonly string uriToString;

	private DateTime? lastFailedRequestUtc;

	private TransportAddressHealthState healthState;

	public Uri Uri { get; }

	public string PathAndQuery { get; }

	public ServerKey ReplicaServerKey { get; }

	public TransportAddressUri(Uri addressUri)
	{
		Uri = addressUri ?? throw new ArgumentNullException("addressUri");
		ReplicaServerKey = new ServerKey(addressUri);
		uriToString = addressUri.ToString();
		PathAndQuery = addressUri.PathAndQuery.TrimEnd(TransportSerialization.UrlTrim);
		healthState = new TransportAddressHealthState(addressUri, TransportAddressHealthState.HealthStatus.Unknown, DateTime.UtcNow, null, null);
		lastFailedRequestUtc = null;
	}

	public bool IsUnhealthy()
	{
		DateTime? dateTime = lastFailedRequestUtc;
		if (!dateTime.HasValue || !dateTime.HasValue)
		{
			return false;
		}
		if (dateTime.Value + idleTimeInMinutes > DateTime.UtcNow)
		{
			return true;
		}
		lastFailedRequestUtc = null;
		return false;
	}

	public void SetUnhealthy()
	{
		TransportAddressHealthState previousState = healthState;
		SetHealthStatus(previousState, TransportAddressHealthState.HealthStatus.Unhealthy);
		lastFailedRequestUtc = DateTime.UtcNow;
	}

	public void SetConnected()
	{
		TransportAddressHealthState transportAddressHealthState = healthState;
		if (transportAddressHealthState.GetHealthStatus() != TransportAddressHealthState.HealthStatus.Connected)
		{
			SetHealthStatus(transportAddressHealthState, TransportAddressHealthState.HealthStatus.Connected);
		}
	}

	public void SetRefreshedIfUnhealthy()
	{
		TransportAddressHealthState transportAddressHealthState = healthState;
		if (transportAddressHealthState.GetHealthStatus() == TransportAddressHealthState.HealthStatus.Unhealthy)
		{
			SetHealthStatus(transportAddressHealthState, TransportAddressHealthState.HealthStatus.UnhealthyPending);
		}
	}

	public override int GetHashCode()
	{
		return Uri.GetHashCode();
	}

	public override string ToString()
	{
		return uriToString;
	}

	public bool Equals(TransportAddressUri other)
	{
		if (this == other)
		{
			return true;
		}
		return Uri.Equals(other?.Uri);
	}

	public override bool Equals(object obj)
	{
		if (this != obj)
		{
			if (obj is TransportAddressUri other)
			{
				return Equals(other);
			}
			return false;
		}
		return true;
	}

	public void ResetHealthStatus(TransportAddressHealthState.HealthStatus status, DateTime? lastUnknownTimestamp, DateTime? lastUnhealthyPendingTimestamp, DateTime? lastUnhealthyTimestamp)
	{
		CreateAndUpdateCurrentHealthState(status, lastUnknownTimestamp, lastUnhealthyPendingTimestamp, lastUnhealthyTimestamp, healthState);
	}

	public TransportAddressHealthState GetCurrentHealthState()
	{
		return healthState;
	}

	public TransportAddressHealthState.HealthStatus GetEffectiveHealthStatus()
	{
		TransportAddressHealthState transportAddressHealthState = healthState;
		switch (transportAddressHealthState.GetHealthStatus())
		{
		case TransportAddressHealthState.HealthStatus.Connected:
		case TransportAddressHealthState.HealthStatus.Unhealthy:
			return transportAddressHealthState.GetHealthStatus();
		case TransportAddressHealthState.HealthStatus.Unknown:
		{
			DateTime utcNow = DateTime.UtcNow;
			DateTime? dateTime2 = transportAddressHealthState.GetLastKnownTimestampByHealthStatus(TransportAddressHealthState.HealthStatus.Unknown) + idleTimeInMinutes;
			if (utcNow > dateTime2)
			{
				return TransportAddressHealthState.HealthStatus.Connected;
			}
			return transportAddressHealthState.GetHealthStatus();
		}
		case TransportAddressHealthState.HealthStatus.UnhealthyPending:
		{
			DateTime utcNow = DateTime.UtcNow;
			DateTime? dateTime = transportAddressHealthState.GetLastKnownTimestampByHealthStatus(TransportAddressHealthState.HealthStatus.UnhealthyPending) + idleTimeInMinutes;
			if (utcNow > dateTime)
			{
				return TransportAddressHealthState.HealthStatus.Connected;
			}
			return transportAddressHealthState.GetHealthStatus();
		}
		default:
			throw new ArgumentException($"Unknown status :{transportAddressHealthState.GetHealthStatus()}");
		}
	}

	public bool ShouldRefreshHealthStatus()
	{
		TransportAddressHealthState transportAddressHealthState = healthState;
		if (transportAddressHealthState.GetHealthStatus() == TransportAddressHealthState.HealthStatus.Unhealthy)
		{
			DateTime utcNow = DateTime.UtcNow;
			DateTime? dateTime = transportAddressHealthState.GetLastKnownTimestampByHealthStatus(TransportAddressHealthState.HealthStatus.Unhealthy) + idleTimeInMinutes;
			return utcNow >= dateTime;
		}
		return false;
	}

	private void SetHealthStatus(TransportAddressHealthState previousState, TransportAddressHealthState.HealthStatus status)
	{
		switch (status)
		{
		case TransportAddressHealthState.HealthStatus.Unhealthy:
			CreateAndUpdateCurrentHealthState(TransportAddressHealthState.HealthStatus.Unhealthy, previousState.GetLastKnownTimestampByHealthStatus(TransportAddressHealthState.HealthStatus.Unknown), previousState.GetLastKnownTimestampByHealthStatus(TransportAddressHealthState.HealthStatus.UnhealthyPending), DateTime.UtcNow, previousState);
			break;
		case TransportAddressHealthState.HealthStatus.UnhealthyPending:
			if (previousState.GetHealthStatus() == TransportAddressHealthState.HealthStatus.Unhealthy || previousState.GetHealthStatus() == TransportAddressHealthState.HealthStatus.UnhealthyPending)
			{
				CreateAndUpdateCurrentHealthState(TransportAddressHealthState.HealthStatus.UnhealthyPending, previousState.GetLastKnownTimestampByHealthStatus(TransportAddressHealthState.HealthStatus.Unknown), DateTime.UtcNow, previousState.GetLastKnownTimestampByHealthStatus(TransportAddressHealthState.HealthStatus.Unhealthy), previousState);
			}
			break;
		case TransportAddressHealthState.HealthStatus.Connected:
			CreateAndUpdateCurrentHealthState(TransportAddressHealthState.HealthStatus.Connected, previousState.GetLastKnownTimestampByHealthStatus(TransportAddressHealthState.HealthStatus.Unknown), previousState.GetLastKnownTimestampByHealthStatus(TransportAddressHealthState.HealthStatus.UnhealthyPending), previousState.GetLastKnownTimestampByHealthStatus(TransportAddressHealthState.HealthStatus.Unhealthy), previousState);
			break;
		default:
			throw new ArgumentException($"Cannot set an unsupported health status: {status}");
		}
	}

	private void CreateAndUpdateCurrentHealthState(TransportAddressHealthState.HealthStatus healthStatus, DateTime? lastUnknownTimestamp, DateTime? lastUnhealthyPendingTimestamp, DateTime? lastUnhealthyTimestamp, TransportAddressHealthState previousState)
	{
		TransportAddressHealthState transportAddressHealthState = previousState;
		TransportAddressHealthState transportAddressHealthState2 = new TransportAddressHealthState(Uri, healthStatus, lastUnknownTimestamp, lastUnhealthyPendingTimestamp, lastUnhealthyTimestamp);
		while (true)
		{
			TransportAddressHealthState transportAddressHealthState3 = Interlocked.CompareExchange(ref healthState, transportAddressHealthState2, transportAddressHealthState);
			if (transportAddressHealthState3 != transportAddressHealthState && transportAddressHealthState3 != transportAddressHealthState2)
			{
				transportAddressHealthState = transportAddressHealthState3;
				DefaultTrace.TraceVerbose("Re-attempting to update the current health state. Previous health status: {0}, current health status: {1}", previousState.GetHealthStatus(), transportAddressHealthState3.GetHealthStatus());
				continue;
			}
			break;
		}
	}
}
