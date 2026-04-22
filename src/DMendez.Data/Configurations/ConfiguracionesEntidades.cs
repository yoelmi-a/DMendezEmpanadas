using DMendez.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
// test
namespace DMendez.Data.Configurations;

/// <summary>Configuración de la entidad Producto para EF Core.</summary>
public class ConfiguracionProducto : IEntityTypeConfiguration<Producto>
{
    /// <summary>Aplica la configuración de la entidad Producto.</summary>
    /// <param name="constructor">El constructor de tipo de entidad.</param>
    public void Configure(EntityTypeBuilder<Producto> constructor)
    {
        constructor.HasKey(p => p.Id);
        constructor.Property(p => p.Nombre).IsRequired().HasMaxLength(150);
        constructor.Property(p => p.Descripcion).IsRequired().HasMaxLength(500);
        constructor.Property(p => p.Precio).HasPrecision(18, 2);
        constructor.Property(p => p.Categoria).IsRequired();
    }
}

/// <summary>Configuración de la entidad Pedido para EF Core.</summary>
public class ConfiguracionPedido : IEntityTypeConfiguration<Pedido>
{
    /// <summary>Aplica la configuración de la entidad Pedido.</summary>
    /// <param name="constructor">El constructor de tipo de entidad.</param>
    public void Configure(EntityTypeBuilder<Pedido> constructor)
    {
        constructor.HasKey(p => p.Id);
        constructor.Property(p => p.IdCliente).IsRequired();
        constructor.Property(p => p.Estado).IsRequired().HasMaxLength(50);
        constructor.Property(p => p.Subtotal).HasPrecision(18, 2);
        constructor.Property(p => p.Impuestos).HasPrecision(18, 2);
        constructor.Property(p => p.Descuento).HasPrecision(18, 2);
        constructor.Property(p => p.Total).HasPrecision(18, 2);

        constructor.HasMany(p => p.Items)
            .WithOne(i => i.Pedido)
            .HasForeignKey(i => i.IdPedido)
            .OnDelete(DeleteBehavior.Cascade);

        constructor.HasMany(p => p.HistorialEstados)
            .WithOne(h => h.Pedido)
            .HasForeignKey(h => h.IdPedido)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>Configuración de la entidad ItemPedido para EF Core.</summary>
public class ConfiguracionItemPedido : IEntityTypeConfiguration<ItemPedido>
{
    /// <summary>Aplica la configuración de la entidad ItemPedido.</summary>
    /// <param name="constructor">El constructor de tipo de entidad.</param>
    public void Configure(EntityTypeBuilder<ItemPedido> constructor)
    {
        constructor.HasKey(i => i.Id);
        constructor.Property(i => i.Cantidad).IsRequired();
        constructor.Property(i => i.PrecioUnitario).HasPrecision(18, 2);

        constructor.HasOne(i => i.Producto)
            .WithMany(p => p.ItemsPedido)
            .HasForeignKey(i => i.IdProducto)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>Configuración de la entidad HistorialEstado para EF Core.</summary>
public class ConfiguracionHistorialEstado : IEntityTypeConfiguration<HistorialEstado>
{
    /// <summary>Aplica la configuración de la entidad HistorialEstado.</summary>
    /// <param name="constructor">El constructor de tipo de entidad.</param>
    public void Configure(EntityTypeBuilder<HistorialEstado> constructor)
    {
        constructor.HasKey(h => h.Id);
        constructor.Property(h => h.Estado).IsRequired().HasMaxLength(50);
        constructor.Property(h => h.Observacion).HasMaxLength(300);
    }
}

/// <summary>Configuración de la entidad Promocion para EF Core.</summary>
public class ConfiguracionPromocion : IEntityTypeConfiguration<Promocion>
{
    /// <summary>Aplica la configuración de la entidad Promocion.</summary>
    /// <param name="constructor">El constructor de tipo de entidad.</param>
    public void Configure(EntityTypeBuilder<Promocion> constructor)
    {
        constructor.HasKey(p => p.Id);
        constructor.Property(p => p.Nombre).IsRequired().HasMaxLength(150);
        constructor.Property(p => p.Codigo).IsRequired().HasMaxLength(50);
        constructor.Property(p => p.ValorDescuento).HasPrecision(18, 2);
        constructor.HasIndex(p => p.Codigo).IsUnique();
    }
}
