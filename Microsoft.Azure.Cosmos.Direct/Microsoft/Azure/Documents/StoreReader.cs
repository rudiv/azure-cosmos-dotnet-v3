using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Core.Trace;

namespace Microsoft.Azure.Documents;

internal sealed class StoreReader
{
	private sealed class ReadReplicaResult : IDisposable
	{
		public bool RetryWithForceRefresh { get; private set; }

		public StoreResultList StoreResultList { get; private set; }

		public ReadReplicaResult(bool retryWithForceRefresh, IList<ReferenceCountedDisposable<StoreResult>> responses)
		{
			RetryWithForceRefresh = retryWithForceRefresh;
			StoreResultList = new StoreResultList(responses);
		}

		public void Dispose()
		{
			StoreResultList.Dispose();
		}
	}

	private class StoreResultList : IDisposable
	{
		private IList<ReferenceCountedDisposable<StoreResult>> value;

		public int Count => GetValueOrThrow().Count;

		public StoreResultList(IList<ReferenceCountedDisposable<StoreResult>> collection)
		{
			value = collection ?? throw new ArgumentNullException();
		}

		public void Add(ReferenceCountedDisposable<StoreResult> storeResult)
		{
			GetValueOrThrow().Add(storeResult);
		}

		public ReferenceCountedDisposable<StoreResult> GetFirstStoreResultAndDereference()
		{
			IList<ReferenceCountedDisposable<StoreResult>> valueOrThrow = GetValueOrThrow();
			if (valueOrThrow.Count > 0)
			{
				ReferenceCountedDisposable<StoreResult> result = valueOrThrow[0];
				value[0] = null;
				return result;
			}
			return null;
		}

		public IList<ReferenceCountedDisposable<StoreResult>> GetValue()
		{
			return GetValueOrThrow();
		}

		public IList<ReferenceCountedDisposable<StoreResult>> GetValueAndDereference()
		{
			IList<ReferenceCountedDisposable<StoreResult>> valueOrThrow = GetValueOrThrow();
			value = null;
			return valueOrThrow;
		}

		public void Dispose()
		{
			if (value != null)
			{
				for (int i = 0; i < value.Count; i++)
				{
					value[i]?.Dispose();
				}
			}
		}

		private IList<ReferenceCountedDisposable<StoreResult>> GetValueOrThrow()
		{
			if (value == null || (value.Count > 0 && value[0] == null))
			{
				throw new InvalidOperationException("Call on the StoreResultList with deferenced collection");
			}
			return value;
		}
	}

	private readonly TransportClient transportClient;

	private readonly AddressSelector addressSelector;

	private readonly IAddressEnumerator addressEnumerator;

	private readonly ISessionContainer sessionContainer;

	private readonly bool canUseLocalLSNBasedHeaders;

	private readonly bool isReplicaAddressValidationEnabled;

	internal string LastReadAddress { get; set; }

	public StoreReader(TransportClient transportClient, AddressSelector addressSelector, IAddressEnumerator addressEnumerator, ISessionContainer sessionContainer, bool enableReplicaValidation)
	{
		this.transportClient = transportClient;
		this.addressSelector = addressSelector;
		this.addressEnumerator = addressEnumerator ?? throw new ArgumentNullException("addressEnumerator");
		this.sessionContainer = sessionContainer;
		canUseLocalLSNBasedHeaders = VersionUtility.IsLaterThan(HttpConstants.Versions.CurrentVersion, HttpConstants.Versions.v2018_06_18);
		isReplicaAddressValidationEnabled = enableReplicaValidation;
	}

	public async Task<IList<ReferenceCountedDisposable<StoreResult>>> ReadMultipleReplicaAsync(DocumentServiceRequest entity, bool includePrimary, int replicaCountToRead, bool requiresValidLsn, bool useSessionToken, ReadMode readMode, bool checkMinLSN = false, bool forceReadAll = false)
	{
		entity.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
		string originalSessionToken = entity.Headers["x-ms-session-token"];
		try
		{
			using ReadReplicaResult readQuorumResult = await ReadMultipleReplicasInternalAsync(entity, includePrimary, replicaCountToRead, requiresValidLsn, useSessionToken, readMode, checkMinLSN, forceReadAll);
			if (entity.RequestContext.PerformLocalRefreshOnGoneException && readQuorumResult.RetryWithForceRefresh && !entity.RequestContext.ForceRefreshAddressCache)
			{
				entity.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
				entity.RequestContext.ForceRefreshAddressCache = true;
				using ReadReplicaResult readReplicaResult = await ReadMultipleReplicasInternalAsync(entity, includePrimary, replicaCountToRead, requiresValidLsn, useSessionToken, readMode, checkMinLSN: false, forceReadAll);
				return readReplicaResult.StoreResultList.GetValueAndDereference();
			}
			return readQuorumResult.StoreResultList.GetValueAndDereference();
		}
		finally
		{
			SessionTokenHelper.SetOriginalSessionToken(entity, originalSessionToken);
		}
	}

	public async Task<ReferenceCountedDisposable<StoreResult>> ReadPrimaryAsync(DocumentServiceRequest entity, bool requiresValidLsn, bool useSessionToken)
	{
		entity.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
		string originalSessionToken = entity.Headers["x-ms-session-token"];
		try
		{
			using ReadReplicaResult readQuorumResult = await ReadPrimaryInternalAsync(entity, requiresValidLsn, useSessionToken, isRetryAfterRefresh: false);
			if (entity.RequestContext.PerformLocalRefreshOnGoneException && readQuorumResult.RetryWithForceRefresh && !entity.RequestContext.ForceRefreshAddressCache)
			{
				entity.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
				entity.RequestContext.ForceRefreshAddressCache = true;
				using ReadReplicaResult readReplicaResult = await ReadPrimaryInternalAsync(entity, requiresValidLsn, useSessionToken, isRetryAfterRefresh: true);
				return GetStoreResultOrThrowGoneException(readReplicaResult);
			}
			return GetStoreResultOrThrowGoneException(readQuorumResult);
		}
		finally
		{
			SessionTokenHelper.SetOriginalSessionToken(entity, originalSessionToken);
		}
	}

	private static ReferenceCountedDisposable<StoreResult> GetStoreResultOrThrowGoneException(ReadReplicaResult readReplicaResult)
	{
		StoreResultList storeResultList = readReplicaResult.StoreResultList;
		if (storeResultList.Count == 0)
		{
			throw new GoneException(RMResources.Gone, SubStatusCodes.Server_NoValidStoreResponse);
		}
		return storeResultList.GetFirstStoreResultAndDereference();
	}

	private async Task<ReadReplicaResult> ReadMultipleReplicasInternalAsync(DocumentServiceRequest entity, bool includePrimary, int replicaCountToRead, bool requiresValidLsn, bool useSessionToken, ReadMode readMode, bool checkMinLSN = false, bool forceReadAll = false)
	{
		entity.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
		using StoreResultList storeResultList = new StoreResultList(new List<ReferenceCountedDisposable<StoreResult>>(replicaCountToRead));
		_ = entity.RequestContext.ResolvedCollectionRid;
		(IReadOnlyList<TransportAddressUri>, IReadOnlyList<string>) tuple = await addressSelector.ResolveAllTransportAddressUriAsync(entity, includePrimary, entity.RequestContext.ForceRefreshAddressCache);
		IReadOnlyList<TransportAddressUri> resolveApiResults = tuple.Item1;
		IReadOnlyList<string> replicaHealthStatuses = tuple.Item2;
		ISessionToken requestSessionToken = null;
		if (useSessionToken)
		{
			SessionTokenHelper.SetPartitionLocalSessionToken(entity, sessionContainer);
			if (checkMinLSN)
			{
				requestSessionToken = entity.RequestContext.SessionToken;
			}
		}
		else
		{
			entity.Headers.Remove("x-ms-session-token");
		}
		if (resolveApiResults.Count < replicaCountToRead)
		{
			if (!entity.RequestContext.ForceRefreshAddressCache)
			{
				return new ReadReplicaResult(retryWithForceRefresh: true, storeResultList.GetValueAndDereference());
			}
			return new ReadReplicaResult(retryWithForceRefresh: false, storeResultList.GetValueAndDereference());
		}
		int num = 1;
		string text = entity.Headers["x-ms-version"];
		bool enforceSessionCheck = !string.IsNullOrEmpty(text) && VersionUtility.IsLaterThan(text, HttpConstants.VersionDates.v2016_05_30);
		UpdateContinuationTokenIfReadFeedOrQuery(entity);
		bool hasGoneException = false;
		bool hasCancellationException = false;
		Exception cancellationException = null;
		Exception exceptionToThrow = null;
		SubStatusCodes subStatusCodeForException = SubStatusCodes.Unknown;
		IEnumerator<TransportAddressUri> uriEnumerator = addressEnumerator.GetTransportAddresses(resolveApiResults, entity.RequestContext.FailedEndpoints, isReplicaAddressValidationEnabled).GetEnumerator();
		while (num > 0 && uriEnumerator.MoveNext())
		{
			entity.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
			Dictionary<Task<(StoreResponse response, DateTime endTime)>, (TransportAddressUri, DateTime startTime)> readStoreTasks = new Dictionary<Task<(StoreResponse, DateTime)>, (TransportAddressUri, DateTime)>();
			do
			{
				readStoreTasks.Add(ReadFromStoreAsync(uriEnumerator.Current, entity), (uriEnumerator.Current, DateTime.UtcNow));
			}
			while ((forceReadAll || readStoreTasks.Count != num) && uriEnumerator.MoveNext());
			try
			{
				await Task.WhenAll(readStoreTasks.Keys);
			}
			catch (Exception ex)
			{
				exceptionToThrow = ex;
				if (ex is DocumentClientException ex2)
				{
					subStatusCodeForException = ex2.GetSubStatus();
				}
				if (ex is DocumentClientException { StatusCode: var statusCode } ex3 && (statusCode == HttpStatusCode.NotFound || ex3.StatusCode == HttpStatusCode.Conflict || ex3.StatusCode.Value == HttpStatusCode.TooManyRequests))
				{
					DefaultTrace.TraceInformation("StoreReader.ReadMultipleReplicasInternalAsync exception thrown: StatusCode: {0}; SubStatusCode:{1}; Exception.Message: {2}", ex3.StatusCode, ex3.Headers?.Get("x-ms-substatus"), ex3.Message);
				}
				else
				{
					DefaultTrace.TraceInformation("StoreReader.ReadMultipleReplicasInternalAsync exception thrown: Exception: {0}", ex);
				}
			}
			foreach (KeyValuePair<Task<(StoreResponse, DateTime)>, (TransportAddressUri, DateTime)> item2 in readStoreTasks)
			{
				Task<(StoreResponse, DateTime)> key = item2.Key;
				StoreResponse storeResponse;
				DateTime endTimeUtc;
				if (key.Status == TaskStatus.RanToCompletion)
				{
					(storeResponse, endTimeUtc) = key.Result;
				}
				else
				{
					DateTime utcNow = DateTime.UtcNow;
					endTimeUtc = utcNow;
					storeResponse = null;
				}
				Exception ex4 = key.Exception?.InnerException;
				TransportAddressUri item = item2.Value.Item1;
				if (ex4 != null)
				{
					entity.RequestContext.AddToFailedEndpoints(ex4, item);
				}
				if (key.IsCanceled || ex4 is OperationCanceledException)
				{
					hasCancellationException = true;
					if (cancellationException == null)
					{
						cancellationException = ex4;
					}
					continue;
				}
				using (ReferenceCountedDisposable<StoreResult> referenceCountedDisposable = StoreResult.CreateStoreResult(storeResponse, ex4, requiresValidLsn, canUseLocalLSNBasedHeaders && readMode != ReadMode.Strong, replicaHealthStatuses, item.Uri))
				{
					StoreResult target = referenceCountedDisposable.Target;
					entity.RequestContext.RequestChargeTracker.AddCharge(target.RequestCharge);
					if (storeResponse != null)
					{
						entity.RequestContext.ClientRequestStatistics.ContactedReplicas.Add(item);
					}
					if (ex4 != null && ex4.InnerException is TransportException)
					{
						entity.RequestContext.ClientRequestStatistics.FailedReplicas.Add(item);
					}
					entity.RequestContext.ClientRequestStatistics.RecordResponse(entity, target, item2.Value.Item2, endTimeUtc);
					if (target.Exception != null)
					{
						StoreResult.VerifyCanContinueOnException(target.Exception);
					}
					if (target.IsValid && (requestSessionToken == null || (target.SessionToken != null && requestSessionToken.IsValid(target.SessionToken)) || (!enforceSessionCheck && target.StatusCode != StatusCodes.NotFound)))
					{
						storeResultList.Add(referenceCountedDisposable.TryAddReference());
					}
					hasGoneException |= target.StatusCode == StatusCodes.Gone && target.SubStatusCode != SubStatusCodes.NameCacheIsStale;
				}
				if (hasGoneException && !entity.RequestContext.PerformedBackgroundAddressRefresh)
				{
					addressSelector.StartBackgroundAddressRefresh(entity);
					entity.RequestContext.PerformedBackgroundAddressRefresh = true;
				}
			}
			if (storeResultList.Count >= replicaCountToRead)
			{
				return new ReadReplicaResult(retryWithForceRefresh: false, storeResultList.GetValueAndDereference());
			}
			num = replicaCountToRead - storeResultList.Count;
		}
		if (storeResultList.Count < replicaCountToRead)
		{
			DefaultTrace.TraceInformation("Could not get quorum number of responses. ValidResponsesReceived: {0} ResponsesExpected: {1}, ResolvedAddressCount: {2}, ResponsesString: {3}", storeResultList.Count, replicaCountToRead, resolveApiResults.Count, string.Join(";", storeResultList.GetValue()));
			if (hasGoneException)
			{
				if (!entity.RequestContext.PerformLocalRefreshOnGoneException)
				{
					throw new GoneException(exceptionToThrow, subStatusCodeForException);
				}
				if (!entity.RequestContext.ForceRefreshAddressCache)
				{
					return new ReadReplicaResult(retryWithForceRefresh: true, storeResultList.GetValueAndDereference());
				}
			}
			else if (hasCancellationException)
			{
				throw cancellationException ?? new OperationCanceledException();
			}
		}
		return new ReadReplicaResult(retryWithForceRefresh: false, storeResultList.GetValueAndDereference());
	}

	private async Task<ReadReplicaResult> ReadPrimaryInternalAsync(DocumentServiceRequest entity, bool requiresValidLsn, bool useSessionToken, bool isRetryAfterRefresh)
	{
		entity.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
		TransportAddressUri primaryUri = await addressSelector.ResolvePrimaryTransportAddressUriAsync(entity, entity.RequestContext.ForceRefreshAddressCache);
		if (useSessionToken)
		{
			SessionTokenHelper.SetPartitionLocalSessionToken(entity, sessionContainer);
		}
		else
		{
			entity.Headers.Remove("x-ms-session-token");
		}
		DateTime startTimeUtc = DateTime.UtcNow;
		StrongBox<DateTime?> endTimeUtc = new StrongBox<DateTime?>();
		using ReferenceCountedDisposable<StoreResult> referenceCountedDisposable = await GetResult(entity, requiresValidLsn, primaryUri, endTimeUtc);
		entity.RequestContext.ClientRequestStatistics.RecordResponse(entity, referenceCountedDisposable.Target, startTimeUtc, endTimeUtc.Value ?? DateTime.UtcNow);
		entity.RequestContext.RequestChargeTracker.AddCharge(referenceCountedDisposable.Target.RequestCharge);
		if (referenceCountedDisposable.Target.Exception != null)
		{
			StoreResult.VerifyCanContinueOnException(referenceCountedDisposable.Target.Exception);
		}
		if (referenceCountedDisposable.Target.StatusCode == StatusCodes.Gone && referenceCountedDisposable.Target.SubStatusCode != SubStatusCodes.NameCacheIsStale)
		{
			if (isRetryAfterRefresh || !entity.RequestContext.PerformLocalRefreshOnGoneException || entity.RequestContext.ForceRefreshAddressCache)
			{
				throw new GoneException(RMResources.Gone, referenceCountedDisposable.Target.SubStatusCode);
			}
			return new ReadReplicaResult(retryWithForceRefresh: true, new List<ReferenceCountedDisposable<StoreResult>>());
		}
		return new ReadReplicaResult(retryWithForceRefresh: false, new ReferenceCountedDisposable<StoreResult>[1] { referenceCountedDisposable.TryAddReference() });
	}

	private async Task<ReferenceCountedDisposable<StoreResult>> GetResult(DocumentServiceRequest entity, bool requiresValidLsn, TransportAddressUri primaryUri, StrongBox<DateTime?> endTimeUtc)
	{
		List<string> primaryReplicaHealthStatus = new List<string> { primaryUri.GetCurrentHealthState().GetHealthStatusDiagnosticString() };
		ReferenceCountedDisposable<StoreResult> result;
		try
		{
			UpdateContinuationTokenIfReadFeedOrQuery(entity);
			StoreResponse item = (await ReadFromStoreAsync(primaryUri, entity)).Item1;
			endTimeUtc.Value = DateTime.UtcNow;
			result = StoreResult.CreateStoreResult(item, null, requiresValidLsn, canUseLocalLSNBasedHeaders, primaryReplicaHealthStatus, primaryUri.Uri);
		}
		catch (Exception ex)
		{
			DefaultTrace.TraceInformation("Exception {0} is thrown while doing Read Primary", ex);
			result = StoreResult.CreateStoreResult(null, ex, requiresValidLsn, canUseLocalLSNBasedHeaders, primaryReplicaHealthStatus, primaryUri.Uri);
		}
		return result;
	}

	private async Task<(StoreResponse, DateTime endTime)> ReadFromStoreAsync(TransportAddressUri physicalAddress, DocumentServiceRequest request)
	{
		request.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
		LastReadAddress = physicalAddress.ToString();
		switch (request.OperationType)
		{
		case OperationType.ExecuteJavaScript:
		case OperationType.Read:
		case OperationType.SqlQuery:
		case OperationType.Head:
		case OperationType.HeadFeed:
			return (await transportClient.InvokeResourceOperationAsync(physicalAddress, request), endTime: DateTime.UtcNow);
		case OperationType.ReadFeed:
		case OperationType.Query:
		{
			QueryRequestPerformanceActivity activity = CustomTypeExtensions.StartActivity(request);
			return (await CompleteActivity(transportClient.InvokeResourceOperationAsync(physicalAddress, request), activity), endTime: DateTime.UtcNow);
		}
		default:
			throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unexpected operation type {0}", request.OperationType));
		}
	}

	private void UpdateContinuationTokenIfReadFeedOrQuery(DocumentServiceRequest request)
	{
		if (request.OperationType != OperationType.ReadFeed && request.OperationType != OperationType.Query)
		{
			return;
		}
		string continuation = request.Continuation;
		if (continuation == null)
		{
			return;
		}
		int num = continuation.IndexOf(';');
		if (num < 0)
		{
			return;
		}
		int num2 = 1;
		for (int i = num + 1; i < continuation.Length; i++)
		{
			if (continuation[i] == ';')
			{
				num2++;
				if (num2 >= 3)
				{
					break;
				}
			}
		}
		if (num2 < 3)
		{
			throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, continuation, "x-ms-continuation"));
		}
		request.Continuation = continuation.Substring(0, num);
	}

	private static async Task<StoreResponse> CompleteActivity(Task<StoreResponse> task, QueryRequestPerformanceActivity activity)
	{
		if (activity == null)
		{
			return await task;
		}
		StoreResponse result;
		try
		{
			result = await task;
		}
		catch
		{
			activity.ActivityComplete(markComplete: false);
			throw;
		}
		activity.ActivityComplete(markComplete: true);
		return result;
	}
}
