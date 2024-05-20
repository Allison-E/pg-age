using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using ApacheAGE.Data;

namespace ApacheAGE.JsonConverters
{
    internal class PathConverter: JsonConverter<object>
    {
        private int _counter = 0;

        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            /*
             * Every path consists of vertices and edges. It is certain that
             * the first and last elements of a path are vertices. Also, it is
             * certain that an edge exists between two contiguous vertices.
             * Therefore, a path will look like this:
             * path = v -> e -> v ->...-> v -> e -> v.
             * 
             * Because of this, if we use a zero-based counter, we can be sure that
             * all vertices will fall on even numbers and edges will fall on odd numbers.
             */

            string json;
            var serializingOptions = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                Converters = { new InferredObjectConverter() },
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            };

            if (_counter % 2 == 0)
            {
                json = JsonDocument.ParseValue(ref reader).RootElement.GetRawText();
                _counter++;
                return JsonSerializer.Deserialize<Vertex>(json, serializingOptions);
            }

            json = JsonDocument.ParseValue(ref reader).RootElement.GetRawText();
            _counter++;
            return JsonSerializer.Deserialize<Edge>(json, serializingOptions);
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}
