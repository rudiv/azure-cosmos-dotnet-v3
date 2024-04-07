using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;


namespace Microsoft.Azure.Documents;

using System.Text.Json;
using System.Text.Json.Serialization;

internal sealed class SnapshotContent : JsonSerializable
{
	private Database databaseResource;

	private DocumentCollection collectionResource;

	private IList<PartitionKeyRange> partitionKeyRangeList;

	private SerializableNameValueCollection geoLinkIdToPKRangeRid;

	private IList<ClientEncryptionKey> clientEncryptionKeysList;

	[System.Text.Json.Serialization.JsonPropertyName(name: "operationType")]
	public OperationType OperationType { get; set; }

	[JsonIgnore]
	public Database Database
	{
		get
		{
			if (databaseResource == null && SerializedDatabase != null)
			{
				databaseResource = GetResourceIfDeserialized<Database>(SerializedDatabase);
			}
			return databaseResource;
		}
	}

	[JsonIgnore]
	public DocumentCollection DocumentCollection
	{
		get
		{
			if (collectionResource == null && SerializedCollection != null)
			{
				collectionResource = GetResourceIfDeserialized<DocumentCollection>(SerializedCollection);
			}
			return collectionResource;
		}
	}

	[JsonIgnore]
	public IList<PartitionKeyRange> PartitionKeyRangeList
	{
		get
		{
			if (partitionKeyRangeList == null && SerializedPartitionKeyRanges != null)
			{
				partitionKeyRangeList = new List<PartitionKeyRange>();
				foreach (string serializedPartitionKeyRange in SerializedPartitionKeyRanges)
				{
					partitionKeyRangeList.Add(GetResourceIfDeserialized<PartitionKeyRange>(serializedPartitionKeyRange));
				}
			}
			return partitionKeyRangeList;
		}
	}

	[System.Text.Json.Serialization.JsonPropertyName(name: "geoLinkIdToPKRangeRid")]
	public SerializableNameValueCollection GeoLinkIdToPKRangeRid { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "partitionKeyRangeResourceIds")]
	public IList<string> PartitionKeyRangeResourceIds { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "dataDirectories")]
	public IList<string> DataDirectories { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "metadataDirectory")]
	public string MetadataDirectory { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "databaseContent")]
	public string SerializedDatabase { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "offerContent")]
	public string SerializedOffer { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "collectionContent")]
	public string SerializedCollection { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "isMasterResourcesDeletionPending")]
	public bool? IsMasterResourcesDeletionPending { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "partitionKeyRanges")]
	public IList<string> SerializedPartitionKeyRanges { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "clientEncryptionKeyResources")]
	public IList<string> SerializedClientEncryptionKeyResources { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "storageAccountUris")]
	public IList<string> StorageAccountUris { get; set; }

	[JsonIgnore]
	public IList<ClientEncryptionKey> ClientEncryptionKeysList
	{
		get
		{
			if (clientEncryptionKeysList == null && SerializedClientEncryptionKeyResources != null)
			{
				clientEncryptionKeysList = new List<ClientEncryptionKey>();
				foreach (string serializedClientEncryptionKeyResource in SerializedClientEncryptionKeyResources)
				{
					clientEncryptionKeysList.Add(GetResourceIfDeserialized<ClientEncryptionKey>(serializedClientEncryptionKeyResource));
				}
			}
			return clientEncryptionKeysList;
		}
	}

	[JsonConstructor]
	public SnapshotContent()
	{
	}

	internal SnapshotContent(OperationType operationType, string serializedDatabase, string serializedCollection, string serializedOffer, IList<string> serializedPkranges)
	{
		if (operationType == OperationType.Invalid)
		{
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "operationType"));
		}
		ArgumentStringNotNullOrWhiteSpace(serializedDatabase, "serializedDatabase");
		ArgumentStringNotNullOrWhiteSpace(serializedCollection, "serializedCollection");
		ArgumentStringNotNullOrWhiteSpace(serializedOffer, "serializedOffer");
		if (serializedPkranges == null || serializedPkranges.Count == 0)
		{
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "serializedPkranges"));
		}
		OperationType = operationType;
		SerializedDatabase = serializedDatabase;
		SerializedCollection = serializedCollection;
		SerializedOffer = serializedOffer;
		SerializedPartitionKeyRanges = serializedPkranges;
	}

	internal SnapshotContent(OperationType operationType, string serializedDatabase)
	{
		if (operationType == OperationType.Invalid)
		{
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "operationType"));
		}
		ArgumentStringNotNullOrWhiteSpace(serializedDatabase, "serializedDatabase");
		OperationType = operationType;
		SerializedDatabase = serializedDatabase;
	}

	internal SnapshotContent(OperationType operationType, string serializedDatabase, string serializedOffer, SerializableNameValueCollection geoLinkIdToPKRangeRid)
	{
		if (operationType == OperationType.Invalid)
		{
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "operationType"));
		}
		ArgumentStringNotNullOrWhiteSpace(serializedDatabase, "serializedDatabase");
		ArgumentStringNotNullOrWhiteSpace(serializedOffer, "serializedOffer");
		if (geoLinkIdToPKRangeRid == null || geoLinkIdToPKRangeRid.Collection.Count == 0)
		{
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "geoLinkIdToPKRangeRid"));
		}
		OperationType = operationType;
		SerializedDatabase = serializedDatabase;
		SerializedOffer = serializedOffer;
		GeoLinkIdToPKRangeRid = geoLinkIdToPKRangeRid;
	}

	internal SnapshotContent(OperationType operationType, string serializedDatabase, string serializedCollection, IList<string> serializedPkranges, SerializableNameValueCollection geoLinkIdToPKRangeRid)
	{
		if (operationType == OperationType.Invalid)
		{
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "operationType"));
		}
		ArgumentStringNotNullOrWhiteSpace(serializedDatabase, "serializedDatabase");
		ArgumentStringNotNullOrWhiteSpace(serializedCollection, "serializedCollection");
		if (serializedPkranges == null || serializedPkranges.Count == 0)
		{
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "serializedPkranges"));
		}
		if (geoLinkIdToPKRangeRid == null || geoLinkIdToPKRangeRid.Collection.Count == 0)
		{
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "geoLinkIdToPKRangeRid"));
		}
		OperationType = operationType;
		SerializedDatabase = serializedDatabase;
		SerializedCollection = serializedCollection;
		SerializedPartitionKeyRanges = serializedPkranges;
		GeoLinkIdToPKRangeRid = geoLinkIdToPKRangeRid;
	}


	private T GetResourceIfDeserialized<T>(string body) where T : Resource, new()
	{
		try
        {
            return JsonSerializer.Deserialize<T>(body);
        }
		catch (JsonException)
		{
			return null;
		}
	}

	private void ArgumentStringNotNullOrWhiteSpace(string parameter, string parameterName)
	{
		if (string.IsNullOrWhiteSpace(parameterName))
		{
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "parameterName"));
		}
		if (string.IsNullOrWhiteSpace(parameter))
		{
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, parameterName));
		}
	}
}
