

namespace Microsoft.Azure.Documents;

internal sealed class DataMaskingIncludedPath : JsonSerializable
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "path")]
	public string Path
	{ get; set; }
}
