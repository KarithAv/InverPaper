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
    public class CategoriaController : Controller
    {
        private CategoriaRepositorio _categoriaRepo = new CategoriaRepositorio(); // Repositorio para manejar la base de datos
        private CategoriaServicio _categoriaServicio = new CategoriaServicio();    // Servicio para la lógica de negocio, si se necesita
        private EstadoRepositorio _estadoRepo = new EstadoRepositorio();                                                // GET: Categoria
        public ActionResult Index()
        {
            return View();
        }

        // Acción para listar todas las categorías
        public ActionResult Categorias()
        {
            var lista = _categoriaRepo.ListarCategorias();  // Usamos el repositorio para obtener las categorías
            return View(lista);  // Pasamos la lista de categorías a la vista
        }

//-------------------------------------------------------------------------------------------------------------------------------------------
        public ActionResult Crear()
        {
            return View();
        }

        // Método para crear una nueva categoría
        [HttpPost]
        public ActionResult Crear(CrearCategoriaViewModel viewModel)
        {
            if (ModelState.IsValid)
                try
                {
                    // ✅ Aquí se transforma el ViewModel a un DTO
                    var categoriaDto = new CategoriaDto
                    {
                        NombreCategoria = viewModel.NombreCategoria,
                    };

                    // ✅ Aquí realmente se crea el usuario en la base de datos
                    _categoriaServicio.CrearCategoria(categoriaDto);

                    return RedirectToAction("Categorias");
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
                _categoriaServicio.EliminarCategoria(id);
                TempData["Mensaje"] = "La categoria ha sido desactivada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Categorias", "Categoria");
        }
        [HttpPost]
        public ActionResult Activar(int id)
        {
            var repositorio = new CategoriaRepositorio();
            repositorio.ActivarCategoria(id);
            TempData["Mensaje"] = "Categoria activada correctamente.";
            return RedirectToAction("Categorias");
        }
    }
}