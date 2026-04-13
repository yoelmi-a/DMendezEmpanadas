using DMendez.Data.Common;
using DMendez.Data.Entities;

namespace DMendez.Data.Repositories.Interfaces;

/// <summary>Contrato para el repositorio de promociones.</summary>
public interface IRepositorioPromocion
{
    /// <summary>Obtiene todas las promociones registradas.</summary>
    /// <returns>Lista completa de promociones.</returns>
    Task<OperationResult<IEnumerable<Promocion>>> ObtenerTodasAsync();

    /// <summary>Obtiene una promoción por su identificador.</summary>
    /// <param name="id">Identificador de la promoción.</param>
    /// <returns>La promoción encontrada o un error si no existe.</returns>
    Task<OperationResult<Promocion>> ObtenerPorIdAsync(int id);

    /// <summary>Obtiene una promoción válida a partir de su código.</summary>
    /// <param name="codigo">Código de la promoción.</param>
    /// <returns>La promoción si existe y es válida en la fecha actual.</returns>
    Task<OperationResult<Promocion>> ObtenerPorCodigoValidoAsync(string codigo);

    /// <summary>Crea una nueva promoción.</summary>
    /// <param name="promocion">La promoción a crear.</param>
    /// <returns>El identificador de la promoción creada.</returns>
    Task<OperationResult<int>> CrearAsync(Promocion promocion);

    /// <summary>Actualiza una promoción existente.</summary>
    /// <param name="promocion">La promoción con los datos actualizados.</param>
    /// <returns>Resultado de la operación de actualización.</returns>
    Task<OperationResult> ActualizarAsync(Promocion promocion);

    /// <summary>Cambia el estado activo/inactivo de una promoción.</summary>
    /// <param name="id">Identificador de la promoción.</param>
    /// <param name="activa">El nuevo estado de activación.</param>
    /// <returns>Resultado de la operación.</returns>
    Task<OperationResult> CambiarEstadoAsync(int id, bool activa);
}
