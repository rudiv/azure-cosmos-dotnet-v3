using System;
using System.Collections.ObjectModel;
using Microsoft.Azure.Documents.Routing;


namespace Microsoft.Azure.Documents;

using System.Text.Json.Serialization;

internal sealed class PartitionKeyRange : Resource, IEquatable<PartitionKeyRange>
{
	internal const string MasterPartitionKeyRangeId = "M";

	[System.Text.Json.Serialization.JsonPropertyName(name: "minInclusive")]
    [JsonInclude]
	internal string MinInclusive { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "maxExclusive")]
    [JsonInclude]
	internal string MaxExclusive { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "ridPrefix")]
    [JsonInclude]
	internal int? RidPrefix
	{ get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "throughputFraction")]
    [JsonInclude]
	internal double ThroughputFraction
	{ get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "status")]
    [JsonConverter(typeof(JsonStringEnumConverter<PartitionKeyRangeStatus>))]
    [JsonInclude]
	internal PartitionKeyRangeStatus Status
	{ get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "lsn")]
    [JsonInclude]
	public long LSN
	{ get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "parents")]
    [JsonInclude]
	public Collection<string> Parents
	{ get; set; }

	internal Range<string> ToRange()
	{
		return new Range<string>(MinInclusive, MaxExclusive, isMinInclusive: true, isMaxInclusive: false);
	}

	public override bool Equals(object obj)
	{
		return Equals(obj as PartitionKeyRange);
	}

	public override int GetHashCode()
	{
		int num = 0;
		num = (num * 397) ^ Id.GetHashCode();
		if (!string.IsNullOrEmpty(ResourceId))
		{
			num = (num * 397) ^ ResourceId.GetHashCode();
		}
		num = (num * 397) ^ MinInclusive.GetHashCode();
		return (num * 397) ^ MaxExclusive.GetHashCode();
	}

	public bool Equals(PartitionKeyRange other)
	{
		if (other == null)
		{
			return false;
		}
		if (Id == other.Id && string.Equals(ResourceId, other.ResourceId, StringComparison.Ordinal) && MinInclusive.Equals(other.MinInclusive) && MaxExclusive.Equals(other.MaxExclusive))
		{
			return ThroughputFraction == other.ThroughputFraction;
		}
		return false;
	}
}
