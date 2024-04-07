using System.Collections.ObjectModel;

namespace Microsoft.Azure.Documents;

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

internal sealed class UniqueKey : JsonSerializable
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "paths")]
	public Collection<string> Paths { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "filter")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	internal JsonNode? Filter { get; set; }
}
