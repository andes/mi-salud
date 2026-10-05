using RecetarServices.DTOs;
using SaludPortal.Application.Models.Recetas;
using SaludPortal.Application.Utils;

namespace SaludPortal.Application.Mappers;

public static class RecetaMapper
{
    public static Receta MapToRecetaModel(this AndesServices.Entities.MisReceta r)
    {
        return new Receta
        {
            Id = r.id ?? r._id,
            Origen = OrigenReceta.Andes,
            FechaRegistro = DateTimeHelper.ToArgentinaTime(r.fechaRegistro),
            FechaPrestacion = DateTimeHelper.ToArgentinaTime(r.fechaPrestacion),
            Profesional = r.profesional == null ? null : new ProfesionalReceta
            {
                Nombre = r.profesional.nombre,
                Apellido = r.profesional.apellido,
                Matricula = r.profesional.matricula,
                Especialidad = r.profesional.especialidad
            },
            Medicamento = r.medicamento == null ? null : new MedicamentoReceta
            {
                Nombre = r.medicamento.concepto?.term,
                Cantidad = r.medicamento.cantidad,
                CantEnvases = r.medicamento.cantEnvases,
                Presentacion = r.medicamento.presentacion
            },
            Diagnostico = r.diagnostico?.term,
            EstadoDispensa = r.estadoDispensaActual?.tipo
        };
    }

    public static Receta MapToRecetaModel(this PrescripcionRecetarDto p)
    {
        var fecha = p.FechaCreacion.HasValue
            ? DateTimeHelper.ToArgentinaTime(p.FechaCreacion.Value)
            : default;

        return new Receta
        {
            Id = p.IdPrescripcion,
            Origen = OrigenReceta.Recetar,
            FechaRegistro = fecha,
            FechaPrestacion = fecha,
            Profesional = p.Profesional == null ? null : MapProfesional(p.Profesional),
            Medicamento = p.Medicamento == null ? null : new MedicamentoReceta
            {
                Nombre = !string.IsNullOrWhiteSpace(p.Medicamento.Concepto?.Term)
                    ? p.Medicamento.Concepto.Term
                    : p.Medicamento.Nombre,
                Cantidad = (int)Math.Round(p.Medicamento.Cantidad ?? 0),
                Presentacion = p.Medicamento.Presentacion,
                UnidadMedida = p.Medicamento.UnidadMedida
            },
            Diagnostico = p.Diagnostico,
            EstadoDispensa = !string.IsNullOrWhiteSpace(p.EstadoActual) ? p.EstadoActual : p.EstadoDispensa
        };
    }

    private static ProfesionalReceta MapProfesional(ProfesionalRecetarDto profesional)
    {
        var (apellido, nombre) = SepararApellidoNombre(profesional.Nombre);

        return new ProfesionalReceta
        {
            Apellido = apellido,
            Nombre = nombre,
            Matricula = int.TryParse(profesional.Matricula, out var matricula) ? matricula : null,
            Especialidad = profesional.Especialidad
        };
    }

    private static (string? Apellido, string? Nombre) SepararApellidoNombre(string? nombreCompleto)
    {
        if (string.IsNullOrWhiteSpace(nombreCompleto))
            return (null, null);

        var partes = nombreCompleto.Split(',', 2, StringSplitOptions.TrimEntries);
        return partes.Length == 2
            ? (partes[0], partes[1])
            : (nombreCompleto.Trim(), null);
    }
}
