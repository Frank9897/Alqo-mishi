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

    public async Task<Mascota> CrearMascotaAsync(
    string nombre,
    string especie,
    string raza,
    int? edad,
    string sexo,
    string notas,
    int propietarioId)
    {

    var mascota = new Mascota
    {
    Nombre = string.IsNullOrWhiteSpace(nombre)
    ? "Mascota sin nombre"
    : nombre,

    Especie = string.IsNullOrWhiteSpace(especie)
    ? "No especificado"
    : especie,

    Raza = string.IsNullOrWhiteSpace(raza)
    ? "No especificado"
    : raza,

    Edad = edad ?? 0,

    Sexo = string.IsNullOrWhiteSpace(sexo)
    ? "No especificado"
    : sexo,

    Notas = notas,

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

    public async Task<bool> CrearTurnoAsync(
        int mascotaId,
        int clienteId,
        int franjaId,
        string observaciones)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        var franja = await _context.Franjas
            .Include(f => f.Turnos)
            .FirstOrDefaultAsync(f => f.Id == franjaId);

        if (franja == null)
            return false;

        var ocupados = await _context.Turnos
            .CountAsync(t => t.FranjaId == franjaId && t.Estado != "Cancelado");

        if (ocupados >= franja.Capacidad)
            return false;

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

        await transaction.CommitAsync();

        return true;
    }
}