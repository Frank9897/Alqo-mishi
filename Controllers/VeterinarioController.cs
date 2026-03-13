using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AlqoMishi.Datos;

namespace AlqoMishi.Controllers;

[Authorize(Roles = "Empleado")]
public class VeterinarioController : Controller
{
    private readonly AlqoMishiDbContext _context;

    public VeterinarioController(AlqoMishiDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Agenda()
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var empleado = await _context.Empleados
            .FirstOrDefaultAsync(e => e.UsuarioId == usuarioId);

        if (empleado == null)
            return NotFound();

        var hoy = DateTime.Today;

        var turnos = await _context.Turnos
            .Include(t => t.Mascota)
                .ThenInclude(m => m.Propietario)
            .Include(t => t.Franja)
            .Where(t =>
                t.EmpleadoId == empleado.Id &&
                t.Franja.Fecha.Date == hoy &&
                t.Estado != "Cancelado")
            .OrderBy(t => t.Franja.HoraInicio)
            .ToListAsync();

        return View(turnos);
    }
}