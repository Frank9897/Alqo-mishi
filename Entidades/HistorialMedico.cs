using System.ComponentModel.DataAnnotations;

namespace AlqoMishi.Entidades;

public class HistorialMedico
{
    public int Id { get; set; }

    public int MascotaId { get; set; }
    public Mascota Mascota { get; set; }

    public int TurnoId { get; set; }
    public Turno Turno { get; set; }

    public int VeterinarioId { get; set; }
    public Empleado Veterinario { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    [Required]
    [Display(Name = "Diagnóstico")]
    public string Diagnostico { get; set; }

    [Display(Name = "Tratamiento")]
    public string Tratamiento { get; set; }

    [Display(Name = "Medicamentos")]
    public string Medicamentos { get; set; }

    [Display(Name = "Peso (kg)")]
    public decimal? Peso { get; set; }

    [Display(Name = "Temperatura (°C)")]
    public decimal? Temperatura { get; set; }

    [Display(Name = "Observaciones")]
    public string? Observaciones { get; set; }
}