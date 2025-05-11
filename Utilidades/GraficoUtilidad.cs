using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.DataVisualization.Charting;


namespace InverPaper.Utilidades
{
    public class GraficoUtilidad
    {
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