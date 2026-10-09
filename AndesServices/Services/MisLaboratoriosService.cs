using AndesServices.Entities;
using AndesServices.Helpers;
using AndesServices.Interfaces;
using Newtonsoft.Json.Linq;

namespace AndesServices.Services
{
    public class MisLaboratoriosService : IMisLaboratorios
    {
        private readonly ILogger<MisLaboratoriosService> _logger;
        private readonly HttpClient _andesClient;
        private readonly HttpClient _andesNoJwtClient;

        public MisLaboratoriosService(IHttpClientFactory httpClientFactory, ILogger<MisLaboratoriosService> logger)
        {
            _logger = logger;
            _andesClient = httpClientFactory.CreateClient("Andes");
            _andesNoJwtClient = httpClientFactory.CreateClient("Andes-NoJWT");
        }

        public Task<bool> ActualizarLaboratorioAsync(string idProtocolo, string documento, string apellido, string nombre, string codigoHIV, string fechanacimiento, string sexobiologico, string numero, string fecha, string laboratorio, string medicoSolicitante, string efectorSolicitante, string origen, string tipoMuestra)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EliminarLaboratorioAsync(string idProtocolo)
        {
            throw new NotImplementedException();
        }

        public Task<MisLaboratorios> ObtenerLaboratorioPorIdAsync(string idProtocolo)
        {
            throw new NotImplementedException();
        }

        public async Task<Byte[]> DescargarLaboratorioPorIdAsync(string idProtocolo, string documento)
        {
            byte[] unByte = null;

            try
            {
                var parametrosBody = new StringContent("{\"protocolo\":{\"data\":{\"idProtocolo\":" + idProtocolo + ",\"documento\":" + documento + "}}}", System.Text.Encoding.UTF8, "application/json");
                using (HttpResponseMessage res = await _andesClient.PostAsync("modules/descargas/laboratorio", parametrosBody))
                {
                    if (res.IsSuccessStatusCode)
                    {
                        byte[]? fileResponse = await res.Content.ReadAsByteArrayAsync();
                        if (fileResponse == null)
                        {
                            _logger.LogWarning("El contenido del archivo es null para protocolo {IdProtocolo} y documento {Documento}", idProtocolo, documento);
                            return await Task.FromResult(unByte);
                        }

                        return fileResponse;
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al obtener el archivo del laboratorio para protocolo {IdProtocolo} y documento {Documento}", idProtocolo, documento);
                return await Task.FromResult(unByte);
            }
            return await Task.FromResult(unByte);
        }

        public async Task<Byte[]> DescargarLaboratorioCDAPorIdAsync(string documento, string fileToken)
        {
            byte[] unByte = null;

            try
            {
                // Sin header JWT: Andes prioriza el header sobre ?token= y el módulo CDA rechaza paciente-token.
                using (HttpResponseMessage res = await _andesNoJwtClient.GetAsync($"modules/cda/{documento}?token={Uri.EscapeDataString(fileToken)}"))
                {
                    if (res.IsSuccessStatusCode)
                    {
                        byte[]? fileResponse = await res.Content.ReadAsByteArrayAsync();
                        if (fileResponse == null)
                        {
                            _logger.LogWarning("El contenido del archivo es null para documento {Documento}", documento);
                            return await Task.FromResult(unByte);
                        }

                        var pdf = CdaPdfHelper.NormalizarPdf(fileResponse);
                        if (pdf == null)
                        {
                            _logger.LogWarning("La respuesta del CDA {Documento} no es un PDF ni Base64 válido.", documento);
                        }

                        return pdf;
                    }

                    _logger.LogWarning("El endpoint CDA devolvió {Status} para el CDA {Documento}.", res.StatusCode, documento);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al obtener el archivo del laboratorio para documento {Documento}", documento);
                return await Task.FromResult(unByte);
            }
            return await Task.FromResult(unByte);
        }

        public async Task<List<MisLaboratorios>> ObtenerMisLaboratoriosAsync(string pacienteId, string fechaDde, string fechaHta)
        {
            try
            {

                using (HttpResponseMessage res = await _andesClient.GetAsync($"modules/rup/protocolosLab?pacienteId={pacienteId}&fechaDde={fechaDde}&fechaHta={fechaHta}"))
                {
                    if (res.IsSuccessStatusCode)
                    {
                        string jsonString = await res.Content.ReadAsStringAsync();
                        var root = JToken.Parse(jsonString);

                        // Desanidar si vino como string con JSON dentro
                        if (root.Type == JTokenType.String)
                        {
                            root = JToken.Parse(root.Value<string>());
                        }

                        // Soportar objeto o array con propiedad Data
                        JToken data = root.Type == JTokenType.Array ? root[0]?["Data"] : root["Data"];
                        var listaLaboratorios = data?.ToObject<List<MisLaboratorios>>();

                        if (listaLaboratorios == null)
                        {
                            _logger.LogError("Respuesta sin Data. Body: {Body}", jsonString);
                            return null;
                        }

                        // Normalizar fechas a formato dd/MM/yyyy
                        foreach (var laboratorio in listaLaboratorios)
                        {
                            if (!string.IsNullOrEmpty(laboratorio.fecha))
                            {
                                laboratorio.fecha = NormalizarFecha(laboratorio.fecha);
                            }
                        }

                        return listaLaboratorios;
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al obtener los laboratorios");
                return null;
            }
            return null;
        }

        /// <summary>
        /// Normaliza una fecha a formato dd/MM/yyyy desde cualquier formato reconocible
        /// </summary>
        private string NormalizarFecha(string fecha)
        {
            if (string.IsNullOrWhiteSpace(fecha))
                return fecha;

            try
            {
                // Intentar parsear en formato dd/MM/yyyy (ya está en el formato correcto)
                if (DateTime.TryParseExact(fecha, "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out DateTime resultado))
                {
                    return resultado.ToString("dd/MM/yyyy");
                }

                // Intentar parsear en formato ISO (yyyyMMdd)
                if (DateTime.TryParseExact(fecha, "yyyyMMdd",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out resultado))
                {
                    return resultado.ToString("dd/MM/yyyy");
                }

                // Intentar parsear en formato yyyy-MM-dd
                if (DateTime.TryParseExact(fecha, "yyyy-MM-dd",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out resultado))
                {
                    return resultado.ToString("dd/MM/yyyy");
                }

                // Intentar parsear con el parseador general
                if (DateTime.TryParse(fecha, out resultado))
                {
                    return resultado.ToString("dd/MM/yyyy");
                }

                // Si no se pudo parsear, devolver la fecha original
                _logger.LogWarning("No se pudo parsear la fecha: {Fecha}", fecha);
                return fecha;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al normalizar la fecha: {Fecha}", fecha);
                return fecha;
            }
        }

        public async Task<List<MisLaboratoriosCDA>> ObtenerMisLaboratoriosCDAAsync(string pacienteId, string fechaDde, string fechaHta)
        {
            try
            {
                using (HttpResponseMessage res = await _andesClient.GetAsync($"modules/cda/paciente/{pacienteId}"))
                {
                    if (res.IsSuccessStatusCode)
                    {
                        List<MisLaboratoriosCDA?> misLaboratorios = new List<MisLaboratoriosCDA?>();

                        misLaboratorios = await res.Content.ReadFromJsonAsync<List<MisLaboratoriosCDA>>();

                        if (misLaboratorios == null)
                        {
                            return null;
                        }

                        return misLaboratorios;
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al obtener los laboratorios CDA para paciente {PacienteId}", pacienteId);
                return null;
            }
            return null;
        }

        public Task<List<MisLaboratorios>> ObtenerMisLaboratoriosAsync(string documento)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RegistrarLaboratorioAsync(string idProtocolo, string documento, string apellido, string nombre, string codigoHIV, string fechanacimiento, string sexobiologico, string numero, string fecha, string laboratorio, string medicoSolicitante, string efectorSolicitante, string origen, string tipoMuestra)
        {
            throw new NotImplementedException();
        }
    }
}
