using DMendez.Web.Seed;
using DMendez.Tests.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DMendez.Tests.Unit.Services;

/// <summary>Pruebas unitarias para <see cref="SemillaAdminPorDefecto"/>.</summary>
public class SemillaAdminPorDefectoPruebas
{
    private static IConfiguration CrearConfiguracion(
        string email = "admin@test.com",
        string password = "Admin@12345")
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["DefaultAdmin:Email"] = email,
                ["DefaultAdmin:Password"] = password
            })
            .Build();
    }

    // ── EjecutarAsync — Roles ────────────────────────────────────────────────

    /// <summary>Debe crear ambos roles cuando no existen.</summary>
    [Fact]
    public async Task EjecutarAsync_CuandoRolesNoExisten_CreaRoles()
    {
        // Arrange
        var mockGestorUsuarios = AyudanteIdentity.CrearUserManagerMock();
        var mockGestorRoles = AyudanteIdentity.CrearRoleManagerMock();

        mockGestorRoles
            .Setup(r => r.RoleExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        mockGestorRoles
            .Setup(r => r.CreateAsync(It.IsAny<IdentityRole>()))
            .ReturnsAsync(IdentityResult.Success);

        mockGestorUsuarios
            .Setup(u => u.GetUsersInRoleAsync(SemillaAdminPorDefecto.NombreRolAdministrador))
            .ReturnsAsync(new List<IdentityUser>());

        mockGestorUsuarios
            .Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        mockGestorUsuarios
            .Setup(u => u.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var semilla = new SemillaAdminPorDefecto(
            mockGestorUsuarios.Object,
            mockGestorRoles.Object,
            CrearConfiguracion(),
            new Mock<ILogger<SemillaAdminPorDefecto>>().Object);

        // Act
        var resultado = await semilla.EjecutarAsync();

        // Assert
        Assert.True(resultado.EsExitoso);
        mockGestorRoles.Verify(r => r.CreateAsync(It.IsAny<IdentityRole>()), Times.Exactly(2));
    }

    /// <summary>Debe omitir la creación de roles cuando ya existen (idempotente).</summary>
    [Fact]
    public async Task EjecutarAsync_CuandoRolesYaExisten_NoCreaDuplicados()
    {
        // Arrange
        var mockGestorUsuarios = AyudanteIdentity.CrearUserManagerMock();
        var mockGestorRoles = AyudanteIdentity.CrearRoleManagerMock();

        mockGestorRoles
            .Setup(r => r.RoleExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        mockGestorUsuarios
            .Setup(u => u.GetUsersInRoleAsync(SemillaAdminPorDefecto.NombreRolAdministrador))
            .ReturnsAsync(new List<IdentityUser> { new IdentityUser("admin@test.com") });

        var semilla = new SemillaAdminPorDefecto(
            mockGestorUsuarios.Object,
            mockGestorRoles.Object,
            CrearConfiguracion(),
            new Mock<ILogger<SemillaAdminPorDefecto>>().Object);

        // Act
        var resultado = await semilla.EjecutarAsync();

        // Assert
        Assert.True(resultado.EsExitoso);
        mockGestorRoles.Verify(r => r.CreateAsync(It.IsAny<IdentityRole>()), Times.Never);
        mockGestorUsuarios.Verify(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Never);
    }

    // ── EjecutarAsync — Admin ────────────────────────────────────────────────

    /// <summary>Debe crear el admin cuando no existe ningún administrador.</summary>
    [Fact]
    public async Task EjecutarAsync_SinAdministrador_CreaUsuarioAdmin()
    {
        // Arrange
        var mockGestorUsuarios = AyudanteIdentity.CrearUserManagerMock();
        var mockGestorRoles = AyudanteIdentity.CrearRoleManagerMock();

        mockGestorRoles
            .Setup(r => r.RoleExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        mockGestorUsuarios
            .Setup(u => u.GetUsersInRoleAsync(SemillaAdminPorDefecto.NombreRolAdministrador))
            .ReturnsAsync(new List<IdentityUser>());

        mockGestorUsuarios
            .Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        mockGestorUsuarios
            .Setup(u => u.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var semilla = new SemillaAdminPorDefecto(
            mockGestorUsuarios.Object,
            mockGestorRoles.Object,
            CrearConfiguracion(),
            new Mock<ILogger<SemillaAdminPorDefecto>>().Object);

        // Act
        var resultado = await semilla.EjecutarAsync();

        // Assert
        Assert.True(resultado.EsExitoso);
        mockGestorUsuarios.Verify(
            u => u.CreateAsync(It.IsAny<IdentityUser>(), "Admin@12345"),
            Times.Once);
        mockGestorUsuarios.Verify(
            u => u.AddToRoleAsync(It.IsAny<IdentityUser>(), SemillaAdminPorDefecto.NombreRolAdministrador),
            Times.Once);
    }

    /// <summary>Debe retornar fallo cuando la creación del admin falla en Identity.</summary>
    [Fact]
    public async Task EjecutarAsync_CuandoCreacionAdminFalla_RetornaFallo()
    {
        // Arrange
        var mockGestorUsuarios = AyudanteIdentity.CrearUserManagerMock();
        var mockGestorRoles = AyudanteIdentity.CrearRoleManagerMock();

        mockGestorRoles
            .Setup(r => r.RoleExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        mockGestorUsuarios
            .Setup(u => u.GetUsersInRoleAsync(SemillaAdminPorDefecto.NombreRolAdministrador))
            .ReturnsAsync(new List<IdentityUser>());

        var errorIdentity = IdentityResult.Failed(
            new IdentityError { Code = "WeakPassword", Description = "Contraseña muy débil." });

        mockGestorUsuarios
            .Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(errorIdentity);

        var semilla = new SemillaAdminPorDefecto(
            mockGestorUsuarios.Object,
            mockGestorRoles.Object,
            CrearConfiguracion(),
            new Mock<ILogger<SemillaAdminPorDefecto>>().Object);

        // Act
        var resultado = await semilla.EjecutarAsync();

        // Assert
        Assert.False(resultado.EsExitoso);
        Assert.NotNull(resultado.MensajeError);
    }

    /// <summary>Debe retornar fallo cuando las credenciales no están configuradas.</summary>
    [Fact]
    public async Task EjecutarAsync_SinCredencialesConfiguradas_RetornaFallo()
    {
        // Arrange
        var mockGestorUsuarios = AyudanteIdentity.CrearUserManagerMock();
        var mockGestorRoles = AyudanteIdentity.CrearRoleManagerMock();

        mockGestorRoles
            .Setup(r => r.RoleExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        mockGestorUsuarios
            .Setup(u => u.GetUsersInRoleAsync(SemillaAdminPorDefecto.NombreRolAdministrador))
            .ReturnsAsync(new List<IdentityUser>());

        // Configuración vacía — sin credenciales
        var configuracionVacia = new ConfigurationBuilder().Build();

        var semilla = new SemillaAdminPorDefecto(
            mockGestorUsuarios.Object,
            mockGestorRoles.Object,
            configuracionVacia,
            new Mock<ILogger<SemillaAdminPorDefecto>>().Object);

        // Act
        var resultado = await semilla.EjecutarAsync();

        // Assert
        Assert.False(resultado.EsExitoso);
        Assert.NotNull(resultado.MensajeError);
    }

    /// <summary>Debe retornar fallo cuando la creación del rol falla.</summary>
    [Fact]
    public async Task EjecutarAsync_CuandoCreacionRolFalla_RetornaFallo()
    {
        // Arrange
        var mockGestorUsuarios = AyudanteIdentity.CrearUserManagerMock();
        var mockGestorRoles = AyudanteIdentity.CrearRoleManagerMock();

        mockGestorRoles
            .Setup(r => r.RoleExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        mockGestorRoles
            .Setup(r => r.CreateAsync(It.IsAny<IdentityRole>()))
            .ReturnsAsync(IdentityResult.Failed(
                new IdentityError { Description = "Error al crear rol." }));

        var semilla = new SemillaAdminPorDefecto(
            mockGestorUsuarios.Object,
            mockGestorRoles.Object,
            CrearConfiguracion(),
            new Mock<ILogger<SemillaAdminPorDefecto>>().Object);

        // Act
        var resultado = await semilla.EjecutarAsync();

        // Assert
        Assert.False(resultado.EsExitoso);
    }
}
