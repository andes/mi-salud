using AdminLogsServices.Configuration;
using AdminLogsServices.Logging;
using AdminLogsServices.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AdminLogsServices.DependencyInjection;

public static class AdminLogsServiceCollectionExtensions
{
    /// <summary>
    /// Registra el envío de logs y telemetría al panel Admin.
    /// Si <c>AdminLogs:ApiKey</c> está vacío no registra nada y el pipeline queda desactivado.
    /// Para enriquecer los logs, el host puede registrar su propio <see cref="IAdminLogsClientContextAccessor"/>.
    /// </summary>
    public static IServiceCollection AddAdminLogsServices(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(AdminIngestionOptions.SectionName);
        var options = section.Get<AdminIngestionOptions>() ?? new AdminIngestionOptions();
        if (!options.EstaConfigurado)
            return services;

        if (string.IsNullOrWhiteSpace(options.BaseUrl))
            throw new InvalidOperationException("AdminLogs:BaseUrl is required when AdminLogs:ApiKey is set.");

        services
            .AddOptions<AdminIngestionOptions>()
            .Bind(section);

        services.AddHttpClient<AdminApiClient>(client =>
        {
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(Math.Max(1, options.HttpTimeoutSeconds));
            client.DefaultRequestHeaders.Add("X-Api-Key", options.ApiKey);
        });

        services.TryAddSingleton<IAdminLogsClientContextAccessor, NullAdminLogsClientContextAccessor>();
        services.AddSingleton<AdminIngestionQueue>();
        services.AddHostedService<AdminIngestionWorker>();
        services.AddSingleton<ILoggerProvider, ApiLoggerProvider>();

        return services;
    }
}
