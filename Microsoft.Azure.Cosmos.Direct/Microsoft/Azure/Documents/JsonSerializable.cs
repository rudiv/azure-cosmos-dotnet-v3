using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;


namespace Microsoft.Azure.Documents;

using System.Text.Json;
using System.Xml;

internal abstract class JsonSerializable
{
    /*
	internal Dictionary<string, JsonElement> propertyBag;

	private const string POCOSerializationOnly = "POCOSerializationOnly";

	internal static bool JustPocoSerialization;

	static JsonSerializable()
	{
		if (int.TryParse(string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("POCOSerializationOnly")) ? "0" : Environment.GetEnvironmentVariable("POCOSerializationOnly"), out var result) && result == 1)
		{
			JustPocoSerialization = true;
		}
		else
		{
			JustPocoSerialization = false;
		}
	}

	internal JsonSerializable()
	{
		propertyBag = new Dictionary<string, JsonElement>();
	}

	public void SaveTo(Stream stream, SerializationFormattingPolicy formattingPolicy = SerializationFormattingPolicy.None)
	{
		SaveTo(stream, formattingPolicy, null);
	}

	public void SaveTo(Stream stream, SerializationFormattingPolicy formattingPolicy, object settings)
	{
		if (stream == null)
		{
			throw new ArgumentNullException("stream");
		}
		SerializerSettings = settings;
		JsonSerializer serializer = ((settings == null) ? new JsonSerializer() : JsonSerializer.Create(settings));
		JsonTextWriter writer = new JsonTextWriter(new StreamWriter(stream));
		SaveTo(writer, serializer, formattingPolicy);
	}

	internal void SaveTo(JsonWriter writer, JsonSerializer serializer, SerializationFormattingPolicy formattingPolicy = SerializationFormattingPolicy.None)
	{
		if (writer == null)
		{
			throw new ArgumentNullException("writer");
		}
		if (serializer == null)
		{
			throw new ArgumentNullException("serializer");
		}
		if (formattingPolicy == SerializationFormattingPolicy.Indented)
		{
			writer.Formatting = Formatting.Indented;
		}
		else
		{
			writer.Formatting = Formatting.None;
		}
		OnSave();
		if ((typeof(Document).IsAssignableFrom(GetType()) && !GetType().Equals(typeof(Document))) || (typeof(Attachment).IsAssignableFrom(GetType()) && !GetType().Equals(typeof(Attachment))))
		{
			serializer.Serialize(writer, this);
		}
		else if (JustPocoSerialization)
		{
			propertyBag.WriteTo(writer);
		}
		else
		{
			serializer.Serialize(writer, propertyBag);
		}
		writer.Flush();
	}

	internal void SaveTo(StringBuilder stringBuilder, SerializationFormattingPolicy formattingPolicy = SerializationFormattingPolicy.None)
	{
		if (stringBuilder == null)
		{
			throw new ArgumentNullException("stringBuilder");
		}
		SaveTo(new JsonTextWriter(new StringWriter(stringBuilder, CultureInfo.CurrentCulture)), new JsonSerializer(), formattingPolicy);
	}

	public virtual void LoadFrom(Utf8JsonReader reader)
	{
		if (reader == null)
		{
			throw new ArgumentNullException("reader");
		}
		propertyBag = JsonElement.ParseValue(ref reader);
	}

	public virtual void LoadFrom(Utf8JsonReader reader, object serializerSettings)
    {
        return LoadFrom(reader);
    }

	public static T LoadFrom<T>(Stream stream) where T : JsonSerializable, new()
	{
		return LoadFrom<T>(stream, null);
	}

	internal static T LoadFrom<T>(Stream stream, ITypeResolver<T> typeResolver, object settings = null) where T : JsonSerializable, new()
	{
		if (stream == null)
		{
			throw new ArgumentNullException("stream");
		}
		return LoadFrom(new JsonTextReader(new StreamReader(stream)), typeResolver, settings);
	}

	internal static T LoadFromWithResolver<T>(Stream stream, ITypeResolver<T> typeResolver, JsonSerializerSettings settings = null) where T : JsonSerializable
	{
		if (stream == null)
		{
			throw new ArgumentNullException("stream");
		}
		if (typeResolver == null)
		{
			throw new ArgumentNullException("typeResolver");
		}
		JsonTextReader jsonReader = new JsonTextReader(new StreamReader(stream));
		return LoadFromWithResolver(typeResolver, settings, jsonReader);
	}

	internal static T LoadFromWithResolver<T>(string serialized, ITypeResolver<T> typeResolver, JsonSerializerSettings settings = null) where T : JsonSerializable
	{
		if (serialized == null)
		{
			throw new ArgumentNullException("serialized");
		}
		if (typeResolver == null)
		{
			throw new ArgumentNullException("typeResolver");
		}
		JsonTextReader jsonReader = new JsonTextReader(new StringReader(serialized));
		return LoadFromWithResolver(typeResolver, settings, jsonReader);
	}

	internal static T LoadFrom<T>(string serialized, ITypeResolver<T> typeResolver, JsonSerializerSettings settings = null) where T : JsonSerializable, new()
	{
		if (serialized == null)
		{
			throw new ArgumentNullException("serialized");
		}
		return LoadFrom(new JsonTextReader(new StringReader(serialized)), typeResolver, settings);
	}

	public static T LoadFromWithConstructor<T>(Stream stream, Func<T> constructorFunction)
	{
		return LoadFromWithConstructor(stream, constructorFunction, null);
	}

	public static T LoadFromWithConstructor<T>(Stream stream, Func<T> constructorFunction, JsonSerializerSettings settings)
	{
		if (stream == null)
		{
			throw new ArgumentNullException("stream");
		}
		if (!typeof(T).IsSubclassOf(typeof(JsonSerializable)))
		{
			throw new ArgumentException("type is not serializable");
		}
		T val = constructorFunction();
		JsonTextReader reader = new JsonTextReader(new StreamReader(stream));
		((JsonSerializable)(object)val).LoadFrom(reader, settings);
		return val;
	}

	public override string ToString()
	{
		OnSave();
		return propertyBag.ToString();
	}

	internal virtual void Validate()
	{
	}

	internal T GetValue<T>(string propertyName)
    {
		if (propertyBag != null)
		{
			JsonElement jToken = propertyBag[propertyName];
			if (jToken != null)
			{
				if (typeof(T).IsEnum() && jToken.ValueKind == JsonValueKind.String)
				{
					if (Enum.TryParse<T>(jToken.GetString(), out T res))
                    {
                        return res;
                    }
                    else
                    {
                        return default(T);
                    }
				}
				if (SerializerSettings != null)
				{
					return jToken.ToObject<T>(JsonSerializer.Create(SerializerSettings));
				}
				return jToken.ToObject<T>();
			}
		}
		return default(T);
	}

	internal T GetValue<T>(string propertyName, T defaultValue)
	{
		if (propertyBag != null)
		{
			JToken jToken = propertyBag[propertyName];
			if (jToken != null)
			{
				if (typeof(T).IsEnum() && jToken.Type == JTokenType.String)
				{
					return jToken.ToObject<T>(JsonSerializer.CreateDefault());
				}
				if (SerializerSettings != null)
				{
					return jToken.ToObject<T>(JsonSerializer.Create(SerializerSettings));
				}
				return jToken.ToObject<T>();
			}
		}
		return defaultValue;
	}

	internal TEnum? GetEnumValue<TEnum>(string propertyName) where TEnum : struct
	{
		if (!typeof(TEnum).IsEnum())
		{
			throw new ArgumentException($"{typeof(TEnum)} is not an Enum.");
		}
		string value = GetValue<string>(propertyName);
		if (string.IsNullOrWhiteSpace(value))
		{
			return null;
		}
		if (!Enum.TryParse<TEnum>(value, ignoreCase: true, out var result) || !Enum.IsDefined(typeof(TEnum), result))
		{
			throw new BadRequestException("Could not parse [" + value + "] as a valid enum value for property [" + propertyName + "].");
		}
		return result;
	}

	internal T GetValueByPath<T>(string[] fieldNames, T defaultValue)
	{
		if (fieldNames == null)
		{
			throw new ArgumentNullException("fieldNames");
		}
		if (fieldNames.Length == 0)
		{
			throw new ArgumentException("fieldNames is empty.");
		}
		if (propertyBag != null)
		{
			JToken jToken = propertyBag[fieldNames[0]];
			for (int i = 1; i < fieldNames.Length; i++)
			{
				if (jToken == null)
				{
					break;
				}
				jToken = ((jToken is JObject) ? jToken[fieldNames[i]] : null);
			}
			if (jToken != null)
			{
				if (typeof(T).IsEnum() && jToken.Type == JTokenType.String)
				{
					return jToken.ToObject<T>(JsonSerializer.CreateDefault());
				}
				if (SerializerSettings != null)
				{
					return jToken.ToObject<T>(JsonSerializer.Create(SerializerSettings));
				}
				return jToken.ToObject<T>();
			}
		}
		return defaultValue;
	}

	internal void SetValue(string name, object value)
	{
		if (propertyBag == null)
		{
			propertyBag = new JObject();
		}
		if (value != null)
		{
			propertyBag[name] = JToken.FromObject(value);
		}
		else
		{
			propertyBag.Remove(name);
		}
	}

	internal void SetValueByPath<T>(string[] fieldNames, T value)
	{
		if (fieldNames == null)
		{
			throw new ArgumentNullException("fieldNames");
		}
		if (fieldNames.Length == 0)
		{
			throw new ArgumentException("fieldNames is empty.");
		}
		if (propertyBag == null)
		{
			propertyBag = new JObject();
		}
		JToken jToken = propertyBag;
		for (int i = 0; i < fieldNames.Length - 1; i++)
		{
			if (jToken[fieldNames[i]] == null)
			{
				jToken[fieldNames[i]] = new JObject();
			}
			jToken = jToken[fieldNames[i]];
		}
		JObject jObject = jToken as JObject;
		if (value == null && jObject != null)
		{
			jObject.Remove(fieldNames[^1]);
		}
		else
		{
			jToken[fieldNames[^1]] = ((value == null) ? null : JToken.FromObject(value));
		}
	}

	internal Collection<T> GetValueCollection<T>(string propertyName)
	{
		if (propertyBag != null)
		{
			JToken jToken = propertyBag[propertyName];
			if (jToken != null)
			{
				Collection<JToken> collection = jToken.ToObject<Collection<JToken>>();
				Collection<T> collection2 = new Collection<T>();
				{
					foreach (JToken item in collection)
					{
						if (item != null)
						{
							if (typeof(T).IsEnum() && item.Type == JTokenType.String)
							{
								collection2.Add(item.ToObject<T>(JsonSerializer.CreateDefault()));
							}
							else
							{
								collection2.Add((SerializerSettings == null) ? item.ToObject<T>() : item.ToObject<T>(JsonSerializer.Create(SerializerSettings)));
							}
						}
						else
						{
							collection2.Add(default(T));
						}
					}
					return collection2;
				}
			}
		}
		return null;
	}

	internal void SetValueCollection<T>(string propertyName, Collection<T> value)
	{
		if (propertyBag == null)
		{
			propertyBag = new JObject();
		}
		if (value != null)
		{
			Collection<JToken> collection = new Collection<JToken>();
			foreach (T item in value)
			{
				if (item != null)
				{
					collection.Add(JToken.FromObject(item));
				}
				else
				{
					collection.Add(null);
				}
			}
			propertyBag[propertyName] = JToken.FromObject(collection);
		}
		else
		{
			propertyBag.Remove(propertyName);
		}
	}

	internal TSerializable GetObject<TSerializable>(string propertyName, bool returnEmptyObject = false) where TSerializable : JsonSerializable, new()
	{
		if (propertyBag != null)
		{
			JToken jToken = propertyBag[propertyName];
			if (jToken != null && (returnEmptyObject || jToken.HasValues))
			{
				return new TSerializable
				{
					propertyBag = JObject.FromObject(jToken)
				};
			}
		}
		return null;
	}

	internal TSerializable GetObjectWithResolver<TSerializable>(string propertyName, ITypeResolver<TSerializable> typeResolver, bool returnEmptyObject = false) where TSerializable : JsonSerializable
	{
		if (propertyBag != null)
		{
			JToken jToken = propertyBag[propertyName];
			if (jToken != null && (returnEmptyObject || jToken.HasValues))
			{
				if (jToken is JObject)
				{
					return typeResolver.Resolve(jToken as JObject);
				}
				throw new ArgumentException($"Cannot resolve property type. The property {propertyName} is not an object, it is a {jToken.Type}.");
			}
		}
		return null;
	}

	internal void SetObject<TSerializable>(string propertyName, TSerializable value) where TSerializable : JsonSerializable
	{
		if (propertyBag == null)
		{
			propertyBag = new JObject();
		}
		propertyBag[propertyName] = value?.propertyBag;
	}

	internal Collection<TSerializable> GetObjectCollection<TSerializable>(string propertyName, Type resourceType = null, string ownerName = null, ITypeResolver<TSerializable> typeResolver = null) where TSerializable : JsonSerializable, new()
	{
		if (propertyBag != null)
		{
			JToken jToken = propertyBag[propertyName];
			if (typeResolver == null)
			{
				typeResolver = GetTypeResolver<TSerializable>();
			}
			if (jToken != null)
			{
				Collection<JObject> collection = jToken.ToObject<Collection<JObject>>();
				Collection<TSerializable> collection2 = new Collection<TSerializable>();
				{
					foreach (JObject item in collection)
					{
						if (item != null)
						{
							TSerializable val = ((typeResolver != null) ? typeResolver.Resolve(item) : new TSerializable());
							val.propertyBag = item;
							if (PathsHelper.IsPublicResource(typeof(TSerializable)))
							{
								Resource resource = val as Resource;
								resource.AltLink = PathsHelper.GeneratePathForNameBased(resourceType, ownerName, resource.Id);
							}
							collection2.Add(val);
						}
					}
					return collection2;
				}
			}
		}
		return null;
	}

	internal Collection<TSerializable> GetObjectCollectionWithResolver<TSerializable>(string propertyName, ITypeResolver<TSerializable> typeResolver) where TSerializable : JsonSerializable
	{
		if (propertyBag != null)
		{
			JToken jToken = propertyBag[propertyName];
			if (jToken != null)
			{
				Collection<JObject> collection = jToken.ToObject<Collection<JObject>>();
				Collection<TSerializable> collection2 = new Collection<TSerializable>();
				{
					foreach (JObject item in collection)
					{
						if (item != null)
						{
							TSerializable val = typeResolver.Resolve(item);
							val.propertyBag = item;
							collection2.Add(val);
						}
					}
					return collection2;
				}
			}
		}
		return null;
	}

	internal void SetObjectCollection<TSerializable>(string propertyName, Collection<TSerializable> value) where TSerializable : JsonSerializable
	{
		if (propertyBag == null)
		{
			propertyBag = new JObject();
		}
		if (value == null)
		{
			return;
		}
		Collection<JObject> collection = new Collection<JObject>();
		foreach (TSerializable item in value)
		{
			item.OnSave();
			collection.Add(item.propertyBag ?? new JObject());
		}
		propertyBag[propertyName] = JToken.FromObject(collection);
	}

	internal Dictionary<string, TSerializable> GetObjectDictionary<TSerializable>(string propertyName, ITypeResolver<TSerializable> typeResolver = null, IEqualityComparer<string> comparer = null) where TSerializable : JsonSerializable, new()
	{
		if (propertyBag != null)
		{
			JToken jToken = propertyBag[propertyName];
			if (typeResolver == null)
			{
				typeResolver = GetTypeResolver<TSerializable>();
			}
			if (jToken != null)
			{
				Dictionary<string, TSerializable> dictionary = ((comparer == null) ? new Dictionary<string, TSerializable>() : new Dictionary<string, TSerializable>(comparer));
				{
					foreach (KeyValuePair<string, JObject> item in jToken.ToObject<Dictionary<string, JObject>>())
					{
						TSerializable val = ((typeResolver != null) ? typeResolver.Resolve(item.Value) : new TSerializable());
						val.propertyBag = item.Value;
						dictionary.Add(item.Key, val);
					}
					return dictionary;
				}
			}
		}
		return null;
	}

	internal Dictionary<string, TSerializable> GetObjectDictionaryWithNullableValues<TSerializable>(string propertyName) where TSerializable : JsonSerializable, new()
	{
		if (propertyBag != null)
		{
			JToken jToken = propertyBag[propertyName];
			if (jToken != null)
			{
				Dictionary<string, JObject> dictionary = jToken.ToObject<Dictionary<string, JObject>>();
				if (dictionary == null)
				{
					return null;
				}
				Dictionary<string, TSerializable> dictionary2 = new Dictionary<string, TSerializable>();
				{
					foreach (KeyValuePair<string, JObject> item in dictionary)
					{
						dictionary2.Add(value: (item.Value != null) ? new TSerializable
						{
							propertyBag = item.Value
						} : null, key: item.Key);
					}
					return dictionary2;
				}
			}
		}
		return null;
	}

	internal void SetObjectDictionary<TSerializable>(string propertyName, Dictionary<string, TSerializable> value) where TSerializable : JsonSerializable, new()
	{
		if (propertyBag == null)
		{
			propertyBag = new JObject();
		}
		if (value == null)
		{
			return;
		}
		Dictionary<string, JObject> dictionary = new Dictionary<string, JObject>();
		foreach (KeyValuePair<string, TSerializable> item in value)
		{
			item.Value.OnSave();
			dictionary.Add(item.Key, item.Value.propertyBag ?? new JObject());
		}
		propertyBag[propertyName] = JToken.FromObject(dictionary);
	}

	internal void SetObjectDictionaryWithNullableValues<TSerializable>(string propertyName, Dictionary<string, TSerializable> value) where TSerializable : JsonSerializable, new()
	{
		if (propertyBag == null)
		{
			propertyBag = new JObject();
		}
		if (value == null)
		{
			return;
		}
		Dictionary<string, JObject> dictionary = new Dictionary<string, JObject>();
		foreach (KeyValuePair<string, TSerializable> item in value)
		{
			if (item.Value != null)
			{
				item.Value.OnSave();
				dictionary.Add(item.Key, item.Value.propertyBag ?? new JObject());
			}
			else
			{
				dictionary.Add(item.Key, null);
			}
		}
		propertyBag[propertyName] = JToken.FromObject(dictionary);
	}

	internal virtual void OnSave()
	{
	}

	internal static ITypeResolver<TResource> GetTypeResolver<TResource>() where TResource : JsonSerializable, new()
	{
		ITypeResolver<TResource> result = null;
		if (typeof(TResource) == typeof(Offer))
		{
			result = (ITypeResolver<TResource>)OfferTypeResolver.ResponseOfferTypeResolver;
		}
		return result;
	}

	private static T LoadFrom<T>(JsonTextReader jsonReader, ITypeResolver<T> typeResolver, JsonSerializerSettings settings = null) where T : JsonSerializable, new()
	{
		T val = new T();
		val.LoadFrom(jsonReader, settings);
		return (typeResolver != null) ? typeResolver.Resolve(val.propertyBag) : val;
	}

	private static T LoadFromWithResolver<T>(ITypeResolver<T> typeResolver, JsonSerializerSettings settings, JsonTextReader jsonReader) where T : JsonSerializable
	{
		Helpers.SetupJsonReader(jsonReader, settings);
		JObject jObject = JObject.Load(jsonReader);
		return typeResolver.Resolve(jObject);
	}*/
}
