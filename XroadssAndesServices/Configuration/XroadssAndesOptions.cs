namespace XroadssAndesServices.Configuration;

public class XroadssAndesOptions
{
    public const string SectionName = "ApiXroadssAndes";
    public const string XRoadClientHeader = "X-ROAD-CLIENT";
    public const string XRoadClient = "OPTIC/GOB/GOB00008/GP-SALUD";

    public string? BaseUrl { get; set; }
    public string? CertPath { get; set; }
    public string? CertPassword { get; set; }

    public bool EstaConfigurado => !string.IsNullOrWhiteSpace(BaseUrl);
}
