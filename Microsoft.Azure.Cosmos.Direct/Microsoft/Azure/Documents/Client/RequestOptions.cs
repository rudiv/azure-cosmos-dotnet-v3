using System;
using System.Collections.Generic;


namespace Microsoft.Azure.Documents.Client;

internal sealed class RequestOptions
{
	public IList<string> PreTriggerInclude { get; set; }

	public IList<string> PostTriggerInclude { get; set; }

	public AccessCondition AccessCondition { get; set; }

	public IndexingDirective? IndexingDirective { get; set; }

	public ConsistencyLevel? ConsistencyLevel { get; set; }

	public string SessionToken { get; set; }

	public int? ResourceTokenExpirySeconds { get; set; }

	public string OfferType { get; set; }

	public int? OfferThroughput { get; set; }

	public bool OfferEnableRUPerMinuteThroughput { get; set; }

	internal double? BackgroundTaskMaxAllowedThroughputPercent { get; set; }

	public PartitionKey PartitionKey { get; set; }

	public bool EnableScriptLogging { get; set; }

	internal bool IsReadOnlyScript { get; set; }

	internal bool IncludeSnapshotDirectories { get; set; }

	public bool PopulateQuotaInfo { get; set; }

	public bool DisableRUPerMinuteUsage { get; set; }

	public bool PopulatePartitionKeyRangeStatistics { get; set; }

	internal bool PopulateUniqueIndexReIndexProgress { get; set; }

	internal bool PopulateAnalyticalMigrationProgress { get; set; }

	internal bool PopulateBYOKEncryptionProgress { get; set; }

	internal RemoteStorageType? RemoteStorageType { get; set; }

	internal string PartitionKeyRangeId { get; set; }

	internal string SourceDatabaseId { get; set; }

	internal string SourceCollectionId { get; set; }

	internal long? RestorePointInTime { get; set; }

	internal bool PopulateRestoreStatus { get; set; }

	internal bool PopulateCapacityType { get; set; }

	internal bool? ExcludeSystemProperties { get; set; }

	internal bool InsertSystemPartitionKey { get; set; }

	internal string MergeStaticId { get; set; }

	internal bool PreserveFullContent { get; set; }

	internal bool ForceSideBySideIndexMigration { get; set; }

	internal bool CollectionTruncate { get; set; }

	internal bool IsClientEncrypted { get; set; }

	[Obsolete("Deprecated")]
	public int? SharedOfferThroughput { get; set; }

	internal PriorityLevel? PriorityLevel { get; set; }
}
