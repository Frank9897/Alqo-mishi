namespace AlqoMishi.Entidades;

public class DisponibilidadEmpleado
{
    public int Id { get; set; }

    public int EmpleadoId { get; set; }

    public Empleado Empleado { get; set; }

    public DateTime Fecha { get; set; }

    public TimeSpan HoraInicio { get; set; }

    public TimeSpan HoraFin { get; set; }

    public string Tipo { get; set; } // Disponible / NoDisponible

    public string Notas { get; set; }
}