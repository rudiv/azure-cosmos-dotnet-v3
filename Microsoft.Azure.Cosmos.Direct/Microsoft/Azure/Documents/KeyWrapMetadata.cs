

namespace Microsoft.Azure.Documents;

using System.Text.Json.Serialization;

internal class KeyWrapMetadata : JsonSerializable
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "name")]
[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	internal string Name
	{ get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "type")]
[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	internal string Type
	{ get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "value")]
[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	internal string Value
	{ get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "algorithm")]
[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	internal string Algorithm
	{ get; set; }
}
