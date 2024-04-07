using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Microsoft.Azure.Documents;

internal static class VersionUtility
{
	private const string versionDateTimeFormat = "yyyy-MM-dd";

	private const string previewVersionDateTimeFormat = "yyyy-MM-dd-preview";

	private static readonly IReadOnlyDictionary<string, DateTime> KnownDateTimes;

	static VersionUtility()
	{
		Dictionary<string, DateTime> dictionary = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
		KnownDateTimes = new ReadOnlyDictionary<string, DateTime>(dictionary);
		string[] supportedRuntimeAPIVersions = HttpConstants.Versions.SupportedRuntimeAPIVersions;
		foreach (string text in supportedRuntimeAPIVersions)
		{
			if (TryParseApiVersion(text, out var apiVersionDate))
			{
				dictionary[text] = apiVersionDate;
			}
		}
	}

	internal static bool IsLaterThan(string compareVersion, string baseVersion)
	{
		if (IsPreviewApiVersion(baseVersion) && !IsPreviewApiVersion(compareVersion))
		{
			return false;
		}
		if (!TryParseApiVersion(baseVersion, out var apiVersionDate))
		{
			throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidVersionFormat, "base", baseVersion));
		}
		return IsLaterThan(compareVersion, apiVersionDate);
	}

	internal static bool IsValidApiVersion(string apiVersion)
	{
		DateTime apiVersionDate;
		return TryParseApiVersion(apiVersion, out apiVersionDate);
	}

	internal static bool IsPreviewApiVersion(string apiVersion)
	{
		return apiVersion.ToLowerInvariant().Contains("preview");
	}

	internal static bool IsLaterThan(string compareVersion, DateTime baseVersion)
	{
		if (!TryParseApiVersion(compareVersion, out var apiVersionDate))
		{
			throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidVersionFormat, "compare", compareVersion));
		}
		return apiVersionDate.CompareTo(baseVersion) >= 0;
	}

	internal static bool IsLaterThanNotEqualTo(string compareVersion, DateTime baseVersion)
	{
		if (!TryParseApiVersion(compareVersion, out var apiVersionDate))
		{
			throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidVersionFormat, "compare", compareVersion));
		}
		return apiVersionDate.CompareTo(baseVersion) > 0;
	}

	internal static DateTime ParseNonPreviewDateTimeExact(string apiVersion)
	{
		return DateTime.ParseExact(apiVersion, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None);
	}

	private static bool TryParseApiVersion(string apiVersion, out DateTime apiVersionDate)
	{
		if (!KnownDateTimes.TryGetValue(apiVersion, out apiVersionDate))
		{
			return TryParseApiVersionCore(apiVersion, out apiVersionDate);
		}
		return true;
	}

	private static bool TryParseApiVersionCore(string apiVersion, out DateTime apiVersionDate)
	{
		string format = ((!apiVersion.ToLowerInvariant().Contains("preview")) ? "yyyy-MM-dd" : "yyyy-MM-dd-preview");
		return DateTime.TryParseExact(apiVersion, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out apiVersionDate);
	}
}
