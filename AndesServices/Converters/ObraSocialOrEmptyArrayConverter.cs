using System.Text.Json;
using System.Text.Json.Serialization;
using AndesServices.Entities;

namespace AndesServices.Converters
{
    public sealed class ObraSocialOrEmptyArrayConverter : JsonConverter<ObraSocial>
    {
        public override ObraSocial? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                return JsonSerializer.Deserialize<ObraSocial>(ref reader, options);
            }

            if (reader.TokenType == JsonTokenType.StartArray)
            {
                using var document = JsonDocument.ParseValue(ref reader);
                var array = document.RootElement;

                if (array.GetArrayLength() == 0)
                {
                    return null;
                }

                if (array[0].ValueKind == JsonValueKind.Object)
                {
                    return array[0].Deserialize<ObraSocial>(options);
                }
            }

            throw new JsonException($"No se puede convertir el token {reader.TokenType} a ObraSocial");
        }

        public override void Write(Utf8JsonWriter writer, ObraSocial value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}