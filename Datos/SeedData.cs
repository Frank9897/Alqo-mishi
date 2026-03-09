using Microsoft.AspNetCore.Identity;
using AlqoMishi.Entidades;

namespace AlqoMishi.Datos;

public static class SeedData
{
    public static async Task Inicializar(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
        var userManager = services.GetRequiredService<UserManager<Usuario>>();

        string[] roles = { "Admin", "Empleado", "Cliente" };

        foreach (var rol in roles)
        {
            if (!await roleManager.RoleExistsAsync(rol))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(rol));
            }
        }

        var adminEmail = "admin@alqomishi.com";

        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin == null)
        {
            admin = new Usuario
            {
                UserName = adminEmail,
                Email = adminEmail,
                Nombre = "Admin",
                Apellido = "Sistema",
                RolSistema = "Admin"
            };

            await userManager.CreateAsync(admin, "Admin123!");
        }

        if (!await userManager.IsInRoleAsync(admin, "Admin"))
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}