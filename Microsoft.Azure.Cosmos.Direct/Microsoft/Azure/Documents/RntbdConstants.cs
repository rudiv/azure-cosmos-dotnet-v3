using System;
using System.Collections.Concurrent;

namespace Microsoft.Azure.Documents;

internal static class RntbdConstants
{
	public enum RntbdResourceType : ushort
	{
		Connection = 0,
		Database = 1,
		Collection = 2,
		Document = 3,
		Attachment = 4,
		User = 5,
		Permission = 6,
		StoredProcedure = 7,
		Conflict = 8,
		Trigger = 9,
		UserDefinedFunction = 10,
		Module = 11,
		Replica = 12,
		ModuleCommand = 13,
		Record = 14,
		Offer = 15,
		PartitionSetInformation = 16,
		XPReplicatorAddress = 17,
		MasterPartition = 18,
		ServerPartition = 19,
		DatabaseAccount = 20,
		Topology = 21,
		PartitionKeyRange = 22,
		Schema = 24,
		BatchApply = 25,
		RestoreMetadata = 26,
		ComputeGatewayCharges = 27,
		RidRange = 28,
		UserDefinedType = 29,
		VectorClock = 31,
		PartitionKey = 32,
		Snapshot = 33,
		ClientEncryptionKey = 35,
		Transaction = 37,
		PartitionedSystemDocument = 38,
		RoleDefinition = 39,
		RoleAssignment = 40,
		SystemDocument = 41,
		InteropUser = 42,
		TransportControlCommand = 43,
		AuthPolicyElement = 44,
		StorageAuthToken = 45,
		RetriableWriteCachedResponse = 46,
		EncryptionScope = 48
	}

	public enum RntbdOperationType : ushort
	{
		Connection = 0,
		Create = 1,
		Patch = 2,
		Read = 3,
		ReadFeed = 4,
		Delete = 5,
		Replace = 6,
		ExecuteJavaScript = 8,
		SQLQuery = 9,
		Pause = 10,
		Resume = 11,
		Stop = 12,
		Recycle = 13,
		Crash = 14,
		Query = 15,
		ForceConfigRefresh = 16,
		Head = 17,
		HeadFeed = 18,
		Upsert = 19,
		Recreate = 20,
		Throttle = 21,
		GetSplitPoint = 22,
		PreCreateValidation = 23,
		BatchApply = 24,
		AbortSplit = 25,
		CompleteSplit = 26,
		OfferUpdateOperation = 27,
		OfferPreGrowValidation = 28,
		BatchReportThroughputUtilization = 29,
		CompletePartitionMigration = 30,
		AbortPartitionMigration = 31,
		PreReplaceValidation = 32,
		AddComputeGatewayRequestCharges = 33,
		MigratePartition = 34,
		MasterReplaceOfferOperation = 35,
		ProvisionedCollectionOfferUpdateOperation = 36,
		Batch = 37,
		InitiateDatabaseOfferPartitionShrink = 38,
		CompleteDatabaseOfferPartitionShrink = 39,
		EnsureSnapshotOperation = 40,
		GetSplitPoints = 41,
		CompleteMergeOnTarget = 42,
		CompleteMergeOnMaster = 44,
		ForcePartitionBackup = 46,
		CompleteUserTransaction = 47,
		MasterInitiatedProgressCoordination = 48,
		MetadataCheckAccess = 49,
		CreateSystemSnapshot = 50,
		UpdateFailoverPriorityList = 51,
		GetStorageAuthToken = 52,
		UpdatePartitionThroughput = 53,
		CreateRidRangeResources = 54,
		Truncate = 55
	}

	public enum ConnectionContextRequestTokenIdentifiers : ushort
	{
		ProtocolVersion,
		ClientVersion,
		UserAgent,
		CallerId,
		EnableChannelMultiplexing
	}

	public sealed class ConnectionContextRequest : RntbdTokenStream<ConnectionContextRequestTokenIdentifiers>
	{
		public RntbdToken protocolVersion;

		public RntbdToken clientVersion;

		public RntbdToken userAgent;

		public RntbdToken callerId;

		public RntbdToken enableChannelMultiplexing;

		public override int RequiredTokenCount => 3;

		public ConnectionContextRequest()
		{
			protocolVersion = new RntbdToken(isRequired: true, RntbdTokenTypes.ULong, 0);
			clientVersion = new RntbdToken(isRequired: true, RntbdTokenTypes.SmallString, 1);
			userAgent = new RntbdToken(isRequired: true, RntbdTokenTypes.SmallString, 2);
			callerId = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 3);
			enableChannelMultiplexing = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 4);
			tokens = new RntbdToken[5] { protocolVersion, clientVersion, userAgent, callerId, enableChannelMultiplexing };
		}
	}

	public enum ConnectionContextResponseTokenIdentifiers : ushort
	{
		ProtocolVersion,
		ClientVersion,
		ServerAgent,
		ServerVersion,
		IdleTimeoutInSeconds,
		UnauthenticatedTimeoutInSeconds
	}

	public sealed class ConnectionContextResponse : RntbdTokenStream<ConnectionContextResponseTokenIdentifiers>
	{
		public RntbdToken protocolVersion;

		public RntbdToken clientVersion;

		public RntbdToken serverAgent;

		public RntbdToken serverVersion;

		public RntbdToken idleTimeoutInSeconds;

		public RntbdToken unauthenticatedTimeoutInSeconds;

		public override int RequiredTokenCount => 2;

		public ConnectionContextResponse()
		{
			protocolVersion = new RntbdToken(isRequired: false, RntbdTokenTypes.ULong, 0);
			clientVersion = new RntbdToken(isRequired: false, RntbdTokenTypes.SmallString, 1);
			serverAgent = new RntbdToken(isRequired: true, RntbdTokenTypes.SmallString, 2);
			serverVersion = new RntbdToken(isRequired: true, RntbdTokenTypes.SmallString, 3);
			idleTimeoutInSeconds = new RntbdToken(isRequired: false, RntbdTokenTypes.ULong, 4);
			unauthenticatedTimeoutInSeconds = new RntbdToken(isRequired: false, RntbdTokenTypes.ULong, 5);
			tokens = new RntbdToken[6] { protocolVersion, clientVersion, serverAgent, serverVersion, idleTimeoutInSeconds, unauthenticatedTimeoutInSeconds };
		}
	}

	public enum RntbdIndexingDirective : byte
	{
		Default = 0,
		Include = 1,
		Exclude = 2,
		Invalid = byte.MaxValue
	}

	public enum RntbdMigrateCollectionDirective : byte
	{
		Thaw = 0,
		Freeze = 1,
		Invalid = byte.MaxValue
	}

	public enum RntbdRemoteStorageType : byte
	{
		Invalid,
		NotSpecified,
		Standard,
		Premium
	}

	public enum RntbdConsistencyLevel : byte
	{
		Strong = 0,
		BoundedStaleness = 1,
		Session = 2,
		Eventual = 3,
		ConsistentPrefix = 4,
		Invalid = byte.MaxValue
	}

	public enum RntdbEnumerationDirection : byte
	{
		Invalid,
		Forward,
		Reverse
	}

	public enum RntbdFanoutOperationState : byte
	{
		Started = 1,
		Completed
	}

	public enum RntdbReadFeedKeyType : byte
	{
		Invalid,
		ResourceId,
		EffectivePartitionKey,
		EffectivePartitionKeyRange
	}

	public enum RntbdContentSerializationFormat : byte
	{
		JsonText = 0,
		CosmosBinary = 1,
		HybridRow = 2,
		Invalid = byte.MaxValue
	}

	[Flags]
	public enum RntbdSupportedSerializationFormats : byte
	{
		None = 0,
		JsonText = 1,
		CosmosBinary = 2,
		HybridRow = 4
	}

	public enum RntbdSystemDocumentType : byte
	{
		PartitionKey = 0,
		MaterializedViewLeaseDocument = 1,
		MaterializedViewBuilderOwnershipDocument = 2,
		MaterializedViewLeaseStoreInitDocument = 3,
		Invalid = byte.MaxValue
	}

	public enum RntbdRequestedCollectionType : byte
	{
		All,
		Standard,
		MaterializedView
	}

	public enum RntbdPriorityLevel : byte
	{
		High = 1,
		Low
	}

	public enum RequestIdentifiers : ushort
	{
		ResourceId = 0,
		AuthorizationToken = 1,
		PayloadPresent = 2,
		Date = 3,
		PageSize = 4,
		SessionToken = 5,
		ContinuationToken = 6,
		IndexingDirective = 7,
		Match = 8,
		PreTriggerInclude = 9,
		PostTriggerInclude = 10,
		IsFanout = 11,
		CollectionPartitionIndex = 12,
		CollectionServiceIndex = 13,
		PreTriggerExclude = 14,
		PostTriggerExclude = 15,
		ConsistencyLevel = 16,
		EntityId = 17,
		ResourceSchemaName = 18,
		ReplicaPath = 19,
		ResourceTokenExpiry = 20,
		DatabaseName = 21,
		CollectionName = 22,
		DocumentName = 23,
		AttachmentName = 24,
		UserName = 25,
		PermissionName = 26,
		StoredProcedureName = 27,
		UserDefinedFunctionName = 28,
		TriggerName = 29,
		EnableScanInQuery = 30,
		EmitVerboseTracesInQuery = 31,
		ConflictName = 32,
		BindReplicaDirective = 33,
		PrimaryMasterKey = 34,
		SecondaryMasterKey = 35,
		PrimaryReadonlyKey = 36,
		SecondaryReadonlyKey = 37,
		ProfileRequest = 38,
		EnableLowPrecisionOrderBy = 39,
		ClientVersion = 40,
		CanCharge = 41,
		CanThrottle = 42,
		PartitionKey = 43,
		PartitionKeyRangeId = 44,
		MigrateCollectionDirective = 49,
		SupportSpatialLegacyCoordinates = 51,
		PartitionCount = 52,
		CollectionRid = 53,
		PartitionKeyRangeName = 54,
		SchemaName = 58,
		FilterBySchemaRid = 59,
		UsePolygonsSmallerThanAHemisphere = 60,
		GatewaySignature = 61,
		EnableLogging = 62,
		A_IM = 63,
		PopulateQuotaInfo = 64,
		DisableRUPerMinuteUsage = 65,
		PopulateQueryMetrics = 66,
		ResponseContinuationTokenLimitInKb = 67,
		PopulatePartitionStatistics = 68,
		RemoteStorageType = 69,
		CollectionRemoteStorageSecurityIdentifier = 70,
		IfModifiedSince = 71,
		PopulateCollectionThroughputInfo = 72,
		RemainingTimeInMsOnClientRequest = 73,
		ClientRetryAttemptCount = 74,
		TargetLsn = 75,
		TargetGlobalCommittedLsn = 76,
		TransportRequestID = 77,
		RestoreMetadataFilter = 78,
		RestoreParams = 79,
		ShareThroughput = 80,
		PartitionResourceFilter = 81,
		IsReadOnlyScript = 82,
		IsAutoScaleRequest = 83,
		ForceQueryScan = 84,
		CanOfferReplaceComplete = 86,
		ExcludeSystemProperties = 87,
		BinaryId = 88,
		TimeToLiveInSeconds = 89,
		EffectivePartitionKey = 90,
		BinaryPassthroughRequest = 91,
		UserDefinedTypeName = 92,
		EnableDynamicRidRangeAllocation = 93,
		EnumerationDirection = 94,
		StartId = 95,
		EndId = 96,
		FanoutOperationState = 97,
		StartEpk = 98,
		EndEpk = 99,
		ReadFeedKeyType = 100,
		ContentSerializationFormat = 101,
		AllowTentativeWrites = 102,
		IsUserRequest = 103,
		PreserveFullContent = 105,
		IncludeTentativeWrites = 112,
		PopulateResourceCount = 113,
		MergeStaticId = 114,
		IsBatchAtomic = 115,
		ShouldBatchContinueOnError = 116,
		IsBatchOrdered = 117,
		SchemaOwnerRid = 118,
		SchemaHash = 119,
		IsRUPerGBEnforcementRequest = 120,
		MaxPollingIntervalMilliseconds = 121,
		SnapshotName = 122,
		PopulateLogStoreInfo = 123,
		GetAllPartitionKeyStatistics = 124,
		ForceSideBySideIndexMigration = 125,
		CollectionChildResourceNameLimitInBytes = 126,
		CollectionChildResourceContentLengthLimitInKB = 127,
		ClientEncryptionKeyName = 128,
		MergeCheckpointGLSNKeyName = 129,
		ReturnPreference = 130,
		UniqueIndexNameEncodingMode = 131,
		PopulateUnflushedMergeEntryCount = 132,
		MigrateOfferToManualThroughput = 133,
		MigrateOfferToAutopilot = 134,
		IsClientEncrypted = 135,
		SystemDocumentType = 136,
		IsofferStorageRefreshRequest = 137,
		ResourceTypes = 138,
		TransactionId = 139,
		TransactionFirstRequest = 140,
		TransactionCommit = 141,
		SystemDocumentName = 142,
		UpdateMaxThroughputEverProvisioned = 143,
		UniqueIndexReIndexingState = 144,
		RoleDefinitionName = 145,
		RoleAssignmentName = 146,
		UseSystemBudget = 147,
		IgnoreSystemLoweringMaxThroughput = 148,
		TruncateMergeLogRequest = 149,
		RetriableWriteRequestId = 150,
		IsRetriedWriteRequest = 151,
		RetriableWriteRequestStartTimestamp = 152,
		AddResourcePropertiesToResponse = 153,
		ChangeFeedStartFullFidelityIfNoneMatch = 154,
		SystemRestoreOperation = 155,
		SkipRefreshDatabaseAccountConfigs = 156,
		IntendedCollectionRid = 157,
		UseArchivalPartition = 158,
		PopulateUniqueIndexReIndexProgress = 159,
		CollectionSchemaId = 160,
		CollectionTruncate = 161,
		SDKSupportedCapabilities = 162,
		IsMaterializedViewBuild = 163,
		BuilderClientIdentifier = 164,
		SourceCollectionIfMatch = 165,
		RequestedCollectionType = 166,
		InteropUserName = 168,
		PopulateIndexMetrics = 169,
		PopulateAnalyticalMigrationProgress = 170,
		AuthPolicyElementName = 171,
		ShouldReturnCurrentServerDateTime = 172,
		RbacUserId = 173,
		RbacAction = 174,
		RbacResource = 175,
		CorrelatedActivityId = 176,
		IsThroughputCapRequest = 177,
		ChangeFeedWireFormatVersion = 178,
		PopulateBYOKEncryptionProgress = 179,
		UseUserBackgroundBudget = 180,
		IncludePhysicalPartitionThroughputInfo = 181,
		IsServerlessStorageRefreshRequest = 182,
		UpdateOfferStateToPending = 183,
		PopulateOldestActiveSchemaId = 184,
		IsInternalServerlessRequest = 185,
		OfferReplaceRURedistribution = 186,
		IsCassandraAlterTypeRequest = 187,
		IsMaterializedViewSourceSchemaReplaceBatchRequest = 188,
		ForceDatabaseAccountUpdate = 189,
		EncryptionScopeName = 190,
		PriorityLevel = 191,
		AllowRestoreParamsUpdate = 192,
		PruneCollectionSchemas = 193,
		PopulateIndexMetricsV2 = 194,
		IsMigratedFixedCollection = 195,
		SupportedSerializationFormats = 196,
		UpdateOfferStateToRestorePending = 197,
		SetMasterResourcesDeletionPending = 198,
		HighPriorityForcedBackup = 199,
		OptimisticDirectExecute = 200,
		PopulateMinGLSNForDocumentOperations = 201,
		PopulateHighestTentativeWriteLLSN = 202,
		PopulateCapacityType = 203,
		TraceParent = 204,
		TraceState = 205,
		GlobalDatabaseAccountName = 206,
		EnableConflictResolutionPolicyUpdate = 207,
		ClientIpAddress = 208,
		IsRequestNotAuthorized = 209,
		StartEpkHash = 210,
		EndEpkHash = 211,
		AllowDocumentReadsInOfflineRegion = 212,
		PopulateCurrentPartitionThroughputInfo = 213,
		PopulateDocumentRecordCount = 214
	}

	public sealed class Request : RntbdTokenStream<RequestIdentifiers>
	{
		public RntbdToken resourceId;

		public RntbdToken authorizationToken;

		public RntbdToken payloadPresent;

		public RntbdToken date;

		public RntbdToken pageSize;

		public RntbdToken sessionToken;

		public RntbdToken continuationToken;

		public RntbdToken indexingDirective;

		public RntbdToken match;

		public RntbdToken preTriggerInclude;

		public RntbdToken postTriggerInclude;

		public RntbdToken isFanout;

		public RntbdToken collectionPartitionIndex;

		public RntbdToken collectionServiceIndex;

		public RntbdToken preTriggerExclude;

		public RntbdToken postTriggerExclude;

		public RntbdToken consistencyLevel;

		public RntbdToken entityId;

		public RntbdToken resourceSchemaName;

		public RntbdToken replicaPath;

		public RntbdToken resourceTokenExpiry;

		public RntbdToken databaseName;

		public RntbdToken collectionName;

		public RntbdToken documentName;

		public RntbdToken attachmentName;

		public RntbdToken userName;

		public RntbdToken permissionName;

		public RntbdToken storedProcedureName;

		public RntbdToken userDefinedFunctionName;

		public RntbdToken triggerName;

		public RntbdToken enableScanInQuery;

		public RntbdToken emitVerboseTracesInQuery;

		public RntbdToken conflictName;

		public RntbdToken bindReplicaDirective;

		public RntbdToken primaryMasterKey;

		public RntbdToken secondaryMasterKey;

		public RntbdToken primaryReadonlyKey;

		public RntbdToken secondaryReadonlyKey;

		public RntbdToken profileRequest;

		public RntbdToken enableLowPrecisionOrderBy;

		public RntbdToken clientVersion;

		public RntbdToken canCharge;

		public RntbdToken canThrottle;

		public RntbdToken partitionKey;

		public RntbdToken partitionKeyRangeId;

		public RntbdToken migrateCollectionDirective;

		public RntbdToken supportSpatialLegacyCoordinates;

		public RntbdToken partitionCount;

		public RntbdToken collectionRid;

		public RntbdToken partitionKeyRangeName;

		public RntbdToken schemaName;

		public RntbdToken filterBySchemaRid;

		public RntbdToken usePolygonsSmallerThanAHemisphere;

		public RntbdToken gatewaySignature;

		public RntbdToken enableLogging;

		public RntbdToken a_IM;

		public RntbdToken populateQuotaInfo;

		public RntbdToken disableRUPerMinuteUsage;

		public RntbdToken populateQueryMetrics;

		public RntbdToken responseContinuationTokenLimitInKb;

		public RntbdToken populatePartitionStatistics;

		public RntbdToken remoteStorageType;

		public RntbdToken collectionRemoteStorageSecurityIdentifier;

		public RntbdToken ifModifiedSince;

		public RntbdToken populateCollectionThroughputInfo;

		public RntbdToken remainingTimeInMsOnClientRequest;

		public RntbdToken clientRetryAttemptCount;

		public RntbdToken targetLsn;

		public RntbdToken targetGlobalCommittedLsn;

		public RntbdToken transportRequestID;

		public RntbdToken restoreMetadataFilter;

		public RntbdToken restoreParams;

		public RntbdToken shareThroughput;

		public RntbdToken partitionResourceFilter;

		public RntbdToken isReadOnlyScript;

		public RntbdToken isAutoScaleRequest;

		public RntbdToken forceQueryScan;

		public RntbdToken canOfferReplaceComplete;

		public RntbdToken excludeSystemProperties;

		public RntbdToken binaryId;

		public RntbdToken timeToLiveInSeconds;

		public RntbdToken effectivePartitionKey;

		public RntbdToken binaryPassthroughRequest;

		public RntbdToken userDefinedTypeName;

		public RntbdToken enableDynamicRidRangeAllocation;

		public RntbdToken enumerationDirection;

		public RntbdToken startId;

		public RntbdToken endId;

		public RntbdToken fanoutOperationState;

		public RntbdToken startEpk;

		public RntbdToken endEpk;

		public RntbdToken readFeedKeyType;

		public RntbdToken contentSerializationFormat;

		public RntbdToken allowTentativeWrites;

		public RntbdToken isUserRequest;

		public RntbdToken preserveFullContent;

		public RntbdToken includeTentativeWrites;

		public RntbdToken populateResourceCount;

		public RntbdToken mergeStaticId;

		public RntbdToken isBatchAtomic;

		public RntbdToken shouldBatchContinueOnError;

		public RntbdToken isBatchOrdered;

		public RntbdToken schemaOwnerRid;

		public RntbdToken schemaHash;

		public RntbdToken isRUPerGBEnforcementRequest;

		public RntbdToken maxPollingIntervalMilliseconds;

		public RntbdToken snapshotName;

		public RntbdToken populateLogStoreInfo;

		public RntbdToken getAllPartitionKeyStatistics;

		public RntbdToken forceSideBySideIndexMigration;

		public RntbdToken collectionChildResourceNameLimitInBytes;

		public RntbdToken collectionChildResourceContentLengthLimitInKB;

		public RntbdToken clientEncryptionKeyName;

		public RntbdToken mergeCheckpointGLSNKeyName;

		public RntbdToken returnPreference;

		public RntbdToken uniqueIndexNameEncodingMode;

		public RntbdToken populateUnflushedMergeEntryCount;

		public RntbdToken migrateOfferToManualThroughput;

		public RntbdToken migrateOfferToAutopilot;

		public RntbdToken isClientEncrypted;

		public RntbdToken systemDocumentType;

		public RntbdToken isofferStorageRefreshRequest;

		public RntbdToken resourceTypes;

		public RntbdToken transactionId;

		public RntbdToken transactionFirstRequest;

		public RntbdToken transactionCommit;

		public RntbdToken systemDocumentName;

		public RntbdToken updateMaxThroughputEverProvisioned;

		public RntbdToken uniqueIndexReIndexingState;

		public RntbdToken roleDefinitionName;

		public RntbdToken roleAssignmentName;

		public RntbdToken useSystemBudget;

		public RntbdToken ignoreSystemLoweringMaxThroughput;

		public RntbdToken truncateMergeLogRequest;

		public RntbdToken retriableWriteRequestId;

		public RntbdToken isRetriedWriteRequest;

		public RntbdToken retriableWriteRequestStartTimestamp;

		public RntbdToken addResourcePropertiesToResponse;

		public RntbdToken changeFeedStartFullFidelityIfNoneMatch;

		public RntbdToken systemRestoreOperation;

		public RntbdToken skipRefreshDatabaseAccountConfigs;

		public RntbdToken intendedCollectionRid;

		public RntbdToken useArchivalPartition;

		public RntbdToken populateUniqueIndexReIndexProgress;

		public RntbdToken collectionSchemaId;

		public RntbdToken collectionTruncate;

		public RntbdToken sDKSupportedCapabilities;

		public RntbdToken isMaterializedViewBuild;

		public RntbdToken builderClientIdentifier;

		public RntbdToken sourceCollectionIfMatch;

		public RntbdToken requestedCollectionType;

		public RntbdToken interopUserName;

		public RntbdToken populateIndexMetrics;

		public RntbdToken populateAnalyticalMigrationProgress;

		public RntbdToken authPolicyElementName;

		public RntbdToken shouldReturnCurrentServerDateTime;

		public RntbdToken rbacUserId;

		public RntbdToken rbacAction;

		public RntbdToken rbacResource;

		public RntbdToken correlatedActivityId;

		public RntbdToken isThroughputCapRequest;

		public RntbdToken changeFeedWireFormatVersion;

		public RntbdToken populateBYOKEncryptionProgress;

		public RntbdToken useUserBackgroundBudget;

		public RntbdToken includePhysicalPartitionThroughputInfo;

		public RntbdToken isServerlessStorageRefreshRequest;

		public RntbdToken updateOfferStateToPending;

		public RntbdToken populateOldestActiveSchemaId;

		public RntbdToken isInternalServerlessRequest;

		public RntbdToken offerReplaceRURedistribution;

		public RntbdToken isCassandraAlterTypeRequest;

		public RntbdToken isMaterializedViewSourceSchemaReplaceBatchRequest;

		public RntbdToken forceDatabaseAccountUpdate;

		public RntbdToken encryptionScopeName;

		public RntbdToken priorityLevel;

		public RntbdToken allowRestoreParamsUpdate;

		public RntbdToken pruneCollectionSchemas;

		public RntbdToken populateIndexMetricsV2;

		public RntbdToken isMigratedFixedCollection;

		public RntbdToken supportedSerializationFormats;

		public RntbdToken updateOfferStateToRestorePending;

		public RntbdToken setMasterResourcesDeletionPending;

		public RntbdToken highPriorityForcedBackup;

		public RntbdToken optimisticDirectExecute;

		public RntbdToken populateMinGLSNForDocumentOperations;

		public RntbdToken populateHighestTentativeWriteLLSN;

		public RntbdToken populateCapacityType;

		public RntbdToken traceParent;

		public RntbdToken traceState;

		public RntbdToken globalDatabaseAccountName;

		public RntbdToken enableConflictResolutionPolicyUpdate;

		public RntbdToken clientIpAddress;

		public RntbdToken isRequestNotAuthorized;

		public RntbdToken startEpkHash;

		public RntbdToken endEpkHash;

		public RntbdToken allowDocumentReadsInOfflineRegion;

		public RntbdToken populateCurrentPartitionThroughputInfo;

		public RntbdToken populateDocumentRecordCount;

		public override int RequiredTokenCount => 2;

		public Request()
		{
			resourceId = new RntbdToken(isRequired: false, RntbdTokenTypes.Bytes, 0);
			authorizationToken = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 1);
			payloadPresent = new RntbdToken(isRequired: true, RntbdTokenTypes.Byte, 2);
			date = new RntbdToken(isRequired: false, RntbdTokenTypes.SmallString, 3);
			pageSize = new RntbdToken(isRequired: false, RntbdTokenTypes.ULong, 4);
			sessionToken = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 5);
			continuationToken = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 6);
			indexingDirective = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 7);
			match = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 8);
			preTriggerInclude = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 9);
			postTriggerInclude = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 10);
			isFanout = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 11);
			collectionPartitionIndex = new RntbdToken(isRequired: false, RntbdTokenTypes.ULong, 12);
			collectionServiceIndex = new RntbdToken(isRequired: false, RntbdTokenTypes.ULong, 13);
			preTriggerExclude = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 14);
			postTriggerExclude = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 15);
			consistencyLevel = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 16);
			entityId = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 17);
			resourceSchemaName = new RntbdToken(isRequired: false, RntbdTokenTypes.SmallString, 18);
			replicaPath = new RntbdToken(isRequired: true, RntbdTokenTypes.String, 19);
			resourceTokenExpiry = new RntbdToken(isRequired: false, RntbdTokenTypes.ULong, 20);
			databaseName = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 21);
			collectionName = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 22);
			documentName = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 23);
			attachmentName = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 24);
			userName = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 25);
			permissionName = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 26);
			storedProcedureName = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 27);
			userDefinedFunctionName = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 28);
			triggerName = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 29);
			enableScanInQuery = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 30);
			emitVerboseTracesInQuery = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 31);
			conflictName = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 32);
			bindReplicaDirective = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 33);
			primaryMasterKey = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 34);
			secondaryMasterKey = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 35);
			primaryReadonlyKey = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 36);
			secondaryReadonlyKey = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 37);
			profileRequest = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 38);
			enableLowPrecisionOrderBy = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 39);
			clientVersion = new RntbdToken(isRequired: false, RntbdTokenTypes.SmallString, 40);
			canCharge = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 41);
			canThrottle = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 42);
			partitionKey = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 43);
			partitionKeyRangeId = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 44);
			migrateCollectionDirective = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 49);
			supportSpatialLegacyCoordinates = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 51);
			partitionCount = new RntbdToken(isRequired: false, RntbdTokenTypes.ULong, 52);
			collectionRid = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 53);
			partitionKeyRangeName = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 54);
			schemaName = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 58);
			filterBySchemaRid = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 59);
			usePolygonsSmallerThanAHemisphere = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 60);
			gatewaySignature = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 61);
			enableLogging = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 62);
			a_IM = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 63);
			populateQuotaInfo = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 64);
			disableRUPerMinuteUsage = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 65);
			populateQueryMetrics = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 66);
			responseContinuationTokenLimitInKb = new RntbdToken(isRequired: false, RntbdTokenTypes.ULong, 67);
			populatePartitionStatistics = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 68);
			remoteStorageType = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 69);
			collectionRemoteStorageSecurityIdentifier = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 70);
			ifModifiedSince = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 71);
			populateCollectionThroughputInfo = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 72);
			remainingTimeInMsOnClientRequest = new RntbdToken(isRequired: false, RntbdTokenTypes.ULong, 73);
			clientRetryAttemptCount = new RntbdToken(isRequired: false, RntbdTokenTypes.ULong, 74);
			targetLsn = new RntbdToken(isRequired: false, RntbdTokenTypes.LongLong, 75);
			targetGlobalCommittedLsn = new RntbdToken(isRequired: false, RntbdTokenTypes.LongLong, 76);
			transportRequestID = new RntbdToken(isRequired: false, RntbdTokenTypes.ULong, 77);
			restoreMetadataFilter = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 78);
			restoreParams = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 79);
			shareThroughput = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 80);
			partitionResourceFilter = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 81);
			isReadOnlyScript = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 82);
			isAutoScaleRequest = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 83);
			forceQueryScan = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 84);
			canOfferReplaceComplete = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 86);
			excludeSystemProperties = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 87);
			binaryId = new RntbdToken(isRequired: false, RntbdTokenTypes.Bytes, 88);
			timeToLiveInSeconds = new RntbdToken(isRequired: false, RntbdTokenTypes.ULong, 89);
			effectivePartitionKey = new RntbdToken(isRequired: false, RntbdTokenTypes.Bytes, 90);
			binaryPassthroughRequest = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 91);
			userDefinedTypeName = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 92);
			enableDynamicRidRangeAllocation = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 93);
			enumerationDirection = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 94);
			startId = new RntbdToken(isRequired: false, RntbdTokenTypes.Bytes, 95);
			endId = new RntbdToken(isRequired: false, RntbdTokenTypes.Bytes, 96);
			fanoutOperationState = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 97);
			startEpk = new RntbdToken(isRequired: false, RntbdTokenTypes.Bytes, 98);
			endEpk = new RntbdToken(isRequired: false, RntbdTokenTypes.Bytes, 99);
			readFeedKeyType = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 100);
			contentSerializationFormat = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 101);
			allowTentativeWrites = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 102);
			isUserRequest = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 103);
			preserveFullContent = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 105);
			includeTentativeWrites = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 112);
			populateResourceCount = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 113);
			mergeStaticId = new RntbdToken(isRequired: false, RntbdTokenTypes.Bytes, 114);
			isBatchAtomic = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 115);
			shouldBatchContinueOnError = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 116);
			isBatchOrdered = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 117);
			schemaOwnerRid = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 118);
			schemaHash = new RntbdToken(isRequired: false, RntbdTokenTypes.Bytes, 119);
			isRUPerGBEnforcementRequest = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 120);
			maxPollingIntervalMilliseconds = new RntbdToken(isRequired: false, RntbdTokenTypes.ULong, 121);
			snapshotName = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 122);
			populateLogStoreInfo = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 123);
			getAllPartitionKeyStatistics = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 124);
			forceSideBySideIndexMigration = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 125);
			collectionChildResourceNameLimitInBytes = new RntbdToken(isRequired: false, RntbdTokenTypes.Bytes, 126);
			collectionChildResourceContentLengthLimitInKB = new RntbdToken(isRequired: false, RntbdTokenTypes.Bytes, 127);
			clientEncryptionKeyName = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 128);
			mergeCheckpointGLSNKeyName = new RntbdToken(isRequired: false, RntbdTokenTypes.LongLong, 129);
			returnPreference = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 130);
			uniqueIndexNameEncodingMode = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 131);
			populateUnflushedMergeEntryCount = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 132);
			migrateOfferToManualThroughput = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 133);
			migrateOfferToAutopilot = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 134);
			isClientEncrypted = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 135);
			systemDocumentType = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 136);
			isofferStorageRefreshRequest = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 137);
			resourceTypes = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 138);
			transactionId = new RntbdToken(isRequired: false, RntbdTokenTypes.Bytes, 139);
			transactionFirstRequest = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 140);
			transactionCommit = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 141);
			systemDocumentName = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 142);
			updateMaxThroughputEverProvisioned = new RntbdToken(isRequired: false, RntbdTokenTypes.ULong, 143);
			uniqueIndexReIndexingState = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 144);
			roleDefinitionName = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 145);
			roleAssignmentName = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 146);
			useSystemBudget = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 147);
			ignoreSystemLoweringMaxThroughput = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 148);
			truncateMergeLogRequest = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 149);
			retriableWriteRequestId = new RntbdToken(isRequired: false, RntbdTokenTypes.Bytes, 150);
			isRetriedWriteRequest = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 151);
			retriableWriteRequestStartTimestamp = new RntbdToken(isRequired: false, RntbdTokenTypes.ULongLong, 152);
			addResourcePropertiesToResponse = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 153);
			changeFeedStartFullFidelityIfNoneMatch = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 154);
			systemRestoreOperation = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 155);
			skipRefreshDatabaseAccountConfigs = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 156);
			intendedCollectionRid = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 157);
			useArchivalPartition = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 158);
			populateUniqueIndexReIndexProgress = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 159);
			collectionSchemaId = new RntbdToken(isRequired: false, RntbdTokenTypes.Long, 160);
			collectionTruncate = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 161);
			sDKSupportedCapabilities = new RntbdToken(isRequired: false, RntbdTokenTypes.ULong, 162);
			isMaterializedViewBuild = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 163);
			builderClientIdentifier = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 164);
			sourceCollectionIfMatch = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 165);
			requestedCollectionType = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 166);
			interopUserName = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 168);
			populateIndexMetrics = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 169);
			populateAnalyticalMigrationProgress = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 170);
			authPolicyElementName = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 171);
			shouldReturnCurrentServerDateTime = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 172);
			rbacUserId = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 173);
			rbacAction = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 174);
			rbacResource = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 175);
			correlatedActivityId = new RntbdToken(isRequired: false, RntbdTokenTypes.Guid, 176);
			isThroughputCapRequest = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 177);
			changeFeedWireFormatVersion = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 178);
			populateBYOKEncryptionProgress = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 179);
			useUserBackgroundBudget = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 180);
			includePhysicalPartitionThroughputInfo = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 181);
			isServerlessStorageRefreshRequest = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 182);
			updateOfferStateToPending = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 183);
			populateOldestActiveSchemaId = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 184);
			isInternalServerlessRequest = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 185);
			offerReplaceRURedistribution = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 186);
			isCassandraAlterTypeRequest = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 187);
			isMaterializedViewSourceSchemaReplaceBatchRequest = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 188);
			forceDatabaseAccountUpdate = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 189);
			encryptionScopeName = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 190);
			priorityLevel = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 191);
			allowRestoreParamsUpdate = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 192);
			pruneCollectionSchemas = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 193);
			populateIndexMetricsV2 = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 194);
			isMigratedFixedCollection = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 195);
			supportedSerializationFormats = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 196);
			updateOfferStateToRestorePending = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 197);
			setMasterResourcesDeletionPending = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 198);
			highPriorityForcedBackup = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 199);
			optimisticDirectExecute = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 200);
			populateMinGLSNForDocumentOperations = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 201);
			populateHighestTentativeWriteLLSN = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 202);
			populateCapacityType = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 203);
			traceParent = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 204);
			traceState = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 205);
			globalDatabaseAccountName = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 206);
			enableConflictResolutionPolicyUpdate = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 207);
			clientIpAddress = new RntbdToken(isRequired: false, RntbdTokenTypes.String, 208);
			isRequestNotAuthorized = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 209);
			startEpkHash = new RntbdToken(isRequired: false, RntbdTokenTypes.Bytes, 210);
			endEpkHash = new RntbdToken(isRequired: false, RntbdTokenTypes.Bytes, 211);
			allowDocumentReadsInOfflineRegion = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 212);
			populateCurrentPartitionThroughputInfo = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 213);
			populateDocumentRecordCount = new RntbdToken(isRequired: false, RntbdTokenTypes.Byte, 214);
			tokens = new RntbdToken[215]
			{
				resourceId, authorizationToken, payloadPresent, date, pageSize, sessionToken, continuationToken, indexingDirective, match, preTriggerInclude,
				postTriggerInclude, isFanout, collectionPartitionIndex, collectionServiceIndex, preTriggerExclude, postTriggerExclude, consistencyLevel, entityId, resourceSchemaName, replicaPath,
				resourceTokenExpiry, databaseName, collectionName, documentName, attachmentName, userName, permissionName, storedProcedureName, userDefinedFunctionName, triggerName,
				enableScanInQuery, emitVerboseTracesInQuery, conflictName, bindReplicaDirective, primaryMasterKey, secondaryMasterKey, primaryReadonlyKey, secondaryReadonlyKey, profileRequest, enableLowPrecisionOrderBy,
				clientVersion, canCharge, canThrottle, partitionKey, partitionKeyRangeId, null, null, null, null, migrateCollectionDirective,
				null, supportSpatialLegacyCoordinates, partitionCount, collectionRid, partitionKeyRangeName, null, null, null, schemaName, filterBySchemaRid,
				usePolygonsSmallerThanAHemisphere, gatewaySignature, enableLogging, a_IM, populateQuotaInfo, disableRUPerMinuteUsage, populateQueryMetrics, responseContinuationTokenLimitInKb, populatePartitionStatistics, remoteStorageType,
				collectionRemoteStorageSecurityIdentifier, ifModifiedSince, populateCollectionThroughputInfo, remainingTimeInMsOnClientRequest, clientRetryAttemptCount, targetLsn, targetGlobalCommittedLsn, transportRequestID, restoreMetadataFilter, restoreParams,
				shareThroughput, partitionResourceFilter, isReadOnlyScript, isAutoScaleRequest, forceQueryScan, null, canOfferReplaceComplete, excludeSystemProperties, binaryId, timeToLiveInSeconds,
				effectivePartitionKey, binaryPassthroughRequest, userDefinedTypeName, enableDynamicRidRangeAllocation, enumerationDirection, startId, endId, fanoutOperationState, startEpk, endEpk,
				readFeedKeyType, contentSerializationFormat, allowTentativeWrites, isUserRequest, null, preserveFullContent, null, null, null, null,
				null, null, includeTentativeWrites, populateResourceCount, mergeStaticId, isBatchAtomic, shouldBatchContinueOnError, isBatchOrdered, schemaOwnerRid, schemaHash,
				isRUPerGBEnforcementRequest, maxPollingIntervalMilliseconds, snapshotName, populateLogStoreInfo, getAllPartitionKeyStatistics, forceSideBySideIndexMigration, collectionChildResourceNameLimitInBytes, collectionChildResourceContentLengthLimitInKB, clientEncryptionKeyName, mergeCheckpointGLSNKeyName,
				returnPreference, uniqueIndexNameEncodingMode, populateUnflushedMergeEntryCount, migrateOfferToManualThroughput, migrateOfferToAutopilot, isClientEncrypted, systemDocumentType, isofferStorageRefreshRequest, resourceTypes, transactionId,
				transactionFirstRequest, transactionCommit, systemDocumentName, updateMaxThroughputEverProvisioned, uniqueIndexReIndexingState, roleDefinitionName, roleAssignmentName, useSystemBudget, ignoreSystemLoweringMaxThroughput, truncateMergeLogRequest,
				retriableWriteRequestId, isRetriedWriteRequest, retriableWriteRequestStartTimestamp, addResourcePropertiesToResponse, changeFeedStartFullFidelityIfNoneMatch, systemRestoreOperation, skipRefreshDatabaseAccountConfigs, intendedCollectionRid, useArchivalPartition, populateUniqueIndexReIndexProgress,
				collectionSchemaId, collectionTruncate, sDKSupportedCapabilities, isMaterializedViewBuild, builderClientIdentifier, sourceCollectionIfMatch, requestedCollectionType, null, interopUserName, populateIndexMetrics,
				populateAnalyticalMigrationProgress, authPolicyElementName, shouldReturnCurrentServerDateTime, rbacUserId, rbacAction, rbacResource, correlatedActivityId, isThroughputCapRequest, changeFeedWireFormatVersion, populateBYOKEncryptionProgress,
				useUserBackgroundBudget, includePhysicalPartitionThroughputInfo, isServerlessStorageRefreshRequest, updateOfferStateToPending, populateOldestActiveSchemaId, isInternalServerlessRequest, offerReplaceRURedistribution, isCassandraAlterTypeRequest, isMaterializedViewSourceSchemaReplaceBatchRequest, forceDatabaseAccountUpdate,
				encryptionScopeName, priorityLevel, allowRestoreParamsUpdate, pruneCollectionSchemas, populateIndexMetricsV2, isMigratedFixedCollection, supportedSerializationFormats, updateOfferStateToRestorePending, setMasterResourcesDeletionPending, highPriorityForcedBackup,
				optimisticDirectExecute, populateMinGLSNForDocumentOperations, populateHighestTentativeWriteLLSN, populateCapacityType, traceParent, traceState, globalDatabaseAccountName, enableConflictResolutionPolicyUpdate, clientIpAddress, isRequestNotAuthorized,
				startEpkHash, endEpkHash, allowDocumentReadsInOfflineRegion, populateCurrentPartitionThroughputInfo, populateDocumentRecordCount
			};
		}
	}

	public enum ResponseIdentifiers : ushort
	{
		TransportRequestID = 53,
		ServerDateTimeUtc = 57,
		SubStatus = 28,
		ETag = 4,
		ResourceName = 71,
		RequestCharge = 21,
		SessionToken = 62,
		ContinuationToken = 3,
		LSN = 19,
		GlobalCommittedLSN = 41,
		ItemLSN = 50,
		LocalLSN = 58,
		QuorumAckedLocalLSN = 59,
		ItemLocalLSN = 60,
		PayloadPresent = 0,
		LastStateChangeDateTime = 2,
		RetryAfterMilliseconds = 12,
		IndexingDirective = 13,
		StorageMaxResoureQuota = 14,
		StorageResourceQuotaUsage = 15,
		SchemaVersion = 16,
		CollectionPartitionIndex = 17,
		CollectionServiceIndex = 18,
		ItemCount = 20,
		OwnerFullName = 23,
		OwnerId = 24,
		DatabaseAccountId = 25,
		QuorumAckedLSN = 26,
		RequestValidationFailure = 27,
		CollectionUpdateProgress = 29,
		CurrentWriteQuorum = 30,
		CurrentReplicaSetSize = 31,
		CollectionLazyIndexProgress = 32,
		PartitionKeyRangeId = 33,
		LogResults = 37,
		XPRole = 38,
		IsRUPerMinuteUsed = 39,
		QueryMetrics = 40,
		NumberOfReadRegions = 48,
		OfferReplacePending = 49,
		RestoreState = 51,
		CollectionSecurityIdentifier = 52,
		ShareThroughput = 54,
		DisableRntbdChannel = 56,
		HasTentativeWrites = 61,
		ReplicatorLSNToGLSNDelta = 63,
		ReplicatorLSNToLLSNDelta = 64,
		VectorClockLocalProgress = 65,
		MinimumRUsForOffer = 66,
		XPConfigurationSessionsCount = 67,
		IndexUtilization = 68,
		QueryExecutionInfo = 69,
		UnflushedMergeLogEntryCount = 70,
		TimeToLiveInSeconds = 72,
		ReplicaStatusRevoked = 73,
		SoftMaxAllowedThroughput = 80,
		BackendRequestDurationMilliseconds = 81,
		CorrelatedActivityId = 82,
		ConfirmedStoreChecksum = 83,
		TentativeStoreChecksum = 84,
		PendingPKDelete = 85,
		AadAppliedRoleAssignmentId = 86,
		CollectionUniqueIndexReIndexProgress = 87,
		CollectionUniqueKeysUnderReIndex = 88,
		AnalyticalMigrationProgress = 89,
		TotalAccountThroughput = 90,
		BYOKEncryptionProgress = 91,
		AppliedPolicyElementId = 92,
		MergeProgressBlocked = 93,
		ChangeFeedInfo = 94,
		ReindexerProgress = 95,
		OfferReplacePendingForMerge = 96,
		MaxContentLength = 97,
		OldestActiveSchemaId = 98,
		PhysicalPartitionId = 99,
		OfferRestorePending = 100,
		InstantScaleUpValue = 101,
		RequiresDistribution = 102,
		CapacityType = 103,
		MinGLSNForDocumentOperations = 104,
		MinGLSNForTombstoneOperations = 105,
		HighestTentativeWriteLLSN = 112,
		PartitionThroughputInfo = 113,
		DocumentRecordCount = 114
	}

	public enum CallerId : byte
	{
		Anonymous,
		Gateway,
		BackgroundTask,
		ManagementWorker,
		Invalid
	}

	internal sealed class RntbdEntityPool<T, TU> where T : RntbdTokenStream<TU>, new() where TU : Enum
	{
		public readonly struct EntityOwner : IDisposable
		{
			public T Entity { get; }

			public EntityOwner(T entity)
			{
				Entity = entity;
			}

			public void Dispose()
			{
				if (Entity != null)
				{
					RntbdEntityPool<T, TU>.Instance.Return(Entity);
				}
			}
		}

		public static readonly RntbdEntityPool<T, TU> Instance = new RntbdEntityPool<T, TU>();

		private readonly ConcurrentQueue<T> entities = new ConcurrentQueue<T>();

		private RntbdEntityPool()
		{
		}

		public EntityOwner Get()
		{
			if (entities.TryDequeue(out var result))
			{
				return new EntityOwner(result);
			}
			return new EntityOwner(new T());
		}

		private void Return(T entity)
		{
			entity.Reset();
			entities.Enqueue(entity);
		}
	}

	public const uint CurrentProtocolVersion = 1u;
}
