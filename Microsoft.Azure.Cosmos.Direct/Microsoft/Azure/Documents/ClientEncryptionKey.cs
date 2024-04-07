

namespace Microsoft.Azure.Documents;

internal class ClientEncryptionKey : Resource
{
	private KeyWrapMetadata keyWrapMetadata;

	[System.Text.Json.Serialization.JsonPropertyName(name: "encryptionAlgorithm")]
	internal string EncryptionAlgorithm { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "wrappedDataEncryptionKey")]
	internal byte[] WrappedDataEncryptionKey { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "keyWrapMetadata")]
	internal KeyWrapMetadata KeyWrapMetadata { get; set; }
}
