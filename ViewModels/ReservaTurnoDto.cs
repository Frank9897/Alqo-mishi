using System.ComponentModel.DataAnnotations;

namespace AlqoMishi.ViewModels;

public class ReservaTurnoDto
{
    public int FranjaId { get; set; }

    public int? MascotaId { get; set; }

    public string? NombreCliente { get; set; }

    public string? Telefono { get; set; }

    public string? Mascota { get; set; }

    public string? Especie { get; set; }

    public string? Raza { get; set; }

    public int? Edad { get; set; }

    public string? Sexo { get; set; }

    public string? Notas { get; set; }

    public string? Observaciones { get; set; }
}