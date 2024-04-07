using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Microsoft.Azure.Documents;

internal static class PlatformApis
{
	private class DistroInfo
	{
		public string Id;

		public string VersionId;
	}

	private static readonly Lazy<Platform> _platform = new Lazy<Platform>(DetermineOSPlatform);

	private static readonly Lazy<DistroInfo> _distroInfo = new Lazy<DistroInfo>(LoadDistroInfo);

	public static string GetOSName()
	{
		return GetOSPlatform() switch
		{
			Platform.Windows => "Windows", 
			Platform.Linux => GetDistroId() ?? "Linux", 
			Platform.Darwin => "Mac OS X", 
			_ => "Unknown", 
		};
	}

	public static string GetOSVersion()
	{
		try
		{
			return GetOSPlatform() switch
			{
				Platform.Windows => GetWindowsVersion(RuntimeInformation.OSDescription) ?? string.Empty, 
				Platform.Linux => GetDistroVersionId() ?? string.Empty, 
				Platform.Darwin => GetDarwinVersion() ?? string.Empty, 
				_ => string.Empty, 
			};
		}
		catch
		{
			return string.Empty;
		}
	}

	public static string GetWindowsVersion(string osDescipiton)
	{
		return new Regex("([0-9]+\\.*)+").Match(osDescipiton).ToString();
	}

	private static string GetDarwinVersion()
	{
		if (!Version.TryParse(NativeMethods.Darwin.GetKernelRelease(), out Version result) || result.Major < 5)
		{
			return "10.0";
		}
		return $"10.{result.Major - 4}";
	}

	public static Platform GetOSPlatform()
	{
		return _platform.Value;
	}

	private static string GetDistroId()
	{
		return _distroInfo.Value?.Id;
	}

	private static string GetDistroVersionId()
	{
		return _distroInfo.Value?.VersionId;
	}

	private static DistroInfo LoadDistroInfo()
	{
		if (File.Exists("/etc/os-release"))
		{
			string[] array = File.ReadAllLines("/etc/os-release");
			DistroInfo distroInfo = new DistroInfo();
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (text.StartsWith("ID=", StringComparison.Ordinal))
				{
					distroInfo.Id = text.Substring(3).Trim('"', '\'');
				}
				else if (text.StartsWith("VERSION_ID=", StringComparison.Ordinal))
				{
					distroInfo.VersionId = text.Substring(11).Trim('"', '\'');
				}
			}
			return distroInfo;
		}
		string id = (File.Exists("/proc/sys/kernel/ostype") ? File.ReadAllText("/proc/sys/kernel/ostype") : null);
		string versionId = (File.Exists("/proc/sys/kernel/osrelease") ? File.ReadAllText("/proc/sys/kernel/osrelease") : null);
		return new DistroInfo
		{
			Id = id,
			VersionId = versionId
		};
	}

	private static Platform DetermineOSPlatform()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			return Platform.Windows;
		}
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			return Platform.Linux;
		}
		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			return Platform.Darwin;
		}
		return Platform.Unknown;
	}
}
