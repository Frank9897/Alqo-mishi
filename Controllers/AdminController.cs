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
                    .ThenInclude(e => e.Usuario)
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

        var empleados = await _context.Empleados
        .Include(e => e.Usuario)
        .Select(e => new
        {
        Id = e.Id,
        Nombre = e.Usuario.Nombre + " " + e.Usuario.Apellido
        })
        .ToListAsync();

        ViewBag.Empleados = new SelectList(empleados,"Id","Nombre");
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
        var empleados = await _context.Empleados
            .Include(e => e.Usuario)
            .ToListAsync();

        return View(empleados);
    }



    public async Task<IActionResult> EditarEmpleado(int id)
    {
        var empleado = await _context.Empleados
            .Include(e => e.Usuario)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (empleado == null)
            return NotFound();

        var vm = new EditarEmpleadoViewModel
        {
            Id = empleado.Id,
            Nombre = empleado.Usuario.Nombre,
            Apellido = empleado.Usuario.Apellido,
            Email = empleado.Usuario.Email,
            Especialidad = empleado.Especialidad,
            EstaDisponible = empleado.EstaDisponible
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> EditarEmpleado(EditarEmpleadoViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var empleado = await _context.Empleados
            .Include(e => e.Usuario)
            .FirstOrDefaultAsync(e => e.Id == model.Id);

        if (empleado == null)
            return NotFound();

        // actualizar datos usuario
        empleado.Usuario.Nombre = model.Nombre;
        empleado.Usuario.Apellido = model.Apellido;
        empleado.Usuario.Email = model.Email;
        empleado.Usuario.UserName = model.Email;

        // actualizar datos empleado
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

    public async Task<IActionResult> Turnos(DateTime? fecha, int? veterinarioId, string estado)
    {
        var query = _context.Turnos
            .Include(t => t.Mascota)
            .Include(t => t.Franja)
            .Include(t => t.Empleado)
                .ThenInclude(e => e.Usuario)
            .Include(t => t.Cliente)
            .AsQueryable();

        if (fecha.HasValue)
        {
            query = query.Where(t => t.Franja.Fecha.Date == fecha.Value.Date);
        }

        if (veterinarioId.HasValue)
        {
            query = query.Where(t => t.EmpleadoId == veterinarioId.Value);
        }

        if (!string.IsNullOrEmpty(estado))
        {
            query = query.Where(t => t.Estado == estado);
        }

       var turnos = await query.ToListAsync();

        turnos = turnos
            .OrderBy(t => t.Franja.Fecha)
            .ThenBy(t => t.Franja.HoraInicio)
            .ToList();

        var lista = turnos.Select(t => new TurnoAdminViewModel
        {
            Id = t.Id,
            MascotaId = t.MascotaId,
            Mascota = t.Mascota.Nombre,
            Cliente = t.Cliente.Nombre + " " + t.Cliente.Apellido,
            Veterinario = t.Empleado.Usuario.Nombre + " " + t.Empleado.Usuario.Apellido,
            Fecha = t.Franja.Fecha,
            HoraInicio = t.Franja.HoraInicio,
            Estado = t.Estado,
            Precio = t.PrecioFinal
        });

        ViewBag.Veterinarios = await _context.Empleados
            .Include(e => e.Usuario)
            .ToListAsync();

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

    public async Task<IActionResult> EnEspera(int id)
    {
        var turno = await _context.Turnos.FindAsync(id);

        if (turno == null)
            return NotFound();

        turno.Estado = "En espera";

        await _context.SaveChangesAsync();

        return RedirectToAction("Turnos");
    }

    public async Task<IActionResult> Atender(int id)
    {
        var turno = await _context.Turnos
            .Include(t => t.Mascota)
                .ThenInclude(m => m.Propietario)
            .Include(t => t.Franja)
            .Include(t => t.Empleado)
                .ThenInclude(e => e.Usuario)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (turno == null)
            return NotFound();

        var vm = new AtencionTurnoViewModel
        {
            TurnoId = turno.Id,
            Mascota = turno.Mascota.Nombre,
            Cliente = turno.Mascota.Propietario.Nombre,
            Fecha = turno.Franja.Fecha
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> GuardarHistorial(HistorialMedico historial)
    {
        if (!ModelState.IsValid)
            return RedirectToAction("Turnos");

        _context.HistorialesMedicos.Add(historial);

        var turno = await _context.Turnos.FindAsync(historial.TurnoId);

        if (turno != null)
            turno.Estado = "Atendido";

        await _context.SaveChangesAsync();

        return RedirectToAction("Historial");
    }

    public async Task<IActionResult> Historial(int mascotaId)
    {
        var mascota = await _context.Mascotas
            .Include(m => m.HistorialesMedicos)
            .ThenInclude(h => h.Veterinario)
            .ThenInclude(v => v.Usuario)
            .FirstOrDefaultAsync(m => m.Id == mascotaId);

        if (mascota == null)
            return NotFound();

        return View(mascota);
    }
}