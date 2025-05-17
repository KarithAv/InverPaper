using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Web.UI.DataVisualization.Charting;
using System.IO;
using iTextSharp.text.pdf.draw;

namespace InverPaper.Utilidades
{
    public class PdfUtilidad
    {
        static PdfUtilidad()
        {
            string rutaFuenteT = HttpContext.Current.Server.MapPath("~/Content/Fuentes/moon_get-Heavy.ttf");
            BaseFont fuenteTitulos = BaseFont.CreateFont(rutaFuenteT, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            string rutaFuenteS= HttpContext.Current.Server.MapPath("~/Content/Fuentes/Quicksand_Bold.otf");
            BaseFont fuenteSubtitulos = BaseFont.CreateFont(rutaFuenteS, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            string rutaFuenteTxt = HttpContext.Current.Server.MapPath("~/Content/Fuentes/Quicksand_Book.otf");
            BaseFont fuenteTxt = BaseFont.CreateFont(rutaFuenteTxt, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            TituloFont = new Font(fuenteTitulos, 16, Font.BOLD, BaseColor.BLACK);
            SubtituloFont = new Font(fuenteSubtitulos, 12, Font.BOLD, BaseColor.BLACK);
            TextoFont = new Font(fuenteTxt, 12, Font.NORMAL, BaseColor.BLACK);
        }

        // Fuentes
        public static readonly Font TituloFont;
        public static readonly Font SubtituloFont;
        public static readonly Font TextoFont;

        // Colores
        public static readonly BaseColor ColorEncabezado = new BaseColor(255, 153, 51); // naranja
        public static readonly BaseColor ColorFilaPar = new BaseColor(245, 245, 245); // gris claro

        // Agrega encabezado con título y fecha
        public static void AgregarEncabezado(Document doc, PdfWriter writer, string titulo, DateTime fecha)
        {
            PdfPTable encabezado = new PdfPTable(1);
            encabezado.WidthPercentage = 100;

            PdfPTable tablaInterna = new PdfPTable(2);
            tablaInterna.WidthPercentage = 100;
            float[] widths = { 1f, 5f };
            tablaInterna.SetWidths(widths);

            string rutaLogo = HttpContext.Current.Server.MapPath("~/Content/imagenes/logo-inverpaper.png");

            if (File.Exists(rutaLogo))
            {
                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(rutaLogo);
                logo.ScaleAbsolute(100, 100);

                PdfPCell celdaLogo = new PdfPCell(logo)
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    Rowspan = 2
                };
                tablaInterna.AddCell(celdaLogo);
            }

            // Usamos las fuentes personalizadas de PdfUtilidad
            PdfPCell celdaTitulo = new PdfPCell(new Phrase(titulo, PdfUtilidad.TituloFont))
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_BOTTOM
            };
            tablaInterna.AddCell(celdaTitulo);

            PdfPCell celdaFecha = new PdfPCell(new Phrase(fecha.ToString("dd/MM/yyyy"), PdfUtilidad.SubtituloFont))
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_TOP
            };
            tablaInterna.AddCell(celdaFecha);

            PdfPCell celdaTablaInterna = new PdfPCell(tablaInterna)
            {
                Border = Rectangle.NO_BORDER
            };

            encabezado.AddCell(celdaTablaInterna);
            doc.Add(encabezado);
            doc.Add(new Paragraph(" ")); // Espacio después del encabezado
        }

        public static void AgregarSubtitulo(Document doc, string texto)
        {
            Paragraph subtitulo = new Paragraph(texto, SubtituloFont)
            {
                SpacingBefore = 10f,
                SpacingAfter = 5f,
                Alignment = Element.ALIGN_LEFT
            };
            doc.Add(subtitulo);
        }


        // Tabla vertical: encabezados en columna 1 (con fondo naranja), datos en columna 2
        public static PdfPTable CrearTablaVertical(string[] encabezados, string[] valores)
        {
            PdfPTable tabla = new PdfPTable(2);
            tabla.WidthPercentage = 80;
            tabla.SpacingBefore = 10f;
            tabla.SpacingAfter = 10f;
            tabla.SetWidths(new float[] { 1f, 2f });

            for (int i = 0; i < encabezados.Length; i++)
            {
                PdfPCell celdaEncabezado = new PdfPCell(new Phrase(encabezados[i], SubtituloFont))
                {
                    BackgroundColor = ColorEncabezado,
                    Padding = 5,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                tabla.AddCell(celdaEncabezado);

                PdfPCell celdaValor = new PdfPCell(new Phrase(valores[i], TextoFont))
                {
                    Padding = 5,
                    BackgroundColor = i % 2 == 0 ? BaseColor.WHITE : ColorFilaPar,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                tabla.AddCell(celdaValor);
            }

            return tabla;
        }

        // Tabla horizontal: encabezados en fila superior (fondo naranja), datos en filas siguientes
        public static PdfPTable CrearTablaHorizontal(string[] encabezados, string[,] valores)
        {
            int columnas = encabezados.Length;
            int filas = valores.GetLength(0);

            PdfPTable tabla = new PdfPTable(columnas);
            tabla.WidthPercentage = 100;
            tabla.SpacingBefore = 10f;
            tabla.SpacingAfter = 10f;

            // Encabezados
            foreach (var encabezado in encabezados)
            {
                PdfPCell celdaEncabezado = new PdfPCell(new Phrase(encabezado, SubtituloFont))
                {
                    BackgroundColor = ColorEncabezado,
                    Padding = 5,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                tabla.AddCell(celdaEncabezado);
            }

            // Datos
            for (int i = 0; i < filas; i++)
            {
                for (int j = 0; j < columnas; j++)
                {
                    PdfPCell celdaDato = new PdfPCell(new Phrase(valores[i, j], TextoFont))
                    {
                        Padding = 5,
                        BackgroundColor = (i % 2 == 0) ? BaseColor.WHITE : ColorFilaPar,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    tabla.AddCell(celdaDato);
                }
            }

            return tabla;
        }


        // Agrega espacio entre elementos
        public static void AgregarEspacio(Document doc, float alto = 10f)
        {
            doc.Add(new Paragraph(" ") { SpacingBefore = alto });
        }
        public static void AgregarLineaSeparadora(Document doc)
        {
            LineSeparator line = new LineSeparator(1f, 100f, BaseColor.GRAY, Element.ALIGN_CENTER, -2);
            doc.Add(new Chunk(line));
        }

        public static byte[] GenerarGraficoBarras(string titulo, string[] categorias, int[] valores)
        {
            var chart = new Chart();
            chart.Width = 600;
            chart.Height = 400;

            var chartArea = new ChartArea();
            chart.ChartAreas.Add(chartArea);

            var series = new Series
            {
                ChartType = SeriesChartType.Bar
            };

            for (int i = 0; i < categorias.Length; i++)
            {
                series.Points.AddXY(categorias[i], valores[i]);
            }

            chart.Series.Add(series);
            chart.Titles.Add(titulo);

            using (var ms = new MemoryStream())
            {
                chart.SaveImage(ms, ChartImageFormat.Png);
                return ms.ToArray();
            }
        }
    }
}