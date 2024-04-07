using System;

namespace Microsoft.Azure.Documents;

using System.Text.Json.Serialization;

internal sealed class CompositePath : JsonSerializable, ICloneable
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "path")]
	public string Path { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "order")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public CompositePathSortOrder Order { get; set; }
    
	public object Clone()
	{
		return new CompositePath
		{
			Path = Path,
			Order = Order
		};
	}
}
