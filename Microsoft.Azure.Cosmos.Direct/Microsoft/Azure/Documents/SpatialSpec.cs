using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Microsoft.Azure.Documents;

using System.Text.Json.Serialization;

internal sealed class SpatialSpec : JsonSerializable
{
	private Collection<SpatialType> spatialTypes;

	private BoundingBoxSpec boundingBoxSpec;

	[System.Text.Json.Serialization.JsonPropertyName(name: "path")]
	public string Path { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "types")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
	public Collection<SpatialType> SpatialTypes { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "boundingBox")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public BoundingBoxSpec BoundingBox { get; set; }

	internal object Clone()
	{
		SpatialSpec spatialSpec = new SpatialSpec
		{
			Path = Path
		};
		foreach (SpatialType spatialType in SpatialTypes)
		{
			spatialSpec.SpatialTypes.Add(spatialType);
		}
		if (boundingBoxSpec != null)
		{
			spatialSpec.boundingBoxSpec = (BoundingBoxSpec)boundingBoxSpec.Clone();
		}
		return spatialSpec;
	}
}
