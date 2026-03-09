using Microsoft.EntityFrameworkCore;
using AlqoMishi.Datos;
using AlqoMishi.Entidades;

namespace AlqoMishi.Servicios;

public class TurnoServicio
{
    private readonly AlqoMishiDbContext _context;

    public TurnoServicio(AlqoMishiDbContext context)
    {
        _context = context;
    }

    public async Task<Mascota> CrearMascotaAsync(string nombre, string especie, string raza, string observaciones, int propietarioId)
    {
        var mascota = new Mascota
        {
            Nombre = nombre,
            Especie = especie,
            Raza = raza,
            Notas = observaciones,
            PropietarioId = propietarioId
        };

        _context.Mascotas.Add(mascota);
        await _context.SaveChangesAsync();
        return mascota;
    }

    public async Task<Franja?> ObtenerFranjaAsync(int franjaId)
    {
        return await _context.Franjas
            .Include(f => f.Turnos)
            .FirstOrDefaultAsync(f => f.Id == franjaId);
    }

    public async Task<bool> CrearTurnoAsync(int mascotaId, int clienteId, int franjaId, string observaciones)
    {
        var franja = await ObtenerFranjaAsync(franjaId);
        if (franja == null) return false;

        var turnosOcupados = franja.Turnos.Count(t => t.Estado != "Cancelado");
        if (turnosOcupados >= franja.Capacidad) return false;

        var turno = new Turno
        {
            MascotaId = mascotaId,
            ClienteId = clienteId,
            FranjaId = franjaId,
            EmpleadoId = franja.EmpleadoId,
            PrecioFinal = franja.Precio,
            Observaciones = observaciones
        };

        _context.Turnos.Add(turno);
        await _context.SaveChangesAsync();
        return true;
    }
}