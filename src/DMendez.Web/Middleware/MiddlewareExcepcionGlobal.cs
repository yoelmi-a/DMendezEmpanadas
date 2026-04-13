namespace DMendez.Web.Middleware;

/// <summary>Middleware global para capturar y manejar excepciones no controladas.</summary>
public class MiddlewareExcepcionGlobal
{
    private readonly RequestDelegate _siguiente;
    private readonly ILogger<MiddlewareExcepcionGlobal> _logger;

    /// <summary>Inicializa el middleware con el delegado siguiente y el logger.</summary>
    /// <param name="siguiente">El siguiente middleware en el pipeline.</param>
    /// <param name="logger">El logger para registrar excepciones.</param>
    public MiddlewareExcepcionGlobal(
        RequestDelegate siguiente,
        ILogger<MiddlewareExcepcionGlobal> logger)
    {
        _siguiente = siguiente;
        _logger = logger;
    }

    /// <summary>Procesa la solicitud y captura cualquier excepción no controlada.</summary>
    /// <param name="contexto">El contexto HTTP de la solicitud.</param>
    public async Task InvokeAsync(HttpContext contexto)
    {
        try
        {
            await _siguiente(contexto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado en la solicitud {Ruta}",
                contexto.Request.Path);
            contexto.Response.Redirect("/Error");
        }
    }
}
