using XroadssAndesServices.DTOs.Renaper;

namespace XroadssAndesServices.Interfaces;

public interface IXroadssRenaperService
{
    /// <summary>
    /// Verifica que el gateway X-Road responda. Devuelve false ante cualquier error.
    /// </summary>
    Task<bool> ValidarConexionAsync();

    /// <summary>
    /// Consulta los datos del DNI en RENAPER. Propaga las excepciones HTTP/serialización.
    /// </summary>
    Task<VerificarUsuarioXroadssResponseDto?> VerificarUsuarioAsync(string dni, char sexo);
}
