namespace DMendez.Data.Entities;

/// <summary>Define los posibles estados de un pedido a lo largo de su ciclo de vida.</summary>
public static class EstadoPedido
{
    /// <summary>El pedido ha sido recibido y está pendiente de preparación.</summary>
    public const string Recibido = "Recibido";

    /// <summary>El pedido está siendo preparado en cocina.</summary>
    public const string EnPreparacion = "EnPreparacion";

    /// <summary>El pedido está en camino hacia el cliente.</summary>
    public const string EnCamino = "EnCamino";

    /// <summary>El pedido ha sido entregado exitosamente.</summary>
    public const string Entregado = "Entregado";

    /// <summary>El pedido fue cancelado por el cliente.</summary>
    public const string Cancelado = "Cancelado";

    /// <summary>Devuelve la secuencia de transiciones válidas del ciclo de vida del pedido.</summary>
    /// <returns>Lista ordenada de estados en secuencia.</returns>
    public static IReadOnlyList<string> SecuenciaNormal =>
        new[] { Recibido, EnPreparacion, EnCamino, Entregado };

    /// <summary>Obtiene el siguiente estado válido en el ciclo de vida normal.</summary>
    /// <param name="estadoActual">El estado actual del pedido.</param>
    /// <returns>El siguiente estado, o null si no existe transición válida.</returns>
    public static string? SiguienteEstado(string estadoActual)
    {
        var secuencia = SecuenciaNormal;
        int indice = Array.IndexOf(secuencia.ToArray(), estadoActual);
        return indice >= 0 && indice < secuencia.Count - 1 ? secuencia[indice + 1] : null;
    }
}

/// <summary>Define los métodos de entrega disponibles para un pedido.</summary>
public enum MetodoEntrega
{
    /// <summary>El cliente recoge el pedido en el local.</summary>
    Recogida,

    /// <summary>El pedido se entrega en el domicilio del cliente.</summary>
    Domicilio
}

/// <summary>Representa un pedido realizado por un cliente.</summary>
public class Pedido : EntidadBase
{
    /// <summary>Identificador del cliente que realizó el pedido.</summary>
    public string IdCliente { get; set; } = string.Empty;

    /// <summary>Estado actual del pedido.</summary>
    public string Estado { get; set; } = EstadoPedido.Recibido;

    /// <summary>Método de entrega seleccionado por el cliente.</summary>
    public MetodoEntrega MetodoEntrega { get; set; }

    /// <summary>Subtotal del pedido antes de impuestos y descuentos.</summary>
    public decimal Subtotal { get; set; }

    /// <summary>Monto de impuestos aplicados al pedido.</summary>
    public decimal Impuestos { get; set; }

    /// <summary>Descuento aplicado mediante promoción.</summary>
    public decimal Descuento { get; set; }

    /// <summary>Total final del pedido.</summary>
    public decimal Total { get; set; }

    /// <summary>Referencia de transacción generada por el servicio de pago simulado.</summary>
    public string? ReferenciaPago { get; set; }

    /// <summary>Código de promoción aplicado al pedido.</summary>
    public string? CodigoPromocion { get; set; }

    /// <summary>Lista de ítems que conforman el pedido.</summary>
    public ICollection<ItemPedido> Items { get; set; } = new List<ItemPedido>();

    /// <summary>Historial de cambios de estado del pedido.</summary>
    public ICollection<HistorialEstado> HistorialEstados { get; set; } = new List<HistorialEstado>();
}
