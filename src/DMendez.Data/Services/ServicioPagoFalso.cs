using DMendez.Data.Common;
using DMendez.Data.Services.Interfaces;

namespace DMendez.Data.Services;

/// <summary>
/// Implementación simulada del servicio de pago para propósitos educativos.
/// Genera una referencia GUID en caso de éxito o retorna un fallo controlado.
/// </summary>
public class ServicioPagoFalso : IServicioPago
{
    /// <summary>
    /// Procesa el pago de forma simulada.
    /// Siempre tiene éxito salvo que <see cref="SolicitudPago.SimularFallo"/> sea true.
    /// </summary>
    /// <param name="solicitud">Los datos del pago a procesar.</param>
    /// <returns>
    /// Éxito con una referencia GUID si <c>SimularFallo</c> es false;
    /// fallo con mensaje descriptivo si es true.
    /// </returns>
    public Task<OperationResult<string>> ProcesarAsync(SolicitudPago solicitud)
    {
        if (solicitud.SimularFallo)
        {
            return Task.FromResult(
                OperationResult<string>.Fallido(
                    "Pago rechazado: simulación de fallo activa."));
        }

        string referencia = Guid.NewGuid().ToString();
        return Task.FromResult(OperationResult<string>.Exitoso(referencia));
    }
}
