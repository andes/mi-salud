using AndesServices.Interfaces;
using PacienteEntity = AndesServices.Entities.Paciente;

namespace SaludPortal.Application.UseCases.Turnos;

public class CancelarTurnoUseCase
{
    private readonly IPaciente _pacienteService;
    private readonly IMisTurnos _misTurnosService;

    public CancelarTurnoUseCase(IPaciente pacienteService, IMisTurnos misTurnosService)
    {
        _pacienteService = pacienteService;
        _misTurnosService = misTurnosService;
    }

    /// <param name="pacienteOverride">Snapshot del familiar si GET paciente/{id} no está autorizado.</param>
    public async Task<bool> EjecutarAsync(
        string pacienteId,
        string idTurno,
        string idBloque,
        string idAgenda,
        PacienteEntity? pacienteOverride = null)
    {
        var paciente = await _pacienteService.ObtenerPacientePorIdAsync(pacienteId)
            ?? pacienteOverride;
        if (paciente == null)
            return false;

        return await _misTurnosService.CancelarTurnoAsync(idTurno, idBloque, idAgenda, paciente);
    }
}
