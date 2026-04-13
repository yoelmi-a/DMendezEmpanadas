using DMendez.Data.Common;
using DMendez.Data.Context;
using DMendez.Data.Entities;
using DMendez.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DMendez.Data.Repositories;

/// <summary>Implementación del repositorio de promociones usando EF Core InMemory.</summary>
public class RepositorioPromocion : IRepositorioPromocion
{
    private readonly AppDbContext _contexto;

    /// <summary>Inicializa el repositorio con el contexto de base de datos.</summary>
    /// <param name="contexto">El contexto de base de datos de la aplicación.</param>
    public RepositorioPromocion(AppDbContext contexto) => _contexto = contexto;

    /// <summary>Obtiene todas las promociones registradas.</summary>
    /// <returns>Lista completa de promociones.</returns>
    public async Task<OperationResult<IEnumerable<Promocion>>> ObtenerTodasAsync()
    {
        var promociones = await _contexto.Promociones
            .OrderByDescending(p => p.CreadoEn)
            .ToListAsync();

        return OperationResult<IEnumerable<Promocion>>.Exitoso(promociones);
    }

    /// <summary>Obtiene una promoción por su identificador.</summary>
    /// <param name="id">Identificador de la promoción.</param>
    /// <returns>La promoción encontrada o un error si no existe.</returns>
    public async Task<OperationResult<Promocion>> ObtenerPorIdAsync(int id)
    {
        var promocion = await _contexto.Promociones.FindAsync(id);

        return promocion is null
            ? OperationResult<Promocion>.Fallido($"Promoción con Id {id} no encontrada.")
            : OperationResult<Promocion>.Exitoso(promocion);
    }

    /// <summary>Obtiene una promoción válida a partir de su código.</summary>
    /// <param name="codigo">Código de la promoción a validar.</param>
    /// <returns>La promoción si existe y es válida en la fecha actual.</returns>
    public async Task<OperationResult<Promocion>> ObtenerPorCodigoValidoAsync(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            return OperationResult<Promocion>.Fallido("El código de promoción no puede estar vacío.");

        var ahora = DateTime.UtcNow;
        var promocion = await _contexto.Promociones
            .FirstOrDefaultAsync(p => p.Codigo == codigo.Trim().ToUpper());

        if (promocion is null)
            return OperationResult<Promocion>.Fallido("El código de promoción no existe.");

        if (!promocion.EsValidaEn(ahora))
            return OperationResult<Promocion>.Fallido("El código de promoción ha expirado o está inactivo.");

        return OperationResult<Promocion>.Exitoso(promocion);
    }

    /// <summary>Crea una nueva promoción.</summary>
    /// <param name="promocion">La promoción a crear.</param>
    /// <returns>El identificador de la promoción creada.</returns>
    public async Task<OperationResult<int>> CrearAsync(Promocion promocion)
    {
        if (string.IsNullOrWhiteSpace(promocion.Nombre))
            return OperationResult<int>.Fallido("El nombre de la promoción es obligatorio.");

        if (string.IsNullOrWhiteSpace(promocion.Codigo))
            return OperationResult<int>.Fallido("El código de la promoción es obligatorio.");

        if (promocion.FechaFin < promocion.FechaInicio)
            return OperationResult<int>.Fallido("La fecha de fin debe ser posterior a la fecha de inicio.");

        if (promocion.ValorDescuento <= 0)
            return OperationResult<int>.Fallido("El valor del descuento debe ser mayor a cero.");

        promocion.Codigo = promocion.Codigo.Trim().ToUpper();
        promocion.CreadoEn = DateTime.UtcNow;

        bool codigoExiste = await _contexto.Promociones
            .AnyAsync(p => p.Codigo == promocion.Codigo);

        if (codigoExiste)
            return OperationResult<int>.Fallido($"Ya existe una promoción con el código '{promocion.Codigo}'.");

        _contexto.Promociones.Add(promocion);
        await _contexto.SaveChangesAsync();

        return OperationResult<int>.Exitoso(promocion.Id);
    }

    /// <summary>Actualiza una promoción existente.</summary>
    /// <param name="promocion">La promoción con los datos actualizados.</param>
    /// <returns>Resultado de la operación de actualización.</returns>
    public async Task<OperationResult> ActualizarAsync(Promocion promocion)
    {
        var existente = await _contexto.Promociones.FindAsync(promocion.Id);
        if (existente is null)
            return OperationResult.Fallido($"Promoción con Id {promocion.Id} no encontrada.");

        if (promocion.FechaFin < promocion.FechaInicio)
            return OperationResult.Fallido("La fecha de fin debe ser posterior a la fecha de inicio.");

        existente.Nombre = promocion.Nombre;
        existente.Codigo = promocion.Codigo.Trim().ToUpper();
        existente.TipoDescuento = promocion.TipoDescuento;
        existente.ValorDescuento = promocion.ValorDescuento;
        existente.FechaInicio = promocion.FechaInicio;
        existente.FechaFin = promocion.FechaFin;
        existente.EstaActiva = promocion.EstaActiva;
        existente.ActualizadoEn = DateTime.UtcNow;

        await _contexto.SaveChangesAsync();
        return OperationResult.Exitoso();
    }

    /// <summary>Cambia el estado activo/inactivo de una promoción.</summary>
    /// <param name="id">Identificador de la promoción.</param>
    /// <param name="activa">El nuevo estado de activación.</param>
    /// <returns>Resultado de la operación.</returns>
    public async Task<OperationResult> CambiarEstadoAsync(int id, bool activa)
    {
        var promocion = await _contexto.Promociones.FindAsync(id);
        if (promocion is null)
            return OperationResult.Fallido($"Promoción con Id {id} no encontrada.");

        promocion.EstaActiva = activa;
        promocion.ActualizadoEn = DateTime.UtcNow;
        await _contexto.SaveChangesAsync();
        return OperationResult.Exitoso();
    }
}
