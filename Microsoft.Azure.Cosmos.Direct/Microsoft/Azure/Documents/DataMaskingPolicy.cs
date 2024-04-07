using System;
using System.Collections.ObjectModel;
using System.Globalization;


namespace Microsoft.Azure.Documents;

internal sealed class DataMaskingPolicy : JsonSerializable
{
	private Collection<DataMaskingIncludedPath> includedPaths;

	private bool isPolicyEnabled;

	[System.Text.Json.Serialization.JsonPropertyName(name: "includedPaths")]
	public Collection<DataMaskingIncludedPath> IncludedPaths { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "isPolicyEnabled")]
	public bool IsPolicyEnabled { get; set; }
}
