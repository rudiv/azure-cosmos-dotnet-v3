using System;
using System.Globalization;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Rntbd;

namespace Microsoft.Azure.Documents;

internal sealed class ConnectionHealthChecker
{
	private const int MinNumberOfSendsSinceLastReceiveForUnhealthyConnection = 3;

	private const bool AggressiveTimeoutDetectionEnabledDefaultValue = false;

	private const int TimeoutDetectionTimeLimitDefaultValueInSeconds = 60;

	private const int TimeoutDetectionOnWriteThresholdDefaultValue = 1;

	private const int TimeoutDetectionOnWriteTimeLimitDefaultValueInSeconds = 6;

	private const int TimeoutDetectionOnHighFrequencyThresholdDefaultValue = 3;

	private const int TimeoutDetectionOnHighFrequencyTimeLimitDefaultValueInSeconds = 10;

	private const int TimeoutDetectionDisabledOnCPUThresholdDefaultValue = 90;

	private static readonly TimeSpan TimeoutDetectionCPUUsageCacheEvictionTimeInSeconds = TimeSpan.FromSeconds(10.0);

	private static readonly TimeSpan sendHangGracePeriod = TimeSpan.FromSeconds(2.0);

	private static readonly TimeSpan receiveHangGracePeriod = TimeSpan.FromSeconds(10.0);

	private static readonly TimeSpan recentReceiveWindow = TimeSpan.FromSeconds(1.0);

	private static readonly byte[] healthCheckBuffer = new byte[1];

	private readonly TimeSpan receiveDelayLimit;

	private readonly TimeSpan sendDelayLimit;

	private readonly TimeSpan idleConnectionTimeout;

	private readonly bool aggressiveTimeoutDetectionEnabled;

	private readonly TimeSpan timeoutDetectionTimeLimit;

	private readonly int timeoutDetectionOnWriteThreshold;

	private readonly TimeSpan timeoutDetectionOnWriteTimeLimit;

	private readonly int timeoutDetectionOnHighFrequencyThreshold;

	private readonly TimeSpan timeoutDetectionOnHighFrequencyTimeLimit;

	private readonly double timeoutDetectionDisableCPUThreshold;

	private readonly SystemUtilizationReaderBase systemUtilizationReader;

	private int transitTimeoutOnReadCounter;

	private int transitTimeoutOnWriteCounter;

	public ConnectionHealthChecker(TimeSpan sendDelayLimit, TimeSpan receiveDelayLimit, TimeSpan idleConnectionTimeout)
	{
		if (receiveDelayLimit <= receiveHangGracePeriod)
		{
			throw new ArgumentOutOfRangeException("receiveDelayLimit", receiveDelayLimit, string.Format(CultureInfo.InvariantCulture, "{0} must be greater than {1} ({2})", "receiveDelayLimit", "receiveHangGracePeriod", receiveHangGracePeriod));
		}
		if (sendDelayLimit <= sendHangGracePeriod)
		{
			throw new ArgumentOutOfRangeException("sendDelayLimit", sendDelayLimit, string.Format(CultureInfo.InvariantCulture, "{0} must be greater than {1} ({2})", "sendDelayLimit", "sendHangGracePeriod", sendHangGracePeriod));
		}
		this.sendDelayLimit = sendDelayLimit;
		this.receiveDelayLimit = receiveDelayLimit;
		this.idleConnectionTimeout = idleConnectionTimeout;
		transitTimeoutOnWriteCounter = 0;
		transitTimeoutOnReadCounter = 0;
		aggressiveTimeoutDetectionEnabled = Helpers.GetEnvironmentVariable("AZURE_COSMOS_AGGRESSIVE_TIMEOUT_DETECTION_ENABLED", defaultValue: false);
		if (aggressiveTimeoutDetectionEnabled)
		{
			systemUtilizationReader = SystemUtilizationReaderBase.SingletonInstance;
			timeoutDetectionTimeLimit = TimeSpan.FromSeconds(Helpers.GetEnvironmentVariable("AZURE_COSMOS_TIMEOUT_DETECTION_TIME_LIMIT_IN_SECONDS", 60));
			timeoutDetectionOnWriteThreshold = Helpers.GetEnvironmentVariable("AZURE_COSMOS_TIMEOUT_DETECTION_ON_WRITE_THRESHOLD", 1);
			timeoutDetectionOnWriteTimeLimit = TimeSpan.FromSeconds(Helpers.GetEnvironmentVariable("AZURE_COSMOS_TIMEOUT_DETECTION_ON_WRITE_TIME_LIMIT_IN_SECONDS", 6));
			timeoutDetectionOnHighFrequencyThreshold = Helpers.GetEnvironmentVariable("AZURE_COSMOS_TIMEOUT_DETECTION_ON_HIGH_FREQUENCY_THRESHOLD", 3);
			timeoutDetectionOnHighFrequencyTimeLimit = TimeSpan.FromSeconds(Helpers.GetEnvironmentVariable("AZURE_COSMOS_TIMEOUT_DETECTION_ON_HIGH_FREQUENCY_TIME_LIMIT_IN_SECONDS", 10));
			timeoutDetectionDisableCPUThreshold = Helpers.GetEnvironmentVariable("AZURE_COSMOS_TIMEOUT_DETECTION_DISABLED_ON_CPU_THRESHOLD", 90);
		}
	}

	public bool IsHealthy(DateTime currentTime, DateTime lastSendAttempt, DateTime lastSend, DateTime lastReceive, DateTime? firstSendSinceLastReceive, long numberOfSendsSinceLastReceive, Socket socket)
	{
		if (IsDataReceivedRecently(currentTime, lastReceive))
		{
			return true;
		}
		if (IsBlackholeDetected(currentTime, lastSendAttempt, lastSend, lastReceive, firstSendSinceLastReceive, numberOfSendsSinceLastReceive))
		{
			return false;
		}
		if (IsConnectionIdled(currentTime, lastReceive))
		{
			return false;
		}
		if (IsTransitTimeoutsDetected(currentTime, lastReceive))
		{
			return false;
		}
		return IsSocketConnectionEstablished(socket);
	}

	internal void UpdateTransitTimeoutCounters(bool isCompleted, bool isReadReqeust)
	{
		if (isCompleted)
		{
			ResetTransitTimeoutCounters();
		}
		else if (isReadReqeust)
		{
			IncrementTransitTimeoutOnReadCounter();
		}
		else
		{
			IncrementTransitTimeoutOnWriteCounter();
		}
	}

	private void IncrementTransitTimeoutOnWriteCounter()
	{
		Interlocked.Increment(ref transitTimeoutOnWriteCounter);
	}

	private void IncrementTransitTimeoutOnReadCounter()
	{
		Interlocked.Increment(ref transitTimeoutOnReadCounter);
	}

	private void ResetTransitTimeoutCounters()
	{
		Interlocked.Exchange(ref transitTimeoutOnReadCounter, 0);
		Interlocked.Exchange(ref transitTimeoutOnWriteCounter, 0);
	}

	private bool IsTransitTimeoutsDetected(DateTime currentTime, DateTime lastReceiveTime)
	{
		if (!aggressiveTimeoutDetectionEnabled)
		{
			return false;
		}
		SnapshotTransitTimeoutCounters(out var totalTransitTimeoutCounter, out var num);
		if (totalTransitTimeoutCounter == 0)
		{
			return false;
		}
		TimeSpan timeSpan = currentTime - lastReceiveTime;
		if (totalTransitTimeoutCounter > 0 && timeSpan >= timeoutDetectionTimeLimit)
		{
			DefaultTrace.TraceWarning("Unhealthy RNTBD connection: Health check failed due to transit timeout detection time limit exceeded. " + $"Last channel receive: {lastReceiveTime}. Timeout detection time limit: {timeoutDetectionTimeLimit}.");
			return IsCpuUtilizationBelowDisableTimeoutDetectionThreshold();
		}
		if (totalTransitTimeoutCounter >= timeoutDetectionOnHighFrequencyThreshold && timeSpan >= timeoutDetectionOnHighFrequencyTimeLimit)
		{
			DefaultTrace.TraceWarning("Unhealthy RNTBD connection: Health check failed due to transit timeout high frequency threshold hit. " + $"Last channel receive: {lastReceiveTime}. Timeout counts: {totalTransitTimeoutCounter}. " + $"Timeout detection high frequency threshold: {timeoutDetectionOnHighFrequencyThreshold}. Timeout detection high frequency time limit: {timeoutDetectionOnHighFrequencyTimeLimit}.");
			return IsCpuUtilizationBelowDisableTimeoutDetectionThreshold();
		}
		if (num >= timeoutDetectionOnWriteThreshold && timeSpan >= timeoutDetectionOnWriteTimeLimit)
		{
			DefaultTrace.TraceWarning("Unhealthy RNTBD connection: Health check failed due to transit timeout on write threshold hit: {0}. " + $"Last channel receive: {lastReceiveTime}. Write timeout counts: {num}. " + $"Timeout detection on write threshold: {timeoutDetectionOnWriteThreshold}. Timeout detection on write time limit: {timeoutDetectionOnWriteTimeLimit}.");
			return IsCpuUtilizationBelowDisableTimeoutDetectionThreshold();
		}
		return false;
	}

	private bool IsBlackholeDetected(DateTime currentTime, DateTime lastSendAttempt, DateTime lastSend, DateTime lastReceive, DateTime? firstSendSinceLastReceive, long numberOfSendsSinceLastReceive)
	{
		if (lastSendAttempt - lastSend > sendDelayLimit && currentTime - lastSendAttempt > sendHangGracePeriod)
		{
			DefaultTrace.TraceWarning("Unhealthy RNTBD connection: Hung send: {0}. Last send attempt: {1:o}. Last send: {2:o}. Tolerance {3:c}");
			return true;
		}
		if (lastSend - lastReceive > receiveDelayLimit)
		{
			if (currentTime - lastSend > receiveHangGracePeriod)
			{
				goto IL_00b8;
			}
			if (numberOfSendsSinceLastReceive >= 3 && firstSendSinceLastReceive.HasValue)
			{
				DateTime? dateTime = firstSendSinceLastReceive;
				if (currentTime - dateTime > receiveHangGracePeriod)
				{
					goto IL_00b8;
				}
			}
		}
		return false;
		IL_00b8:
		DefaultTrace.TraceWarning("Unhealthy RNTBD connection: Replies not getting back: {0}. Last send: {1:o}. Last receive: {2:o}. Tolerance: {3:c}. First send since last receieve: {4:o}. # of sends since last receive: {5}");
		return true;
	}

	private static bool IsDataReceivedRecently(DateTime currentTime, DateTime lastReceiveTime)
	{
		return currentTime - lastReceiveTime < recentReceiveWindow;
	}

	private static bool IsSocketConnectionEstablished(Socket socket)
	{
		try
		{
			if (socket == null || !socket.Connected)
			{
				return false;
			}
			socket.Send(healthCheckBuffer, 0, SocketFlags.None);
			return true;
		}
		catch (SocketException ex)
		{
			bool flag = ex.SocketErrorCode == SocketError.WouldBlock;
			if (!flag)
			{
				DefaultTrace.TraceWarning("Unhealthy RNTBD connection. Socket error code: {0}", ex.SocketErrorCode.ToString());
			}
			return flag;
		}
		catch (ObjectDisposedException)
		{
			return false;
		}
	}

	private bool IsConnectionIdled(DateTime currentTime, DateTime lastReceive)
	{
		if (idleConnectionTimeout > TimeSpan.Zero)
		{
			return currentTime - lastReceive > idleConnectionTimeout;
		}
		return false;
	}

	private bool IsCpuUtilizationBelowDisableTimeoutDetectionThreshold()
	{
		return (double)systemUtilizationReader.GetSystemWideCpuUsageCached(TimeoutDetectionCPUUsageCacheEvictionTimeInSeconds) <= timeoutDetectionDisableCPUThreshold;
	}

	private void SnapshotTransitTimeoutCounters(out int totalTransitTimeoutCounter, out int transitTimeoutOnWriteCounter)
	{
		totalTransitTimeoutCounter = this.transitTimeoutOnWriteCounter + transitTimeoutOnReadCounter;
		transitTimeoutOnWriteCounter = this.transitTimeoutOnWriteCounter;
	}
}
