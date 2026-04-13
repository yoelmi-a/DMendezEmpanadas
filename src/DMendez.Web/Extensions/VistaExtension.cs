namespace DMendez.Web.Extensions;

/// <summary>
/// Extensión para configurar las ubicaciones de vistas personalizadas de los controladores administrativos.
/// </summary>
public static class VistaExtension
{
    /// <summary>
    /// Configura las rutas de búsqueda de vistas para controladores con atributos de ruta.
    /// </summary>
    /// <param name="servicios">El contenedor de servicios.</param>
    /// <returns>El mismo contenedor para encadenamiento.</returns>
    public static IServiceCollection AgregarUbicacionesVistas(this IServiceCollection servicios)
    {
        servicios.Configure<Microsoft.AspNetCore.Mvc.Razor.RazorViewEngineOptions>(opciones =>
        {
            // Permite que los controladores bajo Controllers/Admin/ busquen vistas en Views/Admin/{Accion}
            opciones.ViewLocationFormats.Add("/Views/Admin/Products/{0}.cshtml");
            opciones.ViewLocationFormats.Add("/Views/Admin/Orders/{0}.cshtml");
            opciones.ViewLocationFormats.Add("/Views/Admin/Promotions/{0}.cshtml");
            opciones.ViewLocationFormats.Add("/Views/Admin/Reports/{0}.cshtml");
        });

        return servicios;
    }
}
