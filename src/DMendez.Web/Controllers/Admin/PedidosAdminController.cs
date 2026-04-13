using DMendez.Data.Repositories.Interfaces;
using DMendez.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMendez.Web.Controllers.Admin;

/// <summary>Controlador administrativo para la gestión de pedidos.</summary>
[Authorize(Roles = "Administrador")]
[Route("Admin/Pedidos")]
public class PedidosAdminController : Controller
{
    private readonly IRepositorioPedido _repositorioPedido;

    /// <summary>Inicializa el controlador con el repositorio de pedidos.</summary>
    /// <param name="repositorioPedido">El repositorio de pedidos.</param>
    public PedidosAdminController(IRepositorioPedido repositorioPedido)
    {
        _repositorioPedido = repositorioPedido;
    }

    /// <summary>Lista todos los pedidos con filtro opcional por estado.</summary>
    /// <param name="estado">Estado opcional para filtrar los pedidos.</param>
    /// <returns>Vista con la lista de pedidos.</returns>
    [HttpGet("")]
    public async Task<IActionResult> Index(string? estado = null)
    {
        var resultado = string.IsNullOrEmpty(estado)
            ? await _repositorioPedido.ObtenerTodosAsync()
            : await _repositorioPedido.ObtenerPorEstadoAsync(estado);

        if (!resultado.EsExitoso)
        {
            TempData["MensajeError"] = resultado.MensajeError;
            return View(Enumerable.Empty<ResumenPedidoViewModel>());
        }

        var viewModels = resultado.Valor!.Select(p => new ResumenPedidoViewModel
        {
            Id = p.Id,
            CreadoEn = p.CreadoEn,
            Estado = p.Estado,
            MetodoEntrega = p.MetodoEntrega,
            Total = p.Total,
            CantidadItems = p.Items.Sum(i => i.Cantidad)
        });

        ViewBag.EstadoFiltro = estado;
        return View(viewModels);
    }

    /// <summary>Muestra el detalle completo de un pedido.</summary>
    /// <param name="id">Identificador del pedido.</param>
    /// <returns>Vista de detalle o NotFound.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> Detalle(int id)
    {
        var resultado = await _repositorioPedido.ObtenerPorIdAsync(id);

        if (!resultado.EsExitoso)
            return NotFound();

        var pedido = resultado.Valor!;
        var viewModel = new DetallePedidoViewModel
        {
            Id = pedido.Id,
            Estado = pedido.Estado,
            MetodoEntrega = pedido.MetodoEntrega,
            Subtotal = pedido.Subtotal,
            Impuestos = pedido.Impuestos,
            Descuento = pedido.Descuento,
            Total = pedido.Total,
            ReferenciaPago = pedido.ReferenciaPago,
            CodigoPromocion = pedido.CodigoPromocion,
            CreadoEn = pedido.CreadoEn,
            Items = pedido.Items.Select(i => new ItemPedidoViewModel
            {
                NombreProducto = i.Producto?.Nombre ?? "Producto eliminado",
                Cantidad = i.Cantidad,
                PrecioUnitario = i.PrecioUnitario
            }).ToList(),
            Historial = pedido.HistorialEstados
                .OrderBy(h => h.FechaHora)
                .Select(h => new HistorialEstadoViewModel
                {
                    Estado = h.Estado,
                    FechaHora = h.FechaHora,
                    Observacion = h.Observacion
                }).ToList()
        };

        return View(viewModel);
    }

    /// <summary>Avanza el estado de un pedido al siguiente en el ciclo de vida.</summary>
    /// <param name="id">Identificador del pedido.</param>
    /// <returns>Redirección al detalle con mensaje de resultado.</returns>
    [HttpPost("AvanzarEstado/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AvanzarEstado(int id)
    {
        var resultado = await _repositorioPedido.AvanzarEstadoAsync(id);

        if (!resultado.EsExitoso)
            TempData["MensajeError"] = resultado.MensajeError;
        else
            TempData["MensajeExito"] = $"Estado del pedido actualizado a '{resultado.Valor}'.";

        return RedirectToAction("Detalle", new { id });
    }
}
