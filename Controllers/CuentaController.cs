using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AlqoMishi.Entidades;

namespace AlqoMishi.Controllers;

public class CuentaController : Controller
{
    private readonly SignInManager<Usuario> _signInManager;
    private readonly UserManager<Usuario> _userManager;

    public CuentaController(
        SignInManager<Usuario> signInManager,
        UserManager<Usuario> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var usuario = await _userManager.FindByEmailAsync(email);

        if (usuario == null)
            return View();

        var resultado = await _signInManager.PasswordSignInAsync(
            usuario,
            password,
            false,
            false);

        if (resultado.Succeeded)
            return RedirectToAction("Index", "Home");

        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}