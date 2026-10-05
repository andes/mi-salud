using System.Net;
using XroadssAndesServices.DTOs.Renaper;
using XroadssAndesServices.Interfaces;

namespace XroadssAndesServices.Services;

public class XroadssRenaperService : IXroadssRenaperService
{
    private const string RutaRenaper = "r1/OPTIC/GOB/GOB00001/GP-RENAPER/WS_RENAPER_DOCUMENTO";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<XroadssRenaperService> _logger;

    public XroadssRenaperService(IHttpClientFactory httpClientFactory, ILogger<XroadssRenaperService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<bool> ValidarConexionAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient(XroadssAndesHttpClient.Name);
            using var response = await client.GetAsync($"{RutaRenaper}/00000000/M");
            return response.StatusCode == HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error al validar conexión con xroadss.andes.gob.ar");
            return false;
        }
    }

    public async Task<VerificarUsuarioXroadssResponseDto?> VerificarUsuarioAsync(string dni, char sexo)
    {
        var client = _httpClientFactory.CreateClient(XroadssAndesHttpClient.Name);
        return await client.GetFromJsonAsync<VerificarUsuarioXroadssResponseDto>(
            $"{RutaRenaper}/{dni}/{char.ToUpperInvariant(sexo)}");
    }
}
