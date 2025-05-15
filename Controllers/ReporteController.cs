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
    [AutorizarSesionUtilidad]
    public class ReporteController : Controller
    {
        VentaServicio _ventaServicio = new VentaServicio();
        AjustesInventarioServicio _ajusteInventarioServicio = new AjustesInventarioServicio();

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
            else if (reporte == "ajustes")
            {
                return ReporteAjustesInventario(fechaReporte);
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
                PdfPTable tabla = new PdfPTable(2);
                tabla.WidthPercentage = 80;
                tabla.SpacingBefore = 10f;
                tabla.SpacingAfter = 10f;

                // Estilo del encabezado
                var fontEncabezado = FontFactory.GetFont("Arial", 12, Font.BOLD, BaseColor.WHITE);
                var fondoEncabezado = new BaseColor(255, 153, 51); // Naranja

                // Celda encabezado 1
                var celda1 = new PdfPCell(new Phrase("Producto:", fontEncabezado));
                celda1.BackgroundColor = fondoEncabezado;
                tabla.AddCell(celda1);

                // Celda valor 1
                tabla.AddCell(new Phrase(producto.Nombre, FontFactory.GetFont("Arial", 12, Font.NORMAL)));

                // Celda encabezado 2
                var celda2 = new PdfPCell(new Phrase("Cantidad Vendida:", fontEncabezado));
                celda2.BackgroundColor = fondoEncabezado;
                tabla.AddCell(celda2);

                // Celda valor 2
                tabla.AddCell(new Phrase(producto.CantidadVendida.ToString(), FontFactory.GetFont("Arial", 12, Font.NORMAL)));

                doc.Add(tabla);
            }


            doc.Close();
            ms.Position = 0;

            return File(ms, "application/pdf", $"ReporteProductoMasVendido_{fechaReporte:yyyyMMdd}.pdf");
        }
        public ActionResult ReporteAjustesInventario(DateTime fechaReporte)
        {
            var ajustes = _ajusteInventarioServicio.ObtenerAjustesPorFecha(fechaReporte);

            if (ajustes == null || ajustes.Count == 0)
            {
                TempData["Mensaje"] = "No se encontraron ajustes de inventario en la fecha seleccionada.";
                return RedirectToAction("Reportes");
            }

            MemoryStream ms = new MemoryStream();
            Document doc = new Document(PageSize.A4, 25, 25, 30, 30);
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);
            writer.CloseStream = false;

            doc.Open();
            EncabezadoPdfUtilidad.AgregarEncabezado(doc, writer, "REPORTE DE AJUSTES DE INVENTARIO", fechaReporte);

            foreach (var ajuste in ajustes)
            {
                PdfPTable tabla = new PdfPTable(2);
                tabla.WidthPercentage = 80;
                tabla.SpacingBefore = 10f;
                tabla.SpacingAfter = 10f;

                tabla.AddCell("Producto:");
                tabla.AddCell(ajuste.NombreProducto);

                tabla.AddCell("Cantidad Ajustada:");
                tabla.AddCell(ajuste.CantidadAjustada.ToString());

                tabla.AddCell("Motivo:");
                tabla.AddCell(ajuste.NombreMotivo);

                tabla.AddCell("Usuario:");
                tabla.AddCell(ajuste.NombreUsuario);

                tabla.AddCell("Fecha de Ajuste:");
                tabla.AddCell(ajuste.FechaAjuste.ToString("yyyy-MM-dd HH:mm"));

                tabla.AddCell("Acción:");
                tabla.AddCell(ajuste.Accion ? "Entrada" : "Salida");

                tabla.AddCell("Comentarios:");
                tabla.AddCell(ajuste.Comentarios ?? "");

                doc.Add(tabla);
                doc.Add(new Paragraph(" ")); // Espacio entre tablas
            }

            doc.Close();
            ms.Position = 0;

            return File(ms, "application/pdf", $"ReporteAjustesInventario_{fechaReporte:yyyyMMdd}.pdf");
        }


    }
}