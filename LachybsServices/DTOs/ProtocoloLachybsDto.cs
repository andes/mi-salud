using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace LachybsServices.DTOs;

public class ProtocoloLachybsDto
{
    [JsonProperty("protocolo_id")]
    [JsonPropertyName("protocolo_id")]
    public string? ProtocoloId { get; set; }

    [JsonProperty("protocolo")]
    [JsonPropertyName("protocolo")]
    public string? Protocolo { get; set; }

    [JsonProperty("fecha")]
    [JsonPropertyName("fecha")]
    public string? Fecha { get; set; }

    [JsonProperty("solicitante_apellido")]
    [JsonPropertyName("solicitante_apellido")]
    public string? SolicitanteApellido { get; set; }

    [JsonProperty("solicitante_nombre")]
    [JsonPropertyName("solicitante_nombre")]
    public string? SolicitanteNombre { get; set; }

    [JsonProperty("matricula")]
    [JsonPropertyName("matricula")]
    public string? Matricula { get; set; }

    [JsonProperty("profesion")]
    [JsonPropertyName("profesion")]
    public string? Profesion { get; set; }
}
