using System;

namespace Microsoft.Azure.Documents;

using System.Text.Json.Serialization;

internal class InAccountRestoreParameters : JsonSerializable
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "instanceId")]
	public string InstanceId { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "restoreTimestampInUtc")]
	public DateTime RestoreTimestampInUtc { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "restoreSource")]
	public string RestoreSource { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "sourceBackupLocation")]
	public string SourceBackupLocation { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "restoreWithTTLDisabled")]
	public bool RestoreWithTTLDisabled
    {
        get;
        set;
    }
}
