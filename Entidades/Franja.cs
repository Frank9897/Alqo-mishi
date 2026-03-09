namespace AlqoMishi.Entidades;

public class Franja
{
    public int Id { get; set; }

    public string Nombre { get; set; }

    public DateTime Fecha { get; set; }

    public TimeSpan HoraInicio { get; set; }

    public TimeSpan HoraFin { get; set; }

    public decimal Precio { get; set; }

    public int Capacidad { get; set; }

    public int? EmpleadoId { get; set; }

    public Empleado Empleado { get; set; }

    public string Estado { get; set; } = "Disponible";

    public ICollection<Turno> Turnos { get; set; }
}