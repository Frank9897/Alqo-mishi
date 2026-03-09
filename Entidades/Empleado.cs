namespace AlqoMishi.Entidades;

public class Empleado
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public Usuario Usuario { get; set; }

    public string Especialidad { get; set; }

    public bool EstaDisponible { get; set; } = true;

    public DateTime FechaAlta { get; set; } = DateTime.UtcNow;
}