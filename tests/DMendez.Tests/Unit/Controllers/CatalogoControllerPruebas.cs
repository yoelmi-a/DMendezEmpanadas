using DMendez.Data.Common;
using DMendez.Data.Entities;
using DMendez.Data.Repositories.Interfaces;
using DMendez.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DMendez.Tests.Unit.Controllers;

/// <summary>Pruebas unitarias para <see cref="CatalogoController"/>.</summary>
public class CatalogoControllerPruebas
{
    private static List<Producto> CrearProductos() =>
    [
        new() { Id = 1, Nombre = "Empanada A", Descripcion = "d", Precio = 100, Categoria = CategoriaProducto.Empanadas, EstaDisponible = true },
        new() { Id = 2, Nombre = "Bebida A",   Descripcion = "d", Precio = 50,  Categoria = CategoriaProducto.Bebidas,  EstaDisponible = true }
    ];

    // ── Index ────────────────────────────────────────────────────────────────

    /// <summary>Debe retornar ViewResult con productos cuando el repositorio responde correctamente.</summary>
    [Fact]
    public async Task Index_SinFiltro_RetornaViewConProductos()
    {
        // Arrange
        var mockRepo = new Mock<IRepositorioProducto>();
        mockRepo.Setup(r => r.ObtenerDisponiblesAsync())
            .ReturnsAsync(OperationResult<IEnumerable<Producto>>.Exitoso(CrearProductos()));

        var controller = new CatalogoController(mockRepo.Object);

        // Act
        var resultado = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(resultado);
        var modelo = Assert.IsAssignableFrom<IEnumerable<Producto>>(viewResult.Model);
        Assert.Equal(2, modelo.Count());
    }

    /// <summary>Debe filtrar por categoría cuando se provee parámetro.</summary>
    [Fact]
    public async Task Index_ConCategoria_RetornaViewConProductosFiltrados()
    {
        // Arrange
        var mockRepo = new Mock<IRepositorioProducto>();
        mockRepo.Setup(r => r.ObtenerPorCategoriaAsync(CategoriaProducto.Empanadas))
            .ReturnsAsync(OperationResult<IEnumerable<Producto>>.Exitoso(
                CrearProductos().Where(p => p.Categoria == CategoriaProducto.Empanadas)));

        var controller = new CatalogoController(mockRepo.Object);

        // Act
        var resultado = await controller.Index(CategoriaProducto.Empanadas);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(resultado);
        var modelo = Assert.IsAssignableFrom<IEnumerable<Producto>>(viewResult.Model);
        Assert.Single(modelo);
    }

    /// <summary>Debe retornar vista con lista vacía cuando el repositorio falla.</summary>
    [Fact]
    public async Task Index_CuandoRepositorioFalla_RetornaVistaVacia()
    {
        // Arrange
        var mockRepo = new Mock<IRepositorioProducto>();
        mockRepo.Setup(r => r.ObtenerDisponiblesAsync())
            .ReturnsAsync(OperationResult<IEnumerable<Producto>>.Fallido("Error BD."));

        var controller = new CatalogoController(mockRepo.Object);

        // Act
        var resultado = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(resultado);
        var modelo = Assert.IsAssignableFrom<IEnumerable<Producto>>(viewResult.Model);
        Assert.Empty(modelo);
    }

    // ── Detalle ──────────────────────────────────────────────────────────────

    /// <summary>Debe retornar ViewResult con el producto cuando existe y está disponible.</summary>
    [Fact]
    public async Task Detalle_ConIdExistenteYDisponible_RetornaView()
    {
        // Arrange
        var producto = new Producto
        {
            Id = 1, Nombre = "X", Descripcion = "d", Precio = 100,
            Categoria = CategoriaProducto.Empanadas, EstaDisponible = true
        };
        var mockRepo = new Mock<IRepositorioProducto>();
        mockRepo.Setup(r => r.ObtenerPorIdAsync(1))
            .ReturnsAsync(OperationResult<Producto>.Exitoso(producto));

        var controller = new CatalogoController(mockRepo.Object);

        // Act
        var resultado = await controller.Detalle(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(resultado);
        Assert.Equal(producto, viewResult.Model);
    }

    /// <summary>Debe retornar NotFound cuando el producto no existe.</summary>
    [Fact]
    public async Task Detalle_ConIdInexistente_RetornaNotFound()
    {
        // Arrange
        var mockRepo = new Mock<IRepositorioProducto>();
        mockRepo.Setup(r => r.ObtenerPorIdAsync(99))
            .ReturnsAsync(OperationResult<Producto>.Fallido("No encontrado."));

        var controller = new CatalogoController(mockRepo.Object);

        // Act
        var resultado = await controller.Detalle(99);

        // Assert
        Assert.IsType<NotFoundResult>(resultado);
    }

    /// <summary>Debe retornar NotFound cuando el producto existe pero no está disponible.</summary>
    [Fact]
    public async Task Detalle_ConProductoNoDisponible_RetornaNotFound()
    {
        // Arrange
        var producto = new Producto
        {
            Id = 5, Nombre = "Oculto", Descripcion = "d", Precio = 100,
            Categoria = CategoriaProducto.Empanadas, EstaDisponible = false
        };
        var mockRepo = new Mock<IRepositorioProducto>();
        mockRepo.Setup(r => r.ObtenerPorIdAsync(5))
            .ReturnsAsync(OperationResult<Producto>.Exitoso(producto));

        var controller = new CatalogoController(mockRepo.Object);

        // Act
        var resultado = await controller.Detalle(5);

        // Assert
        Assert.IsType<NotFoundResult>(resultado);
    }
}
