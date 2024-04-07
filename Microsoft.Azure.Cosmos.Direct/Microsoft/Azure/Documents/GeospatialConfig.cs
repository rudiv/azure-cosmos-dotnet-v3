using System;

namespace Microsoft.Azure.Documents;

using System.Text.Json.Serialization;

internal sealed class GeospatialConfig : JsonSerializable, ICloneable
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "type")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public GeospatialType GeospatialType { get; set; }

	public GeospatialConfig()
	{
		GeospatialType = GeospatialType.Geography;
	}

	public GeospatialConfig(GeospatialType geospatialType)
	{
		GeospatialType = geospatialType;
	}

	public object Clone()
	{
		return new GeospatialConfig
		{
			GeospatialType = GeospatialType
		};
	}
}
