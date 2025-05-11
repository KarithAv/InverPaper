using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InverPaper.Servicios;
using iTextSharp.text;
using iTextSharp.text.pdf;
using InverPaper.Repositorios;
using InverPaper.Utilidades;

namespace InverPaper.Controllers
{
    public class ReporteController : Controller
    {
        VentaServicio _ventaServicio = new VentaServicio();
        public ActionResult Reportes ()
        {
            return View();
        }
        public ActionResult GenerarReporte(DateTime? fecha, string reporte)
        {
            var fechaReporte = fecha ?? DateTime.Now.Date;

            if (reporte == "ventas")
            {
                return ReporteDiario(fechaReporte);
            }
            else if (reporte == "producto")
            {
                return ReporteProductoMasVendido(fechaReporte);
            }

            return RedirectToAction("Reportes");
        }

        // GET: Reporte
        public ActionResult ReporteDiario(DateTime fechaReporte)
        {
            var ventas = _ventaServicio.ObtenerVentasDelDia(fechaReporte);
            var detalleRepo = new DetalleVentaRepositorio();
            if (ventas == null || !ventas.Any())
            {
                TempData["Mensaje"] = "No se encontraron ventas en la fecha seleccionada.";
                return RedirectToAction("Reportes");
            }

            MemoryStream ms = new MemoryStream();
            Document doc = new Document(PageSize.A4, 25, 25, 30, 30);
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);
            writer.CloseStream = false;

            doc.Open();
            EncabezadoPdfUtilidad.AgregarEncabezado(doc, writer, "REPORTE DE VENTAS", fechaReporte);

            decimal totalGeneral = 0;

            foreach (var venta in ventas)
            {
                doc.Add(new Paragraph($"Venta ID: {venta.Id} - Usuario: {venta.NombreUsuario} - Fecha: {venta.FechaVenta:dd/MM/yyyy}", FontFactory.GetFont("Arial", 12, Font.BOLD)));
                doc.Add(new Paragraph(" "));

                PdfPTable tablaDetalle = new PdfPTable(3) { WidthPercentage = 100 };
                tablaDetalle.AddCell("Producto");
                tablaDetalle.AddCell("Cantidad");
                tablaDetalle.AddCell("Subtotal");

                var detalles = detalleRepo.ObtenerDetallesPorIdVenta(venta.Id);
                foreach (var d in detalles)
                {
                    tablaDetalle.AddCell(d.NombreProducto);
                    tablaDetalle.AddCell(d.Cantidad.ToString());
                    tablaDetalle.AddCell(d.Subtotal.ToString("C"));
                }

                doc.Add(tablaDetalle);
                doc.Add(new Paragraph($"Total Venta: {venta.Total:C}", FontFactory.GetFont("Arial", 11, Font.BOLD)));
                doc.Add(new Paragraph(" "));
                doc.Add(new Paragraph("--------------------------------------------------------------------------------"));
                doc.Add(new Paragraph(" "));

                totalGeneral += venta.Total;
            }

            doc.Add(new Paragraph($"TOTAL GENERAL DEL DÍA: {totalGeneral:C}", FontFactory.GetFont("Arial", 12, Font.BOLD)));
            doc.Close();

            ms.Position = 0;
            return File(ms, "application/pdf", $"ReporteVentas_{fechaReporte:yyyyMMdd}.pdf");
        }
        public ActionResult ReporteProductoMasVendido(DateTime fechaReporte)
        {
            var productosMasVendidos = _ventaServicio.ObtenerProductosMasVendidosDelDia(fechaReporte);

            if (productosMasVendidos == null || productosMasVendidos.Count == 0)
            {
                TempData["Mensaje"] = "No se encontró ningún producto vendido en la fecha seleccionada.";
                return RedirectToAction("Reportes");
            }

            // Extraer datos para el gráfico
            string[] categorias = productosMasVendidos.Select(p => p.Nombre).ToArray();
            int[] valores = productosMasVendidos.Select(p => p.CantidadVendida).ToArray();

            // Generar el gráfico de barras como imagen (byte[])
            byte[] imagenGrafico = GraficoUtilidad.GenerarGraficoBarras("Top 5 Productos Más Vendidos", categorias, valores);

            // Crear PDF
            MemoryStream ms = new MemoryStream();
            Document doc = new Document(PageSize.A4, 25, 25, 30, 30);
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);
            writer.CloseStream = false;

            doc.Open();
            EncabezadoPdfUtilidad.AgregarEncabezado(doc, writer, "REPORTE PRODUCTO MÁS VENDIDO", fechaReporte);

            // Agregar gráfico al PDF
            iTextSharp.text.Image grafico = iTextSharp.text.Image.GetInstance(imagenGrafico);
            grafico.ScaleToFit(500f, 300f);
            grafico.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
            doc.Add(grafico);

            doc.Add(new Paragraph(" ")); // Espacio

            // Agregar lista de productos
            foreach (var producto in productosMasVendidos)
            {
                doc.Add(new Paragraph($"Producto: {producto.Nombre}", FontFactory.GetFont("Arial", 12, Font.BOLD)));
                doc.Add(new Paragraph($"Cantidad Vendida: {producto.CantidadVendida}", FontFactory.GetFont("Arial", 12, Font.NORMAL)));
                doc.Add(new Paragraph(" "));
            }

            doc.Close();
            ms.Position = 0;

            return File(ms, "application/pdf", $"ReporteProductoMasVendido_{fechaReporte:yyyyMMdd}.pdf");
        }

    }
}