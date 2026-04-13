using DMendez.Data.Common;
using Xunit;

namespace DMendez.Tests.Unit.Services;

/// <summary>Pruebas unitarias para los tipos <see cref="OperationResult{T}"/> y <see cref="OperationResult"/>.</summary>
public class OperationResultPruebas
{
    // ── OperationResult<T> ───────────────────────────────────────────────────

    /// <summary>Exitoso debe establecer EsExitoso=true y el valor correcto.</summary>
    [Fact]
    public void Exitoso_ConValor_EstableceExitosoYValor()
    {
        // Act
        var resultado = OperationResult<int>.Exitoso(42);

        // Assert
        Assert.True(resultado.EsExitoso);
        Assert.Equal(42, resultado.Valor);
        Assert.Null(resultado.MensajeError);
    }

    /// <summary>Fallido debe establecer EsExitoso=false y el mensaje de error.</summary>
    [Fact]
    public void Fallido_ConMensaje_EstableceExitosoFalsoYMensaje()
    {
        // Act
        var resultado = OperationResult<int>.Fallido("Error de prueba.");

        // Assert
        Assert.False(resultado.EsExitoso);
        Assert.Equal("Error de prueba.", resultado.MensajeError);
        Assert.Equal(default, resultado.Valor);
    }

    // ── OperationResult (no genérico) ────────────────────────────────────────

    /// <summary>Exitoso debe establecer EsExitoso=true sin mensaje de error.</summary>
    [Fact]
    public void Exitoso_SinValor_EstableceExitosoTrue()
    {
        // Act
        var resultado = OperationResult.Exitoso();

        // Assert
        Assert.True(resultado.EsExitoso);
        Assert.Null(resultado.MensajeError);
    }

    /// <summary>Fallido debe establecer EsExitoso=false con el mensaje de error.</summary>
    [Fact]
    public void Fallido_SinValor_EstableceExitosoFalsoYMensaje()
    {
        // Act
        var resultado = OperationResult.Fallido("Fallo genérico.");

        // Assert
        Assert.False(resultado.EsExitoso);
        Assert.Equal("Fallo genérico.", resultado.MensajeError);
    }
}
