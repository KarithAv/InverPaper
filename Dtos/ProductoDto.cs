using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InverPaper.Dtos
{
    public class ProductoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = String.Empty;
        public string Descripcion { get; set; } = String.Empty;
        public int StockMin { get; set; } 
        public int StockActual { get; set; }
        public decimal PrecioVenta { get; set; }
        public int IdEstado { get; set; }
        public string NombreEstado { get; set; } = String.Empty;
        public int IdCategoria { get; set; }
        public string NombreCategoria { get; set; } = String.Empty;
        public int IdMarca { get; set; }
        public string NombreMarca { get; set; } = String.Empty;
        public string Mensaje { get; set; } = String.Empty;
        public int Respuesta { get; set; }
    }
}