namespace Microsoft.Azure.Documents;

using System.Text.Json.Serialization;

internal sealed class ReplicationPolicy : JsonSerializable
{
	private const int DefaultMaxReplicaSetSize = 4;

	private const int DefaultMinReplicaSetSize = 3;

	private const bool DefaultAsyncReplication = false;

    [JsonPropertyName("maxReplicaSetSize")]
	public int MaxReplicaSetSize { get; set; }

    [JsonPropertyName("minReplicaSetSize")]
	public int MinReplicaSetSize { get; set; }

    [JsonPropertyName("asyncReplication")]
	public bool AsyncReplication { get; set; }
}
