using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlqoMishi.Datos;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace AlqoMishi.Controllers;

[Authorize(Roles = "Veterinario")]
public class VeterinarioController : Controller
{
    private readonly AlqoMishiDbContext _context;

    public VeterinarioController(AlqoMishiDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Agenda()
    {
        int usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var empleado = await _context.Empleados
            .FirstOrDefaultAsync(e => e.UsuarioId == usuarioId);

        if (empleado == null)
            return Unauthorized();

        var hoy = DateTime.Today;

        var turnos = await _context.Turnos
            .Include(t => t.Mascota)
            .Include(t => t.Franja)
            .Include(t => t.Cliente)
            .Where(t =>
                t.EmpleadoId == empleado.Id &&
                t.Franja.Fecha.Date == hoy &&
                t.Estado != "Cancelado")
            .OrderBy(t => t.Franja.HoraInicio)
            .ToListAsync();

        return View(turnos);
    }
}