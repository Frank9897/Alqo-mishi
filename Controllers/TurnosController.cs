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

        int usuarioId;

        if (User?.Identity?.IsAuthenticated == true)
        {
            usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
        else
        {
            var anon = await _userManager.FindByEmailAsync("anonimo@alqomishi.com");

            if (anon == null)
            {
                anon = new Usuario
                {
                    UserName = "anonimo@alqomishi.com",
                    Email = "anonimo@alqomishi.com",
                    Nombre = "Cliente",
                    Apellido = "Anonimo",
                    EmailConfirmed = true
                };

                await _userManager.CreateAsync(anon, "Anonimo123!");
            }

            usuarioId = anon.Id;
        }
        var mascota = await _turnoServicio.CrearMascotaAsync(
            dto.Mascota ?? "",
            dto.Especie ?? "",
            dto.Raza ?? "",
            dto.Edad,
            dto.Sexo ?? "",
            dto.Notas ?? "",
            usuarioId
        );

        var franja = await _turnoServicio.ObtenerFranjaAsync(dto.FranjaId);

        if (franja == null)
            return BadRequest("Franja inválida");

        var turnoCreado = await _turnoServicio.CrearTurnoAsync(
            mascota.Id,
            usuarioId,
            franja.Id,
            dto.Observaciones ?? ""
        );

        if (!turnoCreado)
            return BadRequest("No hay cupos disponibles");

        return Ok();
    }
}