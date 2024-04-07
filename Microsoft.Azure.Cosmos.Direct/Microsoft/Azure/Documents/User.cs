namespace Microsoft.Azure.Documents;

using System.Text.Json.Serialization;

internal class User : Resource
{
    [JsonPropertyName("_permissions")]
    public string Permissions { get; set; }

    public string PermissionsLink => base.SelfLink.TrimEnd(new char[1] { '/' }) + "/" + Permissions;
}
