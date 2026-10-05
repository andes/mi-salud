using AndesServices.Configuration;
using AndesServices.Entities;
using AndesServices.Handlers;
using AndesServices.Interfaces;
using AndesServices.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AndesServices.DependencyInjection;

public static class AndesServiceCollectionExtensions
{
    private const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/125.0.0.0 Safari/537.36";

    public static IServiceCollection AddAndesServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<AndesOptions>()
            .Bind(configuration.GetSection(AndesOptions.SectionName));

        services.AddHttpContextAccessor();
        services.AddTransient<AndesTokenHandler>();

        services.AddHttpClient("Andes", (sp, client) =>
            {
                ConfigurarBaseAddress(sp, client);
                client.DefaultRequestHeaders.Add("User-Agent", UserAgent);
            })
            .AddHttpMessageHandler<AndesTokenHandler>();

        services.AddHttpClient("Andes-NoJWT", ConfigurarBaseAddress);

        services.AddScoped<ILoginService<User>, LoginService>();
        services.AddScoped<IMisLaboratorios, MisLaboratoriosService>();
        services.AddScoped<IPaciente, PacienteService>();
        services.AddScoped<IFarmaciasTurno, FarmaciasTurnoService>();
        services.AddScoped<IHistoriaSalud, HistoriaSaludService>();
        services.AddScoped<IVacunacion, VacunacionService>();
        services.AddScoped<IMisRecetas, MisRecetasService>();
        services.AddScoped<IOrganizacion, OrganizacionService>();
        services.AddScoped<IMisTurnos, MisTurnosService>();
        services.AddScoped<ITerritorio, TerritorioService>();
        services.AddScoped<ICentrosSalud, CentrosSaludService>();
        services.AddScoped<IConsentimientoProgramaCuidadoIntegral, ConsentimientoProgramaCuidadoIntegralService>();

        return services;
    }

    private static void ConfigurarBaseAddress(IServiceProvider sp, HttpClient client)
    {
        var options = sp.GetRequiredService<IOptions<AndesOptions>>().Value;
        if (!options.EstaConfigurado)
            return;

        client.BaseAddress = new Uri(options.BaseUrl!.TrimEnd('/') + "/");
    }
}
