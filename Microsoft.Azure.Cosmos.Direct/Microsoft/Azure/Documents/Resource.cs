using System;
using System.IO;


namespace Microsoft.Azure.Documents;

using System.Text.Json;
using System.Text.Json.Serialization;

internal abstract class Resource : JsonSerializable
{
	internal static DateTime UnixStartTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

	[System.Text.Json.Serialization.JsonPropertyName(name: "id")]
	public virtual string Id { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "_rid")]
	public virtual string ResourceId { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "_self")]
	public string SelfLink { get; set; }

	[JsonIgnore]
	public string AltLink { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "_ts")]
	[JsonConverter(typeof(UnixDateTimeConverter))]
	public virtual DateTime Timestamp { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "_etag")]
	public string ETag { get; set; }

	protected Resource()
	{
	}

	protected Resource(Resource resource)
	{
		Id = resource.Id;
		ResourceId = resource.ResourceId;
		SelfLink = resource.SelfLink;
		AltLink = resource.AltLink;
		Timestamp = resource.Timestamp;
		ETag = resource.ETag;
	}

	public byte[] ToByteArray()
	{
		using MemoryStream memoryStream = new MemoryStream();
		JsonSerializer.Serialize(memoryStream, this);
		return memoryStream.ToArray();
	}
}
