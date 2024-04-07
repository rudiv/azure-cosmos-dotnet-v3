using System;


namespace Microsoft.Azure.Documents;

internal sealed class ExcludedPath : JsonSerializable, ICloneable
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "path")]
	public string Path { get; set; }

	public object Clone()
	{
		return new ExcludedPath
		{
			Path = Path
		};
	}
}
