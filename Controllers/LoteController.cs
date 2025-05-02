using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InverPaper.Dtos;
using InverPaper.Repositorios;
using InverPaper.Servicios;
using InverPaper.Models;
namespace InverPaper.Controllers
{
    public class LoteController : Controller
    {
        private readonly LoteRepositorio _loteRepo = new LoteRepositorio();
        private readonly ProductoRepositorio _productoRepo = new ProductoRepositorio();
        private readonly LoteServicio _loteServicio = new LoteServicio();

        // GET: Lote
        [HttpGet]
        public ActionResult Crear()
        {
            var model = new LoteViewModel
            {
                // Llenamos el dropdown con los productos
                Productos = _productoRepo.ListarProductos()
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Nombre
                    }),
                FechaIngreso = DateTime.Now  // Asignamos la fecha de ingreso
            };

            return View(model);  // Asegúrate de que estamos pasando LoteViewModel aquí
        }

        [HttpPost]
        public ActionResult Crear(LoteViewModel model)
        {
            // Recargar los productos en caso de error para que se mantenga la selección
            model.Productos = _productoRepo.ListarProductos()
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nombre
                });

            if (!ModelState.IsValid)
            {
                return View(model);  // Si hay un error, volvemos con el mismo LoteViewModel
            }

            // Convertir el LoteViewModel a LoteDto
            var dto = new LoteDto
            {
                IdProducto = model.IdProducto,
                Cantidad = model.Cantidad,
                PrecioCompra = model.PrecioCompra,
                NumeroLote = model.NumeroLote,
               // FechaCompra = model.FechaIngreso  // Asignamos la fecha de ingreso aquí
            };

            // Llamamos al servicio
            var resultado = _loteServicio.RegistrarLote(dto);

            if (resultado.Response == 1)
            {
                ViewBag.Advertencia = resultado.Mensaje;  // Mostramos la advertencia si el precio es diferente
                return View(model);  // Mostramos la vista con la advertencia
            }

            TempData["Mensaje"] = "Lote registrado con éxito";
            return RedirectToAction("Productos", "Producto");
        }
    }
}