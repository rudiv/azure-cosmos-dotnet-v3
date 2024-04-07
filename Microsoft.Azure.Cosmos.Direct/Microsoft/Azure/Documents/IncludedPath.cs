using System;
using System.Collections.ObjectModel;
using System.Globalization;


namespace Microsoft.Azure.Documents;

internal sealed class IncludedPath : JsonSerializable, ICloneable
{
	private Collection<Index> indexes;

	[System.Text.Json.Serialization.JsonPropertyName(name: "path")]
	public string Path { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "indexes")]
	public Collection<Index> Indexes { get; set; }

	public object Clone()
	{
		IncludedPath includedPath = new IncludedPath
		{
			Path = Path
		};
		foreach (Index index in Indexes)
		{
			includedPath.Indexes.Add(index);
		}
		return includedPath;
	}
}
