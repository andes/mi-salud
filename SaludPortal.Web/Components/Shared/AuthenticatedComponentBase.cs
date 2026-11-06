using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SaludPortal.Application.Models.GrupoFamiliar;
using SaludPortal.Web.Services;
using System.Security.Claims;

namespace SaludPortal.Web.Components.Shared;

public abstract class AuthenticatedComponentBase : ComponentBase
{
    [CascadingParameter] private Task<AuthenticationState>? AuthStateTask { get; set; }
    [Inject] protected NavigationManager Nav { get; set; } = default!;
    [Inject] protected PerfilActivoService PerfilActivo { get; set; } = default!;

    protected ClaimsPrincipal User { get; private set; } = new(new ClaimsIdentity());

    /// <summary>Claim del titular (login). No cambia al operar como familiar.</summary>
    protected string? PacienteId => User.FindFirst("PacienteId")?.Value;
    protected string? Documento => User.FindFirst("Documento")?.Value;
    protected string? BackendToken => User.FindFirst("TokenBackend")?.Value ?? User.FindFirst("Token")?.Value;
    protected string? Sexo => User.FindFirst("Sexo")?.Value;

    /// <summary>Id MPI del perfil activo (familiar o titular).</summary>
    protected string? IdPacienteActivo { get; private set; }

    protected FamiliarPerfil? FamiliarActivo { get; private set; }

    protected bool EsPerfilFamiliar => FamiliarActivo != null && !string.IsNullOrWhiteSpace(FamiliarActivo.Id);

    /// <summary>Documento del perfil activo (familiar o claim del titular).</summary>
    protected string? DocumentoActivo =>
        !string.IsNullOrWhiteSpace(FamiliarActivo?.Documento) ? FamiliarActivo.Documento : Documento;

    /// <summary>Id para query/body familiar de Andes; null si el perfil es el titular.</summary>
    protected string? FamiliarIdParaAndes =>
        EsPerfilFamiliar ? IdPacienteActivo : null;

    protected string NombrePerfilActivoDisplay
    {
        get
        {
            if (EsPerfilFamiliar && FamiliarActivo is not null)
                return FamiliarActivo.NombreCompleto;

            return $"{User.GetNombre()} {User.GetApellido()}".Trim();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        if (AuthStateTask == null)
            return;

        var state = await AuthStateTask;
        User = state.User;

        if (User.Identity?.IsAuthenticated != true)
        {
            var returnUrl = Nav.ToBaseRelativePath(Nav.Uri);
            Nav.NavigateTo($"/login?returnUrl={Uri.EscapeDataString(string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl)}", true);
            return;
        }

        await RefreshPerfilActivoAsync();
        await OnAuthenticatedInitializedAsync();
    }

    protected async Task RefreshPerfilActivoAsync()
    {
        FamiliarActivo = await PerfilActivo.GetAsync();
        IdPacienteActivo = PerfilActivo.ResolveIdPacienteActivo(PacienteId);
        if (string.IsNullOrWhiteSpace(IdPacienteActivo))
            IdPacienteActivo = PacienteId;
    }

    protected virtual Task OnAuthenticatedInitializedAsync() => Task.CompletedTask;
}