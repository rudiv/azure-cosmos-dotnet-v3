using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


namespace Microsoft.Azure.Documents;

using System.Text.Json;
using System.Text.Json.Serialization;

internal class Document : Resource
{
    [JsonPropertyName("_attachments")]
    public string Attachments { get; set; }
    
	public string AttachmentsLink => base.SelfLink.TrimEnd(new char[1] { '/' }) + "/" + Attachments;

	[System.Text.Json.Serialization.JsonPropertyName(name: "ttl")]
[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public int? TimeToLive
	{ get; set; }

	internal static Document FromObject(object document, object settings = null)
	{
		if (document != null)
        {
            return document as Document;
        }
		return null;
	}
}
