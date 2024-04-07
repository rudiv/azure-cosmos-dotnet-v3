using System;


namespace Microsoft.Azure.Documents;

internal sealed class ComputedProperty : JsonSerializable, ICloneable
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "name")]
	public string Name { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "query")]
	public string Query { get; set; }

	public object Clone()
	{
		return new ComputedProperty
		{
			Name = Name,
			Query = Query
		};
	}
}
