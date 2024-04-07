using System;


namespace Microsoft.Azure.Documents;

internal sealed class BoundingBoxSpec : JsonSerializable, ICloneable
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "xmin")]
	public double Xmin { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "ymin")]
	public double Ymin { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "xmax")]
	public double Xmax { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "ymax")]
	public double Ymax { get; set; }

	public object Clone()
	{
		return new BoundingBoxSpec
		{
			Xmin = Xmin,
			Ymin = Ymin,
			Xmax = Xmax,
			Ymax = Ymax
		};
	}

}
