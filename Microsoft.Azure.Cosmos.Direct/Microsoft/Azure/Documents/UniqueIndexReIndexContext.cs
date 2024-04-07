using System;
using System.Collections.ObjectModel;
using System.Globalization;


namespace Microsoft.Azure.Documents;

internal sealed class UniqueIndexReIndexContext : JsonSerializable
{
	private Collection<UniqueKey> uniqueKeys;

	[System.Text.Json.Serialization.JsonPropertyName(name: "uniqueKeys")]
	public Collection<UniqueKey> UniqueKeys { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "lastDocumentGLSN")]
	public ulong LastDocumentGLSN { get; set; }

}
