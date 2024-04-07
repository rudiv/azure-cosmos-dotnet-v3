using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Microsoft.Azure.Documents.Routing;

namespace Microsoft.Azure.Documents;

using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

internal class DocumentCollection : Resource
{

    [JsonPropertyName("indexingPolicy")]
	public IndexingPolicy IndexingPolicy { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "computedProperties")]
	internal Collection<ComputedProperty> ComputedProperties { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "geospatialConfig")]
	public GeospatialConfig GeospatialConfig { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "encryptionScopeId")]
	internal string EncryptionScopeId { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "encryptionScope")]
	internal EncryptionScopeMetadata EncryptionScopeMetadata { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "materializedViewDefinition")]
	internal MaterializedViewDefinition MaterializedViewDefinition { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "byokConfig")]
	internal ByokConfig ByokConfig { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "uniqueIndexNameEncodingMode")]
	internal byte UniqueIndexNameEncodingMode { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "_docs")]
    public string DocsInternal { get; set; }

    public string DocumentsLink => base.SelfLink.TrimEnd(new char[1] { '/' }) + "/" + DocsInternal;

    [System.Text.Json.Serialization.JsonPropertyName(name: "_sprocs")]
    public string SpInternal { get; set; }
    
    public string StoredProceduresLink => base.SelfLink.TrimEnd(new char[1] { '/' }) + "/" + SpInternal;

    [JsonPropertyName("_triggers")]
    public string TrigInternal { get; set; }
    public string TriggersLink => base.SelfLink.TrimEnd(new char[1] { '/' }) + "/" + TrigInternal;

    [JsonPropertyName("_udfs")]
    public string UdfInternal { get; set; }
    
	public string UserDefinedFunctionsLink => base.SelfLink.TrimEnd(new char[1] { '/' }) + "/" + UdfInternal;

    [JsonPropertyName("_conflicts")]
    public string ConflictsInternal { get; set; }
    
	public string ConflictsLink => base.SelfLink.TrimEnd(new char[1] { '/' }) + "/" + ConflictsInternal;

	[System.Text.Json.Serialization.JsonPropertyName(name: "partitionKey")]
	public PartitionKeyDefinition PartitionKey { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "defaultTtl")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public int? DefaultTimeToLive { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "ttlPropertyPath")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public string TimeToLivePropertyPath { get; set; }

	internal SchemaDiscoveryPolicy SchemaDiscoveryPolicy { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "uniqueKeyPolicy")]
	public UniqueKeyPolicy UniqueKeyPolicy { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "uniqueIndexReIndexContext")]
	internal UniqueIndexReIndexContext UniqueIndexReIndexContext { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "conflictResolutionPolicy")]
	public ConflictResolutionPolicy ConflictResolutionPolicy { get; set; }

	[Obsolete("PartitionKeyDeleteThroughputFraction is deprecated.")]
	[System.Text.Json.Serialization.JsonPropertyName(name: "partitionKeyDeleteThroughputFraction")]
	public double PartitionKeyDeleteThroughputFraction { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "statistics")]
	internal IReadOnlyList<object> StatisticsJRaw { get; set; }

	internal bool HasPartitionKey
	{
		get
		{
			return PartitionKey != null;
		}
	}

	internal PartitionKeyInternal NonePartitionKeyValue
	{
		get
		{
			if (PartitionKey.Paths.Count == 0 || PartitionKey.IsSystemKey.GetValueOrDefault(false))
			{
				return PartitionKeyInternal.Empty;
			}
			return PartitionKeyInternal.Undefined;
		}
	}

	[System.Text.Json.Serialization.JsonPropertyName(name: "changeFeedPolicy")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	internal ChangeFeedPolicy ChangeFeedPolicy { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "analyticalStorageTtl")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	internal int? AnalyticalStorageTimeToLive { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "allowMaterializedViews")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	internal bool? AllowMaterializedViews { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "backupPolicy")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	internal CollectionBackupPolicy CollectionBackupPolicy { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "schemaPolicy")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	internal JsonNode SchemaPolicy { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "internalSchemaProperties")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	internal InternalSchemaProperties InternalSchemaProperties { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "clientEncryptionPolicy")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	internal ClientEncryptionPolicy ClientEncryptionPolicy { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "dataMaskingPolicy")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	internal DataMaskingPolicy DataMaskingPolicy { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "materializedViews")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	internal Collection<MaterializedViews> MaterializedViews { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "createMode")]
[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	internal DatabaseOrCollectionCreateMode? CreateMode { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "restoreParameters")]
[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	internal InAccountRestoreParameters RestoreParameters { get; set; }

	internal bool IsMaterializedView()
	{
		if (MaterializedViewDefinition != null)
		{
			if (string.IsNullOrEmpty(MaterializedViewDefinition.SourceCollectionRid))
			{
				return !string.IsNullOrEmpty(MaterializedViewDefinition.SourceCollectionId);
			}
			return true;
		}
		return false;
	}
}
