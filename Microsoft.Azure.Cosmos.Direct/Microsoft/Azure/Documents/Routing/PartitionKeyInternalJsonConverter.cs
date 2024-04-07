using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Azure.Documents.Routing;

using System.Text.Json;
using System.Text.Json.Serialization;

internal sealed class PartitionKeyInternalJsonConverter : JsonConverter<PartitionKeyInternal>
{
	private const string Type = "type";

	private const string MinNumber = "MinNumber";

	private const string MaxNumber = "MaxNumber";

	private const string MinString = "MinString";

	private const string MaxString = "MaxString";

	private const string Infinity = "Infinity";

	public override void Write(Utf8JsonWriter writer, PartitionKeyInternal value, JsonSerializerOptions options)
    {
        if (value.Equals(PartitionKeyInternal.ExclusiveMaximum))
        {
            writer.WriteStringValue(Infinity);
            return;
        }
        writer.WriteStartArray();
        IEnumerable<IPartitionKeyComponent> components = value.Components;
        foreach (IPartitionKeyComponent item in components ?? Enumerable.Empty<IPartitionKeyComponent>())
        {
            item.JsonEncode(writer);
        }
        writer.WriteEndArray();
    }

    public override PartitionKeyInternal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument document = JsonDocument.ParseValue(ref reader))
        {
            JsonElement jToken = document.RootElement;
            if (jToken.ValueKind == JsonValueKind.String && jToken.GetString() == Infinity)
            {
                return PartitionKeyInternal.ExclusiveMaximum;
            }
            List<object> list = new List<object>();
            if (jToken.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement item in jToken.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.Object)
                    {
                        JsonElement jObject = item;
                        if (!jObject.EnumerateObject().Any())
                        {
                            list.Add(Undefined.Value);
                            continue;
                        }
                        bool flag = false;
                        if (jObject.TryGetProperty(Type, out var value) && value.ValueKind == JsonValueKind.String)
                        {
                            flag = true;
                            string typeValue = value.GetString();
                            if (typeValue == MinNumber)
                            {
                                list.Add(Microsoft.Azure.Documents.Routing.MinNumber.Value);
                            }
                            else if (typeValue == MaxNumber)
                            {
                                list.Add(Microsoft.Azure.Documents.Routing.MaxNumber.Value);
                            }
                            else if (typeValue == MinString)
                            {
                                list.Add(Microsoft.Azure.Documents.Routing.MinString.Value);
                            }
                            else if (typeValue == MaxString)
                            {
                                list.Add(Microsoft.Azure.Documents.Routing.MaxString.Value);
                            }
                            else
                            {
                                flag = false;
                            }
                        }
                        if (!flag)
                        {
                            throw new JsonException(string.Format(CultureInfo.InvariantCulture, RMResources.UnableToDeserializePartitionKeyValue, jToken));
                        }
                    }
                    else
                    {
                        if (item.ValueKind != JsonValueKind.String)
                        {
                            throw new JsonException(string.Format(CultureInfo.InvariantCulture, RMResources.UnableToDeserializePartitionKeyValue, jToken));
                        }
                        list.Add(item.GetString());
                    }
                }
                return PartitionKeyInternal.FromObjectArray(list, strict: true);
            }
            throw new JsonException(string.Format(CultureInfo.InvariantCulture, RMResources.UnableToDeserializePartitionKeyValue, jToken));
        }
    }

    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(PartitionKeyInternal).IsAssignableFrom(typeToConvert);
    }

	public static void JsonEncode(MinNumberPartitionKeyComponent component, Utf8JsonWriter writer)
	{
		JsonEncodeLimit(writer, "MinNumber");
	}

	public static void JsonEncode(MaxNumberPartitionKeyComponent component, Utf8JsonWriter writer)
	{
		JsonEncodeLimit(writer, "MaxNumber");
	}

	public static void JsonEncode(MinStringPartitionKeyComponent component, Utf8JsonWriter writer)
	{
		JsonEncodeLimit(writer, "MinString");
	}

	public static void JsonEncode(MaxStringPartitionKeyComponent component, Utf8JsonWriter writer)
	{
		JsonEncodeLimit(writer, "MaxString");
	}

	private static void JsonEncodeLimit(Utf8JsonWriter writer, string value)
	{
		writer.WriteStartObject();
		writer.WritePropertyName("type");
		writer.WriteStringValue(value);
		writer.WriteEndObject();
	}
}
