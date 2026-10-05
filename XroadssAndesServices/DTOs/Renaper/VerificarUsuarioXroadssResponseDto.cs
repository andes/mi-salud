using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace XroadssAndesServices.DTOs.Renaper;

public class VerificarUsuarioXroadssResponseDto
{
    [JsonProperty("resultado")]
    [JsonPropertyName("resultado")]
    public string Resultado { get; set; } = string.Empty;

    [JsonProperty("mensaje")]
    [JsonPropertyName("mensaje")]
    public string? Mensaje { get; set; }

    [System.Text.Json.Serialization.JsonConverter(typeof(XroadssDataDtoConverter))]
    [JsonProperty("data")]
    [JsonPropertyName("data")]
    public XroadssDataDto? Data { get; set; }
}

/// <summary>
/// RENAPER responde <c>"data": []</c> en los errores y un objeto en los éxitos.
/// </summary>
public class XroadssDataDtoConverter : System.Text.Json.Serialization.JsonConverter<XroadssDataDto?>
{
    public override XroadssDataDto? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.StartArray)
        {
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray) { }
            return null;
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            return System.Text.Json.JsonSerializer.Deserialize<XroadssDataDto>(ref reader, options);
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, XroadssDataDto? value, JsonSerializerOptions options)
    {
        System.Text.Json.JsonSerializer.Serialize(writer, value, options);
    }
}

public class XroadssDataDto
{
    [JsonProperty("id_tramite_principal")]
    [JsonPropertyName("id_tramite_principal")]
    public string IdTramitePrincipal { get; set; } = string.Empty;

    [JsonProperty("id_tramite_tarjeta_reimpresa")]
    [JsonPropertyName("id_tramite_tarjeta_reimpresa")]
    public int IdTramiteTarjetaReimpresa { get; set; }

    [JsonProperty("ejemplar")]
    [JsonPropertyName("ejemplar")]
    public string Ejemplar { get; set; } = string.Empty;

    [JsonProperty("vencimiento")]
    [JsonPropertyName("vencimiento")]
    public string Vencimiento { get; set; } = string.Empty;

    [JsonProperty("emision")]
    [JsonPropertyName("emision")]
    public string Emision { get; set; } = string.Empty;

    [JsonProperty("apellido")]
    [JsonPropertyName("apellido")]
    public string Apellido { get; set; } = string.Empty;

    [JsonProperty("nombres")]
    [JsonPropertyName("nombres")]
    public string Nombres { get; set; } = string.Empty;

    [JsonProperty("fecha_nacimiento")]
    [JsonPropertyName("fecha_nacimiento")]
    public string FechaNacimiento { get; set; } = string.Empty;

    [JsonProperty("cuil")]
    [JsonPropertyName("cuil")]
    public string Cuil { get; set; } = string.Empty;

    [JsonProperty("calle")]
    [JsonPropertyName("calle")]
    public string Calle { get; set; } = string.Empty;

    [JsonProperty("numero")]
    [JsonPropertyName("numero")]
    public string Numero { get; set; } = string.Empty;

    [JsonProperty("piso")]
    [JsonPropertyName("piso")]
    public string Piso { get; set; } = string.Empty;

    [JsonProperty("departamento")]
    [JsonPropertyName("departamento")]
    public string Departamento { get; set; } = string.Empty;

    [JsonProperty("codigo_postal")]
    [JsonPropertyName("codigo_postal")]
    public string CodigoPostal { get; set; } = string.Empty;

    [JsonProperty("barrio")]
    [JsonPropertyName("barrio")]
    public string Barrio { get; set; } = string.Empty;

    [JsonProperty("monoblock")]
    [JsonPropertyName("monoblock")]
    public string Monoblock { get; set; } = string.Empty;

    [JsonProperty("ciudad")]
    [JsonPropertyName("ciudad")]
    public string Ciudad { get; set; } = string.Empty;

    [JsonProperty("municipio")]
    [JsonPropertyName("municipio")]
    public string Municipio { get; set; } = string.Empty;

    [JsonProperty("provincia")]
    [JsonPropertyName("provincia")]
    public string Provincia { get; set; } = string.Empty;

    [JsonProperty("pais")]
    [JsonPropertyName("pais")]
    public string Pais { get; set; } = string.Empty;

    [JsonProperty("nacionalidad")]
    [JsonPropertyName("nacionalidad")]
    public string Nacionalidad { get; set; } = string.Empty;

    [JsonProperty("codigo_fallecido")]
    [JsonPropertyName("codigo_fallecido")]
    public int CodigoFallecido { get; set; }

    [JsonProperty("mensaje_fallecido")]
    [JsonPropertyName("mensaje_fallecido")]
    public string MensajeFallecido { get; set; } = string.Empty;

    [JsonProperty("fecha_fallecimiento")]
    [JsonPropertyName("fecha_fallecimiento")]
    public string FechaFallecimiento { get; set; } = string.Empty;

    [JsonProperty("id_ciudadano")]
    [JsonPropertyName("id_ciudadano")]
    public string IdCiudadano { get; set; } = string.Empty;

    [JsonProperty("codigo")]
    [JsonPropertyName("codigo")]
    public int Codigo { get; set; }

    [JsonProperty("mensaje")]
    [JsonPropertyName("mensaje")]
    public string Mensaje { get; set; } = string.Empty;
}
