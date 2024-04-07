using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Text;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Rntbd;

namespace Microsoft.Azure.Documents;

internal sealed class ClientSideRequestStatistics : IClientSideRequestStatistics
{
	public struct StoreResponseStatistics
	{
		public DateTime RequestStartTime;

		public DateTime RequestResponseTime;

		public StoreResult StoreResult;

		public ResourceType RequestResourceType;

		public OperationType RequestOperationType;

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			AppendToBuilder(stringBuilder);
			return stringBuilder.ToString();
		}

		public void AppendToBuilder(StringBuilder stringBuilder)
		{
			if (stringBuilder == null)
			{
				throw new ArgumentNullException("stringBuilder");
			}
			stringBuilder.Append("RequestStart: ");
			stringBuilder.Append(RequestStartTime.ToString("o", CultureInfo.InvariantCulture));
			stringBuilder.Append("; ResponseTime: ");
			stringBuilder.Append(RequestResponseTime.ToString("o", CultureInfo.InvariantCulture));
			stringBuilder.Append("; StoreResult: ");
			StoreResult?.AppendToBuilder(stringBuilder);
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, " ResourceType: {0}, OperationType: {1}", RequestResourceType, RequestOperationType);
		}
	}

	private class AddressResolutionStatistics
	{
		public DateTime StartTime { get; set; }

		public DateTime EndTime { get; set; }

		public string TargetEndpoint { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			AppendToBuilder(stringBuilder);
			return stringBuilder.ToString();
		}

		public void AppendToBuilder(StringBuilder stringBuilder)
		{
			if (stringBuilder == null)
			{
				throw new ArgumentNullException("stringBuilder");
			}
			stringBuilder.Append("AddressResolution - StartTime: " + StartTime.ToString("o", CultureInfo.InvariantCulture) + ", ").Append("EndTime: " + EndTime.ToString("o", CultureInfo.InvariantCulture) + ", ").Append("TargetEndpoint: ")
				.Append(TargetEndpoint);
		}
	}

	public readonly struct HttpResponseStatistics
	{
		public DateTime RequestStartTime { get; }

		public TimeSpan Duration { get; }

		public HttpResponseMessage HttpResponseMessage { get; }

		public Exception Exception { get; }

		public ResourceType ResourceType { get; }

		public HttpMethod HttpMethod { get; }

		public Uri RequestUri { get; }

		public string ActivityId { get; }

		public HttpResponseStatistics(DateTime requestStartTime, DateTime requestEndTime, Uri requestUri, HttpMethod httpMethod, ResourceType resourceType, HttpResponseMessage responseMessage, Exception exception)
		{
			RequestStartTime = requestStartTime;
			Duration = requestEndTime - requestStartTime;
			HttpResponseMessage = responseMessage;
			Exception = exception;
			ResourceType = resourceType;
			HttpMethod = httpMethod;
			RequestUri = requestUri;
			ActivityId = Trace.CorrelationManager.ActivityId.ToString();
		}

		public void AppendToBuilder(StringBuilder stringBuilder)
		{
			if (stringBuilder == null)
			{
				throw new ArgumentNullException("stringBuilder");
			}
			stringBuilder.Append("HttpResponseStatistics - ").Append("RequestStartTime: ").Append(RequestStartTime.ToString("o", CultureInfo.InvariantCulture))
				.Append(", DurationInMs: ")
				.Append(Duration.TotalMilliseconds)
				.Append(", RequestUri: ")
				.Append(RequestUri)
				.Append(", ResourceType: ")
				.Append(ResourceType)
				.Append(", HttpMethod: ")
				.Append(HttpMethod);
			if (Exception != null)
			{
				stringBuilder.Append(", ExceptionType: ").Append(Exception.GetType()).Append(", ExceptionMessage: ")
					.Append(Exception.Message);
			}
			if (HttpResponseMessage != null)
			{
				stringBuilder.Append(", StatusCode: ").Append(HttpResponseMessage.StatusCode);
				if (!HttpResponseMessage.IsSuccessStatusCode)
				{
					stringBuilder.Append(", ReasonPhrase: ").Append(HttpResponseMessage.ReasonPhrase);
				}
			}
		}
	}

	private static readonly SystemUsageMonitor systemUsageMonitor;

	private static readonly SystemUsageRecorder systemRecorder;

	private static readonly TimeSpan SystemUsageRecordInterval;

	private const string EnableCpuMonitorConfig = "CosmosDbEnableCpuMonitor";

	private const int MaxSupplementalRequestsForToString = 10;

	private static bool enableCpuMonitorFlag;

	private DateTime requestStartTime;

	private DateTime? requestEndTime;

	private object lockObject = new object();

	private object requestEndTimeLock = new object();

	private List<StoreResponseStatistics> responseStatisticsList;

	private List<StoreResponseStatistics> supplementalResponseStatisticsList;

	private Dictionary<string, AddressResolutionStatistics> addressResolutionStatistics;

	private Lazy<List<HttpResponseStatistics>> httpResponseStatisticsList;

	private SystemUsageHistory systemUsageHistory;

	public List<TransportAddressUri> ContactedReplicas { get; set; }

	public HashSet<TransportAddressUri> FailedReplicas { get; private set; }

	public HashSet<(string, Uri)> RegionsContacted { get; private set; }

	public TimeSpan? RequestLatency
	{
		get
		{
			if (!requestEndTime.HasValue)
			{
				return null;
			}
			return requestEndTime.Value - requestStartTime;
		}
	}

	public bool? IsCpuHigh => systemUsageHistory?.IsCpuHigh;

	public bool? IsCpuThreadStarvation => systemUsageHistory?.IsCpuThreadStarvation;

	static ClientSideRequestStatistics()
	{
		SystemUsageRecordInterval = TimeSpan.FromSeconds(10.0);
		enableCpuMonitorFlag = true;
		if (enableCpuMonitorFlag)
		{
			systemRecorder = new SystemUsageRecorder("ClientSideRequestStatistics", 6, SystemUsageRecordInterval);
			systemUsageMonitor = SystemUsageMonitor.CreateAndStart(new List<SystemUsageRecorder> { systemRecorder });
		}
	}

	public ClientSideRequestStatistics()
	{
		requestStartTime = DateTime.UtcNow;
		requestEndTime = null;
		responseStatisticsList = new List<StoreResponseStatistics>();
		supplementalResponseStatisticsList = new List<StoreResponseStatistics>();
		addressResolutionStatistics = new Dictionary<string, AddressResolutionStatistics>();
		ContactedReplicas = new List<TransportAddressUri>();
		FailedReplicas = new HashSet<TransportAddressUri>();
		RegionsContacted = new HashSet<(string, Uri)>();
		httpResponseStatisticsList = new Lazy<List<HttpResponseStatistics>>();
	}

	internal static void DisableCpuMonitor()
	{
		if (enableCpuMonitorFlag)
		{
			enableCpuMonitorFlag = false;
			if (systemRecorder != null)
			{
				systemUsageMonitor.Stop();
				systemUsageMonitor.Dispose();
			}
		}
	}

	public void RecordRequest(DocumentServiceRequest request)
	{
	}

	public void RecordResponse(DocumentServiceRequest request, StoreResult storeResult, DateTime startTimeUtc, DateTime endTimeUtc)
	{
		UpdateRequestEndTime(endTimeUtc);
		StoreResponseStatistics item = default(StoreResponseStatistics);
		item.RequestStartTime = startTimeUtc;
		item.RequestResponseTime = endTimeUtc;
		item.StoreResult = storeResult;
		item.RequestOperationType = request.OperationType;
		item.RequestResourceType = request.ResourceType;
		Uri locationEndpointToRoute = request.RequestContext.LocationEndpointToRoute;
		string item2 = request.RequestContext.RegionName ?? string.Empty;
		lock (lockObject)
		{
			if (locationEndpointToRoute != null)
			{
				RegionsContacted.Add((item2, locationEndpointToRoute));
			}
			if (item.RequestOperationType == OperationType.Head || item.RequestOperationType == OperationType.HeadFeed)
			{
				supplementalResponseStatisticsList.Add(item);
			}
			else
			{
				responseStatisticsList.Add(item);
			}
		}
	}

	public void RecordException(DocumentServiceRequest request, Exception exception, DateTime startTime, DateTime endTimeUtc)
	{
		UpdateRequestEndTime(endTimeUtc);
	}

	public string RecordAddressResolutionStart(Uri targetEndpoint)
	{
		string text = Guid.NewGuid().ToString();
		AddressResolutionStatistics value = new AddressResolutionStatistics
		{
			StartTime = DateTime.UtcNow,
			EndTime = DateTime.MaxValue,
			TargetEndpoint = ((targetEndpoint == null) ? "<NULL>" : targetEndpoint.ToString())
		};
		lock (lockObject)
		{
			addressResolutionStatistics.Add(text, value);
			return text;
		}
	}

	public void RecordAddressResolutionEnd(string identifier)
	{
		if (string.IsNullOrEmpty(identifier))
		{
			return;
		}
		DateTime utcNow = DateTime.UtcNow;
		UpdateRequestEndTime(DateTime.UtcNow);
		lock (lockObject)
		{
			if (!addressResolutionStatistics.ContainsKey(identifier))
			{
				throw new ArgumentException("Identifier {0} does not exist. Please call start before calling end.", identifier);
			}
			addressResolutionStatistics[identifier].EndTime = utcNow;
		}
	}

	public void RecordHttpResponse(HttpRequestMessage request, HttpResponseMessage response, ResourceType resourceType, DateTime requestStartTimeUtc)
	{
		DateTime utcNow = DateTime.UtcNow;
		UpdateRequestEndTime(utcNow);
		lock (httpResponseStatisticsList)
		{
			httpResponseStatisticsList.Value.Add(new HttpResponseStatistics(requestStartTimeUtc, utcNow, request.RequestUri, request.Method, resourceType, response, null));
		}
	}

	public void RecordHttpException(HttpRequestMessage request, Exception exception, ResourceType resourceType, DateTime requestStartTimeUtc)
	{
		DateTime utcNow = DateTime.UtcNow;
		UpdateRequestEndTime(utcNow);
		lock (httpResponseStatisticsList)
		{
			httpResponseStatisticsList.Value.Add(new HttpResponseStatistics(requestStartTimeUtc, utcNow, request.RequestUri, request.Method, resourceType, null, exception));
		}
	}

	private void UpdateRequestEndTime(DateTime requestEndTimeUtc)
	{
		lock (requestEndTimeLock)
		{
			if (requestEndTime.HasValue)
			{
				DateTime? dateTime = requestEndTime;
				if (!(requestEndTimeUtc > dateTime))
				{
					return;
				}
			}
			UpdateSystemUsageHistory();
			requestEndTime = requestEndTimeUtc;
		}
	}

	private void UpdateSystemUsageHistory()
	{
		if (enableCpuMonitorFlag && systemRecorder != null && (systemUsageHistory == null || systemUsageHistory.LastTimestamp + SystemUsageRecordInterval < DateTime.UtcNow))
		{
			try
			{
				systemUsageHistory = systemRecorder.Data;
			}
			catch (Exception ex)
			{
				DefaultTrace.TraceCritical("System usage monitor failed with an unexpected exception: {0}", ex);
			}
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		AppendToBuilder(stringBuilder);
		return stringBuilder.ToString();
	}

	public void AppendToBuilder(StringBuilder stringBuilder)
	{
		if (stringBuilder == null)
		{
			throw new ArgumentNullException("stringBuilder");
		}
		lock (lockObject)
		{
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat(arg1: (!requestEndTime.HasValue) ? ("No response recorded; Current Time: " + DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture)) : requestEndTime.Value.ToString("o", CultureInfo.InvariantCulture), provider: CultureInfo.InvariantCulture, format: "RequestStartTime: {0}, RequestEndTime: {1},  Number of regions attempted:{2}", arg0: requestStartTime.ToString("o", CultureInfo.InvariantCulture), arg2: (RegionsContacted.Count == 0) ? 1 : RegionsContacted.Count);
			stringBuilder.AppendLine();
			if (systemUsageHistory == null)
			{
				UpdateSystemUsageHistory();
			}
			if (systemUsageHistory != null && systemUsageHistory.Values.Count > 0)
			{
				systemUsageHistory.AppendJsonString(stringBuilder);
				stringBuilder.AppendLine();
			}
			else
			{
				stringBuilder.AppendLine("System history not available.");
			}
			foreach (StoreResponseStatistics responseStatistics in responseStatisticsList)
			{
				responseStatistics.AppendToBuilder(stringBuilder);
				stringBuilder.AppendLine();
			}
			foreach (AddressResolutionStatistics value in addressResolutionStatistics.Values)
			{
				value.AppendToBuilder(stringBuilder);
				stringBuilder.AppendLine();
			}
			lock (httpResponseStatisticsList)
			{
				if (httpResponseStatisticsList.IsValueCreated)
				{
					foreach (HttpResponseStatistics item in httpResponseStatisticsList.Value)
					{
						item.AppendToBuilder(stringBuilder);
						stringBuilder.AppendLine();
					}
				}
			}
			int count = supplementalResponseStatisticsList.Count;
			int num = Math.Max(count - 10, 0);
			if (num != 0)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "  -- Displaying only the last {0} head/headfeed requests. Total head/headfeed requests: {1}", 10, count);
				stringBuilder.AppendLine();
			}
			for (int i = num; i < count; i++)
			{
				supplementalResponseStatisticsList[i].AppendToBuilder(stringBuilder);
				stringBuilder.AppendLine();
			}
		}
	}
}
