using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Options;
using XroadssAndesServices.Configuration;

namespace XroadssAndesServices.Handlers;

public class XRoadCertificateHandler : HttpClientHandler
{
    public XRoadCertificateHandler(IOptions<XroadssAndesOptions> options, ILogger<XRoadCertificateHandler> logger)
    {
        var certPath = options.Value.CertPath;
        var certPassword = options.Value.CertPassword;

        if (string.IsNullOrWhiteSpace(certPath) || !File.Exists(certPath))
            return;

        try
        {
            var cert = X509CertificateLoader.LoadPkcs12FromFile(certPath, certPassword);
            ClientCertificates.Add(cert);
            logger.LogInformation("Certificado X-Road cargado correctamente desde {CertPath}", certPath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al cargar el certificado X-Road desde {CertPath}", certPath);
        }
    }
}
