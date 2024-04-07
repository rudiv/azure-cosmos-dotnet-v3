using System;

namespace Microsoft.Azure.Documents;

using System.Text.Json.Serialization;

[JsonConverter(typeof(IndexJsonConverter))]
internal abstract class Index : JsonSerializable
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "kind")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public IndexKind Kind { get; set; }

	protected Index(IndexKind kind)
	{
		Kind = kind;
	}

	public static RangeIndex Range(DataType dataType)
	{
		return new RangeIndex(dataType);
	}

	public static RangeIndex Range(DataType dataType, short precision)
	{
		return new RangeIndex(dataType, precision);
	}

	public static HashIndex Hash(DataType dataType)
	{
		return new HashIndex(dataType);
	}

	public static HashIndex Hash(DataType dataType, short precision)
	{
		return new HashIndex(dataType, precision);
	}

	public static SpatialIndex Spatial(DataType dataType)
	{
		return new SpatialIndex(dataType);
	}
}
