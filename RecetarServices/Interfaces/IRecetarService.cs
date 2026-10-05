using RecetarServices.DTOs;

namespace RecetarServices.Interfaces;

public interface IRecetarService
{
    /// <summary>
    /// Obtiene todas las prescripciones emitidas en Recetar para un paciente.
    /// Devuelve una lista vacía si Recetar no está configurado o la consulta falla.
    /// </summary>
    /// <param name="idMPI">Identificador del paciente en el MPI.</param>
    Task<List<PrescripcionRecetarDto>> ObtenerPrescripcionesPacienteAsync(string idMPI, CancellationToken ct = default);
}
