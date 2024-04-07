

namespace Microsoft.Azure.Documents;

using System.Text.Json.Serialization;

internal class Database : Resource
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "_colls")]
    public string CollsInternal { get; set; }
    
	public string CollectionsLink => base.SelfLink.TrimEnd(new char[1] { '/' }) + "/" + CollsInternal;

	[System.Text.Json.Serialization.JsonPropertyName(name: "_users")]
    public string UsersInternal { get; set; }
    
	public string UsersLink => base.SelfLink.TrimEnd(new char[1] { '/' }) + "/" + UsersInternal;

	internal string UserDefinedTypesLink => base.SelfLink.TrimEnd(new char[1] { '/' }) + "/udts/";

	[System.Text.Json.Serialization.JsonPropertyName(name: "createMode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	internal DatabaseOrCollectionCreateMode? CreateMode { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "restoreParameters")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	internal InAccountRestoreParameters RestoreParameters { get; set; }
}
