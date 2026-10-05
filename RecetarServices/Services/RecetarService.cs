using Microsoft.Extensions.Options;
using RecetarServices.Configuration;
using RecetarServices.DTOs;
using RecetarServices.Interfaces;

namespace RecetarServices.Services;

public class RecetarService : IRecetarService
{
    public const string HttpClientName = "Recetar";

    private const int PageSize = 100;
    private const int MaxPages = 50;

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly RecetarOptions _options;
    private readonly ILogger<RecetarService> _logger;

    public RecetarService(
        IHttpClientFactory httpClientFactory,
        IOptions<RecetarOptions> options,
        ILogger<RecetarService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<List<PrescripcionRecetarDto>> ObtenerPrescripcionesPacienteAsync(string idMPI, CancellationToken ct = default)
    {
        if (!_options.EstaConfigurado || string.IsNullOrWhiteSpace(idMPI))
            return [];

        var client = _httpClientFactory.CreateClient(HttpClientName);
        var prescripciones = new List<PrescripcionRecetarDto>();

        try
        {
            for (var pagina = 0; pagina < MaxPages; pagina++)
            {
                var skip = pagina * PageSize;
                var url = $"prescriptions/by-patient/{Uri.EscapeDataString(idMPI)}?skip={skip}&limit={PageSize}";

                using var res = await client.GetAsync(url, ct);
                if (!res.IsSuccessStatusCode)
                {
                    var body = await res.Content.ReadAsStringAsync(ct);
                    _logger.LogError("Obtener prescripciones Recetar fallo HTTP {Code} {Reason} Body:{Body}",
                        (int)res.StatusCode, res.ReasonPhrase, body);
                    return prescripciones;
                }

                var respuesta = await res.Content.ReadFromJsonAsync<PrescripcionesRecetarResponseDto>(ct);
                var lote = respuesta?.Prescripciones ?? [];
                prescripciones.AddRange(lote);

                if (lote.Count == 0 || prescripciones.Count >= (respuesta?.Total ?? 0))
                    break;
            }
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _logger.LogError(exception, "Error al obtener las prescripciones Recetar para el paciente {IdMPI}", idMPI);
        }

        return prescripciones;
    }
}
