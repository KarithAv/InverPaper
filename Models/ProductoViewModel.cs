using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InverPaper.Models
{
    public class ProductoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "El nombre debe tener al menos 3 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción del producto es obligatoria.")]
        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres.")]
        public string Descripcion { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El stock mínimo debe ser mayor que 0.")]
        public int StockMin { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El stock actual debe ser mayor que 0.")]
        public int StockActual { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "El precio de venta debe ser mayor que 0.")]
        public decimal PrecioVenta { get; set; }

        [Required(ErrorMessage = "El estado del producto es obligatorio.")]
        public int IdEstado { get; set; }

        [Required(ErrorMessage = "La categoría del producto es obligatoria.")]
        public int IdCategoria { get; set; }

        [Required(ErrorMessage = "La marca del producto es obligatoria.")]
        public int IdMarca { get; set; }
    }
}