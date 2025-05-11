using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;


namespace InverPaper.Utilidades
{
    public class EncabezadoPdfUtilidad
    {
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
                logo.ScaleAbsolute(100,100);

                PdfPCell celdaLogo = new PdfPCell(logo)
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    Rowspan = 2
                };
                tablaInterna.AddCell(celdaLogo);
            }

            var fuenteTitulo = FontFactory.GetFont("Arial", 16, Font.BOLD);
            var fuenteFecha = FontFactory.GetFont("Arial", 10, Font.NORMAL);

            PdfPCell celdaTitulo = new PdfPCell(new Phrase(titulo, fuenteTitulo))
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_BOTTOM
            };
            tablaInterna.AddCell(celdaTitulo);

            PdfPCell celdaFecha = new PdfPCell(new Phrase(fecha.ToString("dd/MM/yyyy"), fuenteFecha))
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
            doc.Add(new Paragraph(" "));
        }
    }
}