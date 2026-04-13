using DMendez.Data.Context;
using DMendez.Data.Repositories;
using DMendez.Data.Repositories.Interfaces;
using DMendez.Data.Services;
using DMendez.Data.Services.Interfaces;
using DMendez.Web.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DMendez.Web.Extensions;

/// <summary>Extensiones del contenedor de servicios para registrar los servicios de la aplicación.</summary>
public static class ServiciosExtension
{
    /// <summary>Registra el contexto de base de datos InMemory en el contenedor de DI.</summary>
    /// <param name="servicios">El contenedor de servicios.</param>
    /// <returns>El mismo contenedor para encadenamiento.</returns>
    public static IServiceCollection AgregarBaseDatos(this IServiceCollection servicios)
    {
        servicios.AddDbContext<AppDbContext>(opciones =>
            opciones.UseInMemoryDatabase("DMendezDb"));

        return servicios;
    }

    /// <summary>Registra y configura ASP.NET Core Identity con roles y cookies.</summary>
    /// <param name="servicios">El contenedor de servicios.</param>
    /// <param name="configuracion">La configuración de la aplicación.</param>
    /// <returns>El mismo contenedor para encadenamiento.</returns>
    public static IServiceCollection AgregarIdentidad(
        this IServiceCollection servicios,
        IConfiguration configuracion)
    {
        servicios.AddIdentity<IdentityUser, IdentityRole>(opciones =>
        {
            opciones.Password.RequiredLength = 8;
            opciones.Password.RequireDigit = true;
            opciones.Password.RequireUppercase = true;
            opciones.Password.RequireNonAlphanumeric = true;
            opciones.SignIn.RequireConfirmedAccount = false;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        int tiempoSesion = configuracion.GetValue<int>("Session:TimeoutMinutes");

        servicios.ConfigureApplicationCookie(opciones =>
        {
            opciones.LoginPath = "/Cuenta/Iniciar";
            opciones.LogoutPath = "/Cuenta/Cerrar";
            opciones.AccessDeniedPath = "/Cuenta/AccesoDenegado";
            opciones.ExpireTimeSpan = TimeSpan.FromMinutes(tiempoSesion > 0 ? tiempoSesion : 30);
            opciones.SlidingExpiration = true;
        });

        return servicios;
    }

    /// <summary>Registra todos los repositorios y servicios de la aplicación.</summary>
    /// <param name="servicios">El contenedor de servicios.</param>
    /// <returns>El mismo contenedor para encadenamiento.</returns>
    public static IServiceCollection AgregarServiciosAplicacion(this IServiceCollection servicios)
    {
        // Repositorios
        servicios.AddScoped<IRepositorioProducto, RepositorioProducto>();
        servicios.AddScoped<IRepositorioPedido, RepositorioPedido>();
        servicios.AddScoped<IRepositorioPromocion, RepositorioPromocion>();

        // Servicios
        servicios.AddScoped<IServicioPago, ServicioPagoFalso>();

        // Seed
        servicios.AddScoped<SemillaAdminPorDefecto>();

        return servicios;
    }
}
