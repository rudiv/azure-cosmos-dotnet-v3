namespace AotTest;

using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Core.Serialization;
using Microsoft.Azure.Cosmos;

public static class DefaultJsonSerializerOptions
{
    public static JsonSerializerOptions Options { get; }

    static DefaultJsonSerializerOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            TypeInfoResolver = Context.Default
        };
        Options = options;
    }
}

[JsonSerializable(typeof(TestDocument))]
[JsonSerializable(typeof(TestDocument[]))]
public partial class Context : JsonSerializerContext
{
    
}

public class CosmosSystemTextJsonSerializer : CosmosSerializer
{
    private readonly JsonObjectSerializer systemTextJsonSerializer;
    
    public static CosmosSystemTextJsonSerializer Default => new (DefaultJsonSerializerOptions.Options);

    public CosmosSystemTextJsonSerializer(JsonSerializerOptions jsonSerializerOptions)
    {
        systemTextJsonSerializer = new JsonObjectSerializer(jsonSerializerOptions);
    }

    public override T FromStream<T>(Stream stream)
    {
        using (stream)
        {
            if (stream.CanSeek
                && stream.Length == 0)
            {
                return default!;
            }

            if (typeof(Stream).IsAssignableFrom(typeof(T)))
            {
                return (T)(object)stream;
            }

            return (T)systemTextJsonSerializer.Deserialize(stream, typeof(T), default)!;
        }
    }

    public override Stream ToStream<T>(T input)
    {
        MemoryStream streamPayload = new MemoryStream();
        if (input != null)
        {
            systemTextJsonSerializer.Serialize(streamPayload, input, input.GetType(), default);
            streamPayload.Position = 0;
        }
        else
        {
            systemTextJsonSerializer.Serialize(streamPayload, default, typeof(T), default);
        }

        return streamPayload;
    }
}
