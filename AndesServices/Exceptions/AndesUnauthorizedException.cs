namespace AndesServices.Exceptions;

/// <summary>
/// Andes respondió 401/403 (p. ej. agendasDisponibles cuando idPaciente no está en token.pacientes).
/// </summary>
public class AndesUnauthorizedException : Exception
{
    public int StatusCode { get; }

    public AndesUnauthorizedException(string message, int statusCode = 401)
        : base(message)
    {
        StatusCode = statusCode;
    }
}
