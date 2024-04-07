using System;


namespace Microsoft.Azure.Documents;

internal sealed class CMKMetadataInfo : JsonSerializable, ICloneable
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "keyVaultKeyUri")]
	public string KeyVaultKeyUri
	{ get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "defaultIdentity")]
	public string DefaultIdentity
	{ get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "msiClientId")]
	public string MsiClientId
	{ get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "msiClientSecretEncrypted")]
	public string MsiClientSecretEncrypted
	{ get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "wrappedDek")]
	public string WrappedDek
	{ get; set; }

	public object Clone()
	{
		return new CMKMetadataInfo
		{
			KeyVaultKeyUri = KeyVaultKeyUri,
			DefaultIdentity = DefaultIdentity,
			MsiClientId = MsiClientId,
			MsiClientSecretEncrypted = MsiClientSecretEncrypted,
			WrappedDek = WrappedDek
		};
	}
}
