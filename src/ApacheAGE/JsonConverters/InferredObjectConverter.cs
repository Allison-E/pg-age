using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApacheAGE.JsonConverters;
internal class InferredObjectConverter: JsonConverter<object>
{
    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.StartArray:
                return JsonDocument.ParseValue(ref reader).Deserialize<List<object?>>(options);

            case JsonTokenType.String:
                if ((options.NumberHandling & JsonNumberHandling.AllowNamedFloatingPointLiterals) != 0)
                {
                    if (reader.GetString()!.Equals("Infinity", StringComparison.OrdinalIgnoreCase))
                        return double.PositiveInfinity;
                    if (reader.GetString()!.Equals("-Infinity", StringComparison.OrdinalIgnoreCase))
                        return double.NegativeInfinity;
                    if (reader.GetString()!.Equals("NaN", StringComparison.OrdinalIgnoreCase))
                        return double.NaN;
                }
                return reader.GetString()!;

            case JsonTokenType.Number:
                if (reader.TryGetInt32(out int integer))
                    return integer;
                else if (reader.TryGetInt64(out long @long))
                    return @long;
                else if (reader.TryGetDecimal(out decimal @decimal))
                    return @decimal;
                else
                    return reader.GetDouble();

            case JsonTokenType.True:
                return true;

            case JsonTokenType.False:
                return false;

            case JsonTokenType.Null:
                return null;

            default:
                return JsonDocument.ParseValue(ref reader).RootElement.Clone();
        }
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
