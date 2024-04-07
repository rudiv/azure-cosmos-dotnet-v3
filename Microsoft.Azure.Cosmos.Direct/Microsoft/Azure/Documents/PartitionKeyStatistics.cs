using Microsoft.Azure.Documents.Routing;


namespace Microsoft.Azure.Documents;

using System.Text.Json;
using System.Text.Json.Serialization;

internal sealed class PartitionKeyStatistics
{
    [JsonIgnore]
	public PartitionKey PartitionKey => PartitionKey.FromInternalKey(PartitionKeyInternal);

	[System.Text.Json.Serialization.JsonPropertyName(name: "sizeInKB")]
	public long SizeInKB { get; private set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "partitionKey")]
	internal PartitionKeyInternal PartitionKeyInternal { get; private set; }

	public override string ToString()
	{
		return JsonSerializer.Serialize(this);
	}
}
