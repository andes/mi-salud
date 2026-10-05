using AndesServices.Interfaces;
using RecetarServices.Interfaces;
using SaludPortal.Application.Mappers;
using SaludPortal.Application.Models.Recetas;

namespace SaludPortal.Application.UseCases.Recetas;

public class ObtenerRecetasUseCase
{
    private readonly IMisRecetas _recetasService;
    private readonly IRecetarService _recetarService;

    public ObtenerRecetasUseCase(IMisRecetas recetasService, IRecetarService recetarService)
    {
        _recetasService = recetasService;
        _recetarService = recetarService;
    }

    public async Task<List<Receta>> EjecutarAsync(string pacienteId)
    {
        var recetasAndesTask = _recetasService.ObtenerRecetasPacienteAsync(pacienteId);
        var prescripcionesRecetarTask = _recetarService.ObtenerPrescripcionesPacienteAsync(pacienteId);

        await Task.WhenAll(recetasAndesTask, prescripcionesRecetarTask);

        var recetasAndes = await recetasAndesTask ?? [];
        var prescripcionesRecetar = await prescripcionesRecetarTask;

        return [.. recetasAndes.Select(r => r.MapToRecetaModel())
            .Concat(prescripcionesRecetar.Select(p => p.MapToRecetaModel()))
            .OrderByDescending(r => r.FechaRegistro)];
    }
}
