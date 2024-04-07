using System;

namespace Microsoft.Azure.Documents;

using System.Text.Json.Serialization;

internal sealed class ConflictResolutionPolicy : JsonSerializable
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "mode")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public ConflictResolutionMode Mode { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "conflictResolutionPath")]
	public string ConflictResolutionPath { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "conflictResolutionProcedure")]
	public string ConflictResolutionProcedure { get; set; }

	public ConflictResolutionPolicy()
	{
		Mode = ConflictResolutionMode.LastWriterWins;
	}

}
