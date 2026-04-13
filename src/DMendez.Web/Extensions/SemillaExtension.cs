using DMendez.Web.Seed;

namespace DMendez.Web.Extensions;

/// <summary>Extensiones del pipeline de la aplicación para ejecutar el seed inicial.</summary>
public static class SemillaExtension
{
    /// <summary>Ejecuta el proceso de seed del administrador por defecto al iniciar la aplicación.</summary>
    /// <param name="app">La aplicación web construida.</param>
    /// <returns>La misma aplicación para encadenamiento.</returns>
    public static async Task<WebApplication> SembrarAdminPorDefectoAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();

        var semilla = scope.ServiceProvider.GetRequiredService<SemillaAdminPorDefecto>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<SemillaAdminPorDefecto>>();

        var resultado = await semilla.EjecutarAsync();

        if (resultado.EsExitoso)
            logger.LogInformation("Seed de administrador completado exitosamente.");
        else
            logger.LogWarning("Seed de administrador falló: {Error}", resultado.MensajeError);

        return app;
    }
}
