using System.ComponentModel.DataAnnotations;

public class RecuperarPasswordViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string NuevaPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("NuevaPassword")]
    public string ConfirmarPassword { get; set; }
}