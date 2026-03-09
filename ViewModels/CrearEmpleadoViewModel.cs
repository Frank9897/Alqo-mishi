namespace AlqoMishi.ViewModels;

public class CrearEmpleadoViewModel
{
    public string Nombre { get; set; }

    public string Apellido { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string Especialidad { get; set; }

    public bool EstaDisponible { get; set; } = true;
}