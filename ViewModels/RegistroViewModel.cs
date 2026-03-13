using System.ComponentModel.DataAnnotations;

namespace AlqoMishi.ViewModels;

public class RegistroViewModel
{
    [Required]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; }

    [Required]
    [Display(Name = "Apellido")]
    public string Apellido { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}