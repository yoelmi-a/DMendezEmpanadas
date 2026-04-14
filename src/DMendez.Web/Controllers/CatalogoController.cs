using DMendez.Data.Entities;
using DMendez.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DMendez.Web.Controllers;

/// <summary>Controlador del catálogo de productos accesible para todos los visitantes.</summary>
public class CatalogoController : Controller
{
    private readonly IRepositorioProducto _repositorioProducto;

    /// <summary>Inicializa el controlador con el repositorio de productos.</summary>
    /// <param name="repositorioProducto">El repositorio de productos.</param>
    public CatalogoController(IRepositorioProducto repositorioProducto)
    {
        _repositorioProducto = repositorioProducto;
    }
//no
    /// <summary>Muestra el catálogo completo de productos disponibles.</summary>
    /// <param name="categoria">Categoría opcional para filtrar productos.</param>
    /// <returns>Vista del catálogo con los productos disponibles.</returns>
    [HttpGet]
    public async Task<IActionResult> Index(CategoriaProducto? categoria = null)
    {
        if (categoria.HasValue)
        {
            var resultadoFiltrado = await _repositorioProducto.ObtenerPorCategoriaAsync(categoria.Value);
            if (!resultadoFiltrado.EsExitoso)
            {
                TempData["MensajeError"] = resultadoFiltrado.MensajeError;
                return View(Enumerable.Empty<Producto>());
            }
            ViewBag.CategoriaActiva = categoria.Value;
            return View(resultadoFiltrado.Valor);
        }

        var resultado = await _repositorioProducto.ObtenerDisponiblesAsync();
        if (!resultado.EsExitoso)
        {
            TempData["MensajeError"] = resultado.MensajeError;
            return View(Enumerable.Empty<Producto>());
        }

        ViewBag.CategoriaActiva = (CategoriaProducto?)null;
        return View(resultado.Valor);
    }

    /// <summary>Muestra el detalle de un producto específico.</summary>
    /// <param name="id">Identificador del producto.</param>
    /// <returns>Vista de detalle del producto o NotFound si no existe.</returns>
    [HttpGet]
    public async Task<IActionResult> Detalle(int id)
    {
        var resultado = await _repositorioProducto.ObtenerPorIdAsync(id);

        if (!resultado.EsExitoso || !resultado.Valor!.EstaDisponible)
            return NotFound();

        return View(resultado.Valor);
    }
}
