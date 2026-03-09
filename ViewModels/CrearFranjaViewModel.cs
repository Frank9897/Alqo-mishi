using Microsoft.AspNetCore.Mvc.Rendering;

namespace AlqoMishi.ViewModels;

public class CrearFranjaViewModel
{
    public string Nombre { get; set; }

    public DateTime Fecha { get; set; }

    public TimeSpan HoraInicio { get; set; }

    public TimeSpan HoraFin { get; set; }

    public int Capacidad { get; set; }

    public decimal Precio { get; set; }

    public int EmpleadoId { get; set; }

    public List<SelectListItem> Empleados { get; set; }
}