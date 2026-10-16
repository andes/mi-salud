using System.Text.Json;
using System.Text.Json.Serialization;
using AndesServices.Entities;

namespace AndesServices.Converters;

/// <summary>
/// Andes may send referencia as an ObjectId string or as a populated patient object.
/// </summary>
public sealed class RelacionReferenciaConverter : JsonConverter<RelacionReferencia?>
{
    public override RelacionReferencia? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return null;
            case JsonTokenType.String:
                var id = reader.GetString();
                return string.IsNullOrEmpty(id)
                    ? null
                    : new RelacionReferencia { _id = id, id = id };
            case JsonTokenType.StartObject:
                return JsonSerializer.Deserialize<RelacionReferencia>(ref reader, options);
            default:
                throw new JsonException($"No se puede convertir el token {reader.TokenType} a RelacionReferencia");
        }
    }

    public override void Write(Utf8JsonWriter writer, RelacionReferencia? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        JsonSerializer.Serialize(writer, value, options);
    }
}
