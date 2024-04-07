

namespace Microsoft.Azure.Documents;

internal sealed class Address : Resource
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "isPrimary")]
	public bool IsPrimary { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "protocol")]
	public string Protocol { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "logicalUri")]
	public string LogicalUri { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "physcialUri")]
	public string PhysicalUri { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "partitionIndex")]
	public string PartitionIndex { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "partitionKeyRangeId")]
	public string PartitionKeyRangeId { get; set; }
}
