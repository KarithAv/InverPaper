using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InverPaper.Dtos
{
    public class VentaDto
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = String.Empty;
        public DateTime FechaVenta { get; set; }
        public int IdMetPago { get; set; }
        public decimal Total { get; set; }
        public string Mensaje { get; set; } = String.Empty;
        public int Respuesta { get; set; }
        public List<DetalleVentaDto> Detalles { get; set; } = new List<DetalleVentaDto>();
    }
}