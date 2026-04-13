using DMendez.Data.Common;
using DMendez.Data.Context;
using DMendez.Data.Entities;
using DMendez.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DMendez.Data.Repositories;

/// <summary>Implementación del repositorio de productos usando EF Core InMemory.</summary>
public class RepositorioProducto : IRepositorioProducto
{
    private readonly AppDbContext _contexto;

    /// <summary>Inicializa el repositorio con el contexto de base de datos.</summary>
    /// <param name="contexto">El contexto de base de datos de la aplicación.</param>
    public RepositorioProducto(AppDbContext contexto) => _contexto = contexto;

    /// <summary>Obtiene todos los productos disponibles en el catálogo.</summary>
    /// <returns>Lista de productos con disponibilidad activa.</returns>
    public async Task<OperationResult<IEnumerable<Producto>>> ObtenerDisponiblesAsync()
    {
        var productos = await _contexto.Productos
            .Where(p => p.EstaDisponible)
            .OrderBy(p => p.Categoria)
            .ThenBy(p => p.Nombre)
            .ToListAsync();

        return OperationResult<IEnumerable<Producto>>.Exitoso(productos);
    }

    /// <summary>Obtiene todos los productos incluyendo no disponibles.</summary>
    /// <returns>Lista completa de productos.</returns>
    public async Task<OperationResult<IEnumerable<Producto>>> ObtenerTodosAsync()
    {
        var productos = await _contexto.Productos
            .OrderBy(p => p.Categoria)
            .ThenBy(p => p.Nombre)
            .ToListAsync();

        return OperationResult<IEnumerable<Producto>>.Exitoso(productos);
    }

    /// <summary>Obtiene un producto por su identificador.</summary>
    /// <param name="id">Identificador del producto.</param>
    /// <returns>El producto encontrado o un error si no existe.</returns>
    public async Task<OperationResult<Producto>> ObtenerPorIdAsync(int id)
    {
        var producto = await _contexto.Productos.FindAsync(id);

        return producto is null
            ? OperationResult<Producto>.Fallido($"Producto con Id {id} no encontrado.")
            : OperationResult<Producto>.Exitoso(producto);
    }

    /// <summary>Obtiene productos filtrados por categoría.</summary>
    /// <param name="categoria">Categoría a filtrar.</param>
    /// <returns>Lista de productos de la categoría especificada que estén disponibles.</returns>
    public async Task<OperationResult<IEnumerable<Producto>>> ObtenerPorCategoriaAsync(CategoriaProducto categoria)
    {
        var productos = await _contexto.Productos
            .Where(p => p.Categoria == categoria && p.EstaDisponible)
            .OrderBy(p => p.Nombre)
            .ToListAsync();

        return OperationResult<IEnumerable<Producto>>.Exitoso(productos);
    }

    /// <summary>Agrega un nuevo producto al catálogo.</summary>
    /// <param name="producto">El producto a agregar.</param>
    /// <returns>El identificador del producto creado.</returns>
    public async Task<OperationResult<int>> AgregarAsync(Producto producto)
    {
        if (string.IsNullOrWhiteSpace(producto.Nombre))
            return OperationResult<int>.Fallido("El nombre del producto es obligatorio.");

        if (producto.Precio <= 0)
            return OperationResult<int>.Fallido("El precio debe ser mayor a cero.");

        producto.CreadoEn = DateTime.UtcNow;
        _contexto.Productos.Add(producto);
        await _contexto.SaveChangesAsync();

        return OperationResult<int>.Exitoso(producto.Id);
    }

    /// <summary>Actualiza los datos de un producto existente.</summary>
    /// <param name="producto">El producto con los datos actualizados.</param>
    /// <returns>Resultado de la operación de actualización.</returns>
    public async Task<OperationResult> ActualizarAsync(Producto producto)
    {
        var existente = await _contexto.Productos.FindAsync(producto.Id);
        if (existente is null)
            return OperationResult.Fallido($"Producto con Id {producto.Id} no encontrado.");

        if (string.IsNullOrWhiteSpace(producto.Nombre))
            return OperationResult.Fallido("El nombre del producto es obligatorio.");

        if (producto.Precio <= 0)
            return OperationResult.Fallido("El precio debe ser mayor a cero.");

        existente.Nombre = producto.Nombre;
        existente.Descripcion = producto.Descripcion;
        existente.Precio = producto.Precio;
        existente.Categoria = producto.Categoria;
        existente.EstaDisponible = producto.EstaDisponible;
        existente.EsCombo = producto.EsCombo;
        existente.ComponentesCombo = producto.ComponentesCombo;
        existente.ActualizadoEn = DateTime.UtcNow;

        await _contexto.SaveChangesAsync();
        return OperationResult.Exitoso();
    }

    /// <summary>Elimina un producto del catálogo por su identificador.</summary>
    /// <param name="id">Identificador del producto a eliminar.</param>
    /// <returns>Resultado de la operación de eliminación.</returns>
    public async Task<OperationResult> EliminarAsync(int id)
    {
        var producto = await _contexto.Productos.FindAsync(id);
        if (producto is null)
            return OperationResult.Fallido($"Producto con Id {id} no encontrado.");

        _contexto.Productos.Remove(producto);
        await _contexto.SaveChangesAsync();
        return OperationResult.Exitoso();
    }

    /// <summary>Cambia la disponibilidad de un producto.</summary>
    /// <param name="id">Identificador del producto.</param>
    /// <param name="disponible">El nuevo estado de disponibilidad.</param>
    /// <returns>Resultado de la operación.</returns>
    public async Task<OperationResult> CambiarDisponibilidadAsync(int id, bool disponible)
    {
        var producto = await _contexto.Productos.FindAsync(id);
        if (producto is null)
            return OperationResult.Fallido($"Producto con Id {id} no encontrado.");

        producto.EstaDisponible = disponible;
        producto.ActualizadoEn = DateTime.UtcNow;
        await _contexto.SaveChangesAsync();
        return OperationResult.Exitoso();
    }
}
