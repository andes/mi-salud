using AdminLogsServices.Logging;

namespace SaludPortal.Web.Services;

public class BrowserContextCache : IAdminLogsClientContextAccessor
{
    private volatile ClientContextPayload? _cached;

    public void Update(ClientContextPayload payload)
        => _cached = payload;

    public ClientContextPayload? Current => _cached;

    AdminLogsClientContext? IAdminLogsClientContextAccessor.Current => _cached is { } c
        ? new AdminLogsClientContext(
            c.ClientIp,
            c.Latitude,
            c.Longitude,
            c.UserAgent,
            c.Browser,
            c.OsName,
            c.OsVersion,
            c.DeviceType,
            c.PatientId,
            c.SessionId)
        : null;
}
