using System.Text.Json;
using DMendez.Web.ViewModels;

namespace DMendez.Web.Filters;

/// <summary>
/// Utilidad estática para gestionar el carrito de compras almacenado en la sesión del usuario.
/// </summary>
public static class GestorCarritoSesion
{
    private const string ClaveCarrito = "carrito_usuario";

    /// <summary>Obtiene el carrito de la sesión actual.</summary>
    /// <param name="sesion">La sesión HTTP del usuario.</param>
    /// <returns>El carrito almacenado, o un carrito vacío si no existe.</returns>
    public static CarritoViewModel ObtenerCarrito(ISession sesion)
    {
        string? json = sesion.GetString(ClaveCarrito);
        if (string.IsNullOrEmpty(json))
            return new CarritoViewModel();

        return JsonSerializer.Deserialize<CarritoViewModel>(json) ?? new CarritoViewModel();
    }

    /// <summary>Guarda el carrito en la sesión actual.</summary>
    /// <param name="sesion">La sesión HTTP del usuario.</param>
    /// <param name="carrito">El carrito a guardar.</param>
    public static void GuardarCarrito(ISession sesion, CarritoViewModel carrito)
    {
        string json = JsonSerializer.Serialize(carrito);
        sesion.SetString(ClaveCarrito, json);
    }

    /// <summary>Elimina el carrito de la sesión actual.</summary>
    /// <param name="sesion">La sesión HTTP del usuario.</param>
    public static void LimpiarCarrito(ISession sesion)
    {
        sesion.Remove(ClaveCarrito);
    }
}
