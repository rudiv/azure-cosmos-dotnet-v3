using System;


namespace Microsoft.Azure.Documents;

internal sealed class CollectionBackupPolicy : JsonSerializable, ICloneable
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "type")]
	public CollectionBackupType CollectionBackupType { get; set; }

	public CollectionBackupPolicy()
	{
		CollectionBackupType = CollectionBackupType.Invalid;
	}

	public object Clone()
	{
		return new CollectionBackupPolicy
		{
			CollectionBackupType = CollectionBackupType
		};
	}
}
