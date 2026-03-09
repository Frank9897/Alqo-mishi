using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlqoMishi.Datos;
using AlqoMishi.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AlqoMishi.ViewModels; 
using Microsoft.AspNetCore.Mvc.Rendering;
namespace AlqoMishi.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly AlqoMishiDbContext _context;
    private readonly UserManager<Usuario> _userManager;
    public AdminController(AlqoMishiDbContext context,UserManager<Usuario> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult CrearEmpleado()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CrearEmpleado(CrearEmpleadoViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var usuario = new Usuario
        {
            UserName = model.Email,
            Email = model.Email,
            Nombre = model.Nombre,
            Apellido = model.Apellido,
            RolSistema = "Empleado"
        };

        var resultado = await _userManager.CreateAsync(usuario, model.Password);

        if (!resultado.Succeeded)
        {
            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        await _userManager.AddToRoleAsync(usuario, "Empleado");

        var empleado = new Empleado
        {
            UsuarioId = usuario.Id,
            Especialidad = model.Especialidad,
            EstaDisponible = true
        };

        _context.Empleados.Add(empleado);

        await _context.SaveChangesAsync();

        return RedirectToAction("Empleados");
    }
    public async Task<IActionResult> Franjas()
    {
        var franjas = await _context.Franjas
            .Include(f => f.Empleado)
            .ToListAsync();

        return View(franjas);
    }

    public async Task<IActionResult> CrearFranja()
    {
        var empleados = await _context.Empleados
            .Include(e => e.Usuario)
            .ToListAsync();

        var model = new CrearFranjaViewModel
        {
            Empleados = empleados.Select(e => new SelectListItem
            {
                Value = e.Id.ToString(),
                Text = e.Usuario.Nombre + " " + e.Usuario.Apellido
            }).ToList()
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> CrearFranja(CrearFranjaViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var franja = new Franja
        {
            Nombre = model.Nombre,
            Fecha = model.Fecha,
            HoraInicio = model.HoraInicio,
            HoraFin = model.HoraFin,
            Capacidad = model.Capacidad,
            Precio = model.Precio,
            EmpleadoId = model.EmpleadoId,
            Estado = "Disponible"
        };

        _context.Franjas.Add(franja);

        await _context.SaveChangesAsync();

        return RedirectToAction("Franjas");
    }

    public async Task<IActionResult> EditarFranja(int id)
    {
        var franja = await _context.Franjas.FindAsync(id);

        if (franja == null)
            return NotFound();

        return View(franja);
    }

    [HttpPost]
    public async Task<IActionResult> EditarFranja(Franja franja)
    {
        _context.Franjas.Update(franja);

        await _context.SaveChangesAsync();

        return RedirectToAction("Franjas");
    }

    public async Task<IActionResult> EliminarFranja(int id)
    {
        var franja = await _context.Franjas.FindAsync(id);

        if (franja != null)
        {
            _context.Franjas.Remove(franja);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Franjas");
    }

    public async Task<IActionResult> Empleados()
    {
        var empleados = await _context.Empleados.ToListAsync();

        return View(empleados);
    }



    public async Task<IActionResult> EditarEmpleado(int id)
    {
        var empleado = await _context.Empleados.FindAsync(id);

        if (empleado == null)
            return NotFound();

        return View(empleado);
    }

    [HttpPost]
    public async Task<IActionResult> EditarEmpleado(int id, Empleado model)
    {
        var empleado = await _context.Empleados.FindAsync(id);

        if (empleado == null)
            return NotFound();

        empleado.Especialidad = model.Especialidad;
        empleado.EstaDisponible = model.EstaDisponible;

        await _context.SaveChangesAsync();

        return RedirectToAction("Empleados");
    }

    public async Task<IActionResult> EliminarEmpleado(int id)
    {
        var empleado = await _context.Empleados.FindAsync(id);

        if (empleado != null)
        {
            _context.Empleados.Remove(empleado);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Empleados");
    }

    public async Task<IActionResult> Turnos()
    {
        var turnos = await _context.Turnos
            .Include(t => t.Mascota)
            .Include(t => t.Franja)
            .Include(t => t.Empleado)
                .ThenInclude(e => e.Usuario)
            .Include(t => t.Cliente)
            .ToListAsync();

        var lista = turnos.Select(t => new TurnoAdminViewModel
        {
            Id = t.Id,
            Mascota = t.Mascota.Nombre,
            Cliente = t.Cliente.Nombre + " " + t.Cliente.Apellido,
            Veterinario = t.Empleado.Usuario.Nombre + " " + t.Empleado.Usuario.Apellido,
            Fecha = t.Franja.Fecha,
            HoraInicio = t.Franja.HoraInicio,
            Estado = t.Estado,
            Precio = t.PrecioFinal
        });

        return View(lista);
    }

    public async Task<IActionResult> MarcarAtendido(int id)
    {
        var turno = await _context.Turnos.FindAsync(id);

        if (turno == null)
            return NotFound();

        turno.Estado = "Atendido";

        await _context.SaveChangesAsync();

        return RedirectToAction("Turnos");
    }

    public async Task<IActionResult> CancelarTurno(int id)
    {
        var turno = await _context.Turnos.FindAsync(id);

        if (turno == null)
            return NotFound();

        turno.Estado = "Cancelado";

        await _context.SaveChangesAsync();

        return RedirectToAction("Turnos");
    }
}