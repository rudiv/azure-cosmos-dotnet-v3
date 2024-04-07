namespace Microsoft.Azure.Documents;

using System.Text.Json.Serialization;

internal class Trigger : Resource
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "body")]
	public string Body
	{ get; set; }

	[JsonConverter(typeof(JsonStringEnumConverter))]
	[JsonPropertyName(name: "triggerType")]
	public TriggerType TriggerType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    [System.Text.Json.Serialization.JsonPropertyName(name: "triggerOperation")]
    public TriggerOperation TriggerOperation { get; set; } = TriggerOperation.All;
}
