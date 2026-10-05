using System.Globalization;

namespace LachybsServices.Utils;

internal static class FechaHelper
{
    private static readonly string[] FormatosExactos = ["dd/MM/yyyy", "yyyyMMdd", "yyyy-MM-dd"];

    /// <summary>
    /// Normaliza una fecha a formato dd/MM/yyyy desde cualquier formato reconocible.
    /// Si no se puede parsear, devuelve la fecha original.
    /// </summary>
    public static string? NormalizarFecha(string? fecha, ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(fecha))
            return fecha;

        if (DateTime.TryParseExact(fecha, FormatosExactos, CultureInfo.InvariantCulture, DateTimeStyles.None, out var resultado)
            || DateTime.TryParse(fecha, out resultado))
        {
            return resultado.ToString("dd/MM/yyyy");
        }

        logger.LogWarning("No se pudo parsear la fecha: {Fecha}", fecha);
        return fecha;
    }
}
