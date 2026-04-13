using DMendez.Data.Entities;
using DMendez.Data.Repositories;
using DMendez.Tests.Helpers;
using Xunit;

namespace DMendez.Tests.Unit.Repositories;

/// <summary>Pruebas unitarias para <see cref="RepositorioPedido"/>.</summary>
public class RepositorioPedidoPruebas
{
    // ── Fixture de datos ─────────────────────────────────────────────────────

    private static Pedido CrearPedidoValido(string idCliente = "cliente-1") =>
        new()
        {
            IdCliente = idCliente,
            MetodoEntrega = MetodoEntrega.Recogida,
            Subtotal = 300,
            Impuestos = 54,
            Total = 354,
            Items =
            [
                new ItemPedido { IdProducto = 1, Cantidad = 2, PrecioUnitario = 150 }
            ]
        };

    // ── CrearAsync ───────────────────────────────────────────────────────────

    /// <summary>Debe crear pedido con estado Recibido y retornar Id.</summary>
    [Fact]
    public async Task CrearAsync_ConDatosValidos_RetornaIdYEstadoRecibido()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioPedido(contexto);

        // Act
        var resultado = await repo.CrearAsync(CrearPedidoValido());

        // Assert
        Assert.True(resultado.EsExitoso);
        Assert.True(resultado.Valor > 0);

        var pedido = await contexto.Pedidos.FindAsync(resultado.Valor);
        Assert.Equal(EstadoPedido.Recibido, pedido!.Estado);
    }

    /// <summary>Debe crear entrada en historial con estado Recibido al crear pedido.</summary>
    [Fact]
    public async Task CrearAsync_ConDatosValidos_AgregaEntradaHistorial()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioPedido(contexto);

        // Act
        var resultado = await repo.CrearAsync(CrearPedidoValido());

        // Assert
        Assert.True(resultado.EsExitoso);
        bool tieneHistorial = contexto.HistorialesEstado
            .Any(h => h.IdPedido == resultado.Valor && h.Estado == EstadoPedido.Recibido);
        Assert.True(tieneHistorial);
    }

    /// <summary>Debe retornar fallo cuando el pedido no tiene ítems.</summary>
    [Fact]
    public async Task CrearAsync_SinItems_RetornaFallo()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioPedido(contexto);
        var pedidoVacio = new Pedido { IdCliente = "c1", Items = [] };

        // Act
        var resultado = await repo.CrearAsync(pedidoVacio);

        // Assert
        Assert.False(resultado.EsExitoso);
        Assert.NotNull(resultado.MensajeError);
    }

    // ── ObtenerPorIdAsync ────────────────────────────────────────────────────

    /// <summary>Debe retornar el pedido cuando el Id existe.</summary>
    [Fact]
    public async Task ObtenerPorIdAsync_ConIdExistente_RetornaPedido()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioPedido(contexto);
        var creacion = await repo.CrearAsync(CrearPedidoValido());

        // Act
        var resultado = await repo.ObtenerPorIdAsync(creacion.Valor);

        // Assert
        Assert.True(resultado.EsExitoso);
        Assert.Equal(creacion.Valor, resultado.Valor!.Id);
    }

    /// <summary>Debe retornar fallo cuando el pedido no existe.</summary>
    [Fact]
    public async Task ObtenerPorIdAsync_ConIdInexistente_RetornaFallo()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioPedido(contexto);

        // Act
        var resultado = await repo.ObtenerPorIdAsync(9999);

        // Assert
        Assert.False(resultado.EsExitoso);
    }

    // ── ObtenerPorClienteAsync ───────────────────────────────────────────────

    /// <summary>Debe retornar solo los pedidos del cliente especificado.</summary>
    [Fact]
    public async Task ObtenerPorClienteAsync_ConPedidosDeVariosClientes_RetornaSoloDelCliente()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioPedido(contexto);
        await repo.CrearAsync(CrearPedidoValido("cliente-A"));
        await repo.CrearAsync(CrearPedidoValido("cliente-A"));
        await repo.CrearAsync(CrearPedidoValido("cliente-B"));

        // Act
        var resultado = await repo.ObtenerPorClienteAsync("cliente-A");

        // Assert
        Assert.True(resultado.EsExitoso);
        Assert.Equal(2, resultado.Valor!.Count());
        Assert.All(resultado.Valor!, p => Assert.Equal("cliente-A", p.IdCliente));
    }

    /// <summary>Debe retornar lista vacía si el cliente no tiene pedidos.</summary>
    [Fact]
    public async Task ObtenerPorClienteAsync_SinPedidos_RetornaListaVacia()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioPedido(contexto);

        // Act
        var resultado = await repo.ObtenerPorClienteAsync("cliente-sin-pedidos");

        // Assert
        Assert.True(resultado.EsExitoso);
        Assert.Empty(resultado.Valor!);
    }

    // ── AvanzarEstadoAsync ───────────────────────────────────────────────────

    /// <summary>Debe avanzar de Recibido a EnPreparacion.</summary>
    [Fact]
    public async Task AvanzarEstadoAsync_DesdeRecibido_AvanzaAEnPreparacion()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioPedido(contexto);
        var creacion = await repo.CrearAsync(CrearPedidoValido());

        // Act
        var resultado = await repo.AvanzarEstadoAsync(creacion.Valor);

        // Assert
        Assert.True(resultado.EsExitoso);
        Assert.Equal(EstadoPedido.EnPreparacion, resultado.Valor);
    }

    /// <summary>Debe retornar fallo si el pedido ya está Entregado.</summary>
    [Fact]
    public async Task AvanzarEstadoAsync_DesdeEntregado_RetornaFallo()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioPedido(contexto);
        var creacion = await repo.CrearAsync(CrearPedidoValido());

        // Avanzar hasta Entregado
        await repo.AvanzarEstadoAsync(creacion.Valor); // -> EnPreparacion
        await repo.AvanzarEstadoAsync(creacion.Valor); // -> EnCamino
        await repo.AvanzarEstadoAsync(creacion.Valor); // -> Entregado

        // Act
        var resultado = await repo.AvanzarEstadoAsync(creacion.Valor);

        // Assert
        Assert.False(resultado.EsExitoso);
    }

    /// <summary>Debe retornar fallo si el pedido no existe.</summary>
    [Fact]
    public async Task AvanzarEstadoAsync_ConIdInexistente_RetornaFallo()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioPedido(contexto);

        // Act
        var resultado = await repo.AvanzarEstadoAsync(999);

        // Assert
        Assert.False(resultado.EsExitoso);
    }

    // ── CancelarAsync ────────────────────────────────────────────────────────

    /// <summary>Debe cancelar el pedido cuando está en Recibido y pertenece al cliente.</summary>
    [Fact]
    public async Task CancelarAsync_ConPedidoRecibido_RetornaExito()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioPedido(contexto);
        var creacion = await repo.CrearAsync(CrearPedidoValido("cli-1"));

        // Act
        var resultado = await repo.CancelarAsync(creacion.Valor, "cli-1");

        // Assert
        Assert.True(resultado.EsExitoso);
        var pedido = await repo.ObtenerPorIdAsync(creacion.Valor);
        Assert.Equal(EstadoPedido.Cancelado, pedido.Valor!.Estado);
    }

    /// <summary>Debe retornar fallo si el pedido no está en estado Recibido.</summary>
    [Fact]
    public async Task CancelarAsync_ConPedidoEnPreparacion_RetornaFallo()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioPedido(contexto);
        var creacion = await repo.CrearAsync(CrearPedidoValido("cli-1"));
        await repo.AvanzarEstadoAsync(creacion.Valor); // -> EnPreparacion

        // Act
        var resultado = await repo.CancelarAsync(creacion.Valor, "cli-1");

        // Assert
        Assert.False(resultado.EsExitoso);
        Assert.NotNull(resultado.MensajeError);
    }

    /// <summary>Debe retornar fallo si el cliente no es el dueño del pedido.</summary>
    [Fact]
    public async Task CancelarAsync_ConClienteDistinto_RetornaFallo()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioPedido(contexto);
        var creacion = await repo.CrearAsync(CrearPedidoValido("propietario"));

        // Act
        var resultado = await repo.CancelarAsync(creacion.Valor, "intruso");

        // Assert
        Assert.False(resultado.EsExitoso);
    }

    // ── ObtenerParaReporteAsync ──────────────────────────────────────────────

    /// <summary>Debe retornar solo pedidos Entregados dentro del rango de fechas.</summary>
    [Fact]
    public async Task ObtenerParaReporteAsync_ConPedidosEntregados_RetornaSoloEnRango()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioPedido(contexto);
        var creacion = await repo.CrearAsync(CrearPedidoValido());

        // Avanzar hasta Entregado
        await repo.AvanzarEstadoAsync(creacion.Valor);
        await repo.AvanzarEstadoAsync(creacion.Valor);
        await repo.AvanzarEstadoAsync(creacion.Valor);

        // Act
        var resultado = await repo.ObtenerParaReporteAsync(
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddDays(1));

        // Assert
        Assert.True(resultado.EsExitoso);
        Assert.Single(resultado.Valor!);
    }

    /// <summary>Debe retornar lista vacía si no hay pedidos entregados en el rango.</summary>
    [Fact]
    public async Task ObtenerParaReporteAsync_SinPedidosEnRango_RetornaListaVacia()
    {
        // Arrange
        await using var contexto = AyudanteDbEnMemoria.CrearContexto();
        var repo = new RepositorioPedido(contexto);

        // Act — rango en el pasado
        var resultado = await repo.ObtenerParaReporteAsync(
            DateTime.UtcNow.AddDays(-30),
            DateTime.UtcNow.AddDays(-20));

        // Assert
        Assert.True(resultado.EsExitoso);
        Assert.Empty(resultado.Valor!);
    }
}
