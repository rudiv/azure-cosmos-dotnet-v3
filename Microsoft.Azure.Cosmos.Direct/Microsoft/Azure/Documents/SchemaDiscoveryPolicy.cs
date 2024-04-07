using System;
namespace Microsoft.Azure.Documents;

using System.Text.Json.Serialization;

internal sealed class SchemaDiscoveryPolicy : JsonSerializable, ICloneable
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "mode")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public SchemaBuilderMode SchemaBuilderMode { get; set; }

	public SchemaDiscoveryPolicy()
	{
		SchemaBuilderMode = SchemaBuilderMode.None;
	}

	public object Clone()
	{
		return new SchemaDiscoveryPolicy();
	}

}
