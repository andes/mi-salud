using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace XroadssAndesServices.DTOs.Renaper;

public class VerificarUsuarioXroadssErrorDto
{
    [JsonProperty("resultado")]
    [JsonPropertyName("resultado")]
    public string Resultado { get; set; } = string.Empty;

    [JsonProperty("mensaje")]
    [JsonPropertyName("mensaje")]
    public string? Mensaje { get; set; }
}
