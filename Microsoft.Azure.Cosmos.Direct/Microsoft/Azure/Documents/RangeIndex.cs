using System;
using System.Globalization;

namespace Microsoft.Azure.Documents;

using System.Text.Json.Serialization;

internal sealed class RangeIndex : Index, ICloneable
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "dataType")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public DataType DataType { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "precision")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public short? Precision { get; set; }

	internal RangeIndex()
		: base(IndexKind.Range)
	{
	}

	public RangeIndex(DataType dataType)
		: this()
	{
		DataType = dataType;
	}

	public RangeIndex(DataType dataType, short precision)
		: this(dataType)
	{
		Precision = precision;
	}

	public object Clone()
	{
		return new RangeIndex(DataType)
		{
			Precision = Precision
		};
	}
}
