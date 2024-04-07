using System;
using System.Collections.ObjectModel;


namespace Microsoft.Azure.Documents;

internal sealed class EncryptionScopeMetadata : JsonSerializable, ICloneable
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "id")]
	public string Id { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "name")]
	public string Name { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "dataEncryptionKeyStatus")]
	public DataEncryptionKeyStatus DataEncryptionKeyStatus { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "cmkMetadataList")]
	public Collection<CMKMetadataInfo> CMKMetadataList { get; set; }

	public object Clone()
	{
		EncryptionScopeMetadata encryptionScopeMetadata = new EncryptionScopeMetadata
		{
			Id = Id,
			Name = Name,
			DataEncryptionKeyStatus = DataEncryptionKeyStatus
		};
		foreach (CMKMetadataInfo cMKMetadata in CMKMetadataList)
		{
			encryptionScopeMetadata.CMKMetadataList.Add((CMKMetadataInfo)cMKMetadata.Clone());
		}
		return encryptionScopeMetadata;
	}
}
