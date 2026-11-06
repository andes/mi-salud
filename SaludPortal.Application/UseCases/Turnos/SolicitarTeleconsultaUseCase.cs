using AndesServices.Interfaces;
using SaludPortal.Application.Models.Turnos;
using SaludPortal.Application.Utils;
using PacienteEntity = AndesServices.Entities.Paciente;

namespace SaludPortal.Application.UseCases.Turnos;

public class SolicitarTeleconsultaUseCase
{
    private readonly IPaciente _pacienteService;
    private readonly ObtenerTurnosUseCase _obtenerTurnosUseCase;
    private readonly IMisTurnos _misTurnosService;

    public SolicitarTeleconsultaUseCase(IPaciente pacienteService, ObtenerTurnosUseCase obtenerTurnosUseCase, IMisTurnos misTurnosService)
    {
        _pacienteService = pacienteService;
        _obtenerTurnosUseCase = obtenerTurnosUseCase;
        _misTurnosService = misTurnosService;
    }

    public async Task<bool> EjecutarAsync(
        string pacienteId,
        TurnoDisponible t,
        string motivoConsulta,
        string telefono,
        PacienteEntity? pacienteOverride = null,
        string? familiarIdParaListar = null)
    {
        var paciente = await _pacienteService.ObtenerPacientePorIdAsync(pacienteId)
            ?? pacienteOverride;
        if (paciente == null)
            return false;

        var turnosPaciente = await _obtenerTurnosUseCase.EjecutarAsync(familiarIdParaListar);
        if (turnosPaciente != null && turnosPaciente.Any())
        {
            var tieneColision = turnosPaciente.Any(turno => turno.EsTeleconsulta);
            if (tieneColision)
                return false;
        }

        var tipoTurno = t.FechaHora.Date == DateTimeHelper.NowArgentina().Date
            ? "delDia"
            : "programado";

        return await _misTurnosService.RegistrarTurnoTeleConsultaAsync(
            t.Id,
            t.BloqueId,
            t.AgendaId,
            paciente,
            t.TipoPrestacionDetalle,
            motivoConsulta,
            telefono,
            tipoTurno);
    }
}