using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AlqoMishi.Entidades;

namespace AlqoMishi.Datos;

public class AlqoMishiDbContext 
: IdentityDbContext<Usuario, IdentityRole<int>, int>
{
    public AlqoMishiDbContext(DbContextOptions<AlqoMishiDbContext> options)
        : base(options)
    {
    }

    public DbSet<Mascota> Mascotas { get; set; }

    public DbSet<Empleado> Empleados { get; set; }

    public DbSet<Franja> Franjas { get; set; }

    public DbSet<Turno> Turnos { get; set; }

    public DbSet<DisponibilidadEmpleado> DisponibilidadesEmpleado { get; set; }
}