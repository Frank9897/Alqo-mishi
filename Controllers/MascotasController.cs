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

    public async Task<IActionResult> Index()
    {
        return await MisMascotas();
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

    [Authorize]
    public async Task<IActionResult> Editar(int id)
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var mascota = await _context.Mascotas
            .FirstOrDefaultAsync(m => m.Id == id && m.PropietarioId == usuarioId);

        if (mascota == null)
            return NotFound();

        return View(mascota);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Editar(Mascota mascota)
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var mascotaDb = await _context.Mascotas
            .FirstOrDefaultAsync(m => m.Id == mascota.Id && m.PropietarioId == usuarioId);

        if (mascotaDb == null)
            return NotFound();

        mascotaDb.Nombre = mascota.Nombre;
        mascotaDb.Especie = mascota.Especie;
        mascotaDb.Raza = mascota.Raza;
        mascotaDb.Edad = mascota.Edad;
        mascotaDb.Sexo = mascota.Sexo;

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
    [Authorize]
    public async Task<IActionResult> Eliminar(int id)
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
    
        var mascota = await _context.Mascotas
            .FirstOrDefaultAsync(m => m.Id == id && m.PropietarioId == usuarioId);

        if (mascota == null)
            return NotFound();

        _context.Mascotas.Remove(mascota);

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}