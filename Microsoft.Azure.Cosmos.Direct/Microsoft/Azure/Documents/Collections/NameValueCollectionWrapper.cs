using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Microsoft.Azure.Documents.Collections;

internal class NameValueCollectionWrapper : INameValueCollection, IEnumerable
{
	private NameValueCollection collection;

	public string this[string key]
	{
		get
		{
			return collection[key];
		}
		set
		{
			collection[key] = value;
		}
	}

	public NameValueCollectionWrapper()
	{
		collection = new NameValueCollection();
	}

	public NameValueCollectionWrapper(int capacity)
	{
		collection = new NameValueCollection(capacity);
	}

	public NameValueCollectionWrapper(StringComparer comparer)
	{
		collection = new NameValueCollection(comparer);
	}

	public NameValueCollectionWrapper(NameValueCollectionWrapper values)
	{
		collection = new NameValueCollection(values.collection);
	}

	public NameValueCollectionWrapper(NameValueCollection collection)
	{
		this.collection = new NameValueCollection(collection);
	}

	public NameValueCollectionWrapper(INameValueCollection collection)
	{
		if (collection == null)
		{
			throw new ArgumentNullException("collection");
		}
		this.collection = new NameValueCollection();
		foreach (string item in collection)
		{
			this.collection.Add(item, collection[item]);
		}
	}

	public static NameValueCollectionWrapper Create(NameValueCollection collection)
	{
		return new NameValueCollectionWrapper
		{
			collection = collection
		};
	}

	public void Add(INameValueCollection c)
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		if (c is NameValueCollectionWrapper nameValueCollectionWrapper)
		{
			collection.Add(nameValueCollectionWrapper.collection);
			return;
		}
		foreach (string item in c)
		{
			string[] values = c.GetValues(item);
			foreach (string value in values)
			{
				collection.Add(item, value);
			}
		}
	}

	public void Add(string key, string value)
	{
		collection.Add(key, value);
	}

	public INameValueCollection Clone()
	{
		return new NameValueCollectionWrapper(this);
	}

	public string Get(string key)
	{
		return collection.Get(key);
	}

	public IEnumerator GetEnumerator()
	{
		return collection.GetEnumerator();
	}

	public string[] GetValues(string key)
	{
		return collection.GetValues(key);
	}

	public void Remove(string key)
	{
		collection.Remove(key);
	}

	public void Clear()
	{
		collection.Clear();
	}

	public int Count()
	{
		return collection.Count;
	}

	public void Set(string key, string value)
	{
		collection.Set(key, value);
	}

	public string[] AllKeys()
	{
		return collection.AllKeys;
	}

	public IEnumerable<string> Keys()
	{
		foreach (string key in collection.Keys)
		{
			yield return key;
		}
	}

	public NameValueCollection ToNameValueCollection()
	{
		return collection;
	}
}
