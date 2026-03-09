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
            .ToListAsync();

        var eventos = franjas.Select(f =>
        {
            var ocupados = f.Turnos.Count(t => t.Estado != "Cancelado");

            var disponibles = f.Capacidad - ocupados;

            string titulo;

            if (disponibles <= 0)
            {
                titulo = $"🔴 COMPLETO\n{f.Nombre}";
            }
            else
            {
                titulo = $"{f.Nombre}\n🟢 {disponibles}/{f.Capacidad} cupos\n${f.Precio}";
            }

            return new
            {
                id = f.Id,
                title = titulo,
                start = f.Fecha.Add(f.HoraInicio),
                end = f.Fecha.Add(f.HoraFin),
                color = disponibles <= 0 ? "#dc3545" : "#198754"
            };
        });

        return Json(eventos);
    }

    [HttpPost]
    public async Task<IActionResult> ReservarTurno([FromBody] ReservaTurnoDto dto)
    {
        int usuarioId;

        if (User.Identity.IsAuthenticated)
        {
            usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
        else
        {
            // crear usuario temporal
            var usuario = new Usuario
            {
                UserName = dto.Telefono,
                PhoneNumber = dto.Telefono,
                Nombre = dto.NombreCliente,
                RolSistema = "ClienteAnonimo"
            };

            var result = await _userManager.CreateAsync(usuario);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            usuarioId = usuario.Id;
        }

        int mascotaId;

        if (dto.MascotaId.HasValue)
        {
            mascotaId = dto.MascotaId.Value;
        }
        else
        {
            var mascota = await _turnoServicio.CrearMascotaAsync(
                dto.Mascota,
                dto.Especie,
                dto.Raza,
                dto.Observaciones,
                usuarioId
            );

            mascotaId = mascota.Id;
        }

        var franja = await _turnoServicio.ObtenerFranjaAsync(dto.FranjaId);

        if (franja == null)
            return BadRequest("Franja inválida");

        var turnoCreado = await _turnoServicio.CrearTurnoAsync(
            mascotaId,
            usuarioId,
            franja.Id,
            dto.Observaciones
        );

        if (!turnoCreado)
            return BadRequest("No hay cupos");

        return Ok();
    }
}