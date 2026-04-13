using DMendez.Data.Common;
using DMendez.Data.Entities;

namespace DMendez.Data.Repositories.Interfaces;

/// <summary>Contrato para el repositorio de pedidos.</summary>
public interface IRepositorioPedido
{
    /// <summary>Obtiene todos los pedidos de un cliente específico.</summary>
    /// <param name="idCliente">Identificador del cliente.</param>
    /// <returns>Lista de pedidos del cliente ordenados por fecha descendente.</returns>
    Task<OperationResult<IEnumerable<Pedido>>> ObtenerPorClienteAsync(string idCliente);

    /// <summary>Obtiene todos los pedidos del sistema (uso administrativo).</summary>
    /// <returns>Lista de todos los pedidos.</returns>
    Task<OperationResult<IEnumerable<Pedido>>> ObtenerTodosAsync();

    /// <summary>Obtiene todos los pedidos filtrados por estado.</summary>
    /// <param name="estado">Estado por el que filtrar.</param>
    /// <returns>Lista de pedidos con el estado especificado.</returns>
    Task<OperationResult<IEnumerable<Pedido>>> ObtenerPorEstadoAsync(string estado);

    /// <summary>Obtiene un pedido por su identificador con todos sus detalles.</summary>
    /// <param name="id">Identificador del pedido.</param>
    /// <returns>El pedido con sus ítems e historial de estados.</returns>
    Task<OperationResult<Pedido>> ObtenerPorIdAsync(int id);

    /// <summary>Crea un nuevo pedido en el sistema.</summary>
    /// <param name="pedido">El pedido a crear.</param>
    /// <returns>El identificador del pedido creado.</returns>
    Task<OperationResult<int>> CrearAsync(Pedido pedido);

    /// <summary>Avanza el estado del pedido al siguiente en el ciclo de vida.</summary>
    /// <param name="id">Identificador del pedido.</param>
    /// <returns>Resultado de la operación con el nuevo estado.</returns>
    Task<OperationResult<string>> AvanzarEstadoAsync(int id);

    /// <summary>Cancela un pedido que se encuentra en estado Recibido.</summary>
    /// <param name="id">Identificador del pedido.</param>
    /// <param name="idCliente">Identificador del cliente solicitante.</param>
    /// <returns>Resultado de la operación de cancelación.</returns>
    Task<OperationResult> CancelarAsync(int id, string idCliente);

    /// <summary>Obtiene datos agregados de ventas para el panel de reportes.</summary>
    /// <param name="desde">Fecha de inicio del período.</param>
    /// <param name="hasta">Fecha de fin del período.</param>
    /// <returns>Lista de pedidos entregados en el rango de fechas.</returns>
    Task<OperationResult<IEnumerable<Pedido>>> ObtenerParaReporteAsync(DateTime desde, DateTime hasta);
}
