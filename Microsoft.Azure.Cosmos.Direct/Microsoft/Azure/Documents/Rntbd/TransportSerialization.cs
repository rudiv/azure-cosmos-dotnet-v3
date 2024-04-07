using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Rntbd;
using Microsoft.Azure.Documents.Collections;

namespace Microsoft.Azure.Documents.Rntbd;

internal static class TransportSerialization
{
	internal class RntbdHeader
	{
		public StatusCodes Status { get; private set; }

		public Guid ActivityId { get; private set; }

		public RntbdHeader(StatusCodes status, Guid activityId)
		{
			Status = status;
			ActivityId = activityId;
		}
	}

	internal sealed class SerializedRequest : IDisposable
	{
		private readonly BufferProvider.DisposableBuffer requestHeader;

		private readonly CloneableStream requestBody;

		public int RequestSize => requestHeader.Buffer.Count + (int)(requestBody?.Length ?? 0);

		public SerializedRequest(BufferProvider.DisposableBuffer requestHeader, CloneableStream requestBody)
		{
			this.requestHeader = requestHeader;
			this.requestBody = requestBody;
		}

		public void Dispose()
		{
			requestHeader.Dispose();
			requestBody?.Dispose();
		}

		internal void CopyTo(ArraySegment<byte> buffer)
		{
			if (buffer.Count < RequestSize)
			{
				throw new ArgumentException("Buffer should at least be as big as the request size");
			}
			Array.Copy(requestHeader.Buffer.Array, requestHeader.Buffer.Offset, buffer.Array, buffer.Offset, requestHeader.Buffer.Count);
			if (requestBody != null)
			{
				requestBody.CopyBufferTo(buffer.Array, buffer.Offset + requestHeader.Buffer.Count);
			}
		}

		internal async Task CopyToStreamAsync(Stream stream)
		{
			await stream.WriteAsync(requestHeader.Buffer.Array, requestHeader.Buffer.Offset, requestHeader.Buffer.Count);
			if (requestBody != null)
			{
				requestBody.Position = 0L;
				await requestBody.CopyToAsync(stream);
			}
		}
	}

	internal static readonly char[] UrlTrim = new char[1] { '/' };

	internal static SerializedRequest BuildRequestForProxy(DocumentServiceRequest request, ResourceOperation resourceOperation, Guid activityId, BufferProvider bufferProvider, string globalDatabaseAccountName, out int headerSize, out int? bodySize)
	{
		if (string.IsNullOrEmpty(globalDatabaseAccountName))
		{
			throw new ArgumentNullException("globalDatabaseAccountName");
		}
		RntbdConstants.Request rntbdRequest = new RntbdConstants.Request();
		int num = Array.IndexOf(rntbdRequest.tokens, rntbdRequest.replicaPath);
		rntbdRequest.replicaPath = new RntbdToken(isRequired: false, rntbdRequest.replicaPath.GetTokenType(), rntbdRequest.replicaPath.GetTokenIdentifier());
		rntbdRequest.tokens[num] = rntbdRequest.replicaPath;
		num = Array.IndexOf(rntbdRequest.tokens, rntbdRequest.transportRequestID);
		rntbdRequest.transportRequestID = new RntbdToken(isRequired: false, rntbdRequest.transportRequestID.GetTokenType(), rntbdRequest.transportRequestID.GetTokenIdentifier());
		rntbdRequest.tokens[num] = rntbdRequest.transportRequestID;
		num = Array.IndexOf(rntbdRequest.tokens, rntbdRequest.effectivePartitionKey);
		rntbdRequest.tokens[num] = rntbdRequest.tokens[0];
		rntbdRequest.tokens[0] = rntbdRequest.effectivePartitionKey;
		num = Array.IndexOf(rntbdRequest.tokens, rntbdRequest.globalDatabaseAccountName);
		rntbdRequest.tokens[num] = rntbdRequest.tokens[1];
		rntbdRequest.tokens[1] = rntbdRequest.globalDatabaseAccountName;
		rntbdRequest.globalDatabaseAccountName.value.valueBytes = BytesSerializer.GetBytesForString(globalDatabaseAccountName, rntbdRequest);
		rntbdRequest.globalDatabaseAccountName.isPresent = true;
		return BuildRequestCore(request, ref rntbdRequest, "thinClientReplica", resourceOperation, activityId, bufferProvider, out headerSize, out bodySize);
	}

	internal static SerializedRequest BuildRequest(DocumentServiceRequest request, string replicaPath, ResourceOperation resourceOperation, Guid activityId, BufferProvider bufferProvider, string transportRequestIDOverride, out int headerSize, out int? bodySize)
	{
		using RntbdConstants.RntbdEntityPool<RntbdConstants.Request, RntbdConstants.RequestIdentifiers>.EntityOwner entityOwner = RntbdConstants.RntbdEntityPool<RntbdConstants.Request, RntbdConstants.RequestIdentifiers>.Instance.Get();
		RntbdConstants.Request rntbdRequest = entityOwner.Entity;
		rntbdRequest.replicaPath.value.valueBytes = BytesSerializer.GetBytesForString(replicaPath, rntbdRequest);
		rntbdRequest.replicaPath.isPresent = true;
		FillTokenFromHeader(request, "x-ms-transport-request-id", transportRequestIDOverride, rntbdRequest.transportRequestID, rntbdRequest);
		return BuildRequestCore(request, ref rntbdRequest, replicaPath, resourceOperation, activityId, bufferProvider, out headerSize, out bodySize);
	}

	private static SerializedRequest BuildRequestCore(DocumentServiceRequest request, ref RntbdConstants.Request rntbdRequest, string replicaPathForDiagnostics, ResourceOperation resourceOperation, Guid activityId, BufferProvider bufferProvider, out int headerSize, out int? bodySize)
	{
		RntbdConstants.RntbdOperationType rntbdOperationType = GetRntbdOperationType(resourceOperation.operationType);
		RntbdConstants.RntbdResourceType rntbdResourceType = GetRntbdResourceType(resourceOperation.resourceType);
		RequestNameValueCollection requestNameValueCollection = request.Headers as RequestNameValueCollection;
		if (requestNameValueCollection == null)
		{
			requestNameValueCollection = RequestNameValueCollection.BuildRequestNameValueCollectionWithKnownHeadersOnly(request.Headers);
		}
		AddResourceIdOrPathHeaders(request, rntbdRequest);
		AddDateHeader(requestNameValueCollection, rntbdRequest);
		AddContinuation(requestNameValueCollection, rntbdRequest);
		AddMatchHeader(requestNameValueCollection, rntbdOperationType, rntbdRequest);
		AddIfModifiedSinceHeader(requestNameValueCollection, rntbdRequest);
		AddA_IMHeader(requestNameValueCollection, rntbdRequest);
		AddIndexingDirectiveHeader(requestNameValueCollection, rntbdRequest);
		AddMigrateCollectionDirectiveHeader(requestNameValueCollection, rntbdRequest);
		AddConsistencyLevelHeader(requestNameValueCollection, rntbdRequest);
		AddIsFanout(requestNameValueCollection, rntbdRequest);
		AddEntityId(request, rntbdRequest);
		AddAllowScanOnQuery(requestNameValueCollection, rntbdRequest);
		AddEmitVerboseTracesInQuery(requestNameValueCollection, rntbdRequest);
		AddCanCharge(requestNameValueCollection, rntbdRequest);
		AddCanThrottle(requestNameValueCollection, rntbdRequest);
		AddProfileRequest(requestNameValueCollection, rntbdRequest);
		AddEnableLowPrecisionOrderBy(requestNameValueCollection, rntbdRequest);
		AddPageSize(requestNameValueCollection, rntbdRequest);
		AddSupportSpatialLegacyCoordinates(requestNameValueCollection, rntbdRequest);
		AddUsePolygonsSmallerThanAHemisphere(requestNameValueCollection, rntbdRequest);
		AddEnableLogging(requestNameValueCollection, rntbdRequest);
		AddPopulateQuotaInfo(requestNameValueCollection, rntbdRequest);
		AddPopulateResourceCount(requestNameValueCollection, rntbdRequest);
		AddDisableRUPerMinuteUsage(requestNameValueCollection, rntbdRequest);
		AddPopulateQueryMetrics(requestNameValueCollection, rntbdRequest);
		AddPopulateQueryMetricsIndexUtilization(requestNameValueCollection, rntbdRequest);
		AddPopulateIndexMetricsV2(requestNameValueCollection, rntbdRequest);
		AddOptimisticDirectExecute(requestNameValueCollection, rntbdRequest);
		AddQueryForceScan(requestNameValueCollection, rntbdRequest);
		AddResponseContinuationTokenLimitInKb(requestNameValueCollection, rntbdRequest);
		AddPopulatePartitionStatistics(requestNameValueCollection, rntbdRequest);
		AddRemoteStorageType(requestNameValueCollection, rntbdRequest);
		AddCollectionRemoteStorageSecurityIdentifier(requestNameValueCollection, rntbdRequest);
		AddCollectionChildResourceNameLimitInBytes(requestNameValueCollection, rntbdRequest);
		AddCollectionChildResourceContentLengthLimitInKB(requestNameValueCollection, rntbdRequest);
		AddUniqueIndexNameEncodingMode(requestNameValueCollection, rntbdRequest);
		AddUniqueIndexReIndexingState(requestNameValueCollection, rntbdRequest);
		AddCorrelatedActivityId(requestNameValueCollection, rntbdRequest);
		AddPopulateCollectionThroughputInfo(requestNameValueCollection, rntbdRequest);
		AddShareThroughput(requestNameValueCollection, rntbdRequest);
		AddIsReadOnlyScript(requestNameValueCollection, rntbdRequest);
		AddCanOfferReplaceComplete(requestNameValueCollection, rntbdRequest);
		AddIgnoreSystemLoweringMaxThroughput(requestNameValueCollection, rntbdRequest);
		AddExcludeSystemProperties(requestNameValueCollection, rntbdRequest);
		AddEnumerationDirection(request, requestNameValueCollection, rntbdRequest);
		AddFanoutOperationStateHeader(requestNameValueCollection, rntbdRequest);
		AddStartAndEndKeys(request, requestNameValueCollection, rntbdRequest);
		AddContentSerializationFormat(requestNameValueCollection, rntbdRequest);
		AddSupportedSerializationFormats(requestNameValueCollection, rntbdRequest);
		AddIsUserRequest(requestNameValueCollection, rntbdRequest);
		AddPreserveFullContent(requestNameValueCollection, rntbdRequest);
		AddIsRUPerGBEnforcementRequest(requestNameValueCollection, rntbdRequest);
		AddIsOfferStorageRefreshRequest(requestNameValueCollection, rntbdRequest);
		AddGetAllPartitionKeyStatistics(requestNameValueCollection, rntbdRequest);
		AddForceSideBySideIndexMigration(requestNameValueCollection, rntbdRequest);
		AddIsMigrateOfferToManualThroughputRequest(requestNameValueCollection, rntbdRequest);
		AddIsMigrateOfferToAutopilotRequest(requestNameValueCollection, rntbdRequest);
		AddSystemDocumentTypeHeader(requestNameValueCollection, rntbdRequest);
		AddTransactionMetaData(request, rntbdRequest);
		AddTransactionCompletionFlag(request, rntbdRequest);
		AddResourceTypes(requestNameValueCollection, rntbdRequest);
		AddUpdateMaxthroughputEverProvisioned(requestNameValueCollection, rntbdRequest);
		AddUseSystemBudget(requestNameValueCollection, rntbdRequest);
		AddTruncateMergeLogRequest(requestNameValueCollection, rntbdRequest);
		AddRetriableWriteRequestMetadata(request, rntbdRequest);
		AddRequestedCollectionType(requestNameValueCollection, rntbdRequest);
		AddIsThroughputCapRequest(requestNameValueCollection, rntbdRequest);
		AddUpdateOfferStateToPending(requestNameValueCollection, rntbdRequest);
		AddUpdateOfferStateToRestorePending(requestNameValueCollection, rntbdRequest);
		AddMasterResourcesDeletionPending(requestNameValueCollection, rntbdRequest);
		AddIsInternalServerlessRequest(requestNameValueCollection, rntbdRequest);
		AddOfferReplaceRURedistribution(requestNameValueCollection, rntbdRequest);
		AddIsMaterializedViewSourceSchemaReplaceBatchRequest(requestNameValueCollection, rntbdRequest);
		AddIsCassandraAlterTypeRequest(request, rntbdRequest);
		AddHighPriorityForcedBackup(requestNameValueCollection, rntbdRequest);
		AddEnableConflictResolutionPolicyUpdate(requestNameValueCollection, rntbdRequest);
		AddAllowDocumentReadsInOfflineRegion(requestNameValueCollection, rntbdRequest);
		FillTokenFromHeader(request, "authorization", requestNameValueCollection.Authorization, rntbdRequest.authorizationToken, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-session-token", requestNameValueCollection.SessionToken, rntbdRequest.sessionToken, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-documentdb-pre-trigger-include", requestNameValueCollection.PreTriggerInclude, rntbdRequest.preTriggerInclude, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-documentdb-pre-trigger-exclude", requestNameValueCollection.PreTriggerExclude, rntbdRequest.preTriggerExclude, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-documentdb-post-trigger-include", requestNameValueCollection.PostTriggerInclude, rntbdRequest.postTriggerInclude, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-documentdb-post-trigger-exclude", requestNameValueCollection.PostTriggerExclude, rntbdRequest.postTriggerExclude, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-documentdb-partitionkey", requestNameValueCollection.PartitionKey, rntbdRequest.partitionKey, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-documentdb-partitionkeyrangeid", requestNameValueCollection.PartitionKeyRangeId, rntbdRequest.partitionKeyRangeId, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-documentdb-expiry-seconds", requestNameValueCollection.ResourceTokenExpiry, rntbdRequest.resourceTokenExpiry, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-documentdb-filterby-schema-rid", requestNameValueCollection.FilterBySchemaResourceId, rntbdRequest.filterBySchemaRid, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-batch-continue-on-error", requestNameValueCollection.ShouldBatchContinueOnError, rntbdRequest.shouldBatchContinueOnError, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-batch-ordered", requestNameValueCollection.IsBatchOrdered, rntbdRequest.isBatchOrdered, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-batch-atomic", requestNameValueCollection.IsBatchAtomic, rntbdRequest.isBatchAtomic, rntbdRequest);
		FillTokenFromHeader(request, "collection-partition-index", requestNameValueCollection.CollectionPartitionIndex, rntbdRequest.collectionPartitionIndex, rntbdRequest);
		FillTokenFromHeader(request, "collection-service-index", requestNameValueCollection.CollectionServiceIndex, rntbdRequest.collectionServiceIndex, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-resource-schema-name", requestNameValueCollection.ResourceSchemaName, rntbdRequest.resourceSchemaName, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-bind-replica", requestNameValueCollection.BindReplicaDirective, rntbdRequest.bindReplicaDirective, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-primary-master-key", requestNameValueCollection.PrimaryMasterKey, rntbdRequest.primaryMasterKey, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-secondary-master-key", requestNameValueCollection.SecondaryMasterKey, rntbdRequest.secondaryMasterKey, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-primary-readonly-key", requestNameValueCollection.PrimaryReadonlyKey, rntbdRequest.primaryReadonlyKey, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-secondary-readonly-key", requestNameValueCollection.SecondaryReadonlyKey, rntbdRequest.secondaryReadonlyKey, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-documentdb-partitioncount", requestNameValueCollection.PartitionCount, rntbdRequest.partitionCount, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-documentdb-collection-rid", requestNameValueCollection.CollectionRid, rntbdRequest.collectionRid, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-gateway-signature", requestNameValueCollection.GatewaySignature, rntbdRequest.gatewaySignature, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-remaining-time-in-ms-on-client", requestNameValueCollection.RemainingTimeInMsOnClientRequest, rntbdRequest.remainingTimeInMsOnClientRequest, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-client-retry-attempt-count", requestNameValueCollection.ClientRetryAttemptCount, rntbdRequest.clientRetryAttemptCount, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-target-lsn", requestNameValueCollection.TargetLsn, rntbdRequest.targetLsn, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-target-global-committed-lsn", requestNameValueCollection.TargetGlobalCommittedLsn, rntbdRequest.targetGlobalCommittedLsn, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-restore-metadata-filter", requestNameValueCollection.RestoreMetadataFilter, rntbdRequest.restoreMetadataFilter, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-restore-params", requestNameValueCollection.RestoreParams, rntbdRequest.restoreParams, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-partition-resource-filter", requestNameValueCollection.PartitionResourceFilter, rntbdRequest.partitionResourceFilter, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-enable-dynamic-rid-range-allocation", requestNameValueCollection.EnableDynamicRidRangeAllocation, rntbdRequest.enableDynamicRidRangeAllocation, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-schema-owner-rid", requestNameValueCollection.SchemaOwnerRid, rntbdRequest.schemaOwnerRid, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-schema-hash", requestNameValueCollection.SchemaHash, rntbdRequest.schemaHash, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-schema-id", requestNameValueCollection.SchemaId, rntbdRequest.collectionSchemaId, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-is-client-encrypted", requestNameValueCollection.IsClientEncrypted, rntbdRequest.isClientEncrypted, rntbdRequest);
		AddReturnPreferenceIfPresent(requestNameValueCollection, rntbdRequest);
		AddBinaryIdIfPresent(request, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-time-to-live-in-seconds", requestNameValueCollection.TimeToLiveInSeconds, rntbdRequest.timeToLiveInSeconds, rntbdRequest);
		AddEffectivePartitionKeyIfPresent(request, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-binary-passthrough-request", requestNameValueCollection.BinaryPassthroughRequest, rntbdRequest.binaryPassthroughRequest, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-allow-tentative-writes", requestNameValueCollection.AllowTentativeWrites, rntbdRequest.allowTentativeWrites, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-include-tentative-writes", requestNameValueCollection.IncludeTentativeWrites, rntbdRequest.includeTentativeWrites, rntbdRequest);
		AddMergeStaticIdIfPresent(request, rntbdRequest);
		AddMergeStaticIdIfPresent(request, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-max-polling-interval", requestNameValueCollection.MaxPollingIntervalMilliseconds, rntbdRequest.maxPollingIntervalMilliseconds, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-populate-logstoreinfo", requestNameValueCollection.PopulateLogStoreInfo, rntbdRequest.populateLogStoreInfo, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-internal-merge-checkpoint-glsn", requestNameValueCollection.MergeCheckPointGLSN, rntbdRequest.mergeCheckpointGLSNKeyName, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-internal-populate-unflushed-merge-entry-count", requestNameValueCollection.PopulateUnflushedMergeEntryCount, rntbdRequest.populateUnflushedMergeEntryCount, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-add-resource-properties-to-response", requestNameValueCollection.AddResourcePropertiesToResponse, rntbdRequest.addResourcePropertiesToResponse, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-internal-system-restore-operation", requestNameValueCollection.SystemRestoreOperation, rntbdRequest.systemRestoreOperation, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-start-full-fidelity-if-none-match", requestNameValueCollection.ChangeFeedStartFullFidelityIfNoneMatch, rntbdRequest.changeFeedStartFullFidelityIfNoneMatch, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-skip-refresh-databaseaccountconfig", requestNameValueCollection.SkipRefreshDatabaseAccountConfigs, rntbdRequest.skipRefreshDatabaseAccountConfigs, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-intended-collection-rid", requestNameValueCollection.IntendedCollectionRid, rntbdRequest.intendedCollectionRid, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-use-archival-partition", requestNameValueCollection.UseArchivalPartition, rntbdRequest.useArchivalPartition, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-collection-truncate", requestNameValueCollection.CollectionTruncate, rntbdRequest.collectionTruncate, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-sdk-supportedcapabilities", requestNameValueCollection.SDKSupportedCapabilities, rntbdRequest.sDKSupportedCapabilities, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmosdb-populateuniqueindexreindexprogress", requestNameValueCollection.PopulateUniqueIndexReIndexProgress, rntbdRequest.populateUniqueIndexReIndexProgress, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-internal-is-materialized-view-build", requestNameValueCollection.IsMaterializedViewBuild, rntbdRequest.isMaterializedViewBuild, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-builder-client-identifier", requestNameValueCollection.BuilderClientIdentifier, rntbdRequest.builderClientIdentifier, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-source-collection-if-match", requestNameValueCollection.SourceCollectionIfMatch, rntbdRequest.sourceCollectionIfMatch, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-populate-analytical-migration-progress", requestNameValueCollection.PopulateAnalyticalMigrationProgress, rntbdRequest.populateAnalyticalMigrationProgress, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-should-return-current-server-datetime", requestNameValueCollection.ShouldReturnCurrentServerDateTime, rntbdRequest.shouldReturnCurrentServerDateTime, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-rbac-user-id", requestNameValueCollection.RbacUserId, rntbdRequest.rbacUserId, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-rbac-action", requestNameValueCollection.RbacAction, rntbdRequest.rbacAction, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-rbac-resource", requestNameValueCollection.RbacResource, rntbdRequest.rbacResource, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-changefeed-wire-format-version", requestNameValueCollection.ChangeFeedWireFormatVersion, rntbdRequest.changeFeedWireFormatVersion, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-populate-byok-encryption-progress", requestNameValueCollection.PopulateByokEncryptionProgress, rntbdRequest.populateBYOKEncryptionProgress, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-use-background-task-budget", requestNameValueCollection.UseUserBackgroundBudget, rntbdRequest.useUserBackgroundBudget, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-include-physical-partition-throughput-info", requestNameValueCollection.IncludePhysicalPartitionThroughputInfo, rntbdRequest.includePhysicalPartitionThroughputInfo, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-populate-oldest-active-schema-id", requestNameValueCollection.PopulateOldestActiveSchemaId, rntbdRequest.populateOldestActiveSchemaId, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-force-database-account-update", requestNameValueCollection.ForceDatabaseAccountUpdate, rntbdRequest.forceDatabaseAccountUpdate, rntbdRequest);
		AddPriorityLevelHeader(request, "x-ms-cosmos-priority-level", requestNameValueCollection.PriorityLevel, requestNameValueCollection, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-internal-allow-restore-params-update", requestNameValueCollection.AllowRestoreParamsUpdate, rntbdRequest.allowRestoreParamsUpdate, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-prune-collection-schemas", requestNameValueCollection.PruneCollectionSchemas, rntbdRequest.pruneCollectionSchemas, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-internal-migrated-fixed-collection", requestNameValueCollection.IsMigratedFixedCollection, rntbdRequest.isMigratedFixedCollection, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-internal-populate-min-glsn-for-relocation", requestNameValueCollection.PopulateMinGLSNForDocumentOperations, rntbdRequest.populateMinGLSNForDocumentOperations, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-internal-populate-highest-tentative-write-llsn", requestNameValueCollection.PopulateHighestTentativeWriteLLSN, rntbdRequest.populateHighestTentativeWriteLLSN, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-populate-capacity-type", requestNameValueCollection.PopulateCapacityType, rntbdRequest.populateCapacityType, rntbdRequest);
		FillTokenFromHeader(request, "traceparent", requestNameValueCollection.TraceParent, rntbdRequest.traceParent, rntbdRequest);
		FillTokenFromHeader(request, "tracestate", requestNameValueCollection.TraceState, rntbdRequest.traceState, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-client-ip-address", requestNameValueCollection.ClientIpAddress, rntbdRequest.clientIpAddress, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-is-request-not-authorized", requestNameValueCollection.IsRequestNotAuthorized, rntbdRequest.isRequestNotAuthorized, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-start-epk-hash", null, rntbdRequest.startEpkHash, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-end-epk-hash", null, rntbdRequest.endEpkHash, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-populate-current-partition-throughput-info", requestNameValueCollection.PopulateCurrentPartitionThroughputInfo, rntbdRequest.populateCurrentPartitionThroughputInfo, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-cosmos-internal-populate-document-record-count", requestNameValueCollection.PopulateDocumentRecordCount, rntbdRequest.populateDocumentRecordCount, rntbdRequest);
		FillTokenFromHeader(request, "x-ms-version", requestNameValueCollection.Version, rntbdRequest.clientVersion, rntbdRequest);
		int num;
		int num2 = (num = 8 + BytesSerializer.GetSizeOfGuid());
		int num3 = 0;
		bodySize = null;
		int num4 = 0;
		CloneableStream cloneableStream = null;
		if (request.CloneableBody != null)
		{
			cloneableStream = request.CloneableBody.Clone();
			num4 = (int)cloneableStream.Length;
		}
		if (num4 > 0)
		{
			num3 += 4;
			rntbdRequest.payloadPresent.value.valueByte = 1;
			rntbdRequest.payloadPresent.isPresent = true;
		}
		else
		{
			rntbdRequest.payloadPresent.value.valueByte = 0;
			rntbdRequest.payloadPresent.isPresent = true;
		}
		num += rntbdRequest.CalculateLength();
		num3 += num;
		BufferProvider.DisposableBuffer buffer = bufferProvider.GetBuffer(num3);
		BytesSerializer writer = new BytesSerializer(buffer.Buffer.Array, num3);
		writer.Write((uint)num);
		writer.Write((ushort)rntbdResourceType);
		writer.Write((ushort)rntbdOperationType);
		writer.Write(activityId);
		int num5 = num2;
		rntbdRequest.SerializeToBinaryWriter(ref writer, out var tokensLength);
		num5 += tokensLength;
		if (num5 != num)
		{
			cloneableStream?.Dispose();
			DefaultTrace.TraceCritical("Bug in RNTBD token serialization. Calculated header size: {0}. Actual header size: {1}", num, num5);
			throw new InternalServerErrorException();
		}
		if (num4 > 0)
		{
			writer.Write((uint)num4);
			bodySize = 4 + num4;
		}
		headerSize = num;
		if (headerSize > 131072)
		{
			DefaultTrace.TraceWarning("The request header is large. Header size: {0}. Warning threshold: {1}. RID: {2}. Resource type: {3}. Operation: {4}. Address: {5}", headerSize, 131072, request.ResourceAddress, request.ResourceType, resourceOperation, replicaPathForDiagnostics);
		}
		if (bodySize > 16777216)
		{
			DefaultTrace.TraceWarning("The request body is large. Body size: {0}. Warning threshold: {1}. RID: {2}. Resource type: {3}. Operation: {4}. Address: {5}", bodySize, 16777216, request.ResourceAddress, request.ResourceType, resourceOperation, replicaPathForDiagnostics);
		}
		return new SerializedRequest(buffer, cloneableStream);
	}

	internal static byte[] BuildContextRequest(Guid activityId, UserAgentContainer userAgent, RntbdConstants.CallerId callerId, bool enableChannelMultiplexing)
	{
		byte[] array = activityId.ToByteArray();
		RntbdConstants.ConnectionContextRequest connectionContextRequest = new RntbdConstants.ConnectionContextRequest();
		connectionContextRequest.protocolVersion.value.valueULong = 1u;
		connectionContextRequest.protocolVersion.isPresent = true;
		connectionContextRequest.clientVersion.value.valueBytes = HttpConstants.Versions.CurrentVersionUTF8;
		connectionContextRequest.clientVersion.isPresent = true;
		connectionContextRequest.userAgent.value.valueBytes = userAgent.UserAgentUTF8;
		connectionContextRequest.userAgent.isPresent = true;
		connectionContextRequest.callerId.isPresent = false;
		if (callerId != RntbdConstants.CallerId.Invalid)
		{
			connectionContextRequest.callerId.value.valueByte = (byte)callerId;
			connectionContextRequest.callerId.isPresent = true;
		}
		connectionContextRequest.enableChannelMultiplexing.isPresent = true;
		connectionContextRequest.enableChannelMultiplexing.value.valueByte = (enableChannelMultiplexing ? ((byte)1) : ((byte)0));
		int num = 8 + array.Length;
		num += connectionContextRequest.CalculateLength();
		byte[] array2 = new byte[num];
		BytesSerializer writer = new BytesSerializer(array2, num);
		writer.Write(num);
		writer.Write((ushort)0);
		writer.Write((ushort)0);
		writer.Write(array);
		connectionContextRequest.SerializeToBinaryWriter(ref writer, out var _);
		return array2;
	}

	internal static StoreResponse MakeStoreResponse(StatusCodes status, Guid activityId, Stream body, string serverVersion, ref BytesDeserializer rntbdHeaderReader)
	{
		return new StoreResponse
		{
			Headers = HeadersTransportSerialization.BuildStoreResponseNameValueCollection(activityId, serverVersion, ref rntbdHeaderReader),
			ResponseBody = body,
			Status = (int)status
		};
	}

	internal static RntbdHeader DecodeRntbdHeader(byte[] header)
	{
		uint status = BitConverter.ToUInt32(header, 4);
		Guid activityId = BytesSerializer.ReadGuidFromBytes(new ArraySegment<byte>(header, 8, 16));
		return new RntbdHeader((StatusCodes)status, activityId);
	}

	private static RntbdConstants.RntbdOperationType GetRntbdOperationType(OperationType operationType)
	{
		return operationType switch
		{
			OperationType.Create => RntbdConstants.RntbdOperationType.Create, 
			OperationType.Delete => RntbdConstants.RntbdOperationType.Delete, 
			OperationType.ExecuteJavaScript => RntbdConstants.RntbdOperationType.ExecuteJavaScript, 
			OperationType.Query => RntbdConstants.RntbdOperationType.Query, 
			OperationType.Read => RntbdConstants.RntbdOperationType.Read, 
			OperationType.ReadFeed => RntbdConstants.RntbdOperationType.ReadFeed, 
			OperationType.Replace => RntbdConstants.RntbdOperationType.Replace, 
			OperationType.SqlQuery => RntbdConstants.RntbdOperationType.SQLQuery, 
			OperationType.Patch => RntbdConstants.RntbdOperationType.Patch, 
			OperationType.Head => RntbdConstants.RntbdOperationType.Head, 
			OperationType.HeadFeed => RntbdConstants.RntbdOperationType.HeadFeed, 
			OperationType.Upsert => RntbdConstants.RntbdOperationType.Upsert, 
			OperationType.BatchApply => RntbdConstants.RntbdOperationType.BatchApply, 
			OperationType.Batch => RntbdConstants.RntbdOperationType.Batch, 
			OperationType.CompleteUserTransaction => RntbdConstants.RntbdOperationType.CompleteUserTransaction, 
			OperationType.MetadataCheckAccess => RntbdConstants.RntbdOperationType.MetadataCheckAccess, 
			OperationType.AddComputeGatewayRequestCharges => RntbdConstants.RntbdOperationType.AddComputeGatewayRequestCharges, 
			_ => throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid operation type: {0}", operationType), "operationType"), 
		};
	}

	private static RntbdConstants.RntbdResourceType GetRntbdResourceType(ResourceType resourceType)
	{
		return resourceType switch
		{
			ResourceType.Attachment => RntbdConstants.RntbdResourceType.Attachment, 
			ResourceType.Collection => RntbdConstants.RntbdResourceType.Collection, 
			ResourceType.Conflict => RntbdConstants.RntbdResourceType.Conflict, 
			ResourceType.Database => RntbdConstants.RntbdResourceType.Database, 
			ResourceType.Document => RntbdConstants.RntbdResourceType.Document, 
			ResourceType.Record => RntbdConstants.RntbdResourceType.Record, 
			ResourceType.Permission => RntbdConstants.RntbdResourceType.Permission, 
			ResourceType.StoredProcedure => RntbdConstants.RntbdResourceType.StoredProcedure, 
			ResourceType.Trigger => RntbdConstants.RntbdResourceType.Trigger, 
			ResourceType.User => RntbdConstants.RntbdResourceType.User, 
			ResourceType.ClientEncryptionKey => RntbdConstants.RntbdResourceType.ClientEncryptionKey, 
			ResourceType.UserDefinedType => RntbdConstants.RntbdResourceType.UserDefinedType, 
			ResourceType.UserDefinedFunction => RntbdConstants.RntbdResourceType.UserDefinedFunction, 
			ResourceType.Offer => RntbdConstants.RntbdResourceType.Offer, 
			ResourceType.DatabaseAccount => RntbdConstants.RntbdResourceType.DatabaseAccount, 
			ResourceType.PartitionKeyRange => RntbdConstants.RntbdResourceType.PartitionKeyRange, 
			ResourceType.Schema => RntbdConstants.RntbdResourceType.Schema, 
			ResourceType.BatchApply => RntbdConstants.RntbdResourceType.BatchApply, 
			ResourceType.ComputeGatewayCharges => RntbdConstants.RntbdResourceType.ComputeGatewayCharges, 
			ResourceType.PartitionKey => RntbdConstants.RntbdResourceType.PartitionKey, 
			ResourceType.PartitionedSystemDocument => RntbdConstants.RntbdResourceType.PartitionedSystemDocument, 
			ResourceType.SystemDocument => RntbdConstants.RntbdResourceType.SystemDocument, 
			ResourceType.RoleDefinition => RntbdConstants.RntbdResourceType.RoleDefinition, 
			ResourceType.RoleAssignment => RntbdConstants.RntbdResourceType.RoleAssignment, 
			ResourceType.Transaction => RntbdConstants.RntbdResourceType.Transaction, 
			ResourceType.InteropUser => RntbdConstants.RntbdResourceType.InteropUser, 
			ResourceType.AuthPolicyElement => RntbdConstants.RntbdResourceType.AuthPolicyElement, 
			ResourceType.RetriableWriteCachedResponse => RntbdConstants.RntbdResourceType.RetriableWriteCachedResponse, 
			ResourceType.EncryptionScope => RntbdConstants.RntbdResourceType.EncryptionScope, 
			_ => throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid resource type: {0}", resourceType), "resourceType"), 
		};
	}

	private static void AddMatchHeader(RequestNameValueCollection requestHeaders, RntbdConstants.RntbdOperationType operationType, RntbdConstants.Request rntbdRequest)
	{
		string text = ((operationType - 3 > RntbdConstants.RntbdOperationType.Create) ? requestHeaders.IfMatch : requestHeaders.IfNoneMatch);
		if (!string.IsNullOrEmpty(text))
		{
			rntbdRequest.match.value.valueBytes = BytesSerializer.GetBytesForString(text, rntbdRequest);
			rntbdRequest.match.isPresent = true;
		}
	}

	private static void AddIfModifiedSinceHeader(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		string ifModifiedSince = requestHeaders.IfModifiedSince;
		if (!string.IsNullOrEmpty(ifModifiedSince))
		{
			rntbdRequest.ifModifiedSince.value.valueBytes = BytesSerializer.GetBytesForString(ifModifiedSince, rntbdRequest);
			rntbdRequest.ifModifiedSince.isPresent = true;
		}
	}

	private static void AddA_IMHeader(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		string a_IM = requestHeaders.A_IM;
		if (!string.IsNullOrEmpty(a_IM))
		{
			rntbdRequest.a_IM.value.valueBytes = BytesSerializer.GetBytesForString(a_IM, rntbdRequest);
			rntbdRequest.a_IM.isPresent = true;
		}
	}

	private static void AddDateHeader(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		string dateHeader = Helpers.GetDateHeader(requestHeaders);
		if (!string.IsNullOrEmpty(dateHeader))
		{
			rntbdRequest.date.value.valueBytes = BytesSerializer.GetBytesForString(dateHeader, rntbdRequest);
			rntbdRequest.date.isPresent = true;
		}
	}

	private static void AddContinuation(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.Continuation))
		{
			rntbdRequest.continuationToken.value.valueBytes = BytesSerializer.GetBytesForString(requestHeaders.Continuation, rntbdRequest);
			rntbdRequest.continuationToken.isPresent = true;
		}
	}

	private static void AddResourceIdOrPathHeaders(DocumentServiceRequest request, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(request.ResourceId))
		{
			rntbdRequest.resourceId.value.valueBytes = ResourceId.Parse(request.ResourceType, request.ResourceId);
			rntbdRequest.resourceId.isPresent = true;
		}
		if (request.IsNameBased)
		{
			if (request.ResourceType == ResourceType.Document && request.IsResourceNameParsedFromUri)
			{
				SetResourceIdHeadersFromDocumentServiceRequest(request, rntbdRequest);
			}
			else
			{
				SetResourceIdHeadersFromUri(request, rntbdRequest);
			}
		}
	}

	private static void SetResourceIdHeadersFromUri(DocumentServiceRequest request, RntbdConstants.Request rntbdRequest)
	{
		string[] array = request.ResourceAddress.Split(UrlTrim, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length >= 2)
		{
			switch (array[0])
			{
			case "dbs":
				rntbdRequest.databaseName.value.valueBytes = BytesSerializer.GetBytesForString(array[1], rntbdRequest);
				rntbdRequest.databaseName.isPresent = true;
				break;
			case "snapshots":
				rntbdRequest.snapshotName.value.valueBytes = BytesSerializer.GetBytesForString(array[1], rntbdRequest);
				rntbdRequest.snapshotName.isPresent = true;
				break;
			case "roledefinitions":
				rntbdRequest.roleDefinitionName.value.valueBytes = BytesSerializer.GetBytesForString(array[1], rntbdRequest);
				rntbdRequest.roleDefinitionName.isPresent = true;
				break;
			case "roleassignments":
				rntbdRequest.roleAssignmentName.value.valueBytes = BytesSerializer.GetBytesForString(array[1], rntbdRequest);
				rntbdRequest.roleAssignmentName.isPresent = true;
				break;
			case "interopusers":
				rntbdRequest.interopUserName.value.valueBytes = BytesSerializer.GetBytesForString(array[1], rntbdRequest);
				rntbdRequest.interopUserName.isPresent = true;
				break;
			case "authpolicyelements":
				rntbdRequest.authPolicyElementName.value.valueBytes = BytesSerializer.GetBytesForString(array[1], rntbdRequest);
				rntbdRequest.authPolicyElementName.isPresent = true;
				break;
			case "encryptionscopes":
				rntbdRequest.encryptionScopeName.value.valueBytes = BytesSerializer.GetBytesForString(array[1], rntbdRequest);
				rntbdRequest.encryptionScopeName.isPresent = true;
				break;
			default:
				throw new BadRequestException();
			}
		}
		if (array.Length >= 4)
		{
			switch (array[2])
			{
			case "colls":
				rntbdRequest.collectionName.value.valueBytes = BytesSerializer.GetBytesForString(array[3], rntbdRequest);
				rntbdRequest.collectionName.isPresent = true;
				break;
			case "clientencryptionkeys":
				rntbdRequest.clientEncryptionKeyName.value.valueBytes = BytesSerializer.GetBytesForString(array[3], rntbdRequest);
				rntbdRequest.clientEncryptionKeyName.isPresent = true;
				break;
			case "users":
				rntbdRequest.userName.value.valueBytes = BytesSerializer.GetBytesForString(array[3], rntbdRequest);
				rntbdRequest.userName.isPresent = true;
				break;
			case "udts":
				rntbdRequest.userDefinedTypeName.value.valueBytes = BytesSerializer.GetBytesForString(array[3], rntbdRequest);
				rntbdRequest.userDefinedTypeName.isPresent = true;
				break;
			}
		}
		if (array.Length >= 6)
		{
			switch (array[4])
			{
			case "docs":
				rntbdRequest.documentName.value.valueBytes = BytesSerializer.GetBytesForString(array[5], rntbdRequest);
				rntbdRequest.documentName.isPresent = true;
				break;
			case "sprocs":
				rntbdRequest.storedProcedureName.value.valueBytes = BytesSerializer.GetBytesForString(array[5], rntbdRequest);
				rntbdRequest.storedProcedureName.isPresent = true;
				break;
			case "permissions":
				rntbdRequest.permissionName.value.valueBytes = BytesSerializer.GetBytesForString(array[5], rntbdRequest);
				rntbdRequest.permissionName.isPresent = true;
				break;
			case "udfs":
				rntbdRequest.userDefinedFunctionName.value.valueBytes = BytesSerializer.GetBytesForString(array[5], rntbdRequest);
				rntbdRequest.userDefinedFunctionName.isPresent = true;
				break;
			case "triggers":
				rntbdRequest.triggerName.value.valueBytes = BytesSerializer.GetBytesForString(array[5], rntbdRequest);
				rntbdRequest.triggerName.isPresent = true;
				break;
			case "conflicts":
				rntbdRequest.conflictName.value.valueBytes = BytesSerializer.GetBytesForString(array[5], rntbdRequest);
				rntbdRequest.conflictName.isPresent = true;
				break;
			case "pkranges":
				rntbdRequest.partitionKeyRangeName.value.valueBytes = BytesSerializer.GetBytesForString(array[5], rntbdRequest);
				rntbdRequest.partitionKeyRangeName.isPresent = true;
				break;
			case "schemas":
				rntbdRequest.schemaName.value.valueBytes = BytesSerializer.GetBytesForString(array[5], rntbdRequest);
				rntbdRequest.schemaName.isPresent = true;
				break;
			case "partitionedsystemdocuments":
				rntbdRequest.systemDocumentName.value.valueBytes = BytesSerializer.GetBytesForString(array[5], rntbdRequest);
				rntbdRequest.systemDocumentName.isPresent = true;
				break;
			case "systemdocuments":
				rntbdRequest.systemDocumentName.value.valueBytes = BytesSerializer.GetBytesForString(array[5], rntbdRequest);
				rntbdRequest.systemDocumentName.isPresent = true;
				break;
			}
		}
		if (array.Length >= 8 && array[6] == "attachments")
		{
			rntbdRequest.attachmentName.value.valueBytes = BytesSerializer.GetBytesForString(array[7], rntbdRequest);
			rntbdRequest.attachmentName.isPresent = true;
		}
	}

	private static void SetResourceIdHeadersFromDocumentServiceRequest(DocumentServiceRequest request, RntbdConstants.Request rntbdRequest)
	{
		if (string.IsNullOrEmpty(request.DatabaseName))
		{
			throw new ArgumentException("DatabaseName");
		}
		rntbdRequest.databaseName.value.valueBytes = BytesSerializer.GetBytesForString(request.DatabaseName, rntbdRequest);
		rntbdRequest.databaseName.isPresent = true;
		if (string.IsNullOrEmpty(request.CollectionName))
		{
			throw new ArgumentException("CollectionName");
		}
		rntbdRequest.collectionName.value.valueBytes = BytesSerializer.GetBytesForString(request.CollectionName, rntbdRequest);
		rntbdRequest.collectionName.isPresent = true;
		if (!string.IsNullOrEmpty(request.DocumentName))
		{
			rntbdRequest.documentName.value.valueBytes = BytesSerializer.GetBytesForString(request.DocumentName, rntbdRequest);
			rntbdRequest.documentName.isPresent = true;
		}
	}

	private static void AddBinaryIdIfPresent(DocumentServiceRequest request, RntbdConstants.Request rntbdRequest)
	{
		string headerValue;
		if (request.Properties != null && request.Properties.TryGetValue("x-ms-binary-id", out var value))
		{
			if (value is byte[] array)
			{
				rntbdRequest.binaryId.value.valueBytes = array;
			}
			else
			{
				if (!(value is ReadOnlyMemory<byte> valueBytes))
				{
					throw new ArgumentOutOfRangeException("x-ms-binary-id");
				}
				rntbdRequest.binaryId.value.valueBytes = valueBytes;
			}
			rntbdRequest.binaryId.isPresent = true;
		}
		else if (TryGetHeaderValueString(request, "x-ms-binary-id", out headerValue))
		{
			rntbdRequest.binaryId.value.valueBytes = Convert.FromBase64String(headerValue);
			rntbdRequest.binaryId.isPresent = true;
		}
	}

	private static void AddReturnPreferenceIfPresent(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		string prefer = requestHeaders.Prefer;
		if (!string.IsNullOrEmpty(prefer))
		{
			if (string.Equals(prefer, "return=minimal", StringComparison.OrdinalIgnoreCase))
			{
				rntbdRequest.returnPreference.value.valueByte = 1;
				rntbdRequest.returnPreference.isPresent = true;
			}
			else if (string.Equals(prefer, "return=representation", StringComparison.OrdinalIgnoreCase))
			{
				rntbdRequest.returnPreference.value.valueByte = 0;
				rntbdRequest.returnPreference.isPresent = true;
			}
		}
	}

	private static void AddEffectivePartitionKeyIfPresent(DocumentServiceRequest request, RntbdConstants.Request rntbdRequest)
	{
		if (request.Properties != null && request.Properties.TryGetValue("x-ms-effective-partition-key", out var value))
		{
			if (!(value is byte[] array))
			{
				throw new ArgumentOutOfRangeException("x-ms-effective-partition-key");
			}
			rntbdRequest.effectivePartitionKey.value.valueBytes = array;
			rntbdRequest.effectivePartitionKey.isPresent = true;
		}
	}

	private static bool TryGetHeaderValueString(DocumentServiceRequest request, string headerName, out string headerValue)
	{
		headerValue = null;
		if (request.Headers != null)
		{
			headerValue = request.Headers.Get(headerName);
		}
		return !string.IsNullOrWhiteSpace(headerValue);
	}

	private static void AddMergeStaticIdIfPresent(DocumentServiceRequest request, RntbdConstants.Request rntbdRequest)
	{
		if (request.Properties == null || !request.Properties.TryGetValue("x-ms-cosmos-merge-static-id", out var value))
		{
			return;
		}
		if (value is byte[] array)
		{
			rntbdRequest.mergeStaticId.value.valueBytes = array;
		}
		else
		{
			if (!(value is ReadOnlyMemory<byte> valueBytes))
			{
				throw new ArgumentOutOfRangeException("x-ms-cosmos-merge-static-id");
			}
			rntbdRequest.mergeStaticId.value.valueBytes = valueBytes;
		}
		rntbdRequest.mergeStaticId.isPresent = true;
	}

	private static void AddEntityId(DocumentServiceRequest request, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(request.EntityId))
		{
			rntbdRequest.entityId.value.valueBytes = BytesSerializer.GetBytesForString(request.EntityId, rntbdRequest);
			rntbdRequest.entityId.isPresent = true;
		}
	}

	private static void AddIndexingDirectiveHeader(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.IndexingDirective))
		{
			RntbdConstants.RntbdIndexingDirective rntbdIndexingDirective = RntbdConstants.RntbdIndexingDirective.Invalid;
			if (!Enum.TryParse<IndexingDirective>(requestHeaders.IndexingDirective, ignoreCase: true, out var result))
			{
				throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, requestHeaders.IndexingDirective, typeof(IndexingDirective).Name));
			}
			rntbdIndexingDirective = result switch
			{
				IndexingDirective.Default => RntbdConstants.RntbdIndexingDirective.Default, 
				IndexingDirective.Exclude => RntbdConstants.RntbdIndexingDirective.Exclude, 
				IndexingDirective.Include => RntbdConstants.RntbdIndexingDirective.Include, 
				_ => throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, requestHeaders.IndexingDirective, typeof(IndexingDirective).Name)), 
			};
			rntbdRequest.indexingDirective.value.valueByte = (byte)rntbdIndexingDirective;
			rntbdRequest.indexingDirective.isPresent = true;
		}
	}

	private static void AddMigrateCollectionDirectiveHeader(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.MigrateCollectionDirective))
		{
			RntbdConstants.RntbdMigrateCollectionDirective rntbdMigrateCollectionDirective = RntbdConstants.RntbdMigrateCollectionDirective.Invalid;
			if (!Enum.TryParse<MigrateCollectionDirective>(requestHeaders.MigrateCollectionDirective, ignoreCase: true, out var result))
			{
				throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, requestHeaders.MigrateCollectionDirective, typeof(MigrateCollectionDirective).Name));
			}
			rntbdMigrateCollectionDirective = result switch
			{
				MigrateCollectionDirective.Freeze => RntbdConstants.RntbdMigrateCollectionDirective.Freeze, 
				MigrateCollectionDirective.Thaw => RntbdConstants.RntbdMigrateCollectionDirective.Thaw, 
				_ => throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, requestHeaders.MigrateCollectionDirective, typeof(MigrateCollectionDirective).Name)), 
			};
			rntbdRequest.migrateCollectionDirective.value.valueByte = (byte)rntbdMigrateCollectionDirective;
			rntbdRequest.migrateCollectionDirective.isPresent = true;
		}
	}

	private static void AddConsistencyLevelHeader(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.ConsistencyLevel))
		{
			RntbdConstants.RntbdConsistencyLevel rntbdConsistencyLevel = RntbdConstants.RntbdConsistencyLevel.Invalid;
			if (!Enum.TryParse<ConsistencyLevel>(requestHeaders.ConsistencyLevel, ignoreCase: true, out var result))
			{
				throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, requestHeaders.ConsistencyLevel, typeof(ConsistencyLevel).Name));
			}
			rntbdConsistencyLevel = result switch
			{
				ConsistencyLevel.Strong => RntbdConstants.RntbdConsistencyLevel.Strong, 
				ConsistencyLevel.BoundedStaleness => RntbdConstants.RntbdConsistencyLevel.BoundedStaleness, 
				ConsistencyLevel.Session => RntbdConstants.RntbdConsistencyLevel.Session, 
				ConsistencyLevel.Eventual => RntbdConstants.RntbdConsistencyLevel.Eventual, 
				ConsistencyLevel.ConsistentPrefix => RntbdConstants.RntbdConsistencyLevel.ConsistentPrefix, 
				_ => throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, requestHeaders.ConsistencyLevel, typeof(ConsistencyLevel).Name)), 
			};
			rntbdRequest.consistencyLevel.value.valueByte = (byte)rntbdConsistencyLevel;
			rntbdRequest.consistencyLevel.isPresent = true;
		}
	}

	private static void AddIsThroughputCapRequest(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.IsThroughputCapRequest))
		{
			rntbdRequest.isThroughputCapRequest.value.valueByte = (requestHeaders.IsThroughputCapRequest.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.isThroughputCapRequest.isPresent = true;
		}
	}

	private static void AddIsFanout(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.IsFanoutRequest))
		{
			rntbdRequest.isFanout.value.valueByte = (requestHeaders.IsFanoutRequest.Equals(bool.TrueString) ? ((byte)1) : ((byte)0));
			rntbdRequest.isFanout.isPresent = true;
		}
	}

	private static void AddAllowScanOnQuery(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.EnableScanInQuery))
		{
			rntbdRequest.enableScanInQuery.value.valueByte = (requestHeaders.EnableScanInQuery.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.enableScanInQuery.isPresent = true;
		}
	}

	private static void AddEnableLowPrecisionOrderBy(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.EnableLowPrecisionOrderBy))
		{
			rntbdRequest.enableLowPrecisionOrderBy.value.valueByte = (requestHeaders.EnableLowPrecisionOrderBy.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.enableLowPrecisionOrderBy.isPresent = true;
		}
	}

	private static void AddEmitVerboseTracesInQuery(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.EmitVerboseTracesInQuery))
		{
			rntbdRequest.emitVerboseTracesInQuery.value.valueByte = (requestHeaders.EmitVerboseTracesInQuery.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.emitVerboseTracesInQuery.isPresent = true;
		}
	}

	private static void AddCanCharge(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.CanCharge))
		{
			rntbdRequest.canCharge.value.valueByte = (requestHeaders.CanCharge.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.canCharge.isPresent = true;
		}
	}

	private static void AddCanThrottle(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.CanThrottle))
		{
			rntbdRequest.canThrottle.value.valueByte = (requestHeaders.CanThrottle.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.canThrottle.isPresent = true;
		}
	}

	private static void AddProfileRequest(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.ProfileRequest))
		{
			rntbdRequest.profileRequest.value.valueByte = (requestHeaders.ProfileRequest.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.profileRequest.isPresent = true;
		}
	}

	private static void AddPageSize(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		string pageSize = requestHeaders.PageSize;
		if (string.IsNullOrEmpty(pageSize))
		{
			return;
		}
		if (!int.TryParse(pageSize, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
		{
			throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidPageSize, pageSize));
		}
		if (result == -1)
		{
			rntbdRequest.pageSize.value.valueULong = uint.MaxValue;
		}
		else
		{
			if (result < 0)
			{
				throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidPageSize, pageSize));
			}
			rntbdRequest.pageSize.value.valueULong = (uint)result;
		}
		rntbdRequest.pageSize.isPresent = true;
	}

	private static void AddEnableLogging(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.EnableLogging))
		{
			rntbdRequest.enableLogging.value.valueByte = (requestHeaders.EnableLogging.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.enableLogging.isPresent = true;
		}
	}

	private static void AddSupportSpatialLegacyCoordinates(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.SupportSpatialLegacyCoordinates))
		{
			rntbdRequest.supportSpatialLegacyCoordinates.value.valueByte = (requestHeaders.SupportSpatialLegacyCoordinates.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.supportSpatialLegacyCoordinates.isPresent = true;
		}
	}

	private static void AddUsePolygonsSmallerThanAHemisphere(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.UsePolygonsSmallerThanAHemisphere))
		{
			rntbdRequest.usePolygonsSmallerThanAHemisphere.value.valueByte = (requestHeaders.UsePolygonsSmallerThanAHemisphere.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.usePolygonsSmallerThanAHemisphere.isPresent = true;
		}
	}

	private static void AddPopulateQuotaInfo(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.PopulateQuotaInfo))
		{
			rntbdRequest.populateQuotaInfo.value.valueByte = (requestHeaders.PopulateQuotaInfo.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.populateQuotaInfo.isPresent = true;
		}
	}

	private static void AddPopulateResourceCount(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.PopulateResourceCount))
		{
			rntbdRequest.populateResourceCount.value.valueByte = (requestHeaders.PopulateResourceCount.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.populateResourceCount.isPresent = true;
		}
	}

	private static void AddPopulatePartitionStatistics(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.PopulatePartitionStatistics))
		{
			rntbdRequest.populatePartitionStatistics.value.valueByte = (requestHeaders.PopulatePartitionStatistics.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.populatePartitionStatistics.isPresent = true;
		}
	}

	private static void AddDisableRUPerMinuteUsage(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.DisableRUPerMinuteUsage))
		{
			rntbdRequest.disableRUPerMinuteUsage.value.valueByte = (requestHeaders.DisableRUPerMinuteUsage.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.disableRUPerMinuteUsage.isPresent = true;
		}
	}

	private static void AddPopulateQueryMetrics(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.PopulateQueryMetrics))
		{
			rntbdRequest.populateQueryMetrics.value.valueByte = (requestHeaders.PopulateQueryMetrics.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.populateQueryMetrics.isPresent = true;
		}
	}

	private static void AddPopulateQueryMetricsIndexUtilization(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.PopulateIndexMetrics))
		{
			rntbdRequest.populateIndexMetrics.value.valueByte = (requestHeaders.PopulateIndexMetrics.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.populateIndexMetrics.isPresent = true;
		}
	}

	private static void AddPopulateIndexMetricsV2(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.PopulateIndexMetricsV2))
		{
			rntbdRequest.populateIndexMetricsV2.value.valueByte = (requestHeaders.PopulateIndexMetricsV2.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.populateIndexMetricsV2.isPresent = true;
		}
	}

	private static void AddOptimisticDirectExecute(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.OptimisticDirectExecute))
		{
			rntbdRequest.optimisticDirectExecute.value.valueByte = (requestHeaders.OptimisticDirectExecute.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.optimisticDirectExecute.isPresent = true;
		}
	}

	private static void AddQueryForceScan(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.ForceQueryScan))
		{
			rntbdRequest.forceQueryScan.value.valueByte = (requestHeaders.ForceQueryScan.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.forceQueryScan.isPresent = true;
		}
	}

	private static void AddPopulateCollectionThroughputInfo(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.PopulateCollectionThroughputInfo))
		{
			rntbdRequest.populateCollectionThroughputInfo.value.valueByte = (requestHeaders.PopulateCollectionThroughputInfo.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.populateCollectionThroughputInfo.isPresent = true;
		}
	}

	private static void AddShareThroughput(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.ShareThroughput))
		{
			rntbdRequest.shareThroughput.value.valueByte = (requestHeaders.ShareThroughput.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.shareThroughput.isPresent = true;
		}
	}

	private static void AddIsReadOnlyScript(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.IsReadOnlyScript))
		{
			rntbdRequest.isReadOnlyScript.value.valueByte = (requestHeaders.IsReadOnlyScript.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.isReadOnlyScript.isPresent = true;
		}
	}

	private static void AddCanOfferReplaceComplete(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.CanOfferReplaceComplete))
		{
			rntbdRequest.canOfferReplaceComplete.value.valueByte = (requestHeaders.CanOfferReplaceComplete.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.canOfferReplaceComplete.isPresent = true;
		}
	}

	private static void AddIgnoreSystemLoweringMaxThroughput(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.IgnoreSystemLoweringMaxThroughput))
		{
			rntbdRequest.ignoreSystemLoweringMaxThroughput.value.valueByte = (requestHeaders.IgnoreSystemLoweringMaxThroughput.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.ignoreSystemLoweringMaxThroughput.isPresent = true;
		}
	}

	private static void AddUpdateMaxthroughputEverProvisioned(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.UpdateMaxThroughputEverProvisioned))
		{
			string updateMaxThroughputEverProvisioned = requestHeaders.UpdateMaxThroughputEverProvisioned;
			if (!int.TryParse(updateMaxThroughputEverProvisioned, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
			{
				throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidUpdateMaxthroughputEverProvisioned, updateMaxThroughputEverProvisioned));
			}
			if (result < 0)
			{
				throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidUpdateMaxthroughputEverProvisioned, updateMaxThroughputEverProvisioned));
			}
			rntbdRequest.updateMaxThroughputEverProvisioned.value.valueULong = (uint)result;
			rntbdRequest.updateMaxThroughputEverProvisioned.isPresent = true;
		}
	}

	private static void AddGetAllPartitionKeyStatistics(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.GetAllPartitionKeyStatistics))
		{
			rntbdRequest.getAllPartitionKeyStatistics.value.valueByte = (requestHeaders.GetAllPartitionKeyStatistics.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.getAllPartitionKeyStatistics.isPresent = true;
		}
	}

	private static void AddResponseContinuationTokenLimitInKb(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.ResponseContinuationTokenLimitInKB))
		{
			string responseContinuationTokenLimitInKB = requestHeaders.ResponseContinuationTokenLimitInKB;
			if (!int.TryParse(responseContinuationTokenLimitInKB, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
			{
				throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidPageSize, responseContinuationTokenLimitInKB));
			}
			if (result < 0)
			{
				throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidResponseContinuationTokenLimit, responseContinuationTokenLimitInKB));
			}
			rntbdRequest.responseContinuationTokenLimitInKb.value.valueULong = (uint)result;
			rntbdRequest.responseContinuationTokenLimitInKb.isPresent = true;
		}
	}

	private static void AddRemoteStorageType(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.RemoteStorageType))
		{
			RntbdConstants.RntbdRemoteStorageType rntbdRemoteStorageType = RntbdConstants.RntbdRemoteStorageType.Invalid;
			if (!Enum.TryParse<RemoteStorageType>(requestHeaders.RemoteStorageType, ignoreCase: true, out var result))
			{
				throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, requestHeaders.RemoteStorageType, typeof(RemoteStorageType).Name));
			}
			rntbdRemoteStorageType = result switch
			{
				RemoteStorageType.Standard => RntbdConstants.RntbdRemoteStorageType.Standard, 
				RemoteStorageType.Premium => RntbdConstants.RntbdRemoteStorageType.Premium, 
				_ => throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, requestHeaders.RemoteStorageType, typeof(RemoteStorageType).Name)), 
			};
			rntbdRequest.remoteStorageType.value.valueByte = (byte)rntbdRemoteStorageType;
			rntbdRequest.remoteStorageType.isPresent = true;
		}
	}

	private static void AddCollectionChildResourceNameLimitInBytes(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		string collectionChildResourceNameLimitInBytes = requestHeaders.CollectionChildResourceNameLimitInBytes;
		if (!string.IsNullOrEmpty(collectionChildResourceNameLimitInBytes))
		{
			if (!int.TryParse(collectionChildResourceNameLimitInBytes, NumberStyles.Integer, CultureInfo.InvariantCulture, out rntbdRequest.collectionChildResourceNameLimitInBytes.value.valueLong))
			{
				throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, collectionChildResourceNameLimitInBytes, "x-ms-cosmos-collection-child-resourcename-limit"));
			}
			rntbdRequest.collectionChildResourceNameLimitInBytes.isPresent = true;
		}
	}

	private static void AddCollectionChildResourceContentLengthLimitInKB(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		string collectionChildResourceContentLimitInKB = requestHeaders.CollectionChildResourceContentLimitInKB;
		if (!string.IsNullOrEmpty(collectionChildResourceContentLimitInKB))
		{
			if (!int.TryParse(collectionChildResourceContentLimitInKB, NumberStyles.Integer, CultureInfo.InvariantCulture, out rntbdRequest.collectionChildResourceContentLengthLimitInKB.value.valueLong))
			{
				throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, collectionChildResourceContentLimitInKB, "x-ms-cosmos-collection-child-contentlength-resourcelimit"));
			}
			rntbdRequest.collectionChildResourceContentLengthLimitInKB.isPresent = true;
		}
	}

	private static void AddUniqueIndexNameEncodingMode(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		string uniqueIndexNameEncodingMode = requestHeaders.UniqueIndexNameEncodingMode;
		if (!string.IsNullOrEmpty(uniqueIndexNameEncodingMode))
		{
			if (!byte.TryParse(uniqueIndexNameEncodingMode, NumberStyles.Integer, CultureInfo.InvariantCulture, out rntbdRequest.uniqueIndexNameEncodingMode.value.valueByte))
			{
				throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, uniqueIndexNameEncodingMode, "x-ms-cosmos-unique-index-name-encoding-mode"));
			}
			rntbdRequest.uniqueIndexNameEncodingMode.isPresent = true;
		}
	}

	private static void AddUniqueIndexReIndexingState(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		string uniqueIndexReIndexingState = requestHeaders.UniqueIndexReIndexingState;
		if (!string.IsNullOrEmpty(uniqueIndexReIndexingState))
		{
			if (!byte.TryParse(uniqueIndexReIndexingState, NumberStyles.Integer, CultureInfo.InvariantCulture, out rntbdRequest.uniqueIndexReIndexingState.value.valueByte))
			{
				throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, uniqueIndexReIndexingState, "x-ms-cosmos-uniqueindex-reindexing-state"));
			}
			rntbdRequest.uniqueIndexReIndexingState.isPresent = true;
		}
	}

	private static void AddIsInternalServerlessRequest(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.IsInternalServerlessRequest))
		{
			rntbdRequest.isInternalServerlessRequest.value.valueByte = (requestHeaders.IsInternalServerlessRequest.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.isInternalServerlessRequest.isPresent = true;
		}
	}

	private static void AddCorrelatedActivityId(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		string correlatedActivityId = requestHeaders.CorrelatedActivityId;
		if (!string.IsNullOrEmpty(correlatedActivityId))
		{
			if (!Guid.TryParse(correlatedActivityId, out rntbdRequest.correlatedActivityId.value.valueGuid))
			{
				throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, correlatedActivityId, "x-ms-cosmos-correlated-activityid"));
			}
			rntbdRequest.correlatedActivityId.isPresent = true;
		}
	}

	private static void AddCollectionRemoteStorageSecurityIdentifier(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		string collectionRemoteStorageSecurityIdentifier = requestHeaders.CollectionRemoteStorageSecurityIdentifier;
		if (!string.IsNullOrEmpty(collectionRemoteStorageSecurityIdentifier))
		{
			rntbdRequest.collectionRemoteStorageSecurityIdentifier.value.valueBytes = BytesSerializer.GetBytesForString(collectionRemoteStorageSecurityIdentifier, rntbdRequest);
			rntbdRequest.collectionRemoteStorageSecurityIdentifier.isPresent = true;
		}
	}

	private static void AddIsUserRequest(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.IsUserRequest))
		{
			rntbdRequest.isUserRequest.value.valueByte = (requestHeaders.IsUserRequest.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.isUserRequest.isPresent = true;
		}
	}

	private static void AddPreserveFullContent(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.PreserveFullContent))
		{
			rntbdRequest.preserveFullContent.value.valueByte = (requestHeaders.PreserveFullContent.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.preserveFullContent.isPresent = true;
		}
	}

	private static void AddForceSideBySideIndexMigration(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.ForceSideBySideIndexMigration))
		{
			rntbdRequest.forceSideBySideIndexMigration.value.valueByte = (requestHeaders.ForceSideBySideIndexMigration.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.forceSideBySideIndexMigration.isPresent = true;
		}
	}

	private static void AddPopulateUniqueIndexReIndexProgress(object headerObjectValue, RntbdConstants.Request rntbdRequest)
	{
		if (headerObjectValue is string text && !string.IsNullOrEmpty(text))
		{
			if (string.Equals(bool.TrueString, text, StringComparison.OrdinalIgnoreCase))
			{
				rntbdRequest.populateUniqueIndexReIndexProgress.value.valueByte = 1;
			}
			else
			{
				rntbdRequest.populateUniqueIndexReIndexProgress.value.valueByte = 0;
			}
			rntbdRequest.populateUniqueIndexReIndexProgress.isPresent = true;
		}
	}

	private static void AddIsRUPerGBEnforcementRequest(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.IsRUPerGBEnforcementRequest))
		{
			rntbdRequest.isRUPerGBEnforcementRequest.value.valueByte = (requestHeaders.IsRUPerGBEnforcementRequest.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.isRUPerGBEnforcementRequest.isPresent = true;
		}
	}

	private static void AddIsOfferStorageRefreshRequest(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.IsOfferStorageRefreshRequest))
		{
			rntbdRequest.isofferStorageRefreshRequest.value.valueByte = (requestHeaders.IsOfferStorageRefreshRequest.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.isofferStorageRefreshRequest.isPresent = true;
		}
	}

	private static void AddIsMigrateOfferToManualThroughputRequest(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.MigrateOfferToManualThroughput))
		{
			rntbdRequest.migrateOfferToManualThroughput.value.valueByte = (requestHeaders.MigrateOfferToManualThroughput.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.migrateOfferToManualThroughput.isPresent = true;
		}
	}

	private static void AddIsMigrateOfferToAutopilotRequest(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.MigrateOfferToAutopilot))
		{
			rntbdRequest.migrateOfferToAutopilot.value.valueByte = (requestHeaders.MigrateOfferToAutopilot.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.migrateOfferToAutopilot.isPresent = true;
		}
	}

	private static void AddTruncateMergeLogRequest(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.TruncateMergeLogRequest))
		{
			rntbdRequest.truncateMergeLogRequest.value.valueByte = (requestHeaders.TruncateMergeLogRequest.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.truncateMergeLogRequest.isPresent = true;
		}
	}

	private static void AddEnumerationDirection(DocumentServiceRequest request, RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (request.Properties != null && request.Properties.TryGetValue("x-ms-enumeration-direction", out var value))
		{
			byte? b = value as byte?;
			if (!b.HasValue)
			{
				throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, "x-ms-enumeration-direction", "EnumerationDirection"));
			}
			rntbdRequest.enumerationDirection.value.valueByte = b.Value;
			rntbdRequest.enumerationDirection.isPresent = true;
		}
		else if (!string.IsNullOrEmpty(requestHeaders.EnumerationDirection))
		{
			RntbdConstants.RntdbEnumerationDirection rntdbEnumerationDirection = RntbdConstants.RntdbEnumerationDirection.Invalid;
			if (!Enum.TryParse<EnumerationDirection>(requestHeaders.EnumerationDirection, ignoreCase: true, out var result))
			{
				throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, requestHeaders.EnumerationDirection, "EnumerationDirection"));
			}
			rntdbEnumerationDirection = result switch
			{
				EnumerationDirection.Forward => RntbdConstants.RntdbEnumerationDirection.Forward, 
				EnumerationDirection.Reverse => RntbdConstants.RntdbEnumerationDirection.Reverse, 
				_ => throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, requestHeaders.EnumerationDirection, typeof(EnumerationDirection).Name)), 
			};
			rntbdRequest.enumerationDirection.value.valueByte = (byte)rntdbEnumerationDirection;
			rntbdRequest.enumerationDirection.isPresent = true;
		}
	}

	private static void AddStartAndEndKeys(DocumentServiceRequest request, RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (request.Properties == null || !string.IsNullOrEmpty(requestHeaders.ReadFeedKeyType))
		{
			AddStartAndEndKeysFromHeaders(requestHeaders, rntbdRequest);
			return;
		}
		RntbdConstants.RntdbReadFeedKeyType? rntdbReadFeedKeyType = null;
		if (request.Properties.TryGetValue("x-ms-read-key-type", out var value))
		{
			if (!(value is byte))
			{
				throw new ArgumentOutOfRangeException("x-ms-read-key-type");
			}
			rntbdRequest.readFeedKeyType.value.valueByte = (byte)value;
			rntbdRequest.readFeedKeyType.isPresent = true;
			rntdbReadFeedKeyType = (RntbdConstants.RntdbReadFeedKeyType)value;
		}
		if (rntdbReadFeedKeyType == RntbdConstants.RntdbReadFeedKeyType.ResourceId)
		{
			SetBytesValue(request, "x-ms-start-id", rntbdRequest.startId);
			SetBytesValue(request, "x-ms-end-id", rntbdRequest.endId);
		}
		else if (rntdbReadFeedKeyType == RntbdConstants.RntdbReadFeedKeyType.EffectivePartitionKey || rntdbReadFeedKeyType == RntbdConstants.RntdbReadFeedKeyType.EffectivePartitionKeyRange)
		{
			SetBytesValue(request, "x-ms-start-epk", rntbdRequest.startEpk);
			SetBytesValue(request, "x-ms-end-epk", rntbdRequest.endEpk);
		}
	}

	private static void AddStartAndEndKeysFromHeaders(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		bool flag = false;
		if (!string.IsNullOrEmpty(requestHeaders.ReadFeedKeyType))
		{
			RntbdConstants.RntdbReadFeedKeyType rntdbReadFeedKeyType = RntbdConstants.RntdbReadFeedKeyType.Invalid;
			if (!Enum.TryParse<ReadFeedKeyType>(requestHeaders.ReadFeedKeyType, ignoreCase: true, out var result))
			{
				throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, requestHeaders.ReadFeedKeyType, "ReadFeedKeyType"));
			}
			switch (result)
			{
			case ReadFeedKeyType.ResourceId:
				rntdbReadFeedKeyType = RntbdConstants.RntdbReadFeedKeyType.ResourceId;
				break;
			case ReadFeedKeyType.EffectivePartitionKey:
				rntdbReadFeedKeyType = RntbdConstants.RntdbReadFeedKeyType.EffectivePartitionKey;
				break;
			case ReadFeedKeyType.EffectivePartitionKeyRange:
				rntdbReadFeedKeyType = RntbdConstants.RntdbReadFeedKeyType.EffectivePartitionKeyRange;
				flag = true;
				break;
			default:
				throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, requestHeaders.ReadFeedKeyType, typeof(ReadFeedKeyType).Name));
			}
			rntbdRequest.readFeedKeyType.value.valueByte = (byte)rntdbReadFeedKeyType;
			rntbdRequest.readFeedKeyType.isPresent = true;
		}
		string startId = requestHeaders.StartId;
		if (!string.IsNullOrEmpty(startId))
		{
			rntbdRequest.startId.value.valueBytes = Convert.FromBase64String(startId);
			rntbdRequest.startId.isPresent = true;
		}
		string endId = requestHeaders.EndId;
		if (!string.IsNullOrEmpty(endId))
		{
			rntbdRequest.endId.value.valueBytes = Convert.FromBase64String(endId);
			rntbdRequest.endId.isPresent = true;
		}
		string startEpk = requestHeaders.StartEpk;
		if (!string.IsNullOrEmpty(startEpk))
		{
			rntbdRequest.startEpk.value.valueBytes = (flag ? BytesSerializer.GetBytesForString(startEpk, rntbdRequest) : ((ReadOnlyMemory<byte>)Convert.FromBase64String(startEpk)));
			rntbdRequest.startEpk.isPresent = true;
		}
		string endEpk = requestHeaders.EndEpk;
		if (!string.IsNullOrEmpty(endEpk))
		{
			rntbdRequest.endEpk.value.valueBytes = (flag ? BytesSerializer.GetBytesForString(endEpk, rntbdRequest) : ((ReadOnlyMemory<byte>)Convert.FromBase64String(endEpk)));
			rntbdRequest.endEpk.isPresent = true;
		}
	}

	private static void SetBytesValue(DocumentServiceRequest request, string headerName, RntbdToken token)
	{
		if (!request.Properties.TryGetValue(headerName, out var value))
		{
			return;
		}
		if (value is byte[] array)
		{
			token.value.valueBytes = array;
		}
		else
		{
			if (!(value is ReadOnlyMemory<byte> valueBytes))
			{
				throw new ArgumentOutOfRangeException(headerName);
			}
			token.value.valueBytes = valueBytes;
		}
		token.isPresent = true;
	}

	private static void AddContentSerializationFormat(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.ContentSerializationFormat))
		{
			RntbdConstants.RntbdContentSerializationFormat rntbdContentSerializationFormat = RntbdConstants.RntbdContentSerializationFormat.Invalid;
			if (!Enum.TryParse<ContentSerializationFormat>(requestHeaders.ContentSerializationFormat, ignoreCase: true, out var result))
			{
				throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, requestHeaders.ContentSerializationFormat, "ContentSerializationFormat"));
			}
			rntbdContentSerializationFormat = result switch
			{
				ContentSerializationFormat.JsonText => RntbdConstants.RntbdContentSerializationFormat.JsonText, 
				ContentSerializationFormat.CosmosBinary => RntbdConstants.RntbdContentSerializationFormat.CosmosBinary, 
				ContentSerializationFormat.HybridRow => RntbdConstants.RntbdContentSerializationFormat.HybridRow, 
				_ => throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, requestHeaders.ContentSerializationFormat, "ContentSerializationFormat")), 
			};
			rntbdRequest.contentSerializationFormat.value.valueByte = (byte)rntbdContentSerializationFormat;
			rntbdRequest.contentSerializationFormat.isPresent = true;
		}
	}

	[SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "Roslyn Baseline 12/12/2022 16:40")]
	private static void AddSupportedSerializationFormats(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (requestHeaders.SupportedSerializationFormats != null)
		{
			RntbdConstants.RntbdSupportedSerializationFormats rntbdSupportedSerializationFormats = RntbdConstants.RntbdSupportedSerializationFormats.None;
			if (requestHeaders.SupportedSerializationFormats.Length == 0 || !Enum.TryParse<SupportedSerializationFormats>(requestHeaders.SupportedSerializationFormats, ignoreCase: true, out var result))
			{
				throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, requestHeaders.SupportedSerializationFormats, "SupportedSerializationFormats"));
			}
			if (result.HasFlag(SupportedSerializationFormats.JsonText))
			{
				rntbdSupportedSerializationFormats |= RntbdConstants.RntbdSupportedSerializationFormats.JsonText;
			}
			if (result.HasFlag(SupportedSerializationFormats.CosmosBinary))
			{
				rntbdSupportedSerializationFormats |= RntbdConstants.RntbdSupportedSerializationFormats.CosmosBinary;
			}
			if (result.HasFlag(SupportedSerializationFormats.HybridRow))
			{
				rntbdSupportedSerializationFormats |= RntbdConstants.RntbdSupportedSerializationFormats.HybridRow;
			}
			if (((uint)result & 0xFFFFFFF8u) != 0)
			{
				throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, requestHeaders.SupportedSerializationFormats, "SupportedSerializationFormats"));
			}
			rntbdRequest.supportedSerializationFormats.value.valueByte = (byte)rntbdSupportedSerializationFormats;
			rntbdRequest.supportedSerializationFormats.isPresent = true;
		}
	}

	private static void FillTokenFromHeader(DocumentServiceRequest request, string headerName, string headerStringValue, RntbdToken token, RntbdConstants.Request rntbdRequest)
	{
		object value = null;
		if (string.IsNullOrEmpty(headerStringValue))
		{
			if (request.Properties == null || !request.Properties.TryGetValue(headerName, out value) || value == null)
			{
				return;
			}
			if (value is string text)
			{
				headerStringValue = text;
				if (string.IsNullOrEmpty(headerStringValue))
				{
					return;
				}
			}
		}
		switch (token.GetTokenType())
		{
		case RntbdTokenTypes.SmallString:
		case RntbdTokenTypes.String:
		case RntbdTokenTypes.ULongString:
			if (headerStringValue == null)
			{
				throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, headerStringValue, headerName));
			}
			token.value.valueBytes = BytesSerializer.GetBytesForString(headerStringValue, rntbdRequest);
			break;
		case RntbdTokenTypes.ULong:
		{
			uint result5;
			if (headerStringValue != null)
			{
				if (!uint.TryParse(headerStringValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out result5))
				{
					throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, headerStringValue, headerName));
				}
			}
			else
			{
				if (!(value is uint num4))
				{
					throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, value, headerName));
				}
				result5 = num4;
			}
			token.value.valueULong = result5;
			break;
		}
		case RntbdTokenTypes.Long:
		{
			int result4;
			if (headerStringValue != null)
			{
				if (!int.TryParse(headerStringValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out result4))
				{
					throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, headerStringValue, headerName));
				}
			}
			else
			{
				if (!(value is int num3))
				{
					throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, value, headerName));
				}
				result4 = num3;
			}
			token.value.valueLong = result4;
			break;
		}
		case RntbdTokenTypes.Double:
		{
			double result2;
			if (headerStringValue != null)
			{
				if (!double.TryParse(headerStringValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out result2))
				{
					throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, headerStringValue, headerName));
				}
			}
			else
			{
				if (!(value is double num))
				{
					throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, value, headerName));
				}
				result2 = num;
			}
			token.value.valueDouble = result2;
			break;
		}
		case RntbdTokenTypes.LongLong:
		{
			long result3;
			if (headerStringValue != null)
			{
				if (!long.TryParse(headerStringValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out result3))
				{
					throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, headerStringValue, headerName));
				}
			}
			else
			{
				if (!(value is long num2))
				{
					throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, value, headerName));
				}
				result3 = num2;
			}
			token.value.valueLongLong = result3;
			break;
		}
		case RntbdTokenTypes.Byte:
		{
			bool flag;
			if (headerStringValue != null)
			{
				flag = string.Equals(headerStringValue, bool.TrueString, StringComparison.OrdinalIgnoreCase);
			}
			else
			{
				if (!(value is bool flag2))
				{
					throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, value, headerName));
				}
				flag = flag2;
			}
			token.value.valueByte = (flag ? ((byte)1) : ((byte)0));
			break;
		}
		case RntbdTokenTypes.Bytes:
		{
			if (headerStringValue != null)
			{
				throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, headerStringValue, headerName));
			}
			byte[] array = (value as byte[]) ?? throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, value, headerName));
			token.value.valueBytes = array;
			break;
		}
		case RntbdTokenTypes.Guid:
		{
			Guid result;
			if (headerStringValue != null)
			{
				if (!Guid.TryParse(headerStringValue, out result))
				{
					throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, headerStringValue, headerName));
				}
			}
			else
			{
				if (!(value is Guid guid))
				{
					throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, value, headerName));
				}
				result = guid;
			}
			token.value.valueGuid = result;
			break;
		}
		default:
			throw new BadRequestException();
		}
		token.isPresent = true;
	}

	private static void AddExcludeSystemProperties(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.ExcludeSystemProperties))
		{
			rntbdRequest.excludeSystemProperties.value.valueByte = (requestHeaders.ExcludeSystemProperties.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.excludeSystemProperties.isPresent = true;
		}
	}

	private static void AddFanoutOperationStateHeader(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		string fanoutOperationState = requestHeaders.FanoutOperationState;
		if (!string.IsNullOrEmpty(fanoutOperationState))
		{
			if (!Enum.TryParse<FanoutOperationState>(fanoutOperationState, ignoreCase: true, out var result))
			{
				throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, fanoutOperationState, "FanoutOperationState"));
			}
			RntbdConstants.RntbdFanoutOperationState valueByte = result switch
			{
				FanoutOperationState.Started => RntbdConstants.RntbdFanoutOperationState.Started, 
				FanoutOperationState.Completed => RntbdConstants.RntbdFanoutOperationState.Completed, 
				_ => throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, fanoutOperationState, "FanoutOperationState")), 
			};
			rntbdRequest.fanoutOperationState.value.valueByte = (byte)valueByte;
			rntbdRequest.fanoutOperationState.isPresent = true;
		}
	}

	private static void AddResourceTypes(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		string resourceTypes = requestHeaders.ResourceTypes;
		if (!string.IsNullOrEmpty(resourceTypes))
		{
			rntbdRequest.resourceTypes.value.valueBytes = BytesSerializer.GetBytesForString(resourceTypes, rntbdRequest);
			rntbdRequest.resourceTypes.isPresent = true;
		}
	}

	private static void AddSystemDocumentTypeHeader(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.SystemDocumentType))
		{
			RntbdConstants.RntbdSystemDocumentType rntbdSystemDocumentType = RntbdConstants.RntbdSystemDocumentType.Invalid;
			if (!Enum.TryParse<SystemDocumentType>(requestHeaders.SystemDocumentType, ignoreCase: true, out var result))
			{
				throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, requestHeaders.SystemDocumentType, "SystemDocumentType"));
			}
			rntbdSystemDocumentType = result switch
			{
				SystemDocumentType.MaterializedViewLeaseDocument => RntbdConstants.RntbdSystemDocumentType.MaterializedViewLeaseDocument, 
				SystemDocumentType.MaterializedViewBuilderOwnershipDocument => RntbdConstants.RntbdSystemDocumentType.MaterializedViewBuilderOwnershipDocument, 
				SystemDocumentType.MaterializedViewLeaseStoreInitDocument => RntbdConstants.RntbdSystemDocumentType.MaterializedViewLeaseStoreInitDocument, 
				_ => throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, requestHeaders.SystemDocumentType, typeof(SystemDocumentType).Name)), 
			};
			rntbdRequest.systemDocumentType.value.valueByte = (byte)rntbdSystemDocumentType;
			rntbdRequest.systemDocumentType.isPresent = true;
		}
	}

	private static void AddTransactionMetaData(DocumentServiceRequest request, RntbdConstants.Request rntbdRequest)
	{
		if (request.Properties != null && request.Properties.TryGetValue("x-ms-cosmos-tx-id", out var value) && request.Properties.TryGetValue("x-ms-cosmos-tx-init", out var value2))
		{
			if (!(value is byte[] array))
			{
				throw new ArgumentOutOfRangeException("x-ms-cosmos-tx-id");
			}
			bool? flag = value2 as bool?;
			if (!flag.HasValue)
			{
				throw new ArgumentOutOfRangeException("x-ms-cosmos-tx-init");
			}
			rntbdRequest.transactionId.value.valueBytes = array;
			rntbdRequest.transactionId.isPresent = true;
			rntbdRequest.transactionFirstRequest.value.valueByte = (flag.Value ? ((byte)1) : ((byte)0));
			rntbdRequest.transactionFirstRequest.isPresent = true;
		}
	}

	private static void AddTransactionCompletionFlag(DocumentServiceRequest request, RntbdConstants.Request rntbdRequest)
	{
		if (request.Properties != null && request.Properties.TryGetValue("x-ms-cosmos-tx-commit", out var value))
		{
			bool? flag = value as bool?;
			if (!flag.HasValue)
			{
				throw new ArgumentOutOfRangeException("x-ms-cosmos-tx-commit");
			}
			rntbdRequest.transactionCommit.value.valueByte = (flag.Value ? ((byte)1) : ((byte)0));
			rntbdRequest.transactionCommit.isPresent = true;
		}
	}

	private static void AddRetriableWriteRequestMetadata(DocumentServiceRequest request, RntbdConstants.Request rntbdRequest)
	{
		if (request.Properties == null || !request.Properties.TryGetValue("x-ms-cosmos-retriable-write-request-id", out var value))
		{
			return;
		}
		if (!(value is byte[] array))
		{
			throw new ArgumentOutOfRangeException("x-ms-cosmos-retriable-write-request-id");
		}
		rntbdRequest.retriableWriteRequestId.value.valueBytes = array;
		rntbdRequest.retriableWriteRequestId.isPresent = true;
		if (request.Properties.TryGetValue("x-ms-cosmos-is-retried-write-request", out var value2))
		{
			bool? flag = value2 as bool?;
			if (!flag.HasValue)
			{
				throw new ArgumentOutOfRangeException("x-ms-cosmos-is-retried-write-request");
			}
			rntbdRequest.isRetriedWriteRequest.value.valueByte = (flag.Value ? ((byte)1) : ((byte)0));
			rntbdRequest.isRetriedWriteRequest.isPresent = true;
		}
		if (request.Properties.TryGetValue("x-ms-cosmos-retriable-write-request-start-timestamp", out var value3))
		{
			if (!ulong.TryParse(value3.ToString(), out var result) || result == 0)
			{
				throw new ArgumentOutOfRangeException("x-ms-cosmos-retriable-write-request-start-timestamp");
			}
			rntbdRequest.retriableWriteRequestStartTimestamp.value.valueULongLong = result;
			rntbdRequest.retriableWriteRequestStartTimestamp.isPresent = true;
		}
	}

	private static void AddUseSystemBudget(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.UseSystemBudget))
		{
			rntbdRequest.useSystemBudget.value.valueByte = (requestHeaders.UseSystemBudget.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.useSystemBudget.isPresent = true;
		}
	}

	private static void AddRequestedCollectionType(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		string requestedCollectionType = requestHeaders.RequestedCollectionType;
		if (!string.IsNullOrEmpty(requestedCollectionType))
		{
			if (!Enum.TryParse<RequestedCollectionType>(requestedCollectionType, ignoreCase: true, out var result))
			{
				throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, requestedCollectionType, "RequestedCollectionType"));
			}
			RntbdConstants.RntbdRequestedCollectionType valueByte = result switch
			{
				RequestedCollectionType.All => RntbdConstants.RntbdRequestedCollectionType.All, 
				RequestedCollectionType.Standard => RntbdConstants.RntbdRequestedCollectionType.Standard, 
				RequestedCollectionType.MaterializedView => RntbdConstants.RntbdRequestedCollectionType.MaterializedView, 
				_ => throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, requestedCollectionType, "RequestedCollectionType")), 
			};
			rntbdRequest.requestedCollectionType.value.valueByte = (byte)valueByte;
			rntbdRequest.requestedCollectionType.isPresent = true;
		}
	}

	private static void AddUpdateOfferStateToPending(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.UpdateOfferStateToPending))
		{
			rntbdRequest.updateOfferStateToPending.value.valueByte = (requestHeaders.UpdateOfferStateToPending.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.updateOfferStateToPending.isPresent = true;
		}
	}

	private static void AddUpdateOfferStateToRestorePending(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.UpdateOfferStateToRestorePending))
		{
			rntbdRequest.updateOfferStateToRestorePending.value.valueByte = (requestHeaders.UpdateOfferStateToRestorePending.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.updateOfferStateToRestorePending.isPresent = true;
		}
	}

	private static void AddMasterResourcesDeletionPending(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.SetMasterResourcesDeletionPending))
		{
			rntbdRequest.setMasterResourcesDeletionPending.value.valueByte = (requestHeaders.SetMasterResourcesDeletionPending.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.setMasterResourcesDeletionPending.isPresent = true;
		}
	}

	private static void AddOfferReplaceRURedistribution(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.OfferReplaceRURedistribution))
		{
			rntbdRequest.offerReplaceRURedistribution.value.valueByte = (requestHeaders.OfferReplaceRURedistribution.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.offerReplaceRURedistribution.isPresent = true;
		}
	}

	private static void AddIsMaterializedViewSourceSchemaReplaceBatchRequest(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.IsMaterializedViewSourceSchemaReplaceBatchRequest))
		{
			rntbdRequest.isMaterializedViewSourceSchemaReplaceBatchRequest.value.valueByte = (requestHeaders.IsMaterializedViewSourceSchemaReplaceBatchRequest.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.isMaterializedViewSourceSchemaReplaceBatchRequest.isPresent = true;
		}
	}

	private static void AddIsCassandraAlterTypeRequest(DocumentServiceRequest request, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(request.Headers["x-ms-cosmos-alter-type-request"]))
		{
			rntbdRequest.isCassandraAlterTypeRequest.value.valueByte = (request.Headers["x-ms-cosmos-alter-type-request"].Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.isCassandraAlterTypeRequest.isPresent = true;
		}
	}

	private static void AddHighPriorityForcedBackup(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.HighPriorityForcedBackup))
		{
			rntbdRequest.highPriorityForcedBackup.value.valueByte = (requestHeaders.HighPriorityForcedBackup.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.highPriorityForcedBackup.isPresent = true;
		}
	}

	private static void AddEnableConflictResolutionPolicyUpdate(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.EnableConflictResolutionPolicyUpdate))
		{
			rntbdRequest.enableConflictResolutionPolicyUpdate.value.valueByte = (requestHeaders.EnableConflictResolutionPolicyUpdate.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.enableConflictResolutionPolicyUpdate.isPresent = true;
		}
	}

	private static void AddPriorityLevelHeader(DocumentServiceRequest request, string headerName, string headerStringValue, RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		PriorityLevel result = PriorityLevel.High;
		RntbdConstants.RntbdPriorityLevel rntbdPriorityLevel = RntbdConstants.RntbdPriorityLevel.High;
		if (string.IsNullOrEmpty(headerStringValue))
		{
			object value = null;
			if (request.Properties == null || !request.Properties.TryGetValue(headerName, out value) || value == null)
			{
				return;
			}
			if (value is Enum @enum)
			{
				result = (PriorityLevel)(object)@enum;
			}
		}
		else if (!Enum.TryParse<PriorityLevel>(requestHeaders.PriorityLevel, ignoreCase: true, out result))
		{
			throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, requestHeaders.PriorityLevel, typeof(PriorityLevel).Name));
		}
		rntbdPriorityLevel = result switch
		{
			PriorityLevel.Low => RntbdConstants.RntbdPriorityLevel.Low, 
			PriorityLevel.High => RntbdConstants.RntbdPriorityLevel.High, 
			_ => throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, requestHeaders.PriorityLevel, typeof(PriorityLevel).Name)), 
		};
		rntbdRequest.priorityLevel.value.valueByte = (byte)rntbdPriorityLevel;
		rntbdRequest.priorityLevel.isPresent = true;
	}

	private static void AddAllowDocumentReadsInOfflineRegion(RequestNameValueCollection requestHeaders, RntbdConstants.Request rntbdRequest)
	{
		if (!string.IsNullOrEmpty(requestHeaders.AllowDocumentReadsInOfflineRegion))
		{
			rntbdRequest.allowDocumentReadsInOfflineRegion.value.valueByte = (requestHeaders.AllowDocumentReadsInOfflineRegion.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? ((byte)1) : ((byte)0));
			rntbdRequest.allowDocumentReadsInOfflineRegion.isPresent = true;
		}
	}
}
