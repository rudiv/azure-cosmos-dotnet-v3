using System;


namespace Microsoft.Azure.Documents;

using System.Text.Json.Serialization;

internal sealed class MaterializedViewDefinition : JsonSerializable, ICloneable
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "sourceCollectionRid")]
	public string SourceCollectionRid
	{ get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "sourceCollectionId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public string SourceCollectionId
	{ get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "definition")]
	public string Definition
	{ get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "apiSpecificDefinition")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public string ApiSpecificDefinition
	{ get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "containerType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public string ContainerType
	{ get; set; }

	public object Clone()
	{
		return new MaterializedViewDefinition
		{
			SourceCollectionRid = SourceCollectionRid,
			Definition = Definition,
			ApiSpecificDefinition = ApiSpecificDefinition,
			ContainerType = ContainerType
		};
	}
}
