using DMendez.Data.Entities;
using DMendez.Data.Repositories;
using DMendez.Tests.Helpers;
using Xunit;

namespace DMendez.Tests.Unit.Repositories;

/// <summary>Pruebas unitarias para <see cref="RepositorioPromocion"/>.</summary>
public class RepositorioPromocionPruebas
{
    private static Promocion CrearPromocionValida(string codigo = "PROMO10") =>
        new()
        {
            Nombre = "Promoción de prueba",
            Codigo = codigo,
            TipoDescuento = TipoDescuento.Porcentaje,
            ValorDescuento = 10,
            FechaInicio = DateTime.UtcNow.AddDays(-1),
            FechaFin = DateTime.UtcNow.AddDays(30),
            EstaActiva = true
        };

    // ── CrearAsync ───────────────────────────────────────────────────────────

    /// <summary>Debe retornar Id cuando la promoción es válida.</summary>
    [Fact]
    public async Task CrearAsync_ConDatosValidos_RetornaId()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioPromocion(contexto);

        // Act
        var resultado = await repo.CrearAsync(CrearPromocionValida());

        // Assert
        Assert.True(resultado.EsExitoso);
        Assert.True(resultado.Valor > 0);
    }

    /// <summary>Debe retornar fallo cuando el código ya existe.</summary>
    [Fact]
    public async Task CrearAsync_ConCodigoDuplicado_RetornaFallo()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioPromocion(contexto);
        await repo.CrearAsync(CrearPromocionValida("UNICO"));

        // Act
        var resultado = await repo.CrearAsync(CrearPromocionValida("UNICO"));

        // Assert
        Assert.False(resultado.EsExitoso);
        Assert.NotNull(resultado.MensajeError);
    }

    /// <summary>Debe retornar fallo cuando la fecha fin es anterior a la fecha inicio.</summary>
    [Fact]
    public async Task CrearAsync_ConFechasInvertidas_RetornaFallo()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioPromocion(contexto);
        var promo = CrearPromocionValida();
        promo.FechaInicio = DateTime.UtcNow.AddDays(10);
        promo.FechaFin = DateTime.UtcNow.AddDays(1);

        // Act
        var resultado = await repo.CrearAsync(promo);

        // Assert
        Assert.False(resultado.EsExitoso);
    }

    /// <summary>Debe retornar fallo cuando el valor de descuento es cero.</summary>
    [Fact]
    public async Task CrearAsync_ConValorCero_RetornaFallo()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioPromocion(contexto);
        var promo = CrearPromocionValida();
        promo.ValorDescuento = 0;

        // Act
        var resultado = await repo.CrearAsync(promo);

        // Assert
        Assert.False(resultado.EsExitoso);
    }

    // ── ObtenerPorCodigoValidoAsync ──────────────────────────────────────────

    /// <summary>Debe retornar la promoción cuando el código es válido y activo.</summary>
    [Fact]
    public async Task ObtenerPorCodigoValidoAsync_ConCodigoActivo_RetornaPromocion()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioPromocion(contexto);
        await repo.CrearAsync(CrearPromocionValida("VALID20"));

        // Act
        var resultado = await repo.ObtenerPorCodigoValidoAsync("VALID20");

        // Assert
        Assert.True(resultado.EsExitoso);
        Assert.Equal("VALID20", resultado.Valor!.Codigo);
    }

    /// <summary>Debe retornar fallo cuando el código no existe.</summary>
    [Fact]
    public async Task ObtenerPorCodigoValidoAsync_ConCodigoInexistente_RetornaFallo()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioPromocion(contexto);

        // Act
        var resultado = await repo.ObtenerPorCodigoValidoAsync("NOEXISTE");

        // Assert
        Assert.False(resultado.EsExitoso);
    }

    /// <summary>Debe retornar fallo cuando la promoción está inactiva.</summary>
    [Fact]
    public async Task ObtenerPorCodigoValidoAsync_ConPromocionInactiva_RetornaFallo()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioPromocion(contexto);
        var promo = CrearPromocionValida("INACT");
        promo.EstaActiva = false;
        await repo.CrearAsync(promo);

        // Act
        var resultado = await repo.ObtenerPorCodigoValidoAsync("INACT");

        // Assert
        Assert.False(resultado.EsExitoso);
    }

    /// <summary>Debe retornar fallo cuando la promoción está expirada.</summary>
    [Fact]
    public async Task ObtenerPorCodigoValidoAsync_ConPromocionExpirada_RetornaFallo()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioPromocion(contexto);
        var promo = CrearPromocionValida("EXPIRADA");
        promo.FechaInicio = DateTime.UtcNow.AddDays(-30);
        promo.FechaFin = DateTime.UtcNow.AddDays(-1);
        await repo.CrearAsync(promo);

        // Act
        var resultado = await repo.ObtenerPorCodigoValidoAsync("EXPIRADA");

        // Assert
        Assert.False(resultado.EsExitoso);
    }

    // ── CambiarEstadoAsync ───────────────────────────────────────────────────

    /// <summary>Debe cambiar el estado cuando la promoción existe.</summary>
    [Fact]
    public async Task CambiarEstadoAsync_ConIdExistente_ActualizaEstado()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioPromocion(contexto);
        var idResultado = await repo.CrearAsync(CrearPromocionValida("TOGGLE"));

        // Act
        var resultado = await repo.CambiarEstadoAsync(idResultado.Valor, false);

        // Assert
        Assert.True(resultado.EsExitoso);
        var promo = await contexto.Promociones.FindAsync(idResultado.Valor);
        Assert.False(promo!.EstaActiva);
    }

    /// <summary>Debe retornar fallo cuando la promoción no existe.</summary>
    [Fact]
    public async Task CambiarEstadoAsync_ConIdInexistente_RetornaFallo()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioPromocion(contexto);

        // Act
        var resultado = await repo.CambiarEstadoAsync(999, true);

        // Assert
        Assert.False(resultado.EsExitoso);
    }

    // ── ActualizarAsync ──────────────────────────────────────────────────────

    /// <summary>Debe actualizar la promoción cuando existe.</summary>
    [Fact]
    public async Task ActualizarAsync_ConPromocionExistente_RetornaExito()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioPromocion(contexto);
        var idResultado = await repo.CrearAsync(CrearPromocionValida("UPD"));

        var actualizada = CrearPromocionValida("UPD");
        actualizada.Id = idResultado.Valor;
        actualizada.Nombre = "Nombre actualizado";
        actualizada.ValorDescuento = 20;

        // Act
        var resultado = await repo.ActualizarAsync(actualizada);

        // Assert
        Assert.True(resultado.EsExitoso);
        var verificacion = await contexto.Promociones.FindAsync(idResultado.Valor);
        Assert.Equal("Nombre actualizado", verificacion!.Nombre);
    }

    /// <summary>Debe retornar fallo cuando la promoción a actualizar no existe.</summary>
    [Fact]
    public async Task ActualizarAsync_ConIdInexistente_RetornaFallo()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioPromocion(contexto);
        var promo = CrearPromocionValida();
        promo.Id = 999;

        // Act
        var resultado = await repo.ActualizarAsync(promo);

        // Assert
        Assert.False(resultado.EsExitoso);
    }
}
