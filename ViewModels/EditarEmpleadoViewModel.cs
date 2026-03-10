using System.ComponentModel.DataAnnotations;

namespace AlqoMishi.ViewModels;

public class EditarEmpleadoViewModel
{
    public int Id { get; set; }

    [Required]
    public string Nombre { get; set; }

    [Required]
    public string Apellido { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Especialidad { get; set; }

    public bool EstaDisponible { get; set; }
}