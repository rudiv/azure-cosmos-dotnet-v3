using System.Runtime.InteropServices;

namespace Microsoft.Azure.Documents.Rntbd;

internal sealed class WindowsSystemUtilizationReader : SystemUtilizationReaderBase
{
	private static class NativeMethods
	{
		internal struct MemoryInfo
		{
			internal uint dwLength;

			internal uint dwMemoryLoad;

			internal ulong ullTotalPhys;

			internal ulong ullAvailPhys;

			internal ulong ullTotalPageFile;

			internal ulong ullAvailPageFile;

			internal ulong ullTotalVirtual;

			internal ulong ullAvailVirtual;

			internal ulong ullAvailExtendedVirtual;
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool GetSystemTimes(out long idle, out long kernel, out long user);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool GlobalMemoryStatusEx(out MemoryInfo memInfo);
	}

	private long lastIdleTime;

	private long lastKernelTime;

	private long lastUserTime;

	public WindowsSystemUtilizationReader()
	{
		lastIdleTime = 0L;
		lastKernelTime = 0L;
		lastUserTime = 0L;
	}

	protected override float GetSystemWideCpuUsageCore()
	{
		if (!NativeMethods.GetSystemTimes(out var idle, out var kernel, out var user))
		{
			return float.NaN;
		}
		long num = idle - lastIdleTime;
		long num2 = kernel - lastKernelTime;
		long num3 = user - lastUserTime;
		lastIdleTime = idle;
		lastUserTime = user;
		lastKernelTime = kernel;
		long num4 = num3 + num2;
		if (num4 == 0L)
		{
			return float.NaN;
		}
		long num5 = num3 + num2 - num;
		return (float)(100 * num5) / (float)num4;
	}

	protected override long? GetSystemWideMemoryAvailabiltyCore()
	{
		NativeMethods.MemoryInfo memInfo = default(NativeMethods.MemoryInfo);
		memInfo.dwLength = (uint)Marshal.SizeOf(memInfo);
		NativeMethods.GlobalMemoryStatusEx(out memInfo);
		return (long)memInfo.ullAvailPhys / 1024L;
	}
}
