using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using XroadssAndesServices.Configuration;
using XroadssAndesServices.Handlers;
using XroadssAndesServices.Interfaces;
using XroadssAndesServices.Services;

namespace XroadssAndesServices.DependencyInjection;

public static class XroadssAndesServiceCollectionExtensions
{
    public static IServiceCollection AddXroadssAndesServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<XroadssAndesOptions>()
            .Bind(configuration.GetSection(XroadssAndesOptions.SectionName));

        services.AddTransient<XRoadCertificateHandler>();

        services.AddHttpClient(XroadssAndesHttpClient.Name, (sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<XroadssAndesOptions>>().Value;
                if (options.EstaConfigurado)
                    client.BaseAddress = new Uri(options.BaseUrl!.TrimEnd('/') + "/");

                client.DefaultRequestHeaders.Add(XroadssAndesOptions.XRoadClientHeader, XroadssAndesOptions.XRoadClient);
            })
            .ConfigurePrimaryHttpMessageHandler<XRoadCertificateHandler>();

        services.AddScoped<IXroadssRenaperService, XroadssRenaperService>();
        services.AddScoped<IXroadssRaniaService, XroadssRaniaService>();

        return services;
    }
}
