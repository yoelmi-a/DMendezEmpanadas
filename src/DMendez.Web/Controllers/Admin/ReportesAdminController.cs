using DMendez.Data.Repositories.Interfaces;
using DMendez.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMendez.Web.Controllers.Admin;

/// <summary>Controlador administrativo para la visualización de reportes de ventas.</summary>
[Authorize(Roles = "Administrador")]
[Route("Admin/Reportes")]
public class ReportesAdminController : Controller
{
    private readonly IRepositorioPedido _repositorioPedido;

    /// <summary>Inicializa el controlador con el repositorio de pedidos.</summary>
    /// <param name="repositorioPedido">El repositorio de pedidos.</param>
    public ReportesAdminController(IRepositorioPedido repositorioPedido)
    {
        _repositorioPedido = repositorioPedido;
    }

    /// <summary>Muestra el reporte de ventas con datos agregados por período.</summary>
    /// <param name="modelo">Los parámetros de filtro del reporte.</param>
    /// <returns>Vista con los datos del reporte.</returns>
    [HttpGet("")]
    public async Task<IActionResult> Index(ReporteVentasViewModel? modelo)
    {
        modelo ??= new ReporteVentasViewModel();

        var resultado = await _repositorioPedido.ObtenerParaReporteAsync(
            modelo.Desde.ToUniversalTime(),
            modelo.Hasta.AddDays(1).ToUniversalTime());

        if (!resultado.EsExitoso)
        {
            TempData["MensajeError"] = resultado.MensajeError;
            return View(modelo);
        }

        var pedidos = resultado.Valor!.ToList();

        // Agrupar según el período seleccionado
        modelo.Filas = modelo.Periodo switch
        {
            "semana" => pedidos
                .GroupBy(p => $"Semana {System.Globalization.ISOWeek.GetWeekOfYear(p.CreadoEn)}/{p.CreadoEn.Year}")
                .Select(g => new FilaReporteViewModel
                {
                    Etiqueta = g.Key,
                    CantidadPedidos = g.Count(),
                    TotalVentas = g.Sum(p => p.Total)
                }).ToList(),

            "mes" => pedidos
                .GroupBy(p => p.CreadoEn.ToString("MMMM yyyy",
                    new System.Globalization.CultureInfo("es-DO")))
                .Select(g => new FilaReporteViewModel
                {
                    Etiqueta = g.Key,
                    CantidadPedidos = g.Count(),
                    TotalVentas = g.Sum(p => p.Total)
                }).ToList(),

            _ => pedidos
                .GroupBy(p => p.CreadoEn.ToString("dd/MM/yyyy"))
                .Select(g => new FilaReporteViewModel
                {
                    Etiqueta = g.Key,
                    CantidadPedidos = g.Count(),
                    TotalVentas = g.Sum(p => p.Total)
                }).ToList()
        };

        return View(modelo);
    }
}
