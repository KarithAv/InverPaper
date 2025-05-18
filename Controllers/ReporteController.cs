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
        ProductoServicio _productoServicio = new ProductoServicio();

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

            PdfUtilidad.AgregarEncabezado(doc, writer, "REPORTE DE VENTAS", fechaReporte);
            PdfUtilidad.AgregarSubtitulo(doc, "Detalle de ventas del día");

            decimal totalGeneral = 0;

            foreach (var venta in ventas)
            {
                string infoVenta = $"Venta ID: {venta.Id} | Usuario: {venta.NombreUsuario} | Fecha: {venta.FechaVenta:dd/MM/yyyy}";
                PdfUtilidad.AgregarSubtitulo(doc, infoVenta);

                var detalles = detalleRepo.ObtenerDetallesPorIdVenta(venta.Id);

                string[] encabezados = { "Producto", "Cantidad", "Subtotal" };
                string[,] valores = new string[detalles.Count, 3];
                for (int i = 0; i < detalles.Count; i++)
                {
                    valores[i, 0] = detalles[i].NombreProducto;
                    valores[i, 1] = detalles[i].Cantidad.ToString();
                    valores[i, 2] = detalles[i].Subtotal.ToString("C");
                }
                var tablaDetalle = PdfUtilidad.CrearTablaHorizontal(encabezados, valores);
                doc.Add(tablaDetalle);

                var totalVenta = new Paragraph($"Total Venta: {venta.Total:C}", PdfUtilidad.TextoFont)
                {
                    Alignment = Element.ALIGN_RIGHT,
                    SpacingBefore = 5f
                };
                doc.Add(totalVenta);

                PdfUtilidad.AgregarLineaSeparadora(doc);

                totalGeneral += venta.Total;
            }
            Paragraph totalFinal = new Paragraph($"TOTAL GENERAL DEL DÍA: {totalGeneral:C}", PdfUtilidad.TituloFont)
            {
                Alignment = Element.ALIGN_RIGHT,
                SpacingBefore = 10f
            };
            doc.Add(totalFinal);

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
            byte[] imagenGrafico = PdfUtilidad.GenerarGraficoBarras("Top 5 Productos Más Vendidos", categorias, valores);

            // Crear PDF
            MemoryStream ms = new MemoryStream();
            Document doc = new Document(PageSize.A4, 25, 25, 30, 30);
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);
            writer.CloseStream = false;

            doc.Open();

            // Agregar encabezado usando PdfUtilidad
            PdfUtilidad.AgregarEncabezado(doc, writer, "REPORTE PRODUCTO MÁS VENDIDO", fechaReporte);

            // Subtítulo
            PdfUtilidad.AgregarSubtitulo(doc, "Gráfico de los productos más vendidos");

                // Agregar gráfico al PDF
                var grafico = iTextSharp.text.Image.GetInstance(imagenGrafico);
                grafico.ScaleToFit(500f, 300f);
                grafico.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                doc.Add(grafico);

                doc.Add(new Paragraph(" ")); // Espacio

                // Subtítulo
                PdfUtilidad.AgregarSubtitulo(doc, "Detalle de los productos");

            // Preparar datos para tabla vertical
            string[] encabezados = { "Producto", "Cantidad Vendida" };
            string[] valoresTabla = productosMasVendidos
                                    .Select(p => new string[] { p.Nombre, p.CantidadVendida.ToString() })
                                    .SelectMany(x => x)
                                    .ToArray();

            foreach (var producto in productosMasVendidos)
            {
                string[] encs = { "Producto", "Cantidad Vendida" };
                string[] vals = { producto.Nombre, producto.CantidadVendida.ToString() };

                PdfPTable tabla = PdfUtilidad.CrearTablaVertical(encs, vals);
                doc.Add(tabla);
                PdfUtilidad.AgregarLineaSeparadora(doc);
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

            PdfUtilidad.AgregarEncabezado(doc, writer, "REPORTE DE AJUSTES DE INVENTARIO", fechaReporte);

            foreach (var ajuste in ajustes)
            {
                string[] encabezados = {
            "Producto:",
            "Cantidad Ajustada:",
            "Motivo:",
            "Usuario:",
            "Fecha de Ajuste:",
            "Acción:",
            "Comentarios:"
        };

                string[] valores = {
            $"{ajuste.NombreProducto} - {ajuste.NombreMarca}",
            ajuste.CantidadAjustada.ToString(),
            ajuste.NombreMotivo,
            ajuste.NombreUsuario,
            ajuste.FechaAjuste.ToString("yyyy-MM-dd"),
            ajuste.Accion ? "Entrada" : "Salida",
            ajuste.Comentarios ?? ""
        };

                PdfPTable tabla = PdfUtilidad.CrearTablaVertical(encabezados, valores);
                doc.Add(tabla);

                PdfUtilidad.AgregarLineaSeparadora(doc);
            }

            doc.Close();
            ms.Position = 0;

            return File(ms, "application/pdf", $"ReporteAjustesInventario_{fechaReporte:yyyyMMdd}.pdf");
        }
        public ActionResult ReporteInventario()
        {
            var productos = _productoServicio.ObtenerInventarioActual();

            if (productos == null || !productos.Any())
            {
                TempData["Mensaje"] = "No se encontraron productos en el inventario.";
                return RedirectToAction("Index", "Principal");
            }

            MemoryStream ms = new MemoryStream();
            Document doc = new Document(PageSize.A4.Rotate(), 25, 25, 30, 30); // Orientación horizontal
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);
            writer.CloseStream = false;

            doc.Open();

            PdfUtilidad.AgregarEncabezado(doc, writer, "REPORTE DE INVENTARIO", DateTime.Now);
            PdfUtilidad.AgregarSubtitulo(doc, "Listado completo de productos registrados");

            // Encabezados y cuerpo de la tabla
            string[] encabezados = {
        "Producto", "Marca", "Stock Actual", "Stock Mínimo", "Estado"
    };

            string[,] valores = new string[productos.Count, encabezados.Length];

            for (int i = 0; i < productos.Count; i++)
            {
                var p = productos[i];
                valores[i, 0] = p.Nombre;
                valores[i, 1] = p.NombreMarca;
                valores[i, 2] = p.StockActual.ToString();
                valores[i, 3] = p.StockMin.ToString();
                valores[i, 4] = p.NombreEstado;
            }

            PdfPTable tabla = PdfUtilidad.CrearTablaHorizontal(encabezados, valores);
            doc.Add(tabla);

            doc.Close();
            ms.Position = 0;

            return File(ms, "application/pdf", $"ReporteInventario_{DateTime.Now:yyyyMMdd}.pdf");
        }

    }
}