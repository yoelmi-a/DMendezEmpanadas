using DMendez.Data.Common;
using DMendez.Data.Context;
using DMendez.Data.Entities;
using DMendez.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DMendez.Data.Repositories;

/// <summary>Implementación del repositorio de pedidos usando EF Core InMemory.</summary>
public class RepositorioPedido : IRepositorioPedido
{
    private readonly AppDbContext _contexto;

    /// <summary>Inicializa el repositorio con el contexto de base de datos.</summary>
    /// <param name="contexto">El contexto de base de datos de la aplicación.</param>
    public RepositorioPedido(AppDbContext contexto) => _contexto = contexto;

    /// <summary>Obtiene todos los pedidos de un cliente específico.</summary>
    /// <param name="idCliente">Identificador del cliente.</param>
    /// <returns>Lista de pedidos del cliente ordenados por fecha descendente.</returns>
    public async Task<OperationResult<IEnumerable<Pedido>>> ObtenerPorClienteAsync(string idCliente)
    {
        var pedidos = await _contexto.Pedidos
            .Include(p => p.Items)
                .ThenInclude(i => i.Producto)
            .Include(p => p.HistorialEstados)
            .Where(p => p.IdCliente == idCliente)
            .OrderByDescending(p => p.CreadoEn)
            .ToListAsync();

        return OperationResult<IEnumerable<Pedido>>.Exitoso(pedidos);
    }

    /// <summary>Obtiene todos los pedidos del sistema.</summary>
    /// <returns>Lista de todos los pedidos ordenados por fecha descendente.</returns>
    public async Task<OperationResult<IEnumerable<Pedido>>> ObtenerTodosAsync()
    {
        var pedidos = await _contexto.Pedidos
            .Include(p => p.Items)
                .ThenInclude(i => i.Producto)
            .Include(p => p.HistorialEstados)
            .OrderByDescending(p => p.CreadoEn)
            .ToListAsync();

        return OperationResult<IEnumerable<Pedido>>.Exitoso(pedidos);
    }

    /// <summary>Obtiene todos los pedidos filtrados por estado.</summary>
    /// <param name="estado">Estado por el que filtrar.</param>
    /// <returns>Lista de pedidos con el estado especificado.</returns>
    public async Task<OperationResult<IEnumerable<Pedido>>> ObtenerPorEstadoAsync(string estado)
    {
        var pedidos = await _contexto.Pedidos
            .Include(p => p.Items)
                .ThenInclude(i => i.Producto)
            .Include(p => p.HistorialEstados)
            .Where(p => p.Estado == estado)
            .OrderByDescending(p => p.CreadoEn)
            .ToListAsync();

        return OperationResult<IEnumerable<Pedido>>.Exitoso(pedidos);
    }

    /// <summary>Obtiene un pedido por su identificador con todos sus detalles.</summary>
    /// <param name="id">Identificador del pedido.</param>
    /// <returns>El pedido con sus ítems e historial de estados.</returns>
    public async Task<OperationResult<Pedido>> ObtenerPorIdAsync(int id)
    {
        var pedido = await _contexto.Pedidos
            .Include(p => p.Items)
                .ThenInclude(i => i.Producto)
            .Include(p => p.HistorialEstados)
            .FirstOrDefaultAsync(p => p.Id == id);

        return pedido is null
            ? OperationResult<Pedido>.Fallido($"Pedido con Id {id} no encontrado.")
            : OperationResult<Pedido>.Exitoso(pedido);
    }

    /// <summary>Crea un nuevo pedido en el sistema y registra el estado inicial.</summary>
    /// <param name="pedido">El pedido a crear.</param>
    /// <returns>El identificador del pedido creado.</returns>
    public async Task<OperationResult<int>> CrearAsync(Pedido pedido)
    {
        if (pedido.Items == null || !pedido.Items.Any())
            return OperationResult<int>.Fallido("El pedido debe contener al menos un producto.");

        pedido.Estado = EstadoPedido.Recibido;
        pedido.CreadoEn = DateTime.UtcNow;

        pedido.HistorialEstados.Add(new HistorialEstado
        {
            Estado = EstadoPedido.Recibido,
            FechaHora = DateTime.UtcNow,
            Observacion = "Pedido recibido."
        });

        _contexto.Pedidos.Add(pedido);
        await _contexto.SaveChangesAsync();

        return OperationResult<int>.Exitoso(pedido.Id);
    }

    /// <summary>Avanza el estado del pedido al siguiente en el ciclo de vida.</summary>
    /// <param name="id">Identificador del pedido.</param>
    /// <returns>Resultado con el nuevo estado del pedido.</returns>
    public async Task<OperationResult<string>> AvanzarEstadoAsync(int id)
    {
        var pedido = await _contexto.Pedidos
            .Include(p => p.HistorialEstados)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pedido is null)
            return OperationResult<string>.Fallido($"Pedido con Id {id} no encontrado.");

        if (pedido.Estado == EstadoPedido.Entregado)
            return OperationResult<string>.Fallido("El pedido ya fue entregado y no puede modificarse.");

        if (pedido.Estado == EstadoPedido.Cancelado)
            return OperationResult<string>.Fallido("El pedido está cancelado y no puede modificarse.");

        string? nuevoEstado = EstadoPedido.SiguienteEstado(pedido.Estado);
        if (nuevoEstado is null)
            return OperationResult<string>.Fallido("No existe una transición válida desde el estado actual.");

        pedido.Estado = nuevoEstado;
        pedido.ActualizadoEn = DateTime.UtcNow;

        pedido.HistorialEstados.Add(new HistorialEstado
        {
            Estado = nuevoEstado,
            FechaHora = DateTime.UtcNow
        });

        await _contexto.SaveChangesAsync();
        return OperationResult<string>.Exitoso(nuevoEstado);
    }

    /// <summary>Cancela un pedido que se encuentra en estado Recibido.</summary>
    /// <param name="id">Identificador del pedido.</param>
    /// <param name="idCliente">Identificador del cliente solicitante.</param>
    /// <returns>Resultado de la operación de cancelación.</returns>
    public async Task<OperationResult> CancelarAsync(int id, string idCliente)
    {
        var pedido = await _contexto.Pedidos
            .Include(p => p.HistorialEstados)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pedido is null)
            return OperationResult.Fallido($"Pedido con Id {id} no encontrado.");

        if (pedido.IdCliente != idCliente)
            return OperationResult.Fallido("No tiene permiso para cancelar este pedido.");

        if (pedido.Estado != EstadoPedido.Recibido)
            return OperationResult.Fallido("El pedido no puede ser cancelado en su estado actual.");

        pedido.Estado = EstadoPedido.Cancelado;
        pedido.ActualizadoEn = DateTime.UtcNow;

        pedido.HistorialEstados.Add(new HistorialEstado
        {
            Estado = EstadoPedido.Cancelado,
            FechaHora = DateTime.UtcNow,
            Observacion = "Cancelado por el cliente."
        });

        await _contexto.SaveChangesAsync();
        return OperationResult.Exitoso();
    }

    /// <summary>Obtiene pedidos entregados dentro del rango de fechas para reportes.</summary>
    /// <param name="desde">Fecha de inicio del período.</param>
    /// <param name="hasta">Fecha de fin del período.</param>
    /// <returns>Lista de pedidos entregados en el rango.</returns>
    public async Task<OperationResult<IEnumerable<Pedido>>> ObtenerParaReporteAsync(DateTime desde, DateTime hasta)
    {
        var pedidos = await _contexto.Pedidos
            .Include(p => p.Items)
            .Where(p => p.Estado == EstadoPedido.Entregado
                     && p.CreadoEn >= desde
                     && p.CreadoEn <= hasta)
            .OrderBy(p => p.CreadoEn)
            .ToListAsync();

        return OperationResult<IEnumerable<Pedido>>.Exitoso(pedidos);
    }
}
