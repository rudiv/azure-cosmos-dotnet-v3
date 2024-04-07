using Microsoft.Azure.Documents.Routing;

namespace Microsoft.Azure.Documents;

using System.Text.Json.Serialization;

internal class Permission : Resource
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "resource")]
	public string ResourceLink
	{ get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "resourcePartitionKey")]
	public PartitionKey ResourcePartitionKey { get; set; }

	[JsonConverter(typeof(JsonStringEnumConverter))]
	[System.Text.Json.Serialization.JsonPropertyName(name: "permissionMode")]
	public PermissionMode PermissionMode { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "_token")]
	public string Token { get; set; }
}
