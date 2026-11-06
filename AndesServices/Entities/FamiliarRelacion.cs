using System.Text.Json.Serialization;
using AndesServices.Converters;

namespace AndesServices.Entities;

/// <summary>
/// Item de GET modules/mobileApp/relaciones?id={titular}.
/// Andes parte de la relación MPI (toObject) y agrega id/edad/documento del paciente vinculado.
/// </summary>
public class FamiliarRelacion
{
    public string? _id { get; set; }

    /// <summary>Id MPI del familiar vinculado (Andes sobrescribe con pacienteRel.id).</summary>
    public string? id { get; set; }

    public int? edad { get; set; }
    public string? documento { get; set; }
    public string? nombre { get; set; }
    public string? apellido { get; set; }
    public string? sexo { get; set; }
    public string? genero { get; set; }
    public DateTime? fechaNacimiento { get; set; }
    public DateTime? fechaFallecimiento { get; set; }
    public RelacionTipo? relacion { get; set; }

    [JsonConverter(typeof(RelacionReferenciaConverter))]
    public RelacionReferencia? referencia { get; set; }
}
