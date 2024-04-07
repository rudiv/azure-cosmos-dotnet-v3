using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.Azure.Documents;

using System.Text.Json.Serialization;

internal sealed class PartitionKeyDefinition : JsonSerializable
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "paths")]
	public Collection<string> Paths { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "kind")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public PartitionKind Kind { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "version")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public PartitionKeyDefinitionVersion? Version { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "systemKey")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? IsSystemKey { get; set; }

	internal static bool AreEquivalent(PartitionKeyDefinition pkd1, PartitionKeyDefinition pkd2)
	{
		if (pkd1.Kind != pkd2.Kind)
		{
			return false;
		}
		if (pkd1.Version != pkd2.Version)
		{
			return false;
		}
		if (!pkd1.Paths.OrderBy((string i) => i).SequenceEqual(pkd2.Paths.OrderBy((string i) => i)))
		{
			return false;
		}
		if (pkd1.IsSystemKey != pkd2.IsSystemKey)
		{
			return false;
		}
		return true;
	}
}
