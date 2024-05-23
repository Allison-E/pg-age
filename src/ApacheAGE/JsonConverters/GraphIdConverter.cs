using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using ApacheAGE.Types;

namespace ApacheAGE.JsonConverters
{
    internal class GraphIdConverter: JsonConverter<GraphId>
    {
        public override GraphId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetUInt64(out ulong ul))
                    return new GraphId(ul);
            }

            throw new InvalidCastException("Cannot parse JSON value to GraphId.");
        }

        public override void Write(Utf8JsonWriter writer, GraphId value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.Value);
        }
    }
}
