using System.Text.Json;
using XroadssAndesServices.DTOs.Rania;
using XroadssAndesServices.Interfaces;
using XroadssAndesServices.Utils;

namespace XroadssAndesServices.Services;

public class XroadssRaniaService : IXroadssRaniaService
{
    private const string RutaRania = "r1/OPTIC/COM/COM00007/GP-LABRANIA";

    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<XroadssRaniaService> _logger;

    public XroadssRaniaService(IHttpClientFactory httpClientFactory, ILogger<XroadssRaniaService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<List<ProtocoloRaniaResponseDto>> ObtenerProtocolosAsync(string dni)
    {
        try
        {
            var client = _httpClientFactory.CreateClient(XroadssAndesHttpClient.Name);
            using var res = await client.GetAsync($"{RutaRania}/protocolo?dni={dni}");
            if (!res.IsSuccessStatusCode)
                return [];

            var lista = await res.Content.ReadFromJsonAsync<List<ProtocoloRaniaResponseDto>>(JsonOpts);
            if (lista == null)
                return [];

            foreach (var protocolo in lista)
                protocolo.Fecha = FechaHelper.NormalizarFecha(protocolo.Fecha, _logger);

            return lista;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error al obtener los laboratorios Rania para el DNI {Dni}", dni);
            return [];
        }
    }

    public async Task<InformeRaniaResponseDto?> ObtenerInformeAsync(string protocoloId)
    {
        if (string.IsNullOrWhiteSpace(protocoloId))
        {
            _logger.LogWarning("Protocolo ID no proporcionado en ObtenerInformeAsync");
            return null;
        }

        try
        {
            var client = _httpClientFactory.CreateClient(XroadssAndesHttpClient.Name);
            using var res = await client.GetAsync($"{RutaRania}/informe?protocolo_id={protocoloId}");
            if (res.IsSuccessStatusCode)
                return await res.Content.ReadFromJsonAsync<InformeRaniaResponseDto>(JsonOpts);

            var body = await res.Content.ReadAsStringAsync();
            _logger.LogWarning("Error al obtener informe Rania. Status: {StatusCode} Body: {Body} para protocolo {ProtocoloId}",
                res.StatusCode, body, protocoloId);
            return null;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error al obtener el informe Rania para protocolo {ProtocoloId}", protocoloId);
            return null;
        }
    }

    public async Task<byte[]?> DescargarInformeAsync(string protocoloId)
    {
        if (string.IsNullOrWhiteSpace(protocoloId))
        {
            _logger.LogWarning("Protocolo ID no proporcionado en DescargarInformeAsync");
            return null;
        }

        try
        {
            var informe = await ObtenerInformeAsync(protocoloId);
            if (informe == null || string.IsNullOrWhiteSpace(informe.InformeUrl))
            {
                _logger.LogWarning("No se pudo obtener la URL del informe para protocolo {ProtocoloId}", protocoloId);
                return null;
            }

            // informe_url apunta a un host externo al gateway: no lleva certificado ni header X-Road.
            var client = _httpClientFactory.CreateClient();
            using var res = await client.GetAsync(informe.InformeUrl);
            if (res.IsSuccessStatusCode)
                return await res.Content.ReadAsByteArrayAsync();

            _logger.LogWarning("Error al descargar informe Rania. Status: {StatusCode} para protocolo {ProtocoloId}",
                res.StatusCode, protocoloId);
            return null;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error al descargar el informe Rania para protocolo {ProtocoloId}", protocoloId);
            return null;
        }
    }
}
