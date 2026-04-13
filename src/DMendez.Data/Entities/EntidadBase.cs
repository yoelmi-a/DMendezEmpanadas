namespace DMendez.Data.Entities;

/// <summary>Entidad base con campos de auditoría comunes a todas las entidades del dominio.</summary>
public abstract class EntidadBase
{
    /// <summary>Identificador único de la entidad.</summary>
    public int Id { get; set; }

    /// <summary>Fecha y hora en que fue creada la entidad.</summary>
    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

    /// <summary>Fecha y hora de la última actualización de la entidad.</summary>
    public DateTime? ActualizadoEn { get; set; }
}
