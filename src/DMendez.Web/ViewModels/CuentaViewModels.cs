using System.ComponentModel.DataAnnotations;

namespace DMendez.Web.ViewModels;

/// <summary>ViewModel para el formulario de registro de nuevos clientes.</summary>
public class RegistroViewModel
{
    /// <summary>Correo electrónico del nuevo usuario.</summary>
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    /// <summary>Contraseña del nuevo usuario.</summary>
    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    /// <summary>Confirmación de la contraseña del nuevo usuario.</summary>
    [Required(ErrorMessage = "La confirmación de contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmarPassword { get; set; } = string.Empty;
}

/// <summary>ViewModel para el formulario de inicio de sesión.</summary>
public class InicioSesionViewModel
{
    /// <summary>Correo electrónico del usuario.</summary>
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    /// <summary>Contraseña del usuario.</summary>
    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    /// <summary>Indica si se debe mantener la sesión activa al cerrar el navegador.</summary>
    [Display(Name = "Recordarme")]
    public bool Recordarme { get; set; }
}
