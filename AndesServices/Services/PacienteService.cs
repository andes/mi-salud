using System.Net.Http.Headers;
using AndesServices.DTOs;
using AndesServices.Entities;
using AndesServices.Interfaces;
using Newtonsoft.Json;

namespace AndesServices.Services
{
    public class PacienteService : IPaciente
    {
        private readonly ILogger<PacienteService> _logger;
        private readonly HttpClient _andesClient;

        public PacienteService(IHttpClientFactory httpClientFactory, ILogger<PacienteService> logger)
        {
            _logger = logger;
            _andesClient = httpClientFactory.CreateClient("Andes");;
        }

        /// <summary>
        /// GET modules/mobileApp/paciente/:id — respuesta MPI (PacienteSchema + virtuals/audit).
        /// Excluye claveBlocking, entidadesValidadoras, carpetaEfectores, createdBy.
        /// </summary>
        public async Task<Paciente> ObtenerPacientePorIdAsync(string idPaciente, string? token = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    _andesClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("JWT", token);
                }

                using (HttpResponseMessage res = await _andesClient.GetAsync($"modules/mobileApp/paciente/{idPaciente}"))
                {
                    res.EnsureSuccessStatusCode();

                    Paciente unPaciente = await res.Content.ReadFromJsonAsync<Paciente>();

                    if (unPaciente == null)
                    {
                        return null;
                    }

                    return unPaciente;
                }
            }

            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al obtener los datos del paciente.");
                return null;
            }
        }

        /// <summary>
        /// GET modules/mobileApp/relaciones?id={titularId} — resuelve ObjectIds de relaciones MPI
        /// y devuelve id/edad/documento del vinculado. No usar GET paciente/:id/relaciones
        /// (en Andes stock compara mal referencia.documento → unauthorized).
        /// </summary>
        public async Task<List<FamiliarRelacion>?> ObtenerRelacionesAsync(string titularId)
        {
            if (string.IsNullOrWhiteSpace(titularId))
                return null;

            try
            {
                var url = $"modules/mobileApp/relaciones?id={Uri.EscapeDataString(titularId)}";
                using var res = await _andesClient.GetAsync(url);
                if (!res.IsSuccessStatusCode)
                {
                    var body = await res.Content.ReadAsStringAsync();
                    _logger.LogError(
                        "Obtener relaciones fallo HTTP {Code} {Reason} Body:{Body}",
                        (int)res.StatusCode,
                        res.ReasonPhrase,
                        body);
                    return null;
                }

                return await res.Content.ReadFromJsonAsync<List<FamiliarRelacion>>();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al obtener relaciones del paciente {TitularId}", titularId);
                return null;
            }
        }

        public async Task<userLocation> ObtenerGeoreferenciaPaciente(string direccion)
        {
            try
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"modules/georeferencia/georeferenciar?direccion={Uri.EscapeDataString(direccion)}", UriKind.Relative),
                };

                using (HttpResponseMessage res = await _andesClient.SendAsync(request))
                {
                    res.EnsureSuccessStatusCode();

                    userLocation unaGeoreferencia = await res.Content.ReadFromJsonAsync<userLocation>();

                    if (unaGeoreferencia == null)
                    {
                        return null;
                    }

                    return unaGeoreferencia;
                }
            }

            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al obtener los datos del paciente.");
                return null;
            }
        }

        /// <summary>
        /// PUT modules/mobileApp/paciente/:id — body ActualizarPacienteDto.
        /// Autoriza titular (en token.pacientes) o familiar vinculado por ObjectId (Andes).
        /// Respuesta Andes: { status: "OK" }; se reintenta GET o se devuelve el original fusionado.
        /// </summary>
        public async Task<Paciente> ModificarDatos(string idPaciente, ActualizarPacienteDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            // Validar género si está presente
            if (!string.IsNullOrEmpty(dto.Genero))
            {
                var generosValidos = new List<string> { "mujer", "mujer trans", "varon", "varon trans", "no binario", "travesti", "masculinidad trans", "femenino", "masculino", "otro" };
                if (!generosValidos.Contains(dto.Genero))
                {
                    throw new ArgumentException("Genero no permitido.");
                }
            }
            
            try
            {
                // Obtener el paciente original del servidor para comparar cambios (puede ser null si es familiar)
                Paciente? pacienteOriginal = await ObtenerPacientePorIdAsync(idPaciente);

                // Obtener fecha/hora actual para ultimaActualizacion
                DateTime fechaActual = DateTime.UtcNow;

                // Actualizar ultimaActualizacion en contactos
                if (dto.Contacto != null)
                {
                    foreach (var c in dto.Contacto)
                    {
                        var contactoOriginal = pacienteOriginal?.contacto?.FirstOrDefault(co =>
                            co.id == c.Id ||
                            co._id == c.IdInterno ||
                            co.id == c.IdInterno ||
                            co._id == c.Id);

                        if (contactoOriginal == null ||
                            contactoOriginal.valor != c.Valor ||
                            contactoOriginal.tipo != c.Tipo)
                        {
                            c.UltimaActualizacion = fechaActual;
                        }
                    }
                }

                // Actualizar ultimaActualizacion en direcciones y obtener georeferencia
                if (dto.Direccion != null)
                {
                    foreach (var d in dto.Direccion)
                    {
                        var direccionOriginal = pacienteOriginal?.direccion?.FirstOrDefault(di =>
                            di.id == d.Id ||
                            di._id == d.IdInterno ||
                            di.id == d.IdInterno ||
                            di._id == d.Id);

                        if (direccionOriginal == null ||
                            direccionOriginal.valor != d.Valor ||
                            direccionOriginal.codigoPostal != d.CodigoPostal ||
                            direccionOriginal.ranking != d.Ranking ||
                            direccionOriginal.ubicacion?.pais?.nombre != d.Ubicacion?.Pais?.Nombre ||
                            direccionOriginal.ubicacion?.provincia?.nombre != d.Ubicacion?.Provincia?.Nombre ||
                            direccionOriginal.ubicacion?.localidad?.nombre != d.Ubicacion?.Localidad?.Nombre)
                        {
                            d.UltimaActualizacion = fechaActual;
                        }
                    }
                }

                // PUT autoriza dueño o familiar vinculado (PATCH plural solo dueño en token).
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri($"modules/mobileApp/paciente/{idPaciente}", UriKind.Relative),
                    Content = new StringContent(JsonConvert.SerializeObject(dto), System.Text.Encoding.UTF8, "application/json")
                };

                using (HttpResponseMessage res = await _andesClient.SendAsync(request))
                {
                    res.EnsureSuccessStatusCode();

                    var refetch = await ObtenerPacientePorIdAsync(idPaciente);
                    if (refetch != null)
                        return refetch;

                    if (pacienteOriginal != null)
                        return pacienteOriginal;

                    return new Paciente
                    {
                        id = idPaciente,
                        _id = idPaciente,
                        alias = dto.Alias,
                        genero = dto.Genero
                    };
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al modificar los datos del paciente.");
                throw;
            }
        }

        public Direccion? ObtenerDireccionPrioritaria(Paciente? paciente)
        {
            List<Direccion>? direccion = paciente?.direccion;

            if (direccion?.Count == 1)
            {
                return direccion[0];
            }
            if (direccion?.Count > 1)
            {
                return direccion[1];
            }
            return null;
        }

        public ActualizarPacienteDireccionDto? ObtenerDireccionPrioritaria(ActualizarPacienteDto pacienteActualizado)
        {
            if (pacienteActualizado.Direccion.Count == 1)
            {
                return pacienteActualizado.Direccion[0];
            }

            if (pacienteActualizado.Direccion.Count > 1)
            {
                return pacienteActualizado.Direccion[1];
            }

            return null;
        }
    }
}
