using DMendez.Web.Filters;
using DMendez.Web.Seed;
using DMendez.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DMendez.Web.Controllers;

/// <summary>Controlador para las operaciones de autenticación y registro de usuarios.</summary>
public class CuentaController : Controller
{
    private readonly UserManager<IdentityUser> _gestorUsuarios;
    private readonly SignInManager<IdentityUser> _gestorSesion;

    /// <summary>Inicializa el controlador con los gestores de Identity.</summary>
    /// <param name="gestorUsuarios">El gestor de usuarios de Identity.</param>
    /// <param name="gestorSesion">El gestor de inicio de sesión de Identity.</param>
    public CuentaController(
        UserManager<IdentityUser> gestorUsuarios,
        SignInManager<IdentityUser> gestorSesion)
    {
        _gestorUsuarios = gestorUsuarios;
        _gestorSesion = gestorSesion;
    }

    /// <summary>Muestra el formulario de registro.</summary>
    /// <returns>Vista de registro.</returns>
    [HttpGet]
    public IActionResult Registrar() => View();

    /// <summary>Procesa el registro de un nuevo cliente.</summary>
    /// <param name="modelo">Los datos del formulario de registro.</param>
    /// <returns>Redirección al inicio si el registro fue exitoso, o la vista con errores.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registrar(RegistroViewModel modelo)
    {
        if (!ModelState.IsValid)
            return View(modelo);

        var usuario = new IdentityUser
        {
            UserName = modelo.Email,
            Email = modelo.Email,
            EmailConfirmed = true
        };

        var resultado = await _gestorUsuarios.CreateAsync(usuario, modelo.Password);

        if (!resultado.Succeeded)
        {
            foreach (var error in resultado.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View(modelo);
        }

        await _gestorUsuarios.AddToRoleAsync(usuario, SemillaAdminPorDefecto.NombreRolCliente);
        await _gestorSesion.SignInAsync(usuario, isPersistent: false);

        TempData["MensajeExito"] = "¡Bienvenido a D' Méndez Empanadas!";
        return RedirectToAction("Index", "Catalogo");
    }

    /// <summary>Muestra el formulario de inicio de sesión.</summary>
    /// <param name="urlRetorno">URL a la que redirigir después del login.</param>
    /// <returns>Vista de inicio de sesión.</returns>
    [HttpGet]
    public IActionResult Iniciar(string? urlRetorno = null)
    {
        ViewData["UrlRetorno"] = urlRetorno;
        return View();
    }

    /// <summary>Procesa el inicio de sesión del usuario.</summary>
    /// <param name="modelo">Los datos del formulario de inicio de sesión.</param>
    /// <param name="urlRetorno">URL a la que redirigir después del login.</param>
    /// <returns>Redirección al destino o la vista con errores.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Iniciar(InicioSesionViewModel modelo, string? urlRetorno = null)
    {
        if (!ModelState.IsValid)
            return View(modelo);

        var resultado = await _gestorSesion.PasswordSignInAsync(
            modelo.Email, modelo.Password, modelo.Recordarme, lockoutOnFailure: false);

        if (!resultado.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Correo electrónico o contraseña incorrectos.");
            return View(modelo);
        }

        if (!string.IsNullOrEmpty(urlRetorno) && Url.IsLocalUrl(urlRetorno))
            return Redirect(urlRetorno);

        return RedirectToAction("Index", "Catalogo");
    }

    /// <summary>Cierra la sesión del usuario actual.</summary>
    /// <returns>Redirección a la página de inicio del catálogo.</returns>
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cerrar()
    {
        GestorCarritoSesion.LimpiarCarrito(HttpContext.Session);
        await _gestorSesion.SignOutAsync();
        return RedirectToAction("Index", "Catalogo");
    }

    /// <summary>Muestra la página de acceso denegado.</summary>
    /// <returns>Vista de acceso denegado.</returns>
    [HttpGet]
    public IActionResult AccesoDenegado() => View();
}
