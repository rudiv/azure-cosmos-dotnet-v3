

namespace Microsoft.Azure.Documents;

internal sealed class ClientEncryptionIncludedPath : JsonSerializable
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "path")]
	public string Path
	{ get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "clientEncryptionKeyId")]
	public string ClientEncryptionKeyId
	{ get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "encryptionType")]
	public string EncryptionType
	{ get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "encryptionAlgorithm")]
	public string EncryptionAlgorithm
	{ get; set; }
}
