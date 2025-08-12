using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

public class SingleOrArrayConverter<T> : JsonConverter<List<T>>
{
    public override List<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.StartArray)
        {
            return JsonSerializer.Deserialize<List<T>>(ref reader, options);
        }
        else
        {
            var single = JsonSerializer.Deserialize<T>(ref reader, options);
            return new List<T> { single };
        }
    }

    public override void Write(Utf8JsonWriter writer, List<T> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}