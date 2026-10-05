using AdminLogsServices.DTOs;

namespace AdminLogsServices.Services;

public class AdminApiClient(HttpClient http)
{
    public Task<HttpResponseMessage> SendLogAsync(LogIngestionDto request, CancellationToken cancellationToken = default)
        => http.PostAsJsonAsync("/api/logs", request, cancellationToken);

    public Task<HttpResponseMessage> SendTelemetryAsync(TelemetryDto request, CancellationToken cancellationToken = default)
        => http.PostAsJsonAsync("/api/telemetry", request, cancellationToken);
}
