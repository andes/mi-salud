using AndesServices.Interfaces;
using XroadssAndesServices.Interfaces;

namespace SaludPortal.Application.UseCases.Laboratorios;

public class DescargarInformeLaboratorioUseCase
{
    private readonly IMisLaboratorios _laboratoriosService;
    private readonly IHistoriaSalud _historiaSaludService;
    private readonly IXroadssRaniaService _raniaService;

    public DescargarInformeLaboratorioUseCase(
        IMisLaboratorios laboratoriosService,
        IHistoriaSalud historiaSaludService,
        IXroadssRaniaService raniaService)
    {
        _laboratoriosService = laboratoriosService;
        _historiaSaludService = historiaSaludService;
        _raniaService = raniaService;
    }

    public async Task<byte[]?> EjecutarAsync(string idProtocolo, string tipo, string documento, List<string>? cdaAdjuntos = null)
    {
        switch (tipo)
        {
            case "raña":
                return await _raniaService.DescargarInformeAsync(idProtocolo);

            case "cda":
                if (cdaAdjuntos == null || cdaAdjuntos.Count == 0 || string.IsNullOrEmpty(cdaAdjuntos[0]))
                    return null;

                var adjunto = cdaAdjuntos[0];
                var idDescarga = adjunto.Substring(adjunto.LastIndexOf('/') + 1);
                idDescarga = idDescarga.Substring(0, idDescarga.LastIndexOf('.'));

                var fileToken = await _historiaSaludService.ObtenerFileTokenAsync();
                if (string.IsNullOrEmpty(fileToken))
                    return null;

                return await _laboratoriosService.DescargarLaboratorioCDAPorIdAsync(idDescarga, fileToken);

            default: // rup
                return await _laboratoriosService.DescargarLaboratorioPorIdAsync(idProtocolo, documento);
        }
    }
}
