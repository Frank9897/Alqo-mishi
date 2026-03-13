using System.ComponentModel.DataAnnotations;

namespace AlqoMishi.ViewModels;

public class CrearUsuarioViewModel
{
    [Required]
    public string Nombre { get; set; }

    [Required]
    public string Apellido { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    public string Rol { get; set; }
    public string? Especialidad { get; set; }
}