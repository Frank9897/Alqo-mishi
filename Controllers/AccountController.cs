using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AlqoMishi.Entidades;
using AlqoMishi.ViewModels;
using System.Security.Claims;
namespace AlqoMishi.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<Usuario> _signInManager;
    private readonly UserManager<Usuario> _userManager;

    public AccountController(
        SignInManager<Usuario> signInManager,
        UserManager<Usuario> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    // =========================
    // LOGIN
    // =========================

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var resultado = await _signInManager.PasswordSignInAsync(
            model.Email,
            model.Password,
            false,
            false);

        if (resultado.Succeeded)
        {
            var usuario = await _userManager.FindByEmailAsync(model.Email);

            var claims = new List<Claim>
            {
                new Claim("Nombre", usuario.Nombre + " " + usuario.Apellido)
            };

            await _userManager.AddClaimsAsync(usuario, claims);

            return RedirectToAction("Index","Home");
        }

        ModelState.AddModelError("","Credenciales incorrectas");

        return View(model);
    }

    // =========================
    // REGISTRO
    // =========================

    public IActionResult Registro()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Registro(RegistroViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var usuario = new Usuario
        {
            UserName = model.Email,
            Email = model.Email,
            Nombre = model.Nombre,
            Apellido = model.Apellido,
            RolSistema = "Cliente"
        };

        var resultado = await _userManager.CreateAsync(usuario, model.Password);

        if (!resultado.Succeeded)
        {
            foreach(var error in resultado.Errors)
                ModelState.AddModelError("",error.Description);

            return View(model);
        }

        await _userManager.AddToRoleAsync(usuario,"Cliente");

        await _signInManager.SignInAsync(usuario,false);

        return RedirectToAction("Index","Home");
    }

    // =========================
    // CERRAR SESIÓN
    // =========================

    [HttpPost]
    public async Task<IActionResult> CerrarSesion()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction("Index","Home");
    }
}