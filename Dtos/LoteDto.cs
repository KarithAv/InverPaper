using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InverPaper.Dtos
{
    public class LoteDto
    {
            public int Id { get; set; }
            public int IdProducto { get; set; }

            public string NombreProducto { get; set; } = string.Empty;
            public decimal PrecioCompra { get; set; }
            public decimal Cantidad { get; set; }
            public DateTime FechaIngreso { get; set; }
            public string NumeroLote { get; set; } = string.Empty;
            public bool ConfirmarPrecioDiferente { get; set; }
            public int Response { get; set; }
            public string Mensaje { get; set; } = String.Empty;
    }
}