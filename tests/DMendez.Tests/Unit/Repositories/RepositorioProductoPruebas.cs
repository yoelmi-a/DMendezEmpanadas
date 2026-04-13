using DMendez.Data.Entities;
using DMendez.Data.Repositories;
using DMendez.Tests.Helpers;
using Xunit;

namespace DMendez.Tests.Unit.Repositories;

/// <summary>Pruebas unitarias para <see cref="RepositorioProducto"/>.</summary>
public class RepositorioProductoPruebas
{
    // ── ObtenerPorIdAsync ────────────────────────────────────────────────────

    /// <summary>Debe retornar el producto cuando el Id existe.</summary>
    [Fact]
    public async Task ObtenerPorIdAsync_ConIdExistente_RetornaProducto()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        contexto.Productos.Add(new Producto
        {
            Id = 1, Nombre = "Empanada de Pollo", Descripcion = "Crujiente", Precio = 150,
            Categoria = CategoriaProducto.Empanadas
        });
        await contexto.SaveChangesAsync();
        var repo = new RepositorioProducto(contexto);

        // Act
        var resultado = await repo.ObtenerPorIdAsync(1);

        // Assert
        Assert.True(resultado.EsExitoso);
        Assert.NotNull(resultado.Valor);
        Assert.Equal("Empanada de Pollo", resultado.Valor!.Nombre);
    }

    /// <summary>Debe retornar fallo cuando el Id no existe.</summary>
    [Fact]
    public async Task ObtenerPorIdAsync_ConIdInexistente_RetornaFallo()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioProducto(contexto);

        // Act
        var resultado = await repo.ObtenerPorIdAsync(99);

        // Assert
        Assert.False(resultado.EsExitoso);
        Assert.NotNull(resultado.MensajeError);
    }

    // ── ObtenerDisponiblesAsync ──────────────────────────────────────────────

    /// <summary>Debe retornar solo los productos con EstaDisponible true.</summary>
    [Fact]
    public async Task ObtenerDisponiblesAsync_ConProductosMixtos_RetornaSoloDisponibles()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        contexto.Productos.AddRange(
            new Producto { Nombre = "A", Descripcion = "d", Precio = 100, Categoria = CategoriaProducto.Empanadas, EstaDisponible = true },
            new Producto { Nombre = "B", Descripcion = "d", Precio = 100, Categoria = CategoriaProducto.Bebidas,  EstaDisponible = false });
        await contexto.SaveChangesAsync();
        var repo = new RepositorioProducto(contexto);

        // Act
        var resultado = await repo.ObtenerDisponiblesAsync();

        // Assert
        Assert.True(resultado.EsExitoso);
        Assert.Single(resultado.Valor!);
        Assert.All(resultado.Valor!, p => Assert.True(p.EstaDisponible));
    }

    /// <summary>Debe retornar lista vacía cuando no hay productos disponibles.</summary>
    [Fact]
    public async Task ObtenerDisponiblesAsync_SinProductosDisponibles_RetornaListaVacia()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioProducto(contexto);

        // Act
        var resultado = await repo.ObtenerDisponiblesAsync();

        // Assert
        Assert.True(resultado.EsExitoso);
        Assert.Empty(resultado.Valor!);
    }

    // ── AgregarAsync ─────────────────────────────────────────────────────────

    /// <summary>Debe asignar Id y retornar éxito cuando el producto es válido.</summary>
    [Fact]
    public async Task AgregarAsync_ConProductoValido_RetornaIdAsignado()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioProducto(contexto);
        var producto = new Producto
        {
            Nombre = "Empanada Carne", Descripcion = "Rellena", Precio = 180,
            Categoria = CategoriaProducto.Empanadas
        };

        // Act
        var resultado = await repo.AgregarAsync(producto);

        // Assert
        Assert.True(resultado.EsExitoso);
        Assert.True(resultado.Valor > 0);
    }

    /// <summary>Debe retornar fallo cuando el nombre está vacío.</summary>
    [Fact]
    public async Task AgregarAsync_ConNombreVacio_RetornaFallo()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioProducto(contexto);
        var producto = new Producto { Nombre = "", Descripcion = "d", Precio = 100, Categoria = CategoriaProducto.Empanadas };

        // Act
        var resultado = await repo.AgregarAsync(producto);

        // Assert
        Assert.False(resultado.EsExitoso);
        Assert.NotNull(resultado.MensajeError);
    }

    /// <summary>Debe retornar fallo cuando el precio es cero.</summary>
    [Fact]
    public async Task AgregarAsync_ConPrecioCero_RetornaFallo()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioProducto(contexto);
        var producto = new Producto { Nombre = "Válido", Descripcion = "d", Precio = 0, Categoria = CategoriaProducto.Empanadas };

        // Act
        var resultado = await repo.AgregarAsync(producto);

        // Assert
        Assert.False(resultado.EsExitoso);
    }

    // ── ActualizarAsync ──────────────────────────────────────────────────────

    /// <summary>Debe actualizar correctamente un producto existente.</summary>
    [Fact]
    public async Task ActualizarAsync_ConProductoExistente_RetornaExito()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        contexto.Productos.Add(new Producto
        {
            Id = 1, Nombre = "Original", Descripcion = "d", Precio = 100, Categoria = CategoriaProducto.Empanadas
        });
        await contexto.SaveChangesAsync();
        var repo = new RepositorioProducto(contexto);

        // Act
        var resultado = await repo.ActualizarAsync(new Producto
        {
            Id = 1, Nombre = "Actualizado", Descripcion = "nueva desc", Precio = 200,
            Categoria = CategoriaProducto.Combos
        });

        // Assert
        Assert.True(resultado.EsExitoso);
        var verificacion = await contexto.Productos.FindAsync(1);
        Assert.Equal("Actualizado", verificacion!.Nombre);
    }

    /// <summary>Debe retornar fallo cuando el producto a actualizar no existe.</summary>
    [Fact]
    public async Task ActualizarAsync_ConProductoInexistente_RetornaFallo()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioProducto(contexto);

        // Act
        var resultado = await repo.ActualizarAsync(new Producto
        {
            Id = 999, Nombre = "X", Descripcion = "d", Precio = 100, Categoria = CategoriaProducto.Empanadas
        });

        // Assert
        Assert.False(resultado.EsExitoso);
    }

    // ── EliminarAsync ────────────────────────────────────────────────────────

    /// <summary>Debe eliminar el producto cuando existe.</summary>
    [Fact]
    public async Task EliminarAsync_ConIdExistente_EliminaProducto()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        contexto.Productos.Add(new Producto
        {
            Id = 1, Nombre = "A Eliminar", Descripcion = "d", Precio = 100, Categoria = CategoriaProducto.Empanadas
        });
        await contexto.SaveChangesAsync();
        var repo = new RepositorioProducto(contexto);

        // Act
        var resultado = await repo.EliminarAsync(1);

        // Assert
        Assert.True(resultado.EsExitoso);
        Assert.Null(await contexto.Productos.FindAsync(1));
    }

    /// <summary>Debe retornar fallo cuando el producto a eliminar no existe.</summary>
    [Fact]
    public async Task EliminarAsync_ConIdInexistente_RetornaFallo()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioProducto(contexto);

        // Act
        var resultado = await repo.EliminarAsync(55);

        // Assert
        Assert.False(resultado.EsExitoso);
    }

    // ── CambiarDisponibilidadAsync ───────────────────────────────────────────

    /// <summary>Debe cambiar la disponibilidad cuando el producto existe.</summary>
    [Fact]
    public async Task CambiarDisponibilidadAsync_ConIdExistente_ActualizaDisponibilidad()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        contexto.Productos.Add(new Producto
        {
            Id = 1, Nombre = "P", Descripcion = "d", Precio = 100,
            Categoria = CategoriaProducto.Empanadas, EstaDisponible = true
        });
        await contexto.SaveChangesAsync();
        var repo = new RepositorioProducto(contexto);

        // Act
        var resultado = await repo.CambiarDisponibilidadAsync(1, false);

        // Assert
        Assert.True(resultado.EsExitoso);
        var p = await contexto.Productos.FindAsync(1);
        Assert.False(p!.EstaDisponible);
    }

    /// <summary>Debe retornar fallo cuando el producto no existe.</summary>
    [Fact]
    public async Task CambiarDisponibilidadAsync_ConIdInexistente_RetornaFallo()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioProducto(contexto);

        // Act
        var resultado = await repo.CambiarDisponibilidadAsync(99, true);

        // Assert
        Assert.False(resultado.EsExitoso);
    }
}
