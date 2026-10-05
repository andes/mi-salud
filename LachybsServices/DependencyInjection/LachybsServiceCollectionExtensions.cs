using LachybsServices.Configuration;
using LachybsServices.Interfaces;
using LachybsServices.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LachybsServices.DependencyInjection;

public static class LachybsServiceCollectionExtensions
{
    public static IServiceCollection AddLachybsServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<LachybsOptions>()
            .Bind(configuration.GetSection(LachybsOptions.SectionName));

        services.AddHttpClient(LachybsService.HttpClientName)
            .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
            {
                AllowAutoRedirect = false
            });

        services.AddScoped<ILachybsService, LachybsService>();

        return services;
    }
}
