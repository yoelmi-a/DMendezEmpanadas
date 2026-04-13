using System.ComponentModel.DataAnnotations;
using DMendez.Data.Entities;

namespace DMendez.Web.ViewModels;

/// <summary>ViewModel para crear o editar un producto.</summary>
public class ProductoViewModel
{
    /// <summary>Identificador del producto (0 para nuevos).</summary>
    public int Id { get; set; }

    /// <summary>Nombre del producto.</summary>
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150, ErrorMessage = "El nombre no puede superar 150 caracteres.")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Descripción del producto.</summary>
    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(500, ErrorMessage = "La descripción no puede superar 500 caracteres.")]
    [Display(Name = "Descripción")]
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>Precio del producto.</summary>
    [Required(ErrorMessage = "El precio es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a cero.")]
    [Display(Name = "Precio (RD$)")]
    public decimal Precio { get; set; }

    /// <summary>Categoría del producto.</summary>
    [Required(ErrorMessage = "La categoría es obligatoria.")]
    [Display(Name = "Categoría")]
    public CategoriaProducto Categoria { get; set; }

    /// <summary>Indica si el producto está disponible.</summary>
    [Display(Name = "Disponible")]
    public bool EstaDisponible { get; set; } = true;

    /// <summary>Indica si el producto es un combo.</summary>
    [Display(Name = "Es combo")]
    public bool EsCombo { get; set; }

    /// <summary>Componentes del combo.</summary>
    [StringLength(500)]
    [Display(Name = "Componentes del combo")]
    public string? ComponentesCombo { get; set; }
}

/// <summary>ViewModel para crear o editar una promoción.</summary>
public class PromocionViewModel
{
    /// <summary>Identificador de la promoción (0 para nuevas).</summary>
    public int Id { get; set; }

    /// <summary>Nombre de la promoción.</summary>
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150)]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Código único de la promoción.</summary>
    [Required(ErrorMessage = "El código es obligatorio.")]
    [StringLength(50)]
    [Display(Name = "Código")]
    public string Codigo { get; set; } = string.Empty;

    /// <summary>Tipo de descuento.</summary>
    [Required]
    [Display(Name = "Tipo de descuento")]
    public TipoDescuento TipoDescuento { get; set; }

    /// <summary>Valor del descuento.</summary>
    [Required(ErrorMessage = "El valor del descuento es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El valor debe ser mayor a cero.")]
    [Display(Name = "Valor")]
    public decimal ValorDescuento { get; set; }

    /// <summary>Fecha de inicio de vigencia.</summary>
    [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
    [Display(Name = "Fecha de inicio")]
    [DataType(DataType.Date)]
    public DateTime FechaInicio { get; set; } = DateTime.Today;

    /// <summary>Fecha de fin de vigencia.</summary>
    [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
    [Display(Name = "Fecha de fin")]
    [DataType(DataType.Date)]
    public DateTime FechaFin { get; set; } = DateTime.Today.AddDays(30);

    /// <summary>Indica si la promoción está activa.</summary>
    [Display(Name = "Activa")]
    public bool EstaActiva { get; set; } = true;
}

/// <summary>ViewModel para el panel de reportes de ventas.</summary>
public class ReporteVentasViewModel
{
    /// <summary>Fecha de inicio del período del reporte.</summary>
    [Display(Name = "Desde")]
    [DataType(DataType.Date)]
    public DateTime Desde { get; set; } = DateTime.Today.AddDays(-30);

    /// <summary>Fecha de fin del período del reporte.</summary>
    [Display(Name = "Hasta")]
    [DataType(DataType.Date)]
    public DateTime Hasta { get; set; } = DateTime.Today;

    /// <summary>Período de agrupación seleccionado.</summary>
    [Display(Name = "Agrupar por")]
    public string Periodo { get; set; } = "dia";

    /// <summary>Datos agrupados para el reporte.</summary>
    public List<FilaReporteViewModel> Filas { get; set; } = new();

    /// <summary>Total general de ventas en el período.</summary>
    public decimal TotalGeneral => Filas.Sum(f => f.TotalVentas);

    /// <summary>Cantidad total de pedidos en el período.</summary>
    public int TotalPedidos => Filas.Sum(f => f.CantidadPedidos);
}

/// <summary>Fila individual de datos en el reporte de ventas.</summary>
public class FilaReporteViewModel
{
    /// <summary>Etiqueta del período (fecha o semana).</summary>
    public string Etiqueta { get; set; } = string.Empty;

    /// <summary>Cantidad de pedidos entregados en el período.</summary>
    public int CantidadPedidos { get; set; }

    /// <summary>Total de ventas en el período.</summary>
    public decimal TotalVentas { get; set; }
}
