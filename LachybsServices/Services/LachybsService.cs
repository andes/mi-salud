using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using LachybsServices.Configuration;
using LachybsServices.DTOs;
using LachybsServices.Interfaces;
using LachybsServices.Utils;
using Microsoft.Extensions.Options;

namespace LachybsServices.Services;

public class LachybsService : ILachybsService
{
    public const string HttpClientName = "LACHYBS_NOREDIRECT";

    private const int PuertoProtocolos = 6040;
    private const int PuertoInformes = 6041;
    private const string UserAgent = "SaludPortalClient/1.0";

    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly LachybsOptions _options;
    private readonly ILogger<LachybsService> _logger;

    public LachybsService(
        IHttpClientFactory httpClientFactory,
        IOptions<LachybsOptions> options,
        ILogger<LachybsService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<List<ProtocoloLachybsDto>?> ObtenerProtocolosAsync(string usuario, string clave, string documento)
    {
        if (!_options.EstaConfigurado)
            return null;

        var url = $"{BaseUrl}:{PuertoProtocolos}/protocolo?dni={documento}";

        try
        {
            var (res, body) = await GetSiguiendoRedirectAsync(url, usuario, clave);
            using (res)
            {
                if (!res.IsSuccessStatusCode)
                {
                    _logger.LogError("LACHYBS fallo HTTP {Code} {Reason} Body:{Body}", (int)res.StatusCode, res.ReasonPhrase, body);
                    return null;
                }
            }

            var lista = JsonSerializer.Deserialize<List<ProtocoloLachybsDto>>(body, JsonOpts);
            if (lista == null || lista.Count == 0)
                return null;

            foreach (var protocolo in lista)
                protocolo.Fecha = FechaHelper.NormalizarFecha(protocolo.Fecha, _logger);

            return lista;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo LACHYBS dni {Documento}", documento);
            return null;
        }
    }

    public async Task<string?> ObtenerUrlInformeAsync(string usuario, string clave, string idProtocolo)
    {
        if (string.IsNullOrWhiteSpace(idProtocolo))
        {
            _logger.LogWarning("Id de protocolo vacío en ObtenerUrlInformeAsync");
            return null;
        }

        if (!_options.EstaConfigurado)
            return null;

        var url = $"{BaseUrl}:{PuertoInformes}/informe?protocolo_id={idProtocolo}";

        try
        {
            var (res, body) = await GetSiguiendoRedirectAsync(url, usuario, clave);
            using (res)
            {
                if (!res.IsSuccessStatusCode)
                {
                    _logger.LogError("Descargar informe LACHYBS fallo HTTP {Code} {Reason} Body:{Body}", (int)res.StatusCode, res.ReasonPhrase, body);
                    return null;
                }
            }

            var informe = JsonSerializer.Deserialize<InformeLachybsDto>(body, JsonOpts);
            if (informe == null || string.IsNullOrWhiteSpace(informe.InformeUrl))
            {
                _logger.LogWarning("No se obtuvo informe_url para protocolo {Id}", idProtocolo);
                return null;
            }

            return informe.InformeUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo informe_url de protocolo {Id}", idProtocolo);
            return null;
        }
    }

    private string BaseUrl => _options.BaseUrl!.TrimEnd('/');

    /// <summary>
    /// El cliente no sigue redirects automáticamente porque se perdería el header Basic;
    /// se reenvía manualmente la misma autenticación al destino.
    /// </summary>
    private async Task<(HttpResponseMessage Response, string Body)> GetSiguiendoRedirectAsync(string url, string usuario, string clave)
    {
        var client = _httpClientFactory.CreateClient(HttpClientName);

        var res = await client.SendAsync(CrearRequest(new Uri(url), usuario, clave));

        if ((int)res.StatusCode is 301 or 302 or 307 or 308 && res.Headers.Location != null)
        {
            var redirectUri = res.Headers.Location.IsAbsoluteUri
                ? res.Headers.Location
                : new Uri(new Uri(url), res.Headers.Location);

            res.Dispose();
            res = await client.SendAsync(CrearRequest(redirectUri, usuario, clave));
        }

        var body = await res.Content.ReadAsStringAsync();
        return (res, body);
    }

    private static HttpRequestMessage CrearRequest(Uri uri, string usuario, string clave)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.Headers.Authorization = BuildBasicAuthHeader(usuario?.Trim(), clave?.Trim());
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.UserAgent.ParseAdd(UserAgent);
        return request;
    }

    private static AuthenticationHeaderValue BuildBasicAuthHeader(string? usuario, string? clave)
    {
        var raw = $"{usuario ?? string.Empty}:{clave ?? string.Empty}";
        var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(raw));
        return new AuthenticationHeaderValue("Basic", base64);
    }
}
