using Microsoft.AspNetCore.Mvc;
using AlqoMishi.Servicios;
using Microsoft.EntityFrameworkCore;
using AlqoMishi.Datos;
using AlqoMishi.Entidades;
using AlqoMishi.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
namespace AlqoMishi.Controllers;

[AllowAnonymous]
public class TurnosController : Controller
{
    private readonly TurnoServicio _turnoServicio;
    private readonly AlqoMishiDbContext _context;
    private readonly UserManager<Usuario> _userManager;

    public TurnosController(
        TurnoServicio turnoServicio,
        AlqoMishiDbContext context,
        UserManager<Usuario> userManager)
    {
        _turnoServicio = turnoServicio;
        _context = context;
        _userManager = userManager;
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
        if (dto == null)
            return BadRequest("Datos inválidos");

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        int clienteId;

        if (!string.IsNullOrEmpty(userIdClaim))
        {
            clienteId = int.Parse(userIdClaim);
        }
        else
        {
            var anonimo = await _userManager.FindByEmailAsync("anonimo@alqomishi.com");

            if (anonimo == null)
                return BadRequest("Usuario anónimo no configurado");

            clienteId = anonimo.Id;
        }

        Mascota mascota = null;

        // SI seleccionó mascota existente
        if (dto.MascotaId != null && dto.MascotaId > 0)
        {
            mascota = await _context.Mascotas
                .FirstOrDefaultAsync(m => m.Id == dto.MascotaId);

            if (mascota == null)
                return BadRequest("Mascota no encontrada");
        }
        else
        {
            // Validar que haya datos mínimos
            if (string.IsNullOrWhiteSpace(dto.Mascota))
            {
                return BadRequest("Debe ingresar el nombre de la mascota");
            }

            mascota = new Mascota
            {
                Nombre = dto.Mascota,
                Especie = dto.Especie ?? "",
                Raza = dto.Raza ?? "",
                Edad = dto.Edad ?? 0,
                Sexo = dto.Sexo ?? "",
                Notas = dto.Notas ?? "",
                PropietarioId = clienteId
            };

            _context.Mascotas.Add(mascota);
            await _context.SaveChangesAsync();
        }
        var franja = await _context.Franjas
            .FirstOrDefaultAsync(f => f.Id == dto.FranjaId);

        if (franja == null)
            return BadRequest("Franja no encontrada");
            
        if (franja.Capacidad <= await _context.Turnos.CountAsync(t => t.FranjaId == franja.Id))
        {
            return BadRequest("Esta franja ya está completa");
        }
        var turno = new Turno
        {
            MascotaId = mascota.Id,
            ClienteId = clienteId,
            EmpleadoId = franja.EmpleadoId,
            FranjaId = franja.Id,
            Estado = "Reservado",
            PrecioFinal = franja.Precio
        };

        _context.Turnos.Add(turno);

        await _context.SaveChangesAsync();

        return Ok(new { success = true });
    }
}