using System;
using System.Globalization;

namespace Microsoft.Azure.Documents;

using System.Text.Json;
using System.Text.Json.Serialization;

internal sealed class IndexJsonConverter : JsonConverter<Index>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(Index).IsAssignableFrom(typeToConvert);
    }

    public override Index Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        using (JsonDocument document = JsonDocument.ParseValue(ref reader))
        {
            JsonElement rootElement = document.RootElement;

            if (rootElement.TryGetProperty("kind", out JsonElement kindElement))
            {
                IndexKind result = IndexKind.Hash;
                if (Enum.TryParse<IndexKind>(kindElement.GetString(), ignoreCase: true, out result))
                {
                    Index obj = result switch
                    {
                        IndexKind.Hash => new HashIndex(),
                        IndexKind.Range => new RangeIndex(),
                        IndexKind.Spatial => new SpatialIndex(),
                        _ => throw new JsonException(string.Format(CultureInfo.CurrentCulture, RMResources.InvalidIndexKindValue, result)),
                    };

                    return obj;
                }
                throw new JsonException(string.Format(CultureInfo.CurrentCulture, RMResources.InvalidIndexKindValue, kindElement.GetString()));
            }
            else
            {
                throw new JsonException(string.Format(CultureInfo.CurrentCulture, RMResources.InvalidIndexSpecFormat));
            }
        }
    }

    public override void Write(Utf8JsonWriter writer, Index value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

