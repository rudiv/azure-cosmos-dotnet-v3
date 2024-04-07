using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Text;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Routing;

namespace Microsoft.Azure.Documents;

internal sealed class StoreResult : IDisposable
{
	private readonly StoreResponse storeResponse;

	private static bool UseSessionTokenHeader = VersionUtility.IsLaterThan(HttpConstants.Versions.CurrentVersion, HttpConstants.VersionDates.v2018_06_18);

	public DocumentClientException Exception { get; }

	public long LSN { get; private set; }

	public string PartitionKeyRangeId { get; private set; }

	public long QuorumAckedLSN { get; private set; }

	public long GlobalCommittedLSN { get; private set; }

	public long NumberOfReadRegions { get; private set; }

	public long ItemLSN { get; private set; }

	public ISessionToken SessionToken { get; private set; }

	public bool UsingLocalLSN { get; private set; }

	public double RequestCharge { get; private set; }

	public int CurrentReplicaSetSize { get; private set; }

	public int CurrentWriteQuorum { get; private set; }

	public bool IsValid { get; private set; }

	public Uri StorePhysicalAddress { get; private set; }

	public StatusCodes StatusCode { get; private set; }

	public SubStatusCodes SubStatusCode { get; private set; }

	public string ActivityId { get; private set; }

	public string BackendRequestDurationInMs { get; private set; }

	public string RetryAfterInMs { get; private set; }

	public TransportRequestStats TransportRequestStats { get; private set; }

	public IEnumerable<string> ReplicaHealthStatuses { get; private set; }

	public static ReferenceCountedDisposable<StoreResult> CreateStoreResult(StoreResponse storeResponse, Exception responseException, bool requiresValidLsn, bool useLocalLSNBasedHeaders, IEnumerable<string> replicaHealthStatuses, Uri storePhysicalAddress = null)
	{
		if (storeResponse == null && responseException == null)
		{
			throw new ArgumentException("storeResponse or responseException must be populated.");
		}
		if (responseException == null)
		{
			string value = null;
			long quorumAckedLsn = -1L;
			int currentReplicaSetSize = -1;
			int currentWriteQuorum = -1;
			long globalCommittedLSN = -1L;
			int numberOfReadRegions = -1;
			long itemLSN = -1L;
			if (storeResponse.TryGetHeaderValue(useLocalLSNBasedHeaders ? "x-ms-cosmos-quorum-acked-llsn" : "x-ms-quorum-acked-lsn", out value))
			{
				quorumAckedLsn = long.Parse(value, CultureInfo.InvariantCulture);
			}
			if (storeResponse.TryGetHeaderValue("x-ms-current-replica-set-size", out value))
			{
				currentReplicaSetSize = int.Parse(value, CultureInfo.InvariantCulture);
			}
			if (storeResponse.TryGetHeaderValue("x-ms-current-write-quorum", out value))
			{
				currentWriteQuorum = int.Parse(value, CultureInfo.InvariantCulture);
			}
			double requestCharge = 0.0;
			if (storeResponse.TryGetHeaderValue("x-ms-request-charge", out value))
			{
				requestCharge = double.Parse(value, CultureInfo.InvariantCulture);
			}
			if (storeResponse.TryGetHeaderValue("x-ms-number-of-read-regions", out value))
			{
				numberOfReadRegions = int.Parse(value, CultureInfo.InvariantCulture);
			}
			if (storeResponse.TryGetHeaderValue("x-ms-global-Committed-lsn", out value))
			{
				globalCommittedLSN = long.Parse(value, CultureInfo.InvariantCulture);
			}
			if (storeResponse.TryGetHeaderValue(useLocalLSNBasedHeaders ? "x-ms-cosmos-item-llsn" : "x-ms-item-lsn", out value))
			{
				itemLSN = long.Parse(value, CultureInfo.InvariantCulture);
			}
			long lsn = -1L;
			if (useLocalLSNBasedHeaders)
			{
				if (storeResponse.TryGetHeaderValue("x-ms-cosmos-llsn", out value))
				{
					lsn = long.Parse(value, CultureInfo.InvariantCulture);
				}
			}
			else
			{
				lsn = storeResponse.LSN;
			}
			ISessionToken sessionToken = null;
			if (UseSessionTokenHeader)
			{
				if (storeResponse.TryGetHeaderValue("x-ms-session-token", out value))
				{
					sessionToken = SessionTokenHelper.Parse(value);
				}
			}
			else
			{
				sessionToken = new SimpleSessionToken(storeResponse.LSN);
			}
			storeResponse.TryGetHeaderValue("x-ms-activity-id", out var value2);
			storeResponse.TryGetHeaderValue("x-ms-request-duration-ms", out var value3);
			storeResponse.TryGetHeaderValue("x-ms-retry-after-ms", out var value4);
			return new ReferenceCountedDisposable<StoreResult>(new StoreResult(storeResponse, null, storeResponse.PartitionKeyRangeId, lsn, quorumAckedLsn, requestCharge, currentReplicaSetSize, currentWriteQuorum, isValid: true, storePhysicalAddress, globalCommittedLSN, numberOfReadRegions, itemLSN, sessionToken, useLocalLSNBasedHeaders, value2, value3, value4, storeResponse.TransportRequestStats, replicaHealthStatuses));
		}
		if (responseException is DocumentClientException ex)
		{
			long quorumAckedLsn2 = -1L;
			int currentReplicaSetSize2 = -1;
			int currentWriteQuorum2 = -1;
			long globalCommittedLSN2 = -1L;
			int numberOfReadRegions2 = -1;
			string text = ex.Headers[useLocalLSNBasedHeaders ? "x-ms-cosmos-quorum-acked-llsn" : "x-ms-quorum-acked-lsn"];
			if (!string.IsNullOrEmpty(text))
			{
				quorumAckedLsn2 = long.Parse(text, CultureInfo.InvariantCulture);
			}
			text = ex.Headers["x-ms-current-replica-set-size"];
			if (!string.IsNullOrEmpty(text))
			{
				currentReplicaSetSize2 = int.Parse(text, CultureInfo.InvariantCulture);
			}
			text = ex.Headers["x-ms-current-write-quorum"];
			if (!string.IsNullOrEmpty(text))
			{
				currentWriteQuorum2 = int.Parse(text, CultureInfo.InvariantCulture);
			}
			double requestCharge2 = 0.0;
			text = ex.Headers["x-ms-request-charge"];
			if (!string.IsNullOrEmpty(text))
			{
				requestCharge2 = double.Parse(text, CultureInfo.InvariantCulture);
			}
			text = ex.Headers["x-ms-number-of-read-regions"];
			if (!string.IsNullOrEmpty(text))
			{
				numberOfReadRegions2 = int.Parse(text, CultureInfo.InvariantCulture);
			}
			text = ex.Headers["x-ms-global-Committed-lsn"];
			if (!string.IsNullOrEmpty(text))
			{
				globalCommittedLSN2 = long.Parse(text, CultureInfo.InvariantCulture);
			}
			long num = -1L;
			if (useLocalLSNBasedHeaders)
			{
				text = ex.Headers["x-ms-cosmos-llsn"];
				if (!string.IsNullOrEmpty(text))
				{
					num = long.Parse(text, CultureInfo.InvariantCulture);
				}
			}
			else
			{
				num = ex.LSN;
			}
			ISessionToken sessionToken2 = null;
			if (UseSessionTokenHeader)
			{
				text = ex.Headers["x-ms-session-token"];
				if (!string.IsNullOrEmpty(text))
				{
					sessionToken2 = SessionTokenHelper.Parse(text);
				}
			}
			else
			{
				sessionToken2 = new SimpleSessionToken(ex.LSN);
			}
			return new ReferenceCountedDisposable<StoreResult>(new StoreResult(null, ex, ex.PartitionKeyRangeId, num, quorumAckedLsn2, requestCharge2, currentReplicaSetSize2, currentWriteQuorum2, !requiresValidLsn || ((ex.StatusCode != HttpStatusCode.Gone || ex.GetSubStatus() == SubStatusCodes.NameCacheIsStale) && num >= 0), (storePhysicalAddress == null) ? ex.RequestUri : storePhysicalAddress, globalCommittedLSN2, numberOfReadRegions2, -1L, sessionToken2, useLocalLSNBasedHeaders, ex.ActivityId, ex.Headers["x-ms-request-duration-ms"], ex.Headers["x-ms-retry-after-ms"], ex.TransportRequestStats, replicaHealthStatuses));
		}
		DefaultTrace.TraceCritical("Unexpected exception {0} received while reading from store.", responseException);
		return new ReferenceCountedDisposable<StoreResult>(new StoreResult(null, new InternalServerErrorException(RMResources.InternalServerError, responseException), null, -1L, -1L, 0.0, 0, 0, isValid: false, storePhysicalAddress, -1L, 0, -1L, null, useLocalLSNBasedHeaders, null, null, null, null, replicaHealthStatuses));
	}

	public static ReferenceCountedDisposable<StoreResult> CreateForTesting(StoreResponse storeResponse)
	{
		return new ReferenceCountedDisposable<StoreResult>(new StoreResult(storeResponse, null, null, 0L, 0L, 0.0, 0, 0, isValid: false, null, 0L, 0, 0L, null, usingLocalLSN: false, null, null, null, null, null));
	}

	public static ReferenceCountedDisposable<StoreResult> CreateForTesting(TransportRequestStats transportRequestStats)
	{
		return new ReferenceCountedDisposable<StoreResult>(new StoreResult(new StoreResponse(), null, 42.ToString(), 1337L, 23L, 3.14, 4, 3, isValid: true, new Uri("http://storephysicaladdress.com"), 1234L, 13, 15L, new SimpleSessionToken(42L), usingLocalLSN: true, Guid.Empty.ToString(), "4.2", "9000", transportRequestStats, new List<string> { "http://storephysicaladdress-1p.com:Connected", "http://storephysicaladdress-2s.com:Unknown", "http://storephysicaladdress-3s.com:Unhealthy", "http://storephysicaladdress-4s.com:Unknown" }));
	}

	public static ReferenceCountedDisposable<StoreResult> CreateForTesting(string partitionKeyRangeId)
	{
		return new ReferenceCountedDisposable<StoreResult>(new StoreResult(new StoreResponse(), null, partitionKeyRangeId, 0L, 0L, 0.0, 0, 0, isValid: true, null, 0L, 0, 0L, null, usingLocalLSN: true, Guid.NewGuid().ToString(), "10", "20", new TransportRequestStats(), null));
	}

	private StoreResult(StoreResponse storeResponse, DocumentClientException exception, string partitionKeyRangeId, long lsn, long quorumAckedLsn, double requestCharge, int currentReplicaSetSize, int currentWriteQuorum, bool isValid, Uri storePhysicalAddress, long globalCommittedLSN, int numberOfReadRegions, long itemLSN, ISessionToken sessionToken, bool usingLocalLSN, string activityId, string backendRequestDurationInMs, string retryAfterInMs, TransportRequestStats transportRequestStats, IEnumerable<string> replicaHealthStatuses)
	{
		if (storeResponse == null && exception == null)
		{
			throw new ArgumentException("storeResponse or responseException must be populated.");
		}
		this.storeResponse = storeResponse;
		Exception = exception;
		PartitionKeyRangeId = partitionKeyRangeId;
		LSN = lsn;
		QuorumAckedLSN = quorumAckedLsn;
		RequestCharge = requestCharge;
		CurrentReplicaSetSize = currentReplicaSetSize;
		CurrentWriteQuorum = currentWriteQuorum;
		IsValid = isValid;
		StorePhysicalAddress = storePhysicalAddress;
		GlobalCommittedLSN = globalCommittedLSN;
		NumberOfReadRegions = numberOfReadRegions;
		ItemLSN = itemLSN;
		SessionToken = sessionToken;
		UsingLocalLSN = usingLocalLSN;
		ActivityId = activityId;
		BackendRequestDurationInMs = backendRequestDurationInMs;
		RetryAfterInMs = retryAfterInMs;
		TransportRequestStats = transportRequestStats;
		ReplicaHealthStatuses = replicaHealthStatuses;
		StatusCode = (StatusCodes)((this.storeResponse != null) ? new HttpStatusCode?(this.storeResponse.StatusCode) : ((Exception != null && Exception.StatusCode.HasValue) ? Exception.StatusCode : new HttpStatusCode?((HttpStatusCode)0))).Value;
		SubStatusCode = ((this.storeResponse != null) ? this.storeResponse.SubStatusCode : ((Exception != null) ? Exception.GetSubStatus() : SubStatusCodes.Unknown));
	}

	public DocumentClientException GetException()
	{
		if (Exception == null)
		{
			DefaultTrace.TraceCritical("Exception should be available but found none");
			throw new InternalServerErrorException(RMResources.InternalServerError);
		}
		return Exception;
	}

	public StoreResponse ToResponse(RequestChargeTracker requestChargeTracker = null)
	{
		if (!IsValid)
		{
			if (Exception == null)
			{
				DefaultTrace.TraceCritical("Exception not set for invalid response");
				throw new InternalServerErrorException(RMResources.InternalServerError);
			}
			throw Exception;
		}
		if (requestChargeTracker != null)
		{
			SetRequestCharge(storeResponse, Exception, requestChargeTracker.TotalRequestCharge);
		}
		if (Exception != null)
		{
			throw Exception;
		}
		return storeResponse;
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
		stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "StorePhysicalAddress: {0}, LSN: {1}, GlobalCommittedLsn: {2}, PartitionKeyRangeId: {3}, IsValid: {4}, StatusCode: {5}, SubStatusCode: {6}, RequestCharge: {7}, ItemLSN: {8}, SessionToken: {9}, UsingLocalLSN: {10}, TransportException: {11}, BELatencyMs: {12}, ActivityId: {13}, RetryAfterInMs: {14}", StorePhysicalAddress, LSN, GlobalCommittedLSN, PartitionKeyRangeId, IsValid, (int)StatusCode, (int)SubStatusCode, RequestCharge, ItemLSN, SessionToken?.ConvertToString(), UsingLocalLSN, (Exception?.InnerException is TransportException) ? Exception.InnerException.Message : "null", BackendRequestDurationInMs, ActivityId, RetryAfterInMs);
		if (ReplicaHealthStatuses != null && ReplicaHealthStatuses.Any())
		{
			stringBuilder.Append(", ReplicaHealthStatuses: [");
			int num = 0;
			int num2 = ReplicaHealthStatuses.Count();
			foreach (string replicaHealthStatus in ReplicaHealthStatuses)
			{
				stringBuilder.Append(replicaHealthStatus);
				if (num++ < num2 - 1)
				{
					stringBuilder.Append(",");
				}
			}
			stringBuilder.Append("]");
		}
		if (TransportRequestStats != null)
		{
			stringBuilder.Append(", TransportRequestTimeline: ");
			TransportRequestStats.AppendJsonString(stringBuilder);
		}
		stringBuilder.Append(";");
	}

	private static void SetRequestCharge(StoreResponse response, DocumentClientException documentClientException, double totalRequestCharge)
	{
		if (documentClientException != null)
		{
			documentClientException.Headers["x-ms-request-charge"] = totalRequestCharge.ToString(CultureInfo.InvariantCulture);
		}
		else if (response.Headers?.Get("x-ms-request-charge") != null)
		{
			response.Headers["x-ms-request-charge"] = totalRequestCharge.ToString(CultureInfo.InvariantCulture);
		}
	}

	internal static void VerifyCanContinueOnException(DocumentClientException ex)
	{
		if (ex is PartitionKeyRangeGoneException || ex is PartitionKeyRangeIsSplittingException || ex is PartitionIsMigratingException)
		{
			ExceptionDispatchInfo.Capture(ex).Throw();
		}
		if (!string.IsNullOrWhiteSpace(ex.Headers["x-ms-request-validation-failure"]) && int.TryParse(ex.Headers.GetValues("x-ms-request-validation-failure")[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var result) && result == 1)
		{
			ExceptionDispatchInfo.Capture(ex).Throw();
		}
	}

	public void Dispose()
	{
		storeResponse?.ResponseBody?.Dispose();
	}
}
