using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InverPaper.Servicios;
using InverPaper.Models;
using InverPaper.Repositorios;
using InverPaper.Dtos;
namespace InverPaper.Controllers
{
    public class VentaController : Controller
    {
        private VentaServicio _ventaServicio = new VentaServicio();
        private ProductoRepositorio _productoRepo = new ProductoRepositorio();
        private MetodoPagoRepositorio _metodoPagoRepo = new MetodoPagoRepositorio();

        // GET: Venta
        public ActionResult RegistrarVenta()
        {
                var viewModel = new VentaViewModel
                {
                    ProductosDisponibles = _productoRepo.ObtenerProductosActivos(),
                    MetodosPago = _ventaServicio.ObtenerMetodosPagoDisponibles(),
                    DetallesVenta = new List<DetalleVentaViewModel>() // Para evitar null
                };
                return View(viewModel);
        
        }
        [HttpPost]
        public ActionResult RegistrarVenta(VentaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ProductosDisponibles = _productoRepo.ObtenerProductosActivos();
                model.MetodosPago = _ventaServicio.ObtenerMetodosPagoDisponibles();
                return View(model);
            }

            var venta = new VentaDto
            {
                IdUsuario = model.IdUsuario,
                IdMetPago = model.IdMetPago,
                FechaVenta = DateTime.Now
            };

            var detalles = model.DetallesVenta
                .Where(d => d.Cantidad > 0)
                .Select(d => new DetalleVentaDto
                {
                    IdProducto = d.IdProducto,
                    Cantidad = d.Cantidad,
                    Subtotal = d.Cantidad * d.Precio
                }).ToList();

            _ventaServicio.RegistrarVenta(venta, detalles);

            return RedirectToAction("RegistrarVenta"); // o a una vista de confirmación
        }

    }
}