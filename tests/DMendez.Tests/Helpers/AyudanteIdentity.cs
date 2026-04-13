using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace DMendez.Tests.Helpers;

/// <summary>Fábrica de mocks de UserManager y RoleManager para pruebas unitarias.</summary>
public static class AyudanteIdentity
{
    /// <summary>Crea un mock de <see cref="UserManager{TUser}"/> con configuración mínima.</summary>
    /// <returns>Un mock funcional de UserManager.</returns>
    public static Mock<UserManager<IdentityUser>> CrearUserManagerMock()
    {
        var tienda = new Mock<IUserStore<IdentityUser>>();
        var opciones = new Mock<IOptions<IdentityOptions>>();
        opciones.Setup(o => o.Value).Returns(new IdentityOptions());

        var mock = new Mock<UserManager<IdentityUser>>(
            tienda.Object,
            opciones.Object,
            new Mock<IPasswordHasher<IdentityUser>>().Object,
            Array.Empty<IUserValidator<IdentityUser>>(),
            Array.Empty<IPasswordValidator<IdentityUser>>(),
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<IdentityUser>>>().Object);

        return mock;
    }

    /// <summary>Crea un mock de <see cref="RoleManager{TRole}"/> con configuración mínima.</summary>
    /// <returns>Un mock funcional de RoleManager.</returns>
    public static Mock<RoleManager<IdentityRole>> CrearRoleManagerMock()
    {
        var tienda = new Mock<IRoleStore<IdentityRole>>();

        var mock = new Mock<RoleManager<IdentityRole>>(
            tienda.Object,
            Array.Empty<IRoleValidator<IdentityRole>>(),
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<ILogger<RoleManager<IdentityRole>>>().Object);

        return mock;
    }
}
