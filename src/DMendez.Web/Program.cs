using DMendez.Web.Extensions;
using DMendez.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ── Servicios ────────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

builder.Services.AgregarBaseDatos();
builder.Services.AgregarIdentidad(builder.Configuration);
builder.Services.AgregarServiciosAplicacion();
builder.Services.AgregarUbicacionesVistas();

builder.Services.AddSession(opciones =>
{
    opciones.IdleTimeout = TimeSpan.FromMinutes(
        builder.Configuration.GetValue<int>("Session:TimeoutMinutes") is int t && t > 0 ? t : 30);
    opciones.Cookie.HttpOnly = true;
    opciones.Cookie.IsEssential = true;
});

// ── Pipeline ─────────────────────────────────────────────────────────────────
var app = builder.Build();

app.UseMiddleware<MiddlewareExcepcionGlobal>();

if (!app.Environment.IsDevelopment())
    app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// Rutas convencionales
app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Catalogo}/{action=Index}/{id?}");

// ── Seed ─────────────────────────────────────────────────────────────────────
await app.SembrarAdminPorDefectoAsync();

app.Run();

/// <summary>Clase parcial pública para permitir el acceso desde DMendez.Tests.</summary>
public partial class Program { }
