using System.Collections.Generic;


namespace Microsoft.Azure.Documents;

using System.Text.Json;

internal sealed class PartitionKeyRangeStatistics
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "id")]
	public string PartitionKeyRangeId { get; private set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "sizeInKB")]
	public long SizeInKB { get; private set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "documentCount")]
	public long DocumentCount { get; private set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "sampledDistinctPartitionKeyCount")]
	internal long? SampledDistinctPartitionKeyCount { get; private set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "partitionKeys")]
	public IReadOnlyList<PartitionKeyStatistics> PartitionKeyStatistics { get; private set; }

	public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
