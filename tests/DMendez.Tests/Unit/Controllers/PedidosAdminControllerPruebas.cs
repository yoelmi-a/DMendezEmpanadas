using DMendez.Data.Common;
using DMendez.Data.Entities;
using DMendez.Data.Repositories.Interfaces;
using DMendez.Web.Controllers.Admin;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DMendez.Tests.Unit.Controllers;

/// <summary>Pruebas unitarias para <see cref="PedidosAdminController"/>.</summary>
public class PedidosAdminControllerPruebas
{
    private static Pedido CrearPedidoEjemplo(int id = 1) =>
        new()
        {
            Id = id,
            IdCliente = "cli-1",
            Estado = EstadoPedido.Recibido,
            Subtotal = 300,
            Impuestos = 54,
            Total = 354,
            Items = [new ItemPedido { IdProducto = 1, Cantidad = 2, PrecioUnitario = 150 }],
            HistorialEstados = [new HistorialEstado { Estado = EstadoPedido.Recibido, FechaHora = DateTime.UtcNow }]
        };

    // ── Index ────────────────────────────────────────────────────────────────

    /// <summary>Debe retornar vista con todos los pedidos sin filtro.</summary>
    [Fact]
    public async Task Index_SinFiltro_RetornaViewConPedidos()
    {
        // Arrange
        var mockRepo = new Mock<IRepositorioPedido>();
        mockRepo.Setup(r => r.ObtenerTodosAsync())
            .ReturnsAsync(OperationResult<IEnumerable<Pedido>>.Exitoso(
                new List<Pedido> { CrearPedidoEjemplo() }));

        var controller = new PedidosAdminController(mockRepo.Object);

        // Act
        var resultado = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(resultado);
        Assert.NotNull(viewResult.Model);
    }

    /// <summary>Debe filtrar por estado cuando se provee parámetro.</summary>
    [Fact]
    public async Task Index_ConFiltroEstado_LlamaObtenerPorEstado()
    {
        // Arrange
        var mockRepo = new Mock<IRepositorioPedido>();
        mockRepo.Setup(r => r.ObtenerPorEstadoAsync(EstadoPedido.Recibido))
            .ReturnsAsync(OperationResult<IEnumerable<Pedido>>.Exitoso(
                new List<Pedido> { CrearPedidoEjemplo() }));

        var controller = new PedidosAdminController(mockRepo.Object);

        // Act
        var resultado = await controller.Index(EstadoPedido.Recibido);

        // Assert
        mockRepo.Verify(r => r.ObtenerPorEstadoAsync(EstadoPedido.Recibido), Times.Once);
        Assert.IsType<ViewResult>(resultado);
    }

    // ── Detalle ──────────────────────────────────────────────────────────────

    /// <summary>Debe retornar vista con detalle cuando el pedido existe.</summary>
    [Fact]
    public async Task Detalle_ConIdExistente_RetornaView()
    {
        // Arrange
        var mockRepo = new Mock<IRepositorioPedido>();
        mockRepo.Setup(r => r.ObtenerPorIdAsync(1))
            .ReturnsAsync(OperationResult<Pedido>.Exitoso(CrearPedidoEjemplo()));

        var controller = new PedidosAdminController(mockRepo.Object);

        // Act
        var resultado = await controller.Detalle(1);

        // Assert
        Assert.IsType<ViewResult>(resultado);
    }

    /// <summary>Debe retornar NotFound cuando el pedido no existe.</summary>
    [Fact]
    public async Task Detalle_ConIdInexistente_RetornaNotFound()
    {
        // Arrange
        var mockRepo = new Mock<IRepositorioPedido>();
        mockRepo.Setup(r => r.ObtenerPorIdAsync(99))
            .ReturnsAsync(OperationResult<Pedido>.Fallido("No encontrado."));

        var controller = new PedidosAdminController(mockRepo.Object);

        // Act
        var resultado = await controller.Detalle(99);

        // Assert
        Assert.IsType<NotFoundResult>(resultado);
    }

    // ── AvanzarEstado ────────────────────────────────────────────────────────

    /// <summary>Debe redirigir a Detalle con mensaje de éxito cuando la transición es válida.</summary>
    [Fact]
    public async Task AvanzarEstado_ConTransicionValida_RedirigirADetalle()
    {
        // Arrange
        var mockRepo = new Mock<IRepositorioPedido>();
        mockRepo.Setup(r => r.AvanzarEstadoAsync(1))
            .ReturnsAsync(OperationResult<string>.Exitoso(EstadoPedido.EnPreparacion));

        var controller = new PedidosAdminController(mockRepo.Object);

        // Act
        var resultado = await controller.AvanzarEstado(1);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(resultado);
        Assert.Equal("Detalle", redirect.ActionName);
    }

    /// <summary>Debe redirigir a Detalle con error cuando la transición falla.</summary>
    [Fact]
    public async Task AvanzarEstado_ConPedidoEntregado_RedirigirADetalleConError()
    {
        // Arrange
        var mockRepo = new Mock<IRepositorioPedido>();
        mockRepo.Setup(r => r.AvanzarEstadoAsync(1))
            .ReturnsAsync(OperationResult<string>.Fallido("Ya entregado."));

        var controller = new PedidosAdminController(mockRepo.Object);

        // Act
        var resultado = await controller.AvanzarEstado(1);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(resultado);
        Assert.Equal("Detalle", redirect.ActionName);
    }
}
