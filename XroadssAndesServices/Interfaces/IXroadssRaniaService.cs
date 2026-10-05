using XroadssAndesServices.DTOs.Rania;

namespace XroadssAndesServices.Interfaces;

public interface IXroadssRaniaService
{
    /// <summary>
    /// Obtiene los protocolos de laboratorio RANIA de un paciente, con fechas en dd/MM/yyyy.
    /// Devuelve una lista vacía si la consulta falla.
    /// </summary>
    Task<List<ProtocoloRaniaResponseDto>> ObtenerProtocolosAsync(string dni);

    Task<InformeRaniaResponseDto?> ObtenerInformeAsync(string protocoloId);

    /// <summary>
    /// Descarga el PDF del informe de un protocolo. Devuelve null si no se pudo obtener.
    /// </summary>
    Task<byte[]?> DescargarInformeAsync(string protocoloId);
}
