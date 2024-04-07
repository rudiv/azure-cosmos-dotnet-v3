using System;
using System.Globalization;


namespace Microsoft.Azure.Documents;

using System.Text.Json.Serialization;

internal class Snapshot : Resource
{
	private new static DateTime UnixStartTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

	private SnapshotContent snapshotContent;

	[System.Text.Json.Serialization.JsonPropertyName(name: "resource")]
	public string ResourceLink { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "state")]
	public SnapshotState State { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "kind")]
	public SnapshotKind Kind { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "snapshotTimestamp")]
	[JsonConverter(typeof(UnixDateTimeConverter))]
	public DateTime SnapshotTimestamp { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "ownerResourceId")]
	internal string OwnerResourceId { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "sizeInKB")]
	public ulong SizeInKB { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "compressedSizeInKB")]
	public ulong CompressedSizeInKB { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "lsn")]
	internal long LSN { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "content")]
	internal SnapshotContent Content { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "parentResourceId")]
	internal string ParentResourceId { get; set; }


	internal static Snapshot CloneSystemSnapshot(Snapshot existingSnapshot, OperationType operationType, bool inheritSnapshotTimestamp)
	{
		if (existingSnapshot.Kind != SnapshotKind.System)
		{
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid snapshot kind {0}", existingSnapshot.Kind));
		}
		Snapshot snapshot = new Snapshot();
		snapshot.Kind = existingSnapshot.Kind;
		snapshot.OwnerResourceId = existingSnapshot.OwnerResourceId;
		snapshot.ResourceLink = existingSnapshot.ResourceLink;
		snapshot.Content = new SnapshotContent
		{
			OperationType = operationType,
			SerializedDatabase = existingSnapshot.Content.SerializedDatabase,
			SerializedCollection = existingSnapshot.Content.SerializedCollection,
			SerializedOffer = existingSnapshot.Content.SerializedOffer,
			SerializedPartitionKeyRanges = existingSnapshot.Content.SerializedPartitionKeyRanges,
			GeoLinkIdToPKRangeRid = existingSnapshot.Content.GeoLinkIdToPKRangeRid
		};
		if (inheritSnapshotTimestamp)
		{
			snapshot.SnapshotTimestamp = existingSnapshot.SnapshotTimestamp;
		}
		else
		{
			snapshot.SnapshotTimestamp = UnixStartTime;
		}
		snapshot.State = SnapshotState.Completed;
		return snapshot;
	}
}
