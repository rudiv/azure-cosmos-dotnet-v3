using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace Microsoft.Azure.Documents;

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

internal sealed class SerializableNameValueCollection : JsonSerializable
{
	private Lazy<NameValueCollection> lazyCollection;

	[JsonIgnore]
	public NameValueCollection Collection => lazyCollection.Value;

	public SerializableNameValueCollection()
	{
		lazyCollection = new Lazy<NameValueCollection>(Init);
	}

	private NameValueCollection Init()
	{
		NameValueCollection nameValueCollection = new NameValueCollection();
		return nameValueCollection;
	}

	public override bool Equals(object obj)
	{
		return Equals(obj as SerializableNameValueCollection);
	}

	public bool Equals(SerializableNameValueCollection collection)
	{
		if (collection == null)
		{
			return false;
		}
		if (this == collection)
		{
			return true;
		}
		return IsEqual(collection);
	}

	private bool IsEqual(SerializableNameValueCollection serializableNameValueCollection)
	{
		if (Collection.Count != serializableNameValueCollection.Collection.Count)
		{
			return false;
		}
		string[] allKeys = Collection.AllKeys;
		foreach (string name in allKeys)
		{
			if (Collection[name] != serializableNameValueCollection.Collection[name])
			{
				return false;
			}
		}
		return true;
	}

	public override int GetHashCode()
	{
		int num = 0;
		foreach (string item in Collection)
		{
			num = (num * 397) ^ item.GetHashCode();
			num = (num * 397) ^ ((Collection.Get(item) != null) ? Collection.Get(item).GetHashCode() : 0);
		}
		return num;
	}
}
