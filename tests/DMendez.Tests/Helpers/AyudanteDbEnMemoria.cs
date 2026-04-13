using DMendez.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DMendez.Tests.Helpers;

/// <summary>Factoría de contextos InMemory para pruebas completamente aisladas.</summary>
public static class AyudanteDbEnMemoria
{
    /// <summary>
    /// Crea un nuevo <see cref="AppDbContext"/> respaldado por una base de datos InMemory única.
    /// Cada llamada sin nombre produce una base de datos aislada, garantizando que las pruebas
    /// no interfieran entre sí.
    /// </summary>
    /// <param name="nombreBd">
    /// Nombre opcional de la base de datos. Si se omite, se genera un GUID único.
    /// </param>
    /// <returns>Un <see cref="AppDbContext"/> listo para usar en pruebas.</returns>
    public static AppDbContext CrearContexto(string nombreBd = "")
    {
        string nombre = string.IsNullOrEmpty(nombreBd)
            ? Guid.NewGuid().ToString()
            : nombreBd;

        var opciones = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(nombre)
            .Options;

        return new AppDbContext(opciones);
    }
}
