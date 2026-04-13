using DMendez.Data.Entities;
using DMendez.Data.Repositories.Interfaces;
using DMendez.Data.Services.Interfaces;
using DMendez.Web.Filters;
using DMendez.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DMendez.Web.Controllers;

/// <summary>Controlador para las operaciones de pedidos del cliente.</summary>
[Authorize]
public class PedidoController : Controller
{
    private readonly IRepositorioPedido _repositorioPedido;
    private readonly IRepositorioProducto _repositorioProducto;
    private readonly IRepositorioPromocion _repositorioPromocion;
    private readonly IServicioPago _servicioPago;
    private readonly UserManager<IdentityUser> _gestorUsuarios;

    /// <summary>Inicializa el controlador con todos sus repositorios y servicios.</summary>
    /// <param name="repositorioPedido">Repositorio de pedidos.</param>
    /// <param name="repositorioProducto">Repositorio de productos.</param>
    /// <param name="repositorioPromocion">Repositorio de promociones.</param>
    /// <param name="servicioPago">Servicio de pago.</param>
    /// <param name="gestorUsuarios">Gestor de usuarios de Identity.</param>
    public PedidoController(
        IRepositorioPedido repositorioPedido,
        IRepositorioProducto repositorioProducto,
        IRepositorioPromocion repositorioPromocion,
        IServicioPago servicioPago,
        UserManager<IdentityUser> gestorUsuarios)
    {
        _repositorioPedido = repositorioPedido;
        _repositorioProducto = repositorioProducto;
        _repositorioPromocion = repositorioPromocion;
        _servicioPago = servicioPago;
        _gestorUsuarios = gestorUsuarios;
    }

    /// <summary>Muestra el historial de pedidos del cliente autenticado.</summary>
    /// <returns>Vista con la lista de pedidos del cliente.</returns>
    [HttpGet]
    public async Task<IActionResult> Historial()
    {
        string idCliente = _gestorUsuarios.GetUserId(User)!;
        var resultado = await _repositorioPedido.ObtenerPorClienteAsync(idCliente);

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

        return View(viewModels);
    }

    /// <summary>Muestra el detalle de un pedido específico del cliente.</summary>
    /// <param name="id">Identificador del pedido.</param>
    /// <returns>Vista de detalle o NotFound si no pertenece al cliente.</returns>
    [HttpGet]
    public async Task<IActionResult> Detalle(int id)
    {
        string idCliente = _gestorUsuarios.GetUserId(User)!;
        var resultado = await _repositorioPedido.ObtenerPorIdAsync(id);

        if (!resultado.EsExitoso)
            return NotFound();

        if (resultado.Valor!.IdCliente != idCliente)
            return Forbid();

        var pedido = resultado.Valor;
        var viewModel = MapearDetallePedido(pedido);
        return View(viewModel);
    }

    /// <summary>Muestra el formulario de checkout con el carrito actual.</summary>
    /// <returns>Vista de checkout o redirección si el carrito está vacío.</returns>
    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var carrito = GestorCarritoSesion.ObtenerCarrito(HttpContext.Session);

        if (!carrito.Items.Any())
        {
            TempData["MensajeError"] = "El carrito está vacío.";
            return RedirectToAction("Index", "Carrito");
        }

        // Validar disponibilidad antes de mostrar checkout
        foreach (var item in carrito.Items)
        {
            var res = await _repositorioProducto.ObtenerPorIdAsync(item.IdProducto);
            item.EstaDisponible = res.EsExitoso && res.Valor!.EstaDisponible;
        }

        if (carrito.TieneItemsNoDisponibles)
        {
            GestorCarritoSesion.GuardarCarrito(HttpContext.Session, carrito);
            TempData["MensajeError"] = "Algunos productos ya no están disponibles. Revisa tu carrito.";
            return RedirectToAction("Index", "Carrito");
        }

        var modelo = new CheckoutViewModel { Carrito = carrito };
        return View(modelo);
    }

    /// <summary>Procesa la confirmación del pedido y el pago simulado.</summary>
    /// <param name="modelo">Los datos del formulario de checkout.</param>
    /// <returns>Redirección a confirmación o la vista con errores.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutViewModel modelo)
    {
        var carrito = GestorCarritoSesion.ObtenerCarrito(HttpContext.Session);

        if (!carrito.Items.Any())
        {
            TempData["MensajeError"] = "El carrito está vacío.";
            return RedirectToAction("Index", "Carrito");
        }

        modelo.Carrito = carrito;

        if (!ModelState.IsValid)
            return View(modelo);

        // Aplicar promoción si se ingresó un código
        decimal descuento = 0;
        if (!string.IsNullOrWhiteSpace(modelo.CodigoPromocion))
        {
            var resultadoPromo = await _repositorioPromocion
                .ObtenerPorCodigoValidoAsync(modelo.CodigoPromocion);

            if (!resultadoPromo.EsExitoso)
            {
                ModelState.AddModelError("CodigoPromocion", resultadoPromo.MensajeError!);
                return View(modelo);
            }

            var promo = resultadoPromo.Valor!;
            descuento = promo.TipoDescuento == Data.Entities.TipoDescuento.Porcentaje
                ? Math.Round(carrito.Total * (promo.ValorDescuento / 100), 2)
                : promo.ValorDescuento;

            modelo.Descuento = descuento;
        }

        // Procesar pago simulado
        decimal totalFinal = Math.Max(0, carrito.Total - descuento);
        var solicitudPago = new SolicitudPago
        {
            Monto = totalFinal,
            SimularFallo = modelo.SimularFalloPago,
            ReferenciaPedido = $"PEDIDO-{DateTime.UtcNow.Ticks}"
        };

        var resultadoPago = await _servicioPago.ProcesarAsync(solicitudPago);

        if (!resultadoPago.EsExitoso)
        {
            ModelState.AddModelError(string.Empty, resultadoPago.MensajeError!);
            return View(modelo);
        }

        // Crear el pedido
        string idCliente = _gestorUsuarios.GetUserId(User)!;
        var pedido = new Pedido
        {
            IdCliente = idCliente,
            MetodoEntrega = modelo.MetodoEntrega,
            Subtotal = carrito.Subtotal,
            Impuestos = carrito.Impuestos,
            Descuento = descuento,
            Total = totalFinal,
            ReferenciaPago = resultadoPago.Valor,
            CodigoPromocion = modelo.CodigoPromocion?.Trim().ToUpper(),
            Items = carrito.Items.Select(i => new ItemPedido
            {
                IdProducto = i.IdProducto,
                Cantidad = i.Cantidad,
                PrecioUnitario = i.PrecioUnitario
            }).ToList()
        };

        var resultadoPedido = await _repositorioPedido.CrearAsync(pedido);

        if (!resultadoPedido.EsExitoso)
        {
            ModelState.AddModelError(string.Empty, resultadoPedido.MensajeError!);
            return View(modelo);
        }

        GestorCarritoSesion.LimpiarCarrito(HttpContext.Session);
        TempData["MensajeExito"] = $"¡Pedido #{resultadoPedido.Valor} realizado exitosamente!";
        return RedirectToAction("Detalle", new { id = resultadoPedido.Valor });
    }

    /// <summary>Cancela un pedido que se encuentra en estado Recibido.</summary>
    /// <param name="id">Identificador del pedido a cancelar.</param>
    /// <returns>Redirección al detalle del pedido con mensaje de resultado.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        string idCliente = _gestorUsuarios.GetUserId(User)!;
        var resultado = await _repositorioPedido.CancelarAsync(id, idCliente);

        if (!resultado.EsExitoso)
            TempData["MensajeError"] = resultado.MensajeError;
        else
            TempData["MensajeExito"] = "El pedido fue cancelado exitosamente.";

        return RedirectToAction("Detalle", new { id });
    }

    /// <summary>Mapea una entidad Pedido al ViewModel de detalle.</summary>
    /// <param name="pedido">La entidad de pedido a mapear.</param>
    /// <returns>El ViewModel de detalle del pedido.</returns>
    private static DetallePedidoViewModel MapearDetallePedido(Pedido pedido)
    {
        return new DetallePedidoViewModel
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
    }
}
