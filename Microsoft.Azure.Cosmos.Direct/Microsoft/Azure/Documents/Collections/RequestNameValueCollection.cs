using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.Azure.Cosmos.Core.Trace;

namespace Microsoft.Azure.Documents.Collections;

internal class RequestNameValueCollection : INameValueCollection, IEnumerable
{
	private static readonly StringComparer DefaultStringComparer = StringComparer.OrdinalIgnoreCase;

	private Dictionary<string, string> notCommonHeaders;

	private NameValueCollection nameValueCollection;

	public string A_IM { get; set; }

	public string ActivityId { get; set; }

	public string AddResourcePropertiesToResponse { get; set; }

	public string AllowDocumentReadsInOfflineRegion { get; set; }

	public string AllowRestoreParamsUpdate { get; set; }

	public string AllowTentativeWrites { get; set; }

	public string Authorization { get; set; }

	public string BinaryId { get; set; }

	public string BinaryPassthroughRequest { get; set; }

	public string BindReplicaDirective { get; set; }

	public string BuilderClientIdentifier { get; set; }

	public string CanCharge { get; set; }

	public string CanOfferReplaceComplete { get; set; }

	public string CanThrottle { get; set; }

	public string ChangeFeedStartFullFidelityIfNoneMatch { get; set; }

	public string ChangeFeedWireFormatVersion { get; set; }

	public string ClientIpAddress { get; set; }

	public string ClientRetryAttemptCount { get; set; }

	public string CollectionChildResourceContentLimitInKB { get; set; }

	public string CollectionChildResourceNameLimitInBytes { get; set; }

	public string CollectionPartitionIndex { get; set; }

	public string CollectionRemoteStorageSecurityIdentifier { get; set; }

	public string CollectionRid { get; set; }

	public string CollectionServiceIndex { get; set; }

	public string CollectionTruncate { get; set; }

	public string ConsistencyLevel { get; set; }

	public string ContentSerializationFormat { get; set; }

	public string Continuation { get; set; }

	public string CorrelatedActivityId { get; set; }

	public string DisableRUPerMinuteUsage { get; set; }

	public string EffectivePartitionKey { get; set; }

	public string EmitVerboseTracesInQuery { get; set; }

	public string EnableConflictResolutionPolicyUpdate { get; set; }

	public string EnableCrossPartitionQuery { get; set; }

	public string EnableDynamicRidRangeAllocation { get; set; }

	public string EnableLogging { get; set; }

	public string EnableLowPrecisionOrderBy { get; set; }

	public string EnableScanInQuery { get; set; }

	public string EndEpk { get; set; }

	public string EndId { get; set; }

	public string EntityId { get; set; }

	public string EnumerationDirection { get; set; }

	public string ExcludeSystemProperties { get; set; }

	public string FanoutOperationState { get; set; }

	public string FilterBySchemaResourceId { get; set; }

	public string ForceDatabaseAccountUpdate { get; set; }

	public string ForceQueryScan { get; set; }

	public string ForceSideBySideIndexMigration { get; set; }

	public string GatewaySignature { get; set; }

	public string GetAllPartitionKeyStatistics { get; set; }

	public string HighPriorityForcedBackup { get; set; }

	public string HttpDate { get; set; }

	public string IfMatch { get; set; }

	public string IfModifiedSince { get; set; }

	public string IfNoneMatch { get; set; }

	public string IgnoreSystemLoweringMaxThroughput { get; set; }

	public string IncludePhysicalPartitionThroughputInfo { get; set; }

	public string IncludeTentativeWrites { get; set; }

	public string IndexingDirective { get; set; }

	public string IntendedCollectionRid { get; set; }

	public string IsAutoScaleRequest { get; set; }

	public string IsBatchAtomic { get; set; }

	public string IsBatchOrdered { get; set; }

	public string IsCassandraAlterTypeRequest { get; set; }

	public string IsClientEncrypted { get; set; }

	public string IsContinuationExpected { get; set; }

	public string IsFanoutRequest { get; set; }

	public string IsInternalServerlessRequest { get; set; }

	public string IsMaterializedViewBuild { get; set; }

	public string IsMaterializedViewSourceSchemaReplaceBatchRequest { get; set; }

	public string IsMigratedFixedCollection { get; set; }

	public string IsOfferStorageRefreshRequest { get; set; }

	public string IsReadOnlyScript { get; set; }

	public string IsRequestNotAuthorized { get; set; }

	public string IsRetriedWriteRequest { get; set; }

	public string IsRUPerGBEnforcementRequest { get; set; }

	public string IsServerlessStorageRefreshRequest { get; set; }

	public string IsThroughputCapRequest { get; set; }

	public string IsUserRequest { get; set; }

	public string MaxPollingIntervalMilliseconds { get; set; }

	public string MergeCheckPointGLSN { get; set; }

	public string MergeStaticId { get; set; }

	public string MigrateCollectionDirective { get; set; }

	public string MigrateOfferToAutopilot { get; set; }

	public string MigrateOfferToManualThroughput { get; set; }

	public string NoRetryOn449StatusCode { get; set; }

	public string OfferReplaceRURedistribution { get; set; }

	public string OptimisticDirectExecute { get; set; }

	public string PageSize { get; set; }

	public string ParallelizeCrossPartitionQuery { get; set; }

	public string PartitionCount { get; set; }

	public string PartitionKey { get; set; }

	public string PartitionKeyRangeId { get; set; }

	public string PartitionResourceFilter { get; set; }

	public string PopulateAnalyticalMigrationProgress { get; set; }

	public string PopulateByokEncryptionProgress { get; set; }

	public string PopulateCapacityType { get; set; }

	public string PopulateCollectionThroughputInfo { get; set; }

	public string PopulateCurrentPartitionThroughputInfo { get; set; }

	public string PopulateDocumentRecordCount { get; set; }

	public string PopulateHighestTentativeWriteLLSN { get; set; }

	public string PopulateIndexMetrics { get; set; }

	public string PopulateIndexMetricsV2 { get; set; }

	public string PopulateLogStoreInfo { get; set; }

	public string PopulateMinGLSNForDocumentOperations { get; set; }

	public string PopulateOldestActiveSchemaId { get; set; }

	public string PopulatePartitionStatistics { get; set; }

	public string PopulateQueryMetrics { get; set; }

	public string PopulateQuotaInfo { get; set; }

	public string PopulateResourceCount { get; set; }

	public string PopulateUnflushedMergeEntryCount { get; set; }

	public string PopulateUniqueIndexReIndexProgress { get; set; }

	public string PostTriggerExclude { get; set; }

	public string PostTriggerInclude { get; set; }

	public string Prefer { get; set; }

	public string PreserveFullContent { get; set; }

	public string PreTriggerExclude { get; set; }

	public string PreTriggerInclude { get; set; }

	public string PrimaryMasterKey { get; set; }

	public string PrimaryReadonlyKey { get; set; }

	public string PriorityLevel { get; set; }

	public string ProfileRequest { get; set; }

	public string PruneCollectionSchemas { get; set; }

	public string QueryVersion { get; set; }

	public string RbacAction { get; set; }

	public string RbacResource { get; set; }

	public string RbacUserId { get; set; }

	public string ReadFeedKeyType { get; set; }

	public string RemainingTimeInMsOnClientRequest { get; set; }

	public string RemoteStorageType { get; set; }

	public string RequestedCollectionType { get; set; }

	public string ResourceId { get; set; }

	public string ResourceSchemaName { get; set; }

	public string ResourceTokenExpiry { get; set; }

	public string ResourceTypes { get; set; }

	public string ResponseContinuationTokenLimitInKB { get; set; }

	public string RestoreMetadataFilter { get; set; }

	public string RestoreParams { get; set; }

	public string RetriableWriteRequestId { get; set; }

	public string RetriableWriteRequestStartTimestamp { get; set; }

	public string SchemaHash { get; set; }

	public string SchemaId { get; set; }

	public string SchemaOwnerRid { get; set; }

	public string SDKSupportedCapabilities { get; set; }

	public string SecondaryMasterKey { get; set; }

	public string SecondaryReadonlyKey { get; set; }

	public string SessionToken { get; set; }

	public string SetMasterResourcesDeletionPending { get; set; }

	public string ShareThroughput { get; set; }

	public string ShouldBatchContinueOnError { get; set; }

	public string ShouldReturnCurrentServerDateTime { get; set; }

	public string SkipAdjustThroughputFractionsForOfferReplace { get; set; }

	public string SkipRefreshDatabaseAccountConfigs { get; set; }

	public string SourceCollectionIfMatch { get; set; }

	public string SqlQueryForPartitionKeyExtraction { get; set; }

	public string StartEpk { get; set; }

	public string StartId { get; set; }

	public string SupportedQueryFeatures { get; set; }

	public string SupportedSerializationFormats { get; set; }

	public string SupportSpatialLegacyCoordinates { get; set; }

	public string SystemDocumentType { get; set; }

	public string SystemRestoreOperation { get; set; }

	public string TargetGlobalCommittedLsn { get; set; }

	public string TargetLsn { get; set; }

	public string TimeToLiveInSeconds { get; set; }

	public string TraceParent { get; set; }

	public string TraceState { get; set; }

	public string TransactionCommit { get; set; }

	public string TransactionFirstRequest { get; set; }

	public string TransactionId { get; set; }

	public string TransportRequestID { get; set; }

	public string TruncateMergeLogRequest { get; set; }

	public string UniqueIndexNameEncodingMode { get; set; }

	public string UniqueIndexReIndexingState { get; set; }

	public string UpdateMaxThroughputEverProvisioned { get; set; }

	public string UpdateOfferStateToPending { get; set; }

	public string UpdateOfferStateToRestorePending { get; set; }

	public string UseArchivalPartition { get; set; }

	public string UsePolygonsSmallerThanAHemisphere { get; set; }

	public string UseSystemBudget { get; set; }

	public string UseUserBackgroundBudget { get; set; }

	public string Version { get; set; }

	public string XDate { get; set; }

	public string this[string key]
	{
		get
		{
			return Get(key);
		}
		set
		{
			Set(key, value);
		}
	}

	public RequestNameValueCollection()
	{
	}

	public RequestNameValueCollection(INameValueCollection nameValueCollection)
	{
		foreach (string item in nameValueCollection)
		{
			UpdateHelper(item, nameValueCollection.Get(item), throwIfAlreadyExists: false, ignoreNotCommonHeaders: false);
		}
	}

	public RequestNameValueCollection(IDictionary<string, string> requestHeaders)
	{
		foreach (KeyValuePair<string, string> requestHeader in requestHeaders)
		{
			UpdateHelper(requestHeader.Key, requestHeader.Value, throwIfAlreadyExists: false, ignoreNotCommonHeaders: false);
		}
	}

	public static RequestNameValueCollection BuildRequestNameValueCollectionWithKnownHeadersOnly(IDictionary<string, string> requestHeaders)
	{
		RequestNameValueCollection requestNameValueCollection = new RequestNameValueCollection();
		foreach (KeyValuePair<string, string> requestHeader in requestHeaders)
		{
			requestNameValueCollection.UpdateHelper(requestHeader.Key, requestHeader.Value, throwIfAlreadyExists: false, ignoreNotCommonHeaders: true);
		}
		return requestNameValueCollection;
	}

	public static RequestNameValueCollection BuildRequestNameValueCollectionWithKnownHeadersOnly(INameValueCollection nameValueCollection)
	{
		RequestNameValueCollection requestNameValueCollection = new RequestNameValueCollection();
		try
		{
			foreach (string item in nameValueCollection)
			{
				requestNameValueCollection.UpdateHelper(item, nameValueCollection.Get(item), throwIfAlreadyExists: false, ignoreNotCommonHeaders: true);
			}
		}
		catch (InvalidOperationException ex)
		{
			DefaultTrace.TraceWarning("RequestNameValueCollection Failed to iterate over nameValueCollection headers in a non thread safe manner: " + ex.Message + ". Switching to the per property approach.");
			requestNameValueCollection.ResourceId = nameValueCollection["x-docdb-resource-id"];
			requestNameValueCollection.Authorization = nameValueCollection["authorization"];
			requestNameValueCollection.HttpDate = nameValueCollection["date"];
			requestNameValueCollection.XDate = nameValueCollection["x-ms-date"];
			requestNameValueCollection.PageSize = nameValueCollection["x-ms-max-item-count"];
			requestNameValueCollection.SessionToken = nameValueCollection["x-ms-session-token"];
			requestNameValueCollection.Continuation = nameValueCollection["x-ms-continuation"];
			requestNameValueCollection.IndexingDirective = nameValueCollection["x-ms-indexing-directive"];
			requestNameValueCollection.IfNoneMatch = nameValueCollection["If-None-Match"];
			requestNameValueCollection.PreTriggerInclude = nameValueCollection["x-ms-documentdb-pre-trigger-include"];
			requestNameValueCollection.PostTriggerInclude = nameValueCollection["x-ms-documentdb-post-trigger-include"];
			requestNameValueCollection.IsFanoutRequest = nameValueCollection["x-ms-is-fanout-request"];
			requestNameValueCollection.CollectionPartitionIndex = nameValueCollection["collection-partition-index"];
			requestNameValueCollection.CollectionServiceIndex = nameValueCollection["collection-service-index"];
			requestNameValueCollection.PreTriggerExclude = nameValueCollection["x-ms-documentdb-pre-trigger-exclude"];
			requestNameValueCollection.PostTriggerExclude = nameValueCollection["x-ms-documentdb-post-trigger-exclude"];
			requestNameValueCollection.ConsistencyLevel = nameValueCollection["x-ms-consistency-level"];
			requestNameValueCollection.EntityId = nameValueCollection["x-docdb-entity-id"];
			requestNameValueCollection.ResourceSchemaName = nameValueCollection["x-ms-resource-schema-name"];
			requestNameValueCollection.ResourceTokenExpiry = nameValueCollection["x-ms-documentdb-expiry-seconds"];
			requestNameValueCollection.EnableScanInQuery = nameValueCollection["x-ms-documentdb-query-enable-scan"];
			requestNameValueCollection.EmitVerboseTracesInQuery = nameValueCollection["x-ms-documentdb-query-emit-traces"];
			requestNameValueCollection.BindReplicaDirective = nameValueCollection["x-ms-bind-replica"];
			requestNameValueCollection.PrimaryMasterKey = nameValueCollection["x-ms-primary-master-key"];
			requestNameValueCollection.SecondaryMasterKey = nameValueCollection["x-ms-secondary-master-key"];
			requestNameValueCollection.PrimaryReadonlyKey = nameValueCollection["x-ms-primary-readonly-key"];
			requestNameValueCollection.SecondaryReadonlyKey = nameValueCollection["x-ms-secondary-readonly-key"];
			requestNameValueCollection.ProfileRequest = nameValueCollection["x-ms-profile-request"];
			requestNameValueCollection.EnableLowPrecisionOrderBy = nameValueCollection["x-ms-documentdb-query-enable-low-precision-order-by"];
			requestNameValueCollection.Version = nameValueCollection["x-ms-version"];
			requestNameValueCollection.CanCharge = nameValueCollection["x-ms-cancharge"];
			requestNameValueCollection.CanThrottle = nameValueCollection["x-ms-canthrottle"];
			requestNameValueCollection.PartitionKey = nameValueCollection["x-ms-documentdb-partitionkey"];
			requestNameValueCollection.PartitionKeyRangeId = nameValueCollection["x-ms-documentdb-partitionkeyrangeid"];
			requestNameValueCollection.MigrateCollectionDirective = nameValueCollection["x-ms-migratecollection-directive"];
			requestNameValueCollection.SupportSpatialLegacyCoordinates = nameValueCollection["x-ms-documentdb-supportspatiallegacycoordinates"];
			requestNameValueCollection.PartitionCount = nameValueCollection["x-ms-documentdb-partitioncount"];
			requestNameValueCollection.CollectionRid = nameValueCollection["x-ms-documentdb-collection-rid"];
			requestNameValueCollection.FilterBySchemaResourceId = nameValueCollection["x-ms-documentdb-filterby-schema-rid"];
			requestNameValueCollection.UsePolygonsSmallerThanAHemisphere = nameValueCollection["x-ms-documentdb-usepolygonssmallerthanahemisphere"];
			requestNameValueCollection.GatewaySignature = nameValueCollection["x-ms-gateway-signature"];
			requestNameValueCollection.EnableLogging = nameValueCollection["x-ms-documentdb-script-enable-logging"];
			requestNameValueCollection.A_IM = nameValueCollection["A-IM"];
			requestNameValueCollection.PopulateQuotaInfo = nameValueCollection["x-ms-documentdb-populatequotainfo"];
			requestNameValueCollection.DisableRUPerMinuteUsage = nameValueCollection["x-ms-documentdb-disable-ru-per-minute-usage"];
			requestNameValueCollection.PopulateQueryMetrics = nameValueCollection["x-ms-documentdb-populatequerymetrics"];
			requestNameValueCollection.ResponseContinuationTokenLimitInKB = nameValueCollection["x-ms-documentdb-responsecontinuationtokenlimitinkb"];
			requestNameValueCollection.PopulatePartitionStatistics = nameValueCollection["x-ms-documentdb-populatepartitionstatistics"];
			requestNameValueCollection.RemoteStorageType = nameValueCollection["x-ms-remote-storage-type"];
			requestNameValueCollection.CollectionRemoteStorageSecurityIdentifier = nameValueCollection["x-ms-collection-security-identifier"];
			requestNameValueCollection.IfModifiedSince = nameValueCollection["If-Modified-Since"];
			requestNameValueCollection.PopulateCollectionThroughputInfo = nameValueCollection["x-ms-documentdb-populatecollectionthroughputinfo"];
			requestNameValueCollection.RemainingTimeInMsOnClientRequest = nameValueCollection["x-ms-remaining-time-in-ms-on-client"];
			requestNameValueCollection.ClientRetryAttemptCount = nameValueCollection["x-ms-client-retry-attempt-count"];
			requestNameValueCollection.TargetLsn = nameValueCollection["x-ms-target-lsn"];
			requestNameValueCollection.TargetGlobalCommittedLsn = nameValueCollection["x-ms-target-global-committed-lsn"];
			requestNameValueCollection.TransportRequestID = nameValueCollection["x-ms-transport-request-id"];
			requestNameValueCollection.RestoreMetadataFilter = nameValueCollection["x-ms-restore-metadata-filter"];
			requestNameValueCollection.RestoreParams = nameValueCollection["x-ms-restore-params"];
			requestNameValueCollection.ShareThroughput = nameValueCollection["x-ms-share-throughput"];
			requestNameValueCollection.PartitionResourceFilter = nameValueCollection["x-ms-partition-resource-filter"];
			requestNameValueCollection.IsReadOnlyScript = nameValueCollection["x-ms-is-readonly-script"];
			requestNameValueCollection.IsAutoScaleRequest = nameValueCollection["x-ms-is-auto-scale"];
			requestNameValueCollection.ForceQueryScan = nameValueCollection["x-ms-documentdb-force-query-scan"];
			requestNameValueCollection.CanOfferReplaceComplete = nameValueCollection["x-ms-can-offer-replace-complete"];
			requestNameValueCollection.ExcludeSystemProperties = nameValueCollection["x-ms-exclude-system-properties"];
			requestNameValueCollection.BinaryId = nameValueCollection["x-ms-binary-id"];
			requestNameValueCollection.TimeToLiveInSeconds = nameValueCollection["x-ms-time-to-live-in-seconds"];
			requestNameValueCollection.EffectivePartitionKey = nameValueCollection["x-ms-effective-partition-key"];
			requestNameValueCollection.BinaryPassthroughRequest = nameValueCollection["x-ms-binary-passthrough-request"];
			requestNameValueCollection.EnableDynamicRidRangeAllocation = nameValueCollection["x-ms-enable-dynamic-rid-range-allocation"];
			requestNameValueCollection.EnumerationDirection = nameValueCollection["x-ms-enumeration-direction"];
			requestNameValueCollection.StartId = nameValueCollection["x-ms-start-id"];
			requestNameValueCollection.EndId = nameValueCollection["x-ms-end-id"];
			requestNameValueCollection.FanoutOperationState = nameValueCollection["x-ms-fanout-operation-state"];
			requestNameValueCollection.StartEpk = nameValueCollection["x-ms-start-epk"];
			requestNameValueCollection.EndEpk = nameValueCollection["x-ms-end-epk"];
			requestNameValueCollection.ReadFeedKeyType = nameValueCollection["x-ms-read-key-type"];
			requestNameValueCollection.ContentSerializationFormat = nameValueCollection["x-ms-documentdb-content-serialization-format"];
			requestNameValueCollection.AllowTentativeWrites = nameValueCollection["x-ms-cosmos-allow-tentative-writes"];
			requestNameValueCollection.IsUserRequest = nameValueCollection["x-ms-cosmos-internal-is-user-request"];
			requestNameValueCollection.PreserveFullContent = nameValueCollection["x-ms-cosmos-preserve-full-content"];
			requestNameValueCollection.IncludeTentativeWrites = nameValueCollection["x-ms-cosmos-include-tentative-writes"];
			requestNameValueCollection.PopulateResourceCount = nameValueCollection["x-ms-documentdb-populateresourcecount"];
			requestNameValueCollection.MergeStaticId = nameValueCollection["x-ms-cosmos-merge-static-id"];
			requestNameValueCollection.IsBatchAtomic = nameValueCollection["x-ms-cosmos-batch-atomic"];
			requestNameValueCollection.ShouldBatchContinueOnError = nameValueCollection["x-ms-cosmos-batch-continue-on-error"];
			requestNameValueCollection.IsBatchOrdered = nameValueCollection["x-ms-cosmos-batch-ordered"];
			requestNameValueCollection.SchemaOwnerRid = nameValueCollection["x-ms-schema-owner-rid"];
			requestNameValueCollection.SchemaHash = nameValueCollection["x-ms-schema-hash"];
			requestNameValueCollection.IsRUPerGBEnforcementRequest = nameValueCollection["x-ms-cosmos-internal-is-ru-per-gb-enforcement-request"];
			requestNameValueCollection.MaxPollingIntervalMilliseconds = nameValueCollection["x-ms-cosmos-max-polling-interval"];
			requestNameValueCollection.PopulateLogStoreInfo = nameValueCollection["x-ms-cosmos-populate-logstoreinfo"];
			requestNameValueCollection.GetAllPartitionKeyStatistics = nameValueCollection["x-ms-cosmos-internal-get-all-partition-key-stats"];
			requestNameValueCollection.ForceSideBySideIndexMigration = nameValueCollection["x-ms-cosmos-force-sidebyside-indexmigration"];
			requestNameValueCollection.CollectionChildResourceNameLimitInBytes = nameValueCollection["x-ms-cosmos-collection-child-resourcename-limit"];
			requestNameValueCollection.CollectionChildResourceContentLimitInKB = nameValueCollection["x-ms-cosmos-collection-child-contentlength-resourcelimit"];
			requestNameValueCollection.MergeCheckPointGLSN = nameValueCollection["x-ms-cosmos-internal-merge-checkpoint-glsn"];
			requestNameValueCollection.Prefer = nameValueCollection["Prefer"];
			requestNameValueCollection.UniqueIndexNameEncodingMode = nameValueCollection["x-ms-cosmos-unique-index-name-encoding-mode"];
			requestNameValueCollection.PopulateUnflushedMergeEntryCount = nameValueCollection["x-ms-cosmos-internal-populate-unflushed-merge-entry-count"];
			requestNameValueCollection.MigrateOfferToManualThroughput = nameValueCollection["x-ms-cosmos-migrate-offer-to-manual-throughput"];
			requestNameValueCollection.MigrateOfferToAutopilot = nameValueCollection["x-ms-cosmos-migrate-offer-to-autopilot"];
			requestNameValueCollection.IsClientEncrypted = nameValueCollection["x-ms-cosmos-is-client-encrypted"];
			requestNameValueCollection.SystemDocumentType = nameValueCollection["x-ms-cosmos-systemdocument-type"];
			requestNameValueCollection.IsOfferStorageRefreshRequest = nameValueCollection["x-ms-cosmos-internal-is-offer-storage-refresh-request"];
			requestNameValueCollection.ResourceTypes = nameValueCollection["x-ms-cosmos-resourcetypes"];
			requestNameValueCollection.TransactionId = nameValueCollection["x-ms-cosmos-tx-id"];
			requestNameValueCollection.TransactionFirstRequest = nameValueCollection["x-ms-cosmos-tx-init"];
			requestNameValueCollection.TransactionCommit = nameValueCollection["x-ms-cosmos-tx-commit"];
			requestNameValueCollection.UpdateMaxThroughputEverProvisioned = nameValueCollection["x-ms-cosmos-internal-update-max-throughput-ever-provisioned"];
			requestNameValueCollection.UniqueIndexReIndexingState = nameValueCollection["x-ms-cosmos-uniqueindex-reindexing-state"];
			requestNameValueCollection.UseSystemBudget = nameValueCollection["x-ms-cosmos-use-systembudget"];
			requestNameValueCollection.IgnoreSystemLoweringMaxThroughput = nameValueCollection["x-ms-cosmos-internal-ignore-system-lowering-max-throughput"];
			requestNameValueCollection.TruncateMergeLogRequest = nameValueCollection["x-ms-cosmos-internal-truncate-merge-log"];
			requestNameValueCollection.RetriableWriteRequestId = nameValueCollection["x-ms-cosmos-retriable-write-request-id"];
			requestNameValueCollection.IsRetriedWriteRequest = nameValueCollection["x-ms-cosmos-is-retried-write-request"];
			requestNameValueCollection.RetriableWriteRequestStartTimestamp = nameValueCollection["x-ms-cosmos-retriable-write-request-start-timestamp"];
			requestNameValueCollection.AddResourcePropertiesToResponse = nameValueCollection["x-ms-cosmos-add-resource-properties-to-response"];
			requestNameValueCollection.ChangeFeedStartFullFidelityIfNoneMatch = nameValueCollection["x-ms-cosmos-start-full-fidelity-if-none-match"];
			requestNameValueCollection.SystemRestoreOperation = nameValueCollection["x-ms-cosmos-internal-system-restore-operation"];
			requestNameValueCollection.SkipRefreshDatabaseAccountConfigs = nameValueCollection["x-ms-cosmos-skip-refresh-databaseaccountconfig"];
			requestNameValueCollection.IntendedCollectionRid = nameValueCollection["x-ms-cosmos-intended-collection-rid"];
			requestNameValueCollection.UseArchivalPartition = nameValueCollection["x-ms-cosmos-use-archival-partition"];
			requestNameValueCollection.PopulateUniqueIndexReIndexProgress = nameValueCollection["x-ms-cosmosdb-populateuniqueindexreindexprogress"];
			requestNameValueCollection.SchemaId = nameValueCollection["x-ms-schema-id"];
			requestNameValueCollection.CollectionTruncate = nameValueCollection["x-ms-cosmos-collection-truncate"];
			requestNameValueCollection.SDKSupportedCapabilities = nameValueCollection["x-ms-cosmos-sdk-supportedcapabilities"];
			requestNameValueCollection.IsMaterializedViewBuild = nameValueCollection["x-ms-cosmos-internal-is-materialized-view-build"];
			requestNameValueCollection.BuilderClientIdentifier = nameValueCollection["x-ms-cosmos-builder-client-identifier"];
			requestNameValueCollection.SourceCollectionIfMatch = nameValueCollection["x-ms-cosmos-source-collection-if-match"];
			requestNameValueCollection.RequestedCollectionType = nameValueCollection["x-ms-cosmos-collectiontype"];
			requestNameValueCollection.PopulateIndexMetrics = nameValueCollection["x-ms-cosmos-populateindexmetrics"];
			requestNameValueCollection.PopulateAnalyticalMigrationProgress = nameValueCollection["x-ms-cosmos-populate-analytical-migration-progress"];
			requestNameValueCollection.ShouldReturnCurrentServerDateTime = nameValueCollection["x-ms-should-return-current-server-datetime"];
			requestNameValueCollection.RbacUserId = nameValueCollection["x-ms-rbac-user-id"];
			requestNameValueCollection.RbacAction = nameValueCollection["x-ms-rbac-action"];
			requestNameValueCollection.RbacResource = nameValueCollection["x-ms-rbac-resource"];
			requestNameValueCollection.CorrelatedActivityId = nameValueCollection["x-ms-cosmos-correlated-activityid"];
			requestNameValueCollection.IsThroughputCapRequest = nameValueCollection["x-ms-cosmos-internal-is-throughputcap-request"];
			requestNameValueCollection.ChangeFeedWireFormatVersion = nameValueCollection["x-ms-cosmos-changefeed-wire-format-version"];
			requestNameValueCollection.PopulateByokEncryptionProgress = nameValueCollection["x-ms-cosmos-populate-byok-encryption-progress"];
			requestNameValueCollection.UseUserBackgroundBudget = nameValueCollection["x-ms-cosmos-use-background-task-budget"];
			requestNameValueCollection.IncludePhysicalPartitionThroughputInfo = nameValueCollection["x-ms-cosmos-include-physical-partition-throughput-info"];
			requestNameValueCollection.IsServerlessStorageRefreshRequest = nameValueCollection["x-ms-cosmos-internal-serverless-offer-storage-refresh-request"];
			requestNameValueCollection.UpdateOfferStateToPending = nameValueCollection["x-ms-cosmos-internal-update-offer-state-to-pending"];
			requestNameValueCollection.PopulateOldestActiveSchemaId = nameValueCollection["x-ms-cosmos-populate-oldest-active-schema-id"];
			requestNameValueCollection.IsInternalServerlessRequest = nameValueCollection["x-ms-cosmos-internal-serverless-request"];
			requestNameValueCollection.OfferReplaceRURedistribution = nameValueCollection["x-ms-cosmos-internal-offer-replace-ru-redistribution"];
			requestNameValueCollection.IsCassandraAlterTypeRequest = nameValueCollection["x-ms-cosmos-alter-type-request"];
			requestNameValueCollection.IsMaterializedViewSourceSchemaReplaceBatchRequest = nameValueCollection["x-ms-cosmos-is-materialized-view-source-schema-replace"];
			requestNameValueCollection.ForceDatabaseAccountUpdate = nameValueCollection["x-ms-cosmos-force-database-account-update"];
			requestNameValueCollection.PriorityLevel = nameValueCollection["x-ms-cosmos-priority-level"];
			requestNameValueCollection.AllowRestoreParamsUpdate = nameValueCollection["x-ms-cosmos-internal-allow-restore-params-update"];
			requestNameValueCollection.PruneCollectionSchemas = nameValueCollection["x-ms-cosmos-prune-collection-schemas"];
			requestNameValueCollection.PopulateIndexMetricsV2 = nameValueCollection["x-ms-cosmos-populateindexmetrics-V2"];
			requestNameValueCollection.IsMigratedFixedCollection = nameValueCollection["x-ms-cosmos-internal-migrated-fixed-collection"];
			requestNameValueCollection.SupportedSerializationFormats = nameValueCollection["x-ms-cosmos-supported-serialization-formats"];
			requestNameValueCollection.UpdateOfferStateToRestorePending = nameValueCollection["x-ms-cosmos-internal-update-offer-state-restore-pending"];
			requestNameValueCollection.SetMasterResourcesDeletionPending = nameValueCollection["x-ms-cosmos-internal-set-master-resources-deletion-pending"];
			requestNameValueCollection.HighPriorityForcedBackup = nameValueCollection["x-ms-cosmos-internal-high-priority-forced-backup"];
			requestNameValueCollection.OptimisticDirectExecute = nameValueCollection["x-ms-cosmos-query-optimisticdirectexecute"];
			requestNameValueCollection.PopulateMinGLSNForDocumentOperations = nameValueCollection["x-ms-cosmos-internal-populate-min-glsn-for-relocation"];
			requestNameValueCollection.PopulateHighestTentativeWriteLLSN = nameValueCollection["x-ms-cosmos-internal-populate-highest-tentative-write-llsn"];
			requestNameValueCollection.PopulateCapacityType = nameValueCollection["x-ms-cosmos-populate-capacity-type"];
			requestNameValueCollection.TraceParent = nameValueCollection["traceparent"];
			requestNameValueCollection.TraceState = nameValueCollection["tracestate"];
			requestNameValueCollection.EnableConflictResolutionPolicyUpdate = nameValueCollection["x-ms-cosmos-internal-enable-conflictresolutionpolicy-update"];
			requestNameValueCollection.ClientIpAddress = nameValueCollection["x-ms-cosmos-client-ip-address"];
			requestNameValueCollection.IsRequestNotAuthorized = nameValueCollection["x-ms-cosmos-is-request-not-authorized"];
			requestNameValueCollection.AllowDocumentReadsInOfflineRegion = nameValueCollection["x-ms-cosmos-internal-allow-document-reads-in-offline-region"];
			requestNameValueCollection.PopulateCurrentPartitionThroughputInfo = nameValueCollection["x-ms-cosmos-populate-current-partition-throughput-info"];
			requestNameValueCollection.PopulateDocumentRecordCount = nameValueCollection["x-ms-cosmos-internal-populate-document-record-count"];
			requestNameValueCollection.IfMatch = nameValueCollection["If-Match"];
			requestNameValueCollection.NoRetryOn449StatusCode = nameValueCollection["x-ms-noretry-449"];
			requestNameValueCollection.SkipAdjustThroughputFractionsForOfferReplace = nameValueCollection["x-ms-cosmos-skip-adjust-throughput-fractions-for-offer-replace"];
			requestNameValueCollection.SqlQueryForPartitionKeyExtraction = nameValueCollection["x-ms-documentdb-query-sqlqueryforpartitionkeyextraction"];
			requestNameValueCollection.EnableCrossPartitionQuery = nameValueCollection["x-ms-documentdb-query-enablecrosspartition"];
			requestNameValueCollection.IsContinuationExpected = nameValueCollection["x-ms-documentdb-query-iscontinuationexpected"];
			requestNameValueCollection.ParallelizeCrossPartitionQuery = nameValueCollection["x-ms-documentdb-query-parallelizecrosspartitionquery"];
			requestNameValueCollection.SupportedQueryFeatures = nameValueCollection["x-ms-cosmos-supported-query-features"];
			requestNameValueCollection.QueryVersion = nameValueCollection["x-ms-cosmos-query-version"];
			requestNameValueCollection.ActivityId = nameValueCollection["x-ms-activity-id"];
		}
		return requestNameValueCollection;
	}

	public void Add(INameValueCollection collection)
	{
		if (collection == null)
		{
			throw new ArgumentNullException("collection");
		}
		foreach (string item in collection.Keys())
		{
			Set(item, collection[item]);
		}
	}

	public string[] AllKeys()
	{
		return Keys().ToArray();
	}

	public void Clear()
	{
		if (notCommonHeaders != null)
		{
			notCommonHeaders.Clear();
		}
		A_IM = null;
		ActivityId = null;
		AddResourcePropertiesToResponse = null;
		AllowDocumentReadsInOfflineRegion = null;
		AllowRestoreParamsUpdate = null;
		AllowTentativeWrites = null;
		Authorization = null;
		BinaryId = null;
		BinaryPassthroughRequest = null;
		BindReplicaDirective = null;
		BuilderClientIdentifier = null;
		CanCharge = null;
		CanOfferReplaceComplete = null;
		CanThrottle = null;
		ChangeFeedStartFullFidelityIfNoneMatch = null;
		ChangeFeedWireFormatVersion = null;
		ClientIpAddress = null;
		ClientRetryAttemptCount = null;
		CollectionChildResourceContentLimitInKB = null;
		CollectionChildResourceNameLimitInBytes = null;
		CollectionPartitionIndex = null;
		CollectionRemoteStorageSecurityIdentifier = null;
		CollectionRid = null;
		CollectionServiceIndex = null;
		CollectionTruncate = null;
		ConsistencyLevel = null;
		ContentSerializationFormat = null;
		Continuation = null;
		CorrelatedActivityId = null;
		DisableRUPerMinuteUsage = null;
		EffectivePartitionKey = null;
		EmitVerboseTracesInQuery = null;
		EnableConflictResolutionPolicyUpdate = null;
		EnableCrossPartitionQuery = null;
		EnableDynamicRidRangeAllocation = null;
		EnableLogging = null;
		EnableLowPrecisionOrderBy = null;
		EnableScanInQuery = null;
		EndEpk = null;
		EndId = null;
		EntityId = null;
		EnumerationDirection = null;
		ExcludeSystemProperties = null;
		FanoutOperationState = null;
		FilterBySchemaResourceId = null;
		ForceDatabaseAccountUpdate = null;
		ForceQueryScan = null;
		ForceSideBySideIndexMigration = null;
		GatewaySignature = null;
		GetAllPartitionKeyStatistics = null;
		HighPriorityForcedBackup = null;
		HttpDate = null;
		IfMatch = null;
		IfModifiedSince = null;
		IfNoneMatch = null;
		IgnoreSystemLoweringMaxThroughput = null;
		IncludePhysicalPartitionThroughputInfo = null;
		IncludeTentativeWrites = null;
		IndexingDirective = null;
		IntendedCollectionRid = null;
		IsAutoScaleRequest = null;
		IsBatchAtomic = null;
		IsBatchOrdered = null;
		IsCassandraAlterTypeRequest = null;
		IsClientEncrypted = null;
		IsContinuationExpected = null;
		IsFanoutRequest = null;
		IsInternalServerlessRequest = null;
		IsMaterializedViewBuild = null;
		IsMaterializedViewSourceSchemaReplaceBatchRequest = null;
		IsMigratedFixedCollection = null;
		IsOfferStorageRefreshRequest = null;
		IsReadOnlyScript = null;
		IsRequestNotAuthorized = null;
		IsRetriedWriteRequest = null;
		IsRUPerGBEnforcementRequest = null;
		IsServerlessStorageRefreshRequest = null;
		IsThroughputCapRequest = null;
		IsUserRequest = null;
		MaxPollingIntervalMilliseconds = null;
		MergeCheckPointGLSN = null;
		MergeStaticId = null;
		MigrateCollectionDirective = null;
		MigrateOfferToAutopilot = null;
		MigrateOfferToManualThroughput = null;
		NoRetryOn449StatusCode = null;
		OfferReplaceRURedistribution = null;
		OptimisticDirectExecute = null;
		PageSize = null;
		ParallelizeCrossPartitionQuery = null;
		PartitionCount = null;
		PartitionKey = null;
		PartitionKeyRangeId = null;
		PartitionResourceFilter = null;
		PopulateAnalyticalMigrationProgress = null;
		PopulateByokEncryptionProgress = null;
		PopulateCapacityType = null;
		PopulateCollectionThroughputInfo = null;
		PopulateCurrentPartitionThroughputInfo = null;
		PopulateDocumentRecordCount = null;
		PopulateHighestTentativeWriteLLSN = null;
		PopulateIndexMetrics = null;
		PopulateIndexMetricsV2 = null;
		PopulateLogStoreInfo = null;
		PopulateMinGLSNForDocumentOperations = null;
		PopulateOldestActiveSchemaId = null;
		PopulatePartitionStatistics = null;
		PopulateQueryMetrics = null;
		PopulateQuotaInfo = null;
		PopulateResourceCount = null;
		PopulateUnflushedMergeEntryCount = null;
		PopulateUniqueIndexReIndexProgress = null;
		PostTriggerExclude = null;
		PostTriggerInclude = null;
		Prefer = null;
		PreserveFullContent = null;
		PreTriggerExclude = null;
		PreTriggerInclude = null;
		PrimaryMasterKey = null;
		PrimaryReadonlyKey = null;
		PriorityLevel = null;
		ProfileRequest = null;
		PruneCollectionSchemas = null;
		QueryVersion = null;
		RbacAction = null;
		RbacResource = null;
		RbacUserId = null;
		ReadFeedKeyType = null;
		RemainingTimeInMsOnClientRequest = null;
		RemoteStorageType = null;
		RequestedCollectionType = null;
		ResourceId = null;
		ResourceSchemaName = null;
		ResourceTokenExpiry = null;
		ResourceTypes = null;
		ResponseContinuationTokenLimitInKB = null;
		RestoreMetadataFilter = null;
		RestoreParams = null;
		RetriableWriteRequestId = null;
		RetriableWriteRequestStartTimestamp = null;
		SchemaHash = null;
		SchemaId = null;
		SchemaOwnerRid = null;
		SDKSupportedCapabilities = null;
		SecondaryMasterKey = null;
		SecondaryReadonlyKey = null;
		SessionToken = null;
		SetMasterResourcesDeletionPending = null;
		ShareThroughput = null;
		ShouldBatchContinueOnError = null;
		ShouldReturnCurrentServerDateTime = null;
		SkipAdjustThroughputFractionsForOfferReplace = null;
		SkipRefreshDatabaseAccountConfigs = null;
		SourceCollectionIfMatch = null;
		SqlQueryForPartitionKeyExtraction = null;
		StartEpk = null;
		StartId = null;
		SupportedQueryFeatures = null;
		SupportedSerializationFormats = null;
		SupportSpatialLegacyCoordinates = null;
		SystemDocumentType = null;
		SystemRestoreOperation = null;
		TargetGlobalCommittedLsn = null;
		TargetLsn = null;
		TimeToLiveInSeconds = null;
		TraceParent = null;
		TraceState = null;
		TransactionCommit = null;
		TransactionFirstRequest = null;
		TransactionId = null;
		TransportRequestID = null;
		TruncateMergeLogRequest = null;
		UniqueIndexNameEncodingMode = null;
		UniqueIndexReIndexingState = null;
		UpdateMaxThroughputEverProvisioned = null;
		UpdateOfferStateToPending = null;
		UpdateOfferStateToRestorePending = null;
		UseArchivalPartition = null;
		UsePolygonsSmallerThanAHemisphere = null;
		UseSystemBudget = null;
		UseUserBackgroundBudget = null;
		Version = null;
		XDate = null;
	}

	public INameValueCollection Clone()
	{
		RequestNameValueCollection requestNameValueCollection = new RequestNameValueCollection
		{
			A_IM = A_IM,
			ActivityId = ActivityId,
			AddResourcePropertiesToResponse = AddResourcePropertiesToResponse,
			AllowDocumentReadsInOfflineRegion = AllowDocumentReadsInOfflineRegion,
			AllowRestoreParamsUpdate = AllowRestoreParamsUpdate,
			AllowTentativeWrites = AllowTentativeWrites,
			Authorization = Authorization,
			BinaryId = BinaryId,
			BinaryPassthroughRequest = BinaryPassthroughRequest,
			BindReplicaDirective = BindReplicaDirective,
			BuilderClientIdentifier = BuilderClientIdentifier,
			CanCharge = CanCharge,
			CanOfferReplaceComplete = CanOfferReplaceComplete,
			CanThrottle = CanThrottle,
			ChangeFeedStartFullFidelityIfNoneMatch = ChangeFeedStartFullFidelityIfNoneMatch,
			ChangeFeedWireFormatVersion = ChangeFeedWireFormatVersion,
			ClientIpAddress = ClientIpAddress,
			ClientRetryAttemptCount = ClientRetryAttemptCount,
			CollectionChildResourceContentLimitInKB = CollectionChildResourceContentLimitInKB,
			CollectionChildResourceNameLimitInBytes = CollectionChildResourceNameLimitInBytes,
			CollectionPartitionIndex = CollectionPartitionIndex,
			CollectionRemoteStorageSecurityIdentifier = CollectionRemoteStorageSecurityIdentifier,
			CollectionRid = CollectionRid,
			CollectionServiceIndex = CollectionServiceIndex,
			CollectionTruncate = CollectionTruncate,
			ConsistencyLevel = ConsistencyLevel,
			ContentSerializationFormat = ContentSerializationFormat,
			Continuation = Continuation,
			CorrelatedActivityId = CorrelatedActivityId,
			DisableRUPerMinuteUsage = DisableRUPerMinuteUsage,
			EffectivePartitionKey = EffectivePartitionKey,
			EmitVerboseTracesInQuery = EmitVerboseTracesInQuery,
			EnableConflictResolutionPolicyUpdate = EnableConflictResolutionPolicyUpdate,
			EnableCrossPartitionQuery = EnableCrossPartitionQuery,
			EnableDynamicRidRangeAllocation = EnableDynamicRidRangeAllocation,
			EnableLogging = EnableLogging,
			EnableLowPrecisionOrderBy = EnableLowPrecisionOrderBy,
			EnableScanInQuery = EnableScanInQuery,
			EndEpk = EndEpk,
			EndId = EndId,
			EntityId = EntityId,
			EnumerationDirection = EnumerationDirection,
			ExcludeSystemProperties = ExcludeSystemProperties,
			FanoutOperationState = FanoutOperationState,
			FilterBySchemaResourceId = FilterBySchemaResourceId,
			ForceDatabaseAccountUpdate = ForceDatabaseAccountUpdate,
			ForceQueryScan = ForceQueryScan,
			ForceSideBySideIndexMigration = ForceSideBySideIndexMigration,
			GatewaySignature = GatewaySignature,
			GetAllPartitionKeyStatistics = GetAllPartitionKeyStatistics,
			HighPriorityForcedBackup = HighPriorityForcedBackup,
			HttpDate = HttpDate,
			IfMatch = IfMatch,
			IfModifiedSince = IfModifiedSince,
			IfNoneMatch = IfNoneMatch,
			IgnoreSystemLoweringMaxThroughput = IgnoreSystemLoweringMaxThroughput,
			IncludePhysicalPartitionThroughputInfo = IncludePhysicalPartitionThroughputInfo,
			IncludeTentativeWrites = IncludeTentativeWrites,
			IndexingDirective = IndexingDirective,
			IntendedCollectionRid = IntendedCollectionRid,
			IsAutoScaleRequest = IsAutoScaleRequest,
			IsBatchAtomic = IsBatchAtomic,
			IsBatchOrdered = IsBatchOrdered,
			IsCassandraAlterTypeRequest = IsCassandraAlterTypeRequest,
			IsClientEncrypted = IsClientEncrypted,
			IsContinuationExpected = IsContinuationExpected,
			IsFanoutRequest = IsFanoutRequest,
			IsInternalServerlessRequest = IsInternalServerlessRequest,
			IsMaterializedViewBuild = IsMaterializedViewBuild,
			IsMaterializedViewSourceSchemaReplaceBatchRequest = IsMaterializedViewSourceSchemaReplaceBatchRequest,
			IsMigratedFixedCollection = IsMigratedFixedCollection,
			IsOfferStorageRefreshRequest = IsOfferStorageRefreshRequest,
			IsReadOnlyScript = IsReadOnlyScript,
			IsRequestNotAuthorized = IsRequestNotAuthorized,
			IsRetriedWriteRequest = IsRetriedWriteRequest,
			IsRUPerGBEnforcementRequest = IsRUPerGBEnforcementRequest,
			IsServerlessStorageRefreshRequest = IsServerlessStorageRefreshRequest,
			IsThroughputCapRequest = IsThroughputCapRequest,
			IsUserRequest = IsUserRequest,
			MaxPollingIntervalMilliseconds = MaxPollingIntervalMilliseconds,
			MergeCheckPointGLSN = MergeCheckPointGLSN,
			MergeStaticId = MergeStaticId,
			MigrateCollectionDirective = MigrateCollectionDirective,
			MigrateOfferToAutopilot = MigrateOfferToAutopilot,
			MigrateOfferToManualThroughput = MigrateOfferToManualThroughput,
			NoRetryOn449StatusCode = NoRetryOn449StatusCode,
			OfferReplaceRURedistribution = OfferReplaceRURedistribution,
			OptimisticDirectExecute = OptimisticDirectExecute,
			PageSize = PageSize,
			ParallelizeCrossPartitionQuery = ParallelizeCrossPartitionQuery,
			PartitionCount = PartitionCount,
			PartitionKey = PartitionKey,
			PartitionKeyRangeId = PartitionKeyRangeId,
			PartitionResourceFilter = PartitionResourceFilter,
			PopulateAnalyticalMigrationProgress = PopulateAnalyticalMigrationProgress,
			PopulateByokEncryptionProgress = PopulateByokEncryptionProgress,
			PopulateCapacityType = PopulateCapacityType,
			PopulateCollectionThroughputInfo = PopulateCollectionThroughputInfo,
			PopulateCurrentPartitionThroughputInfo = PopulateCurrentPartitionThroughputInfo,
			PopulateDocumentRecordCount = PopulateDocumentRecordCount,
			PopulateHighestTentativeWriteLLSN = PopulateHighestTentativeWriteLLSN,
			PopulateIndexMetrics = PopulateIndexMetrics,
			PopulateIndexMetricsV2 = PopulateIndexMetricsV2,
			PopulateLogStoreInfo = PopulateLogStoreInfo,
			PopulateMinGLSNForDocumentOperations = PopulateMinGLSNForDocumentOperations,
			PopulateOldestActiveSchemaId = PopulateOldestActiveSchemaId,
			PopulatePartitionStatistics = PopulatePartitionStatistics,
			PopulateQueryMetrics = PopulateQueryMetrics,
			PopulateQuotaInfo = PopulateQuotaInfo,
			PopulateResourceCount = PopulateResourceCount,
			PopulateUnflushedMergeEntryCount = PopulateUnflushedMergeEntryCount,
			PopulateUniqueIndexReIndexProgress = PopulateUniqueIndexReIndexProgress,
			PostTriggerExclude = PostTriggerExclude,
			PostTriggerInclude = PostTriggerInclude,
			Prefer = Prefer,
			PreserveFullContent = PreserveFullContent,
			PreTriggerExclude = PreTriggerExclude,
			PreTriggerInclude = PreTriggerInclude,
			PrimaryMasterKey = PrimaryMasterKey,
			PrimaryReadonlyKey = PrimaryReadonlyKey,
			PriorityLevel = PriorityLevel,
			ProfileRequest = ProfileRequest,
			PruneCollectionSchemas = PruneCollectionSchemas,
			QueryVersion = QueryVersion,
			RbacAction = RbacAction,
			RbacResource = RbacResource,
			RbacUserId = RbacUserId,
			ReadFeedKeyType = ReadFeedKeyType,
			RemainingTimeInMsOnClientRequest = RemainingTimeInMsOnClientRequest,
			RemoteStorageType = RemoteStorageType,
			RequestedCollectionType = RequestedCollectionType,
			ResourceId = ResourceId,
			ResourceSchemaName = ResourceSchemaName,
			ResourceTokenExpiry = ResourceTokenExpiry,
			ResourceTypes = ResourceTypes,
			ResponseContinuationTokenLimitInKB = ResponseContinuationTokenLimitInKB,
			RestoreMetadataFilter = RestoreMetadataFilter,
			RestoreParams = RestoreParams,
			RetriableWriteRequestId = RetriableWriteRequestId,
			RetriableWriteRequestStartTimestamp = RetriableWriteRequestStartTimestamp,
			SchemaHash = SchemaHash,
			SchemaId = SchemaId,
			SchemaOwnerRid = SchemaOwnerRid,
			SDKSupportedCapabilities = SDKSupportedCapabilities,
			SecondaryMasterKey = SecondaryMasterKey,
			SecondaryReadonlyKey = SecondaryReadonlyKey,
			SessionToken = SessionToken,
			SetMasterResourcesDeletionPending = SetMasterResourcesDeletionPending,
			ShareThroughput = ShareThroughput,
			ShouldBatchContinueOnError = ShouldBatchContinueOnError,
			ShouldReturnCurrentServerDateTime = ShouldReturnCurrentServerDateTime,
			SkipAdjustThroughputFractionsForOfferReplace = SkipAdjustThroughputFractionsForOfferReplace,
			SkipRefreshDatabaseAccountConfigs = SkipRefreshDatabaseAccountConfigs,
			SourceCollectionIfMatch = SourceCollectionIfMatch,
			SqlQueryForPartitionKeyExtraction = SqlQueryForPartitionKeyExtraction,
			StartEpk = StartEpk,
			StartId = StartId,
			SupportedQueryFeatures = SupportedQueryFeatures,
			SupportedSerializationFormats = SupportedSerializationFormats,
			SupportSpatialLegacyCoordinates = SupportSpatialLegacyCoordinates,
			SystemDocumentType = SystemDocumentType,
			SystemRestoreOperation = SystemRestoreOperation,
			TargetGlobalCommittedLsn = TargetGlobalCommittedLsn,
			TargetLsn = TargetLsn,
			TimeToLiveInSeconds = TimeToLiveInSeconds,
			TraceParent = TraceParent,
			TraceState = TraceState,
			TransactionCommit = TransactionCommit,
			TransactionFirstRequest = TransactionFirstRequest,
			TransactionId = TransactionId,
			TransportRequestID = TransportRequestID,
			TruncateMergeLogRequest = TruncateMergeLogRequest,
			UniqueIndexNameEncodingMode = UniqueIndexNameEncodingMode,
			UniqueIndexReIndexingState = UniqueIndexReIndexingState,
			UpdateMaxThroughputEverProvisioned = UpdateMaxThroughputEverProvisioned,
			UpdateOfferStateToPending = UpdateOfferStateToPending,
			UpdateOfferStateToRestorePending = UpdateOfferStateToRestorePending,
			UseArchivalPartition = UseArchivalPartition,
			UsePolygonsSmallerThanAHemisphere = UsePolygonsSmallerThanAHemisphere,
			UseSystemBudget = UseSystemBudget,
			UseUserBackgroundBudget = UseUserBackgroundBudget,
			Version = Version,
			XDate = XDate
		};
		if (notCommonHeaders != null)
		{
			requestNameValueCollection.notCommonHeaders = new Dictionary<string, string>(notCommonHeaders, DefaultStringComparer);
		}
		return requestNameValueCollection;
	}

	public int Count()
	{
		return Keys().Count();
	}

	public IEnumerator GetEnumerator()
	{
		return Keys().GetEnumerator();
	}

	public string[] GetValues(string key)
	{
		string text = Get(key);
		if (text != null)
		{
			return new string[1] { text };
		}
		return null;
	}

	public IEnumerable<string> Keys()
	{
		if (ResourceId != null)
		{
			yield return "x-docdb-resource-id";
		}
		if (Authorization != null)
		{
			yield return "authorization";
		}
		if (HttpDate != null)
		{
			yield return "date";
		}
		if (XDate != null)
		{
			yield return "x-ms-date";
		}
		if (PageSize != null)
		{
			yield return "x-ms-max-item-count";
		}
		if (SessionToken != null)
		{
			yield return "x-ms-session-token";
		}
		if (Continuation != null)
		{
			yield return "x-ms-continuation";
		}
		if (IndexingDirective != null)
		{
			yield return "x-ms-indexing-directive";
		}
		if (IfNoneMatch != null)
		{
			yield return "If-None-Match";
		}
		if (PreTriggerInclude != null)
		{
			yield return "x-ms-documentdb-pre-trigger-include";
		}
		if (PostTriggerInclude != null)
		{
			yield return "x-ms-documentdb-post-trigger-include";
		}
		if (IsFanoutRequest != null)
		{
			yield return "x-ms-is-fanout-request";
		}
		if (CollectionPartitionIndex != null)
		{
			yield return "collection-partition-index";
		}
		if (CollectionServiceIndex != null)
		{
			yield return "collection-service-index";
		}
		if (PreTriggerExclude != null)
		{
			yield return "x-ms-documentdb-pre-trigger-exclude";
		}
		if (PostTriggerExclude != null)
		{
			yield return "x-ms-documentdb-post-trigger-exclude";
		}
		if (ConsistencyLevel != null)
		{
			yield return "x-ms-consistency-level";
		}
		if (EntityId != null)
		{
			yield return "x-docdb-entity-id";
		}
		if (ResourceSchemaName != null)
		{
			yield return "x-ms-resource-schema-name";
		}
		if (ResourceTokenExpiry != null)
		{
			yield return "x-ms-documentdb-expiry-seconds";
		}
		if (EnableScanInQuery != null)
		{
			yield return "x-ms-documentdb-query-enable-scan";
		}
		if (EmitVerboseTracesInQuery != null)
		{
			yield return "x-ms-documentdb-query-emit-traces";
		}
		if (BindReplicaDirective != null)
		{
			yield return "x-ms-bind-replica";
		}
		if (PrimaryMasterKey != null)
		{
			yield return "x-ms-primary-master-key";
		}
		if (SecondaryMasterKey != null)
		{
			yield return "x-ms-secondary-master-key";
		}
		if (PrimaryReadonlyKey != null)
		{
			yield return "x-ms-primary-readonly-key";
		}
		if (SecondaryReadonlyKey != null)
		{
			yield return "x-ms-secondary-readonly-key";
		}
		if (ProfileRequest != null)
		{
			yield return "x-ms-profile-request";
		}
		if (EnableLowPrecisionOrderBy != null)
		{
			yield return "x-ms-documentdb-query-enable-low-precision-order-by";
		}
		if (Version != null)
		{
			yield return "x-ms-version";
		}
		if (CanCharge != null)
		{
			yield return "x-ms-cancharge";
		}
		if (CanThrottle != null)
		{
			yield return "x-ms-canthrottle";
		}
		if (PartitionKey != null)
		{
			yield return "x-ms-documentdb-partitionkey";
		}
		if (PartitionKeyRangeId != null)
		{
			yield return "x-ms-documentdb-partitionkeyrangeid";
		}
		if (MigrateCollectionDirective != null)
		{
			yield return "x-ms-migratecollection-directive";
		}
		if (SupportSpatialLegacyCoordinates != null)
		{
			yield return "x-ms-documentdb-supportspatiallegacycoordinates";
		}
		if (PartitionCount != null)
		{
			yield return "x-ms-documentdb-partitioncount";
		}
		if (CollectionRid != null)
		{
			yield return "x-ms-documentdb-collection-rid";
		}
		if (FilterBySchemaResourceId != null)
		{
			yield return "x-ms-documentdb-filterby-schema-rid";
		}
		if (UsePolygonsSmallerThanAHemisphere != null)
		{
			yield return "x-ms-documentdb-usepolygonssmallerthanahemisphere";
		}
		if (GatewaySignature != null)
		{
			yield return "x-ms-gateway-signature";
		}
		if (EnableLogging != null)
		{
			yield return "x-ms-documentdb-script-enable-logging";
		}
		if (A_IM != null)
		{
			yield return "A-IM";
		}
		if (PopulateQuotaInfo != null)
		{
			yield return "x-ms-documentdb-populatequotainfo";
		}
		if (DisableRUPerMinuteUsage != null)
		{
			yield return "x-ms-documentdb-disable-ru-per-minute-usage";
		}
		if (PopulateQueryMetrics != null)
		{
			yield return "x-ms-documentdb-populatequerymetrics";
		}
		if (ResponseContinuationTokenLimitInKB != null)
		{
			yield return "x-ms-documentdb-responsecontinuationtokenlimitinkb";
		}
		if (PopulatePartitionStatistics != null)
		{
			yield return "x-ms-documentdb-populatepartitionstatistics";
		}
		if (RemoteStorageType != null)
		{
			yield return "x-ms-remote-storage-type";
		}
		if (CollectionRemoteStorageSecurityIdentifier != null)
		{
			yield return "x-ms-collection-security-identifier";
		}
		if (IfModifiedSince != null)
		{
			yield return "If-Modified-Since";
		}
		if (PopulateCollectionThroughputInfo != null)
		{
			yield return "x-ms-documentdb-populatecollectionthroughputinfo";
		}
		if (RemainingTimeInMsOnClientRequest != null)
		{
			yield return "x-ms-remaining-time-in-ms-on-client";
		}
		if (ClientRetryAttemptCount != null)
		{
			yield return "x-ms-client-retry-attempt-count";
		}
		if (TargetLsn != null)
		{
			yield return "x-ms-target-lsn";
		}
		if (TargetGlobalCommittedLsn != null)
		{
			yield return "x-ms-target-global-committed-lsn";
		}
		if (TransportRequestID != null)
		{
			yield return "x-ms-transport-request-id";
		}
		if (RestoreMetadataFilter != null)
		{
			yield return "x-ms-restore-metadata-filter";
		}
		if (RestoreParams != null)
		{
			yield return "x-ms-restore-params";
		}
		if (ShareThroughput != null)
		{
			yield return "x-ms-share-throughput";
		}
		if (PartitionResourceFilter != null)
		{
			yield return "x-ms-partition-resource-filter";
		}
		if (IsReadOnlyScript != null)
		{
			yield return "x-ms-is-readonly-script";
		}
		if (IsAutoScaleRequest != null)
		{
			yield return "x-ms-is-auto-scale";
		}
		if (ForceQueryScan != null)
		{
			yield return "x-ms-documentdb-force-query-scan";
		}
		if (CanOfferReplaceComplete != null)
		{
			yield return "x-ms-can-offer-replace-complete";
		}
		if (ExcludeSystemProperties != null)
		{
			yield return "x-ms-exclude-system-properties";
		}
		if (BinaryId != null)
		{
			yield return "x-ms-binary-id";
		}
		if (TimeToLiveInSeconds != null)
		{
			yield return "x-ms-time-to-live-in-seconds";
		}
		if (EffectivePartitionKey != null)
		{
			yield return "x-ms-effective-partition-key";
		}
		if (BinaryPassthroughRequest != null)
		{
			yield return "x-ms-binary-passthrough-request";
		}
		if (EnableDynamicRidRangeAllocation != null)
		{
			yield return "x-ms-enable-dynamic-rid-range-allocation";
		}
		if (EnumerationDirection != null)
		{
			yield return "x-ms-enumeration-direction";
		}
		if (StartId != null)
		{
			yield return "x-ms-start-id";
		}
		if (EndId != null)
		{
			yield return "x-ms-end-id";
		}
		if (FanoutOperationState != null)
		{
			yield return "x-ms-fanout-operation-state";
		}
		if (StartEpk != null)
		{
			yield return "x-ms-start-epk";
		}
		if (EndEpk != null)
		{
			yield return "x-ms-end-epk";
		}
		if (ReadFeedKeyType != null)
		{
			yield return "x-ms-read-key-type";
		}
		if (ContentSerializationFormat != null)
		{
			yield return "x-ms-documentdb-content-serialization-format";
		}
		if (AllowTentativeWrites != null)
		{
			yield return "x-ms-cosmos-allow-tentative-writes";
		}
		if (IsUserRequest != null)
		{
			yield return "x-ms-cosmos-internal-is-user-request";
		}
		if (PreserveFullContent != null)
		{
			yield return "x-ms-cosmos-preserve-full-content";
		}
		if (IncludeTentativeWrites != null)
		{
			yield return "x-ms-cosmos-include-tentative-writes";
		}
		if (PopulateResourceCount != null)
		{
			yield return "x-ms-documentdb-populateresourcecount";
		}
		if (MergeStaticId != null)
		{
			yield return "x-ms-cosmos-merge-static-id";
		}
		if (IsBatchAtomic != null)
		{
			yield return "x-ms-cosmos-batch-atomic";
		}
		if (ShouldBatchContinueOnError != null)
		{
			yield return "x-ms-cosmos-batch-continue-on-error";
		}
		if (IsBatchOrdered != null)
		{
			yield return "x-ms-cosmos-batch-ordered";
		}
		if (SchemaOwnerRid != null)
		{
			yield return "x-ms-schema-owner-rid";
		}
		if (SchemaHash != null)
		{
			yield return "x-ms-schema-hash";
		}
		if (IsRUPerGBEnforcementRequest != null)
		{
			yield return "x-ms-cosmos-internal-is-ru-per-gb-enforcement-request";
		}
		if (MaxPollingIntervalMilliseconds != null)
		{
			yield return "x-ms-cosmos-max-polling-interval";
		}
		if (PopulateLogStoreInfo != null)
		{
			yield return "x-ms-cosmos-populate-logstoreinfo";
		}
		if (GetAllPartitionKeyStatistics != null)
		{
			yield return "x-ms-cosmos-internal-get-all-partition-key-stats";
		}
		if (ForceSideBySideIndexMigration != null)
		{
			yield return "x-ms-cosmos-force-sidebyside-indexmigration";
		}
		if (CollectionChildResourceNameLimitInBytes != null)
		{
			yield return "x-ms-cosmos-collection-child-resourcename-limit";
		}
		if (CollectionChildResourceContentLimitInKB != null)
		{
			yield return "x-ms-cosmos-collection-child-contentlength-resourcelimit";
		}
		if (MergeCheckPointGLSN != null)
		{
			yield return "x-ms-cosmos-internal-merge-checkpoint-glsn";
		}
		if (Prefer != null)
		{
			yield return "Prefer";
		}
		if (UniqueIndexNameEncodingMode != null)
		{
			yield return "x-ms-cosmos-unique-index-name-encoding-mode";
		}
		if (PopulateUnflushedMergeEntryCount != null)
		{
			yield return "x-ms-cosmos-internal-populate-unflushed-merge-entry-count";
		}
		if (MigrateOfferToManualThroughput != null)
		{
			yield return "x-ms-cosmos-migrate-offer-to-manual-throughput";
		}
		if (MigrateOfferToAutopilot != null)
		{
			yield return "x-ms-cosmos-migrate-offer-to-autopilot";
		}
		if (IsClientEncrypted != null)
		{
			yield return "x-ms-cosmos-is-client-encrypted";
		}
		if (SystemDocumentType != null)
		{
			yield return "x-ms-cosmos-systemdocument-type";
		}
		if (IsOfferStorageRefreshRequest != null)
		{
			yield return "x-ms-cosmos-internal-is-offer-storage-refresh-request";
		}
		if (ResourceTypes != null)
		{
			yield return "x-ms-cosmos-resourcetypes";
		}
		if (TransactionId != null)
		{
			yield return "x-ms-cosmos-tx-id";
		}
		if (TransactionFirstRequest != null)
		{
			yield return "x-ms-cosmos-tx-init";
		}
		if (TransactionCommit != null)
		{
			yield return "x-ms-cosmos-tx-commit";
		}
		if (UpdateMaxThroughputEverProvisioned != null)
		{
			yield return "x-ms-cosmos-internal-update-max-throughput-ever-provisioned";
		}
		if (UniqueIndexReIndexingState != null)
		{
			yield return "x-ms-cosmos-uniqueindex-reindexing-state";
		}
		if (UseSystemBudget != null)
		{
			yield return "x-ms-cosmos-use-systembudget";
		}
		if (IgnoreSystemLoweringMaxThroughput != null)
		{
			yield return "x-ms-cosmos-internal-ignore-system-lowering-max-throughput";
		}
		if (TruncateMergeLogRequest != null)
		{
			yield return "x-ms-cosmos-internal-truncate-merge-log";
		}
		if (RetriableWriteRequestId != null)
		{
			yield return "x-ms-cosmos-retriable-write-request-id";
		}
		if (IsRetriedWriteRequest != null)
		{
			yield return "x-ms-cosmos-is-retried-write-request";
		}
		if (RetriableWriteRequestStartTimestamp != null)
		{
			yield return "x-ms-cosmos-retriable-write-request-start-timestamp";
		}
		if (AddResourcePropertiesToResponse != null)
		{
			yield return "x-ms-cosmos-add-resource-properties-to-response";
		}
		if (ChangeFeedStartFullFidelityIfNoneMatch != null)
		{
			yield return "x-ms-cosmos-start-full-fidelity-if-none-match";
		}
		if (SystemRestoreOperation != null)
		{
			yield return "x-ms-cosmos-internal-system-restore-operation";
		}
		if (SkipRefreshDatabaseAccountConfigs != null)
		{
			yield return "x-ms-cosmos-skip-refresh-databaseaccountconfig";
		}
		if (IntendedCollectionRid != null)
		{
			yield return "x-ms-cosmos-intended-collection-rid";
		}
		if (UseArchivalPartition != null)
		{
			yield return "x-ms-cosmos-use-archival-partition";
		}
		if (PopulateUniqueIndexReIndexProgress != null)
		{
			yield return "x-ms-cosmosdb-populateuniqueindexreindexprogress";
		}
		if (SchemaId != null)
		{
			yield return "x-ms-schema-id";
		}
		if (CollectionTruncate != null)
		{
			yield return "x-ms-cosmos-collection-truncate";
		}
		if (SDKSupportedCapabilities != null)
		{
			yield return "x-ms-cosmos-sdk-supportedcapabilities";
		}
		if (IsMaterializedViewBuild != null)
		{
			yield return "x-ms-cosmos-internal-is-materialized-view-build";
		}
		if (BuilderClientIdentifier != null)
		{
			yield return "x-ms-cosmos-builder-client-identifier";
		}
		if (SourceCollectionIfMatch != null)
		{
			yield return "x-ms-cosmos-source-collection-if-match";
		}
		if (RequestedCollectionType != null)
		{
			yield return "x-ms-cosmos-collectiontype";
		}
		if (PopulateIndexMetrics != null)
		{
			yield return "x-ms-cosmos-populateindexmetrics";
		}
		if (PopulateAnalyticalMigrationProgress != null)
		{
			yield return "x-ms-cosmos-populate-analytical-migration-progress";
		}
		if (ShouldReturnCurrentServerDateTime != null)
		{
			yield return "x-ms-should-return-current-server-datetime";
		}
		if (RbacUserId != null)
		{
			yield return "x-ms-rbac-user-id";
		}
		if (RbacAction != null)
		{
			yield return "x-ms-rbac-action";
		}
		if (RbacResource != null)
		{
			yield return "x-ms-rbac-resource";
		}
		if (CorrelatedActivityId != null)
		{
			yield return "x-ms-cosmos-correlated-activityid";
		}
		if (IsThroughputCapRequest != null)
		{
			yield return "x-ms-cosmos-internal-is-throughputcap-request";
		}
		if (ChangeFeedWireFormatVersion != null)
		{
			yield return "x-ms-cosmos-changefeed-wire-format-version";
		}
		if (PopulateByokEncryptionProgress != null)
		{
			yield return "x-ms-cosmos-populate-byok-encryption-progress";
		}
		if (UseUserBackgroundBudget != null)
		{
			yield return "x-ms-cosmos-use-background-task-budget";
		}
		if (IncludePhysicalPartitionThroughputInfo != null)
		{
			yield return "x-ms-cosmos-include-physical-partition-throughput-info";
		}
		if (IsServerlessStorageRefreshRequest != null)
		{
			yield return "x-ms-cosmos-internal-serverless-offer-storage-refresh-request";
		}
		if (UpdateOfferStateToPending != null)
		{
			yield return "x-ms-cosmos-internal-update-offer-state-to-pending";
		}
		if (PopulateOldestActiveSchemaId != null)
		{
			yield return "x-ms-cosmos-populate-oldest-active-schema-id";
		}
		if (IsInternalServerlessRequest != null)
		{
			yield return "x-ms-cosmos-internal-serverless-request";
		}
		if (OfferReplaceRURedistribution != null)
		{
			yield return "x-ms-cosmos-internal-offer-replace-ru-redistribution";
		}
		if (IsCassandraAlterTypeRequest != null)
		{
			yield return "x-ms-cosmos-alter-type-request";
		}
		if (IsMaterializedViewSourceSchemaReplaceBatchRequest != null)
		{
			yield return "x-ms-cosmos-is-materialized-view-source-schema-replace";
		}
		if (ForceDatabaseAccountUpdate != null)
		{
			yield return "x-ms-cosmos-force-database-account-update";
		}
		if (PriorityLevel != null)
		{
			yield return "x-ms-cosmos-priority-level";
		}
		if (AllowRestoreParamsUpdate != null)
		{
			yield return "x-ms-cosmos-internal-allow-restore-params-update";
		}
		if (PruneCollectionSchemas != null)
		{
			yield return "x-ms-cosmos-prune-collection-schemas";
		}
		if (PopulateIndexMetricsV2 != null)
		{
			yield return "x-ms-cosmos-populateindexmetrics-V2";
		}
		if (IsMigratedFixedCollection != null)
		{
			yield return "x-ms-cosmos-internal-migrated-fixed-collection";
		}
		if (SupportedSerializationFormats != null)
		{
			yield return "x-ms-cosmos-supported-serialization-formats";
		}
		if (UpdateOfferStateToRestorePending != null)
		{
			yield return "x-ms-cosmos-internal-update-offer-state-restore-pending";
		}
		if (SetMasterResourcesDeletionPending != null)
		{
			yield return "x-ms-cosmos-internal-set-master-resources-deletion-pending";
		}
		if (HighPriorityForcedBackup != null)
		{
			yield return "x-ms-cosmos-internal-high-priority-forced-backup";
		}
		if (OptimisticDirectExecute != null)
		{
			yield return "x-ms-cosmos-query-optimisticdirectexecute";
		}
		if (PopulateMinGLSNForDocumentOperations != null)
		{
			yield return "x-ms-cosmos-internal-populate-min-glsn-for-relocation";
		}
		if (PopulateHighestTentativeWriteLLSN != null)
		{
			yield return "x-ms-cosmos-internal-populate-highest-tentative-write-llsn";
		}
		if (PopulateCapacityType != null)
		{
			yield return "x-ms-cosmos-populate-capacity-type";
		}
		if (TraceParent != null)
		{
			yield return "traceparent";
		}
		if (TraceState != null)
		{
			yield return "tracestate";
		}
		if (EnableConflictResolutionPolicyUpdate != null)
		{
			yield return "x-ms-cosmos-internal-enable-conflictresolutionpolicy-update";
		}
		if (ClientIpAddress != null)
		{
			yield return "x-ms-cosmos-client-ip-address";
		}
		if (IsRequestNotAuthorized != null)
		{
			yield return "x-ms-cosmos-is-request-not-authorized";
		}
		if (AllowDocumentReadsInOfflineRegion != null)
		{
			yield return "x-ms-cosmos-internal-allow-document-reads-in-offline-region";
		}
		if (PopulateCurrentPartitionThroughputInfo != null)
		{
			yield return "x-ms-cosmos-populate-current-partition-throughput-info";
		}
		if (PopulateDocumentRecordCount != null)
		{
			yield return "x-ms-cosmos-internal-populate-document-record-count";
		}
		if (IfMatch != null)
		{
			yield return "If-Match";
		}
		if (NoRetryOn449StatusCode != null)
		{
			yield return "x-ms-noretry-449";
		}
		if (SkipAdjustThroughputFractionsForOfferReplace != null)
		{
			yield return "x-ms-cosmos-skip-adjust-throughput-fractions-for-offer-replace";
		}
		if (SqlQueryForPartitionKeyExtraction != null)
		{
			yield return "x-ms-documentdb-query-sqlqueryforpartitionkeyextraction";
		}
		if (EnableCrossPartitionQuery != null)
		{
			yield return "x-ms-documentdb-query-enablecrosspartition";
		}
		if (IsContinuationExpected != null)
		{
			yield return "x-ms-documentdb-query-iscontinuationexpected";
		}
		if (ParallelizeCrossPartitionQuery != null)
		{
			yield return "x-ms-documentdb-query-parallelizecrosspartitionquery";
		}
		if (SupportedQueryFeatures != null)
		{
			yield return "x-ms-cosmos-supported-query-features";
		}
		if (QueryVersion != null)
		{
			yield return "x-ms-cosmos-query-version";
		}
		if (ActivityId != null)
		{
			yield return "x-ms-activity-id";
		}
		if (notCommonHeaders == null)
		{
			yield break;
		}
		foreach (string key in notCommonHeaders.Keys)
		{
			yield return key;
		}
	}

	public NameValueCollection ToNameValueCollection()
	{
		if (nameValueCollection == null)
		{
			lock (this)
			{
				if (nameValueCollection == null)
				{
					nameValueCollection = new NameValueCollection(Count(), DefaultStringComparer);
					if (ResourceId != null)
					{
						nameValueCollection.Add("x-docdb-resource-id", ResourceId);
					}
					if (Authorization != null)
					{
						nameValueCollection.Add("authorization", Authorization);
					}
					if (HttpDate != null)
					{
						nameValueCollection.Add("date", HttpDate);
					}
					if (XDate != null)
					{
						nameValueCollection.Add("x-ms-date", XDate);
					}
					if (PageSize != null)
					{
						nameValueCollection.Add("x-ms-max-item-count", PageSize);
					}
					if (SessionToken != null)
					{
						nameValueCollection.Add("x-ms-session-token", SessionToken);
					}
					if (Continuation != null)
					{
						nameValueCollection.Add("x-ms-continuation", Continuation);
					}
					if (IndexingDirective != null)
					{
						nameValueCollection.Add("x-ms-indexing-directive", IndexingDirective);
					}
					if (IfNoneMatch != null)
					{
						nameValueCollection.Add("If-None-Match", IfNoneMatch);
					}
					if (PreTriggerInclude != null)
					{
						nameValueCollection.Add("x-ms-documentdb-pre-trigger-include", PreTriggerInclude);
					}
					if (PostTriggerInclude != null)
					{
						nameValueCollection.Add("x-ms-documentdb-post-trigger-include", PostTriggerInclude);
					}
					if (IsFanoutRequest != null)
					{
						nameValueCollection.Add("x-ms-is-fanout-request", IsFanoutRequest);
					}
					if (CollectionPartitionIndex != null)
					{
						nameValueCollection.Add("collection-partition-index", CollectionPartitionIndex);
					}
					if (CollectionServiceIndex != null)
					{
						nameValueCollection.Add("collection-service-index", CollectionServiceIndex);
					}
					if (PreTriggerExclude != null)
					{
						nameValueCollection.Add("x-ms-documentdb-pre-trigger-exclude", PreTriggerExclude);
					}
					if (PostTriggerExclude != null)
					{
						nameValueCollection.Add("x-ms-documentdb-post-trigger-exclude", PostTriggerExclude);
					}
					if (ConsistencyLevel != null)
					{
						nameValueCollection.Add("x-ms-consistency-level", ConsistencyLevel);
					}
					if (EntityId != null)
					{
						nameValueCollection.Add("x-docdb-entity-id", EntityId);
					}
					if (ResourceSchemaName != null)
					{
						nameValueCollection.Add("x-ms-resource-schema-name", ResourceSchemaName);
					}
					if (ResourceTokenExpiry != null)
					{
						nameValueCollection.Add("x-ms-documentdb-expiry-seconds", ResourceTokenExpiry);
					}
					if (EnableScanInQuery != null)
					{
						nameValueCollection.Add("x-ms-documentdb-query-enable-scan", EnableScanInQuery);
					}
					if (EmitVerboseTracesInQuery != null)
					{
						nameValueCollection.Add("x-ms-documentdb-query-emit-traces", EmitVerboseTracesInQuery);
					}
					if (BindReplicaDirective != null)
					{
						nameValueCollection.Add("x-ms-bind-replica", BindReplicaDirective);
					}
					if (PrimaryMasterKey != null)
					{
						nameValueCollection.Add("x-ms-primary-master-key", PrimaryMasterKey);
					}
					if (SecondaryMasterKey != null)
					{
						nameValueCollection.Add("x-ms-secondary-master-key", SecondaryMasterKey);
					}
					if (PrimaryReadonlyKey != null)
					{
						nameValueCollection.Add("x-ms-primary-readonly-key", PrimaryReadonlyKey);
					}
					if (SecondaryReadonlyKey != null)
					{
						nameValueCollection.Add("x-ms-secondary-readonly-key", SecondaryReadonlyKey);
					}
					if (ProfileRequest != null)
					{
						nameValueCollection.Add("x-ms-profile-request", ProfileRequest);
					}
					if (EnableLowPrecisionOrderBy != null)
					{
						nameValueCollection.Add("x-ms-documentdb-query-enable-low-precision-order-by", EnableLowPrecisionOrderBy);
					}
					if (Version != null)
					{
						nameValueCollection.Add("x-ms-version", Version);
					}
					if (CanCharge != null)
					{
						nameValueCollection.Add("x-ms-cancharge", CanCharge);
					}
					if (CanThrottle != null)
					{
						nameValueCollection.Add("x-ms-canthrottle", CanThrottle);
					}
					if (PartitionKey != null)
					{
						nameValueCollection.Add("x-ms-documentdb-partitionkey", PartitionKey);
					}
					if (PartitionKeyRangeId != null)
					{
						nameValueCollection.Add("x-ms-documentdb-partitionkeyrangeid", PartitionKeyRangeId);
					}
					if (MigrateCollectionDirective != null)
					{
						nameValueCollection.Add("x-ms-migratecollection-directive", MigrateCollectionDirective);
					}
					if (SupportSpatialLegacyCoordinates != null)
					{
						nameValueCollection.Add("x-ms-documentdb-supportspatiallegacycoordinates", SupportSpatialLegacyCoordinates);
					}
					if (PartitionCount != null)
					{
						nameValueCollection.Add("x-ms-documentdb-partitioncount", PartitionCount);
					}
					if (CollectionRid != null)
					{
						nameValueCollection.Add("x-ms-documentdb-collection-rid", CollectionRid);
					}
					if (FilterBySchemaResourceId != null)
					{
						nameValueCollection.Add("x-ms-documentdb-filterby-schema-rid", FilterBySchemaResourceId);
					}
					if (UsePolygonsSmallerThanAHemisphere != null)
					{
						nameValueCollection.Add("x-ms-documentdb-usepolygonssmallerthanahemisphere", UsePolygonsSmallerThanAHemisphere);
					}
					if (GatewaySignature != null)
					{
						nameValueCollection.Add("x-ms-gateway-signature", GatewaySignature);
					}
					if (EnableLogging != null)
					{
						nameValueCollection.Add("x-ms-documentdb-script-enable-logging", EnableLogging);
					}
					if (A_IM != null)
					{
						nameValueCollection.Add("A-IM", A_IM);
					}
					if (PopulateQuotaInfo != null)
					{
						nameValueCollection.Add("x-ms-documentdb-populatequotainfo", PopulateQuotaInfo);
					}
					if (DisableRUPerMinuteUsage != null)
					{
						nameValueCollection.Add("x-ms-documentdb-disable-ru-per-minute-usage", DisableRUPerMinuteUsage);
					}
					if (PopulateQueryMetrics != null)
					{
						nameValueCollection.Add("x-ms-documentdb-populatequerymetrics", PopulateQueryMetrics);
					}
					if (ResponseContinuationTokenLimitInKB != null)
					{
						nameValueCollection.Add("x-ms-documentdb-responsecontinuationtokenlimitinkb", ResponseContinuationTokenLimitInKB);
					}
					if (PopulatePartitionStatistics != null)
					{
						nameValueCollection.Add("x-ms-documentdb-populatepartitionstatistics", PopulatePartitionStatistics);
					}
					if (RemoteStorageType != null)
					{
						nameValueCollection.Add("x-ms-remote-storage-type", RemoteStorageType);
					}
					if (CollectionRemoteStorageSecurityIdentifier != null)
					{
						nameValueCollection.Add("x-ms-collection-security-identifier", CollectionRemoteStorageSecurityIdentifier);
					}
					if (IfModifiedSince != null)
					{
						nameValueCollection.Add("If-Modified-Since", IfModifiedSince);
					}
					if (PopulateCollectionThroughputInfo != null)
					{
						nameValueCollection.Add("x-ms-documentdb-populatecollectionthroughputinfo", PopulateCollectionThroughputInfo);
					}
					if (RemainingTimeInMsOnClientRequest != null)
					{
						nameValueCollection.Add("x-ms-remaining-time-in-ms-on-client", RemainingTimeInMsOnClientRequest);
					}
					if (ClientRetryAttemptCount != null)
					{
						nameValueCollection.Add("x-ms-client-retry-attempt-count", ClientRetryAttemptCount);
					}
					if (TargetLsn != null)
					{
						nameValueCollection.Add("x-ms-target-lsn", TargetLsn);
					}
					if (TargetGlobalCommittedLsn != null)
					{
						nameValueCollection.Add("x-ms-target-global-committed-lsn", TargetGlobalCommittedLsn);
					}
					if (TransportRequestID != null)
					{
						nameValueCollection.Add("x-ms-transport-request-id", TransportRequestID);
					}
					if (RestoreMetadataFilter != null)
					{
						nameValueCollection.Add("x-ms-restore-metadata-filter", RestoreMetadataFilter);
					}
					if (RestoreParams != null)
					{
						nameValueCollection.Add("x-ms-restore-params", RestoreParams);
					}
					if (ShareThroughput != null)
					{
						nameValueCollection.Add("x-ms-share-throughput", ShareThroughput);
					}
					if (PartitionResourceFilter != null)
					{
						nameValueCollection.Add("x-ms-partition-resource-filter", PartitionResourceFilter);
					}
					if (IsReadOnlyScript != null)
					{
						nameValueCollection.Add("x-ms-is-readonly-script", IsReadOnlyScript);
					}
					if (IsAutoScaleRequest != null)
					{
						nameValueCollection.Add("x-ms-is-auto-scale", IsAutoScaleRequest);
					}
					if (ForceQueryScan != null)
					{
						nameValueCollection.Add("x-ms-documentdb-force-query-scan", ForceQueryScan);
					}
					if (CanOfferReplaceComplete != null)
					{
						nameValueCollection.Add("x-ms-can-offer-replace-complete", CanOfferReplaceComplete);
					}
					if (ExcludeSystemProperties != null)
					{
						nameValueCollection.Add("x-ms-exclude-system-properties", ExcludeSystemProperties);
					}
					if (BinaryId != null)
					{
						nameValueCollection.Add("x-ms-binary-id", BinaryId);
					}
					if (TimeToLiveInSeconds != null)
					{
						nameValueCollection.Add("x-ms-time-to-live-in-seconds", TimeToLiveInSeconds);
					}
					if (EffectivePartitionKey != null)
					{
						nameValueCollection.Add("x-ms-effective-partition-key", EffectivePartitionKey);
					}
					if (BinaryPassthroughRequest != null)
					{
						nameValueCollection.Add("x-ms-binary-passthrough-request", BinaryPassthroughRequest);
					}
					if (EnableDynamicRidRangeAllocation != null)
					{
						nameValueCollection.Add("x-ms-enable-dynamic-rid-range-allocation", EnableDynamicRidRangeAllocation);
					}
					if (EnumerationDirection != null)
					{
						nameValueCollection.Add("x-ms-enumeration-direction", EnumerationDirection);
					}
					if (StartId != null)
					{
						nameValueCollection.Add("x-ms-start-id", StartId);
					}
					if (EndId != null)
					{
						nameValueCollection.Add("x-ms-end-id", EndId);
					}
					if (FanoutOperationState != null)
					{
						nameValueCollection.Add("x-ms-fanout-operation-state", FanoutOperationState);
					}
					if (StartEpk != null)
					{
						nameValueCollection.Add("x-ms-start-epk", StartEpk);
					}
					if (EndEpk != null)
					{
						nameValueCollection.Add("x-ms-end-epk", EndEpk);
					}
					if (ReadFeedKeyType != null)
					{
						nameValueCollection.Add("x-ms-read-key-type", ReadFeedKeyType);
					}
					if (ContentSerializationFormat != null)
					{
						nameValueCollection.Add("x-ms-documentdb-content-serialization-format", ContentSerializationFormat);
					}
					if (AllowTentativeWrites != null)
					{
						nameValueCollection.Add("x-ms-cosmos-allow-tentative-writes", AllowTentativeWrites);
					}
					if (IsUserRequest != null)
					{
						nameValueCollection.Add("x-ms-cosmos-internal-is-user-request", IsUserRequest);
					}
					if (PreserveFullContent != null)
					{
						nameValueCollection.Add("x-ms-cosmos-preserve-full-content", PreserveFullContent);
					}
					if (IncludeTentativeWrites != null)
					{
						nameValueCollection.Add("x-ms-cosmos-include-tentative-writes", IncludeTentativeWrites);
					}
					if (PopulateResourceCount != null)
					{
						nameValueCollection.Add("x-ms-documentdb-populateresourcecount", PopulateResourceCount);
					}
					if (MergeStaticId != null)
					{
						nameValueCollection.Add("x-ms-cosmos-merge-static-id", MergeStaticId);
					}
					if (IsBatchAtomic != null)
					{
						nameValueCollection.Add("x-ms-cosmos-batch-atomic", IsBatchAtomic);
					}
					if (ShouldBatchContinueOnError != null)
					{
						nameValueCollection.Add("x-ms-cosmos-batch-continue-on-error", ShouldBatchContinueOnError);
					}
					if (IsBatchOrdered != null)
					{
						nameValueCollection.Add("x-ms-cosmos-batch-ordered", IsBatchOrdered);
					}
					if (SchemaOwnerRid != null)
					{
						nameValueCollection.Add("x-ms-schema-owner-rid", SchemaOwnerRid);
					}
					if (SchemaHash != null)
					{
						nameValueCollection.Add("x-ms-schema-hash", SchemaHash);
					}
					if (IsRUPerGBEnforcementRequest != null)
					{
						nameValueCollection.Add("x-ms-cosmos-internal-is-ru-per-gb-enforcement-request", IsRUPerGBEnforcementRequest);
					}
					if (MaxPollingIntervalMilliseconds != null)
					{
						nameValueCollection.Add("x-ms-cosmos-max-polling-interval", MaxPollingIntervalMilliseconds);
					}
					if (PopulateLogStoreInfo != null)
					{
						nameValueCollection.Add("x-ms-cosmos-populate-logstoreinfo", PopulateLogStoreInfo);
					}
					if (GetAllPartitionKeyStatistics != null)
					{
						nameValueCollection.Add("x-ms-cosmos-internal-get-all-partition-key-stats", GetAllPartitionKeyStatistics);
					}
					if (ForceSideBySideIndexMigration != null)
					{
						nameValueCollection.Add("x-ms-cosmos-force-sidebyside-indexmigration", ForceSideBySideIndexMigration);
					}
					if (CollectionChildResourceNameLimitInBytes != null)
					{
						nameValueCollection.Add("x-ms-cosmos-collection-child-resourcename-limit", CollectionChildResourceNameLimitInBytes);
					}
					if (CollectionChildResourceContentLimitInKB != null)
					{
						nameValueCollection.Add("x-ms-cosmos-collection-child-contentlength-resourcelimit", CollectionChildResourceContentLimitInKB);
					}
					if (MergeCheckPointGLSN != null)
					{
						nameValueCollection.Add("x-ms-cosmos-internal-merge-checkpoint-glsn", MergeCheckPointGLSN);
					}
					if (Prefer != null)
					{
						nameValueCollection.Add("Prefer", Prefer);
					}
					if (UniqueIndexNameEncodingMode != null)
					{
						nameValueCollection.Add("x-ms-cosmos-unique-index-name-encoding-mode", UniqueIndexNameEncodingMode);
					}
					if (PopulateUnflushedMergeEntryCount != null)
					{
						nameValueCollection.Add("x-ms-cosmos-internal-populate-unflushed-merge-entry-count", PopulateUnflushedMergeEntryCount);
					}
					if (MigrateOfferToManualThroughput != null)
					{
						nameValueCollection.Add("x-ms-cosmos-migrate-offer-to-manual-throughput", MigrateOfferToManualThroughput);
					}
					if (MigrateOfferToAutopilot != null)
					{
						nameValueCollection.Add("x-ms-cosmos-migrate-offer-to-autopilot", MigrateOfferToAutopilot);
					}
					if (IsClientEncrypted != null)
					{
						nameValueCollection.Add("x-ms-cosmos-is-client-encrypted", IsClientEncrypted);
					}
					if (SystemDocumentType != null)
					{
						nameValueCollection.Add("x-ms-cosmos-systemdocument-type", SystemDocumentType);
					}
					if (IsOfferStorageRefreshRequest != null)
					{
						nameValueCollection.Add("x-ms-cosmos-internal-is-offer-storage-refresh-request", IsOfferStorageRefreshRequest);
					}
					if (ResourceTypes != null)
					{
						nameValueCollection.Add("x-ms-cosmos-resourcetypes", ResourceTypes);
					}
					if (TransactionId != null)
					{
						nameValueCollection.Add("x-ms-cosmos-tx-id", TransactionId);
					}
					if (TransactionFirstRequest != null)
					{
						nameValueCollection.Add("x-ms-cosmos-tx-init", TransactionFirstRequest);
					}
					if (TransactionCommit != null)
					{
						nameValueCollection.Add("x-ms-cosmos-tx-commit", TransactionCommit);
					}
					if (UpdateMaxThroughputEverProvisioned != null)
					{
						nameValueCollection.Add("x-ms-cosmos-internal-update-max-throughput-ever-provisioned", UpdateMaxThroughputEverProvisioned);
					}
					if (UniqueIndexReIndexingState != null)
					{
						nameValueCollection.Add("x-ms-cosmos-uniqueindex-reindexing-state", UniqueIndexReIndexingState);
					}
					if (UseSystemBudget != null)
					{
						nameValueCollection.Add("x-ms-cosmos-use-systembudget", UseSystemBudget);
					}
					if (IgnoreSystemLoweringMaxThroughput != null)
					{
						nameValueCollection.Add("x-ms-cosmos-internal-ignore-system-lowering-max-throughput", IgnoreSystemLoweringMaxThroughput);
					}
					if (TruncateMergeLogRequest != null)
					{
						nameValueCollection.Add("x-ms-cosmos-internal-truncate-merge-log", TruncateMergeLogRequest);
					}
					if (RetriableWriteRequestId != null)
					{
						nameValueCollection.Add("x-ms-cosmos-retriable-write-request-id", RetriableWriteRequestId);
					}
					if (IsRetriedWriteRequest != null)
					{
						nameValueCollection.Add("x-ms-cosmos-is-retried-write-request", IsRetriedWriteRequest);
					}
					if (RetriableWriteRequestStartTimestamp != null)
					{
						nameValueCollection.Add("x-ms-cosmos-retriable-write-request-start-timestamp", RetriableWriteRequestStartTimestamp);
					}
					if (AddResourcePropertiesToResponse != null)
					{
						nameValueCollection.Add("x-ms-cosmos-add-resource-properties-to-response", AddResourcePropertiesToResponse);
					}
					if (ChangeFeedStartFullFidelityIfNoneMatch != null)
					{
						nameValueCollection.Add("x-ms-cosmos-start-full-fidelity-if-none-match", ChangeFeedStartFullFidelityIfNoneMatch);
					}
					if (SystemRestoreOperation != null)
					{
						nameValueCollection.Add("x-ms-cosmos-internal-system-restore-operation", SystemRestoreOperation);
					}
					if (SkipRefreshDatabaseAccountConfigs != null)
					{
						nameValueCollection.Add("x-ms-cosmos-skip-refresh-databaseaccountconfig", SkipRefreshDatabaseAccountConfigs);
					}
					if (IntendedCollectionRid != null)
					{
						nameValueCollection.Add("x-ms-cosmos-intended-collection-rid", IntendedCollectionRid);
					}
					if (UseArchivalPartition != null)
					{
						nameValueCollection.Add("x-ms-cosmos-use-archival-partition", UseArchivalPartition);
					}
					if (PopulateUniqueIndexReIndexProgress != null)
					{
						nameValueCollection.Add("x-ms-cosmosdb-populateuniqueindexreindexprogress", PopulateUniqueIndexReIndexProgress);
					}
					if (SchemaId != null)
					{
						nameValueCollection.Add("x-ms-schema-id", SchemaId);
					}
					if (CollectionTruncate != null)
					{
						nameValueCollection.Add("x-ms-cosmos-collection-truncate", CollectionTruncate);
					}
					if (SDKSupportedCapabilities != null)
					{
						nameValueCollection.Add("x-ms-cosmos-sdk-supportedcapabilities", SDKSupportedCapabilities);
					}
					if (IsMaterializedViewBuild != null)
					{
						nameValueCollection.Add("x-ms-cosmos-internal-is-materialized-view-build", IsMaterializedViewBuild);
					}
					if (BuilderClientIdentifier != null)
					{
						nameValueCollection.Add("x-ms-cosmos-builder-client-identifier", BuilderClientIdentifier);
					}
					if (SourceCollectionIfMatch != null)
					{
						nameValueCollection.Add("x-ms-cosmos-source-collection-if-match", SourceCollectionIfMatch);
					}
					if (RequestedCollectionType != null)
					{
						nameValueCollection.Add("x-ms-cosmos-collectiontype", RequestedCollectionType);
					}
					if (PopulateIndexMetrics != null)
					{
						nameValueCollection.Add("x-ms-cosmos-populateindexmetrics", PopulateIndexMetrics);
					}
					if (PopulateAnalyticalMigrationProgress != null)
					{
						nameValueCollection.Add("x-ms-cosmos-populate-analytical-migration-progress", PopulateAnalyticalMigrationProgress);
					}
					if (ShouldReturnCurrentServerDateTime != null)
					{
						nameValueCollection.Add("x-ms-should-return-current-server-datetime", ShouldReturnCurrentServerDateTime);
					}
					if (RbacUserId != null)
					{
						nameValueCollection.Add("x-ms-rbac-user-id", RbacUserId);
					}
					if (RbacAction != null)
					{
						nameValueCollection.Add("x-ms-rbac-action", RbacAction);
					}
					if (RbacResource != null)
					{
						nameValueCollection.Add("x-ms-rbac-resource", RbacResource);
					}
					if (CorrelatedActivityId != null)
					{
						nameValueCollection.Add("x-ms-cosmos-correlated-activityid", CorrelatedActivityId);
					}
					if (IsThroughputCapRequest != null)
					{
						nameValueCollection.Add("x-ms-cosmos-internal-is-throughputcap-request", IsThroughputCapRequest);
					}
					if (ChangeFeedWireFormatVersion != null)
					{
						nameValueCollection.Add("x-ms-cosmos-changefeed-wire-format-version", ChangeFeedWireFormatVersion);
					}
					if (PopulateByokEncryptionProgress != null)
					{
						nameValueCollection.Add("x-ms-cosmos-populate-byok-encryption-progress", PopulateByokEncryptionProgress);
					}
					if (UseUserBackgroundBudget != null)
					{
						nameValueCollection.Add("x-ms-cosmos-use-background-task-budget", UseUserBackgroundBudget);
					}
					if (IncludePhysicalPartitionThroughputInfo != null)
					{
						nameValueCollection.Add("x-ms-cosmos-include-physical-partition-throughput-info", IncludePhysicalPartitionThroughputInfo);
					}
					if (IsServerlessStorageRefreshRequest != null)
					{
						nameValueCollection.Add("x-ms-cosmos-internal-serverless-offer-storage-refresh-request", IsServerlessStorageRefreshRequest);
					}
					if (UpdateOfferStateToPending != null)
					{
						nameValueCollection.Add("x-ms-cosmos-internal-update-offer-state-to-pending", UpdateOfferStateToPending);
					}
					if (PopulateOldestActiveSchemaId != null)
					{
						nameValueCollection.Add("x-ms-cosmos-populate-oldest-active-schema-id", PopulateOldestActiveSchemaId);
					}
					if (IsInternalServerlessRequest != null)
					{
						nameValueCollection.Add("x-ms-cosmos-internal-serverless-request", IsInternalServerlessRequest);
					}
					if (OfferReplaceRURedistribution != null)
					{
						nameValueCollection.Add("x-ms-cosmos-internal-offer-replace-ru-redistribution", OfferReplaceRURedistribution);
					}
					if (IsCassandraAlterTypeRequest != null)
					{
						nameValueCollection.Add("x-ms-cosmos-alter-type-request", IsCassandraAlterTypeRequest);
					}
					if (IsMaterializedViewSourceSchemaReplaceBatchRequest != null)
					{
						nameValueCollection.Add("x-ms-cosmos-is-materialized-view-source-schema-replace", IsMaterializedViewSourceSchemaReplaceBatchRequest);
					}
					if (ForceDatabaseAccountUpdate != null)
					{
						nameValueCollection.Add("x-ms-cosmos-force-database-account-update", ForceDatabaseAccountUpdate);
					}
					if (PriorityLevel != null)
					{
						nameValueCollection.Add("x-ms-cosmos-priority-level", PriorityLevel);
					}
					if (AllowRestoreParamsUpdate != null)
					{
						nameValueCollection.Add("x-ms-cosmos-internal-allow-restore-params-update", AllowRestoreParamsUpdate);
					}
					if (PruneCollectionSchemas != null)
					{
						nameValueCollection.Add("x-ms-cosmos-prune-collection-schemas", PruneCollectionSchemas);
					}
					if (PopulateIndexMetricsV2 != null)
					{
						nameValueCollection.Add("x-ms-cosmos-populateindexmetrics-V2", PopulateIndexMetricsV2);
					}
					if (IsMigratedFixedCollection != null)
					{
						nameValueCollection.Add("x-ms-cosmos-internal-migrated-fixed-collection", IsMigratedFixedCollection);
					}
					if (SupportedSerializationFormats != null)
					{
						nameValueCollection.Add("x-ms-cosmos-supported-serialization-formats", SupportedSerializationFormats);
					}
					if (UpdateOfferStateToRestorePending != null)
					{
						nameValueCollection.Add("x-ms-cosmos-internal-update-offer-state-restore-pending", UpdateOfferStateToRestorePending);
					}
					if (SetMasterResourcesDeletionPending != null)
					{
						nameValueCollection.Add("x-ms-cosmos-internal-set-master-resources-deletion-pending", SetMasterResourcesDeletionPending);
					}
					if (HighPriorityForcedBackup != null)
					{
						nameValueCollection.Add("x-ms-cosmos-internal-high-priority-forced-backup", HighPriorityForcedBackup);
					}
					if (OptimisticDirectExecute != null)
					{
						nameValueCollection.Add("x-ms-cosmos-query-optimisticdirectexecute", OptimisticDirectExecute);
					}
					if (PopulateMinGLSNForDocumentOperations != null)
					{
						nameValueCollection.Add("x-ms-cosmos-internal-populate-min-glsn-for-relocation", PopulateMinGLSNForDocumentOperations);
					}
					if (PopulateHighestTentativeWriteLLSN != null)
					{
						nameValueCollection.Add("x-ms-cosmos-internal-populate-highest-tentative-write-llsn", PopulateHighestTentativeWriteLLSN);
					}
					if (PopulateCapacityType != null)
					{
						nameValueCollection.Add("x-ms-cosmos-populate-capacity-type", PopulateCapacityType);
					}
					if (TraceParent != null)
					{
						nameValueCollection.Add("traceparent", TraceParent);
					}
					if (TraceState != null)
					{
						nameValueCollection.Add("tracestate", TraceState);
					}
					if (EnableConflictResolutionPolicyUpdate != null)
					{
						nameValueCollection.Add("x-ms-cosmos-internal-enable-conflictresolutionpolicy-update", EnableConflictResolutionPolicyUpdate);
					}
					if (ClientIpAddress != null)
					{
						nameValueCollection.Add("x-ms-cosmos-client-ip-address", ClientIpAddress);
					}
					if (IsRequestNotAuthorized != null)
					{
						nameValueCollection.Add("x-ms-cosmos-is-request-not-authorized", IsRequestNotAuthorized);
					}
					if (AllowDocumentReadsInOfflineRegion != null)
					{
						nameValueCollection.Add("x-ms-cosmos-internal-allow-document-reads-in-offline-region", AllowDocumentReadsInOfflineRegion);
					}
					if (PopulateCurrentPartitionThroughputInfo != null)
					{
						nameValueCollection.Add("x-ms-cosmos-populate-current-partition-throughput-info", PopulateCurrentPartitionThroughputInfo);
					}
					if (PopulateDocumentRecordCount != null)
					{
						nameValueCollection.Add("x-ms-cosmos-internal-populate-document-record-count", PopulateDocumentRecordCount);
					}
					if (IfMatch != null)
					{
						nameValueCollection.Add("If-Match", IfMatch);
					}
					if (NoRetryOn449StatusCode != null)
					{
						nameValueCollection.Add("x-ms-noretry-449", NoRetryOn449StatusCode);
					}
					if (SkipAdjustThroughputFractionsForOfferReplace != null)
					{
						nameValueCollection.Add("x-ms-cosmos-skip-adjust-throughput-fractions-for-offer-replace", SkipAdjustThroughputFractionsForOfferReplace);
					}
					if (SqlQueryForPartitionKeyExtraction != null)
					{
						nameValueCollection.Add("x-ms-documentdb-query-sqlqueryforpartitionkeyextraction", SqlQueryForPartitionKeyExtraction);
					}
					if (EnableCrossPartitionQuery != null)
					{
						nameValueCollection.Add("x-ms-documentdb-query-enablecrosspartition", EnableCrossPartitionQuery);
					}
					if (IsContinuationExpected != null)
					{
						nameValueCollection.Add("x-ms-documentdb-query-iscontinuationexpected", IsContinuationExpected);
					}
					if (ParallelizeCrossPartitionQuery != null)
					{
						nameValueCollection.Add("x-ms-documentdb-query-parallelizecrosspartitionquery", ParallelizeCrossPartitionQuery);
					}
					if (SupportedQueryFeatures != null)
					{
						nameValueCollection.Add("x-ms-cosmos-supported-query-features", SupportedQueryFeatures);
					}
					if (QueryVersion != null)
					{
						nameValueCollection.Add("x-ms-cosmos-query-version", QueryVersion);
					}
					if (ActivityId != null)
					{
						nameValueCollection.Add("x-ms-activity-id", ActivityId);
					}
					if (notCommonHeaders != null)
					{
						foreach (KeyValuePair<string, string> notCommonHeader in notCommonHeaders)
						{
							nameValueCollection.Add(notCommonHeader.Key, notCommonHeader.Value);
						}
					}
				}
			}
		}
		return nameValueCollection;
	}

	public void Remove(string key)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}
		UpdateHelper(key, null, throwIfAlreadyExists: false, ignoreNotCommonHeaders: false);
	}

	public string Get(string key)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}
		switch (key.Length)
		{
		case 4:
			if ((object)"date" == key)
			{
				return HttpDate;
			}
			if ((object)"A-IM" == key)
			{
				return A_IM;
			}
			if (string.Equals("date", key, StringComparison.OrdinalIgnoreCase))
			{
				return HttpDate;
			}
			if (string.Equals("A-IM", key, StringComparison.OrdinalIgnoreCase))
			{
				return A_IM;
			}
			break;
		case 6:
			if (string.Equals("Prefer", key, StringComparison.OrdinalIgnoreCase))
			{
				return Prefer;
			}
			break;
		case 8:
			if (string.Equals("If-Match", key, StringComparison.OrdinalIgnoreCase))
			{
				return IfMatch;
			}
			break;
		case 9:
			if (string.Equals("x-ms-date", key, StringComparison.OrdinalIgnoreCase))
			{
				return XDate;
			}
			break;
		case 10:
			if (string.Equals("tracestate", key, StringComparison.OrdinalIgnoreCase))
			{
				return TraceState;
			}
			break;
		case 11:
			if ((object)"x-ms-end-id" == key)
			{
				return EndId;
			}
			if ((object)"traceparent" == key)
			{
				return TraceParent;
			}
			if (string.Equals("x-ms-end-id", key, StringComparison.OrdinalIgnoreCase))
			{
				return EndId;
			}
			if (string.Equals("traceparent", key, StringComparison.OrdinalIgnoreCase))
			{
				return TraceParent;
			}
			break;
		case 12:
			if ((object)"x-ms-version" == key)
			{
				return Version;
			}
			if ((object)"x-ms-end-epk" == key)
			{
				return EndEpk;
			}
			if (string.Equals("x-ms-version", key, StringComparison.OrdinalIgnoreCase))
			{
				return Version;
			}
			if (string.Equals("x-ms-end-epk", key, StringComparison.OrdinalIgnoreCase))
			{
				return EndEpk;
			}
			break;
		case 13:
			if ((object)"authorization" == key)
			{
				return Authorization;
			}
			if ((object)"If-None-Match" == key)
			{
				return IfNoneMatch;
			}
			if ((object)"x-ms-start-id" == key)
			{
				return StartId;
			}
			if (string.Equals("authorization", key, StringComparison.OrdinalIgnoreCase))
			{
				return Authorization;
			}
			if (string.Equals("If-None-Match", key, StringComparison.OrdinalIgnoreCase))
			{
				return IfNoneMatch;
			}
			if (string.Equals("x-ms-start-id", key, StringComparison.OrdinalIgnoreCase))
			{
				return StartId;
			}
			break;
		case 14:
			if ((object)"x-ms-cancharge" == key)
			{
				return CanCharge;
			}
			if ((object)"x-ms-binary-id" == key)
			{
				return BinaryId;
			}
			if ((object)"x-ms-start-epk" == key)
			{
				return StartEpk;
			}
			if ((object)"x-ms-schema-id" == key)
			{
				return SchemaId;
			}
			if (string.Equals("x-ms-cancharge", key, StringComparison.OrdinalIgnoreCase))
			{
				return CanCharge;
			}
			if (string.Equals("x-ms-binary-id", key, StringComparison.OrdinalIgnoreCase))
			{
				return BinaryId;
			}
			if (string.Equals("x-ms-start-epk", key, StringComparison.OrdinalIgnoreCase))
			{
				return StartEpk;
			}
			if (string.Equals("x-ms-schema-id", key, StringComparison.OrdinalIgnoreCase))
			{
				return SchemaId;
			}
			break;
		case 15:
			if (string.Equals("x-ms-target-lsn", key, StringComparison.OrdinalIgnoreCase))
			{
				return TargetLsn;
			}
			break;
		case 16:
			if ((object)"x-ms-canthrottle" == key)
			{
				return CanThrottle;
			}
			if ((object)"x-ms-schema-hash" == key)
			{
				return SchemaHash;
			}
			if ((object)"x-ms-rbac-action" == key)
			{
				return RbacAction;
			}
			if ((object)"x-ms-noretry-449" == key)
			{
				return NoRetryOn449StatusCode;
			}
			if ((object)"x-ms-activity-id" == key)
			{
				return ActivityId;
			}
			if (string.Equals("x-ms-canthrottle", key, StringComparison.OrdinalIgnoreCase))
			{
				return CanThrottle;
			}
			if (string.Equals("x-ms-schema-hash", key, StringComparison.OrdinalIgnoreCase))
			{
				return SchemaHash;
			}
			if (string.Equals("x-ms-rbac-action", key, StringComparison.OrdinalIgnoreCase))
			{
				return RbacAction;
			}
			if (string.Equals("x-ms-noretry-449", key, StringComparison.OrdinalIgnoreCase))
			{
				return NoRetryOn449StatusCode;
			}
			if (string.Equals("x-ms-activity-id", key, StringComparison.OrdinalIgnoreCase))
			{
				return ActivityId;
			}
			break;
		case 17:
			if ((object)"x-ms-continuation" == key)
			{
				return Continuation;
			}
			if ((object)"x-docdb-entity-id" == key)
			{
				return EntityId;
			}
			if ((object)"x-ms-bind-replica" == key)
			{
				return BindReplicaDirective;
			}
			if ((object)"If-Modified-Since" == key)
			{
				return IfModifiedSince;
			}
			if ((object)"x-ms-cosmos-tx-id" == key)
			{
				return TransactionId;
			}
			if ((object)"x-ms-rbac-user-id" == key)
			{
				return RbacUserId;
			}
			if (string.Equals("x-ms-continuation", key, StringComparison.OrdinalIgnoreCase))
			{
				return Continuation;
			}
			if (string.Equals("x-docdb-entity-id", key, StringComparison.OrdinalIgnoreCase))
			{
				return EntityId;
			}
			if (string.Equals("x-ms-bind-replica", key, StringComparison.OrdinalIgnoreCase))
			{
				return BindReplicaDirective;
			}
			if (string.Equals("If-Modified-Since", key, StringComparison.OrdinalIgnoreCase))
			{
				return IfModifiedSince;
			}
			if (string.Equals("x-ms-cosmos-tx-id", key, StringComparison.OrdinalIgnoreCase))
			{
				return TransactionId;
			}
			if (string.Equals("x-ms-rbac-user-id", key, StringComparison.OrdinalIgnoreCase))
			{
				return RbacUserId;
			}
			break;
		case 18:
			if ((object)"x-ms-session-token" == key)
			{
				return SessionToken;
			}
			if ((object)"x-ms-is-auto-scale" == key)
			{
				return IsAutoScaleRequest;
			}
			if ((object)"x-ms-read-key-type" == key)
			{
				return ReadFeedKeyType;
			}
			if ((object)"x-ms-rbac-resource" == key)
			{
				return RbacResource;
			}
			if (string.Equals("x-ms-session-token", key, StringComparison.OrdinalIgnoreCase))
			{
				return SessionToken;
			}
			if (string.Equals("x-ms-is-auto-scale", key, StringComparison.OrdinalIgnoreCase))
			{
				return IsAutoScaleRequest;
			}
			if (string.Equals("x-ms-read-key-type", key, StringComparison.OrdinalIgnoreCase))
			{
				return ReadFeedKeyType;
			}
			if (string.Equals("x-ms-rbac-resource", key, StringComparison.OrdinalIgnoreCase))
			{
				return RbacResource;
			}
			break;
		case 19:
			if ((object)"x-docdb-resource-id" == key)
			{
				return ResourceId;
			}
			if ((object)"x-ms-max-item-count" == key)
			{
				return PageSize;
			}
			if ((object)"x-ms-restore-params" == key)
			{
				return RestoreParams;
			}
			if ((object)"x-ms-cosmos-tx-init" == key)
			{
				return TransactionFirstRequest;
			}
			if (string.Equals("x-docdb-resource-id", key, StringComparison.OrdinalIgnoreCase))
			{
				return ResourceId;
			}
			if (string.Equals("x-ms-max-item-count", key, StringComparison.OrdinalIgnoreCase))
			{
				return PageSize;
			}
			if (string.Equals("x-ms-restore-params", key, StringComparison.OrdinalIgnoreCase))
			{
				return RestoreParams;
			}
			if (string.Equals("x-ms-cosmos-tx-init", key, StringComparison.OrdinalIgnoreCase))
			{
				return TransactionFirstRequest;
			}
			break;
		case 20:
			if (string.Equals("x-ms-profile-request", key, StringComparison.OrdinalIgnoreCase))
			{
				return ProfileRequest;
			}
			break;
		case 21:
			if ((object)"x-ms-share-throughput" == key)
			{
				return ShareThroughput;
			}
			if ((object)"x-ms-schema-owner-rid" == key)
			{
				return SchemaOwnerRid;
			}
			if ((object)"x-ms-cosmos-tx-commit" == key)
			{
				return TransactionCommit;
			}
			if (string.Equals("x-ms-share-throughput", key, StringComparison.OrdinalIgnoreCase))
			{
				return ShareThroughput;
			}
			if (string.Equals("x-ms-schema-owner-rid", key, StringComparison.OrdinalIgnoreCase))
			{
				return SchemaOwnerRid;
			}
			if (string.Equals("x-ms-cosmos-tx-commit", key, StringComparison.OrdinalIgnoreCase))
			{
				return TransactionCommit;
			}
			break;
		case 22:
			if ((object)"x-ms-is-fanout-request" == key)
			{
				return IsFanoutRequest;
			}
			if ((object)"x-ms-consistency-level" == key)
			{
				return ConsistencyLevel;
			}
			if ((object)"x-ms-gateway-signature" == key)
			{
				return GatewaySignature;
			}
			if (string.Equals("x-ms-is-fanout-request", key, StringComparison.OrdinalIgnoreCase))
			{
				return IsFanoutRequest;
			}
			if (string.Equals("x-ms-consistency-level", key, StringComparison.OrdinalIgnoreCase))
			{
				return ConsistencyLevel;
			}
			if (string.Equals("x-ms-gateway-signature", key, StringComparison.OrdinalIgnoreCase))
			{
				return GatewaySignature;
			}
			break;
		case 23:
			if ((object)"x-ms-indexing-directive" == key)
			{
				return IndexingDirective;
			}
			if ((object)"x-ms-primary-master-key" == key)
			{
				return PrimaryMasterKey;
			}
			if ((object)"x-ms-is-readonly-script" == key)
			{
				return IsReadOnlyScript;
			}
			if (string.Equals("x-ms-indexing-directive", key, StringComparison.OrdinalIgnoreCase))
			{
				return IndexingDirective;
			}
			if (string.Equals("x-ms-primary-master-key", key, StringComparison.OrdinalIgnoreCase))
			{
				return PrimaryMasterKey;
			}
			if (string.Equals("x-ms-is-readonly-script", key, StringComparison.OrdinalIgnoreCase))
			{
				return IsReadOnlyScript;
			}
			break;
		case 24:
			if ((object)"collection-service-index" == key)
			{
				return CollectionServiceIndex;
			}
			if ((object)"x-ms-remote-storage-type" == key)
			{
				return RemoteStorageType;
			}
			if ((object)"x-ms-cosmos-batch-atomic" == key)
			{
				return IsBatchAtomic;
			}
			if (string.Equals("collection-service-index", key, StringComparison.OrdinalIgnoreCase))
			{
				return CollectionServiceIndex;
			}
			if (string.Equals("x-ms-remote-storage-type", key, StringComparison.OrdinalIgnoreCase))
			{
				return RemoteStorageType;
			}
			if (string.Equals("x-ms-cosmos-batch-atomic", key, StringComparison.OrdinalIgnoreCase))
			{
				return IsBatchAtomic;
			}
			break;
		case 25:
			if ((object)"x-ms-resource-schema-name" == key)
			{
				return ResourceSchemaName;
			}
			if ((object)"x-ms-secondary-master-key" == key)
			{
				return SecondaryMasterKey;
			}
			if ((object)"x-ms-primary-readonly-key" == key)
			{
				return PrimaryReadonlyKey;
			}
			if ((object)"x-ms-transport-request-id" == key)
			{
				return TransportRequestID;
			}
			if ((object)"x-ms-cosmos-batch-ordered" == key)
			{
				return IsBatchOrdered;
			}
			if ((object)"x-ms-cosmos-resourcetypes" == key)
			{
				return ResourceTypes;
			}
			if ((object)"x-ms-cosmos-query-version" == key)
			{
				return QueryVersion;
			}
			if (string.Equals("x-ms-resource-schema-name", key, StringComparison.OrdinalIgnoreCase))
			{
				return ResourceSchemaName;
			}
			if (string.Equals("x-ms-secondary-master-key", key, StringComparison.OrdinalIgnoreCase))
			{
				return SecondaryMasterKey;
			}
			if (string.Equals("x-ms-primary-readonly-key", key, StringComparison.OrdinalIgnoreCase))
			{
				return PrimaryReadonlyKey;
			}
			if (string.Equals("x-ms-transport-request-id", key, StringComparison.OrdinalIgnoreCase))
			{
				return TransportRequestID;
			}
			if (string.Equals("x-ms-cosmos-batch-ordered", key, StringComparison.OrdinalIgnoreCase))
			{
				return IsBatchOrdered;
			}
			if (string.Equals("x-ms-cosmos-resourcetypes", key, StringComparison.OrdinalIgnoreCase))
			{
				return ResourceTypes;
			}
			if (string.Equals("x-ms-cosmos-query-version", key, StringComparison.OrdinalIgnoreCase))
			{
				return QueryVersion;
			}
			break;
		case 26:
			if ((object)"collection-partition-index" == key)
			{
				return CollectionPartitionIndex;
			}
			if ((object)"x-ms-enumeration-direction" == key)
			{
				return EnumerationDirection;
			}
			if ((object)"x-ms-cosmos-collectiontype" == key)
			{
				return RequestedCollectionType;
			}
			if ((object)"x-ms-cosmos-priority-level" == key)
			{
				return PriorityLevel;
			}
			if (string.Equals("collection-partition-index", key, StringComparison.OrdinalIgnoreCase))
			{
				return CollectionPartitionIndex;
			}
			if (string.Equals("x-ms-enumeration-direction", key, StringComparison.OrdinalIgnoreCase))
			{
				return EnumerationDirection;
			}
			if (string.Equals("x-ms-cosmos-collectiontype", key, StringComparison.OrdinalIgnoreCase))
			{
				return RequestedCollectionType;
			}
			if (string.Equals("x-ms-cosmos-priority-level", key, StringComparison.OrdinalIgnoreCase))
			{
				return PriorityLevel;
			}
			break;
		case 27:
			if ((object)"x-ms-secondary-readonly-key" == key)
			{
				return SecondaryReadonlyKey;
			}
			if ((object)"x-ms-fanout-operation-state" == key)
			{
				return FanoutOperationState;
			}
			if ((object)"x-ms-cosmos-merge-static-id" == key)
			{
				return MergeStaticId;
			}
			if (string.Equals("x-ms-secondary-readonly-key", key, StringComparison.OrdinalIgnoreCase))
			{
				return SecondaryReadonlyKey;
			}
			if (string.Equals("x-ms-fanout-operation-state", key, StringComparison.OrdinalIgnoreCase))
			{
				return FanoutOperationState;
			}
			if (string.Equals("x-ms-cosmos-merge-static-id", key, StringComparison.OrdinalIgnoreCase))
			{
				return MergeStaticId;
			}
			break;
		case 28:
			if ((object)"x-ms-documentdb-partitionkey" == key)
			{
				return PartitionKey;
			}
			if ((object)"x-ms-restore-metadata-filter" == key)
			{
				return RestoreMetadataFilter;
			}
			if ((object)"x-ms-time-to-live-in-seconds" == key)
			{
				return TimeToLiveInSeconds;
			}
			if ((object)"x-ms-effective-partition-key" == key)
			{
				return EffectivePartitionKey;
			}
			if ((object)"x-ms-cosmos-use-systembudget" == key)
			{
				return UseSystemBudget;
			}
			if (string.Equals("x-ms-documentdb-partitionkey", key, StringComparison.OrdinalIgnoreCase))
			{
				return PartitionKey;
			}
			if (string.Equals("x-ms-restore-metadata-filter", key, StringComparison.OrdinalIgnoreCase))
			{
				return RestoreMetadataFilter;
			}
			if (string.Equals("x-ms-time-to-live-in-seconds", key, StringComparison.OrdinalIgnoreCase))
			{
				return TimeToLiveInSeconds;
			}
			if (string.Equals("x-ms-effective-partition-key", key, StringComparison.OrdinalIgnoreCase))
			{
				return EffectivePartitionKey;
			}
			if (string.Equals("x-ms-cosmos-use-systembudget", key, StringComparison.OrdinalIgnoreCase))
			{
				return UseSystemBudget;
			}
			break;
		case 29:
			if (string.Equals("x-ms-cosmos-client-ip-address", key, StringComparison.OrdinalIgnoreCase))
			{
				return ClientIpAddress;
			}
			break;
		case 30:
			if ((object)"x-ms-documentdb-expiry-seconds" == key)
			{
				return ResourceTokenExpiry;
			}
			if ((object)"x-ms-documentdb-partitioncount" == key)
			{
				return PartitionCount;
			}
			if ((object)"x-ms-documentdb-collection-rid" == key)
			{
				return CollectionRid;
			}
			if ((object)"x-ms-partition-resource-filter" == key)
			{
				return PartitionResourceFilter;
			}
			if ((object)"x-ms-exclude-system-properties" == key)
			{
				return ExcludeSystemProperties;
			}
			if ((object)"x-ms-cosmos-alter-type-request" == key)
			{
				return IsCassandraAlterTypeRequest;
			}
			if (string.Equals("x-ms-documentdb-expiry-seconds", key, StringComparison.OrdinalIgnoreCase))
			{
				return ResourceTokenExpiry;
			}
			if (string.Equals("x-ms-documentdb-partitioncount", key, StringComparison.OrdinalIgnoreCase))
			{
				return PartitionCount;
			}
			if (string.Equals("x-ms-documentdb-collection-rid", key, StringComparison.OrdinalIgnoreCase))
			{
				return CollectionRid;
			}
			if (string.Equals("x-ms-partition-resource-filter", key, StringComparison.OrdinalIgnoreCase))
			{
				return PartitionResourceFilter;
			}
			if (string.Equals("x-ms-exclude-system-properties", key, StringComparison.OrdinalIgnoreCase))
			{
				return ExcludeSystemProperties;
			}
			if (string.Equals("x-ms-cosmos-alter-type-request", key, StringComparison.OrdinalIgnoreCase))
			{
				return IsCassandraAlterTypeRequest;
			}
			break;
		case 31:
			if ((object)"x-ms-client-retry-attempt-count" == key)
			{
				return ClientRetryAttemptCount;
			}
			if ((object)"x-ms-can-offer-replace-complete" == key)
			{
				return CanOfferReplaceComplete;
			}
			if ((object)"x-ms-binary-passthrough-request" == key)
			{
				return BinaryPassthroughRequest;
			}
			if ((object)"x-ms-cosmos-is-client-encrypted" == key)
			{
				return IsClientEncrypted;
			}
			if ((object)"x-ms-cosmos-systemdocument-type" == key)
			{
				return SystemDocumentType;
			}
			if ((object)"x-ms-cosmos-collection-truncate" == key)
			{
				return CollectionTruncate;
			}
			if (string.Equals("x-ms-client-retry-attempt-count", key, StringComparison.OrdinalIgnoreCase))
			{
				return ClientRetryAttemptCount;
			}
			if (string.Equals("x-ms-can-offer-replace-complete", key, StringComparison.OrdinalIgnoreCase))
			{
				return CanOfferReplaceComplete;
			}
			if (string.Equals("x-ms-binary-passthrough-request", key, StringComparison.OrdinalIgnoreCase))
			{
				return BinaryPassthroughRequest;
			}
			if (string.Equals("x-ms-cosmos-is-client-encrypted", key, StringComparison.OrdinalIgnoreCase))
			{
				return IsClientEncrypted;
			}
			if (string.Equals("x-ms-cosmos-systemdocument-type", key, StringComparison.OrdinalIgnoreCase))
			{
				return SystemDocumentType;
			}
			if (string.Equals("x-ms-cosmos-collection-truncate", key, StringComparison.OrdinalIgnoreCase))
			{
				return CollectionTruncate;
			}
			break;
		case 32:
			if ((object)"x-ms-migratecollection-directive" == key)
			{
				return MigrateCollectionDirective;
			}
			if ((object)"x-ms-target-global-committed-lsn" == key)
			{
				return TargetGlobalCommittedLsn;
			}
			if ((object)"x-ms-documentdb-force-query-scan" == key)
			{
				return ForceQueryScan;
			}
			if ((object)"x-ms-cosmos-max-polling-interval" == key)
			{
				return MaxPollingIntervalMilliseconds;
			}
			if ((object)"x-ms-cosmos-populateindexmetrics" == key)
			{
				return PopulateIndexMetrics;
			}
			if (string.Equals("x-ms-migratecollection-directive", key, StringComparison.OrdinalIgnoreCase))
			{
				return MigrateCollectionDirective;
			}
			if (string.Equals("x-ms-target-global-committed-lsn", key, StringComparison.OrdinalIgnoreCase))
			{
				return TargetGlobalCommittedLsn;
			}
			if (string.Equals("x-ms-documentdb-force-query-scan", key, StringComparison.OrdinalIgnoreCase))
			{
				return ForceQueryScan;
			}
			if (string.Equals("x-ms-cosmos-max-polling-interval", key, StringComparison.OrdinalIgnoreCase))
			{
				return MaxPollingIntervalMilliseconds;
			}
			if (string.Equals("x-ms-cosmos-populateindexmetrics", key, StringComparison.OrdinalIgnoreCase))
			{
				return PopulateIndexMetrics;
			}
			break;
		case 33:
			if ((object)"x-ms-documentdb-query-enable-scan" == key)
			{
				return EnableScanInQuery;
			}
			if ((object)"x-ms-documentdb-query-emit-traces" == key)
			{
				return EmitVerboseTracesInQuery;
			}
			if ((object)"x-ms-documentdb-populatequotainfo" == key)
			{
				return PopulateQuotaInfo;
			}
			if ((object)"x-ms-cosmos-preserve-full-content" == key)
			{
				return PreserveFullContent;
			}
			if ((object)"x-ms-cosmos-populate-logstoreinfo" == key)
			{
				return PopulateLogStoreInfo;
			}
			if ((object)"x-ms-cosmos-correlated-activityid" == key)
			{
				return CorrelatedActivityId;
			}
			if (string.Equals("x-ms-documentdb-query-enable-scan", key, StringComparison.OrdinalIgnoreCase))
			{
				return EnableScanInQuery;
			}
			if (string.Equals("x-ms-documentdb-query-emit-traces", key, StringComparison.OrdinalIgnoreCase))
			{
				return EmitVerboseTracesInQuery;
			}
			if (string.Equals("x-ms-documentdb-populatequotainfo", key, StringComparison.OrdinalIgnoreCase))
			{
				return PopulateQuotaInfo;
			}
			if (string.Equals("x-ms-cosmos-preserve-full-content", key, StringComparison.OrdinalIgnoreCase))
			{
				return PreserveFullContent;
			}
			if (string.Equals("x-ms-cosmos-populate-logstoreinfo", key, StringComparison.OrdinalIgnoreCase))
			{
				return PopulateLogStoreInfo;
			}
			if (string.Equals("x-ms-cosmos-correlated-activityid", key, StringComparison.OrdinalIgnoreCase))
			{
				return CorrelatedActivityId;
			}
			break;
		case 34:
			if ((object)"x-ms-cosmos-allow-tentative-writes" == key)
			{
				return AllowTentativeWrites;
			}
			if ((object)"x-ms-cosmos-use-archival-partition" == key)
			{
				return UseArchivalPartition;
			}
			if ((object)"x-ms-cosmos-populate-capacity-type" == key)
			{
				return PopulateCapacityType;
			}
			if (string.Equals("x-ms-cosmos-allow-tentative-writes", key, StringComparison.OrdinalIgnoreCase))
			{
				return AllowTentativeWrites;
			}
			if (string.Equals("x-ms-cosmos-use-archival-partition", key, StringComparison.OrdinalIgnoreCase))
			{
				return UseArchivalPartition;
			}
			if (string.Equals("x-ms-cosmos-populate-capacity-type", key, StringComparison.OrdinalIgnoreCase))
			{
				return PopulateCapacityType;
			}
			break;
		case 35:
			if ((object)"x-ms-documentdb-pre-trigger-include" == key)
			{
				return PreTriggerInclude;
			}
			if ((object)"x-ms-documentdb-pre-trigger-exclude" == key)
			{
				return PreTriggerExclude;
			}
			if ((object)"x-ms-documentdb-partitionkeyrangeid" == key)
			{
				return PartitionKeyRangeId;
			}
			if ((object)"x-ms-documentdb-filterby-schema-rid" == key)
			{
				return FilterBySchemaResourceId;
			}
			if ((object)"x-ms-collection-security-identifier" == key)
			{
				return CollectionRemoteStorageSecurityIdentifier;
			}
			if ((object)"x-ms-remaining-time-in-ms-on-client" == key)
			{
				return RemainingTimeInMsOnClientRequest;
			}
			if ((object)"x-ms-cosmos-batch-continue-on-error" == key)
			{
				return ShouldBatchContinueOnError;
			}
			if ((object)"x-ms-cosmos-intended-collection-rid" == key)
			{
				return IntendedCollectionRid;
			}
			if ((object)"x-ms-cosmos-populateindexmetrics-V2" == key)
			{
				return PopulateIndexMetricsV2;
			}
			if (string.Equals("x-ms-documentdb-pre-trigger-include", key, StringComparison.OrdinalIgnoreCase))
			{
				return PreTriggerInclude;
			}
			if (string.Equals("x-ms-documentdb-pre-trigger-exclude", key, StringComparison.OrdinalIgnoreCase))
			{
				return PreTriggerExclude;
			}
			if (string.Equals("x-ms-documentdb-partitionkeyrangeid", key, StringComparison.OrdinalIgnoreCase))
			{
				return PartitionKeyRangeId;
			}
			if (string.Equals("x-ms-documentdb-filterby-schema-rid", key, StringComparison.OrdinalIgnoreCase))
			{
				return FilterBySchemaResourceId;
			}
			if (string.Equals("x-ms-collection-security-identifier", key, StringComparison.OrdinalIgnoreCase))
			{
				return CollectionRemoteStorageSecurityIdentifier;
			}
			if (string.Equals("x-ms-remaining-time-in-ms-on-client", key, StringComparison.OrdinalIgnoreCase))
			{
				return RemainingTimeInMsOnClientRequest;
			}
			if (string.Equals("x-ms-cosmos-batch-continue-on-error", key, StringComparison.OrdinalIgnoreCase))
			{
				return ShouldBatchContinueOnError;
			}
			if (string.Equals("x-ms-cosmos-intended-collection-rid", key, StringComparison.OrdinalIgnoreCase))
			{
				return IntendedCollectionRid;
			}
			if (string.Equals("x-ms-cosmos-populateindexmetrics-V2", key, StringComparison.OrdinalIgnoreCase))
			{
				return PopulateIndexMetricsV2;
			}
			break;
		case 36:
			if ((object)"x-ms-documentdb-post-trigger-include" == key)
			{
				return PostTriggerInclude;
			}
			if ((object)"x-ms-documentdb-post-trigger-exclude" == key)
			{
				return PostTriggerExclude;
			}
			if ((object)"x-ms-documentdb-populatequerymetrics" == key)
			{
				return PopulateQueryMetrics;
			}
			if ((object)"x-ms-cosmos-internal-is-user-request" == key)
			{
				return IsUserRequest;
			}
			if ((object)"x-ms-cosmos-include-tentative-writes" == key)
			{
				return IncludeTentativeWrites;
			}
			if ((object)"x-ms-cosmos-is-retried-write-request" == key)
			{
				return IsRetriedWriteRequest;
			}
			if ((object)"x-ms-cosmos-prune-collection-schemas" == key)
			{
				return PruneCollectionSchemas;
			}
			if ((object)"x-ms-cosmos-supported-query-features" == key)
			{
				return SupportedQueryFeatures;
			}
			if (string.Equals("x-ms-documentdb-post-trigger-include", key, StringComparison.OrdinalIgnoreCase))
			{
				return PostTriggerInclude;
			}
			if (string.Equals("x-ms-documentdb-post-trigger-exclude", key, StringComparison.OrdinalIgnoreCase))
			{
				return PostTriggerExclude;
			}
			if (string.Equals("x-ms-documentdb-populatequerymetrics", key, StringComparison.OrdinalIgnoreCase))
			{
				return PopulateQueryMetrics;
			}
			if (string.Equals("x-ms-cosmos-internal-is-user-request", key, StringComparison.OrdinalIgnoreCase))
			{
				return IsUserRequest;
			}
			if (string.Equals("x-ms-cosmos-include-tentative-writes", key, StringComparison.OrdinalIgnoreCase))
			{
				return IncludeTentativeWrites;
			}
			if (string.Equals("x-ms-cosmos-is-retried-write-request", key, StringComparison.OrdinalIgnoreCase))
			{
				return IsRetriedWriteRequest;
			}
			if (string.Equals("x-ms-cosmos-prune-collection-schemas", key, StringComparison.OrdinalIgnoreCase))
			{
				return PruneCollectionSchemas;
			}
			if (string.Equals("x-ms-cosmos-supported-query-features", key, StringComparison.OrdinalIgnoreCase))
			{
				return SupportedQueryFeatures;
			}
			break;
		case 37:
			if ((object)"x-ms-documentdb-script-enable-logging" == key)
			{
				return EnableLogging;
			}
			if ((object)"x-ms-documentdb-populateresourcecount" == key)
			{
				return PopulateResourceCount;
			}
			if ((object)"x-ms-cosmos-sdk-supportedcapabilities" == key)
			{
				return SDKSupportedCapabilities;
			}
			if ((object)"x-ms-cosmos-builder-client-identifier" == key)
			{
				return BuilderClientIdentifier;
			}
			if ((object)"x-ms-cosmos-is-request-not-authorized" == key)
			{
				return IsRequestNotAuthorized;
			}
			if (string.Equals("x-ms-documentdb-script-enable-logging", key, StringComparison.OrdinalIgnoreCase))
			{
				return EnableLogging;
			}
			if (string.Equals("x-ms-documentdb-populateresourcecount", key, StringComparison.OrdinalIgnoreCase))
			{
				return PopulateResourceCount;
			}
			if (string.Equals("x-ms-cosmos-sdk-supportedcapabilities", key, StringComparison.OrdinalIgnoreCase))
			{
				return SDKSupportedCapabilities;
			}
			if (string.Equals("x-ms-cosmos-builder-client-identifier", key, StringComparison.OrdinalIgnoreCase))
			{
				return BuilderClientIdentifier;
			}
			if (string.Equals("x-ms-cosmos-is-request-not-authorized", key, StringComparison.OrdinalIgnoreCase))
			{
				return IsRequestNotAuthorized;
			}
			break;
		case 38:
			if ((object)"x-ms-cosmos-migrate-offer-to-autopilot" == key)
			{
				return MigrateOfferToAutopilot;
			}
			if ((object)"x-ms-cosmos-retriable-write-request-id" == key)
			{
				return RetriableWriteRequestId;
			}
			if ((object)"x-ms-cosmos-source-collection-if-match" == key)
			{
				return SourceCollectionIfMatch;
			}
			if ((object)"x-ms-cosmos-use-background-task-budget" == key)
			{
				return UseUserBackgroundBudget;
			}
			if (string.Equals("x-ms-cosmos-migrate-offer-to-autopilot", key, StringComparison.OrdinalIgnoreCase))
			{
				return MigrateOfferToAutopilot;
			}
			if (string.Equals("x-ms-cosmos-retriable-write-request-id", key, StringComparison.OrdinalIgnoreCase))
			{
				return RetriableWriteRequestId;
			}
			if (string.Equals("x-ms-cosmos-source-collection-if-match", key, StringComparison.OrdinalIgnoreCase))
			{
				return SourceCollectionIfMatch;
			}
			if (string.Equals("x-ms-cosmos-use-background-task-budget", key, StringComparison.OrdinalIgnoreCase))
			{
				return UseUserBackgroundBudget;
			}
			break;
		case 39:
			if ((object)"x-ms-cosmos-internal-truncate-merge-log" == key)
			{
				return TruncateMergeLogRequest;
			}
			if ((object)"x-ms-cosmos-internal-serverless-request" == key)
			{
				return IsInternalServerlessRequest;
			}
			if (string.Equals("x-ms-cosmos-internal-truncate-merge-log", key, StringComparison.OrdinalIgnoreCase))
			{
				return TruncateMergeLogRequest;
			}
			if (string.Equals("x-ms-cosmos-internal-serverless-request", key, StringComparison.OrdinalIgnoreCase))
			{
				return IsInternalServerlessRequest;
			}
			break;
		case 40:
			if ((object)"x-ms-enable-dynamic-rid-range-allocation" == key)
			{
				return EnableDynamicRidRangeAllocation;
			}
			if ((object)"x-ms-cosmos-uniqueindex-reindexing-state" == key)
			{
				return UniqueIndexReIndexingState;
			}
			if (string.Equals("x-ms-enable-dynamic-rid-range-allocation", key, StringComparison.OrdinalIgnoreCase))
			{
				return EnableDynamicRidRangeAllocation;
			}
			if (string.Equals("x-ms-cosmos-uniqueindex-reindexing-state", key, StringComparison.OrdinalIgnoreCase))
			{
				return UniqueIndexReIndexingState;
			}
			break;
		case 41:
			if ((object)"x-ms-cosmos-force-database-account-update" == key)
			{
				return ForceDatabaseAccountUpdate;
			}
			if ((object)"x-ms-cosmos-query-optimisticdirectexecute" == key)
			{
				return OptimisticDirectExecute;
			}
			if (string.Equals("x-ms-cosmos-force-database-account-update", key, StringComparison.OrdinalIgnoreCase))
			{
				return ForceDatabaseAccountUpdate;
			}
			if (string.Equals("x-ms-cosmos-query-optimisticdirectexecute", key, StringComparison.OrdinalIgnoreCase))
			{
				return OptimisticDirectExecute;
			}
			break;
		case 42:
			if ((object)"x-ms-cosmos-internal-merge-checkpoint-glsn" == key)
			{
				return MergeCheckPointGLSN;
			}
			if ((object)"x-ms-should-return-current-server-datetime" == key)
			{
				return ShouldReturnCurrentServerDateTime;
			}
			if ((object)"x-ms-cosmos-changefeed-wire-format-version" == key)
			{
				return ChangeFeedWireFormatVersion;
			}
			if ((object)"x-ms-documentdb-query-enablecrosspartition" == key)
			{
				return EnableCrossPartitionQuery;
			}
			if (string.Equals("x-ms-cosmos-internal-merge-checkpoint-glsn", key, StringComparison.OrdinalIgnoreCase))
			{
				return MergeCheckPointGLSN;
			}
			if (string.Equals("x-ms-should-return-current-server-datetime", key, StringComparison.OrdinalIgnoreCase))
			{
				return ShouldReturnCurrentServerDateTime;
			}
			if (string.Equals("x-ms-cosmos-changefeed-wire-format-version", key, StringComparison.OrdinalIgnoreCase))
			{
				return ChangeFeedWireFormatVersion;
			}
			if (string.Equals("x-ms-documentdb-query-enablecrosspartition", key, StringComparison.OrdinalIgnoreCase))
			{
				return EnableCrossPartitionQuery;
			}
			break;
		case 43:
			if ((object)"x-ms-documentdb-disable-ru-per-minute-usage" == key)
			{
				return DisableRUPerMinuteUsage;
			}
			if ((object)"x-ms-documentdb-populatepartitionstatistics" == key)
			{
				return PopulatePartitionStatistics;
			}
			if ((object)"x-ms-cosmos-force-sidebyside-indexmigration" == key)
			{
				return ForceSideBySideIndexMigration;
			}
			if ((object)"x-ms-cosmos-unique-index-name-encoding-mode" == key)
			{
				return UniqueIndexNameEncodingMode;
			}
			if ((object)"x-ms-cosmos-supported-serialization-formats" == key)
			{
				return SupportedSerializationFormats;
			}
			if (string.Equals("x-ms-documentdb-disable-ru-per-minute-usage", key, StringComparison.OrdinalIgnoreCase))
			{
				return DisableRUPerMinuteUsage;
			}
			if (string.Equals("x-ms-documentdb-populatepartitionstatistics", key, StringComparison.OrdinalIgnoreCase))
			{
				return PopulatePartitionStatistics;
			}
			if (string.Equals("x-ms-cosmos-force-sidebyside-indexmigration", key, StringComparison.OrdinalIgnoreCase))
			{
				return ForceSideBySideIndexMigration;
			}
			if (string.Equals("x-ms-cosmos-unique-index-name-encoding-mode", key, StringComparison.OrdinalIgnoreCase))
			{
				return UniqueIndexNameEncodingMode;
			}
			if (string.Equals("x-ms-cosmos-supported-serialization-formats", key, StringComparison.OrdinalIgnoreCase))
			{
				return SupportedSerializationFormats;
			}
			break;
		case 44:
			if ((object)"x-ms-documentdb-content-serialization-format" == key)
			{
				return ContentSerializationFormat;
			}
			if ((object)"x-ms-cosmos-populate-oldest-active-schema-id" == key)
			{
				return PopulateOldestActiveSchemaId;
			}
			if ((object)"x-ms-documentdb-query-iscontinuationexpected" == key)
			{
				return IsContinuationExpected;
			}
			if (string.Equals("x-ms-documentdb-content-serialization-format", key, StringComparison.OrdinalIgnoreCase))
			{
				return ContentSerializationFormat;
			}
			if (string.Equals("x-ms-cosmos-populate-oldest-active-schema-id", key, StringComparison.OrdinalIgnoreCase))
			{
				return PopulateOldestActiveSchemaId;
			}
			if (string.Equals("x-ms-documentdb-query-iscontinuationexpected", key, StringComparison.OrdinalIgnoreCase))
			{
				return IsContinuationExpected;
			}
			break;
		case 45:
			if ((object)"x-ms-cosmos-start-full-fidelity-if-none-match" == key)
			{
				return ChangeFeedStartFullFidelityIfNoneMatch;
			}
			if ((object)"x-ms-cosmos-internal-system-restore-operation" == key)
			{
				return SystemRestoreOperation;
			}
			if ((object)"x-ms-cosmos-internal-is-throughputcap-request" == key)
			{
				return IsThroughputCapRequest;
			}
			if ((object)"x-ms-cosmos-populate-byok-encryption-progress" == key)
			{
				return PopulateByokEncryptionProgress;
			}
			if (string.Equals("x-ms-cosmos-start-full-fidelity-if-none-match", key, StringComparison.OrdinalIgnoreCase))
			{
				return ChangeFeedStartFullFidelityIfNoneMatch;
			}
			if (string.Equals("x-ms-cosmos-internal-system-restore-operation", key, StringComparison.OrdinalIgnoreCase))
			{
				return SystemRestoreOperation;
			}
			if (string.Equals("x-ms-cosmos-internal-is-throughputcap-request", key, StringComparison.OrdinalIgnoreCase))
			{
				return IsThroughputCapRequest;
			}
			if (string.Equals("x-ms-cosmos-populate-byok-encryption-progress", key, StringComparison.OrdinalIgnoreCase))
			{
				return PopulateByokEncryptionProgress;
			}
			break;
		case 46:
			if ((object)"x-ms-cosmos-migrate-offer-to-manual-throughput" == key)
			{
				return MigrateOfferToManualThroughput;
			}
			if ((object)"x-ms-cosmos-skip-refresh-databaseaccountconfig" == key)
			{
				return SkipRefreshDatabaseAccountConfigs;
			}
			if ((object)"x-ms-cosmos-internal-migrated-fixed-collection" == key)
			{
				return IsMigratedFixedCollection;
			}
			if (string.Equals("x-ms-cosmos-migrate-offer-to-manual-throughput", key, StringComparison.OrdinalIgnoreCase))
			{
				return MigrateOfferToManualThroughput;
			}
			if (string.Equals("x-ms-cosmos-skip-refresh-databaseaccountconfig", key, StringComparison.OrdinalIgnoreCase))
			{
				return SkipRefreshDatabaseAccountConfigs;
			}
			if (string.Equals("x-ms-cosmos-internal-migrated-fixed-collection", key, StringComparison.OrdinalIgnoreCase))
			{
				return IsMigratedFixedCollection;
			}
			break;
		case 47:
			if ((object)"x-ms-documentdb-supportspatiallegacycoordinates" == key)
			{
				return SupportSpatialLegacyCoordinates;
			}
			if ((object)"x-ms-cosmos-collection-child-resourcename-limit" == key)
			{
				return CollectionChildResourceNameLimitInBytes;
			}
			if ((object)"x-ms-cosmos-add-resource-properties-to-response" == key)
			{
				return AddResourcePropertiesToResponse;
			}
			if ((object)"x-ms-cosmos-internal-is-materialized-view-build" == key)
			{
				return IsMaterializedViewBuild;
			}
			if (string.Equals("x-ms-documentdb-supportspatiallegacycoordinates", key, StringComparison.OrdinalIgnoreCase))
			{
				return SupportSpatialLegacyCoordinates;
			}
			if (string.Equals("x-ms-cosmos-collection-child-resourcename-limit", key, StringComparison.OrdinalIgnoreCase))
			{
				return CollectionChildResourceNameLimitInBytes;
			}
			if (string.Equals("x-ms-cosmos-add-resource-properties-to-response", key, StringComparison.OrdinalIgnoreCase))
			{
				return AddResourcePropertiesToResponse;
			}
			if (string.Equals("x-ms-cosmos-internal-is-materialized-view-build", key, StringComparison.OrdinalIgnoreCase))
			{
				return IsMaterializedViewBuild;
			}
			break;
		case 48:
			if ((object)"x-ms-documentdb-populatecollectionthroughputinfo" == key)
			{
				return PopulateCollectionThroughputInfo;
			}
			if ((object)"x-ms-cosmos-internal-get-all-partition-key-stats" == key)
			{
				return GetAllPartitionKeyStatistics;
			}
			if ((object)"x-ms-cosmosdb-populateuniqueindexreindexprogress" == key)
			{
				return PopulateUniqueIndexReIndexProgress;
			}
			if ((object)"x-ms-cosmos-internal-allow-restore-params-update" == key)
			{
				return AllowRestoreParamsUpdate;
			}
			if ((object)"x-ms-cosmos-internal-high-priority-forced-backup" == key)
			{
				return HighPriorityForcedBackup;
			}
			if (string.Equals("x-ms-documentdb-populatecollectionthroughputinfo", key, StringComparison.OrdinalIgnoreCase))
			{
				return PopulateCollectionThroughputInfo;
			}
			if (string.Equals("x-ms-cosmos-internal-get-all-partition-key-stats", key, StringComparison.OrdinalIgnoreCase))
			{
				return GetAllPartitionKeyStatistics;
			}
			if (string.Equals("x-ms-cosmosdb-populateuniqueindexreindexprogress", key, StringComparison.OrdinalIgnoreCase))
			{
				return PopulateUniqueIndexReIndexProgress;
			}
			if (string.Equals("x-ms-cosmos-internal-allow-restore-params-update", key, StringComparison.OrdinalIgnoreCase))
			{
				return AllowRestoreParamsUpdate;
			}
			if (string.Equals("x-ms-cosmos-internal-high-priority-forced-backup", key, StringComparison.OrdinalIgnoreCase))
			{
				return HighPriorityForcedBackup;
			}
			break;
		case 49:
			if (string.Equals("x-ms-documentdb-usepolygonssmallerthanahemisphere", key, StringComparison.OrdinalIgnoreCase))
			{
				return UsePolygonsSmallerThanAHemisphere;
			}
			break;
		case 50:
			if ((object)"x-ms-documentdb-responsecontinuationtokenlimitinkb" == key)
			{
				return ResponseContinuationTokenLimitInKB;
			}
			if ((object)"x-ms-cosmos-populate-analytical-migration-progress" == key)
			{
				return PopulateAnalyticalMigrationProgress;
			}
			if ((object)"x-ms-cosmos-internal-update-offer-state-to-pending" == key)
			{
				return UpdateOfferStateToPending;
			}
			if (string.Equals("x-ms-documentdb-responsecontinuationtokenlimitinkb", key, StringComparison.OrdinalIgnoreCase))
			{
				return ResponseContinuationTokenLimitInKB;
			}
			if (string.Equals("x-ms-cosmos-populate-analytical-migration-progress", key, StringComparison.OrdinalIgnoreCase))
			{
				return PopulateAnalyticalMigrationProgress;
			}
			if (string.Equals("x-ms-cosmos-internal-update-offer-state-to-pending", key, StringComparison.OrdinalIgnoreCase))
			{
				return UpdateOfferStateToPending;
			}
			break;
		case 51:
			if ((object)"x-ms-documentdb-query-enable-low-precision-order-by" == key)
			{
				return EnableLowPrecisionOrderBy;
			}
			if ((object)"x-ms-cosmos-retriable-write-request-start-timestamp" == key)
			{
				return RetriableWriteRequestStartTimestamp;
			}
			if ((object)"x-ms-cosmos-internal-populate-document-record-count" == key)
			{
				return PopulateDocumentRecordCount;
			}
			if (string.Equals("x-ms-documentdb-query-enable-low-precision-order-by", key, StringComparison.OrdinalIgnoreCase))
			{
				return EnableLowPrecisionOrderBy;
			}
			if (string.Equals("x-ms-cosmos-retriable-write-request-start-timestamp", key, StringComparison.OrdinalIgnoreCase))
			{
				return RetriableWriteRequestStartTimestamp;
			}
			if (string.Equals("x-ms-cosmos-internal-populate-document-record-count", key, StringComparison.OrdinalIgnoreCase))
			{
				return PopulateDocumentRecordCount;
			}
			break;
		case 52:
			if ((object)"x-ms-cosmos-internal-offer-replace-ru-redistribution" == key)
			{
				return OfferReplaceRURedistribution;
			}
			if ((object)"x-ms-documentdb-query-parallelizecrosspartitionquery" == key)
			{
				return ParallelizeCrossPartitionQuery;
			}
			if (string.Equals("x-ms-cosmos-internal-offer-replace-ru-redistribution", key, StringComparison.OrdinalIgnoreCase))
			{
				return OfferReplaceRURedistribution;
			}
			if (string.Equals("x-ms-documentdb-query-parallelizecrosspartitionquery", key, StringComparison.OrdinalIgnoreCase))
			{
				return ParallelizeCrossPartitionQuery;
			}
			break;
		case 53:
			if ((object)"x-ms-cosmos-internal-is-ru-per-gb-enforcement-request" == key)
			{
				return IsRUPerGBEnforcementRequest;
			}
			if ((object)"x-ms-cosmos-internal-is-offer-storage-refresh-request" == key)
			{
				return IsOfferStorageRefreshRequest;
			}
			if ((object)"x-ms-cosmos-internal-populate-min-glsn-for-relocation" == key)
			{
				return PopulateMinGLSNForDocumentOperations;
			}
			if (string.Equals("x-ms-cosmos-internal-is-ru-per-gb-enforcement-request", key, StringComparison.OrdinalIgnoreCase))
			{
				return IsRUPerGBEnforcementRequest;
			}
			if (string.Equals("x-ms-cosmos-internal-is-offer-storage-refresh-request", key, StringComparison.OrdinalIgnoreCase))
			{
				return IsOfferStorageRefreshRequest;
			}
			if (string.Equals("x-ms-cosmos-internal-populate-min-glsn-for-relocation", key, StringComparison.OrdinalIgnoreCase))
			{
				return PopulateMinGLSNForDocumentOperations;
			}
			break;
		case 54:
			if ((object)"x-ms-cosmos-include-physical-partition-throughput-info" == key)
			{
				return IncludePhysicalPartitionThroughputInfo;
			}
			if ((object)"x-ms-cosmos-is-materialized-view-source-schema-replace" == key)
			{
				return IsMaterializedViewSourceSchemaReplaceBatchRequest;
			}
			if ((object)"x-ms-cosmos-populate-current-partition-throughput-info" == key)
			{
				return PopulateCurrentPartitionThroughputInfo;
			}
			if (string.Equals("x-ms-cosmos-include-physical-partition-throughput-info", key, StringComparison.OrdinalIgnoreCase))
			{
				return IncludePhysicalPartitionThroughputInfo;
			}
			if (string.Equals("x-ms-cosmos-is-materialized-view-source-schema-replace", key, StringComparison.OrdinalIgnoreCase))
			{
				return IsMaterializedViewSourceSchemaReplaceBatchRequest;
			}
			if (string.Equals("x-ms-cosmos-populate-current-partition-throughput-info", key, StringComparison.OrdinalIgnoreCase))
			{
				return PopulateCurrentPartitionThroughputInfo;
			}
			break;
		case 55:
			if ((object)"x-ms-cosmos-internal-update-offer-state-restore-pending" == key)
			{
				return UpdateOfferStateToRestorePending;
			}
			if ((object)"x-ms-documentdb-query-sqlqueryforpartitionkeyextraction" == key)
			{
				return SqlQueryForPartitionKeyExtraction;
			}
			if (string.Equals("x-ms-cosmos-internal-update-offer-state-restore-pending", key, StringComparison.OrdinalIgnoreCase))
			{
				return UpdateOfferStateToRestorePending;
			}
			if (string.Equals("x-ms-documentdb-query-sqlqueryforpartitionkeyextraction", key, StringComparison.OrdinalIgnoreCase))
			{
				return SqlQueryForPartitionKeyExtraction;
			}
			break;
		case 56:
			if (string.Equals("x-ms-cosmos-collection-child-contentlength-resourcelimit", key, StringComparison.OrdinalIgnoreCase))
			{
				return CollectionChildResourceContentLimitInKB;
			}
			break;
		case 57:
			if (string.Equals("x-ms-cosmos-internal-populate-unflushed-merge-entry-count", key, StringComparison.OrdinalIgnoreCase))
			{
				return PopulateUnflushedMergeEntryCount;
			}
			break;
		case 58:
			if ((object)"x-ms-cosmos-internal-ignore-system-lowering-max-throughput" == key)
			{
				return IgnoreSystemLoweringMaxThroughput;
			}
			if ((object)"x-ms-cosmos-internal-set-master-resources-deletion-pending" == key)
			{
				return SetMasterResourcesDeletionPending;
			}
			if ((object)"x-ms-cosmos-internal-populate-highest-tentative-write-llsn" == key)
			{
				return PopulateHighestTentativeWriteLLSN;
			}
			if (string.Equals("x-ms-cosmos-internal-ignore-system-lowering-max-throughput", key, StringComparison.OrdinalIgnoreCase))
			{
				return IgnoreSystemLoweringMaxThroughput;
			}
			if (string.Equals("x-ms-cosmos-internal-set-master-resources-deletion-pending", key, StringComparison.OrdinalIgnoreCase))
			{
				return SetMasterResourcesDeletionPending;
			}
			if (string.Equals("x-ms-cosmos-internal-populate-highest-tentative-write-llsn", key, StringComparison.OrdinalIgnoreCase))
			{
				return PopulateHighestTentativeWriteLLSN;
			}
			break;
		case 59:
			if ((object)"x-ms-cosmos-internal-update-max-throughput-ever-provisioned" == key)
			{
				return UpdateMaxThroughputEverProvisioned;
			}
			if ((object)"x-ms-cosmos-internal-enable-conflictresolutionpolicy-update" == key)
			{
				return EnableConflictResolutionPolicyUpdate;
			}
			if ((object)"x-ms-cosmos-internal-allow-document-reads-in-offline-region" == key)
			{
				return AllowDocumentReadsInOfflineRegion;
			}
			if (string.Equals("x-ms-cosmos-internal-update-max-throughput-ever-provisioned", key, StringComparison.OrdinalIgnoreCase))
			{
				return UpdateMaxThroughputEverProvisioned;
			}
			if (string.Equals("x-ms-cosmos-internal-enable-conflictresolutionpolicy-update", key, StringComparison.OrdinalIgnoreCase))
			{
				return EnableConflictResolutionPolicyUpdate;
			}
			if (string.Equals("x-ms-cosmos-internal-allow-document-reads-in-offline-region", key, StringComparison.OrdinalIgnoreCase))
			{
				return AllowDocumentReadsInOfflineRegion;
			}
			break;
		case 61:
			if (string.Equals("x-ms-cosmos-internal-serverless-offer-storage-refresh-request", key, StringComparison.OrdinalIgnoreCase))
			{
				return IsServerlessStorageRefreshRequest;
			}
			break;
		case 62:
			if (string.Equals("x-ms-cosmos-skip-adjust-throughput-fractions-for-offer-replace", key, StringComparison.OrdinalIgnoreCase))
			{
				return SkipAdjustThroughputFractionsForOfferReplace;
			}
			break;
		}
		if (notCommonHeaders != null && notCommonHeaders.TryGetValue(key, out var value))
		{
			return value;
		}
		return null;
	}

	public void Add(string key, string value)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		UpdateHelper(key, value, throwIfAlreadyExists: true, ignoreNotCommonHeaders: false);
	}

	public void Set(string key, string value)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}
		UpdateHelper(key, value, throwIfAlreadyExists: false, ignoreNotCommonHeaders: false);
	}

	public void UpdateHelper(string key, string value, bool throwIfAlreadyExists, bool ignoreNotCommonHeaders)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}
		switch (key.Length)
		{
		case 4:
			if ((object)"date" == key)
			{
				if (throwIfAlreadyExists && HttpDate != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				HttpDate = value;
				return;
			}
			if ((object)"A-IM" == key)
			{
				if (throwIfAlreadyExists && A_IM != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				A_IM = value;
				return;
			}
			if (string.Equals("date", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && HttpDate != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				HttpDate = value;
				return;
			}
			if (string.Equals("A-IM", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && A_IM != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				A_IM = value;
				return;
			}
			break;
		case 6:
			if (string.Equals("Prefer", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && Prefer != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				Prefer = value;
				return;
			}
			break;
		case 8:
			if (string.Equals("If-Match", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && IfMatch != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IfMatch = value;
				return;
			}
			break;
		case 9:
			if (string.Equals("x-ms-date", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && XDate != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				XDate = value;
				return;
			}
			break;
		case 10:
			if (string.Equals("tracestate", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && TraceState != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				TraceState = value;
				return;
			}
			break;
		case 11:
			if ((object)"x-ms-end-id" == key)
			{
				if (throwIfAlreadyExists && EndId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				EndId = value;
				return;
			}
			if ((object)"traceparent" == key)
			{
				if (throwIfAlreadyExists && TraceParent != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				TraceParent = value;
				return;
			}
			if (string.Equals("x-ms-end-id", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && EndId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				EndId = value;
				return;
			}
			if (string.Equals("traceparent", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && TraceParent != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				TraceParent = value;
				return;
			}
			break;
		case 12:
			if ((object)"x-ms-version" == key)
			{
				if (throwIfAlreadyExists && Version != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				Version = value;
				return;
			}
			if ((object)"x-ms-end-epk" == key)
			{
				if (throwIfAlreadyExists && EndEpk != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				EndEpk = value;
				return;
			}
			if (string.Equals("x-ms-version", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && Version != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				Version = value;
				return;
			}
			if (string.Equals("x-ms-end-epk", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && EndEpk != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				EndEpk = value;
				return;
			}
			break;
		case 13:
			if ((object)"authorization" == key)
			{
				if (throwIfAlreadyExists && Authorization != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				Authorization = value;
				return;
			}
			if ((object)"If-None-Match" == key)
			{
				if (throwIfAlreadyExists && IfNoneMatch != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IfNoneMatch = value;
				return;
			}
			if ((object)"x-ms-start-id" == key)
			{
				if (throwIfAlreadyExists && StartId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				StartId = value;
				return;
			}
			if (string.Equals("authorization", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && Authorization != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				Authorization = value;
				return;
			}
			if (string.Equals("If-None-Match", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && IfNoneMatch != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IfNoneMatch = value;
				return;
			}
			if (string.Equals("x-ms-start-id", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && StartId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				StartId = value;
				return;
			}
			break;
		case 14:
			if ((object)"x-ms-cancharge" == key)
			{
				if (throwIfAlreadyExists && CanCharge != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CanCharge = value;
				return;
			}
			if ((object)"x-ms-binary-id" == key)
			{
				if (throwIfAlreadyExists && BinaryId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				BinaryId = value;
				return;
			}
			if ((object)"x-ms-start-epk" == key)
			{
				if (throwIfAlreadyExists && StartEpk != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				StartEpk = value;
				return;
			}
			if ((object)"x-ms-schema-id" == key)
			{
				if (throwIfAlreadyExists && SchemaId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SchemaId = value;
				return;
			}
			if (string.Equals("x-ms-cancharge", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && CanCharge != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CanCharge = value;
				return;
			}
			if (string.Equals("x-ms-binary-id", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && BinaryId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				BinaryId = value;
				return;
			}
			if (string.Equals("x-ms-start-epk", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && StartEpk != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				StartEpk = value;
				return;
			}
			if (string.Equals("x-ms-schema-id", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && SchemaId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SchemaId = value;
				return;
			}
			break;
		case 15:
			if (string.Equals("x-ms-target-lsn", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && TargetLsn != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				TargetLsn = value;
				return;
			}
			break;
		case 16:
			if ((object)"x-ms-canthrottle" == key)
			{
				if (throwIfAlreadyExists && CanThrottle != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CanThrottle = value;
				return;
			}
			if ((object)"x-ms-schema-hash" == key)
			{
				if (throwIfAlreadyExists && SchemaHash != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SchemaHash = value;
				return;
			}
			if ((object)"x-ms-rbac-action" == key)
			{
				if (throwIfAlreadyExists && RbacAction != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				RbacAction = value;
				return;
			}
			if ((object)"x-ms-noretry-449" == key)
			{
				if (throwIfAlreadyExists && NoRetryOn449StatusCode != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				NoRetryOn449StatusCode = value;
				return;
			}
			if ((object)"x-ms-activity-id" == key)
			{
				if (throwIfAlreadyExists && ActivityId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ActivityId = value;
				return;
			}
			if (string.Equals("x-ms-canthrottle", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && CanThrottle != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CanThrottle = value;
				return;
			}
			if (string.Equals("x-ms-schema-hash", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && SchemaHash != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SchemaHash = value;
				return;
			}
			if (string.Equals("x-ms-rbac-action", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && RbacAction != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				RbacAction = value;
				return;
			}
			if (string.Equals("x-ms-noretry-449", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && NoRetryOn449StatusCode != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				NoRetryOn449StatusCode = value;
				return;
			}
			if (string.Equals("x-ms-activity-id", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ActivityId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ActivityId = value;
				return;
			}
			break;
		case 17:
			if ((object)"x-ms-continuation" == key)
			{
				if (throwIfAlreadyExists && Continuation != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				Continuation = value;
				return;
			}
			if ((object)"x-docdb-entity-id" == key)
			{
				if (throwIfAlreadyExists && EntityId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				EntityId = value;
				return;
			}
			if ((object)"x-ms-bind-replica" == key)
			{
				if (throwIfAlreadyExists && BindReplicaDirective != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				BindReplicaDirective = value;
				return;
			}
			if ((object)"If-Modified-Since" == key)
			{
				if (throwIfAlreadyExists && IfModifiedSince != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IfModifiedSince = value;
				return;
			}
			if ((object)"x-ms-cosmos-tx-id" == key)
			{
				if (throwIfAlreadyExists && TransactionId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				TransactionId = value;
				return;
			}
			if ((object)"x-ms-rbac-user-id" == key)
			{
				if (throwIfAlreadyExists && RbacUserId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				RbacUserId = value;
				return;
			}
			if (string.Equals("x-ms-continuation", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && Continuation != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				Continuation = value;
				return;
			}
			if (string.Equals("x-docdb-entity-id", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && EntityId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				EntityId = value;
				return;
			}
			if (string.Equals("x-ms-bind-replica", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && BindReplicaDirective != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				BindReplicaDirective = value;
				return;
			}
			if (string.Equals("If-Modified-Since", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && IfModifiedSince != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IfModifiedSince = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-tx-id", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && TransactionId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				TransactionId = value;
				return;
			}
			if (string.Equals("x-ms-rbac-user-id", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && RbacUserId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				RbacUserId = value;
				return;
			}
			break;
		case 18:
			if ((object)"x-ms-session-token" == key)
			{
				if (throwIfAlreadyExists && SessionToken != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SessionToken = value;
				return;
			}
			if ((object)"x-ms-is-auto-scale" == key)
			{
				if (throwIfAlreadyExists && IsAutoScaleRequest != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsAutoScaleRequest = value;
				return;
			}
			if ((object)"x-ms-read-key-type" == key)
			{
				if (throwIfAlreadyExists && ReadFeedKeyType != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ReadFeedKeyType = value;
				return;
			}
			if ((object)"x-ms-rbac-resource" == key)
			{
				if (throwIfAlreadyExists && RbacResource != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				RbacResource = value;
				return;
			}
			if (string.Equals("x-ms-session-token", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && SessionToken != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SessionToken = value;
				return;
			}
			if (string.Equals("x-ms-is-auto-scale", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && IsAutoScaleRequest != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsAutoScaleRequest = value;
				return;
			}
			if (string.Equals("x-ms-read-key-type", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ReadFeedKeyType != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ReadFeedKeyType = value;
				return;
			}
			if (string.Equals("x-ms-rbac-resource", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && RbacResource != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				RbacResource = value;
				return;
			}
			break;
		case 19:
			if ((object)"x-docdb-resource-id" == key)
			{
				if (throwIfAlreadyExists && ResourceId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ResourceId = value;
				return;
			}
			if ((object)"x-ms-max-item-count" == key)
			{
				if (throwIfAlreadyExists && PageSize != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PageSize = value;
				return;
			}
			if ((object)"x-ms-restore-params" == key)
			{
				if (throwIfAlreadyExists && RestoreParams != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				RestoreParams = value;
				return;
			}
			if ((object)"x-ms-cosmos-tx-init" == key)
			{
				if (throwIfAlreadyExists && TransactionFirstRequest != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				TransactionFirstRequest = value;
				return;
			}
			if (string.Equals("x-docdb-resource-id", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ResourceId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ResourceId = value;
				return;
			}
			if (string.Equals("x-ms-max-item-count", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PageSize != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PageSize = value;
				return;
			}
			if (string.Equals("x-ms-restore-params", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && RestoreParams != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				RestoreParams = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-tx-init", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && TransactionFirstRequest != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				TransactionFirstRequest = value;
				return;
			}
			break;
		case 20:
			if (string.Equals("x-ms-profile-request", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ProfileRequest != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ProfileRequest = value;
				return;
			}
			break;
		case 21:
			if ((object)"x-ms-share-throughput" == key)
			{
				if (throwIfAlreadyExists && ShareThroughput != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ShareThroughput = value;
				return;
			}
			if ((object)"x-ms-schema-owner-rid" == key)
			{
				if (throwIfAlreadyExists && SchemaOwnerRid != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SchemaOwnerRid = value;
				return;
			}
			if ((object)"x-ms-cosmos-tx-commit" == key)
			{
				if (throwIfAlreadyExists && TransactionCommit != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				TransactionCommit = value;
				return;
			}
			if (string.Equals("x-ms-share-throughput", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ShareThroughput != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ShareThroughput = value;
				return;
			}
			if (string.Equals("x-ms-schema-owner-rid", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && SchemaOwnerRid != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SchemaOwnerRid = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-tx-commit", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && TransactionCommit != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				TransactionCommit = value;
				return;
			}
			break;
		case 22:
			if ((object)"x-ms-is-fanout-request" == key)
			{
				if (throwIfAlreadyExists && IsFanoutRequest != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsFanoutRequest = value;
				return;
			}
			if ((object)"x-ms-consistency-level" == key)
			{
				if (throwIfAlreadyExists && ConsistencyLevel != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ConsistencyLevel = value;
				return;
			}
			if ((object)"x-ms-gateway-signature" == key)
			{
				if (throwIfAlreadyExists && GatewaySignature != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				GatewaySignature = value;
				return;
			}
			if (string.Equals("x-ms-is-fanout-request", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && IsFanoutRequest != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsFanoutRequest = value;
				return;
			}
			if (string.Equals("x-ms-consistency-level", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ConsistencyLevel != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ConsistencyLevel = value;
				return;
			}
			if (string.Equals("x-ms-gateway-signature", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && GatewaySignature != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				GatewaySignature = value;
				return;
			}
			break;
		case 23:
			if ((object)"x-ms-indexing-directive" == key)
			{
				if (throwIfAlreadyExists && IndexingDirective != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IndexingDirective = value;
				return;
			}
			if ((object)"x-ms-primary-master-key" == key)
			{
				if (throwIfAlreadyExists && PrimaryMasterKey != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PrimaryMasterKey = value;
				return;
			}
			if ((object)"x-ms-is-readonly-script" == key)
			{
				if (throwIfAlreadyExists && IsReadOnlyScript != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsReadOnlyScript = value;
				return;
			}
			if (string.Equals("x-ms-indexing-directive", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && IndexingDirective != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IndexingDirective = value;
				return;
			}
			if (string.Equals("x-ms-primary-master-key", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PrimaryMasterKey != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PrimaryMasterKey = value;
				return;
			}
			if (string.Equals("x-ms-is-readonly-script", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && IsReadOnlyScript != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsReadOnlyScript = value;
				return;
			}
			break;
		case 24:
			if ((object)"collection-service-index" == key)
			{
				if (throwIfAlreadyExists && CollectionServiceIndex != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CollectionServiceIndex = value;
				return;
			}
			if ((object)"x-ms-remote-storage-type" == key)
			{
				if (throwIfAlreadyExists && RemoteStorageType != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				RemoteStorageType = value;
				return;
			}
			if ((object)"x-ms-cosmos-batch-atomic" == key)
			{
				if (throwIfAlreadyExists && IsBatchAtomic != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsBatchAtomic = value;
				return;
			}
			if (string.Equals("collection-service-index", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && CollectionServiceIndex != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CollectionServiceIndex = value;
				return;
			}
			if (string.Equals("x-ms-remote-storage-type", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && RemoteStorageType != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				RemoteStorageType = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-batch-atomic", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && IsBatchAtomic != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsBatchAtomic = value;
				return;
			}
			break;
		case 25:
			if ((object)"x-ms-resource-schema-name" == key)
			{
				if (throwIfAlreadyExists && ResourceSchemaName != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ResourceSchemaName = value;
				return;
			}
			if ((object)"x-ms-secondary-master-key" == key)
			{
				if (throwIfAlreadyExists && SecondaryMasterKey != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SecondaryMasterKey = value;
				return;
			}
			if ((object)"x-ms-primary-readonly-key" == key)
			{
				if (throwIfAlreadyExists && PrimaryReadonlyKey != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PrimaryReadonlyKey = value;
				return;
			}
			if ((object)"x-ms-transport-request-id" == key)
			{
				if (throwIfAlreadyExists && TransportRequestID != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				TransportRequestID = value;
				return;
			}
			if ((object)"x-ms-cosmos-batch-ordered" == key)
			{
				if (throwIfAlreadyExists && IsBatchOrdered != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsBatchOrdered = value;
				return;
			}
			if ((object)"x-ms-cosmos-resourcetypes" == key)
			{
				if (throwIfAlreadyExists && ResourceTypes != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ResourceTypes = value;
				return;
			}
			if ((object)"x-ms-cosmos-query-version" == key)
			{
				if (throwIfAlreadyExists && QueryVersion != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				QueryVersion = value;
				return;
			}
			if (string.Equals("x-ms-resource-schema-name", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ResourceSchemaName != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ResourceSchemaName = value;
				return;
			}
			if (string.Equals("x-ms-secondary-master-key", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && SecondaryMasterKey != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SecondaryMasterKey = value;
				return;
			}
			if (string.Equals("x-ms-primary-readonly-key", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PrimaryReadonlyKey != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PrimaryReadonlyKey = value;
				return;
			}
			if (string.Equals("x-ms-transport-request-id", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && TransportRequestID != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				TransportRequestID = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-batch-ordered", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && IsBatchOrdered != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsBatchOrdered = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-resourcetypes", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ResourceTypes != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ResourceTypes = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-query-version", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && QueryVersion != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				QueryVersion = value;
				return;
			}
			break;
		case 26:
			if ((object)"collection-partition-index" == key)
			{
				if (throwIfAlreadyExists && CollectionPartitionIndex != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CollectionPartitionIndex = value;
				return;
			}
			if ((object)"x-ms-enumeration-direction" == key)
			{
				if (throwIfAlreadyExists && EnumerationDirection != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				EnumerationDirection = value;
				return;
			}
			if ((object)"x-ms-cosmos-collectiontype" == key)
			{
				if (throwIfAlreadyExists && RequestedCollectionType != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				RequestedCollectionType = value;
				return;
			}
			if ((object)"x-ms-cosmos-priority-level" == key)
			{
				if (throwIfAlreadyExists && PriorityLevel != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PriorityLevel = value;
				return;
			}
			if (string.Equals("collection-partition-index", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && CollectionPartitionIndex != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CollectionPartitionIndex = value;
				return;
			}
			if (string.Equals("x-ms-enumeration-direction", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && EnumerationDirection != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				EnumerationDirection = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-collectiontype", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && RequestedCollectionType != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				RequestedCollectionType = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-priority-level", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PriorityLevel != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PriorityLevel = value;
				return;
			}
			break;
		case 27:
			if ((object)"x-ms-secondary-readonly-key" == key)
			{
				if (throwIfAlreadyExists && SecondaryReadonlyKey != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SecondaryReadonlyKey = value;
				return;
			}
			if ((object)"x-ms-fanout-operation-state" == key)
			{
				if (throwIfAlreadyExists && FanoutOperationState != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				FanoutOperationState = value;
				return;
			}
			if ((object)"x-ms-cosmos-merge-static-id" == key)
			{
				if (throwIfAlreadyExists && MergeStaticId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				MergeStaticId = value;
				return;
			}
			if (string.Equals("x-ms-secondary-readonly-key", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && SecondaryReadonlyKey != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SecondaryReadonlyKey = value;
				return;
			}
			if (string.Equals("x-ms-fanout-operation-state", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && FanoutOperationState != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				FanoutOperationState = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-merge-static-id", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && MergeStaticId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				MergeStaticId = value;
				return;
			}
			break;
		case 28:
			if ((object)"x-ms-documentdb-partitionkey" == key)
			{
				if (throwIfAlreadyExists && PartitionKey != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PartitionKey = value;
				return;
			}
			if ((object)"x-ms-restore-metadata-filter" == key)
			{
				if (throwIfAlreadyExists && RestoreMetadataFilter != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				RestoreMetadataFilter = value;
				return;
			}
			if ((object)"x-ms-time-to-live-in-seconds" == key)
			{
				if (throwIfAlreadyExists && TimeToLiveInSeconds != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				TimeToLiveInSeconds = value;
				return;
			}
			if ((object)"x-ms-effective-partition-key" == key)
			{
				if (throwIfAlreadyExists && EffectivePartitionKey != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				EffectivePartitionKey = value;
				return;
			}
			if ((object)"x-ms-cosmos-use-systembudget" == key)
			{
				if (throwIfAlreadyExists && UseSystemBudget != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				UseSystemBudget = value;
				return;
			}
			if (string.Equals("x-ms-documentdb-partitionkey", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PartitionKey != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PartitionKey = value;
				return;
			}
			if (string.Equals("x-ms-restore-metadata-filter", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && RestoreMetadataFilter != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				RestoreMetadataFilter = value;
				return;
			}
			if (string.Equals("x-ms-time-to-live-in-seconds", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && TimeToLiveInSeconds != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				TimeToLiveInSeconds = value;
				return;
			}
			if (string.Equals("x-ms-effective-partition-key", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && EffectivePartitionKey != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				EffectivePartitionKey = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-use-systembudget", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && UseSystemBudget != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				UseSystemBudget = value;
				return;
			}
			break;
		case 29:
			if (string.Equals("x-ms-cosmos-client-ip-address", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ClientIpAddress != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ClientIpAddress = value;
				return;
			}
			break;
		case 30:
			if ((object)"x-ms-documentdb-expiry-seconds" == key)
			{
				if (throwIfAlreadyExists && ResourceTokenExpiry != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ResourceTokenExpiry = value;
				return;
			}
			if ((object)"x-ms-documentdb-partitioncount" == key)
			{
				if (throwIfAlreadyExists && PartitionCount != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PartitionCount = value;
				return;
			}
			if ((object)"x-ms-documentdb-collection-rid" == key)
			{
				if (throwIfAlreadyExists && CollectionRid != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CollectionRid = value;
				return;
			}
			if ((object)"x-ms-partition-resource-filter" == key)
			{
				if (throwIfAlreadyExists && PartitionResourceFilter != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PartitionResourceFilter = value;
				return;
			}
			if ((object)"x-ms-exclude-system-properties" == key)
			{
				if (throwIfAlreadyExists && ExcludeSystemProperties != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ExcludeSystemProperties = value;
				return;
			}
			if ((object)"x-ms-cosmos-alter-type-request" == key)
			{
				if (throwIfAlreadyExists && IsCassandraAlterTypeRequest != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsCassandraAlterTypeRequest = value;
				return;
			}
			if (string.Equals("x-ms-documentdb-expiry-seconds", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ResourceTokenExpiry != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ResourceTokenExpiry = value;
				return;
			}
			if (string.Equals("x-ms-documentdb-partitioncount", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PartitionCount != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PartitionCount = value;
				return;
			}
			if (string.Equals("x-ms-documentdb-collection-rid", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && CollectionRid != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CollectionRid = value;
				return;
			}
			if (string.Equals("x-ms-partition-resource-filter", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PartitionResourceFilter != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PartitionResourceFilter = value;
				return;
			}
			if (string.Equals("x-ms-exclude-system-properties", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ExcludeSystemProperties != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ExcludeSystemProperties = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-alter-type-request", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && IsCassandraAlterTypeRequest != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsCassandraAlterTypeRequest = value;
				return;
			}
			break;
		case 31:
			if ((object)"x-ms-client-retry-attempt-count" == key)
			{
				if (throwIfAlreadyExists && ClientRetryAttemptCount != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ClientRetryAttemptCount = value;
				return;
			}
			if ((object)"x-ms-can-offer-replace-complete" == key)
			{
				if (throwIfAlreadyExists && CanOfferReplaceComplete != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CanOfferReplaceComplete = value;
				return;
			}
			if ((object)"x-ms-binary-passthrough-request" == key)
			{
				if (throwIfAlreadyExists && BinaryPassthroughRequest != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				BinaryPassthroughRequest = value;
				return;
			}
			if ((object)"x-ms-cosmos-is-client-encrypted" == key)
			{
				if (throwIfAlreadyExists && IsClientEncrypted != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsClientEncrypted = value;
				return;
			}
			if ((object)"x-ms-cosmos-systemdocument-type" == key)
			{
				if (throwIfAlreadyExists && SystemDocumentType != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SystemDocumentType = value;
				return;
			}
			if ((object)"x-ms-cosmos-collection-truncate" == key)
			{
				if (throwIfAlreadyExists && CollectionTruncate != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CollectionTruncate = value;
				return;
			}
			if (string.Equals("x-ms-client-retry-attempt-count", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ClientRetryAttemptCount != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ClientRetryAttemptCount = value;
				return;
			}
			if (string.Equals("x-ms-can-offer-replace-complete", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && CanOfferReplaceComplete != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CanOfferReplaceComplete = value;
				return;
			}
			if (string.Equals("x-ms-binary-passthrough-request", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && BinaryPassthroughRequest != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				BinaryPassthroughRequest = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-is-client-encrypted", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && IsClientEncrypted != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsClientEncrypted = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-systemdocument-type", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && SystemDocumentType != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SystemDocumentType = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-collection-truncate", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && CollectionTruncate != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CollectionTruncate = value;
				return;
			}
			break;
		case 32:
			if ((object)"x-ms-migratecollection-directive" == key)
			{
				if (throwIfAlreadyExists && MigrateCollectionDirective != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				MigrateCollectionDirective = value;
				return;
			}
			if ((object)"x-ms-target-global-committed-lsn" == key)
			{
				if (throwIfAlreadyExists && TargetGlobalCommittedLsn != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				TargetGlobalCommittedLsn = value;
				return;
			}
			if ((object)"x-ms-documentdb-force-query-scan" == key)
			{
				if (throwIfAlreadyExists && ForceQueryScan != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ForceQueryScan = value;
				return;
			}
			if ((object)"x-ms-cosmos-max-polling-interval" == key)
			{
				if (throwIfAlreadyExists && MaxPollingIntervalMilliseconds != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				MaxPollingIntervalMilliseconds = value;
				return;
			}
			if ((object)"x-ms-cosmos-populateindexmetrics" == key)
			{
				if (throwIfAlreadyExists && PopulateIndexMetrics != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateIndexMetrics = value;
				return;
			}
			if (string.Equals("x-ms-migratecollection-directive", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && MigrateCollectionDirective != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				MigrateCollectionDirective = value;
				return;
			}
			if (string.Equals("x-ms-target-global-committed-lsn", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && TargetGlobalCommittedLsn != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				TargetGlobalCommittedLsn = value;
				return;
			}
			if (string.Equals("x-ms-documentdb-force-query-scan", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ForceQueryScan != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ForceQueryScan = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-max-polling-interval", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && MaxPollingIntervalMilliseconds != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				MaxPollingIntervalMilliseconds = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-populateindexmetrics", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PopulateIndexMetrics != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateIndexMetrics = value;
				return;
			}
			break;
		case 33:
			if ((object)"x-ms-documentdb-query-enable-scan" == key)
			{
				if (throwIfAlreadyExists && EnableScanInQuery != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				EnableScanInQuery = value;
				return;
			}
			if ((object)"x-ms-documentdb-query-emit-traces" == key)
			{
				if (throwIfAlreadyExists && EmitVerboseTracesInQuery != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				EmitVerboseTracesInQuery = value;
				return;
			}
			if ((object)"x-ms-documentdb-populatequotainfo" == key)
			{
				if (throwIfAlreadyExists && PopulateQuotaInfo != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateQuotaInfo = value;
				return;
			}
			if ((object)"x-ms-cosmos-preserve-full-content" == key)
			{
				if (throwIfAlreadyExists && PreserveFullContent != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PreserveFullContent = value;
				return;
			}
			if ((object)"x-ms-cosmos-populate-logstoreinfo" == key)
			{
				if (throwIfAlreadyExists && PopulateLogStoreInfo != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateLogStoreInfo = value;
				return;
			}
			if ((object)"x-ms-cosmos-correlated-activityid" == key)
			{
				if (throwIfAlreadyExists && CorrelatedActivityId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CorrelatedActivityId = value;
				return;
			}
			if (string.Equals("x-ms-documentdb-query-enable-scan", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && EnableScanInQuery != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				EnableScanInQuery = value;
				return;
			}
			if (string.Equals("x-ms-documentdb-query-emit-traces", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && EmitVerboseTracesInQuery != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				EmitVerboseTracesInQuery = value;
				return;
			}
			if (string.Equals("x-ms-documentdb-populatequotainfo", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PopulateQuotaInfo != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateQuotaInfo = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-preserve-full-content", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PreserveFullContent != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PreserveFullContent = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-populate-logstoreinfo", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PopulateLogStoreInfo != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateLogStoreInfo = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-correlated-activityid", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && CorrelatedActivityId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CorrelatedActivityId = value;
				return;
			}
			break;
		case 34:
			if ((object)"x-ms-cosmos-allow-tentative-writes" == key)
			{
				if (throwIfAlreadyExists && AllowTentativeWrites != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				AllowTentativeWrites = value;
				return;
			}
			if ((object)"x-ms-cosmos-use-archival-partition" == key)
			{
				if (throwIfAlreadyExists && UseArchivalPartition != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				UseArchivalPartition = value;
				return;
			}
			if ((object)"x-ms-cosmos-populate-capacity-type" == key)
			{
				if (throwIfAlreadyExists && PopulateCapacityType != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateCapacityType = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-allow-tentative-writes", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && AllowTentativeWrites != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				AllowTentativeWrites = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-use-archival-partition", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && UseArchivalPartition != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				UseArchivalPartition = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-populate-capacity-type", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PopulateCapacityType != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateCapacityType = value;
				return;
			}
			break;
		case 35:
			if ((object)"x-ms-documentdb-pre-trigger-include" == key)
			{
				if (throwIfAlreadyExists && PreTriggerInclude != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PreTriggerInclude = value;
				return;
			}
			if ((object)"x-ms-documentdb-pre-trigger-exclude" == key)
			{
				if (throwIfAlreadyExists && PreTriggerExclude != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PreTriggerExclude = value;
				return;
			}
			if ((object)"x-ms-documentdb-partitionkeyrangeid" == key)
			{
				if (throwIfAlreadyExists && PartitionKeyRangeId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PartitionKeyRangeId = value;
				return;
			}
			if ((object)"x-ms-documentdb-filterby-schema-rid" == key)
			{
				if (throwIfAlreadyExists && FilterBySchemaResourceId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				FilterBySchemaResourceId = value;
				return;
			}
			if ((object)"x-ms-collection-security-identifier" == key)
			{
				if (throwIfAlreadyExists && CollectionRemoteStorageSecurityIdentifier != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CollectionRemoteStorageSecurityIdentifier = value;
				return;
			}
			if ((object)"x-ms-remaining-time-in-ms-on-client" == key)
			{
				if (throwIfAlreadyExists && RemainingTimeInMsOnClientRequest != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				RemainingTimeInMsOnClientRequest = value;
				return;
			}
			if ((object)"x-ms-cosmos-batch-continue-on-error" == key)
			{
				if (throwIfAlreadyExists && ShouldBatchContinueOnError != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ShouldBatchContinueOnError = value;
				return;
			}
			if ((object)"x-ms-cosmos-intended-collection-rid" == key)
			{
				if (throwIfAlreadyExists && IntendedCollectionRid != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IntendedCollectionRid = value;
				return;
			}
			if ((object)"x-ms-cosmos-populateindexmetrics-V2" == key)
			{
				if (throwIfAlreadyExists && PopulateIndexMetricsV2 != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateIndexMetricsV2 = value;
				return;
			}
			if (string.Equals("x-ms-documentdb-pre-trigger-include", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PreTriggerInclude != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PreTriggerInclude = value;
				return;
			}
			if (string.Equals("x-ms-documentdb-pre-trigger-exclude", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PreTriggerExclude != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PreTriggerExclude = value;
				return;
			}
			if (string.Equals("x-ms-documentdb-partitionkeyrangeid", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PartitionKeyRangeId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PartitionKeyRangeId = value;
				return;
			}
			if (string.Equals("x-ms-documentdb-filterby-schema-rid", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && FilterBySchemaResourceId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				FilterBySchemaResourceId = value;
				return;
			}
			if (string.Equals("x-ms-collection-security-identifier", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && CollectionRemoteStorageSecurityIdentifier != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CollectionRemoteStorageSecurityIdentifier = value;
				return;
			}
			if (string.Equals("x-ms-remaining-time-in-ms-on-client", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && RemainingTimeInMsOnClientRequest != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				RemainingTimeInMsOnClientRequest = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-batch-continue-on-error", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ShouldBatchContinueOnError != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ShouldBatchContinueOnError = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-intended-collection-rid", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && IntendedCollectionRid != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IntendedCollectionRid = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-populateindexmetrics-V2", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PopulateIndexMetricsV2 != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateIndexMetricsV2 = value;
				return;
			}
			break;
		case 36:
			if ((object)"x-ms-documentdb-post-trigger-include" == key)
			{
				if (throwIfAlreadyExists && PostTriggerInclude != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PostTriggerInclude = value;
				return;
			}
			if ((object)"x-ms-documentdb-post-trigger-exclude" == key)
			{
				if (throwIfAlreadyExists && PostTriggerExclude != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PostTriggerExclude = value;
				return;
			}
			if ((object)"x-ms-documentdb-populatequerymetrics" == key)
			{
				if (throwIfAlreadyExists && PopulateQueryMetrics != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateQueryMetrics = value;
				return;
			}
			if ((object)"x-ms-cosmos-internal-is-user-request" == key)
			{
				if (throwIfAlreadyExists && IsUserRequest != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsUserRequest = value;
				return;
			}
			if ((object)"x-ms-cosmos-include-tentative-writes" == key)
			{
				if (throwIfAlreadyExists && IncludeTentativeWrites != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IncludeTentativeWrites = value;
				return;
			}
			if ((object)"x-ms-cosmos-is-retried-write-request" == key)
			{
				if (throwIfAlreadyExists && IsRetriedWriteRequest != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsRetriedWriteRequest = value;
				return;
			}
			if ((object)"x-ms-cosmos-prune-collection-schemas" == key)
			{
				if (throwIfAlreadyExists && PruneCollectionSchemas != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PruneCollectionSchemas = value;
				return;
			}
			if ((object)"x-ms-cosmos-supported-query-features" == key)
			{
				if (throwIfAlreadyExists && SupportedQueryFeatures != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SupportedQueryFeatures = value;
				return;
			}
			if (string.Equals("x-ms-documentdb-post-trigger-include", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PostTriggerInclude != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PostTriggerInclude = value;
				return;
			}
			if (string.Equals("x-ms-documentdb-post-trigger-exclude", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PostTriggerExclude != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PostTriggerExclude = value;
				return;
			}
			if (string.Equals("x-ms-documentdb-populatequerymetrics", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PopulateQueryMetrics != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateQueryMetrics = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-internal-is-user-request", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && IsUserRequest != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsUserRequest = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-include-tentative-writes", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && IncludeTentativeWrites != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IncludeTentativeWrites = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-is-retried-write-request", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && IsRetriedWriteRequest != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsRetriedWriteRequest = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-prune-collection-schemas", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PruneCollectionSchemas != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PruneCollectionSchemas = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-supported-query-features", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && SupportedQueryFeatures != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SupportedQueryFeatures = value;
				return;
			}
			break;
		case 37:
			if ((object)"x-ms-documentdb-script-enable-logging" == key)
			{
				if (throwIfAlreadyExists && EnableLogging != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				EnableLogging = value;
				return;
			}
			if ((object)"x-ms-documentdb-populateresourcecount" == key)
			{
				if (throwIfAlreadyExists && PopulateResourceCount != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateResourceCount = value;
				return;
			}
			if ((object)"x-ms-cosmos-sdk-supportedcapabilities" == key)
			{
				if (throwIfAlreadyExists && SDKSupportedCapabilities != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SDKSupportedCapabilities = value;
				return;
			}
			if ((object)"x-ms-cosmos-builder-client-identifier" == key)
			{
				if (throwIfAlreadyExists && BuilderClientIdentifier != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				BuilderClientIdentifier = value;
				return;
			}
			if ((object)"x-ms-cosmos-is-request-not-authorized" == key)
			{
				if (throwIfAlreadyExists && IsRequestNotAuthorized != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsRequestNotAuthorized = value;
				return;
			}
			if (string.Equals("x-ms-documentdb-script-enable-logging", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && EnableLogging != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				EnableLogging = value;
				return;
			}
			if (string.Equals("x-ms-documentdb-populateresourcecount", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PopulateResourceCount != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateResourceCount = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-sdk-supportedcapabilities", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && SDKSupportedCapabilities != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SDKSupportedCapabilities = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-builder-client-identifier", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && BuilderClientIdentifier != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				BuilderClientIdentifier = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-is-request-not-authorized", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && IsRequestNotAuthorized != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsRequestNotAuthorized = value;
				return;
			}
			break;
		case 38:
			if ((object)"x-ms-cosmos-migrate-offer-to-autopilot" == key)
			{
				if (throwIfAlreadyExists && MigrateOfferToAutopilot != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				MigrateOfferToAutopilot = value;
				return;
			}
			if ((object)"x-ms-cosmos-retriable-write-request-id" == key)
			{
				if (throwIfAlreadyExists && RetriableWriteRequestId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				RetriableWriteRequestId = value;
				return;
			}
			if ((object)"x-ms-cosmos-source-collection-if-match" == key)
			{
				if (throwIfAlreadyExists && SourceCollectionIfMatch != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SourceCollectionIfMatch = value;
				return;
			}
			if ((object)"x-ms-cosmos-use-background-task-budget" == key)
			{
				if (throwIfAlreadyExists && UseUserBackgroundBudget != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				UseUserBackgroundBudget = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-migrate-offer-to-autopilot", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && MigrateOfferToAutopilot != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				MigrateOfferToAutopilot = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-retriable-write-request-id", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && RetriableWriteRequestId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				RetriableWriteRequestId = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-source-collection-if-match", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && SourceCollectionIfMatch != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SourceCollectionIfMatch = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-use-background-task-budget", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && UseUserBackgroundBudget != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				UseUserBackgroundBudget = value;
				return;
			}
			break;
		case 39:
			if ((object)"x-ms-cosmos-internal-truncate-merge-log" == key)
			{
				if (throwIfAlreadyExists && TruncateMergeLogRequest != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				TruncateMergeLogRequest = value;
				return;
			}
			if ((object)"x-ms-cosmos-internal-serverless-request" == key)
			{
				if (throwIfAlreadyExists && IsInternalServerlessRequest != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsInternalServerlessRequest = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-internal-truncate-merge-log", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && TruncateMergeLogRequest != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				TruncateMergeLogRequest = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-internal-serverless-request", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && IsInternalServerlessRequest != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsInternalServerlessRequest = value;
				return;
			}
			break;
		case 40:
			if ((object)"x-ms-enable-dynamic-rid-range-allocation" == key)
			{
				if (throwIfAlreadyExists && EnableDynamicRidRangeAllocation != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				EnableDynamicRidRangeAllocation = value;
				return;
			}
			if ((object)"x-ms-cosmos-uniqueindex-reindexing-state" == key)
			{
				if (throwIfAlreadyExists && UniqueIndexReIndexingState != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				UniqueIndexReIndexingState = value;
				return;
			}
			if (string.Equals("x-ms-enable-dynamic-rid-range-allocation", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && EnableDynamicRidRangeAllocation != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				EnableDynamicRidRangeAllocation = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-uniqueindex-reindexing-state", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && UniqueIndexReIndexingState != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				UniqueIndexReIndexingState = value;
				return;
			}
			break;
		case 41:
			if ((object)"x-ms-cosmos-force-database-account-update" == key)
			{
				if (throwIfAlreadyExists && ForceDatabaseAccountUpdate != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ForceDatabaseAccountUpdate = value;
				return;
			}
			if ((object)"x-ms-cosmos-query-optimisticdirectexecute" == key)
			{
				if (throwIfAlreadyExists && OptimisticDirectExecute != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				OptimisticDirectExecute = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-force-database-account-update", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ForceDatabaseAccountUpdate != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ForceDatabaseAccountUpdate = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-query-optimisticdirectexecute", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && OptimisticDirectExecute != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				OptimisticDirectExecute = value;
				return;
			}
			break;
		case 42:
			if ((object)"x-ms-cosmos-internal-merge-checkpoint-glsn" == key)
			{
				if (throwIfAlreadyExists && MergeCheckPointGLSN != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				MergeCheckPointGLSN = value;
				return;
			}
			if ((object)"x-ms-should-return-current-server-datetime" == key)
			{
				if (throwIfAlreadyExists && ShouldReturnCurrentServerDateTime != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ShouldReturnCurrentServerDateTime = value;
				return;
			}
			if ((object)"x-ms-cosmos-changefeed-wire-format-version" == key)
			{
				if (throwIfAlreadyExists && ChangeFeedWireFormatVersion != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ChangeFeedWireFormatVersion = value;
				return;
			}
			if ((object)"x-ms-documentdb-query-enablecrosspartition" == key)
			{
				if (throwIfAlreadyExists && EnableCrossPartitionQuery != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				EnableCrossPartitionQuery = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-internal-merge-checkpoint-glsn", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && MergeCheckPointGLSN != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				MergeCheckPointGLSN = value;
				return;
			}
			if (string.Equals("x-ms-should-return-current-server-datetime", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ShouldReturnCurrentServerDateTime != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ShouldReturnCurrentServerDateTime = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-changefeed-wire-format-version", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ChangeFeedWireFormatVersion != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ChangeFeedWireFormatVersion = value;
				return;
			}
			if (string.Equals("x-ms-documentdb-query-enablecrosspartition", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && EnableCrossPartitionQuery != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				EnableCrossPartitionQuery = value;
				return;
			}
			break;
		case 43:
			if ((object)"x-ms-documentdb-disable-ru-per-minute-usage" == key)
			{
				if (throwIfAlreadyExists && DisableRUPerMinuteUsage != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				DisableRUPerMinuteUsage = value;
				return;
			}
			if ((object)"x-ms-documentdb-populatepartitionstatistics" == key)
			{
				if (throwIfAlreadyExists && PopulatePartitionStatistics != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulatePartitionStatistics = value;
				return;
			}
			if ((object)"x-ms-cosmos-force-sidebyside-indexmigration" == key)
			{
				if (throwIfAlreadyExists && ForceSideBySideIndexMigration != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ForceSideBySideIndexMigration = value;
				return;
			}
			if ((object)"x-ms-cosmos-unique-index-name-encoding-mode" == key)
			{
				if (throwIfAlreadyExists && UniqueIndexNameEncodingMode != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				UniqueIndexNameEncodingMode = value;
				return;
			}
			if ((object)"x-ms-cosmos-supported-serialization-formats" == key)
			{
				if (throwIfAlreadyExists && SupportedSerializationFormats != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SupportedSerializationFormats = value;
				return;
			}
			if (string.Equals("x-ms-documentdb-disable-ru-per-minute-usage", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && DisableRUPerMinuteUsage != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				DisableRUPerMinuteUsage = value;
				return;
			}
			if (string.Equals("x-ms-documentdb-populatepartitionstatistics", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PopulatePartitionStatistics != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulatePartitionStatistics = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-force-sidebyside-indexmigration", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ForceSideBySideIndexMigration != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ForceSideBySideIndexMigration = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-unique-index-name-encoding-mode", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && UniqueIndexNameEncodingMode != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				UniqueIndexNameEncodingMode = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-supported-serialization-formats", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && SupportedSerializationFormats != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SupportedSerializationFormats = value;
				return;
			}
			break;
		case 44:
			if ((object)"x-ms-documentdb-content-serialization-format" == key)
			{
				if (throwIfAlreadyExists && ContentSerializationFormat != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ContentSerializationFormat = value;
				return;
			}
			if ((object)"x-ms-cosmos-populate-oldest-active-schema-id" == key)
			{
				if (throwIfAlreadyExists && PopulateOldestActiveSchemaId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateOldestActiveSchemaId = value;
				return;
			}
			if ((object)"x-ms-documentdb-query-iscontinuationexpected" == key)
			{
				if (throwIfAlreadyExists && IsContinuationExpected != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsContinuationExpected = value;
				return;
			}
			if (string.Equals("x-ms-documentdb-content-serialization-format", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ContentSerializationFormat != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ContentSerializationFormat = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-populate-oldest-active-schema-id", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PopulateOldestActiveSchemaId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateOldestActiveSchemaId = value;
				return;
			}
			if (string.Equals("x-ms-documentdb-query-iscontinuationexpected", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && IsContinuationExpected != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsContinuationExpected = value;
				return;
			}
			break;
		case 45:
			if ((object)"x-ms-cosmos-start-full-fidelity-if-none-match" == key)
			{
				if (throwIfAlreadyExists && ChangeFeedStartFullFidelityIfNoneMatch != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ChangeFeedStartFullFidelityIfNoneMatch = value;
				return;
			}
			if ((object)"x-ms-cosmos-internal-system-restore-operation" == key)
			{
				if (throwIfAlreadyExists && SystemRestoreOperation != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SystemRestoreOperation = value;
				return;
			}
			if ((object)"x-ms-cosmos-internal-is-throughputcap-request" == key)
			{
				if (throwIfAlreadyExists && IsThroughputCapRequest != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsThroughputCapRequest = value;
				return;
			}
			if ((object)"x-ms-cosmos-populate-byok-encryption-progress" == key)
			{
				if (throwIfAlreadyExists && PopulateByokEncryptionProgress != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateByokEncryptionProgress = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-start-full-fidelity-if-none-match", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ChangeFeedStartFullFidelityIfNoneMatch != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ChangeFeedStartFullFidelityIfNoneMatch = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-internal-system-restore-operation", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && SystemRestoreOperation != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SystemRestoreOperation = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-internal-is-throughputcap-request", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && IsThroughputCapRequest != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsThroughputCapRequest = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-populate-byok-encryption-progress", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PopulateByokEncryptionProgress != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateByokEncryptionProgress = value;
				return;
			}
			break;
		case 46:
			if ((object)"x-ms-cosmos-migrate-offer-to-manual-throughput" == key)
			{
				if (throwIfAlreadyExists && MigrateOfferToManualThroughput != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				MigrateOfferToManualThroughput = value;
				return;
			}
			if ((object)"x-ms-cosmos-skip-refresh-databaseaccountconfig" == key)
			{
				if (throwIfAlreadyExists && SkipRefreshDatabaseAccountConfigs != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SkipRefreshDatabaseAccountConfigs = value;
				return;
			}
			if ((object)"x-ms-cosmos-internal-migrated-fixed-collection" == key)
			{
				if (throwIfAlreadyExists && IsMigratedFixedCollection != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsMigratedFixedCollection = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-migrate-offer-to-manual-throughput", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && MigrateOfferToManualThroughput != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				MigrateOfferToManualThroughput = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-skip-refresh-databaseaccountconfig", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && SkipRefreshDatabaseAccountConfigs != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SkipRefreshDatabaseAccountConfigs = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-internal-migrated-fixed-collection", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && IsMigratedFixedCollection != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsMigratedFixedCollection = value;
				return;
			}
			break;
		case 47:
			if ((object)"x-ms-documentdb-supportspatiallegacycoordinates" == key)
			{
				if (throwIfAlreadyExists && SupportSpatialLegacyCoordinates != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SupportSpatialLegacyCoordinates = value;
				return;
			}
			if ((object)"x-ms-cosmos-collection-child-resourcename-limit" == key)
			{
				if (throwIfAlreadyExists && CollectionChildResourceNameLimitInBytes != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CollectionChildResourceNameLimitInBytes = value;
				return;
			}
			if ((object)"x-ms-cosmos-add-resource-properties-to-response" == key)
			{
				if (throwIfAlreadyExists && AddResourcePropertiesToResponse != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				AddResourcePropertiesToResponse = value;
				return;
			}
			if ((object)"x-ms-cosmos-internal-is-materialized-view-build" == key)
			{
				if (throwIfAlreadyExists && IsMaterializedViewBuild != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsMaterializedViewBuild = value;
				return;
			}
			if (string.Equals("x-ms-documentdb-supportspatiallegacycoordinates", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && SupportSpatialLegacyCoordinates != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SupportSpatialLegacyCoordinates = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-collection-child-resourcename-limit", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && CollectionChildResourceNameLimitInBytes != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CollectionChildResourceNameLimitInBytes = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-add-resource-properties-to-response", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && AddResourcePropertiesToResponse != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				AddResourcePropertiesToResponse = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-internal-is-materialized-view-build", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && IsMaterializedViewBuild != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsMaterializedViewBuild = value;
				return;
			}
			break;
		case 48:
			if ((object)"x-ms-documentdb-populatecollectionthroughputinfo" == key)
			{
				if (throwIfAlreadyExists && PopulateCollectionThroughputInfo != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateCollectionThroughputInfo = value;
				return;
			}
			if ((object)"x-ms-cosmos-internal-get-all-partition-key-stats" == key)
			{
				if (throwIfAlreadyExists && GetAllPartitionKeyStatistics != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				GetAllPartitionKeyStatistics = value;
				return;
			}
			if ((object)"x-ms-cosmosdb-populateuniqueindexreindexprogress" == key)
			{
				if (throwIfAlreadyExists && PopulateUniqueIndexReIndexProgress != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateUniqueIndexReIndexProgress = value;
				return;
			}
			if ((object)"x-ms-cosmos-internal-allow-restore-params-update" == key)
			{
				if (throwIfAlreadyExists && AllowRestoreParamsUpdate != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				AllowRestoreParamsUpdate = value;
				return;
			}
			if ((object)"x-ms-cosmos-internal-high-priority-forced-backup" == key)
			{
				if (throwIfAlreadyExists && HighPriorityForcedBackup != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				HighPriorityForcedBackup = value;
				return;
			}
			if (string.Equals("x-ms-documentdb-populatecollectionthroughputinfo", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PopulateCollectionThroughputInfo != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateCollectionThroughputInfo = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-internal-get-all-partition-key-stats", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && GetAllPartitionKeyStatistics != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				GetAllPartitionKeyStatistics = value;
				return;
			}
			if (string.Equals("x-ms-cosmosdb-populateuniqueindexreindexprogress", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PopulateUniqueIndexReIndexProgress != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateUniqueIndexReIndexProgress = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-internal-allow-restore-params-update", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && AllowRestoreParamsUpdate != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				AllowRestoreParamsUpdate = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-internal-high-priority-forced-backup", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && HighPriorityForcedBackup != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				HighPriorityForcedBackup = value;
				return;
			}
			break;
		case 49:
			if (string.Equals("x-ms-documentdb-usepolygonssmallerthanahemisphere", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && UsePolygonsSmallerThanAHemisphere != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				UsePolygonsSmallerThanAHemisphere = value;
				return;
			}
			break;
		case 50:
			if ((object)"x-ms-documentdb-responsecontinuationtokenlimitinkb" == key)
			{
				if (throwIfAlreadyExists && ResponseContinuationTokenLimitInKB != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ResponseContinuationTokenLimitInKB = value;
				return;
			}
			if ((object)"x-ms-cosmos-populate-analytical-migration-progress" == key)
			{
				if (throwIfAlreadyExists && PopulateAnalyticalMigrationProgress != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateAnalyticalMigrationProgress = value;
				return;
			}
			if ((object)"x-ms-cosmos-internal-update-offer-state-to-pending" == key)
			{
				if (throwIfAlreadyExists && UpdateOfferStateToPending != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				UpdateOfferStateToPending = value;
				return;
			}
			if (string.Equals("x-ms-documentdb-responsecontinuationtokenlimitinkb", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ResponseContinuationTokenLimitInKB != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ResponseContinuationTokenLimitInKB = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-populate-analytical-migration-progress", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PopulateAnalyticalMigrationProgress != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateAnalyticalMigrationProgress = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-internal-update-offer-state-to-pending", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && UpdateOfferStateToPending != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				UpdateOfferStateToPending = value;
				return;
			}
			break;
		case 51:
			if ((object)"x-ms-documentdb-query-enable-low-precision-order-by" == key)
			{
				if (throwIfAlreadyExists && EnableLowPrecisionOrderBy != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				EnableLowPrecisionOrderBy = value;
				return;
			}
			if ((object)"x-ms-cosmos-retriable-write-request-start-timestamp" == key)
			{
				if (throwIfAlreadyExists && RetriableWriteRequestStartTimestamp != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				RetriableWriteRequestStartTimestamp = value;
				return;
			}
			if ((object)"x-ms-cosmos-internal-populate-document-record-count" == key)
			{
				if (throwIfAlreadyExists && PopulateDocumentRecordCount != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateDocumentRecordCount = value;
				return;
			}
			if (string.Equals("x-ms-documentdb-query-enable-low-precision-order-by", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && EnableLowPrecisionOrderBy != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				EnableLowPrecisionOrderBy = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-retriable-write-request-start-timestamp", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && RetriableWriteRequestStartTimestamp != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				RetriableWriteRequestStartTimestamp = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-internal-populate-document-record-count", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PopulateDocumentRecordCount != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateDocumentRecordCount = value;
				return;
			}
			break;
		case 52:
			if ((object)"x-ms-cosmos-internal-offer-replace-ru-redistribution" == key)
			{
				if (throwIfAlreadyExists && OfferReplaceRURedistribution != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				OfferReplaceRURedistribution = value;
				return;
			}
			if ((object)"x-ms-documentdb-query-parallelizecrosspartitionquery" == key)
			{
				if (throwIfAlreadyExists && ParallelizeCrossPartitionQuery != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ParallelizeCrossPartitionQuery = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-internal-offer-replace-ru-redistribution", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && OfferReplaceRURedistribution != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				OfferReplaceRURedistribution = value;
				return;
			}
			if (string.Equals("x-ms-documentdb-query-parallelizecrosspartitionquery", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ParallelizeCrossPartitionQuery != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ParallelizeCrossPartitionQuery = value;
				return;
			}
			break;
		case 53:
			if ((object)"x-ms-cosmos-internal-is-ru-per-gb-enforcement-request" == key)
			{
				if (throwIfAlreadyExists && IsRUPerGBEnforcementRequest != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsRUPerGBEnforcementRequest = value;
				return;
			}
			if ((object)"x-ms-cosmos-internal-is-offer-storage-refresh-request" == key)
			{
				if (throwIfAlreadyExists && IsOfferStorageRefreshRequest != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsOfferStorageRefreshRequest = value;
				return;
			}
			if ((object)"x-ms-cosmos-internal-populate-min-glsn-for-relocation" == key)
			{
				if (throwIfAlreadyExists && PopulateMinGLSNForDocumentOperations != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateMinGLSNForDocumentOperations = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-internal-is-ru-per-gb-enforcement-request", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && IsRUPerGBEnforcementRequest != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsRUPerGBEnforcementRequest = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-internal-is-offer-storage-refresh-request", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && IsOfferStorageRefreshRequest != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsOfferStorageRefreshRequest = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-internal-populate-min-glsn-for-relocation", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PopulateMinGLSNForDocumentOperations != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateMinGLSNForDocumentOperations = value;
				return;
			}
			break;
		case 54:
			if ((object)"x-ms-cosmos-include-physical-partition-throughput-info" == key)
			{
				if (throwIfAlreadyExists && IncludePhysicalPartitionThroughputInfo != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IncludePhysicalPartitionThroughputInfo = value;
				return;
			}
			if ((object)"x-ms-cosmos-is-materialized-view-source-schema-replace" == key)
			{
				if (throwIfAlreadyExists && IsMaterializedViewSourceSchemaReplaceBatchRequest != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsMaterializedViewSourceSchemaReplaceBatchRequest = value;
				return;
			}
			if ((object)"x-ms-cosmos-populate-current-partition-throughput-info" == key)
			{
				if (throwIfAlreadyExists && PopulateCurrentPartitionThroughputInfo != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateCurrentPartitionThroughputInfo = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-include-physical-partition-throughput-info", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && IncludePhysicalPartitionThroughputInfo != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IncludePhysicalPartitionThroughputInfo = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-is-materialized-view-source-schema-replace", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && IsMaterializedViewSourceSchemaReplaceBatchRequest != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsMaterializedViewSourceSchemaReplaceBatchRequest = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-populate-current-partition-throughput-info", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PopulateCurrentPartitionThroughputInfo != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateCurrentPartitionThroughputInfo = value;
				return;
			}
			break;
		case 55:
			if ((object)"x-ms-cosmos-internal-update-offer-state-restore-pending" == key)
			{
				if (throwIfAlreadyExists && UpdateOfferStateToRestorePending != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				UpdateOfferStateToRestorePending = value;
				return;
			}
			if ((object)"x-ms-documentdb-query-sqlqueryforpartitionkeyextraction" == key)
			{
				if (throwIfAlreadyExists && SqlQueryForPartitionKeyExtraction != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SqlQueryForPartitionKeyExtraction = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-internal-update-offer-state-restore-pending", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && UpdateOfferStateToRestorePending != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				UpdateOfferStateToRestorePending = value;
				return;
			}
			if (string.Equals("x-ms-documentdb-query-sqlqueryforpartitionkeyextraction", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && SqlQueryForPartitionKeyExtraction != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SqlQueryForPartitionKeyExtraction = value;
				return;
			}
			break;
		case 56:
			if (string.Equals("x-ms-cosmos-collection-child-contentlength-resourcelimit", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && CollectionChildResourceContentLimitInKB != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CollectionChildResourceContentLimitInKB = value;
				return;
			}
			break;
		case 57:
			if (string.Equals("x-ms-cosmos-internal-populate-unflushed-merge-entry-count", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PopulateUnflushedMergeEntryCount != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateUnflushedMergeEntryCount = value;
				return;
			}
			break;
		case 58:
			if ((object)"x-ms-cosmos-internal-ignore-system-lowering-max-throughput" == key)
			{
				if (throwIfAlreadyExists && IgnoreSystemLoweringMaxThroughput != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IgnoreSystemLoweringMaxThroughput = value;
				return;
			}
			if ((object)"x-ms-cosmos-internal-set-master-resources-deletion-pending" == key)
			{
				if (throwIfAlreadyExists && SetMasterResourcesDeletionPending != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SetMasterResourcesDeletionPending = value;
				return;
			}
			if ((object)"x-ms-cosmos-internal-populate-highest-tentative-write-llsn" == key)
			{
				if (throwIfAlreadyExists && PopulateHighestTentativeWriteLLSN != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateHighestTentativeWriteLLSN = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-internal-ignore-system-lowering-max-throughput", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && IgnoreSystemLoweringMaxThroughput != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IgnoreSystemLoweringMaxThroughput = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-internal-set-master-resources-deletion-pending", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && SetMasterResourcesDeletionPending != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SetMasterResourcesDeletionPending = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-internal-populate-highest-tentative-write-llsn", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PopulateHighestTentativeWriteLLSN != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PopulateHighestTentativeWriteLLSN = value;
				return;
			}
			break;
		case 59:
			if ((object)"x-ms-cosmos-internal-update-max-throughput-ever-provisioned" == key)
			{
				if (throwIfAlreadyExists && UpdateMaxThroughputEverProvisioned != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				UpdateMaxThroughputEverProvisioned = value;
				return;
			}
			if ((object)"x-ms-cosmos-internal-enable-conflictresolutionpolicy-update" == key)
			{
				if (throwIfAlreadyExists && EnableConflictResolutionPolicyUpdate != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				EnableConflictResolutionPolicyUpdate = value;
				return;
			}
			if ((object)"x-ms-cosmos-internal-allow-document-reads-in-offline-region" == key)
			{
				if (throwIfAlreadyExists && AllowDocumentReadsInOfflineRegion != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				AllowDocumentReadsInOfflineRegion = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-internal-update-max-throughput-ever-provisioned", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && UpdateMaxThroughputEverProvisioned != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				UpdateMaxThroughputEverProvisioned = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-internal-enable-conflictresolutionpolicy-update", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && EnableConflictResolutionPolicyUpdate != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				EnableConflictResolutionPolicyUpdate = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-internal-allow-document-reads-in-offline-region", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && AllowDocumentReadsInOfflineRegion != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				AllowDocumentReadsInOfflineRegion = value;
				return;
			}
			break;
		case 61:
			if (string.Equals("x-ms-cosmos-internal-serverless-offer-storage-refresh-request", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && IsServerlessStorageRefreshRequest != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsServerlessStorageRefreshRequest = value;
				return;
			}
			break;
		case 62:
			if (string.Equals("x-ms-cosmos-skip-adjust-throughput-fractions-for-offer-replace", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && SkipAdjustThroughputFractionsForOfferReplace != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SkipAdjustThroughputFractionsForOfferReplace = value;
				return;
			}
			break;
		}
		if (ignoreNotCommonHeaders)
		{
			return;
		}
		if (throwIfAlreadyExists)
		{
			InitializeNotCommonHeadersIfNeeded();
			notCommonHeaders.Add(key, value);
		}
		else if (value == null)
		{
			if (notCommonHeaders != null)
			{
				notCommonHeaders.Remove(key);
			}
		}
		else
		{
			InitializeNotCommonHeadersIfNeeded();
			notCommonHeaders[key] = value;
		}
	}

	private void InitializeNotCommonHeadersIfNeeded()
	{
		if (notCommonHeaders != null)
		{
			return;
		}
		lock (this)
		{
			if (notCommonHeaders == null)
			{
				notCommonHeaders = new Dictionary<string, string>(DefaultStringComparer);
			}
		}
	}
}
