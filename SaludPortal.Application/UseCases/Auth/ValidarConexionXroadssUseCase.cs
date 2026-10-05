using XroadssAndesServices.Interfaces;

namespace SaludPortal.Application.UseCases.Auth;

public class ValidarConexionXroadssUseCase
{
    private readonly IXroadssRenaperService _renaperService;

    public ValidarConexionXroadssUseCase(IXroadssRenaperService renaperService)
    {
        _renaperService = renaperService;
    }

    public async Task<bool> EjecutarAsync()
    {
        return await _renaperService.ValidarConexionAsync();
    }
}
