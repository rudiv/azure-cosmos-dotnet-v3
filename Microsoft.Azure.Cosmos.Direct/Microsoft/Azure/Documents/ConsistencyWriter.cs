using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Core.Trace;

namespace Microsoft.Azure.Documents;

[SuppressMessage("", "AvoidMultiLineComments", Justification = "Multi line business logic")]
internal sealed class ConsistencyWriter
{
	private const int maxNumberOfWriteBarrierReadRetries = 30;

	private const int delayBetweenWriteBarrierCallsInMs = 30;

	private const int maxShortBarrierRetriesForMultiRegion = 4;

	private const int shortbarrierRetryIntervalInMsForMultiRegion = 10;

	private readonly StoreReader storeReader;

	private readonly TransportClient transportClient;

	private readonly AddressSelector addressSelector;

	private readonly ISessionContainer sessionContainer;

	private readonly IServiceConfigurationReader serviceConfigReader;

	private readonly IAuthorizationTokenProvider authorizationTokenProvider;

	private readonly bool useMultipleWriteLocations;

	internal string LastWriteAddress { get; private set; }

	public ConsistencyWriter(AddressSelector addressSelector, ISessionContainer sessionContainer, TransportClient transportClient, IServiceConfigurationReader serviceConfigReader, IAuthorizationTokenProvider authorizationTokenProvider, bool useMultipleWriteLocations, bool enableReplicaValidation)
	{
		this.transportClient = transportClient;
		this.addressSelector = addressSelector;
		this.sessionContainer = sessionContainer;
		this.serviceConfigReader = serviceConfigReader;
		this.authorizationTokenProvider = authorizationTokenProvider;
		this.useMultipleWriteLocations = useMultipleWriteLocations;
		storeReader = new StoreReader(transportClient, addressSelector, new AddressEnumerator(), null, enableReplicaValidation);
	}

	public async Task<StoreResponse> WriteAsync(DocumentServiceRequest entity, TimeoutHelper timeout, bool forceRefresh, CancellationToken cancellationToken = default(CancellationToken))
	{
		timeout.ThrowTimeoutIfElapsed();
		string sessionToken = entity.Headers["x-ms-session-token"];
		try
		{
			return await BackoffRetryUtility<StoreResponse>.ExecuteAsync(() => WritePrivateAsync(entity, timeout, forceRefresh), new SessionTokenMismatchRetryPolicy(), cancellationToken);
		}
		finally
		{
			SessionTokenHelper.SetOriginalSessionToken(entity, sessionToken);
		}
	}

	private async Task<StoreResponse> WritePrivateAsync(DocumentServiceRequest request, TimeoutHelper timeout, bool forceRefresh)
	{
		timeout.ThrowTimeoutIfElapsed();
		request.RequestContext.TimeoutHelper = timeout;
		if (request.RequestContext.RequestChargeTracker == null)
		{
			request.RequestContext.RequestChargeTracker = new RequestChargeTracker();
		}
		if (request.RequestContext.ClientRequestStatistics == null)
		{
			request.RequestContext.ClientRequestStatistics = new ClientSideRequestStatistics();
		}
		request.RequestContext.ForceRefreshAddressCache = forceRefresh;
		if (request.RequestContext.GlobalStrongWriteStoreResult == null)
		{
			string requestedCollectionRid = request.RequestContext.ResolvedCollectionRid;
			PerProtocolPartitionAddressInformation perProtocolPartitionAddressInformation = await addressSelector.ResolveAddressesAsync(request, forceRefresh);
			if (!string.IsNullOrEmpty(requestedCollectionRid) && !string.IsNullOrEmpty(request.RequestContext.ResolvedCollectionRid) && !requestedCollectionRid.Equals(request.RequestContext.ResolvedCollectionRid))
			{
				sessionContainer.ClearTokenByResourceId(requestedCollectionRid);
			}
			request.RequestContext.ClientRequestStatistics.ContactedReplicas = perProtocolPartitionAddressInformation.ReplicaTransportAddressUris.ToList();
			TransportAddressUri primaryUri = perProtocolPartitionAddressInformation.GetPrimaryAddressUri(request);
			LastWriteAddress = primaryUri.ToString();
			if ((useMultipleWriteLocations || request.OperationType == OperationType.Batch) && RequestHelper.GetConsistencyLevelToUse(serviceConfigReader, request) == ConsistencyLevel.Session)
			{
				SessionTokenHelper.SetPartitionLocalSessionToken(request, sessionContainer);
			}
			else
			{
				SessionTokenHelper.ValidateAndRemoveSessionToken(request);
			}
			DateTime startTimeUtc = DateTime.UtcNow;
			ReferenceCountedDisposable<StoreResult> referenceCountedDisposable;
			try
			{
				referenceCountedDisposable = StoreResult.CreateStoreResult(await transportClient.InvokeResourceOperationAsync(primaryUri, request), null, requiresValidLsn: true, useLocalLSNBasedHeaders: false, primaryUri.GetCurrentHealthState().GetHealthStatusDiagnosticsAsReadOnlyEnumerable(), primaryUri.Uri);
				request.RequestContext.ClientRequestStatistics.RecordResponse(request, referenceCountedDisposable.Target, startTimeUtc, DateTime.UtcNow);
			}
			catch (Exception ex)
			{
				referenceCountedDisposable = StoreResult.CreateStoreResult(null, ex, requiresValidLsn: true, useLocalLSNBasedHeaders: false, primaryUri.GetCurrentHealthState().GetHealthStatusDiagnosticsAsReadOnlyEnumerable(), primaryUri.Uri);
				request.RequestContext.ClientRequestStatistics.RecordResponse(request, referenceCountedDisposable.Target, startTimeUtc, DateTime.UtcNow);
				if (ex is DocumentClientException)
				{
					DocumentClientException ex2 = (DocumentClientException)ex;
					StoreResult.VerifyCanContinueOnException(ex2);
					if (!string.IsNullOrWhiteSpace(ex2.Headers["x-ms-write-request-trigger-refresh"]) && int.TryParse(ex2.Headers.GetValues("x-ms-write-request-trigger-refresh")[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var result) && result == 1)
					{
						addressSelector.StartBackgroundAddressRefresh(request);
					}
				}
			}
			if (referenceCountedDisposable?.Target == null)
			{
				DefaultTrace.TraceCritical("ConsistencyWriter did not get storeResult!");
				throw new InternalServerErrorException();
			}
			if (!ReplicatedResourceClient.IsGlobalStrongEnabled() || !ShouldPerformWriteBarrierForGlobalStrong(referenceCountedDisposable.Target))
			{
				return referenceCountedDisposable.Target.ToResponse();
			}
			long lSN = referenceCountedDisposable.Target.LSN;
			long globalCommittedLSN = referenceCountedDisposable.Target.GlobalCommittedLSN;
			if (lSN == -1 || globalCommittedLSN == -1)
			{
				DefaultTrace.TraceWarning("ConsistencyWriter: LSN {0} or GlobalCommittedLsn {1} is not set for global strong request", lSN, globalCommittedLSN);
				throw new GoneException(RMResources.Gone, SubStatusCodes.ServerGenerated410);
			}
			request.RequestContext.GlobalStrongWriteStoreResult = referenceCountedDisposable;
			request.RequestContext.GlobalCommittedSelectedLSN = lSN;
			request.RequestContext.ForceRefreshAddressCache = false;
			DefaultTrace.TraceInformation("ConsistencyWriter: globalCommittedLsn {0}, lsn {1}", globalCommittedLSN, lSN);
			if (globalCommittedLSN < lSN)
			{
				using DocumentServiceRequest barrierRequest2 = await BarrierRequestHelper.CreateAsync(request, authorizationTokenProvider, null, request.RequestContext.GlobalCommittedSelectedLSN);
				if (!(await WaitForWriteBarrierAsync(barrierRequest2, request.RequestContext.GlobalCommittedSelectedLSN)))
				{
					DefaultTrace.TraceError("ConsistencyWriter: Write barrier has not been met for global strong request. SelectedGlobalCommittedLsn: {0}", request.RequestContext.GlobalCommittedSelectedLSN);
					throw new GoneException(RMResources.GlobalStrongWriteBarrierNotMet, SubStatusCodes.Server_GlobalStrongWriteBarrierNotMet);
				}
			}
		}
		else
		{
			using DocumentServiceRequest barrierRequest2 = await BarrierRequestHelper.CreateAsync(request, authorizationTokenProvider, null, request.RequestContext.GlobalCommittedSelectedLSN);
			if (!(await WaitForWriteBarrierAsync(barrierRequest2, request.RequestContext.GlobalCommittedSelectedLSN)))
			{
				DefaultTrace.TraceWarning("ConsistencyWriter: Write barrier has not been met for global strong request. SelectedGlobalCommittedLsn: {0}", request.RequestContext.GlobalCommittedSelectedLSN);
				throw new GoneException(RMResources.GlobalStrongWriteBarrierNotMet, SubStatusCodes.Server_GlobalStrongWriteBarrierNotMet);
			}
		}
		return request.RequestContext.GlobalStrongWriteStoreResult.Target.ToResponse();
	}

	private bool ShouldPerformWriteBarrierForGlobalStrong(StoreResult storeResult)
	{
		if ((storeResult.StatusCode < StatusCodes.StartingErrorCode || storeResult.StatusCode == StatusCodes.Conflict || (storeResult.StatusCode == StatusCodes.NotFound && storeResult.SubStatusCode != SubStatusCodes.PartitionKeyRangeGone) || storeResult.StatusCode == StatusCodes.PreconditionFailed) && serviceConfigReader.DefaultConsistencyLevel == ConsistencyLevel.Strong && storeResult.NumberOfReadRegions > 0)
		{
			return true;
		}
		return false;
	}

	private async Task<bool> WaitForWriteBarrierAsync(DocumentServiceRequest barrierRequest, long selectedGlobalCommittedLsn)
	{
		int writeBarrierRetryCount = 30;
		long maxGlobalCommittedLsnReceived = 0L;
		while (writeBarrierRetryCount-- > 0)
		{
			barrierRequest.RequestContext.TimeoutHelper.ThrowTimeoutIfElapsed();
			IList<ReferenceCountedDisposable<StoreResult>> list = await storeReader.ReadMultipleReplicaAsync(barrierRequest, includePrimary: true, 1, requiresValidLsn: false, useSessionToken: false, ReadMode.Strong);
			if (list != null && list.Any((ReferenceCountedDisposable<StoreResult> response) => response.Target.GlobalCommittedLSN >= selectedGlobalCommittedLsn))
			{
				return true;
			}
			long num = list?.Select((ReferenceCountedDisposable<StoreResult> s) => s.Target.GlobalCommittedLSN).DefaultIfEmpty(0L).Max() ?? 0;
			maxGlobalCommittedLsnReceived = ((maxGlobalCommittedLsnReceived > num) ? maxGlobalCommittedLsnReceived : num);
			barrierRequest.RequestContext.ForceRefreshAddressCache = false;
			if (writeBarrierRetryCount == 0)
			{
				DefaultTrace.TraceInformation("ConsistencyWriter: WaitForWriteBarrierAsync - Last barrier multi-region strong. Responses: {0}", string.Join("; ", list));
			}
			else if (30 - writeBarrierRetryCount > 4)
			{
				await Task.Delay(30);
			}
			else
			{
				await Task.Delay(10);
			}
		}
		DefaultTrace.TraceInformation("ConsistencyWriter: Highest global committed lsn received for write barrier call is {0}", maxGlobalCommittedLsnReceived);
		return false;
	}
}
