using AndesServices.Entities;
using AndesServices.Interfaces;
using SaludPortal.Application.Models.GrupoFamiliar;

namespace SaludPortal.Application.UseCases.GrupoFamiliar;

/// <summary>
/// Familiares a cargo para cambio de perfil: relación hijo/a y edad &lt; 13.
/// Fuente: GET modules/mobileApp/relaciones?id={titular}.
/// </summary>
public class ObtenerFamiliaresElegiblesUseCase
{
    public const int EdadMaximaExclusive = 13;
    public const string RelacionHijo = "hijo/a";

    private readonly IPaciente _pacienteService;

    public ObtenerFamiliaresElegiblesUseCase(IPaciente pacienteService)
    {
        _pacienteService = pacienteService;
    }

    public async Task<List<FamiliarPerfil>> EjecutarAsync(string titularId)
    {
        if (string.IsNullOrWhiteSpace(titularId))
            return [];

        var relaciones = await _pacienteService.ObtenerRelacionesAsync(titularId);
        if (relaciones == null || relaciones.Count == 0)
            return [];

        return relaciones
            .Where(EsElegible)
            .Select(Map)
            .Where(f => !string.IsNullOrWhiteSpace(f.Id))
            .ToList();
    }

    public static bool EsElegible(FamiliarRelacion rel)
    {
        var tipo = rel.relacion?.nombre;
        if (string.IsNullOrWhiteSpace(tipo)
            || !tipo.Equals(RelacionHijo, StringComparison.OrdinalIgnoreCase))
            return false;

        if (rel.edad is null || rel.edad >= EdadMaximaExclusive)
            return false;

        var id = rel.id ?? rel.referencia?.id ?? rel.referencia?._id;
        return !string.IsNullOrWhiteSpace(id);
    }

    public static FamiliarPerfil Map(FamiliarRelacion rel)
    {
        var id = rel.id ?? rel.referencia?.id ?? rel.referencia?._id ?? string.Empty;
        return new FamiliarPerfil
        {
            Id = id,
            Documento = rel.documento ?? rel.referencia?.documento,
            Nombre = rel.nombre ?? rel.referencia?.nombre,
            Apellido = rel.apellido ?? rel.referencia?.apellido,
            Sexo = rel.sexo ?? rel.referencia?.sexo,
            Edad = rel.edad ?? rel.referencia?.edad,
            FechaNacimiento = rel.fechaNacimiento ?? rel.referencia?.fechaNacimiento,
            RelacionNombre = rel.relacion?.nombre
        };
    }
}
