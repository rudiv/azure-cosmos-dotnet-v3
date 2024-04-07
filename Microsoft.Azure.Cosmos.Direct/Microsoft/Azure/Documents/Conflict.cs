using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.Azure.Documents;

using System.Text.Json.Serialization;

internal class Conflict : Resource
{
    [JsonPropertyName("SourceResourceId")]
	public string SourceResourceId
	{ get; set; }

    [JsonPropertyName("conflictLSN")]
	internal long ConflictLSN
	{ get; set; }

    [JsonPropertyName("operationKind")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
	public OperationKind OperationKind { get; set; }

    [JsonPropertyName("resourceType")] public string ResourceTypeString { get; set; }
	public Type ResourceType
	{
		get
        {
            string value = ResourceTypeString;
			if (string.Equals("document", value, StringComparison.OrdinalIgnoreCase))
			{
				return typeof(Document);
			}
			if (string.Equals("storedProcedure", value, StringComparison.OrdinalIgnoreCase))
			{
				return typeof(StoredProcedure);
			}
			if (string.Equals("trigger", value, StringComparison.OrdinalIgnoreCase))
			{
				return typeof(Trigger);
			}
			if (string.Equals("userDefinedFunction", value, StringComparison.OrdinalIgnoreCase))
			{
				return typeof(UserDefinedFunction);
			}
			return null;
		}
		internal set
		{
			string text = null;
			if (value == typeof(Document))
			{
				text = "document";
			}
			else if (value == typeof(StoredProcedure))
			{
				text = "storedProcedure";
			}
			else if (value == typeof(Trigger))
			{
				text = "trigger";
			}
			else if (value == typeof(UserDefinedFunction))
			{
				text = "userDefinedFunction";
			}
			ResourceTypeString = text;
		}
	}
}
