

namespace Microsoft.Azure.Documents;

internal class Error : Resource
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "code")]
	public string Code
	{ get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "message")]
	public string Message
	{ get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "errorDetails")]
	internal string ErrorDetails
	{ get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "additionalErrorInfo")]
	internal string AdditionalErrorInfo
	{ get; set; }
}
