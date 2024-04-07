using System;
using System.Globalization;

namespace Microsoft.Azure.Documents;

using System.Text.Json.Serialization;

internal sealed class Schema : Resource
{
    [JsonPropertyName("resource")]
	public string ResourceLink { get; set; }

	internal static Schema FromObject(object schema)
	{
		return null;
	}
}
