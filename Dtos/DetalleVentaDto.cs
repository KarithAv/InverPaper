using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InverPaper.Dtos
{
    public class DetalleVentaDto
    {
        public int Id { get; set; }
        public int IdVenta { get; set; }
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }
        public string Mensaje { get; set; } = String.Empty;
        public int Respuesta { get; set; }
    }
}