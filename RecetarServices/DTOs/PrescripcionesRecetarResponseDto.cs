using System.Text.Json.Serialization;
using Newtonsoft.Json;
using RecetarServices.Converters;

namespace RecetarServices.DTOs;

public class PrescripcionesRecetarResponseDto
{
    [JsonProperty("total")]
    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonProperty("skip")]
    [JsonPropertyName("skip")]
    public int Skip { get; set; }

    [JsonProperty("limit")]
    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonProperty("prescripciones")]
    [JsonPropertyName("prescripciones")]
    public List<PrescripcionRecetarDto> Prescripciones { get; set; } = [];
}

public class PrescripcionRecetarDto
{
    [JsonProperty("idPrescripcion")]
    [JsonPropertyName("idPrescripcion")]
    public string? IdPrescripcion { get; set; }

    [JsonProperty("profesional")]
    [JsonPropertyName("profesional")]
    public ProfesionalRecetarDto? Profesional { get; set; }

    [JsonProperty("medicamento")]
    [JsonPropertyName("medicamento")]
    public MedicamentoRecetarDto? Medicamento { get; set; }

    [JsonProperty("diagnostico")]
    [JsonPropertyName("diagnostico")]
    public string? Diagnostico { get; set; }

    [JsonProperty("fechaCreacion")]
    [JsonPropertyName("fechaCreacion")]
    public DateTime? FechaCreacion { get; set; }

    [JsonProperty("estadoActual")]
    [JsonPropertyName("estadoActual")]
    public string? EstadoActual { get; set; }

    [JsonProperty("estadoDispensa")]
    [JsonPropertyName("estadoDispensa")]
    public string? EstadoDispensa { get; set; }
}

public class ProfesionalRecetarDto
{
    [JsonProperty("nombre")]
    [JsonPropertyName("nombre")]
    public string? Nombre { get; set; }

    [JsonProperty("matricula")]
    [JsonPropertyName("matricula")]
    [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringOrNumberConverter))]
    public string? Matricula { get; set; }

    [JsonProperty("especialidad")]
    [JsonPropertyName("especialidad")]
    public string? Especialidad { get; set; }
}

public class MedicamentoRecetarDto
{
    [JsonProperty("nombre")]
    [JsonPropertyName("nombre")]
    public string? Nombre { get; set; }

    [JsonProperty("concepto")]
    [JsonPropertyName("concepto")]
    public ConceptoSnomedDto? Concepto { get; set; }

    [JsonProperty("codigo")]
    [JsonPropertyName("codigo")]
    public CodigoMedicamentoDto? Codigo { get; set; }

    [JsonProperty("presentacion")]
    [JsonPropertyName("presentacion")]
    public string? Presentacion { get; set; }

    [JsonProperty("cantidad")]
    [JsonPropertyName("cantidad")]
    public decimal? Cantidad { get; set; }

    [JsonProperty("unidadMedida")]
    [JsonPropertyName("unidadMedida")]
    public string? UnidadMedida { get; set; }
}

public class ConceptoSnomedDto
{
    [JsonProperty("conceptId")]
    [JsonPropertyName("conceptId")]
    public string? ConceptId { get; set; }

    [JsonProperty("term")]
    [JsonPropertyName("term")]
    public string? Term { get; set; }

    [JsonProperty("fsn")]
    [JsonPropertyName("fsn")]
    public string? Fsn { get; set; }

    [JsonProperty("semanticTag")]
    [JsonPropertyName("semanticTag")]
    public string? SemanticTag { get; set; }
}

public class CodigoMedicamentoDto
{
    [JsonProperty("fuente")]
    [JsonPropertyName("fuente")]
    public string? Fuente { get; set; }

    [JsonProperty("valor")]
    [JsonPropertyName("valor")]
    [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringOrNumberConverter))]
    public string? Valor { get; set; }
}
