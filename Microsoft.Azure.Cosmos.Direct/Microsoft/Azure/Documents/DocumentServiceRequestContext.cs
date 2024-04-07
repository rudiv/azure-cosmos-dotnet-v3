using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Documents.Routing;

namespace Microsoft.Azure.Documents;

internal sealed class DocumentServiceRequestContext
{
	private ReferenceCountedDisposable<StoreResult> quorumSelectedStoreResponse;

	public TimeoutHelper TimeoutHelper { get; set; }

	public RequestChargeTracker RequestChargeTracker { get; set; }

	public bool ForceRefreshAddressCache { get; set; }

	public int LastPartitionAddressInformationHashCode { get; set; }

	public ReferenceCountedDisposable<StoreResult> QuorumSelectedStoreResponse => quorumSelectedStoreResponse;

	public ConsistencyLevel? OriginalRequestConsistencyLevel { get; set; }

	public long QuorumSelectedLSN { get; set; }

	public long GlobalCommittedSelectedLSN { get; set; }

	public ReferenceCountedDisposable<StoreResult> GlobalStrongWriteStoreResult { get; set; }

	public ServiceIdentity TargetIdentity { get; set; }

	public bool PerformLocalRefreshOnGoneException { get; set; }

	public PartitionKeyInternal EffectivePartitionKey { get; set; }

	public PartitionKeyRange ResolvedPartitionKeyRange { get; set; }

	public ISessionToken SessionToken { get; set; }

	public bool PerformedBackgroundAddressRefresh { get; set; }

	public IClientSideRequestStatistics ClientRequestStatistics { get; set; }

	public string ResolvedCollectionRid { get; set; }

	public string RegionName { get; set; }

	public bool LocalRegionRequest { get; set; }

	public bool IsRetry { get; set; }

	public bool IsPartitionFailoverRetry { get; set; }

	public List<string> ExcludeRegions { get; set; }

	public Lazy<HashSet<TransportAddressUri>> FailedEndpoints { get; private set; }

	public bool? UsePreferredLocations { get; private set; }

	public int? LocationIndexToRoute { get; private set; }

	public Uri LocationEndpointToRoute { get; private set; }

	public bool EnsureCollectionExistsCheck { get; set; }

	public bool EnableConnectionStateListener { get; set; }

	public string SerializedSourceCollectionForMaterializedView { get; set; }

	public DocumentServiceRequestContext()
	{
		FailedEndpoints = new Lazy<HashSet<TransportAddressUri>>();
	}

	public void UpdateQuorumSelectedStoreResponse(ReferenceCountedDisposable<StoreResult> storeResult)
	{
		ReferenceCountedDisposable<StoreResult> referenceCountedDisposable = quorumSelectedStoreResponse;
		if (referenceCountedDisposable != storeResult)
		{
			referenceCountedDisposable?.Dispose();
			quorumSelectedStoreResponse = storeResult;
		}
	}

	public void AddToFailedEndpoints(Exception storeException, TransportAddressUri targetUri)
	{
		if (storeException is DocumentClientException { StatusCode: var statusCode } ex && (statusCode == HttpStatusCode.Gone || ex.StatusCode == HttpStatusCode.RequestTimeout || ex.StatusCode.Value >= HttpStatusCode.InternalServerError))
		{
			FailedEndpoints.Value.Add(targetUri);
		}
	}

	public void RouteToLocation(int locationIndex, bool usePreferredLocations)
	{
		LocationIndexToRoute = locationIndex;
		UsePreferredLocations = usePreferredLocations;
		LocationEndpointToRoute = null;
	}

	public void RouteToLocation(Uri locationEndpoint)
	{
		LocationEndpointToRoute = locationEndpoint;
		LocationIndexToRoute = null;
		UsePreferredLocations = null;
	}

	public void ClearRouteToLocation()
	{
		LocationIndexToRoute = null;
		LocationEndpointToRoute = null;
		UsePreferredLocations = null;
	}

	public DocumentServiceRequestContext Clone()
	{
		return new DocumentServiceRequestContext
		{
			TimeoutHelper = TimeoutHelper,
			RequestChargeTracker = RequestChargeTracker,
			ForceRefreshAddressCache = ForceRefreshAddressCache,
			TargetIdentity = TargetIdentity,
			PerformLocalRefreshOnGoneException = PerformLocalRefreshOnGoneException,
			SessionToken = SessionToken,
			ResolvedPartitionKeyRange = ResolvedPartitionKeyRange,
			PerformedBackgroundAddressRefresh = PerformedBackgroundAddressRefresh,
			ResolvedCollectionRid = ResolvedCollectionRid,
			EffectivePartitionKey = EffectivePartitionKey,
			ClientRequestStatistics = ClientRequestStatistics,
			OriginalRequestConsistencyLevel = OriginalRequestConsistencyLevel,
			UsePreferredLocations = UsePreferredLocations,
			LocationIndexToRoute = LocationIndexToRoute,
			LocationEndpointToRoute = LocationEndpointToRoute,
			EnsureCollectionExistsCheck = EnsureCollectionExistsCheck,
			EnableConnectionStateListener = EnableConnectionStateListener,
			LocalRegionRequest = LocalRegionRequest,
			FailedEndpoints = FailedEndpoints,
			LastPartitionAddressInformationHashCode = LastPartitionAddressInformationHashCode,
			ExcludeRegions = ExcludeRegions
		};
	}
}
