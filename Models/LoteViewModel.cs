using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InverPaper.Models
{
    public class LoteViewModel
    {
        [Required(ErrorMessage = "Debes seleccionar un producto")]
        [Display(Name = "Producto")]
        public int IdProducto { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que cero")]
        [Display(Name = "Cantidad")]
        public decimal Cantidad { get; set; }

        [Required(ErrorMessage = "El precio de compra es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que cero")]
        [Display(Name = "Precio de compra")]
        public decimal PrecioCompra { get; set; }

        [Required(ErrorMessage = "El número de lote es obligatorio")]
        [Display(Name = "Número de lote")]
        public string NumeroLote { get; set; } = string.Empty;

        // Fecha generada automáticamente en el backend (no editable)
        public DateTime FechaIngreso { get; set; }

        public bool ConfirmarPrecioDiferente { get; set; }

        public IEnumerable<SelectListItem> Productos { get; set; }
    }
}