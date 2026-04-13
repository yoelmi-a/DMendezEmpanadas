using DMendez.Data.Repositories.Interfaces;
using DMendez.Web.Filters;
using DMendez.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMendez.Web.Controllers;

/// <summary>Controlador para la gestión del carrito de compras del cliente.</summary>
[Authorize]
public class CarritoController : Controller
{
    private readonly IRepositorioProducto _repositorioProducto;

    /// <summary>Inicializa el controlador con el repositorio de productos.</summary>
    /// <param name="repositorioProducto">El repositorio de productos.</param>
    public CarritoController(IRepositorioProducto repositorioProducto)
    {
        _repositorioProducto = repositorioProducto;
    }

    /// <summary>Muestra el contenido actual del carrito.</summary>
    /// <returns>Vista del carrito con sus ítems y totales.</returns>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var carrito = GestorCarritoSesion.ObtenerCarrito(HttpContext.Session);

        // Validar disponibilidad de cada ítem en el carrito
        foreach (var item in carrito.Items)
        {
            var resultado = await _repositorioProducto.ObtenerPorIdAsync(item.IdProducto);
            item.EstaDisponible = resultado.EsExitoso && resultado.Valor!.EstaDisponible;
        }

        GestorCarritoSesion.GuardarCarrito(HttpContext.Session, carrito);
        return View(carrito);
    }

    /// <summary>Agrega un producto al carrito o incrementa su cantidad.</summary>
    /// <param name="idProducto">Identificador del producto a agregar.</param>
    /// <returns>Redirección al catálogo con mensaje de confirmación.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Agregar(int idProducto)
    {
        var resultado = await _repositorioProducto.ObtenerPorIdAsync(idProducto);

        if (!resultado.EsExitoso || !resultado.Valor!.EstaDisponible)
        {
            TempData["MensajeError"] = "El producto no está disponible.";
            return RedirectToAction("Index", "Catalogo");
        }

        var producto = resultado.Valor;
        var carrito = GestorCarritoSesion.ObtenerCarrito(HttpContext.Session);

        var itemExistente = carrito.Items.FirstOrDefault(i => i.IdProducto == idProducto);
        if (itemExistente is not null)
        {
            itemExistente.Cantidad++;
        }
        else
        {
            carrito.Items.Add(new ItemCarritoViewModel
            {
                IdProducto = producto.Id,
                NombreProducto = producto.Nombre,
                PrecioUnitario = producto.Precio,
                Cantidad = 1,
                EstaDisponible = true
            });
        }

        GestorCarritoSesion.GuardarCarrito(HttpContext.Session, carrito);
        TempData["MensajeExito"] = $"'{producto.Nombre}' agregado al carrito.";
        return RedirectToAction("Index", "Catalogo");
    }

    /// <summary>Actualiza la cantidad de un producto en el carrito.</summary>
    /// <param name="idProducto">Identificador del producto.</param>
    /// <param name="cantidad">Nueva cantidad (0 elimina el ítem).</param>
    /// <returns>Redirección al carrito.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ActualizarCantidad(int idProducto, int cantidad)
    {
        var carrito = GestorCarritoSesion.ObtenerCarrito(HttpContext.Session);
        var item = carrito.Items.FirstOrDefault(i => i.IdProducto == idProducto);

        if (item is not null)
        {
            if (cantidad <= 0)
                carrito.Items.Remove(item);
            else
                item.Cantidad = cantidad;

            GestorCarritoSesion.GuardarCarrito(HttpContext.Session, carrito);
        }

        return RedirectToAction("Index");
    }

    /// <summary>Elimina un producto del carrito.</summary>
    /// <param name="idProducto">Identificador del producto a eliminar.</param>
    /// <returns>Redirección al carrito.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Eliminar(int idProducto)
    {
        var carrito = GestorCarritoSesion.ObtenerCarrito(HttpContext.Session);
        var item = carrito.Items.FirstOrDefault(i => i.IdProducto == idProducto);

        if (item is not null)
        {
            carrito.Items.Remove(item);
            GestorCarritoSesion.GuardarCarrito(HttpContext.Session, carrito);
        }

        return RedirectToAction("Index");
    }
}
