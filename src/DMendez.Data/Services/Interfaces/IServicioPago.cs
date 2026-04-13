using DMendez.Data.Common;

namespace DMendez.Data.Services.Interfaces;

/// <summary>Representa los datos necesarios para procesar un pago.</summary>
public class SolicitudPago
{
    /// <summary>Monto total a cobrar.</summary>
    public decimal Monto { get; set; }

    /// <summary>Indica si se debe simular un fallo en el pago.</summary>
    public bool SimularFallo { get; set; }

    /// <summary>Referencia interna del pedido asociado al pago.</summary>
    public string ReferenciaPedido { get; set; } = string.Empty;
}

/// <summary>Abstracción del servicio de procesamiento de pagos.</summary>
public interface IServicioPago
{
    /// <summary>Procesa un pago de forma asíncrona.</summary>
    /// <param name="solicitud">Los datos del pago a procesar.</param>
    /// <returns>
    /// Un <see cref="OperationResult{T}"/> con la referencia de la transacción generada,
    /// o un mensaje de error si el pago falla.
    /// </returns>
    Task<OperationResult<string>> ProcesarAsync(SolicitudPago solicitud);
}
