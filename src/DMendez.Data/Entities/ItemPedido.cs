namespace DMendez.Data.Entities;

/// <summary>Representa un ítem individual dentro de un pedido.</summary>
public class ItemPedido : EntidadBase
{
    /// <summary>Identificador del pedido al que pertenece este ítem.</summary>
    public int IdPedido { get; set; }

    /// <summary>Pedido al que pertenece este ítem.</summary>
    public Pedido Pedido { get; set; } = null!;

    /// <summary>Identificador del producto asociado a este ítem.</summary>
    public int IdProducto { get; set; }

    /// <summary>Producto asociado a este ítem.</summary>
    public Producto Producto { get; set; } = null!;

    /// <summary>Cantidad del producto en este ítem.</summary>
    public int Cantidad { get; set; }

    /// <summary>Precio unitario del producto al momento de realizar el pedido.</summary>
    public decimal PrecioUnitario { get; set; }

    /// <summary>Subtotal calculado para este ítem (Cantidad × PrecioUnitario).</summary>
    public decimal Subtotal => Cantidad * PrecioUnitario;
}
