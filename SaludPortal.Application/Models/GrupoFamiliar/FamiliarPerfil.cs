namespace SaludPortal.Application.Models.GrupoFamiliar;

/// <summary>
/// Snapshot del familiar activo (equivalente a storage.familiar en mobile-app).
/// </summary>
public class FamiliarPerfil
{
    public string Id { get; set; } = string.Empty;
    public string? Documento { get; set; }
    public string? Nombre { get; set; }
    public string? Apellido { get; set; }
    public string? Sexo { get; set; }
    public int? Edad { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public string? RelacionNombre { get; set; }

    public string NombreCompleto =>
        $"{Nombre} {Apellido}".Trim();
}
