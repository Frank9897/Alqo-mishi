using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AlqoMishi.Datos;
using AlqoMishi.Entidades;
using AlqoMishi.Servicios;

var builder = WebApplication.CreateBuilder(args);
var rutaDb = Path.Combine(builder.Environment.ContentRootPath, "alqomishi.db");

builder.Services.AddDbContext<AlqoMishiDbContext>(options =>
    options.UseSqlite($"Data Source={rutaDb}"));

builder.Services
.AddIdentity<Usuario, IdentityRole<int>>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<AlqoMishiDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<TurnoServicio>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    await SeedUsuarioAnonimo(services);
}

app.Run();

static async Task SeedUsuarioAnonimo(IServiceProvider services)
{
    var userManager = services.GetRequiredService<UserManager<Usuario>>();

    var usuario = await userManager.FindByEmailAsync("anonimo@alqomishi.com");

    if (usuario == null)
    {
        var anonimo = new Usuario
        {
            UserName = "anonimo@alqomishi.com",
            Email = "anonimo@alqomishi.com",
            Nombre = "Cliente",
            Apellido = "Anonimo",
            EmailConfirmed = true
        };

        await userManager.CreateAsync(anonimo, "Anonimo123!");
    }
}