using DMendez.Data.Entities;
using DMendez.Data.Repositories.Interfaces;
using DMendez.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMendez.Web.Controllers.Admin;

/// <summary>Controlador administrativo para la gestión de promociones.</summary>
[Authorize(Roles = "Administrador")]
[Route("Admin/Promociones")]
public class PromocionesAdminController : Controller
{
    private readonly IRepositorioPromocion _repositorioPromocion;

    /// <summary>Inicializa el controlador con el repositorio de promociones.</summary>
    /// <param name="repositorioPromocion">El repositorio de promociones.</param>
    public PromocionesAdminController(IRepositorioPromocion repositorioPromocion)
    {
        _repositorioPromocion = repositorioPromocion;
    }

    /// <summary>Lista todas las promociones registradas.</summary>
    /// <returns>Vista con la lista de promociones.</returns>
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var resultado = await _repositorioPromocion.ObtenerTodasAsync();

        if (!resultado.EsExitoso)
        {
            TempData["MensajeError"] = resultado.MensajeError;
            return View(Enumerable.Empty<Promocion>());
        }

        return View(resultado.Valor);
    }

    /// <summary>Muestra el formulario para crear una nueva promoción.</summary>
    /// <returns>Vista del formulario de creación.</returns>
    [HttpGet("Crear")]
    public IActionResult Crear() => View(new PromocionViewModel());

    /// <summary>Procesa la creación de una nueva promoción.</summary>
    /// <param name="modelo">Los datos de la promoción a crear.</param>
    /// <returns>Redirección al listado si fue exitoso, o la vista con errores.</returns>
    [HttpPost("Crear")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(PromocionViewModel modelo)
    {
        if (!ModelState.IsValid)
            return View(modelo);

        var promocion = new Promocion
        {
            Nombre = modelo.Nombre,
            Codigo = modelo.Codigo,
            TipoDescuento = modelo.TipoDescuento,
            ValorDescuento = modelo.ValorDescuento,
            FechaInicio = modelo.FechaInicio,
            FechaFin = modelo.FechaFin,
            EstaActiva = modelo.EstaActiva
        };

        var resultado = await _repositorioPromocion.CrearAsync(promocion);

        if (!resultado.EsExitoso)
        {
            ModelState.AddModelError(string.Empty, resultado.MensajeError!);
            return View(modelo);
        }

        TempData["MensajeExito"] = $"Promoción '{modelo.Nombre}' creada exitosamente.";
        return RedirectToAction("Index");
    }

    /// <summary>Muestra el formulario de edición de una promoción.</summary>
    /// <param name="id">Identificador de la promoción.</param>
    /// <returns>Vista del formulario de edición o NotFound.</returns>
    [HttpGet("Editar/{id}")]
    public async Task<IActionResult> Editar(int id)
    {
        var resultado = await _repositorioPromocion.ObtenerPorIdAsync(id);

        if (!resultado.EsExitoso)
            return NotFound();

        var p = resultado.Valor!;
        var modelo = new PromocionViewModel
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Codigo = p.Codigo,
            TipoDescuento = p.TipoDescuento,
            ValorDescuento = p.ValorDescuento,
            FechaInicio = p.FechaInicio,
            FechaFin = p.FechaFin,
            EstaActiva = p.EstaActiva
        };

        return View(modelo);
    }

    /// <summary>Procesa la actualización de una promoción existente.</summary>
    /// <param name="id">Identificador de la promoción.</param>
    /// <param name="modelo">Los datos actualizados.</param>
    /// <returns>Redirección al listado si fue exitoso, o la vista con errores.</returns>
    [HttpPost("Editar/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, PromocionViewModel modelo)
    {
        if (id != modelo.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(modelo);

        var promocion = new Promocion
        {
            Id = modelo.Id,
            Nombre = modelo.Nombre,
            Codigo = modelo.Codigo,
            TipoDescuento = modelo.TipoDescuento,
            ValorDescuento = modelo.ValorDescuento,
            FechaInicio = modelo.FechaInicio,
            FechaFin = modelo.FechaFin,
            EstaActiva = modelo.EstaActiva
        };

        var resultado = await _repositorioPromocion.ActualizarAsync(promocion);

        if (!resultado.EsExitoso)
        {
            ModelState.AddModelError(string.Empty, resultado.MensajeError!);
            return View(modelo);
        }

        TempData["MensajeExito"] = "Promoción actualizada exitosamente.";
        return RedirectToAction("Index");
    }

    /// <summary>Cambia el estado activo/inactivo de una promoción.</summary>
    /// <param name="id">Identificador de la promoción.</param>
    /// <param name="activa">El nuevo estado de activación.</param>
    /// <returns>Redirección al listado con mensaje de resultado.</returns>
    [HttpPost("CambiarEstado/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int id, bool activa)
    {
        var resultado = await _repositorioPromocion.CambiarEstadoAsync(id, activa);

        if (!resultado.EsExitoso)
            TempData["MensajeError"] = resultado.MensajeError;
        else
            TempData["MensajeExito"] = activa ? "Promoción activada." : "Promoción desactivada.";

        return RedirectToAction("Index");
    }
}
