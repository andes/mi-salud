using AdminLogsServices.Services;

namespace AdminLogsServices.Logging;

/// <summary>
/// Logger provider that enqueues log entries for shipping to SaludPortal.Admin.
/// Client context (geo, device) is read from <see cref="IAdminLogsClientContextAccessor"/>,
/// which the host populates.
/// </summary>
public sealed class ApiLoggerProvider : ILoggerProvider
{
    private readonly AdminIngestionQueue _queue;
    private readonly IAdminLogsClientContextAccessor _clientContextAccessor;

    public ApiLoggerProvider(
        AdminIngestionQueue queue,
        IAdminLogsClientContextAccessor clientContextAccessor)
    {
        _queue = queue;
        _clientContextAccessor = clientContextAccessor;
    }

    public ILogger CreateLogger(string categoryName)
        => new ApiLogger(categoryName, _queue, _clientContextAccessor);

    public void Dispose() { }
}
