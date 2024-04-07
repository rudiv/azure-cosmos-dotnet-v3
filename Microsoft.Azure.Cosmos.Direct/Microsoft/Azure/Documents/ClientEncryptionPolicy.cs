using System;
using System.Collections.ObjectModel;
using System.Globalization;


namespace Microsoft.Azure.Documents;

internal sealed class ClientEncryptionPolicy : JsonSerializable
{
	private Collection<ClientEncryptionIncludedPath> includedPaths;

	[System.Text.Json.Serialization.JsonPropertyName(name: "includedPaths")]
	public Collection<ClientEncryptionIncludedPath> IncludedPaths
    {
        get;
        set;
    }
}
