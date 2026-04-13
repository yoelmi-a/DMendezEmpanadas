namespace DMendez.Data.Entities;

/// <summary>Define el tipo de descuento que aplica una promoción.</summary>
public enum TipoDescuento
{
    /// <summary>Descuento expresado como porcentaje del total.</summary>
    Porcentaje,

    /// <summary>Descuento expresado como monto fijo en pesos dominicanos.</summary>
    MontoFijo
}

/// <summary>Representa una promoción o descuento aplicable a un pedido.</summary>
public class Promocion : EntidadBase
{
    /// <summary>Nombre descriptivo de la promoción.</summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Código único que el cliente ingresa para aplicar la promoción.</summary>
    public string Codigo { get; set; } = string.Empty;

    /// <summary>Tipo de descuento que aplica la promoción.</summary>
    public TipoDescuento TipoDescuento { get; set; }

    /// <summary>Valor del descuento (porcentaje o monto fijo según TipoDescuento).</summary>
    public decimal ValorDescuento { get; set; }

    /// <summary>Fecha de inicio de vigencia de la promoción.</summary>
    public DateTime FechaInicio { get; set; }

    /// <summary>Fecha de fin de vigencia de la promoción.</summary>
    public DateTime FechaFin { get; set; }

    /// <summary>Indica si la promoción está activa.</summary>
    public bool EstaActiva { get; set; } = true;

    /// <summary>Determina si la promoción es válida en la fecha especificada.</summary>
    /// <param name="fecha">Fecha a evaluar.</param>
    /// <returns>True si la promoción está activa y dentro del rango de fechas.</returns>
    public bool EsValidaEn(DateTime fecha) =>
        EstaActiva && fecha >= FechaInicio && fecha <= FechaFin;
}
