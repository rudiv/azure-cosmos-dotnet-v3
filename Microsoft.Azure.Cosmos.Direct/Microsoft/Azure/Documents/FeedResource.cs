using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Azure.Documents;

using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Rntbd;


internal sealed class FeedResource<T> : Resource where T : JsonSerializable, new()
{
	private static string collectionName;

	private static string CollectionName
	{
		get
		{
			if (collectionName == null)
			{
				if (typeof(Document).IsAssignableFrom(typeof(T)))
				{
					collectionName = "Documents";
				}
				else
				{
					collectionName = typeof(T).Name + "s";
				}
			}
			return collectionName;
		}
	}

    [JsonIgnore]
	public int Count => InnerCollection.Count;
    
    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; }

    [JsonIgnore]
    public Collection<T> Documents => InnerCollection;

    internal Collection<T> InnerCollection
	{
		get
        {
            Collection<T> collection = AdditionalProperties[CollectionName].Deserialize<Collection<T>>(DefaultOptions.Json);
			return collection;
		}
		set
        {
            var str = JsonSerializer.Serialize(value, DefaultOptions.Json);
			AdditionalProperties[CollectionName] = JsonDocument.Parse(str).RootElement;
		}
	}
/*
	IEnumerator IEnumerable.GetEnumerator()
	{
		return InnerCollection.GetEnumerator();
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return InnerCollection.GetEnumerator();
	}*/
}
