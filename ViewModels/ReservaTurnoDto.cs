namespace AlqoMishi.ViewModels;

public class ReservaTurnoDto
{
    public int FranjaId { get; set; }

    // cuando el usuario está logueado
    public int? MascotaId { get; set; }

    // datos para reserva rápida
    public string? NombreCliente { get; set; }

    public string? Telefono { get; set; }

    public string? Mascota { get; set; }

    public string? Especie { get; set; }

    public string? Raza { get; set; }

    public string? Observaciones { get; set; }
}