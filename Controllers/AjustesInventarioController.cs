using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InverPaper.Repositorios;
using InverPaper.Dtos;
using InverPaper.Models;
using InverPaper.Servicios;
using InverPaper.Utilidades;

namespace InverPaper.Controllers
{
    [AutorizarSesionUtilidad]
    public class AjustesInventarioController : Controller
    {
        private readonly MotivoRepositorio _motivoRepo = new MotivoRepositorio();
        private readonly AjustesInventarioServicio _ajusteServicio = new AjustesInventarioServicio();
        private readonly ProductoRepositorio _productoRepo = new ProductoRepositorio();

        // GET: AjustesInventario
        public ActionResult Ajustes ()
        {
            var vm = new AjustesInventarioViewModel
            {
                ListaMotivos = _motivoRepo.ObtenerMotivos()
             .Select(m => new SelectListItem
             {
                 Value = m.Id.ToString(),
                 Text = m.Motivo
             }).ToList(),

                ListaProductos = _productoRepo.ObtenerProductosActivos()
             .Select(p => new SelectListItem
             {
                 Value = p.Id.ToString(),
                 Text = p.NombreCompleto
             }).ToList(),

             IdUsuario = Convert.ToInt32(Session["IdUsuario"])
            };

            return View(vm);
        }

        [HttpPost]
        public ActionResult Ajustes (AjustesInventarioViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.ListaMotivos = _motivoRepo.ObtenerMotivos()
             .Select(m => new SelectListItem
             {
                 Value = m.Id.ToString(),
                 Text = m.Motivo
             }).ToList();

                vm.ListaProductos = _productoRepo.ObtenerProductosActivos()
             .Select(p => new SelectListItem
             {
                 Value = p.Id.ToString(),
                 Text = p.Nombre
             }).ToList();

                return View(vm);
            }

            var dto = new AjusteInventarioDto
            {
                IdProducto = vm.IdProducto,
                CantidadAjustada = vm.CantidadAjustada,
                IdMotivo = vm.IdMotivo,
                Comentarios = vm.Comentarios,
                IdUsuario = vm.IdUsuario,
                Accion = vm.Accion, // <- Asegúrate de incluirlo aquí
                FechaAjuste = DateTime.Now // <- Opcional: puedes establecerlo aquí si no lo haces en el SP
            };

            try
            {
                _ajusteServicio.RegistrarAjuste(dto);
                TempData["AjusteExitoso"] = true;
                return RedirectToAction("Ajustes","AjustesInventario");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error SQL: " + ex.Message);
                vm.ListaMotivos = _motivoRepo.ObtenerMotivos()
                    .Select(m => new SelectListItem
                    {
                        Value = m.Id.ToString(),
                        Text = m.Motivo
                    }).ToList();
                vm.ListaProductos = _productoRepo.ObtenerProductosActivos()
             .Select(p => new SelectListItem
             {
                 Value = p.Id.ToString(),
                 Text = p.Nombre
             }).ToList();
                return View(vm);
            }
        }
    }
}