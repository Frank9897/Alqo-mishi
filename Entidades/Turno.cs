namespace AlqoMishi.Entidades;

public class Turno
{
    public int Id { get; set; }

    public int MascotaId { get; set; }

    public Mascota Mascota { get; set; }

    public int ClienteId { get; set; }

    public Usuario Cliente { get; set; }

    public int FranjaId { get; set; }

    public Franja Franja { get; set; }

    public int? EmpleadoId { get; set; }

    public Empleado Empleado { get; set; }

    public string Estado { get; set; } = "Reservado";

    public decimal PrecioFinal { get; set; }

    public string Observaciones { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}