using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AlqoMishi.Datos;

namespace AlqoMishi.Controllers
{
    [Route("api/mascotas")]
    [ApiController]
    [Authorize]
    public class ApiMascotasController : ControllerBase
    {
        private readonly AlqoMishiDbContext _context;
        public ApiMascotasController(AlqoMishiDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetMascotas()
        {
            var usuarioIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(usuarioIdStr)) return Unauthorized();

            var usuarioId = int.Parse(usuarioIdStr);

            var mascotas = await _context.Mascotas
                .Where(m => m.PropietarioId == usuarioId)
                .Select(m => new { m.Id, m.Nombre })
                .ToListAsync();

            return Ok(mascotas);
        }
    }
}