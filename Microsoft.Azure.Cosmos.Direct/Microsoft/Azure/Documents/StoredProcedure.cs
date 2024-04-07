

namespace Microsoft.Azure.Documents;

internal class StoredProcedure : Resource
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "body")]
	public string Body
	{ get; set; }
}
