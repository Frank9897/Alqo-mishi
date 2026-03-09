using Microsoft.AspNetCore.Identity;

namespace AlqoMishi.Entidades;

public class Usuario : IdentityUser<int>
{
    public string Nombre { get; set; }
    public string Apellido { get; set; }

    public string RolSistema { get; set; }

    public bool EsActivo { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}