using DMendez.Data.Common;
using Microsoft.AspNetCore.Identity;

namespace DMendez.Web.Seed;

/// <summary>Opciones de configuración para el administrador por defecto.</summary>
public class OpcionesAdminPorDefecto
{
    /// <summary>Correo electrónico del administrador por defecto.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Contraseña del administrador por defecto.</summary>
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Servicio de seed idempotente que crea los roles del sistema y el usuario
/// administrador por defecto al iniciar la aplicación, si aún no existen.
/// </summary>
public class SemillaAdminPorDefecto
{
    /// <summary>Nombre del rol administrador del sistema.</summary>
    public const string NombreRolAdministrador = "Administrador";

    /// <summary>Nombre del rol cliente del sistema.</summary>
    public const string NombreRolCliente = "Cliente";

    private readonly UserManager<IdentityUser> _gestorUsuarios;
    private readonly RoleManager<IdentityRole> _gestorRoles;
    private readonly IConfiguration _configuracion;
    private readonly ILogger<SemillaAdminPorDefecto> _logger;

    /// <summary>Inicializa el servicio de seed con las dependencias necesarias.</summary>
    /// <param name="gestorUsuarios">El gestor de usuarios de Identity.</param>
    /// <param name="gestorRoles">El gestor de roles de Identity.</param>
    /// <param name="configuracion">La configuración de la aplicación.</param>
    /// <param name="logger">El logger del servicio.</param>
    public SemillaAdminPorDefecto(
        UserManager<IdentityUser> gestorUsuarios,
        RoleManager<IdentityRole> gestorRoles,
        IConfiguration configuracion,
        ILogger<SemillaAdminPorDefecto> logger)
    {
        _gestorUsuarios = gestorUsuarios;
        _gestorRoles = gestorRoles;
        _configuracion = configuracion;
        _logger = logger;
    }

    /// <summary>
    /// Ejecuta el proceso de seed de forma idempotente:
    /// crea los roles del sistema y el usuario administrador si no existen.
    /// </summary>
    /// <returns>
    /// <see cref="OperationResult"/> con éxito si el seed se completó correctamente,
    /// o con el mensaje de error si falló algún paso.
    /// </returns>
    public async Task<OperationResult> EjecutarAsync()
    {
        var resultadoRoles = await SembrarRolesAsync();
        if (!resultadoRoles.EsExitoso)
            return resultadoRoles;

        var resultadoAdmin = await SembrarAdministradorAsync();
        return resultadoAdmin;
    }

    /// <summary>Crea los roles del sistema si no existen.</summary>
    /// <returns>Resultado de la operación de creación de roles.</returns>
    private async Task<OperationResult> SembrarRolesAsync()
    {
        string[] roles = { NombreRolAdministrador, NombreRolCliente };

        foreach (string rol in roles)
        {
            if (await _gestorRoles.RoleExistsAsync(rol))
            {
                _logger.LogDebug("Rol '{Rol}' ya existe. Omitiendo creación.", rol);
                continue;
            }

            var resultado = await _gestorRoles.CreateAsync(new IdentityRole(rol));
            if (!resultado.Succeeded)
            {
                string errores = string.Join(", ", resultado.Errors.Select(e => e.Description));
                _logger.LogError("No se pudo crear el rol '{Rol}': {Errores}", rol, errores);
                return OperationResult.Fallido($"No se pudo crear el rol '{rol}': {errores}");
            }

            _logger.LogInformation("Rol '{Rol}' creado exitosamente.", rol);
        }

        return OperationResult.Exitoso();
    }

    /// <summary>Crea el usuario administrador por defecto si no existe ningún administrador.</summary>
    /// <returns>Resultado de la operación de creación del administrador.</returns>
    private async Task<OperationResult> SembrarAdministradorAsync()
    {
        var administradoresExistentes = await _gestorUsuarios
            .GetUsersInRoleAsync(NombreRolAdministrador);

        if (administradoresExistentes.Any())
        {
            _logger.LogDebug("Ya existe un administrador. Omitiendo creación.");
            return OperationResult.Exitoso();
        }

        string? email = _configuracion["DefaultAdmin:Email"];
        string? password = _configuracion["DefaultAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return OperationResult.Fallido(
                "Las credenciales del administrador por defecto no están configuradas en appsettings.json.");
        }

        var admin = new IdentityUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var resultadoCreacion = await _gestorUsuarios.CreateAsync(admin, password);
        if (!resultadoCreacion.Succeeded)
        {
            string errores = string.Join(", ", resultadoCreacion.Errors.Select(e => e.Description));
            return OperationResult.Fallido($"No se pudo crear el administrador: {errores}");
        }

        var resultadoRol = await _gestorUsuarios.AddToRoleAsync(admin, NombreRolAdministrador);
        if (!resultadoRol.Succeeded)
        {
            string errores = string.Join(", ", resultadoRol.Errors.Select(e => e.Description));
            return OperationResult.Fallido($"No se pudo asignar el rol al administrador: {errores}");
        }

        _logger.LogInformation("Administrador por defecto '{Email}' creado exitosamente.", email);
        return OperationResult.Exitoso();
    }
}
