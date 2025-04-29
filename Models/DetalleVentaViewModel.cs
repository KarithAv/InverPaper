using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InverPaper.Models
{
    public class DetalleVentaViewModel
    {
        public int IdProducto { get; set; }

        [Required]
        public int Cantidad { get; set; }

        public string NombreProducto { get; set; }

        public decimal Precio { get; set; }

        public decimal Subtotal => Cantidad * Precio;
    }

}