using Microsoft.AspNetCore.Mvc;
using AlqoMishi.Servicios;
using Microsoft.EntityFrameworkCore;
using AlqoMishi.Datos;
using AlqoMishi.Entidades;
using AlqoMishi.ViewModels;
using System.Security.Claims;

namespace AlqoMishi.Controllers;

public class TurnosController : Controller
{
    private readonly TurnoServicio _turnoServicio;
    private readonly AlqoMishiDbContext _context;

    public TurnosController(
        TurnoServicio turnoServicio,
        AlqoMishiDbContext context)
    {
        _turnoServicio = turnoServicio;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerFranjas()
    {
        var franjas = await _context.Franjas
            .Include(f => f.Turnos)
            .Include(f => f.Empleado)
            .ThenInclude(e => e.Usuario)        
            .ToListAsync();

        var eventos = franjas.Select(f =>
        {
            var ocupados = f.Turnos.Count(t => t.Estado != "Cancelado");

            var veterinario = f.Empleado?.Usuario?.Nombre + " " +
                            f.Empleado?.Usuario?.Apellido;

            var titulo =
                $"Consulta veterinaria\n" +
                $"{veterinario}\n" +
                $"{ocupados}/{f.Capacidad} cupos\n" +
                $"${f.Precio:N0}";

            var color = "#198754";

            if (ocupados >= f.Capacidad)
                color = "#dc3545";
            else if (ocupados >= f.Capacidad - 1)
                color = "#ffc107";

            return new
            {
                id = f.Id,
                title = titulo,
                start = f.Fecha.Date + f.HoraInicio,
                end = f.Fecha.Date + f.HoraFin,
                color = color
            };
        });

        return Json(eventos);
    }

    [HttpPost]
    public async Task<IActionResult> ReservarTurno([FromBody] ReservaTurnoDto dto)
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var mascota = await _turnoServicio.CrearMascotaAsync(
            dto.Mascota,
            dto.Especie,
            dto.Raza,
            dto.Observaciones,
            usuarioId
            );

        var franja = await _turnoServicio.ObtenerFranjaAsync(dto.FranjaId);

        if (franja == null)
            return BadRequest();

        var turnoCreado = await _turnoServicio.CrearTurnoAsync(
            mascota.Id,
            0,
            franja.Id,
            dto.Observaciones
        );

        if (!turnoCreado)
            return BadRequest("No hay cupos disponibles");

        return Ok();
    }
}