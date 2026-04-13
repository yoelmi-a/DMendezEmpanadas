using System.ComponentModel.DataAnnotations;
using DMendez.Data.Entities;

namespace DMendez.Web.ViewModels;

/// <summary>Representa un ítem dentro del carrito de compras en sesión.</summary>
public class ItemCarritoViewModel
{
    /// <summary>Identificador del producto.</summary>
    public int IdProducto { get; set; }

    /// <summary>Nombre del producto.</summary>
    public string NombreProducto { get; set; } = string.Empty;

    /// <summary>Precio unitario del producto.</summary>
    public decimal PrecioUnitario { get; set; }

    /// <summary>Cantidad seleccionada del producto.</summary>
    public int Cantidad { get; set; }

    /// <summary>Indica si el producto sigue disponible.</summary>
    public bool EstaDisponible { get; set; } = true;

    /// <summary>Subtotal calculado para este ítem.</summary>
    public decimal Subtotal => PrecioUnitario * Cantidad;
}

/// <summary>Representa el carrito de compras completo del cliente.</summary>
public class CarritoViewModel
{
    /// <summary>Lista de ítems en el carrito.</summary>
    public List<ItemCarritoViewModel> Items { get; set; } = new();

    /// <summary>Subtotal antes de impuestos.</summary>
    public decimal Subtotal => Items.Sum(i => i.Subtotal);

    /// <summary>Monto de impuestos (ITBIS 18%).</summary>
    public decimal Impuestos => Math.Round(Subtotal * 0.18m, 2);

    /// <summary>Total incluyendo impuestos.</summary>
    public decimal Total => Subtotal + Impuestos;

    /// <summary>Indica si algún ítem del carrito no está disponible.</summary>
    public bool TieneItemsNoDisponibles => Items.Any(i => !i.EstaDisponible);
}

/// <summary>ViewModel para el proceso de checkout.</summary>
public class CheckoutViewModel
{
    /// <summary>Resumen del carrito para mostrar en el checkout.</summary>
    public CarritoViewModel Carrito { get; set; } = new();

    /// <summary>Método de entrega seleccionado por el cliente.</summary>
    [Required(ErrorMessage = "Debe seleccionar un método de entrega.")]
    [Display(Name = "Método de entrega")]
    public MetodoEntrega MetodoEntrega { get; set; }

    /// <summary>Código de promoción opcional.</summary>
    [Display(Name = "Código de promoción")]
    [StringLength(50)]
    public string? CodigoPromocion { get; set; }

    /// <summary>Descuento aplicado por la promoción.</summary>
    public decimal Descuento { get; set; }

    /// <summary>Indica si se debe simular un fallo en el pago.</summary>
    [Display(Name = "Simular fallo de pago")]
    public bool SimularFalloPago { get; set; }

    /// <summary>Total con descuento aplicado.</summary>
    public decimal TotalConDescuento => Math.Max(0, Carrito.Total - Descuento);
}

/// <summary>ViewModel para mostrar el detalle de un pedido.</summary>
public class DetallePedidoViewModel
{
    /// <summary>Identificador del pedido.</summary>
    public int Id { get; set; }

    /// <summary>Estado actual del pedido.</summary>
    public string Estado { get; set; } = string.Empty;

    /// <summary>Método de entrega del pedido.</summary>
    public MetodoEntrega MetodoEntrega { get; set; }

    /// <summary>Subtotal del pedido.</summary>
    public decimal Subtotal { get; set; }

    /// <summary>Impuestos del pedido.</summary>
    public decimal Impuestos { get; set; }

    /// <summary>Descuento aplicado al pedido.</summary>
    public decimal Descuento { get; set; }

    /// <summary>Total final del pedido.</summary>
    public decimal Total { get; set; }

    /// <summary>Referencia de pago generada.</summary>
    public string? ReferenciaPago { get; set; }

    /// <summary>Código de promoción aplicado.</summary>
    public string? CodigoPromocion { get; set; }

    /// <summary>Fecha de creación del pedido.</summary>
    public DateTime CreadoEn { get; set; }

    /// <summary>Lista de ítems del pedido.</summary>
    public List<ItemPedidoViewModel> Items { get; set; } = new();

    /// <summary>Historial de estados del pedido.</summary>
    public List<HistorialEstadoViewModel> Historial { get; set; } = new();

    /// <summary>Indica si el pedido puede ser cancelado por el cliente.</summary>
    public bool PuedeCancelarse => Estado == EstadoPedido.Recibido;

    /// <summary>Siguiente estado disponible para el administrador.</summary>
    public string? SiguienteEstado => EstadoPedido.SiguienteEstado(Estado);
}

/// <summary>ViewModel para un ítem de pedido.</summary>
public class ItemPedidoViewModel
{
    /// <summary>Nombre del producto.</summary>
    public string NombreProducto { get; set; } = string.Empty;

    /// <summary>Cantidad del producto.</summary>
    public int Cantidad { get; set; }

    /// <summary>Precio unitario al momento del pedido.</summary>
    public decimal PrecioUnitario { get; set; }

    /// <summary>Subtotal del ítem.</summary>
    public decimal Subtotal => Cantidad * PrecioUnitario;
}

/// <summary>ViewModel para un registro del historial de estados.</summary>
public class HistorialEstadoViewModel
{
    /// <summary>Estado registrado.</summary>
    public string Estado { get; set; } = string.Empty;

    /// <summary>Fecha y hora del cambio de estado.</summary>
    public DateTime FechaHora { get; set; }

    /// <summary>Observación sobre el cambio de estado.</summary>
    public string? Observacion { get; set; }
}

/// <summary>ViewModel para el listado de pedidos del cliente.</summary>
public class ResumenPedidoViewModel
{
    /// <summary>Identificador del pedido.</summary>
    public int Id { get; set; }

    /// <summary>Fecha de creación del pedido.</summary>
    public DateTime CreadoEn { get; set; }

    /// <summary>Estado actual del pedido.</summary>
    public string Estado { get; set; } = string.Empty;

    /// <summary>Método de entrega del pedido.</summary>
    public MetodoEntrega MetodoEntrega { get; set; }

    /// <summary>Total del pedido.</summary>
    public decimal Total { get; set; }

    /// <summary>Cantidad de ítems en el pedido.</summary>
    public int CantidadItems { get; set; }
}
