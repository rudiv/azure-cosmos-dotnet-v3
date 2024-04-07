namespace Microsoft.Azure.Documents;

internal sealed class WFConstants
{
	public static class WireNames
	{
		public const string NamedEndpoint = "App=";
	}

	public static class BackendHeaders
	{
		public const string ResourceId = "x-docdb-resource-id";

		public const string OwnerId = "x-docdb-owner-id";

		public const string EntityId = "x-docdb-entity-id";

		public const string DatabaseEntityMaxCount = "x-ms-database-entity-max-count";

		public const string DatabaseEntityCurrentCount = "x-ms-database-entity-current-count";

		public const string CollectionEntityMaxCount = "x-ms-collection-entity-max-count";

		public const string CollectionEntityCurrentCount = "x-ms-collection-entity-current-count";

		public const string UserEntityMaxCount = "x-ms-user-entity-max-count";

		public const string UserEntityCurrentCount = "x-ms-user-entity-current-count";

		public const string PermissionEntityMaxCount = "x-ms-permission-entity-max-count";

		public const string PermissionEntityCurrentCount = "x-ms-permission-entity-current-count";

		public const string RootEntityMaxCount = "x-ms-root-entity-max-count";

		public const string RootEntityCurrentCount = "x-ms-root-entity-current-count";

		public const string ResourceSchemaName = "x-ms-resource-schema-name";

		public const string LSN = "lsn";

		public const string QuorumAckedLSN = "x-ms-quorum-acked-lsn";

		public const string QuorumAckedLLSN = "x-ms-cosmos-quorum-acked-llsn";

		public const string CurrentWriteQuorum = "x-ms-current-write-quorum";

		public const string CurrentReplicaSetSize = "x-ms-current-replica-set-size";

		public const string CollectionPartitionIndex = "collection-partition-index";

		public const string CollectionServiceIndex = "collection-service-index";

		public const string Status = "Status";

		public const string ActivityId = "ActivityId";

		public const string IsFanoutRequest = "x-ms-is-fanout-request";

		public const string PrimaryMasterKey = "x-ms-primary-master-key";

		public const string SecondaryMasterKey = "x-ms-secondary-master-key";

		public const string PrimaryReadonlyKey = "x-ms-primary-readonly-key";

		public const string SecondaryReadonlyKey = "x-ms-secondary-readonly-key";

		public const string BindReplicaDirective = "x-ms-bind-replica";

		public const string DatabaseAccountId = "x-ms-database-account-id";

		public const string RequestValidationFailure = "x-ms-request-validation-failure";

		public const string SubStatus = "x-ms-substatus";

		public const string PartitionKeyRangeId = "x-ms-documentdb-partitionkeyrangeid";

		public const string PartitionCount = "x-ms-documentdb-partitioncount";

		public const string CollectionRid = "x-ms-documentdb-collection-rid";

		public const string XPRole = "x-ms-xp-role";

		public const string HasTentativeWrites = "x-ms-cosmosdb-has-tentative-writes";

		public const string IsRUPerMinuteUsed = "x-ms-documentdb-is-ru-per-minute-used";

		public const string QueryMetrics = "x-ms-documentdb-query-metrics";

		public const string QueryExecutionInfo = "x-ms-cosmos-query-execution-info";

		public const string IndexUtilization = "x-ms-cosmos-index-utilization";

		public const string RequiresDistribution = "x-ms-cosmos-query-requiresdistribution";

		public const string CorrelatedActivityId = "x-ms-cosmos-correlated-activityid";

		public const string GlobalCommittedLSN = "x-ms-global-Committed-lsn";

		public const string NumberOfReadRegions = "x-ms-number-of-read-regions";

		public const string OfferReplacePending = "x-ms-offer-replace-pending";

		public const string ItemLSN = "x-ms-item-lsn";

		public const string RemoteStorageType = "x-ms-remote-storage-type";

		public const string RestoreState = "x-ms-restore-state";

		public const string CollectionSecurityIdentifier = "x-ms-collection-security-identifier";

		public const string RestoreParams = "x-ms-restore-params";

		public const string AllowRestoreParamsUpdate = "x-ms-cosmos-internal-allow-restore-params-update";

		public const string ShareThroughput = "x-ms-share-throughput";

		public const string PartitionResourceFilter = "x-ms-partition-resource-filter";

		public const string FederationIdForAuth = "x-ms-federation-for-auth";

		public const string ForceQueryScan = "x-ms-documentdb-force-query-scan";

		public const string EnableConflictResolutionPolicyUpdate = "x-ms-cosmos-internal-enable-conflictresolutionpolicy-update";

		public const string EnableDynamicRidRangeAllocation = "x-ms-enable-dynamic-rid-range-allocation";

		public const string ExcludeSystemProperties = "x-ms-exclude-system-properties";

		public const string LocalLSN = "x-ms-cosmos-llsn";

		public const string QuorumAckedLocalLSN = "x-ms-cosmos-quorum-acked-llsn";

		public const string ItemLocalLSN = "x-ms-cosmos-item-llsn";

		public const string MergeStaticId = "x-ms-cosmos-merge-static-id";

		public const string ReplicatorLSNToGLSNDelta = "x-ms-cosmos-replicator-glsn-delta";

		public const string ReplicatorLSNToLLSNDelta = "x-ms-cosmos-replicator-llsn-delta";

		public const string VectorClockLocalProgress = "x-ms-cosmos-vectorclock-local-progress";

		public const string MinimumRUsForOffer = "x-ms-cosmos-min-throughput";

		public const string XPConfigurationSessionsCount = "x-ms-cosmos-xpconfiguration-sessions-count";

		public const string SoftMaxAllowedThroughput = "x-ms-cosmos-offer-max-allowed-throughput";

		public const string SetMasterResourcesDeletionPending = "x-ms-cosmos-internal-set-master-resources-deletion-pending";

		public const string InstantScaleUpValue = "x-ms-cosmos-instant-scale-up-value";

		public const string HighPriorityForcedBackup = "x-ms-cosmos-internal-high-priority-forced-backup";

		public const string BinaryId = "x-ms-binary-id";

		public const string TimeToLiveInSeconds = "x-ms-time-to-live-in-seconds";

		public const string EffectivePartitionKey = "x-ms-effective-partition-key";

		public const string BinaryPassthroughRequest = "x-ms-binary-passthrough-request";

		public const string FanoutOperationState = "x-ms-fanout-operation-state";

		public const string ContentSerializationFormat = "x-ms-documentdb-content-serialization-format";

		public const string SupportedSerializationFormats = "x-ms-cosmos-supported-serialization-formats";

		public const string AllowTentativeWrites = "x-ms-cosmos-allow-tentative-writes";

		public const string IsUserRequest = "x-ms-cosmos-internal-is-user-request";

		public const string PreserveFullContent = "x-ms-cosmos-preserve-full-content";

		public const string EffectivePartitionKeyString = "x-ms-effective-partition-key-string";

		public const string SchemaOwnerRid = "x-ms-schema-owner-rid";

		public const string SchemaHash = "x-ms-schema-hash";

		public const string SchemaId = "x-ms-schema-id";

		public const string PopulateLogStoreInfo = "x-ms-cosmos-populate-logstoreinfo";

		public const string ForceSideBySideIndexMigration = "x-ms-cosmos-force-sidebyside-indexmigration";

		public const string CollectionChildResourceNameLimitInBytes = "x-ms-cosmos-collection-child-resourcename-limit";

		public const string CollectionChildResourceContentLimitInKB = "x-ms-cosmos-collection-child-contentlength-resourcelimit";

		public const string MergeCheckPointGLSN = "x-ms-cosmos-internal-merge-checkpoint-glsn";

		public const string UniqueIndexNameEncodingMode = "x-ms-cosmos-unique-index-name-encoding-mode";

		public const string PopulateUnflushedMergeEntryCount = "x-ms-cosmos-internal-populate-unflushed-merge-entry-count";

		public const string UnflushedMergLogEntryCount = "x-ms-cosmos-internal-unflushed-merge-log-entry-count";

		public const string ResourceTypes = "x-ms-cosmos-resourcetypes";

		public const string TransactionId = "x-ms-cosmos-tx-id";

		public const string TransactionFirstRequest = "x-ms-cosmos-tx-init";

		public const string TransactionCommit = "x-ms-cosmos-tx-commit";

		public const string ReplicaStatusRevoked = "x-ms-cosmos-is-replica-status-revoked";

		public const string UniqueIndexReIndexingState = "x-ms-cosmos-uniqueindex-reindexing-state";

		public const string UseSystemBudget = "x-ms-cosmos-use-systembudget";

		public const string RetriableWriteRequestId = "x-ms-cosmos-retriable-write-request-id";

		public const string IsRetriedWriteRequest = "x-ms-cosmos-is-retried-write-request";

		public const string RetriableWriteRequestStartTimestamp = "x-ms-cosmos-retriable-write-request-start-timestamp";

		public const string AddResourcePropertiesToResponse = "x-ms-cosmos-add-resource-properties-to-response";

		public const string SkipRefreshDatabaseAccountConfigs = "x-ms-cosmos-skip-refresh-databaseaccountconfig";

		public const string ReplicaConfirmedChecksum = "x-ms-cosmos-replica-confirmed-checksum";

		public const string ReplicaTentativeChecksum = "x-ms-cosmos-replica-tentative-checksum";

		public const string IntendedCollectionRid = "x-ms-cosmos-intended-collection-rid";

		public const string PendingPKDelete = "x-ms-cosmos-is-partition-key-delete-pending";

		public const string CollectionUniqueIndexReIndexProgress = "x-ms-cosmos-collection-unique-index-reindex-progress";

		public const string CollectionUniqueKeysUnderReIndex = "x-ms-cosmos-collection-unique-keys-under-reindex";

		public const string CollectionTruncate = "x-ms-cosmos-collection-truncate";

		public const string SourceCollectionIfMatch = "x-ms-cosmos-source-collection-if-match";

		public const string RequestedCollectionType = "x-ms-cosmos-collectiontype";

		public const string AnalyticalMigrationProgress = "x-ms-cosmos-analytical-migration-progress";

		public const string ByokEncryptionProgress = "x-ms-cosmos-byok-encryption-progress";

		public const string UseUserBackgroundBudget = "x-ms-cosmos-use-background-task-budget";

		public const string MergeProgressBlocked = "x-ms-cosmos-is-merge-progress-blocked";

		public const string PopulateByokEncryptionProgress = "x-ms-cosmos-populate-byok-encryption-progress";

		public const string ReIndexerProgress = "x-ms-cosmos-reindexer-progress";

		public const string OfferReplacePendingForMerge = "x-ms-offer-replace-pending-for-merge";

		public const string OldestActiveSchemaId = "x-ms-cosmos-oldest-active-schema-id";

		public const string PhysicalPartitionId = "x-ms-cosmos-physical-partition-id";

		public const string OfferRestorePending = "x-ms-offer-restore-pending";

		public const string PopulateMinGLSNForDocumentOperations = "x-ms-cosmos-internal-populate-min-glsn-for-relocation";

		public const string MinGLSNForDocumentOperations = "x-ms-cosmos-min-document-glsn";

		public const string MinGLSNForTombstoneOperations = "x-ms-cosmos-min-tombstone-glsn";

		public const string PopulateHighestTentativeWriteLLSN = "x-ms-cosmos-internal-populate-highest-tentative-write-llsn";

		public const string HighestTentativeWriteLLSN = "x-ms-cosmos-highest-tentative-write-llsn";

		public const string PopulateCurrentPartitionThroughputInfo = "x-ms-cosmos-populate-current-partition-throughput-info";

		public const string PartitionThroughputInfo = "x-ms-cosmos-partition-throughput-info";

		public const string PopulateDocumentRecordCount = "x-ms-cosmos-internal-populate-document-record-count";

		public const string DocumentRecordCount = "x-ms-cosmos-document-record-count";

		public const string PopulateCapacityType = "x-ms-cosmos-populate-capacity-type";

		public const string ClientIpAddress = "x-ms-cosmos-client-ip-address";

		public const string IsRequestNotAuthorized = "x-ms-cosmos-is-request-not-authorized";

		public const string StartEpkHash = "x-ms-cosmos-start-epk-hash";

		public const string EndEpkHash = "x-ms-cosmos-end-epk-hash";
	}

	public const int DefaultFabricNameResolutionTimeoutInSeconds = 10;
}
