using LachybsServices.DTOs;

namespace LachybsServices.Interfaces;

public interface ILachybsService
{
    /// <summary>
    /// Obtiene los protocolos de laboratorio de un paciente por DNI.
    /// Devuelve null si LASCHyBS no está configurado, no hay resultados o la consulta falla.
    /// </summary>
    Task<List<ProtocoloLachybsDto>?> ObtenerProtocolosAsync(string usuario, string clave, string documento);

    /// <summary>
    /// Obtiene la URL del informe de un protocolo. Devuelve null si no se pudo obtener.
    /// </summary>
    Task<string?> ObtenerUrlInformeAsync(string usuario, string clave, string idProtocolo);
}
