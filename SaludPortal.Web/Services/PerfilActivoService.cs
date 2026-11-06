using AndesServices.Entities;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using SaludPortal.Application.Models.GrupoFamiliar;

namespace SaludPortal.Web.Services;

/// <summary>
/// Perfil activo local (titular vs familiar) sin re-login. Equivale a storage.familiar en mobile-app.
/// </summary>
public class PerfilActivoService
{
    public const string StorageKey = "misalud.perfilActivo.familiar";

    private readonly ProtectedLocalStorage _localStorage;
    private FamiliarPerfil? _cache;
    private bool _loaded;

    public PerfilActivoService(ProtectedLocalStorage localStorage)
    {
        _localStorage = localStorage;
    }

    public event Action? Changed;

    public bool EsFamiliar => _cache != null && !string.IsNullOrWhiteSpace(_cache.Id);

    public FamiliarPerfil? FamiliarActual => _cache;

    public async Task<FamiliarPerfil?> GetAsync()
    {
        if (_loaded)
            return _cache;

        try
        {
            var result = await _localStorage.GetAsync<FamiliarPerfil>(StorageKey);
            _cache = result.Success ? result.Value : null;
            _loaded = true;
        }
        catch (InvalidOperationException)
        {
            // Prerender / sin JS: no persistir como "cargado" para reintentar en interactivo.
            return null;
        }

        return _cache;
    }

    public async Task SeleccionarFamiliarAsync(FamiliarPerfil familiar)
    {
        ArgumentNullException.ThrowIfNull(familiar);
        if (string.IsNullOrWhiteSpace(familiar.Id))
            throw new ArgumentException("El familiar debe tener Id MPI.", nameof(familiar));

        _cache = familiar;
        _loaded = true;

        try
        {
            await _localStorage.SetAsync(StorageKey, familiar);
        }
        catch (InvalidOperationException)
        {
            // Circuito sin JS: queda en memoria del scope.
        }

        Changed?.Invoke();
    }

    public async Task SalirAsync()
    {
        _cache = null;
        _loaded = true;

        try
        {
            await _localStorage.DeleteAsync(StorageKey);
        }
        catch (InvalidOperationException)
        {
        }

        Changed?.Invoke();
    }

    /// <summary>
    /// Arma un Paciente mínimo desde el snapshot para PATCH/cancelar si GET paciente/{hijo} falla.
    /// </summary>
    public Paciente? ToPacienteSnapshot()
    {
        if (_cache == null || string.IsNullOrWhiteSpace(_cache.Id))
            return null;

        return new Paciente
        {
            id = _cache.Id,
            _id = _cache.Id,
            documento = _cache.Documento,
            nombre = _cache.Nombre,
            apellido = _cache.Apellido,
            sexo = _cache.Sexo,
            fechaNacimiento = _cache.FechaNacimiento,
            edad = _cache.Edad
        };
    }

    public string ResolveIdPacienteActivo(string? titularPacienteId)
    {
        if (_cache != null && !string.IsNullOrWhiteSpace(_cache.Id))
            return _cache.Id;
        return titularPacienteId ?? string.Empty;
    }
}
