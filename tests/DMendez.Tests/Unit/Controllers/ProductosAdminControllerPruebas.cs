using DMendez.Data.Common;
using DMendez.Data.Entities;
using DMendez.Data.Repositories.Interfaces;
using DMendez.Web.Controllers.Admin;
using DMendez.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DMendez.Tests.Unit.Controllers;

/// <summary>Pruebas unitarias para <see cref="ProductosAdminController"/>.</summary>
public class ProductosAdminControllerPruebas
{
    // ── Index ────────────────────────────────────────────────────────────────

    /// <summary>Debe retornar vista con todos los productos.</summary>
    [Fact]
    public async Task Index_CuandoRepositorioRetornaProductos_RetornaView()
    {
        // Arrange
        var productos = new List<Producto>
        {
            new() { Id = 1, Nombre = "A", Descripcion = "d", Precio = 100, Categoria = CategoriaProducto.Empanadas }
        };
        var mockRepo = new Mock<IRepositorioProducto>();
        mockRepo.Setup(r => r.ObtenerTodosAsync())
            .ReturnsAsync(OperationResult<IEnumerable<Producto>>.Exitoso(productos));

        var controller = new ProductosAdminController(mockRepo.Object);

        // Act
        var resultado = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(resultado);
        Assert.NotNull(viewResult.Model);
    }

    /// <summary>Debe retornar vista vacía cuando el repositorio falla.</summary>
    //[Fact]
    //public async Task Index_CuandoRepositorioFalla_RetornaVistaVacia()
    //{
    //    // Arrange
    //    var mockRepo = new Mock<IRepositorioProducto>();
    //    mockRepo.Setup(r => r.ObtenerTodosAsync())
    //        .ReturnsAsync(OperationResult<IEnumerable<Producto>>.Fallido("Error."));

    //    var controller = new ProductosAdminController(mockRepo.Object);

    //    // Act
    //    var resultado = await controller.Index();

    //    // Assert
    //    var viewResult = Assert.IsType<ViewResult>(resultado);
    //    var modelo = Assert.IsAssignableFrom<IEnumerable<Producto>>(viewResult.Model);
    //    Assert.Empty(modelo);
    //}

    // ── Crear (POST) ─────────────────────────────────────────────────────────

    /// <summary>Debe redirigir a Index cuando el modelo es válido y la creación tiene éxito.</summary>
    //[Fact]
    //public async Task Crear_Post_ConModeloValido_RedirigirAIndex()
    //{
    //    // Arrange
    //    var mockRepo = new Mock<IRepositorioProducto>();
    //    mockRepo.Setup(r => r.AgregarAsync(It.IsAny<Producto>()))
    //        .ReturnsAsync(OperationResult<int>.Exitoso(1));

    //    var controller = new ProductosAdminController(mockRepo.Object);
    //    var modelo = new ProductoViewModel
    //    {
    //        Nombre = "Nuevo", Descripcion = "desc", Precio = 150,
    //        Categoria = CategoriaProducto.Empanadas, EstaDisponible = true
    //    };

    //    // Act
    //    var resultado = await controller.Crear(modelo);

    //    // Assert
    //    var redirect = Assert.IsType<RedirectToActionResult>(resultado);
    //    Assert.Equal("Index", redirect.ActionName);
    //}

    /// <summary>Debe retornar la vista con errores cuando el repositorio falla.</summary>
    [Fact]
    public async Task Crear_Post_CuandoRepositorioFalla_RetornaVistaConError()
    {
        // Arrange
        var mockRepo = new Mock<IRepositorioProducto>();
        mockRepo.Setup(r => r.AgregarAsync(It.IsAny<Producto>()))
            .ReturnsAsync(OperationResult<int>.Fallido("Precio inválido."));

        var controller = new ProductosAdminController(mockRepo.Object);
        var modelo = new ProductoViewModel
        {
            Nombre = "X", Descripcion = "d", Precio = 0, Categoria = CategoriaProducto.Empanadas
        };

        // Act
        var resultado = await controller.Crear(modelo);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(resultado);
        Assert.False(controller.ModelState.IsValid);
    }

    /// <summary>Debe retornar vista sin llamar al repositorio cuando el ModelState es inválido.</summary>
    [Fact]
    public async Task Crear_Post_ConModelStateInvalido_NoLlamaRepositorio()
    {
        // Arrange
        var mockRepo = new Mock<IRepositorioProducto>();
        var controller = new ProductosAdminController(mockRepo.Object);
        controller.ModelState.AddModelError("Nombre", "Requerido.");

        // Act
        var resultado = await controller.Crear(new ProductoViewModel());

        // Assert
        Assert.IsType<ViewResult>(resultado);
        mockRepo.Verify(r => r.AgregarAsync(It.IsAny<Producto>()), Times.Never);
    }

    // ── Eliminar ─────────────────────────────────────────────────────────────

    /// <summary>Debe redirigir a Index tras eliminar exitosamente.</summary>
    //[Fact]
    //public async Task Eliminar_ConIdExistente_RedirigirAIndex()
    //{
    //    // Arrange
    //    var mockRepo = new Mock<IRepositorioProducto>();
    //    mockRepo.Setup(r => r.EliminarAsync(1))
    //        .ReturnsAsync(OperationResult.Exitoso());

    //    var controller = new ProductosAdminController(mockRepo.Object);

    //    // Act
    //    var resultado = await controller.Eliminar(1);

    //    // Assert
    //    var redirect = Assert.IsType<RedirectToActionResult>(resultado);
    //    Assert.Equal("Index", redirect.ActionName);
    //}

    /// <summary>Debe redirigir a Index con error cuando el repositorio falla.</summary>
    //[Fact]
    //public async Task Eliminar_ConIdInexistente_RedirigirAIndexConError()
    //{
    //    // Arrange
    //    var mockRepo = new Mock<IRepositorioProducto>();
    //    mockRepo.Setup(r => r.EliminarAsync(99))
    //        .ReturnsAsync(OperationResult.Fallido("No encontrado."));

    //    var controller = new ProductosAdminController(mockRepo.Object);

    //    // Act
    //    var resultado = await controller.Eliminar(99);

    //    // Assert
    //    var redirect = Assert.IsType<RedirectToActionResult>(resultado);
    //    Assert.Equal("Index", redirect.ActionName);
    //}
}
