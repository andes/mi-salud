namespace AdminLogsServices.Logging;

public sealed record AdminLogsClientContext(
    string? ClientIp,
    double? Latitude,
    double? Longitude,
    string? UserAgent,
    string? Browser,
    string? OsName,
    string? OsVersion,
    string? DeviceType,
    string? PatientId,
    string? SessionId);

/// <summary>
/// Provee el contexto del cliente (IP, dispositivo, paciente) que se adjunta a cada log.
/// Se lee de forma sincrónica desde el logger, por lo que debe ser un valor ya cacheado.
/// </summary>
public interface IAdminLogsClientContextAccessor
{
    AdminLogsClientContext? Current { get; }
}

internal sealed class NullAdminLogsClientContextAccessor : IAdminLogsClientContextAccessor
{
    public AdminLogsClientContext? Current => null;
}
