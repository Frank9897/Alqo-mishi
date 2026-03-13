using System.ComponentModel.DataAnnotations;
namespace AlqoMishi.Entidades;

public class Mascota
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; }

    [Required]
    [StringLength(50)]
    public string Especie { get; set; }

    [StringLength(50)]
    public string Raza { get; set; }

    [Range(0,50)]
    public int Edad { get; set; }

    public string Sexo { get; set; }

    public string Notas { get; set; }

    public int PropietarioId { get; set; }

    public Usuario Propietario { get; set; }
    public List<HistorialMedico> HistorialesMedicos { get; set; } = new();
}