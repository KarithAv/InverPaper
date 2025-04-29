using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InverPaper.Dtos;

namespace InverPaper.Models
{
    public class VentaViewModel
    {
        [Required(ErrorMessage = "Seleccione el usuario")]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "Seleccione método de pago")]
        public int IdMetPago { get; set; }

        public List<DetalleVentaViewModel> DetallesVenta { get; set; } = new List<DetalleVentaViewModel>();

        public List<ProductoDto> ProductosDisponibles { get; set; }

        public List<SelectListItem> MetodosPago { get; set; }
    }
}