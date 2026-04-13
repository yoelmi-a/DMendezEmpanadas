namespace DMendez.Data.Entities;

/// <summary>Define las categorías disponibles para los productos del menú.</summary>
public enum CategoriaProducto
{
    /// <summary>Empanadas de diferentes sabores.</summary>
    Empanadas,

    /// <summary>Bebidas frías y calientes.</summary>
    Bebidas,

    /// <summary>Combos de empanadas con bebidas u otros productos.</summary>
    Combos
}

/// <summary>Representa un producto disponible en el catálogo de D' Méndez Empanadas.</summary>
public class Producto : EntidadBase
{
    /// <summary>Nombre del producto.</summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Descripción detallada del producto.</summary>
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>Precio unitario del producto en pesos dominicanos.</summary>
    public decimal Precio { get; set; }

    /// <summary>Categoría a la que pertenece el producto.</summary>
    public CategoriaProducto Categoria { get; set; }

    /// <summary>Indica si el producto está disponible para la venta.</summary>
    public bool EstaDisponible { get; set; } = true;

    /// <summary>Indica si el producto es un combo que contiene otros productos.</summary>
    public bool EsCombo { get; set; }

    /// <summary>Descripción de los componentes del combo (aplica solo si EsCombo es true).</summary>
    public string? ComponentesCombo { get; set; }

    /// <summary>Colección de ítems de pedido que referencian este producto.</summary>
    public ICollection<ItemPedido> ItemsPedido { get; set; } = new List<ItemPedido>();
}
