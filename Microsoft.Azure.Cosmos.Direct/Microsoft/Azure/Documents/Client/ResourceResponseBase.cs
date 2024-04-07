using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using Microsoft.Azure.Documents.Collections;

namespace Microsoft.Azure.Documents.Client;

internal abstract class ResourceResponseBase : IResourceResponseBase
{
	internal DocumentServiceResponse response;

	private Dictionary<string, long> usageHeaders;

	private Dictionary<string, long> quotaHeaders;

	public long DatabaseQuota => GetMaxQuotaHeader("databases");

	public long DatabaseUsage => GetCurrentQuotaHeader("databases");

	public long CollectionQuota => GetMaxQuotaHeader("collections");

	public long CollectionUsage => GetCurrentQuotaHeader("collections");

	public long UserQuota => GetMaxQuotaHeader("users");

	public long UserUsage => GetCurrentQuotaHeader("users");

	public long PermissionQuota => GetMaxQuotaHeader("permissions");

	public long PermissionUsage => GetCurrentQuotaHeader("permissions");

	public long CollectionSizeQuota => GetMaxQuotaHeader("collectionSize");

	public long CollectionSizeUsage => GetCurrentQuotaHeader("collectionSize");

	public long DocumentQuota => GetMaxQuotaHeader("documentsSize");

	public long DocumentUsage => GetCurrentQuotaHeader("documentsSize");

	public long StoredProceduresQuota => GetMaxQuotaHeader("storedProcedures");

	public long StoredProceduresUsage => GetCurrentQuotaHeader("storedProcedures");

	public long TriggersQuota => GetMaxQuotaHeader("triggers");

	public long TriggersUsage => GetCurrentQuotaHeader("triggers");

	public long UserDefinedFunctionsQuota => GetMaxQuotaHeader("functions");

	public long UserDefinedFunctionsUsage => GetCurrentQuotaHeader("functions");

	internal long DocumentCount => GetCurrentQuotaHeader("documentsCount");

	public string ActivityId => response.Headers["x-ms-activity-id"];

	public string SessionToken => response.Headers["x-ms-session-token"];

	public HttpStatusCode StatusCode => response.StatusCode;

	public string MaxResourceQuota => response.Headers["x-ms-resource-quota"];

	public string CurrentResourceQuotaUsage => response.Headers["x-ms-resource-usage"];

	public Stream ResponseStream => response.ResponseBody;

	public double RequestCharge => Helpers.GetHeaderValueDouble(response.Headers, "x-ms-request-charge", 0.0);

	public bool IsRUPerMinuteUsed
	{
		get
		{
			if (Helpers.GetHeaderValueByte(response.Headers, "x-ms-documentdb-is-ru-per-minute-used", 0) != 0)
			{
				return true;
			}
			return false;
		}
	}

	public NameValueCollection ResponseHeaders => response.ResponseHeaders;

	internal INameValueCollection Headers => response.Headers;

	public string ContentLocation => response.Headers["x-ms-alt-content-path"];

	public long IndexTransformationProgress => Helpers.GetHeaderValueLong(response.Headers, "x-ms-documentdb-collection-index-transformation-progress", -1L);

	public long LazyIndexingProgress => Helpers.GetHeaderValueLong(response.Headers, "x-ms-documentdb-collection-lazy-indexing-progress", -1L);

	public TimeSpan RequestLatency
	{
		get
		{
			if (response.RequestStats == null || !response.RequestStats.RequestLatency.HasValue)
			{
				return TimeSpan.Zero;
			}
			return response.RequestStats.RequestLatency.Value;
		}
	}

	public string RequestDiagnosticsString
	{
		get
		{
			if (response.RequestStats == null)
			{
				return string.Empty;
			}
			return response.RequestStats.ToString();
		}
	}

	internal IClientSideRequestStatistics RequestStatistics => response.RequestStats;

	public ResourceResponseBase()
	{
	}

	internal ResourceResponseBase(DocumentServiceResponse response)
	{
		this.response = response;
		usageHeaders = new Dictionary<string, long>();
		quotaHeaders = new Dictionary<string, long>();
	}

	internal long GetCurrentQuotaHeader(string headerName)
	{
		long value = 0L;
		if (usageHeaders.Count == 0 && !string.IsNullOrEmpty(MaxResourceQuota) && !string.IsNullOrEmpty(CurrentResourceQuotaUsage))
		{
			PopulateQuotaHeader(MaxResourceQuota, CurrentResourceQuotaUsage);
		}
		if (usageHeaders.TryGetValue(headerName, out value))
		{
			return value;
		}
		return 0L;
	}

	internal long GetMaxQuotaHeader(string headerName)
	{
		long value = 0L;
		if (quotaHeaders.Count == 0 && !string.IsNullOrEmpty(MaxResourceQuota) && !string.IsNullOrEmpty(CurrentResourceQuotaUsage))
		{
			PopulateQuotaHeader(MaxResourceQuota, CurrentResourceQuotaUsage);
		}
		if (quotaHeaders.TryGetValue(headerName, out value))
		{
			return value;
		}
		return 0L;
	}

	private void PopulateQuotaHeader(string headerMaxQuota, string headerCurrentUsage)
	{
		string[] array = headerMaxQuota.Split(Constants.Quota.DelimiterChars, StringSplitOptions.RemoveEmptyEntries);
		string[] array2 = headerCurrentUsage.Split(Constants.Quota.DelimiterChars, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			if (string.Equals(array[i], "databases", StringComparison.OrdinalIgnoreCase))
			{
				quotaHeaders.Add("databases", long.Parse(array[i + 1], CultureInfo.InvariantCulture));
				usageHeaders.Add("databases", long.Parse(array2[i + 1], CultureInfo.InvariantCulture));
			}
			else if (string.Equals(array[i], "collections", StringComparison.OrdinalIgnoreCase))
			{
				quotaHeaders.Add("collections", long.Parse(array[i + 1], CultureInfo.InvariantCulture));
				usageHeaders.Add("collections", long.Parse(array2[i + 1], CultureInfo.InvariantCulture));
			}
			else if (string.Equals(array[i], "users", StringComparison.OrdinalIgnoreCase))
			{
				quotaHeaders.Add("users", long.Parse(array[i + 1], CultureInfo.InvariantCulture));
				usageHeaders.Add("users", long.Parse(array2[i + 1], CultureInfo.InvariantCulture));
			}
			else if (string.Equals(array[i], "permissions", StringComparison.OrdinalIgnoreCase))
			{
				quotaHeaders.Add("permissions", long.Parse(array[i + 1], CultureInfo.InvariantCulture));
				usageHeaders.Add("permissions", long.Parse(array2[i + 1], CultureInfo.InvariantCulture));
			}
			else if (string.Equals(array[i], "collectionSize", StringComparison.OrdinalIgnoreCase))
			{
				quotaHeaders.Add("collectionSize", long.Parse(array[i + 1], CultureInfo.InvariantCulture));
				usageHeaders.Add("collectionSize", long.Parse(array2[i + 1], CultureInfo.InvariantCulture));
			}
			else if (string.Equals(array[i], "documentsSize", StringComparison.OrdinalIgnoreCase))
			{
				quotaHeaders.Add("documentsSize", long.Parse(array[i + 1], CultureInfo.InvariantCulture));
				usageHeaders.Add("documentsSize", long.Parse(array2[i + 1], CultureInfo.InvariantCulture));
			}
			else if (string.Equals(array[i], "documentsCount", StringComparison.OrdinalIgnoreCase))
			{
				quotaHeaders.Add("documentsCount", long.Parse(array[i + 1], CultureInfo.InvariantCulture));
				usageHeaders.Add("documentsCount", long.Parse(array2[i + 1], CultureInfo.InvariantCulture));
			}
			else if (string.Equals(array[i], "storedProcedures", StringComparison.OrdinalIgnoreCase))
			{
				quotaHeaders.Add("storedProcedures", long.Parse(array[i + 1], CultureInfo.InvariantCulture));
				usageHeaders.Add("storedProcedures", long.Parse(array2[i + 1], CultureInfo.InvariantCulture));
			}
			else if (string.Equals(array[i], "triggers", StringComparison.OrdinalIgnoreCase))
			{
				quotaHeaders.Add("triggers", long.Parse(array[i + 1], CultureInfo.InvariantCulture));
				usageHeaders.Add("triggers", long.Parse(array2[i + 1], CultureInfo.InvariantCulture));
			}
			else if (string.Equals(array[i], "functions", StringComparison.OrdinalIgnoreCase))
			{
				quotaHeaders.Add("functions", long.Parse(array[i + 1], CultureInfo.InvariantCulture));
				usageHeaders.Add("functions", long.Parse(array2[i + 1], CultureInfo.InvariantCulture));
			}
		}
	}
}
