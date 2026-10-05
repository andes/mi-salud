namespace AdminLogsServices.DTOs;

public sealed record LogIngestionDto(
        string Level,
        string Category,
        string? Message,
        string? Exception,
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

public sealed record TelemetryDto(
        string EventType,
        string Route,
        string? Target,
        string? PatientId,
        string? SessionId,
        string? Source,
        string? MetadataJson,
        string? ClientIp,
        double? Latitude,
        double? Longitude,
        string? UserAgent,
        string? Browser,
        string? OsName,
        string? OsVersion,
        string? DeviceType);
