

namespace Microsoft.Azure.Documents;

internal sealed class ReadPolicy : JsonSerializable
{
	private const int DefaultPrimaryReadCoefficient = 0;

	private const int DefaultSecondaryReadCoefficient = 1;

	[System.Text.Json.Serialization.JsonPropertyName(name: "primaryReadCoefficient")]
	public int PrimaryReadCoefficient { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "secondaryReadCoefficient")]
	public int SecondaryReadCoefficient { get; set; }
}
