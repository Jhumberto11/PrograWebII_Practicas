using Biblio.Data;
using Biblio.Models;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Biblio.Controllers
{
    [Authorize(Roles = "Administrador,Bibliotecario,Usuario")]
    public class ReportesController : Controller
    {
        private readonly BiblioContext _context;
        private readonly ILogger<ReportesController> _logger;

        public ReportesController(
            BiblioContext context,
            ILogger<ReportesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                _logger.LogInformation("Se abrió la sección de reportes de libros");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al abrir la sección de reportes");
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GenerarPdf(string autor, string categoria)
        {
            try
            {
                _logger.LogInformation("Se inició la generación del reporte PDF de libros");

                var libros = await ObtenerLibrosAsync(autor, categoria);

                if (libros.Count == 0)
                {
                    _logger.LogWarning("No se encontraron libros para incluir en el reporte PDF");
                }

                var documento = Document.Create(document =>
                {
                    document.Page(page =>
                    {
                        page.Size(PageSizes.A4.Landscape());
                        page.Margin(30);
                        page.DefaultTextStyle(x => x.FontSize(9));

                        page.Header().Column(columna =>
                        {
                            columna.Item()
                                .Text("BibliotecaMVC")
                                .FontSize(18)
                                .Bold()
                                .FontColor(Colors.Blue.Darken2);

                            columna.Item()
                                .Text("Reporte de libros")
                                .FontSize(14)
                                .SemiBold();

                            columna.Item()
                                .AlignRight()
                                .Text($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}");
                        });

                        page.Content()
                            .PaddingVertical(15)
                            .Column(columna =>
                            {
                                columna.Item()
                                    .Text($"Total de libros: {libros.Count}")
                                    .SemiBold();

                                columna.Item()
                                    .PaddingTop(10)
                                    .Table(tabla =>
                                    {
                                        tabla.ColumnsDefinition(columnas =>
                                        {
                                            columnas.ConstantColumn(45);
                                            columnas.RelativeColumn(3);
                                            columnas.RelativeColumn(2);
                                            columnas.RelativeColumn(2);
                                        });

                                        tabla.Header(encabezado =>
                                        {
                                            encabezado.Cell().Element(EstiloEncabezado).Text("ID").FontColor(Colors.White).Bold();
                                            encabezado.Cell().Element(EstiloEncabezado).Text("Nombre del libro").FontColor(Colors.White).Bold();
                                            encabezado.Cell().Element(EstiloEncabezado).Text("Autor").FontColor(Colors.White).Bold();
                                            encabezado.Cell().Element(EstiloEncabezado).Text("Categoría").FontColor(Colors.White).Bold();
                                        });

                                        foreach (var libro in libros)
                                        {
                                            tabla.Cell().Element(EstiloCelda).Text(libro.ID.ToString());
                                            tabla.Cell().Element(EstiloCelda).Text(libro.Title ?? "");
                                            tabla.Cell().Element(EstiloCelda).Text(libro.Autor ?? "");
                                            tabla.Cell().Element(EstiloCelda).Text(libro.Categoria ?? "");
                                        }
                                    });
                            });

                        page.Footer()
                            .AlignCenter()
                            .Text(texto =>
                            {
                                texto.Span("Página ");
                                texto.CurrentPageNumber();
                                texto.Span(" de ");
                                texto.TotalPages();
                            });
                    });
                });

                var contenidoPdf = documento.GeneratePdf();
                var nombreArchivo = $"ReporteLibros_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                _logger.LogInformation(
                    "Se generó correctamente el reporte PDF. Registros: {CantidadLibros}",
                    libros.Count);

                return File(contenidoPdf, "application/pdf", nombreArchivo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al generar el reporte PDF de libros");
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportarExcel(string autor, string categoria)
        {
            try
            {
                _logger.LogInformation("Se inició la exportación del reporte de libros a Excel");

                var libros = await ObtenerLibrosAsync(autor, categoria);

                if (libros.Count == 0)
                {
                    _logger.LogWarning("No se encontraron libros para incluir en el reporte Excel");
                }

                using (var workbook = new XLWorkbook())
                {
                    var hoja = workbook.Worksheets.Add("Reporte de Libros");

                    hoja.Cell("A1").Value = "BIBLIOTECAMVC";
                    hoja.Range("A1:D1").Merge();
                    hoja.Range("A1:D1").Style.Font.Bold = true;
                    hoja.Range("A1:D1").Style.Font.FontSize = 18;
                    hoja.Range("A1:D1").Style.Font.FontColor = XLColor.FromHtml("#1F4E78");

                    hoja.Cell("A2").Value = "Reporte de libros";
                    hoja.Range("A2:D2").Merge();
                    hoja.Range("A2:D2").Style.Font.Bold = true;

                    hoja.Cell("A3").Value = $"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}";
                    hoja.Range("A3:D3").Merge();

                    hoja.Cell("A4").Value = $"Total de libros: {libros.Count}";
                    hoja.Range("A4:D4").Merge();

                    hoja.Cell(6, 1).Value = "ID";
                    hoja.Cell(6, 2).Value = "Nombre del libro";
                    hoja.Cell(6, 3).Value = "Autor";
                    hoja.Cell(6, 4).Value = "Categoría";

                    var encabezados = hoja.Range(6, 1, 6, 4);
                    encabezados.Style.Font.Bold = true;
                    encabezados.Style.Font.FontColor = XLColor.White;
                    encabezados.Style.Fill.BackgroundColor = XLColor.FromHtml("#1F4E78");
                    encabezados.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    encabezados.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    encabezados.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    encabezados.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    var fila = 7;

                    foreach (var libro in libros)
                    {
                        hoja.Cell(fila, 1).Value = libro.ID;
                        hoja.Cell(fila, 2).Value = libro.Title ?? "";
                        hoja.Cell(fila, 3).Value = libro.Autor ?? "";
                        hoja.Cell(fila, 4).Value = libro.Categoria ?? "";

                        fila++;
                    }

                    if (libros.Count > 0)
                    {
                        var datos = hoja.Range(7, 1, fila - 1, 4);
                        datos.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        datos.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        datos.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    }

                    hoja.Range(6, 1, Math.Max(6, fila - 1), 4).SetAutoFilter();
                    hoja.SheetView.FreezeRows(6);
                    hoja.Columns().AdjustToContents();

                    hoja.Column(2).Width = Math.Max(hoja.Column(2).Width, 28);
                    hoja.Column(3).Width = Math.Max(hoja.Column(3).Width, 20);
                    hoja.Column(4).Width = Math.Max(hoja.Column(4).Width, 20);

                    using (var memoria = new MemoryStream())
                    {
                        workbook.SaveAs(memoria);

                        var contenidoExcel = memoria.ToArray();
                        var nombreArchivo = $"ReporteLibros_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                        _logger.LogInformation(
                            "Se exportó correctamente el reporte Excel. Registros: {CantidadLibros}",
                            libros.Count);

                        return File(
                            contenidoExcel,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            nombreArchivo);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al exportar el reporte de libros a Excel");
                throw;
            }
        }

        private async Task<List<Libro>> ObtenerLibrosAsync(string autor, string categoria)
        {
            var consulta = _context.Libros.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(autor))
            {
                consulta = consulta.Where(libro =>
                    libro.Autor != null && libro.Autor.Contains(autor));
            }

            if (!string.IsNullOrWhiteSpace(categoria))
            {
                consulta = consulta.Where(libro =>
                    libro.Categoria != null && libro.Categoria.Contains(categoria));
            }

            return await consulta
                .OrderBy(libro => libro.ID)
                .ToListAsync();
        }

        private static IContainer EstiloEncabezado(IContainer container)
        {
            return container
                .Background(Colors.Blue.Darken2)
                .Border(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(6);
        }

        private static IContainer EstiloCelda(IContainer container)
        {
            return container
                .Border(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(6);
        }
    }
}
