using System;

namespace Microsoft.Azure.Documents;

using System.Text.Json.Serialization;

internal sealed class SpatialIndex : Index, ICloneable
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "dataType")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public DataType DataType { get; set; }

	internal SpatialIndex()
		: base(IndexKind.Spatial)
	{
	}

	public SpatialIndex(DataType dataType)
		: this()
	{
		DataType = dataType;
	}


	public object Clone()
	{
		return new SpatialIndex(DataType);
	}
}
