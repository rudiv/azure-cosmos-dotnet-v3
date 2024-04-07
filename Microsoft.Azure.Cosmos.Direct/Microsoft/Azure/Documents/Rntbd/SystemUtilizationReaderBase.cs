using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Azure.Cosmos.Core.Trace;

namespace Microsoft.Azure.Documents.Rntbd;

internal abstract class SystemUtilizationReaderBase
{
	private float cachedCpuUtilization = float.NaN;

	private long lastCpuUsageReadTimeTicks = DateTime.MinValue.Ticks;

	private static readonly Lazy<SystemUtilizationReaderBase> singletonInstance = new Lazy<SystemUtilizationReaderBase>(Create, LazyThreadSafetyMode.ExecutionAndPublication);

	private static SystemUtilizationReaderBase singletonOverride;

	public static SystemUtilizationReaderBase SingletonInstance
	{
		get
		{
			SystemUtilizationReaderBase result;
			if ((result = singletonOverride) != null)
			{
				return result;
			}
			return singletonInstance.Value;
		}
	}

	internal static void ApplySingletonOverride(SystemUtilizationReaderBase readerOverride)
	{
		singletonOverride = readerOverride;
	}

	public float GetSystemWideCpuUsageCached(TimeSpan cacheEvictionTimeInSeconds)
	{
		long num = Volatile.Read(ref lastCpuUsageReadTimeTicks);
		long timestamp = Stopwatch.GetTimestamp();
		if (timestamp - num >= cacheEvictionTimeInSeconds.Ticks && Interlocked.CompareExchange(ref lastCpuUsageReadTimeTicks, timestamp, num) == num)
		{
			Volatile.Write(ref cachedCpuUtilization, GetSystemWideCpuUsage());
		}
		return Volatile.Read(ref cachedCpuUtilization);
	}

	[SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Intentional catch-all-rethrow here t log exception")]
	public float GetSystemWideCpuUsage()
	{
		try
		{
			return GetSystemWideCpuUsageCore();
		}
		catch (Exception ex)
		{
			DefaultTrace.TraceError("Reading the system-wide CPU usage failed. Exception: {0}", ex);
			return float.NaN;
		}
	}

	public long? GetSystemWideMemoryAvailabilty()
	{
		try
		{
			return GetSystemWideMemoryAvailabiltyCore();
		}
		catch (Exception ex)
		{
			DefaultTrace.TraceError("Reading the system-wide Memory availability failed. Exception: {0}", ex);
			return null;
		}
	}

	private static SystemUtilizationReaderBase Create()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			return new WindowsSystemUtilizationReader();
		}
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			return new LinuxSystemUtilizationReader();
		}
		return new UnsupportedSystemUtilizationReader();
	}

	protected abstract float GetSystemWideCpuUsageCore();

	protected abstract long? GetSystemWideMemoryAvailabiltyCore();
}
