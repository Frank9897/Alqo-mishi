namespace AlqoMishi.ViewModels;

public class TurnoAdminViewModel
{
    public int Id { get; set; }

    public string Mascota { get; set; }

    public string Cliente { get; set; }

    public string Veterinario { get; set; }

    public DateTime Fecha { get; set; }

    public TimeSpan HoraInicio { get; set; }

    public string Estado { get; set; }

    public decimal Precio { get; set; }
    public int MascotaId { get; set; }
}