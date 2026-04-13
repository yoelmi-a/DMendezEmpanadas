using DMendez.Data.Entities;
using DMendez.Data.Repositories.Interfaces;
using DMendez.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMendez.Web.Controllers.Admin;

/// <summary>Controlador administrativo para la gestión CRUD de productos.</summary>
[Authorize(Roles = "Administrador")]
[Route("Admin/Productos")]
public class ProductosAdminController : Controller
{
    private readonly IRepositorioProducto _repositorioProducto;

    /// <summary>Inicializa el controlador con el repositorio de productos.</summary>
    /// <param name="repositorioProducto">El repositorio de productos.</param>
    public ProductosAdminController(IRepositorioProducto repositorioProducto)
    {
        _repositorioProducto = repositorioProducto;
    }

    /// <summary>Lista todos los productos del catálogo.</summary>
    /// <returns>Vista con la lista completa de productos.</returns>
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var resultado = await _repositorioProducto.ObtenerTodosAsync();

        if (!resultado.EsExitoso)
        {
            TempData["MensajeError"] = resultado.MensajeError;
            return View(Enumerable.Empty<Producto>());
        }

        return View(resultado.Valor);
    }

    /// <summary>Muestra el formulario para crear un nuevo producto.</summary>
    /// <returns>Vista del formulario de creación.</returns>
    [HttpGet("Crear")]
    public IActionResult Crear() => View(new ProductoViewModel());

    /// <summary>Procesa la creación de un nuevo producto.</summary>
    /// <param name="modelo">Los datos del producto a crear.</param>
    /// <returns>Redirección al listado si fue exitoso, o la vista con errores.</returns>
    [HttpPost("Crear")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(ProductoViewModel modelo)
    {
        if (!ModelState.IsValid)
            return View(modelo);

        var producto = new Producto
        {
            Nombre = modelo.Nombre,
            Descripcion = modelo.Descripcion,
            Precio = modelo.Precio,
            Categoria = modelo.Categoria,
            EstaDisponible = modelo.EstaDisponible,
            EsCombo = modelo.EsCombo,
            ComponentesCombo = modelo.ComponentesCombo
        };

        var resultado = await _repositorioProducto.AgregarAsync(producto);

        if (!resultado.EsExitoso)
        {
            ModelState.AddModelError(string.Empty, resultado.MensajeError!);
            return View(modelo);
        }

        TempData["MensajeExito"] = $"Producto '{modelo.Nombre}' creado exitosamente.";
        return RedirectToAction("Index");
    }

    /// <summary>Muestra el formulario de edición de un producto.</summary>
    /// <param name="id">Identificador del producto.</param>
    /// <returns>Vista del formulario de edición o NotFound.</returns>
    [HttpGet("Editar/{id}")]
    public async Task<IActionResult> Editar(int id)
    {
        var resultado = await _repositorioProducto.ObtenerPorIdAsync(id);

        if (!resultado.EsExitoso)
            return NotFound();

        var producto = resultado.Valor!;
        var modelo = new ProductoViewModel
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            Precio = producto.Precio,
            Categoria = producto.Categoria,
            EstaDisponible = producto.EstaDisponible,
            EsCombo = producto.EsCombo,
            ComponentesCombo = producto.ComponentesCombo
        };

        return View(modelo);
    }

    /// <summary>Procesa la actualización de un producto existente.</summary>
    /// <param name="id">Identificador del producto.</param>
    /// <param name="modelo">Los datos actualizados del producto.</param>
    /// <returns>Redirección al listado si fue exitoso, o la vista con errores.</returns>
    [HttpPost("Editar/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, ProductoViewModel modelo)
    {
        if (id != modelo.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(modelo);

        var producto = new Producto
        {
            Id = modelo.Id,
            Nombre = modelo.Nombre,
            Descripcion = modelo.Descripcion,
            Precio = modelo.Precio,
            Categoria = modelo.Categoria,
            EstaDisponible = modelo.EstaDisponible,
            EsCombo = modelo.EsCombo,
            ComponentesCombo = modelo.ComponentesCombo
        };

        var resultado = await _repositorioProducto.ActualizarAsync(producto);

        if (!resultado.EsExitoso)
        {
            ModelState.AddModelError(string.Empty, resultado.MensajeError!);
            return View(modelo);
        }

        TempData["MensajeExito"] = $"Producto '{modelo.Nombre}' actualizado exitosamente.";
        return RedirectToAction("Index");
    }

    /// <summary>Elimina un producto del catálogo.</summary>
    /// <param name="id">Identificador del producto a eliminar.</param>
    /// <returns>Redirección al listado con mensaje de resultado.</returns>
    [HttpPost("Eliminar/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        var resultado = await _repositorioProducto.EliminarAsync(id);

        if (!resultado.EsExitoso)
            TempData["MensajeError"] = resultado.MensajeError;
        else
            TempData["MensajeExito"] = "Producto eliminado exitosamente.";

        return RedirectToAction("Index");
    }

    /// <summary>Cambia la disponibilidad de un producto.</summary>
    /// <param name="id">Identificador del producto.</param>
    /// <param name="disponible">El nuevo estado de disponibilidad.</param>
    /// <returns>Redirección al listado con mensaje de resultado.</returns>
    [HttpPost("CambiarDisponibilidad/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarDisponibilidad(int id, bool disponible)
    {
        var resultado = await _repositorioProducto.CambiarDisponibilidadAsync(id, disponible);

        if (!resultado.EsExitoso)
            TempData["MensajeError"] = resultado.MensajeError;
        else
            TempData["MensajeExito"] = disponible
                ? "Producto marcado como disponible."
                : "Producto marcado como no disponible.";

        return RedirectToAction("Index");
    }
}
