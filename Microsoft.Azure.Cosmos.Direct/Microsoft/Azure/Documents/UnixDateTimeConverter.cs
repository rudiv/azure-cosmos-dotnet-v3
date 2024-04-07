using System;
using System.Globalization;

namespace Microsoft.Azure.Documents;

using System.Text.Json;
using System.Text.Json.Serialization;

internal sealed class UnixDateTimeConverter : JsonConverter<DateTime>
{
    private static DateTime UnixStartTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        long value2 = (long)(value - UnixStartTime).TotalSeconds;
        writer.WriteNumberValue(value2);
    }

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.Number)
        {
            throw new Exception(RMResources.DateTimeConverterInvalidReaderValue);
        }
        double num = reader.GetDouble();
        return UnixStartTime.AddSeconds(num);
    }
}