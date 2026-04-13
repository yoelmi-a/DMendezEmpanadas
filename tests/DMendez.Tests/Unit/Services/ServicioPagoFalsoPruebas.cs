using DMendez.Data.Services;
using DMendez.Data.Services.Interfaces;
using Xunit;

namespace DMendez.Tests.Unit.Services;

/// <summary>Pruebas unitarias para <see cref="ServicioPagoFalso"/>.</summary>
public class ServicioPagoFalsoPruebas
{
    // ── ProcesarAsync ────────────────────────────────────────────────────────

    /// <summary>Debe retornar una referencia GUID no vacía cuando SimularFallo es false.</summary>
    [Fact]
    public async Task ProcesarAsync_SinSimularFallo_RetornaReferenciaExitosa()
    {
        // Arrange
        var servicio = new ServicioPagoFalso();
        var solicitud = new SolicitudPago
        {
            Monto = 500,
            SimularFallo = false,
            ReferenciaPedido = "PEDIDO-001"
        };

        // Act
        var resultado = await servicio.ProcesarAsync(solicitud);

        // Assert
        Assert.True(resultado.EsExitoso);
        Assert.False(string.IsNullOrWhiteSpace(resultado.Valor));
        Assert.True(Guid.TryParse(resultado.Valor, out _), "La referencia debe ser un GUID válido.");
    }

    /// <summary>Debe retornar fallo con mensaje cuando SimularFallo es true.</summary>
    [Fact]
    public async Task ProcesarAsync_SimulandoFallo_RetornaFalloConMensaje()
    {
        // Arrange
        var servicio = new ServicioPagoFalso();
        var solicitud = new SolicitudPago
        {
            Monto = 500,
            SimularFallo = true,
            ReferenciaPedido = "PEDIDO-001"
        };

        // Act
        var resultado = await servicio.ProcesarAsync(solicitud);

        // Assert
        Assert.False(resultado.EsExitoso);
        Assert.NotNull(resultado.MensajeError);
        Assert.False(string.IsNullOrWhiteSpace(resultado.MensajeError));
    }

    /// <summary>Cada pago exitoso debe generar una referencia única.</summary>
    [Fact]
    public async Task ProcesarAsync_DosLlamadasExitosas_GeneraReferenciasDistintas()
    {
        // Arrange
        var servicio = new ServicioPagoFalso();
        var solicitud = new SolicitudPago { Monto = 100, SimularFallo = false };

        // Act
        var resultado1 = await servicio.ProcesarAsync(solicitud);
        var resultado2 = await servicio.ProcesarAsync(solicitud);

        // Assert
        Assert.True(resultado1.EsExitoso);
        Assert.True(resultado2.EsExitoso);
        Assert.NotEqual(resultado1.Valor, resultado2.Valor);
    }
}
