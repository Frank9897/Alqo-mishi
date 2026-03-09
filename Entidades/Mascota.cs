namespace AlqoMishi.Entidades;

public class Mascota
{
    public int Id { get; set; }

    public string Nombre { get; set; }

    public int Edad { get; set; }

    public string Especie { get; set; }

    public string Raza { get; set; }

    public string Sexo { get; set; }

    public string Notas { get; set; }

    public int PropietarioId { get; set; }

    public Usuario? Propietario { get; set; }
}