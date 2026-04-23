using DMendez.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DMendez.Data.Context;
// test
/// <summary>Contexto de base de datos principal de la aplicación D' Méndez Empanadas.</summary>
public class AppDbContext : IdentityDbContext<IdentityUser>
{
    /// <summary>Conjunto de productos del catálogo.</summary>
    public DbSet<Producto> Productos => Set<Producto>();

    /// <summary>Conjunto de pedidos realizados por los clientes.</summary>
    public DbSet<Pedido> Pedidos => Set<Pedido>();

    /// <summary>Conjunto de ítems individuales de cada pedido.</summary>
    public DbSet<ItemPedido> ItemsPedido => Set<ItemPedido>();

    /// <summary>Conjunto de registros del historial de estados de pedidos.</summary>
    public DbSet<HistorialEstado> HistorialesEstado => Set<HistorialEstado>();

    /// <summary>Conjunto de promociones disponibles.</summary>
    public DbSet<Promocion> Promociones => Set<Promocion>();

    /// <summary>Inicializa el contexto con las opciones de configuración especificadas.</summary>
    /// <param name="opciones">Opciones de configuración del contexto.</param>
    public AppDbContext(DbContextOptions<AppDbContext> opciones) : base(opciones) { }

    /// <summary>Configura el modelo de datos aplicando las configuraciones de entidades.</summary>
    /// <param name="constructor">El constructor del modelo de datos.</param>
    protected override void OnModelCreating(ModelBuilder constructor)
    {
        base.OnModelCreating(constructor);
        constructor.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
