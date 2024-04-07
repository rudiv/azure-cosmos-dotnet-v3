using System;


namespace Microsoft.Azure.Documents;

internal sealed class MaterializedViews : JsonSerializable, ICloneable
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "id")]
	public string Id
	{ get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "_rid")]
	public string Rid
	{ get; set; }

	public object Clone()
	{
		return new MaterializedViews
		{
			Id = Id,
			Rid = Rid
		};
	}
}
