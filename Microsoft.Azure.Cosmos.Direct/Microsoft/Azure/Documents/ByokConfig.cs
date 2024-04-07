using System;

namespace Microsoft.Azure.Documents;

using System.Text.Json.Serialization;

internal sealed class ByokConfig : JsonSerializable
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "byokStatus")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public ByokStatus ByokStatus { get; set; }

	public ByokConfig()
	{
		ByokStatus = ByokStatus.None;
	}

	public ByokConfig(ByokStatus byokStatus)
	{
		ByokStatus = byokStatus;
	}
}
