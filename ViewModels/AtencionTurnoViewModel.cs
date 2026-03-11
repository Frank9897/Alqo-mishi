using System.ComponentModel.DataAnnotations;

namespace AlqoMishi.ViewModels;

public class AtencionTurnoViewModel
{
    public int TurnoId { get; set; }

    public string Mascota { get; set; }

    public string Cliente { get; set; }

    public DateTime Fecha { get; set; }

    [Required]
    public string Diagnostico { get; set; }

    public string? Tratamiento { get; set; }

    public string? Medicamentos { get; set; }

    public decimal? Peso { get; set; }

    public decimal? Temperatura { get; set; }

    public string? Observaciones { get; set; }
}