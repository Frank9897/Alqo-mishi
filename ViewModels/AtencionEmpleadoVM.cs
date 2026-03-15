public class AtencionEmpleadoVM
{
    public int EmpleadoId { get; set; }

    public string NombreEmpleado { get; set; }

    public int Hoy { get; set; }

    public int Semana { get; set; }

    public int Mes { get; set; }
}


public class HistorialAtencionesVM
{
    public int TotalHoy { get; set; }

    public int TotalSemana { get; set; }

    public int TotalMes { get; set; }

    public string TopEmpleado { get; set; }

    public int TopCantidad { get; set; }

    public List<AtencionEmpleadoVM> Empleados { get; set; } = new();
}
