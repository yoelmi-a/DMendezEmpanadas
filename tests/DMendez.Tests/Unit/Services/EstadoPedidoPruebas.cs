using DMendez.Data.Entities;
using Xunit;

namespace DMendez.Tests.Unit.Services;

/// <summary>Pruebas unitarias para la clase estática <see cref="EstadoPedido"/>.</summary>
public class EstadoPedidoPruebas
{
    // ── SiguienteEstado ──────────────────────────────────────────────────────

    /// <summary>Recibido debe transicionar a EnPreparacion.</summary>
    [Fact]
    public void SiguienteEstado_DesdeRecibido_RetornaEnPreparacion()
    {
        var siguiente = EstadoPedido.SiguienteEstado(EstadoPedido.Recibido);
        Assert.Equal(EstadoPedido.EnPreparacion, siguiente);
    }

    /// <summary>EnPreparacion debe transicionar a EnCamino.</summary>
    [Fact]
    public void SiguienteEstado_DesdeEnPreparacion_RetornaEnCamino()
    {
        var siguiente = EstadoPedido.SiguienteEstado(EstadoPedido.EnPreparacion);
        Assert.Equal(EstadoPedido.EnCamino, siguiente);
    }

    /// <summary>EnCamino debe transicionar a Entregado.</summary>
    [Fact]
    public void SiguienteEstado_DesdeEnCamino_RetornaEntregado()
    {
        var siguiente = EstadoPedido.SiguienteEstado(EstadoPedido.EnCamino);
        Assert.Equal(EstadoPedido.Entregado, siguiente);
    }

    /// <summary>Entregado no debe tener estado siguiente (retorna null).</summary>
    [Fact]
    public void SiguienteEstado_DesdeEntregado_RetornaNull()
    {
        var siguiente = EstadoPedido.SiguienteEstado(EstadoPedido.Entregado);
        Assert.Null(siguiente);
    }

    /// <summary>Un estado desconocido no debe tener transición válida.</summary>
    [Fact]
    public void SiguienteEstado_ConEstadoDesconocido_RetornaNull()
    {
        var siguiente = EstadoPedido.SiguienteEstado("EstadoInexistente");
        Assert.Null(siguiente);
    }
}
