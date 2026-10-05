using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RecetarServices.Configuration;
using RecetarServices.Interfaces;
using RecetarServices.Services;

namespace RecetarServices.DependencyInjection;

public static class RecetarServiceCollectionExtensions
{
    public static IServiceCollection AddRecetarServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<RecetarOptions>()
            .Bind(configuration.GetSection(RecetarOptions.SectionName));

        services.AddHttpClient(RecetarService.HttpClientName, (sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<RecetarOptions>>().Value;
            if (!options.EstaConfigurado)
                return;

            client.BaseAddress = new Uri(options.BaseUrl!.TrimEnd('/') + "/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.AccessToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });

        services.AddScoped<IRecetarService, RecetarService>();

        return services;
    }
}
