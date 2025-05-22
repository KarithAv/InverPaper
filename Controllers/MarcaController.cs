using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InverPaper.Repositorios;
using InverPaper.Servicios;
using InverPaper.Dtos;
using InverPaper.Models;
using InverPaper.Utilidades;

namespace InverPaper.Controllers
{
    [AutorizarSesionUtilidad]
    [AutorizarRolUtilidad(1)]
    [CacheUtilidad]
    public class MarcaController : Controller
    {
     
        private MarcaRepositorio _marcaRepo = new MarcaRepositorio(); // Repositorio para manejar la base de datos
        private MarcaServicio _marcaServicio = new MarcaServicio();  
        public ActionResult Index()
        {
            return View();
        }

        // Acción para listar todas las categorías
        public ActionResult Marcas()
        {
            var lista = _marcaRepo.ListarMarcas();  // Usamos el repositorio para obtener las categorías
            return View(lista);  // Pasamos la lista de categorías a la vista
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------
        public ActionResult Crear()
        {
            return View();
        }

        // Método para crear una nueva categoría
        [HttpPost]
        public ActionResult Crear(CrearMarcaViewModel viewModel)
        {
            if (ModelState.IsValid)
                try
                {
                    // ✅ Aquí se transforma el ViewModel a un DTO
                    var marcaDto = new MarcaDto
                    {
                        NombreMarca = viewModel.NombreMarca,
                    };

                    // ✅ Aquí realmente se crea el usuario en la base de datos
                    _marcaServicio.CrearMarca(marcaDto);

                    return RedirectToAction("Marcas");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }

            return View(viewModel);
        }
        //----------------------------------------------------------------------------------------------------------------------------
        // Acción para eliminar una categoría
        public ActionResult Eliminar(int id)
        {
            try
            {
                _marcaServicio.EliminarMarca(id);
                TempData["Mensaje"] = "La marca ha sido desactivada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Marcas", "Marca");
        }
        [HttpPost]
        public ActionResult Activar(int id)
        {
            var repositorio = new MarcaRepositorio();
            repositorio.ActivarMarca(id);
            TempData["Mensaje"] = "Marca activada correctamente.";
            return RedirectToAction("Marcas");
        }

    }
}