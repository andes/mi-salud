using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace XroadssAndesServices.DTOs.Rania;

public class InformeRaniaResponseDto
{
    [JsonProperty("protocolo_id")]
    [JsonPropertyName("protocolo_id")]
    public string? ProtocoloId { get; set; }

    [JsonProperty("informe_url")]
    [JsonPropertyName("informe_url")]
    public string? InformeUrl { get; set; }
}
