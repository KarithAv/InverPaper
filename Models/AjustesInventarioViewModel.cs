using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InverPaper.Models
{
    public class AjustesInventarioViewModel
    {
        [Required(ErrorMessage = "Seleccione un producto.")]
        public int IdProducto { get; set; }

        [Required(ErrorMessage = "Ingrese una cantidad.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero.")]
        public int CantidadAjustada { get; set; }

        [Required(ErrorMessage = "Seleccione un motivo.")]
        public int IdMotivo { get; set; }

        public string Comentarios { get; set; }

        [Required(ErrorMessage = "Seleccione si es un aumento o reducción.")]
        public bool Accion { get; set; } // true = aumento, false = reducción

        public int IdUsuario { get; set; }

        public List<SelectListItem> ListaProductos { get; set; }
        public List<SelectListItem> ListaMotivos { get; set; }
    }
}