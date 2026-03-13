using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AlqoMishi.Datos;
using AlqoMishi.Entidades;

namespace AlqoMishi.Controllers;

public class VeterinarioController : Controller
{
    private readonly AlqoMishiDbContext _context;

    public VeterinarioController(AlqoMishiDbContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Empleado,Admin")]
    public async Task<IActionResult> Agenda()
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var hoy = DateTime.Today;

        IQueryable<Turno> query = _context.Turnos
            .Include(t => t.Mascota)
                .ThenInclude(m => m.Propietario)
            .Include(t => t.Franja);

        // si es empleado -> solo sus turnos
        if (User.IsInRole("Empleado"))
        {
            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(e => e.UsuarioId == usuarioId);

            if (empleado == null)
                return NotFound();

            query = query.Where(t => t.EmpleadoId == empleado.Id);
        }

        // admin ve todos
        var turnos = await query
        .Where(t => t.Franja.Fecha.Date == hoy && t.Estado != "Cancelado")
        .ToListAsync();

        turnos = turnos
            .OrderBy(t => t.Franja.HoraInicio)
            .ToList();

        return View(turnos);
    }
}