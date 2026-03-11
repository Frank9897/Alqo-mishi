using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AlqoMishi.Datos;
using AlqoMishi.Entidades;

namespace AlqoMishi.Controllers;

[Authorize]
public class MascotasController : Controller
{
    private readonly AlqoMishiDbContext _context;

    public MascotasController(AlqoMishiDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> MisMascotas()
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var mascotas = await _context.Mascotas
            .Where(m => m.PropietarioId == usuarioId)
            .ToListAsync();

        return View(mascotas);
    }

    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Crear(Mascota mascota)
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        mascota.PropietarioId = usuarioId;

        _context.Mascotas.Add(mascota);

        await _context.SaveChangesAsync();

        return RedirectToAction("MisMascotas");
    }

    public async Task<IActionResult> Historial(int id)
{
    var mascota = await _context.Mascotas
        .Include(m => m.HistorialesMedicos)
        .ThenInclude(h => h.Turno)
        .FirstOrDefaultAsync(m => m.Id == id);

    if (mascota == null)
        return NotFound();

    return View(mascota);
}
}