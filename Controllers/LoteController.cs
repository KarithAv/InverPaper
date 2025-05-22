using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InverPaper.Dtos;
using InverPaper.Repositorios;
using InverPaper.Servicios;
using InverPaper.Models;
using InverPaper.Utilidades;

namespace InverPaper.Controllers
{
    [AutorizarSesionUtilidad]
    [AutorizarRolUtilidad(3)]
    [CacheUtilidad]
    public class LoteController : Controller
    {
        private readonly LoteRepositorio _loteRepo = new LoteRepositorio();
        private readonly ProductoRepositorio _productoRepo = new ProductoRepositorio();
        private readonly LoteServicio _loteServicio = new LoteServicio();


        public ActionResult Lotes()
        {

            var lista = _loteRepo.ListarLotes();
            return View(lista);
        }
        // GET: Lote
        [HttpGet]
        public ActionResult Crear()
        {
            var model = new LoteViewModel
            {
                // Llenamos el dropdown con los productos
                Productos = _productoRepo.ObtenerProductosActivos()
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.NombreCompleto
                    }),
                FechaIngreso = DateTime.Now  // Asignamos la fecha de ingreso
            };

            return View(model);  // Asegúrate de que estamos pasando LoteViewModel aquí
        }

        [HttpPost]
        public ActionResult Crear(LoteViewModel model)
        {
            model.Productos = _productoRepo.ObtenerProductosActivos()
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.NombreCompleto
                });

            if (!ModelState.IsValid) 
            {
                return View(model);
            }

            var dto = new LoteDto
            {
                IdProducto = model.IdProducto,
                Cantidad = model.Cantidad,
                PrecioCompra = model.PrecioCompra,
                NumeroLote = model.NumeroLote,
                 ConfirmarPrecioDiferente = model.ConfirmarPrecioDiferente
            };

            var resultado = _loteServicio.RegistrarLote(dto);

            // Si hay advertencia (Precio diferente y no confirmado)
            if (resultado.Response == 2)
            {
                ViewBag.Advertencia = resultado.Mensaje;
                return View(model);
            }
            else if (resultado.Response == 0)
            {
                ModelState.AddModelError("", resultado.Mensaje);
                return View(model);
            }

            // Redirigir a la vista de productos
            return RedirectToAction("Productos", "Producto");
        }

    }
}