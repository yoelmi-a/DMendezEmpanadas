namespace DMendez.Data.Entities;

/// <summary>Registra un cambio de estado en el ciclo de vida de un pedido.</summary>
public class HistorialEstado : EntidadBase
{
    /// <summary>Identificador del pedido al que pertenece este registro.</summary>
    public int IdPedido { get; set; }

    /// <summary>Pedido al que pertenece este registro de historial.</summary>
    public Pedido Pedido { get; set; } = null!;

    /// <summary>Estado registrado en este punto del historial.</summary>
    public string Estado { get; set; } = string.Empty;

    /// <summary>Fecha y hora exacta del cambio de estado.</summary>
    public DateTime FechaHora { get; set; } = DateTime.UtcNow;

    /// <summary>Observación opcional sobre el cambio de estado.</summary>
    public string? Observacion { get; set; }
}
