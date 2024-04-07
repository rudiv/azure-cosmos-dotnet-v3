using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;

namespace Microsoft.Azure.Documents.Collections;

internal class StoreResponseNameValueCollection : INameValueCollection, IEnumerable, IEnumerable<KeyValuePair<string, string>>
{
	private static readonly StringComparer DefaultStringComparer = StringComparer.OrdinalIgnoreCase;

	private Dictionary<string, string> lazyNotCommonHeaders;

	private NameValueCollection nameValueCollection;

	public string AadAppliedRoleAssignmentId { get; set; }

	public string ActivityId { get; set; }

	public string AnalyticalMigrationProgress { get; set; }

	public string AppliedPolicyElementId { get; set; }

	public string BackendRequestDurationMilliseconds { get; set; }

	public string ByokEncryptionProgress { get; set; }

	public string CapacityType { get; set; }

	public string ChangeFeedInfo { get; set; }

	public string CollectionIndexTransformationProgress { get; set; }

	public string CollectionLazyIndexingProgress { get; set; }

	public string CollectionPartitionIndex { get; set; }

	public string CollectionSecurityIdentifier { get; set; }

	public string CollectionServiceIndex { get; set; }

	public string CollectionUniqueIndexReIndexProgress { get; set; }

	public string CollectionUniqueKeysUnderReIndex { get; set; }

	public string ConfirmedStoreChecksum { get; set; }

	public string Continuation { get; set; }

	public string CorrelatedActivityId { get; set; }

	public string CurrentReplicaSetSize { get; set; }

	public string CurrentResourceQuotaUsage { get; set; }

	public string CurrentWriteQuorum { get; set; }

	public string DatabaseAccountId { get; set; }

	public string DisableRntbdChannel { get; set; }

	public string DocumentRecordCount { get; set; }

	public string ETag { get; set; }

	public string GlobalCommittedLSN { get; set; }

	public string HasTentativeWrites { get; set; }

	public string HighestTentativeWriteLLSN { get; set; }

	public string IndexingDirective { get; set; }

	public string IndexUtilization { get; set; }

	public string InstantScaleUpValue { get; set; }

	public string IsOfferRestorePending { get; set; }

	public string IsRUPerMinuteUsed { get; set; }

	public string ItemCount { get; set; }

	public string ItemLocalLSN { get; set; }

	public string ItemLSN { get; set; }

	public string LastStateChangeUtc { get; set; }

	public string LocalLSN { get; set; }

	public string LogResults { get; set; }

	public string LSN { get; set; }

	public string MaxContentLength { get; set; }

	public string MaxResourceQuota { get; set; }

	public string MergeProgressBlocked { get; set; }

	public string MinGLSNForDocumentOperations { get; set; }

	public string MinGLSNForTombstoneOperations { get; set; }

	public string MinimumRUsForOffer { get; set; }

	public string NumberOfReadRegions { get; set; }

	public string OfferReplacePending { get; set; }

	public string OfferReplacePendingForMerge { get; set; }

	public string OldestActiveSchemaId { get; set; }

	public string OwnerFullName { get; set; }

	public string OwnerId { get; set; }

	public string PartitionKeyRangeId { get; set; }

	public string PartitionThroughputInfo { get; set; }

	public string PendingPKDelete { get; set; }

	public string PhysicalPartitionId { get; set; }

	public string QueryExecutionInfo { get; set; }

	public string QueryMetrics { get; set; }

	public string QuorumAckedLocalLSN { get; set; }

	public string QuorumAckedLSN { get; set; }

	public string ReIndexerProgress { get; set; }

	public string ReplicaStatusRevoked { get; set; }

	public string ReplicatorLSNToGLSNDelta { get; set; }

	public string ReplicatorLSNToLLSNDelta { get; set; }

	public string RequestCharge { get; set; }

	public string RequestValidationFailure { get; set; }

	public string RequiresDistribution { get; set; }

	public string ResourceId { get; set; }

	public string RestoreState { get; set; }

	public string RetryAfterInMilliseconds { get; set; }

	public string SchemaVersion { get; set; }

	public string ServerVersion { get; set; }

	public string SessionToken { get; set; }

	public string ShareThroughput { get; set; }

	public string SoftMaxAllowedThroughput { get; set; }

	public string SubStatus { get; set; }

	public string TentativeStoreChecksum { get; set; }

	public string TimeToLiveInSeconds { get; set; }

	public string TotalAccountThroughput { get; set; }

	public string TransportRequestID { get; set; }

	public string UnflushedMergLogEntryCount { get; set; }

	public string VectorClockLocalProgress { get; set; }

	public string XDate { get; set; }

	public string XPConfigurationSessionsCount { get; set; }

	public string XPRole { get; set; }

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

	public StoreResponseNameValueCollection()
	{
	}

	private StoreResponseNameValueCollection(Dictionary<string, string> lazyNotCommonHeaders)
	{
		this.lazyNotCommonHeaders = lazyNotCommonHeaders;
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
		if (lazyNotCommonHeaders != null)
		{
			lazyNotCommonHeaders.Clear();
		}
		AadAppliedRoleAssignmentId = null;
		ActivityId = null;
		AnalyticalMigrationProgress = null;
		AppliedPolicyElementId = null;
		BackendRequestDurationMilliseconds = null;
		ByokEncryptionProgress = null;
		CapacityType = null;
		ChangeFeedInfo = null;
		CollectionIndexTransformationProgress = null;
		CollectionLazyIndexingProgress = null;
		CollectionPartitionIndex = null;
		CollectionSecurityIdentifier = null;
		CollectionServiceIndex = null;
		CollectionUniqueIndexReIndexProgress = null;
		CollectionUniqueKeysUnderReIndex = null;
		ConfirmedStoreChecksum = null;
		Continuation = null;
		CorrelatedActivityId = null;
		CurrentReplicaSetSize = null;
		CurrentResourceQuotaUsage = null;
		CurrentWriteQuorum = null;
		DatabaseAccountId = null;
		DisableRntbdChannel = null;
		DocumentRecordCount = null;
		ETag = null;
		GlobalCommittedLSN = null;
		HasTentativeWrites = null;
		HighestTentativeWriteLLSN = null;
		IndexingDirective = null;
		IndexUtilization = null;
		InstantScaleUpValue = null;
		IsOfferRestorePending = null;
		IsRUPerMinuteUsed = null;
		ItemCount = null;
		ItemLocalLSN = null;
		ItemLSN = null;
		LastStateChangeUtc = null;
		LocalLSN = null;
		LogResults = null;
		LSN = null;
		MaxContentLength = null;
		MaxResourceQuota = null;
		MergeProgressBlocked = null;
		MinGLSNForDocumentOperations = null;
		MinGLSNForTombstoneOperations = null;
		MinimumRUsForOffer = null;
		NumberOfReadRegions = null;
		OfferReplacePending = null;
		OfferReplacePendingForMerge = null;
		OldestActiveSchemaId = null;
		OwnerFullName = null;
		OwnerId = null;
		PartitionKeyRangeId = null;
		PartitionThroughputInfo = null;
		PendingPKDelete = null;
		PhysicalPartitionId = null;
		QueryExecutionInfo = null;
		QueryMetrics = null;
		QuorumAckedLocalLSN = null;
		QuorumAckedLSN = null;
		ReIndexerProgress = null;
		ReplicaStatusRevoked = null;
		ReplicatorLSNToGLSNDelta = null;
		ReplicatorLSNToLLSNDelta = null;
		RequestCharge = null;
		RequestValidationFailure = null;
		RequiresDistribution = null;
		ResourceId = null;
		RestoreState = null;
		RetryAfterInMilliseconds = null;
		SchemaVersion = null;
		ServerVersion = null;
		SessionToken = null;
		ShareThroughput = null;
		SoftMaxAllowedThroughput = null;
		SubStatus = null;
		TentativeStoreChecksum = null;
		TimeToLiveInSeconds = null;
		TotalAccountThroughput = null;
		TransportRequestID = null;
		UnflushedMergLogEntryCount = null;
		VectorClockLocalProgress = null;
		XDate = null;
		XPConfigurationSessionsCount = null;
		XPRole = null;
	}

	public INameValueCollection Clone()
	{
		Dictionary<string, string> dictionary = null;
		if (lazyNotCommonHeaders != null)
		{
			dictionary = new Dictionary<string, string>(lazyNotCommonHeaders, DefaultStringComparer);
		}
		return new StoreResponseNameValueCollection(dictionary)
		{
			AadAppliedRoleAssignmentId = AadAppliedRoleAssignmentId,
			ActivityId = ActivityId,
			AnalyticalMigrationProgress = AnalyticalMigrationProgress,
			AppliedPolicyElementId = AppliedPolicyElementId,
			BackendRequestDurationMilliseconds = BackendRequestDurationMilliseconds,
			ByokEncryptionProgress = ByokEncryptionProgress,
			CapacityType = CapacityType,
			ChangeFeedInfo = ChangeFeedInfo,
			CollectionIndexTransformationProgress = CollectionIndexTransformationProgress,
			CollectionLazyIndexingProgress = CollectionLazyIndexingProgress,
			CollectionPartitionIndex = CollectionPartitionIndex,
			CollectionSecurityIdentifier = CollectionSecurityIdentifier,
			CollectionServiceIndex = CollectionServiceIndex,
			CollectionUniqueIndexReIndexProgress = CollectionUniqueIndexReIndexProgress,
			CollectionUniqueKeysUnderReIndex = CollectionUniqueKeysUnderReIndex,
			ConfirmedStoreChecksum = ConfirmedStoreChecksum,
			Continuation = Continuation,
			CorrelatedActivityId = CorrelatedActivityId,
			CurrentReplicaSetSize = CurrentReplicaSetSize,
			CurrentResourceQuotaUsage = CurrentResourceQuotaUsage,
			CurrentWriteQuorum = CurrentWriteQuorum,
			DatabaseAccountId = DatabaseAccountId,
			DisableRntbdChannel = DisableRntbdChannel,
			DocumentRecordCount = DocumentRecordCount,
			ETag = ETag,
			GlobalCommittedLSN = GlobalCommittedLSN,
			HasTentativeWrites = HasTentativeWrites,
			HighestTentativeWriteLLSN = HighestTentativeWriteLLSN,
			IndexingDirective = IndexingDirective,
			IndexUtilization = IndexUtilization,
			InstantScaleUpValue = InstantScaleUpValue,
			IsOfferRestorePending = IsOfferRestorePending,
			IsRUPerMinuteUsed = IsRUPerMinuteUsed,
			ItemCount = ItemCount,
			ItemLocalLSN = ItemLocalLSN,
			ItemLSN = ItemLSN,
			LastStateChangeUtc = LastStateChangeUtc,
			LocalLSN = LocalLSN,
			LogResults = LogResults,
			LSN = LSN,
			MaxContentLength = MaxContentLength,
			MaxResourceQuota = MaxResourceQuota,
			MergeProgressBlocked = MergeProgressBlocked,
			MinGLSNForDocumentOperations = MinGLSNForDocumentOperations,
			MinGLSNForTombstoneOperations = MinGLSNForTombstoneOperations,
			MinimumRUsForOffer = MinimumRUsForOffer,
			NumberOfReadRegions = NumberOfReadRegions,
			OfferReplacePending = OfferReplacePending,
			OfferReplacePendingForMerge = OfferReplacePendingForMerge,
			OldestActiveSchemaId = OldestActiveSchemaId,
			OwnerFullName = OwnerFullName,
			OwnerId = OwnerId,
			PartitionKeyRangeId = PartitionKeyRangeId,
			PartitionThroughputInfo = PartitionThroughputInfo,
			PendingPKDelete = PendingPKDelete,
			PhysicalPartitionId = PhysicalPartitionId,
			QueryExecutionInfo = QueryExecutionInfo,
			QueryMetrics = QueryMetrics,
			QuorumAckedLocalLSN = QuorumAckedLocalLSN,
			QuorumAckedLSN = QuorumAckedLSN,
			ReIndexerProgress = ReIndexerProgress,
			ReplicaStatusRevoked = ReplicaStatusRevoked,
			ReplicatorLSNToGLSNDelta = ReplicatorLSNToGLSNDelta,
			ReplicatorLSNToLLSNDelta = ReplicatorLSNToLLSNDelta,
			RequestCharge = RequestCharge,
			RequestValidationFailure = RequestValidationFailure,
			RequiresDistribution = RequiresDistribution,
			ResourceId = ResourceId,
			RestoreState = RestoreState,
			RetryAfterInMilliseconds = RetryAfterInMilliseconds,
			SchemaVersion = SchemaVersion,
			ServerVersion = ServerVersion,
			SessionToken = SessionToken,
			ShareThroughput = ShareThroughput,
			SoftMaxAllowedThroughput = SoftMaxAllowedThroughput,
			SubStatus = SubStatus,
			TentativeStoreChecksum = TentativeStoreChecksum,
			TimeToLiveInSeconds = TimeToLiveInSeconds,
			TotalAccountThroughput = TotalAccountThroughput,
			TransportRequestID = TransportRequestID,
			UnflushedMergLogEntryCount = UnflushedMergLogEntryCount,
			VectorClockLocalProgress = VectorClockLocalProgress,
			XDate = XDate,
			XPConfigurationSessionsCount = XPConfigurationSessionsCount,
			XPRole = XPRole
		};
	}

	public int Count()
	{
		return Keys().Count();
	}

	public IEnumerator GetEnumerator()
	{
		return Keys().GetEnumerator();
	}

	IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
	{
		if (ActivityId != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-activity-id", ActivityId);
		}
		if (LastStateChangeUtc != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-last-state-change-utc", LastStateChangeUtc);
		}
		if (Continuation != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-continuation", Continuation);
		}
		if (ETag != null)
		{
			yield return new KeyValuePair<string, string>("etag", ETag);
		}
		if (RetryAfterInMilliseconds != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-retry-after-ms", RetryAfterInMilliseconds);
		}
		if (IndexingDirective != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-indexing-directive", IndexingDirective);
		}
		if (MaxResourceQuota != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-resource-quota", MaxResourceQuota);
		}
		if (CurrentResourceQuotaUsage != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-resource-usage", CurrentResourceQuotaUsage);
		}
		if (SchemaVersion != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-schemaversion", SchemaVersion);
		}
		if (CollectionPartitionIndex != null)
		{
			yield return new KeyValuePair<string, string>("collection-partition-index", CollectionPartitionIndex);
		}
		if (CollectionServiceIndex != null)
		{
			yield return new KeyValuePair<string, string>("collection-service-index", CollectionServiceIndex);
		}
		if (LSN != null)
		{
			yield return new KeyValuePair<string, string>("lsn", LSN);
		}
		if (ItemCount != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-item-count", ItemCount);
		}
		if (RequestCharge != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-request-charge", RequestCharge);
		}
		if (OwnerFullName != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-alt-content-path", OwnerFullName);
		}
		if (OwnerId != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-content-path", OwnerId);
		}
		if (DatabaseAccountId != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-database-account-id", DatabaseAccountId);
		}
		if (QuorumAckedLSN != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-quorum-acked-lsn", QuorumAckedLSN);
		}
		if (RequestValidationFailure != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-request-validation-failure", RequestValidationFailure);
		}
		if (SubStatus != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-substatus", SubStatus);
		}
		if (CollectionIndexTransformationProgress != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-documentdb-collection-index-transformation-progress", CollectionIndexTransformationProgress);
		}
		if (CurrentWriteQuorum != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-current-write-quorum", CurrentWriteQuorum);
		}
		if (CurrentReplicaSetSize != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-current-replica-set-size", CurrentReplicaSetSize);
		}
		if (CollectionLazyIndexingProgress != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-documentdb-collection-lazy-indexing-progress", CollectionLazyIndexingProgress);
		}
		if (PartitionKeyRangeId != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-documentdb-partitionkeyrangeid", PartitionKeyRangeId);
		}
		if (LogResults != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-documentdb-script-log-results", LogResults);
		}
		if (XPRole != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-xp-role", XPRole);
		}
		if (IsRUPerMinuteUsed != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-documentdb-is-ru-per-minute-used", IsRUPerMinuteUsed);
		}
		if (QueryMetrics != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-documentdb-query-metrics", QueryMetrics);
		}
		if (QueryExecutionInfo != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-query-execution-info", QueryExecutionInfo);
		}
		if (IndexUtilization != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-index-utilization", IndexUtilization);
		}
		if (GlobalCommittedLSN != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-global-Committed-lsn", GlobalCommittedLSN);
		}
		if (NumberOfReadRegions != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-number-of-read-regions", NumberOfReadRegions);
		}
		if (OfferReplacePending != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-offer-replace-pending", OfferReplacePending);
		}
		if (ItemLSN != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-item-lsn", ItemLSN);
		}
		if (RestoreState != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-restore-state", RestoreState);
		}
		if (CollectionSecurityIdentifier != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-collection-security-identifier", CollectionSecurityIdentifier);
		}
		if (TransportRequestID != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-transport-request-id", TransportRequestID);
		}
		if (ShareThroughput != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-share-throughput", ShareThroughput);
		}
		if (DisableRntbdChannel != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-disable-rntbd-channel", DisableRntbdChannel);
		}
		if (XDate != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-date", XDate);
		}
		if (LocalLSN != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-llsn", LocalLSN);
		}
		if (QuorumAckedLocalLSN != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-quorum-acked-llsn", QuorumAckedLocalLSN);
		}
		if (ItemLocalLSN != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-item-llsn", ItemLocalLSN);
		}
		if (HasTentativeWrites != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmosdb-has-tentative-writes", HasTentativeWrites);
		}
		if (SessionToken != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-session-token", SessionToken);
		}
		if (ReplicatorLSNToGLSNDelta != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-replicator-glsn-delta", ReplicatorLSNToGLSNDelta);
		}
		if (ReplicatorLSNToLLSNDelta != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-replicator-llsn-delta", ReplicatorLSNToLLSNDelta);
		}
		if (VectorClockLocalProgress != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-vectorclock-local-progress", VectorClockLocalProgress);
		}
		if (MinimumRUsForOffer != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-min-throughput", MinimumRUsForOffer);
		}
		if (XPConfigurationSessionsCount != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-xpconfiguration-sessions-count", XPConfigurationSessionsCount);
		}
		if (UnflushedMergLogEntryCount != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-internal-unflushed-merge-log-entry-count", UnflushedMergLogEntryCount);
		}
		if (ResourceId != null)
		{
			yield return new KeyValuePair<string, string>("x-docdb-resource-id", ResourceId);
		}
		if (TimeToLiveInSeconds != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-time-to-live-in-seconds", TimeToLiveInSeconds);
		}
		if (ReplicaStatusRevoked != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-is-replica-status-revoked", ReplicaStatusRevoked);
		}
		if (SoftMaxAllowedThroughput != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-offer-max-allowed-throughput", SoftMaxAllowedThroughput);
		}
		if (BackendRequestDurationMilliseconds != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-request-duration-ms", BackendRequestDurationMilliseconds);
		}
		if (ServerVersion != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-serviceversion", ServerVersion);
		}
		if (ConfirmedStoreChecksum != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-replica-confirmed-checksum", ConfirmedStoreChecksum);
		}
		if (TentativeStoreChecksum != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-replica-tentative-checksum", TentativeStoreChecksum);
		}
		if (CorrelatedActivityId != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-correlated-activityid", CorrelatedActivityId);
		}
		if (PendingPKDelete != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-is-partition-key-delete-pending", PendingPKDelete);
		}
		if (AadAppliedRoleAssignmentId != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-aad-applied-role-assignment", AadAppliedRoleAssignmentId);
		}
		if (CollectionUniqueIndexReIndexProgress != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-collection-unique-index-reindex-progress", CollectionUniqueIndexReIndexProgress);
		}
		if (CollectionUniqueKeysUnderReIndex != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-collection-unique-keys-under-reindex", CollectionUniqueKeysUnderReIndex);
		}
		if (AnalyticalMigrationProgress != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-analytical-migration-progress", AnalyticalMigrationProgress);
		}
		if (TotalAccountThroughput != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-total-account-throughput", TotalAccountThroughput);
		}
		if (ByokEncryptionProgress != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-byok-encryption-progress", ByokEncryptionProgress);
		}
		if (AppliedPolicyElementId != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-applied-policy-element", AppliedPolicyElementId);
		}
		if (MergeProgressBlocked != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-is-merge-progress-blocked", MergeProgressBlocked);
		}
		if (ChangeFeedInfo != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-changefeed-info", ChangeFeedInfo);
		}
		if (ReIndexerProgress != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-reindexer-progress", ReIndexerProgress);
		}
		if (OfferReplacePendingForMerge != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-offer-replace-pending-for-merge", OfferReplacePendingForMerge);
		}
		if (OldestActiveSchemaId != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-oldest-active-schema-id", OldestActiveSchemaId);
		}
		if (PhysicalPartitionId != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-physical-partition-id", PhysicalPartitionId);
		}
		if (MaxContentLength != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-max-content-length", MaxContentLength);
		}
		if (IsOfferRestorePending != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-offer-restore-pending", IsOfferRestorePending);
		}
		if (InstantScaleUpValue != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-instant-scale-up-value", InstantScaleUpValue);
		}
		if (RequiresDistribution != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-query-requiresdistribution", RequiresDistribution);
		}
		if (CapacityType != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-capacity-type", CapacityType);
		}
		if (MinGLSNForTombstoneOperations != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-min-tombstone-glsn", MinGLSNForTombstoneOperations);
		}
		if (MinGLSNForDocumentOperations != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-min-document-glsn", MinGLSNForDocumentOperations);
		}
		if (HighestTentativeWriteLLSN != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-highest-tentative-write-llsn", HighestTentativeWriteLLSN);
		}
		if (PartitionThroughputInfo != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-partition-throughput-info", PartitionThroughputInfo);
		}
		if (DocumentRecordCount != null)
		{
			yield return new KeyValuePair<string, string>("x-ms-cosmos-document-record-count", DocumentRecordCount);
		}
		if (lazyNotCommonHeaders == null)
		{
			yield break;
		}
		foreach (KeyValuePair<string, string> lazyNotCommonHeader in lazyNotCommonHeaders)
		{
			yield return lazyNotCommonHeader;
		}
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
		if (ActivityId != null)
		{
			yield return "x-ms-activity-id";
		}
		if (LastStateChangeUtc != null)
		{
			yield return "x-ms-last-state-change-utc";
		}
		if (Continuation != null)
		{
			yield return "x-ms-continuation";
		}
		if (ETag != null)
		{
			yield return "etag";
		}
		if (RetryAfterInMilliseconds != null)
		{
			yield return "x-ms-retry-after-ms";
		}
		if (IndexingDirective != null)
		{
			yield return "x-ms-indexing-directive";
		}
		if (MaxResourceQuota != null)
		{
			yield return "x-ms-resource-quota";
		}
		if (CurrentResourceQuotaUsage != null)
		{
			yield return "x-ms-resource-usage";
		}
		if (SchemaVersion != null)
		{
			yield return "x-ms-schemaversion";
		}
		if (CollectionPartitionIndex != null)
		{
			yield return "collection-partition-index";
		}
		if (CollectionServiceIndex != null)
		{
			yield return "collection-service-index";
		}
		if (LSN != null)
		{
			yield return "lsn";
		}
		if (ItemCount != null)
		{
			yield return "x-ms-item-count";
		}
		if (RequestCharge != null)
		{
			yield return "x-ms-request-charge";
		}
		if (OwnerFullName != null)
		{
			yield return "x-ms-alt-content-path";
		}
		if (OwnerId != null)
		{
			yield return "x-ms-content-path";
		}
		if (DatabaseAccountId != null)
		{
			yield return "x-ms-database-account-id";
		}
		if (QuorumAckedLSN != null)
		{
			yield return "x-ms-quorum-acked-lsn";
		}
		if (RequestValidationFailure != null)
		{
			yield return "x-ms-request-validation-failure";
		}
		if (SubStatus != null)
		{
			yield return "x-ms-substatus";
		}
		if (CollectionIndexTransformationProgress != null)
		{
			yield return "x-ms-documentdb-collection-index-transformation-progress";
		}
		if (CurrentWriteQuorum != null)
		{
			yield return "x-ms-current-write-quorum";
		}
		if (CurrentReplicaSetSize != null)
		{
			yield return "x-ms-current-replica-set-size";
		}
		if (CollectionLazyIndexingProgress != null)
		{
			yield return "x-ms-documentdb-collection-lazy-indexing-progress";
		}
		if (PartitionKeyRangeId != null)
		{
			yield return "x-ms-documentdb-partitionkeyrangeid";
		}
		if (LogResults != null)
		{
			yield return "x-ms-documentdb-script-log-results";
		}
		if (XPRole != null)
		{
			yield return "x-ms-xp-role";
		}
		if (IsRUPerMinuteUsed != null)
		{
			yield return "x-ms-documentdb-is-ru-per-minute-used";
		}
		if (QueryMetrics != null)
		{
			yield return "x-ms-documentdb-query-metrics";
		}
		if (QueryExecutionInfo != null)
		{
			yield return "x-ms-cosmos-query-execution-info";
		}
		if (IndexUtilization != null)
		{
			yield return "x-ms-cosmos-index-utilization";
		}
		if (GlobalCommittedLSN != null)
		{
			yield return "x-ms-global-Committed-lsn";
		}
		if (NumberOfReadRegions != null)
		{
			yield return "x-ms-number-of-read-regions";
		}
		if (OfferReplacePending != null)
		{
			yield return "x-ms-offer-replace-pending";
		}
		if (ItemLSN != null)
		{
			yield return "x-ms-item-lsn";
		}
		if (RestoreState != null)
		{
			yield return "x-ms-restore-state";
		}
		if (CollectionSecurityIdentifier != null)
		{
			yield return "x-ms-collection-security-identifier";
		}
		if (TransportRequestID != null)
		{
			yield return "x-ms-transport-request-id";
		}
		if (ShareThroughput != null)
		{
			yield return "x-ms-share-throughput";
		}
		if (DisableRntbdChannel != null)
		{
			yield return "x-ms-disable-rntbd-channel";
		}
		if (XDate != null)
		{
			yield return "x-ms-date";
		}
		if (LocalLSN != null)
		{
			yield return "x-ms-cosmos-llsn";
		}
		if (QuorumAckedLocalLSN != null)
		{
			yield return "x-ms-cosmos-quorum-acked-llsn";
		}
		if (ItemLocalLSN != null)
		{
			yield return "x-ms-cosmos-item-llsn";
		}
		if (HasTentativeWrites != null)
		{
			yield return "x-ms-cosmosdb-has-tentative-writes";
		}
		if (SessionToken != null)
		{
			yield return "x-ms-session-token";
		}
		if (ReplicatorLSNToGLSNDelta != null)
		{
			yield return "x-ms-cosmos-replicator-glsn-delta";
		}
		if (ReplicatorLSNToLLSNDelta != null)
		{
			yield return "x-ms-cosmos-replicator-llsn-delta";
		}
		if (VectorClockLocalProgress != null)
		{
			yield return "x-ms-cosmos-vectorclock-local-progress";
		}
		if (MinimumRUsForOffer != null)
		{
			yield return "x-ms-cosmos-min-throughput";
		}
		if (XPConfigurationSessionsCount != null)
		{
			yield return "x-ms-cosmos-xpconfiguration-sessions-count";
		}
		if (UnflushedMergLogEntryCount != null)
		{
			yield return "x-ms-cosmos-internal-unflushed-merge-log-entry-count";
		}
		if (ResourceId != null)
		{
			yield return "x-docdb-resource-id";
		}
		if (TimeToLiveInSeconds != null)
		{
			yield return "x-ms-time-to-live-in-seconds";
		}
		if (ReplicaStatusRevoked != null)
		{
			yield return "x-ms-cosmos-is-replica-status-revoked";
		}
		if (SoftMaxAllowedThroughput != null)
		{
			yield return "x-ms-cosmos-offer-max-allowed-throughput";
		}
		if (BackendRequestDurationMilliseconds != null)
		{
			yield return "x-ms-request-duration-ms";
		}
		if (ServerVersion != null)
		{
			yield return "x-ms-serviceversion";
		}
		if (ConfirmedStoreChecksum != null)
		{
			yield return "x-ms-cosmos-replica-confirmed-checksum";
		}
		if (TentativeStoreChecksum != null)
		{
			yield return "x-ms-cosmos-replica-tentative-checksum";
		}
		if (CorrelatedActivityId != null)
		{
			yield return "x-ms-cosmos-correlated-activityid";
		}
		if (PendingPKDelete != null)
		{
			yield return "x-ms-cosmos-is-partition-key-delete-pending";
		}
		if (AadAppliedRoleAssignmentId != null)
		{
			yield return "x-ms-aad-applied-role-assignment";
		}
		if (CollectionUniqueIndexReIndexProgress != null)
		{
			yield return "x-ms-cosmos-collection-unique-index-reindex-progress";
		}
		if (CollectionUniqueKeysUnderReIndex != null)
		{
			yield return "x-ms-cosmos-collection-unique-keys-under-reindex";
		}
		if (AnalyticalMigrationProgress != null)
		{
			yield return "x-ms-cosmos-analytical-migration-progress";
		}
		if (TotalAccountThroughput != null)
		{
			yield return "x-ms-cosmos-total-account-throughput";
		}
		if (ByokEncryptionProgress != null)
		{
			yield return "x-ms-cosmos-byok-encryption-progress";
		}
		if (AppliedPolicyElementId != null)
		{
			yield return "x-ms-applied-policy-element";
		}
		if (MergeProgressBlocked != null)
		{
			yield return "x-ms-cosmos-is-merge-progress-blocked";
		}
		if (ChangeFeedInfo != null)
		{
			yield return "x-ms-cosmos-changefeed-info";
		}
		if (ReIndexerProgress != null)
		{
			yield return "x-ms-cosmos-reindexer-progress";
		}
		if (OfferReplacePendingForMerge != null)
		{
			yield return "x-ms-offer-replace-pending-for-merge";
		}
		if (OldestActiveSchemaId != null)
		{
			yield return "x-ms-cosmos-oldest-active-schema-id";
		}
		if (PhysicalPartitionId != null)
		{
			yield return "x-ms-cosmos-physical-partition-id";
		}
		if (MaxContentLength != null)
		{
			yield return "x-ms-cosmos-max-content-length";
		}
		if (IsOfferRestorePending != null)
		{
			yield return "x-ms-offer-restore-pending";
		}
		if (InstantScaleUpValue != null)
		{
			yield return "x-ms-cosmos-instant-scale-up-value";
		}
		if (RequiresDistribution != null)
		{
			yield return "x-ms-cosmos-query-requiresdistribution";
		}
		if (CapacityType != null)
		{
			yield return "x-ms-cosmos-capacity-type";
		}
		if (MinGLSNForTombstoneOperations != null)
		{
			yield return "x-ms-cosmos-min-tombstone-glsn";
		}
		if (MinGLSNForDocumentOperations != null)
		{
			yield return "x-ms-cosmos-min-document-glsn";
		}
		if (HighestTentativeWriteLLSN != null)
		{
			yield return "x-ms-cosmos-highest-tentative-write-llsn";
		}
		if (PartitionThroughputInfo != null)
		{
			yield return "x-ms-cosmos-partition-throughput-info";
		}
		if (DocumentRecordCount != null)
		{
			yield return "x-ms-cosmos-document-record-count";
		}
		if (lazyNotCommonHeaders == null)
		{
			yield break;
		}
		foreach (string key in lazyNotCommonHeaders.Keys)
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
					if (ActivityId != null)
					{
						nameValueCollection.Add("x-ms-activity-id", ActivityId);
					}
					if (LastStateChangeUtc != null)
					{
						nameValueCollection.Add("x-ms-last-state-change-utc", LastStateChangeUtc);
					}
					if (Continuation != null)
					{
						nameValueCollection.Add("x-ms-continuation", Continuation);
					}
					if (ETag != null)
					{
						nameValueCollection.Add("etag", ETag);
					}
					if (RetryAfterInMilliseconds != null)
					{
						nameValueCollection.Add("x-ms-retry-after-ms", RetryAfterInMilliseconds);
					}
					if (IndexingDirective != null)
					{
						nameValueCollection.Add("x-ms-indexing-directive", IndexingDirective);
					}
					if (MaxResourceQuota != null)
					{
						nameValueCollection.Add("x-ms-resource-quota", MaxResourceQuota);
					}
					if (CurrentResourceQuotaUsage != null)
					{
						nameValueCollection.Add("x-ms-resource-usage", CurrentResourceQuotaUsage);
					}
					if (SchemaVersion != null)
					{
						nameValueCollection.Add("x-ms-schemaversion", SchemaVersion);
					}
					if (CollectionPartitionIndex != null)
					{
						nameValueCollection.Add("collection-partition-index", CollectionPartitionIndex);
					}
					if (CollectionServiceIndex != null)
					{
						nameValueCollection.Add("collection-service-index", CollectionServiceIndex);
					}
					if (LSN != null)
					{
						nameValueCollection.Add("lsn", LSN);
					}
					if (ItemCount != null)
					{
						nameValueCollection.Add("x-ms-item-count", ItemCount);
					}
					if (RequestCharge != null)
					{
						nameValueCollection.Add("x-ms-request-charge", RequestCharge);
					}
					if (OwnerFullName != null)
					{
						nameValueCollection.Add("x-ms-alt-content-path", OwnerFullName);
					}
					if (OwnerId != null)
					{
						nameValueCollection.Add("x-ms-content-path", OwnerId);
					}
					if (DatabaseAccountId != null)
					{
						nameValueCollection.Add("x-ms-database-account-id", DatabaseAccountId);
					}
					if (QuorumAckedLSN != null)
					{
						nameValueCollection.Add("x-ms-quorum-acked-lsn", QuorumAckedLSN);
					}
					if (RequestValidationFailure != null)
					{
						nameValueCollection.Add("x-ms-request-validation-failure", RequestValidationFailure);
					}
					if (SubStatus != null)
					{
						nameValueCollection.Add("x-ms-substatus", SubStatus);
					}
					if (CollectionIndexTransformationProgress != null)
					{
						nameValueCollection.Add("x-ms-documentdb-collection-index-transformation-progress", CollectionIndexTransformationProgress);
					}
					if (CurrentWriteQuorum != null)
					{
						nameValueCollection.Add("x-ms-current-write-quorum", CurrentWriteQuorum);
					}
					if (CurrentReplicaSetSize != null)
					{
						nameValueCollection.Add("x-ms-current-replica-set-size", CurrentReplicaSetSize);
					}
					if (CollectionLazyIndexingProgress != null)
					{
						nameValueCollection.Add("x-ms-documentdb-collection-lazy-indexing-progress", CollectionLazyIndexingProgress);
					}
					if (PartitionKeyRangeId != null)
					{
						nameValueCollection.Add("x-ms-documentdb-partitionkeyrangeid", PartitionKeyRangeId);
					}
					if (LogResults != null)
					{
						nameValueCollection.Add("x-ms-documentdb-script-log-results", LogResults);
					}
					if (XPRole != null)
					{
						nameValueCollection.Add("x-ms-xp-role", XPRole);
					}
					if (IsRUPerMinuteUsed != null)
					{
						nameValueCollection.Add("x-ms-documentdb-is-ru-per-minute-used", IsRUPerMinuteUsed);
					}
					if (QueryMetrics != null)
					{
						nameValueCollection.Add("x-ms-documentdb-query-metrics", QueryMetrics);
					}
					if (QueryExecutionInfo != null)
					{
						nameValueCollection.Add("x-ms-cosmos-query-execution-info", QueryExecutionInfo);
					}
					if (IndexUtilization != null)
					{
						nameValueCollection.Add("x-ms-cosmos-index-utilization", IndexUtilization);
					}
					if (GlobalCommittedLSN != null)
					{
						nameValueCollection.Add("x-ms-global-Committed-lsn", GlobalCommittedLSN);
					}
					if (NumberOfReadRegions != null)
					{
						nameValueCollection.Add("x-ms-number-of-read-regions", NumberOfReadRegions);
					}
					if (OfferReplacePending != null)
					{
						nameValueCollection.Add("x-ms-offer-replace-pending", OfferReplacePending);
					}
					if (ItemLSN != null)
					{
						nameValueCollection.Add("x-ms-item-lsn", ItemLSN);
					}
					if (RestoreState != null)
					{
						nameValueCollection.Add("x-ms-restore-state", RestoreState);
					}
					if (CollectionSecurityIdentifier != null)
					{
						nameValueCollection.Add("x-ms-collection-security-identifier", CollectionSecurityIdentifier);
					}
					if (TransportRequestID != null)
					{
						nameValueCollection.Add("x-ms-transport-request-id", TransportRequestID);
					}
					if (ShareThroughput != null)
					{
						nameValueCollection.Add("x-ms-share-throughput", ShareThroughput);
					}
					if (DisableRntbdChannel != null)
					{
						nameValueCollection.Add("x-ms-disable-rntbd-channel", DisableRntbdChannel);
					}
					if (XDate != null)
					{
						nameValueCollection.Add("x-ms-date", XDate);
					}
					if (LocalLSN != null)
					{
						nameValueCollection.Add("x-ms-cosmos-llsn", LocalLSN);
					}
					if (QuorumAckedLocalLSN != null)
					{
						nameValueCollection.Add("x-ms-cosmos-quorum-acked-llsn", QuorumAckedLocalLSN);
					}
					if (ItemLocalLSN != null)
					{
						nameValueCollection.Add("x-ms-cosmos-item-llsn", ItemLocalLSN);
					}
					if (HasTentativeWrites != null)
					{
						nameValueCollection.Add("x-ms-cosmosdb-has-tentative-writes", HasTentativeWrites);
					}
					if (SessionToken != null)
					{
						nameValueCollection.Add("x-ms-session-token", SessionToken);
					}
					if (ReplicatorLSNToGLSNDelta != null)
					{
						nameValueCollection.Add("x-ms-cosmos-replicator-glsn-delta", ReplicatorLSNToGLSNDelta);
					}
					if (ReplicatorLSNToLLSNDelta != null)
					{
						nameValueCollection.Add("x-ms-cosmos-replicator-llsn-delta", ReplicatorLSNToLLSNDelta);
					}
					if (VectorClockLocalProgress != null)
					{
						nameValueCollection.Add("x-ms-cosmos-vectorclock-local-progress", VectorClockLocalProgress);
					}
					if (MinimumRUsForOffer != null)
					{
						nameValueCollection.Add("x-ms-cosmos-min-throughput", MinimumRUsForOffer);
					}
					if (XPConfigurationSessionsCount != null)
					{
						nameValueCollection.Add("x-ms-cosmos-xpconfiguration-sessions-count", XPConfigurationSessionsCount);
					}
					if (UnflushedMergLogEntryCount != null)
					{
						nameValueCollection.Add("x-ms-cosmos-internal-unflushed-merge-log-entry-count", UnflushedMergLogEntryCount);
					}
					if (ResourceId != null)
					{
						nameValueCollection.Add("x-docdb-resource-id", ResourceId);
					}
					if (TimeToLiveInSeconds != null)
					{
						nameValueCollection.Add("x-ms-time-to-live-in-seconds", TimeToLiveInSeconds);
					}
					if (ReplicaStatusRevoked != null)
					{
						nameValueCollection.Add("x-ms-cosmos-is-replica-status-revoked", ReplicaStatusRevoked);
					}
					if (SoftMaxAllowedThroughput != null)
					{
						nameValueCollection.Add("x-ms-cosmos-offer-max-allowed-throughput", SoftMaxAllowedThroughput);
					}
					if (BackendRequestDurationMilliseconds != null)
					{
						nameValueCollection.Add("x-ms-request-duration-ms", BackendRequestDurationMilliseconds);
					}
					if (ServerVersion != null)
					{
						nameValueCollection.Add("x-ms-serviceversion", ServerVersion);
					}
					if (ConfirmedStoreChecksum != null)
					{
						nameValueCollection.Add("x-ms-cosmos-replica-confirmed-checksum", ConfirmedStoreChecksum);
					}
					if (TentativeStoreChecksum != null)
					{
						nameValueCollection.Add("x-ms-cosmos-replica-tentative-checksum", TentativeStoreChecksum);
					}
					if (CorrelatedActivityId != null)
					{
						nameValueCollection.Add("x-ms-cosmos-correlated-activityid", CorrelatedActivityId);
					}
					if (PendingPKDelete != null)
					{
						nameValueCollection.Add("x-ms-cosmos-is-partition-key-delete-pending", PendingPKDelete);
					}
					if (AadAppliedRoleAssignmentId != null)
					{
						nameValueCollection.Add("x-ms-aad-applied-role-assignment", AadAppliedRoleAssignmentId);
					}
					if (CollectionUniqueIndexReIndexProgress != null)
					{
						nameValueCollection.Add("x-ms-cosmos-collection-unique-index-reindex-progress", CollectionUniqueIndexReIndexProgress);
					}
					if (CollectionUniqueKeysUnderReIndex != null)
					{
						nameValueCollection.Add("x-ms-cosmos-collection-unique-keys-under-reindex", CollectionUniqueKeysUnderReIndex);
					}
					if (AnalyticalMigrationProgress != null)
					{
						nameValueCollection.Add("x-ms-cosmos-analytical-migration-progress", AnalyticalMigrationProgress);
					}
					if (TotalAccountThroughput != null)
					{
						nameValueCollection.Add("x-ms-cosmos-total-account-throughput", TotalAccountThroughput);
					}
					if (ByokEncryptionProgress != null)
					{
						nameValueCollection.Add("x-ms-cosmos-byok-encryption-progress", ByokEncryptionProgress);
					}
					if (AppliedPolicyElementId != null)
					{
						nameValueCollection.Add("x-ms-applied-policy-element", AppliedPolicyElementId);
					}
					if (MergeProgressBlocked != null)
					{
						nameValueCollection.Add("x-ms-cosmos-is-merge-progress-blocked", MergeProgressBlocked);
					}
					if (ChangeFeedInfo != null)
					{
						nameValueCollection.Add("x-ms-cosmos-changefeed-info", ChangeFeedInfo);
					}
					if (ReIndexerProgress != null)
					{
						nameValueCollection.Add("x-ms-cosmos-reindexer-progress", ReIndexerProgress);
					}
					if (OfferReplacePendingForMerge != null)
					{
						nameValueCollection.Add("x-ms-offer-replace-pending-for-merge", OfferReplacePendingForMerge);
					}
					if (OldestActiveSchemaId != null)
					{
						nameValueCollection.Add("x-ms-cosmos-oldest-active-schema-id", OldestActiveSchemaId);
					}
					if (PhysicalPartitionId != null)
					{
						nameValueCollection.Add("x-ms-cosmos-physical-partition-id", PhysicalPartitionId);
					}
					if (MaxContentLength != null)
					{
						nameValueCollection.Add("x-ms-cosmos-max-content-length", MaxContentLength);
					}
					if (IsOfferRestorePending != null)
					{
						nameValueCollection.Add("x-ms-offer-restore-pending", IsOfferRestorePending);
					}
					if (InstantScaleUpValue != null)
					{
						nameValueCollection.Add("x-ms-cosmos-instant-scale-up-value", InstantScaleUpValue);
					}
					if (RequiresDistribution != null)
					{
						nameValueCollection.Add("x-ms-cosmos-query-requiresdistribution", RequiresDistribution);
					}
					if (CapacityType != null)
					{
						nameValueCollection.Add("x-ms-cosmos-capacity-type", CapacityType);
					}
					if (MinGLSNForTombstoneOperations != null)
					{
						nameValueCollection.Add("x-ms-cosmos-min-tombstone-glsn", MinGLSNForTombstoneOperations);
					}
					if (MinGLSNForDocumentOperations != null)
					{
						nameValueCollection.Add("x-ms-cosmos-min-document-glsn", MinGLSNForDocumentOperations);
					}
					if (HighestTentativeWriteLLSN != null)
					{
						nameValueCollection.Add("x-ms-cosmos-highest-tentative-write-llsn", HighestTentativeWriteLLSN);
					}
					if (PartitionThroughputInfo != null)
					{
						nameValueCollection.Add("x-ms-cosmos-partition-throughput-info", PartitionThroughputInfo);
					}
					if (DocumentRecordCount != null)
					{
						nameValueCollection.Add("x-ms-cosmos-document-record-count", DocumentRecordCount);
					}
					if (lazyNotCommonHeaders != null)
					{
						foreach (KeyValuePair<string, string> lazyNotCommonHeader in lazyNotCommonHeaders)
						{
							nameValueCollection.Add(lazyNotCommonHeader.Key, lazyNotCommonHeader.Value);
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
		UpdateHelper(key, null, throwIfAlreadyExists: false);
	}

	public string Get(string key)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}
		switch (key.Length)
		{
		case 3:
			if (string.Equals("lsn", key, StringComparison.OrdinalIgnoreCase))
			{
				return LSN;
			}
			break;
		case 4:
			if (string.Equals("etag", key, StringComparison.OrdinalIgnoreCase))
			{
				return ETag;
			}
			break;
		case 9:
			if (string.Equals("x-ms-date", key, StringComparison.OrdinalIgnoreCase))
			{
				return XDate;
			}
			break;
		case 12:
			if (string.Equals("x-ms-xp-role", key, StringComparison.OrdinalIgnoreCase))
			{
				return XPRole;
			}
			break;
		case 13:
			if (string.Equals("x-ms-item-lsn", key, StringComparison.OrdinalIgnoreCase))
			{
				return ItemLSN;
			}
			break;
		case 14:
			if (string.Equals("x-ms-substatus", key, StringComparison.OrdinalIgnoreCase))
			{
				return SubStatus;
			}
			break;
		case 15:
			if (string.Equals("x-ms-item-count", key, StringComparison.OrdinalIgnoreCase))
			{
				return ItemCount;
			}
			break;
		case 16:
			if ((object)"x-ms-activity-id" == key)
			{
				return ActivityId;
			}
			if ((object)"x-ms-cosmos-llsn" == key)
			{
				return LocalLSN;
			}
			if (string.Equals("x-ms-activity-id", key, StringComparison.OrdinalIgnoreCase))
			{
				return ActivityId;
			}
			if (string.Equals("x-ms-cosmos-llsn", key, StringComparison.OrdinalIgnoreCase))
			{
				return LocalLSN;
			}
			break;
		case 17:
			if ((object)"x-ms-continuation" == key)
			{
				return Continuation;
			}
			if ((object)"x-ms-content-path" == key)
			{
				return OwnerId;
			}
			if (string.Equals("x-ms-continuation", key, StringComparison.OrdinalIgnoreCase))
			{
				return Continuation;
			}
			if (string.Equals("x-ms-content-path", key, StringComparison.OrdinalIgnoreCase))
			{
				return OwnerId;
			}
			break;
		case 18:
			if ((object)"x-ms-schemaversion" == key)
			{
				return SchemaVersion;
			}
			if ((object)"x-ms-restore-state" == key)
			{
				return RestoreState;
			}
			if ((object)"x-ms-session-token" == key)
			{
				return SessionToken;
			}
			if (string.Equals("x-ms-schemaversion", key, StringComparison.OrdinalIgnoreCase))
			{
				return SchemaVersion;
			}
			if (string.Equals("x-ms-restore-state", key, StringComparison.OrdinalIgnoreCase))
			{
				return RestoreState;
			}
			if (string.Equals("x-ms-session-token", key, StringComparison.OrdinalIgnoreCase))
			{
				return SessionToken;
			}
			break;
		case 19:
			if ((object)"x-ms-retry-after-ms" == key)
			{
				return RetryAfterInMilliseconds;
			}
			if ((object)"x-ms-resource-quota" == key)
			{
				return MaxResourceQuota;
			}
			if ((object)"x-ms-resource-usage" == key)
			{
				return CurrentResourceQuotaUsage;
			}
			if ((object)"x-ms-request-charge" == key)
			{
				return RequestCharge;
			}
			if ((object)"x-docdb-resource-id" == key)
			{
				return ResourceId;
			}
			if ((object)"x-ms-serviceversion" == key)
			{
				return ServerVersion;
			}
			if (string.Equals("x-ms-retry-after-ms", key, StringComparison.OrdinalIgnoreCase))
			{
				return RetryAfterInMilliseconds;
			}
			if (string.Equals("x-ms-resource-quota", key, StringComparison.OrdinalIgnoreCase))
			{
				return MaxResourceQuota;
			}
			if (string.Equals("x-ms-resource-usage", key, StringComparison.OrdinalIgnoreCase))
			{
				return CurrentResourceQuotaUsage;
			}
			if (string.Equals("x-ms-request-charge", key, StringComparison.OrdinalIgnoreCase))
			{
				return RequestCharge;
			}
			if (string.Equals("x-docdb-resource-id", key, StringComparison.OrdinalIgnoreCase))
			{
				return ResourceId;
			}
			if (string.Equals("x-ms-serviceversion", key, StringComparison.OrdinalIgnoreCase))
			{
				return ServerVersion;
			}
			break;
		case 21:
			if ((object)"x-ms-alt-content-path" == key)
			{
				return OwnerFullName;
			}
			if ((object)"x-ms-quorum-acked-lsn" == key)
			{
				return QuorumAckedLSN;
			}
			if ((object)"x-ms-share-throughput" == key)
			{
				return ShareThroughput;
			}
			if ((object)"x-ms-cosmos-item-llsn" == key)
			{
				return ItemLocalLSN;
			}
			if (string.Equals("x-ms-alt-content-path", key, StringComparison.OrdinalIgnoreCase))
			{
				return OwnerFullName;
			}
			if (string.Equals("x-ms-quorum-acked-lsn", key, StringComparison.OrdinalIgnoreCase))
			{
				return QuorumAckedLSN;
			}
			if (string.Equals("x-ms-share-throughput", key, StringComparison.OrdinalIgnoreCase))
			{
				return ShareThroughput;
			}
			if (string.Equals("x-ms-cosmos-item-llsn", key, StringComparison.OrdinalIgnoreCase))
			{
				return ItemLocalLSN;
			}
			break;
		case 23:
			if (string.Equals("x-ms-indexing-directive", key, StringComparison.OrdinalIgnoreCase))
			{
				return IndexingDirective;
			}
			break;
		case 24:
			if ((object)"collection-service-index" == key)
			{
				return CollectionServiceIndex;
			}
			if ((object)"x-ms-database-account-id" == key)
			{
				return DatabaseAccountId;
			}
			if ((object)"x-ms-request-duration-ms" == key)
			{
				return BackendRequestDurationMilliseconds;
			}
			if (string.Equals("collection-service-index", key, StringComparison.OrdinalIgnoreCase))
			{
				return CollectionServiceIndex;
			}
			if (string.Equals("x-ms-database-account-id", key, StringComparison.OrdinalIgnoreCase))
			{
				return DatabaseAccountId;
			}
			if (string.Equals("x-ms-request-duration-ms", key, StringComparison.OrdinalIgnoreCase))
			{
				return BackendRequestDurationMilliseconds;
			}
			break;
		case 25:
			if ((object)"x-ms-current-write-quorum" == key)
			{
				return CurrentWriteQuorum;
			}
			if ((object)"x-ms-global-Committed-lsn" == key)
			{
				return GlobalCommittedLSN;
			}
			if ((object)"x-ms-transport-request-id" == key)
			{
				return TransportRequestID;
			}
			if ((object)"x-ms-cosmos-capacity-type" == key)
			{
				return CapacityType;
			}
			if (string.Equals("x-ms-current-write-quorum", key, StringComparison.OrdinalIgnoreCase))
			{
				return CurrentWriteQuorum;
			}
			if (string.Equals("x-ms-global-Committed-lsn", key, StringComparison.OrdinalIgnoreCase))
			{
				return GlobalCommittedLSN;
			}
			if (string.Equals("x-ms-transport-request-id", key, StringComparison.OrdinalIgnoreCase))
			{
				return TransportRequestID;
			}
			if (string.Equals("x-ms-cosmos-capacity-type", key, StringComparison.OrdinalIgnoreCase))
			{
				return CapacityType;
			}
			break;
		case 26:
			if ((object)"x-ms-last-state-change-utc" == key)
			{
				return LastStateChangeUtc;
			}
			if ((object)"collection-partition-index" == key)
			{
				return CollectionPartitionIndex;
			}
			if ((object)"x-ms-offer-replace-pending" == key)
			{
				return OfferReplacePending;
			}
			if ((object)"x-ms-disable-rntbd-channel" == key)
			{
				return DisableRntbdChannel;
			}
			if ((object)"x-ms-cosmos-min-throughput" == key)
			{
				return MinimumRUsForOffer;
			}
			if ((object)"x-ms-offer-restore-pending" == key)
			{
				return IsOfferRestorePending;
			}
			if (string.Equals("x-ms-last-state-change-utc", key, StringComparison.OrdinalIgnoreCase))
			{
				return LastStateChangeUtc;
			}
			if (string.Equals("collection-partition-index", key, StringComparison.OrdinalIgnoreCase))
			{
				return CollectionPartitionIndex;
			}
			if (string.Equals("x-ms-offer-replace-pending", key, StringComparison.OrdinalIgnoreCase))
			{
				return OfferReplacePending;
			}
			if (string.Equals("x-ms-disable-rntbd-channel", key, StringComparison.OrdinalIgnoreCase))
			{
				return DisableRntbdChannel;
			}
			if (string.Equals("x-ms-cosmos-min-throughput", key, StringComparison.OrdinalIgnoreCase))
			{
				return MinimumRUsForOffer;
			}
			if (string.Equals("x-ms-offer-restore-pending", key, StringComparison.OrdinalIgnoreCase))
			{
				return IsOfferRestorePending;
			}
			break;
		case 27:
			if ((object)"x-ms-number-of-read-regions" == key)
			{
				return NumberOfReadRegions;
			}
			if ((object)"x-ms-applied-policy-element" == key)
			{
				return AppliedPolicyElementId;
			}
			if ((object)"x-ms-cosmos-changefeed-info" == key)
			{
				return ChangeFeedInfo;
			}
			if (string.Equals("x-ms-number-of-read-regions", key, StringComparison.OrdinalIgnoreCase))
			{
				return NumberOfReadRegions;
			}
			if (string.Equals("x-ms-applied-policy-element", key, StringComparison.OrdinalIgnoreCase))
			{
				return AppliedPolicyElementId;
			}
			if (string.Equals("x-ms-cosmos-changefeed-info", key, StringComparison.OrdinalIgnoreCase))
			{
				return ChangeFeedInfo;
			}
			break;
		case 28:
			if (string.Equals("x-ms-time-to-live-in-seconds", key, StringComparison.OrdinalIgnoreCase))
			{
				return TimeToLiveInSeconds;
			}
			break;
		case 29:
			if ((object)"x-ms-current-replica-set-size" == key)
			{
				return CurrentReplicaSetSize;
			}
			if ((object)"x-ms-documentdb-query-metrics" == key)
			{
				return QueryMetrics;
			}
			if ((object)"x-ms-cosmos-index-utilization" == key)
			{
				return IndexUtilization;
			}
			if ((object)"x-ms-cosmos-quorum-acked-llsn" == key)
			{
				return QuorumAckedLocalLSN;
			}
			if ((object)"x-ms-cosmos-min-document-glsn" == key)
			{
				return MinGLSNForDocumentOperations;
			}
			if (string.Equals("x-ms-current-replica-set-size", key, StringComparison.OrdinalIgnoreCase))
			{
				return CurrentReplicaSetSize;
			}
			if (string.Equals("x-ms-documentdb-query-metrics", key, StringComparison.OrdinalIgnoreCase))
			{
				return QueryMetrics;
			}
			if (string.Equals("x-ms-cosmos-index-utilization", key, StringComparison.OrdinalIgnoreCase))
			{
				return IndexUtilization;
			}
			if (string.Equals("x-ms-cosmos-quorum-acked-llsn", key, StringComparison.OrdinalIgnoreCase))
			{
				return QuorumAckedLocalLSN;
			}
			if (string.Equals("x-ms-cosmos-min-document-glsn", key, StringComparison.OrdinalIgnoreCase))
			{
				return MinGLSNForDocumentOperations;
			}
			break;
		case 30:
			if ((object)"x-ms-cosmos-reindexer-progress" == key)
			{
				return ReIndexerProgress;
			}
			if ((object)"x-ms-cosmos-max-content-length" == key)
			{
				return MaxContentLength;
			}
			if ((object)"x-ms-cosmos-min-tombstone-glsn" == key)
			{
				return MinGLSNForTombstoneOperations;
			}
			if (string.Equals("x-ms-cosmos-reindexer-progress", key, StringComparison.OrdinalIgnoreCase))
			{
				return ReIndexerProgress;
			}
			if (string.Equals("x-ms-cosmos-max-content-length", key, StringComparison.OrdinalIgnoreCase))
			{
				return MaxContentLength;
			}
			if (string.Equals("x-ms-cosmos-min-tombstone-glsn", key, StringComparison.OrdinalIgnoreCase))
			{
				return MinGLSNForTombstoneOperations;
			}
			break;
		case 31:
			if (string.Equals("x-ms-request-validation-failure", key, StringComparison.OrdinalIgnoreCase))
			{
				return RequestValidationFailure;
			}
			break;
		case 32:
			if ((object)"x-ms-cosmos-query-execution-info" == key)
			{
				return QueryExecutionInfo;
			}
			if ((object)"x-ms-aad-applied-role-assignment" == key)
			{
				return AadAppliedRoleAssignmentId;
			}
			if (string.Equals("x-ms-cosmos-query-execution-info", key, StringComparison.OrdinalIgnoreCase))
			{
				return QueryExecutionInfo;
			}
			if (string.Equals("x-ms-aad-applied-role-assignment", key, StringComparison.OrdinalIgnoreCase))
			{
				return AadAppliedRoleAssignmentId;
			}
			break;
		case 33:
			if ((object)"x-ms-cosmos-replicator-glsn-delta" == key)
			{
				return ReplicatorLSNToGLSNDelta;
			}
			if ((object)"x-ms-cosmos-replicator-llsn-delta" == key)
			{
				return ReplicatorLSNToLLSNDelta;
			}
			if ((object)"x-ms-cosmos-correlated-activityid" == key)
			{
				return CorrelatedActivityId;
			}
			if ((object)"x-ms-cosmos-physical-partition-id" == key)
			{
				return PhysicalPartitionId;
			}
			if ((object)"x-ms-cosmos-document-record-count" == key)
			{
				return DocumentRecordCount;
			}
			if (string.Equals("x-ms-cosmos-replicator-glsn-delta", key, StringComparison.OrdinalIgnoreCase))
			{
				return ReplicatorLSNToGLSNDelta;
			}
			if (string.Equals("x-ms-cosmos-replicator-llsn-delta", key, StringComparison.OrdinalIgnoreCase))
			{
				return ReplicatorLSNToLLSNDelta;
			}
			if (string.Equals("x-ms-cosmos-correlated-activityid", key, StringComparison.OrdinalIgnoreCase))
			{
				return CorrelatedActivityId;
			}
			if (string.Equals("x-ms-cosmos-physical-partition-id", key, StringComparison.OrdinalIgnoreCase))
			{
				return PhysicalPartitionId;
			}
			if (string.Equals("x-ms-cosmos-document-record-count", key, StringComparison.OrdinalIgnoreCase))
			{
				return DocumentRecordCount;
			}
			break;
		case 34:
			if ((object)"x-ms-documentdb-script-log-results" == key)
			{
				return LogResults;
			}
			if ((object)"x-ms-cosmosdb-has-tentative-writes" == key)
			{
				return HasTentativeWrites;
			}
			if ((object)"x-ms-cosmos-instant-scale-up-value" == key)
			{
				return InstantScaleUpValue;
			}
			if (string.Equals("x-ms-documentdb-script-log-results", key, StringComparison.OrdinalIgnoreCase))
			{
				return LogResults;
			}
			if (string.Equals("x-ms-cosmosdb-has-tentative-writes", key, StringComparison.OrdinalIgnoreCase))
			{
				return HasTentativeWrites;
			}
			if (string.Equals("x-ms-cosmos-instant-scale-up-value", key, StringComparison.OrdinalIgnoreCase))
			{
				return InstantScaleUpValue;
			}
			break;
		case 35:
			if ((object)"x-ms-documentdb-partitionkeyrangeid" == key)
			{
				return PartitionKeyRangeId;
			}
			if ((object)"x-ms-collection-security-identifier" == key)
			{
				return CollectionSecurityIdentifier;
			}
			if ((object)"x-ms-cosmos-oldest-active-schema-id" == key)
			{
				return OldestActiveSchemaId;
			}
			if (string.Equals("x-ms-documentdb-partitionkeyrangeid", key, StringComparison.OrdinalIgnoreCase))
			{
				return PartitionKeyRangeId;
			}
			if (string.Equals("x-ms-collection-security-identifier", key, StringComparison.OrdinalIgnoreCase))
			{
				return CollectionSecurityIdentifier;
			}
			if (string.Equals("x-ms-cosmos-oldest-active-schema-id", key, StringComparison.OrdinalIgnoreCase))
			{
				return OldestActiveSchemaId;
			}
			break;
		case 36:
			if ((object)"x-ms-cosmos-total-account-throughput" == key)
			{
				return TotalAccountThroughput;
			}
			if ((object)"x-ms-cosmos-byok-encryption-progress" == key)
			{
				return ByokEncryptionProgress;
			}
			if ((object)"x-ms-offer-replace-pending-for-merge" == key)
			{
				return OfferReplacePendingForMerge;
			}
			if (string.Equals("x-ms-cosmos-total-account-throughput", key, StringComparison.OrdinalIgnoreCase))
			{
				return TotalAccountThroughput;
			}
			if (string.Equals("x-ms-cosmos-byok-encryption-progress", key, StringComparison.OrdinalIgnoreCase))
			{
				return ByokEncryptionProgress;
			}
			if (string.Equals("x-ms-offer-replace-pending-for-merge", key, StringComparison.OrdinalIgnoreCase))
			{
				return OfferReplacePendingForMerge;
			}
			break;
		case 37:
			if ((object)"x-ms-documentdb-is-ru-per-minute-used" == key)
			{
				return IsRUPerMinuteUsed;
			}
			if ((object)"x-ms-cosmos-is-replica-status-revoked" == key)
			{
				return ReplicaStatusRevoked;
			}
			if ((object)"x-ms-cosmos-is-merge-progress-blocked" == key)
			{
				return MergeProgressBlocked;
			}
			if ((object)"x-ms-cosmos-partition-throughput-info" == key)
			{
				return PartitionThroughputInfo;
			}
			if (string.Equals("x-ms-documentdb-is-ru-per-minute-used", key, StringComparison.OrdinalIgnoreCase))
			{
				return IsRUPerMinuteUsed;
			}
			if (string.Equals("x-ms-cosmos-is-replica-status-revoked", key, StringComparison.OrdinalIgnoreCase))
			{
				return ReplicaStatusRevoked;
			}
			if (string.Equals("x-ms-cosmos-is-merge-progress-blocked", key, StringComparison.OrdinalIgnoreCase))
			{
				return MergeProgressBlocked;
			}
			if (string.Equals("x-ms-cosmos-partition-throughput-info", key, StringComparison.OrdinalIgnoreCase))
			{
				return PartitionThroughputInfo;
			}
			break;
		case 38:
			if ((object)"x-ms-cosmos-vectorclock-local-progress" == key)
			{
				return VectorClockLocalProgress;
			}
			if ((object)"x-ms-cosmos-replica-confirmed-checksum" == key)
			{
				return ConfirmedStoreChecksum;
			}
			if ((object)"x-ms-cosmos-replica-tentative-checksum" == key)
			{
				return TentativeStoreChecksum;
			}
			if ((object)"x-ms-cosmos-query-requiresdistribution" == key)
			{
				return RequiresDistribution;
			}
			if (string.Equals("x-ms-cosmos-vectorclock-local-progress", key, StringComparison.OrdinalIgnoreCase))
			{
				return VectorClockLocalProgress;
			}
			if (string.Equals("x-ms-cosmos-replica-confirmed-checksum", key, StringComparison.OrdinalIgnoreCase))
			{
				return ConfirmedStoreChecksum;
			}
			if (string.Equals("x-ms-cosmos-replica-tentative-checksum", key, StringComparison.OrdinalIgnoreCase))
			{
				return TentativeStoreChecksum;
			}
			if (string.Equals("x-ms-cosmos-query-requiresdistribution", key, StringComparison.OrdinalIgnoreCase))
			{
				return RequiresDistribution;
			}
			break;
		case 40:
			if ((object)"x-ms-cosmos-offer-max-allowed-throughput" == key)
			{
				return SoftMaxAllowedThroughput;
			}
			if ((object)"x-ms-cosmos-highest-tentative-write-llsn" == key)
			{
				return HighestTentativeWriteLLSN;
			}
			if (string.Equals("x-ms-cosmos-offer-max-allowed-throughput", key, StringComparison.OrdinalIgnoreCase))
			{
				return SoftMaxAllowedThroughput;
			}
			if (string.Equals("x-ms-cosmos-highest-tentative-write-llsn", key, StringComparison.OrdinalIgnoreCase))
			{
				return HighestTentativeWriteLLSN;
			}
			break;
		case 41:
			if (string.Equals("x-ms-cosmos-analytical-migration-progress", key, StringComparison.OrdinalIgnoreCase))
			{
				return AnalyticalMigrationProgress;
			}
			break;
		case 42:
			if (string.Equals("x-ms-cosmos-xpconfiguration-sessions-count", key, StringComparison.OrdinalIgnoreCase))
			{
				return XPConfigurationSessionsCount;
			}
			break;
		case 43:
			if (string.Equals("x-ms-cosmos-is-partition-key-delete-pending", key, StringComparison.OrdinalIgnoreCase))
			{
				return PendingPKDelete;
			}
			break;
		case 48:
			if (string.Equals("x-ms-cosmos-collection-unique-keys-under-reindex", key, StringComparison.OrdinalIgnoreCase))
			{
				return CollectionUniqueKeysUnderReIndex;
			}
			break;
		case 49:
			if (string.Equals("x-ms-documentdb-collection-lazy-indexing-progress", key, StringComparison.OrdinalIgnoreCase))
			{
				return CollectionLazyIndexingProgress;
			}
			break;
		case 52:
			if ((object)"x-ms-cosmos-internal-unflushed-merge-log-entry-count" == key)
			{
				return UnflushedMergLogEntryCount;
			}
			if ((object)"x-ms-cosmos-collection-unique-index-reindex-progress" == key)
			{
				return CollectionUniqueIndexReIndexProgress;
			}
			if (string.Equals("x-ms-cosmos-internal-unflushed-merge-log-entry-count", key, StringComparison.OrdinalIgnoreCase))
			{
				return UnflushedMergLogEntryCount;
			}
			if (string.Equals("x-ms-cosmos-collection-unique-index-reindex-progress", key, StringComparison.OrdinalIgnoreCase))
			{
				return CollectionUniqueIndexReIndexProgress;
			}
			break;
		case 56:
			if (string.Equals("x-ms-documentdb-collection-index-transformation-progress", key, StringComparison.OrdinalIgnoreCase))
			{
				return CollectionIndexTransformationProgress;
			}
			break;
		}
		Dictionary<string, string> dictionary = lazyNotCommonHeaders;
		if (dictionary != null && dictionary.TryGetValue(key, out var value))
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
		UpdateHelper(key, value, throwIfAlreadyExists: true);
	}

	public void Set(string key, string value)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		UpdateHelper(key, value, throwIfAlreadyExists: false);
	}

	public void UpdateHelper(string key, string value, bool throwIfAlreadyExists)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}
		switch (key.Length)
		{
		case 3:
			if (string.Equals("lsn", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && LSN != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				LSN = value;
				return;
			}
			break;
		case 4:
			if (string.Equals("etag", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ETag != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ETag = value;
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
		case 12:
			if (string.Equals("x-ms-xp-role", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && XPRole != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				XPRole = value;
				return;
			}
			break;
		case 13:
			if (string.Equals("x-ms-item-lsn", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ItemLSN != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ItemLSN = value;
				return;
			}
			break;
		case 14:
			if (string.Equals("x-ms-substatus", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && SubStatus != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SubStatus = value;
				return;
			}
			break;
		case 15:
			if (string.Equals("x-ms-item-count", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ItemCount != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ItemCount = value;
				return;
			}
			break;
		case 16:
			if ((object)"x-ms-activity-id" == key)
			{
				if (throwIfAlreadyExists && ActivityId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ActivityId = value;
				return;
			}
			if ((object)"x-ms-cosmos-llsn" == key)
			{
				if (throwIfAlreadyExists && LocalLSN != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				LocalLSN = value;
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
			if (string.Equals("x-ms-cosmos-llsn", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && LocalLSN != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				LocalLSN = value;
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
			if ((object)"x-ms-content-path" == key)
			{
				if (throwIfAlreadyExists && OwnerId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				OwnerId = value;
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
			if (string.Equals("x-ms-content-path", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && OwnerId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				OwnerId = value;
				return;
			}
			break;
		case 18:
			if ((object)"x-ms-schemaversion" == key)
			{
				if (throwIfAlreadyExists && SchemaVersion != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SchemaVersion = value;
				return;
			}
			if ((object)"x-ms-restore-state" == key)
			{
				if (throwIfAlreadyExists && RestoreState != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				RestoreState = value;
				return;
			}
			if ((object)"x-ms-session-token" == key)
			{
				if (throwIfAlreadyExists && SessionToken != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SessionToken = value;
				return;
			}
			if (string.Equals("x-ms-schemaversion", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && SchemaVersion != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SchemaVersion = value;
				return;
			}
			if (string.Equals("x-ms-restore-state", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && RestoreState != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				RestoreState = value;
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
			break;
		case 19:
			if ((object)"x-ms-retry-after-ms" == key)
			{
				if (throwIfAlreadyExists && RetryAfterInMilliseconds != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				RetryAfterInMilliseconds = value;
				return;
			}
			if ((object)"x-ms-resource-quota" == key)
			{
				if (throwIfAlreadyExists && MaxResourceQuota != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				MaxResourceQuota = value;
				return;
			}
			if ((object)"x-ms-resource-usage" == key)
			{
				if (throwIfAlreadyExists && CurrentResourceQuotaUsage != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CurrentResourceQuotaUsage = value;
				return;
			}
			if ((object)"x-ms-request-charge" == key)
			{
				if (throwIfAlreadyExists && RequestCharge != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				RequestCharge = value;
				return;
			}
			if ((object)"x-docdb-resource-id" == key)
			{
				if (throwIfAlreadyExists && ResourceId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ResourceId = value;
				return;
			}
			if ((object)"x-ms-serviceversion" == key)
			{
				if (throwIfAlreadyExists && ServerVersion != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ServerVersion = value;
				return;
			}
			if (string.Equals("x-ms-retry-after-ms", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && RetryAfterInMilliseconds != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				RetryAfterInMilliseconds = value;
				return;
			}
			if (string.Equals("x-ms-resource-quota", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && MaxResourceQuota != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				MaxResourceQuota = value;
				return;
			}
			if (string.Equals("x-ms-resource-usage", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && CurrentResourceQuotaUsage != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CurrentResourceQuotaUsage = value;
				return;
			}
			if (string.Equals("x-ms-request-charge", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && RequestCharge != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				RequestCharge = value;
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
			if (string.Equals("x-ms-serviceversion", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ServerVersion != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ServerVersion = value;
				return;
			}
			break;
		case 21:
			if ((object)"x-ms-alt-content-path" == key)
			{
				if (throwIfAlreadyExists && OwnerFullName != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				OwnerFullName = value;
				return;
			}
			if ((object)"x-ms-quorum-acked-lsn" == key)
			{
				if (throwIfAlreadyExists && QuorumAckedLSN != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				QuorumAckedLSN = value;
				return;
			}
			if ((object)"x-ms-share-throughput" == key)
			{
				if (throwIfAlreadyExists && ShareThroughput != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ShareThroughput = value;
				return;
			}
			if ((object)"x-ms-cosmos-item-llsn" == key)
			{
				if (throwIfAlreadyExists && ItemLocalLSN != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ItemLocalLSN = value;
				return;
			}
			if (string.Equals("x-ms-alt-content-path", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && OwnerFullName != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				OwnerFullName = value;
				return;
			}
			if (string.Equals("x-ms-quorum-acked-lsn", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && QuorumAckedLSN != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				QuorumAckedLSN = value;
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
			if (string.Equals("x-ms-cosmos-item-llsn", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ItemLocalLSN != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ItemLocalLSN = value;
				return;
			}
			break;
		case 23:
			if (string.Equals("x-ms-indexing-directive", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && IndexingDirective != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IndexingDirective = value;
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
			if ((object)"x-ms-database-account-id" == key)
			{
				if (throwIfAlreadyExists && DatabaseAccountId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				DatabaseAccountId = value;
				return;
			}
			if ((object)"x-ms-request-duration-ms" == key)
			{
				if (throwIfAlreadyExists && BackendRequestDurationMilliseconds != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				BackendRequestDurationMilliseconds = value;
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
			if (string.Equals("x-ms-database-account-id", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && DatabaseAccountId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				DatabaseAccountId = value;
				return;
			}
			if (string.Equals("x-ms-request-duration-ms", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && BackendRequestDurationMilliseconds != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				BackendRequestDurationMilliseconds = value;
				return;
			}
			break;
		case 25:
			if ((object)"x-ms-current-write-quorum" == key)
			{
				if (throwIfAlreadyExists && CurrentWriteQuorum != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CurrentWriteQuorum = value;
				return;
			}
			if ((object)"x-ms-global-Committed-lsn" == key)
			{
				if (throwIfAlreadyExists && GlobalCommittedLSN != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				GlobalCommittedLSN = value;
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
			if ((object)"x-ms-cosmos-capacity-type" == key)
			{
				if (throwIfAlreadyExists && CapacityType != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CapacityType = value;
				return;
			}
			if (string.Equals("x-ms-current-write-quorum", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && CurrentWriteQuorum != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CurrentWriteQuorum = value;
				return;
			}
			if (string.Equals("x-ms-global-Committed-lsn", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && GlobalCommittedLSN != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				GlobalCommittedLSN = value;
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
			if (string.Equals("x-ms-cosmos-capacity-type", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && CapacityType != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CapacityType = value;
				return;
			}
			break;
		case 26:
			if ((object)"x-ms-last-state-change-utc" == key)
			{
				if (throwIfAlreadyExists && LastStateChangeUtc != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				LastStateChangeUtc = value;
				return;
			}
			if ((object)"collection-partition-index" == key)
			{
				if (throwIfAlreadyExists && CollectionPartitionIndex != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CollectionPartitionIndex = value;
				return;
			}
			if ((object)"x-ms-offer-replace-pending" == key)
			{
				if (throwIfAlreadyExists && OfferReplacePending != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				OfferReplacePending = value;
				return;
			}
			if ((object)"x-ms-disable-rntbd-channel" == key)
			{
				if (throwIfAlreadyExists && DisableRntbdChannel != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				DisableRntbdChannel = value;
				return;
			}
			if ((object)"x-ms-cosmos-min-throughput" == key)
			{
				if (throwIfAlreadyExists && MinimumRUsForOffer != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				MinimumRUsForOffer = value;
				return;
			}
			if ((object)"x-ms-offer-restore-pending" == key)
			{
				if (throwIfAlreadyExists && IsOfferRestorePending != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsOfferRestorePending = value;
				return;
			}
			if (string.Equals("x-ms-last-state-change-utc", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && LastStateChangeUtc != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				LastStateChangeUtc = value;
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
			if (string.Equals("x-ms-offer-replace-pending", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && OfferReplacePending != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				OfferReplacePending = value;
				return;
			}
			if (string.Equals("x-ms-disable-rntbd-channel", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && DisableRntbdChannel != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				DisableRntbdChannel = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-min-throughput", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && MinimumRUsForOffer != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				MinimumRUsForOffer = value;
				return;
			}
			if (string.Equals("x-ms-offer-restore-pending", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && IsOfferRestorePending != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsOfferRestorePending = value;
				return;
			}
			break;
		case 27:
			if ((object)"x-ms-number-of-read-regions" == key)
			{
				if (throwIfAlreadyExists && NumberOfReadRegions != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				NumberOfReadRegions = value;
				return;
			}
			if ((object)"x-ms-applied-policy-element" == key)
			{
				if (throwIfAlreadyExists && AppliedPolicyElementId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				AppliedPolicyElementId = value;
				return;
			}
			if ((object)"x-ms-cosmos-changefeed-info" == key)
			{
				if (throwIfAlreadyExists && ChangeFeedInfo != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ChangeFeedInfo = value;
				return;
			}
			if (string.Equals("x-ms-number-of-read-regions", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && NumberOfReadRegions != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				NumberOfReadRegions = value;
				return;
			}
			if (string.Equals("x-ms-applied-policy-element", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && AppliedPolicyElementId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				AppliedPolicyElementId = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-changefeed-info", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ChangeFeedInfo != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ChangeFeedInfo = value;
				return;
			}
			break;
		case 28:
			if (string.Equals("x-ms-time-to-live-in-seconds", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && TimeToLiveInSeconds != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				TimeToLiveInSeconds = value;
				return;
			}
			break;
		case 29:
			if ((object)"x-ms-current-replica-set-size" == key)
			{
				if (throwIfAlreadyExists && CurrentReplicaSetSize != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CurrentReplicaSetSize = value;
				return;
			}
			if ((object)"x-ms-documentdb-query-metrics" == key)
			{
				if (throwIfAlreadyExists && QueryMetrics != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				QueryMetrics = value;
				return;
			}
			if ((object)"x-ms-cosmos-index-utilization" == key)
			{
				if (throwIfAlreadyExists && IndexUtilization != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IndexUtilization = value;
				return;
			}
			if ((object)"x-ms-cosmos-quorum-acked-llsn" == key)
			{
				if (throwIfAlreadyExists && QuorumAckedLocalLSN != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				QuorumAckedLocalLSN = value;
				return;
			}
			if ((object)"x-ms-cosmos-min-document-glsn" == key)
			{
				if (throwIfAlreadyExists && MinGLSNForDocumentOperations != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				MinGLSNForDocumentOperations = value;
				return;
			}
			if (string.Equals("x-ms-current-replica-set-size", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && CurrentReplicaSetSize != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CurrentReplicaSetSize = value;
				return;
			}
			if (string.Equals("x-ms-documentdb-query-metrics", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && QueryMetrics != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				QueryMetrics = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-index-utilization", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && IndexUtilization != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IndexUtilization = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-quorum-acked-llsn", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && QuorumAckedLocalLSN != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				QuorumAckedLocalLSN = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-min-document-glsn", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && MinGLSNForDocumentOperations != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				MinGLSNForDocumentOperations = value;
				return;
			}
			break;
		case 30:
			if ((object)"x-ms-cosmos-reindexer-progress" == key)
			{
				if (throwIfAlreadyExists && ReIndexerProgress != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ReIndexerProgress = value;
				return;
			}
			if ((object)"x-ms-cosmos-max-content-length" == key)
			{
				if (throwIfAlreadyExists && MaxContentLength != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				MaxContentLength = value;
				return;
			}
			if ((object)"x-ms-cosmos-min-tombstone-glsn" == key)
			{
				if (throwIfAlreadyExists && MinGLSNForTombstoneOperations != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				MinGLSNForTombstoneOperations = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-reindexer-progress", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ReIndexerProgress != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ReIndexerProgress = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-max-content-length", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && MaxContentLength != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				MaxContentLength = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-min-tombstone-glsn", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && MinGLSNForTombstoneOperations != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				MinGLSNForTombstoneOperations = value;
				return;
			}
			break;
		case 31:
			if (string.Equals("x-ms-request-validation-failure", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && RequestValidationFailure != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				RequestValidationFailure = value;
				return;
			}
			break;
		case 32:
			if ((object)"x-ms-cosmos-query-execution-info" == key)
			{
				if (throwIfAlreadyExists && QueryExecutionInfo != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				QueryExecutionInfo = value;
				return;
			}
			if ((object)"x-ms-aad-applied-role-assignment" == key)
			{
				if (throwIfAlreadyExists && AadAppliedRoleAssignmentId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				AadAppliedRoleAssignmentId = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-query-execution-info", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && QueryExecutionInfo != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				QueryExecutionInfo = value;
				return;
			}
			if (string.Equals("x-ms-aad-applied-role-assignment", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && AadAppliedRoleAssignmentId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				AadAppliedRoleAssignmentId = value;
				return;
			}
			break;
		case 33:
			if ((object)"x-ms-cosmos-replicator-glsn-delta" == key)
			{
				if (throwIfAlreadyExists && ReplicatorLSNToGLSNDelta != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ReplicatorLSNToGLSNDelta = value;
				return;
			}
			if ((object)"x-ms-cosmos-replicator-llsn-delta" == key)
			{
				if (throwIfAlreadyExists && ReplicatorLSNToLLSNDelta != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ReplicatorLSNToLLSNDelta = value;
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
			if ((object)"x-ms-cosmos-physical-partition-id" == key)
			{
				if (throwIfAlreadyExists && PhysicalPartitionId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PhysicalPartitionId = value;
				return;
			}
			if ((object)"x-ms-cosmos-document-record-count" == key)
			{
				if (throwIfAlreadyExists && DocumentRecordCount != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				DocumentRecordCount = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-replicator-glsn-delta", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ReplicatorLSNToGLSNDelta != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ReplicatorLSNToGLSNDelta = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-replicator-llsn-delta", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ReplicatorLSNToLLSNDelta != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ReplicatorLSNToLLSNDelta = value;
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
			if (string.Equals("x-ms-cosmos-physical-partition-id", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PhysicalPartitionId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PhysicalPartitionId = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-document-record-count", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && DocumentRecordCount != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				DocumentRecordCount = value;
				return;
			}
			break;
		case 34:
			if ((object)"x-ms-documentdb-script-log-results" == key)
			{
				if (throwIfAlreadyExists && LogResults != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				LogResults = value;
				return;
			}
			if ((object)"x-ms-cosmosdb-has-tentative-writes" == key)
			{
				if (throwIfAlreadyExists && HasTentativeWrites != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				HasTentativeWrites = value;
				return;
			}
			if ((object)"x-ms-cosmos-instant-scale-up-value" == key)
			{
				if (throwIfAlreadyExists && InstantScaleUpValue != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				InstantScaleUpValue = value;
				return;
			}
			if (string.Equals("x-ms-documentdb-script-log-results", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && LogResults != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				LogResults = value;
				return;
			}
			if (string.Equals("x-ms-cosmosdb-has-tentative-writes", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && HasTentativeWrites != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				HasTentativeWrites = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-instant-scale-up-value", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && InstantScaleUpValue != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				InstantScaleUpValue = value;
				return;
			}
			break;
		case 35:
			if ((object)"x-ms-documentdb-partitionkeyrangeid" == key)
			{
				if (throwIfAlreadyExists && PartitionKeyRangeId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PartitionKeyRangeId = value;
				return;
			}
			if ((object)"x-ms-collection-security-identifier" == key)
			{
				if (throwIfAlreadyExists && CollectionSecurityIdentifier != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CollectionSecurityIdentifier = value;
				return;
			}
			if ((object)"x-ms-cosmos-oldest-active-schema-id" == key)
			{
				if (throwIfAlreadyExists && OldestActiveSchemaId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				OldestActiveSchemaId = value;
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
			if (string.Equals("x-ms-collection-security-identifier", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && CollectionSecurityIdentifier != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CollectionSecurityIdentifier = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-oldest-active-schema-id", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && OldestActiveSchemaId != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				OldestActiveSchemaId = value;
				return;
			}
			break;
		case 36:
			if ((object)"x-ms-cosmos-total-account-throughput" == key)
			{
				if (throwIfAlreadyExists && TotalAccountThroughput != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				TotalAccountThroughput = value;
				return;
			}
			if ((object)"x-ms-cosmos-byok-encryption-progress" == key)
			{
				if (throwIfAlreadyExists && ByokEncryptionProgress != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ByokEncryptionProgress = value;
				return;
			}
			if ((object)"x-ms-offer-replace-pending-for-merge" == key)
			{
				if (throwIfAlreadyExists && OfferReplacePendingForMerge != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				OfferReplacePendingForMerge = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-total-account-throughput", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && TotalAccountThroughput != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				TotalAccountThroughput = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-byok-encryption-progress", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ByokEncryptionProgress != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ByokEncryptionProgress = value;
				return;
			}
			if (string.Equals("x-ms-offer-replace-pending-for-merge", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && OfferReplacePendingForMerge != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				OfferReplacePendingForMerge = value;
				return;
			}
			break;
		case 37:
			if ((object)"x-ms-documentdb-is-ru-per-minute-used" == key)
			{
				if (throwIfAlreadyExists && IsRUPerMinuteUsed != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsRUPerMinuteUsed = value;
				return;
			}
			if ((object)"x-ms-cosmos-is-replica-status-revoked" == key)
			{
				if (throwIfAlreadyExists && ReplicaStatusRevoked != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ReplicaStatusRevoked = value;
				return;
			}
			if ((object)"x-ms-cosmos-is-merge-progress-blocked" == key)
			{
				if (throwIfAlreadyExists && MergeProgressBlocked != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				MergeProgressBlocked = value;
				return;
			}
			if ((object)"x-ms-cosmos-partition-throughput-info" == key)
			{
				if (throwIfAlreadyExists && PartitionThroughputInfo != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PartitionThroughputInfo = value;
				return;
			}
			if (string.Equals("x-ms-documentdb-is-ru-per-minute-used", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && IsRUPerMinuteUsed != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				IsRUPerMinuteUsed = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-is-replica-status-revoked", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ReplicaStatusRevoked != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ReplicaStatusRevoked = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-is-merge-progress-blocked", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && MergeProgressBlocked != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				MergeProgressBlocked = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-partition-throughput-info", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PartitionThroughputInfo != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PartitionThroughputInfo = value;
				return;
			}
			break;
		case 38:
			if ((object)"x-ms-cosmos-vectorclock-local-progress" == key)
			{
				if (throwIfAlreadyExists && VectorClockLocalProgress != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				VectorClockLocalProgress = value;
				return;
			}
			if ((object)"x-ms-cosmos-replica-confirmed-checksum" == key)
			{
				if (throwIfAlreadyExists && ConfirmedStoreChecksum != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ConfirmedStoreChecksum = value;
				return;
			}
			if ((object)"x-ms-cosmos-replica-tentative-checksum" == key)
			{
				if (throwIfAlreadyExists && TentativeStoreChecksum != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				TentativeStoreChecksum = value;
				return;
			}
			if ((object)"x-ms-cosmos-query-requiresdistribution" == key)
			{
				if (throwIfAlreadyExists && RequiresDistribution != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				RequiresDistribution = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-vectorclock-local-progress", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && VectorClockLocalProgress != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				VectorClockLocalProgress = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-replica-confirmed-checksum", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && ConfirmedStoreChecksum != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				ConfirmedStoreChecksum = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-replica-tentative-checksum", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && TentativeStoreChecksum != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				TentativeStoreChecksum = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-query-requiresdistribution", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && RequiresDistribution != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				RequiresDistribution = value;
				return;
			}
			break;
		case 40:
			if ((object)"x-ms-cosmos-offer-max-allowed-throughput" == key)
			{
				if (throwIfAlreadyExists && SoftMaxAllowedThroughput != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SoftMaxAllowedThroughput = value;
				return;
			}
			if ((object)"x-ms-cosmos-highest-tentative-write-llsn" == key)
			{
				if (throwIfAlreadyExists && HighestTentativeWriteLLSN != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				HighestTentativeWriteLLSN = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-offer-max-allowed-throughput", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && SoftMaxAllowedThroughput != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				SoftMaxAllowedThroughput = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-highest-tentative-write-llsn", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && HighestTentativeWriteLLSN != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				HighestTentativeWriteLLSN = value;
				return;
			}
			break;
		case 41:
			if (string.Equals("x-ms-cosmos-analytical-migration-progress", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && AnalyticalMigrationProgress != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				AnalyticalMigrationProgress = value;
				return;
			}
			break;
		case 42:
			if (string.Equals("x-ms-cosmos-xpconfiguration-sessions-count", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && XPConfigurationSessionsCount != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				XPConfigurationSessionsCount = value;
				return;
			}
			break;
		case 43:
			if (string.Equals("x-ms-cosmos-is-partition-key-delete-pending", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && PendingPKDelete != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				PendingPKDelete = value;
				return;
			}
			break;
		case 48:
			if (string.Equals("x-ms-cosmos-collection-unique-keys-under-reindex", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && CollectionUniqueKeysUnderReIndex != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CollectionUniqueKeysUnderReIndex = value;
				return;
			}
			break;
		case 49:
			if (string.Equals("x-ms-documentdb-collection-lazy-indexing-progress", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && CollectionLazyIndexingProgress != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CollectionLazyIndexingProgress = value;
				return;
			}
			break;
		case 52:
			if ((object)"x-ms-cosmos-internal-unflushed-merge-log-entry-count" == key)
			{
				if (throwIfAlreadyExists && UnflushedMergLogEntryCount != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				UnflushedMergLogEntryCount = value;
				return;
			}
			if ((object)"x-ms-cosmos-collection-unique-index-reindex-progress" == key)
			{
				if (throwIfAlreadyExists && CollectionUniqueIndexReIndexProgress != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CollectionUniqueIndexReIndexProgress = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-internal-unflushed-merge-log-entry-count", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && UnflushedMergLogEntryCount != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				UnflushedMergLogEntryCount = value;
				return;
			}
			if (string.Equals("x-ms-cosmos-collection-unique-index-reindex-progress", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && CollectionUniqueIndexReIndexProgress != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CollectionUniqueIndexReIndexProgress = value;
				return;
			}
			break;
		case 56:
			if (string.Equals("x-ms-documentdb-collection-index-transformation-progress", key, StringComparison.OrdinalIgnoreCase))
			{
				if (throwIfAlreadyExists && CollectionIndexTransformationProgress != null)
				{
					throw new ArgumentException("The " + key + " already exists in the collection");
				}
				CollectionIndexTransformationProgress = value;
				return;
			}
			break;
		}
		if (throwIfAlreadyExists)
		{
			GetOrCreateLazyHeaders().Add(key, value);
		}
		else if (value == null)
		{
			if (lazyNotCommonHeaders != null)
			{
				lazyNotCommonHeaders.Remove(key);
			}
		}
		else
		{
			GetOrCreateLazyHeaders()[key] = value;
		}
	}

	private Dictionary<string, string> GetOrCreateLazyHeaders()
	{
		Dictionary<string, string> dictionary = lazyNotCommonHeaders;
		if (dictionary == null)
		{
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>(DefaultStringComparer);
			dictionary = Interlocked.CompareExchange(ref lazyNotCommonHeaders, dictionary2, null) ?? dictionary2;
		}
		return dictionary;
	}
}
