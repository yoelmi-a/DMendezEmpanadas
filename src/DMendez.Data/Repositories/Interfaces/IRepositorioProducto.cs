using DMendez.Data.Common;
using DMendez.Data.Entities;

namespace DMendez.Data.Repositories.Interfaces;

/// <summary>Contrato para el repositorio de productos del catálogo.</summary>
public interface IRepositorioProducto
{
    /// <summary>Obtiene todos los productos disponibles en el catálogo.</summary>
    /// <returns>Lista de productos disponibles.</returns>
    Task<OperationResult<IEnumerable<Producto>>> ObtenerDisponiblesAsync();

    /// <summary>Obtiene todos los productos, incluyendo los no disponibles (uso administrativo).</summary>
    /// <returns>Lista completa de productos.</returns>
    Task<OperationResult<IEnumerable<Producto>>> ObtenerTodosAsync();

    /// <summary>Obtiene un producto por su identificador.</summary>
    /// <param name="id">Identificador del producto.</param>
    /// <returns>El producto encontrado o un error si no existe.</returns>
    Task<OperationResult<Producto>> ObtenerPorIdAsync(int id);

    /// <summary>Obtiene productos filtrados por categoría.</summary>
    /// <param name="categoria">Categoría a filtrar.</param>
    /// <returns>Lista de productos de la categoría especificada.</returns>
    Task<OperationResult<IEnumerable<Producto>>> ObtenerPorCategoriaAsync(CategoriaProducto categoria);

    /// <summary>Agrega un nuevo producto al catálogo.</summary>
    /// <param name="producto">El producto a agregar.</param>
    /// <returns>El identificador del producto creado.</returns>
    Task<OperationResult<int>> AgregarAsync(Producto producto);

    /// <summary>Actualiza los datos de un producto existente.</summary>
    /// <param name="producto">El producto con los datos actualizados.</param>
    /// <returns>Resultado de la operación de actualización.</returns>
    Task<OperationResult> ActualizarAsync(Producto producto);

    /// <summary>Elimina un producto del catálogo por su identificador.</summary>
    /// <param name="id">Identificador del producto a eliminar.</param>
    /// <returns>Resultado de la operación de eliminación.</returns>
    Task<OperationResult> EliminarAsync(int id);

    /// <summary>Cambia la disponibilidad de un producto.</summary>
    /// <param name="id">Identificador del producto.</param>
    /// <param name="disponible">El nuevo estado de disponibilidad.</param>
    /// <returns>Resultado de la operación.</returns>
    Task<OperationResult> CambiarDisponibilidadAsync(int id, bool disponible);
}
