using System;
using System.Globalization;

namespace Microsoft.Azure.Documents;

using System.Text.Json.Serialization;

internal sealed class HashIndex : Index, ICloneable
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "dataType")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public DataType DataType { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "precision")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public short? Precision { get; set; }

	internal HashIndex()
		: base(IndexKind.Hash)
	{
	}

	public HashIndex(DataType dataType)
		: this()
	{
		DataType = dataType;
	}

	public HashIndex(DataType dataType, short precision)
		: this(dataType)
	{
		Precision = precision;
	}

	public object Clone()
	{
		return new HashIndex(DataType)
		{
			Precision = Precision
		};
	}
}
