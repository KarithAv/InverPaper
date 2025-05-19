using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InverPaper.Repositorios;
using InverPaper.Models;
using InverPaper.Servicios;
using InverPaper.Dtos;
using InverPaper.Utilidades;

namespace InverPaper.Controllers
{
    [AutorizarSesionUtilidad]
    public class ProductoController : Controller
    {
        private ProductoRepositorio _productoRepo = new ProductoRepositorio();
        private ProductoServicio _productoServicio = new ProductoServicio();
        private MarcaRepositorio _marcaRepo = new MarcaRepositorio();
        private EstadoRepositorio _estadoRepo = new EstadoRepositorio();
        private CategoriaRepositorio _categoriaRepo = new CategoriaRepositorio();
        // GET: Producto
        public ActionResult Productos()
        {

            var lista = _productoRepo.ListarProductos(); // o usuarioService.ObtenerUsuarios()
            return View(lista);
        }
        [AutorizarRolUtilidad(1)]
        public ActionResult Crear()
        {
            // Obtener las listas de categorías, marcas y estados desde el servicio o repositorio
            ViewBag.Estados = new SelectList(_estadoRepo.ObtenerEstados(), "Id", "NombreEstado");
            ViewBag.Categorias = new SelectList(_categoriaRepo.ObtenerCategorias(), "Id", "NombreCategoria");
            ViewBag.Marcas = new SelectList(_marcaRepo.ObtenerMarcas(), "Id", "NombreMarca");

            return View();
        }
        [AutorizarRolUtilidad(1)]
        [HttpPost]
        public ActionResult Crear(ProductoViewModel productoViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Convertir el ProductoViewModel a ProductoDto
                    var productoDto = new ProductoDto
                    {
                        Nombre = productoViewModel.Nombre,
                        Descripcion = productoViewModel.Descripcion,
                        StockMin = productoViewModel.StockMin,
                        StockActual = productoViewModel.StockActual,
                        PrecioVenta = productoViewModel.PrecioVenta,
                        IdEstado = productoViewModel.IdEstado,
                        IdCategoria = productoViewModel.IdCategoria,
                        IdMarca = productoViewModel.IdMarca
                    };

                    // Crear el producto a través del servicio
                    _productoServicio.CrearProducto(productoDto);

                    return RedirectToAction("Productos"); // Redirige si todo salió bien
                }
                catch (Exception ex)
                {
                    // Agregar el error al ModelState para mostrarlo en la vista
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            // Si el modelo no es válido o hubo error, recargar las listas
            ViewBag.Estados = new SelectList(_estadoRepo.ObtenerEstados(), "Id", "NombreEstado");
            ViewBag.Categorias = new SelectList(_categoriaRepo.ObtenerCategorias(), "Id", "NombreCategoria");
            ViewBag.Marcas = new SelectList(_marcaRepo.ObtenerMarcas(), "Id", "NombreMarca");

            return View(productoViewModel);
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------------
        [AutorizarRolUtilidad(1)]
        public ActionResult Editar(int id)
        {
            var productoDto = _productoRepo.ObtenerProductoPorId(id);

            if (productoDto == null)
            {
                return HttpNotFound(); // Producto no encontrado
            }

            var productoViewModel = new ProductoViewModel
            {
                Id = productoDto.Id,
                Nombre = productoDto.Nombre,
                Descripcion = productoDto.Descripcion,
                StockMin = productoDto.StockMin,
                StockActual = productoDto.StockActual,
                PrecioVenta = productoDto.PrecioVenta,
                IdEstado = productoDto.IdEstado,
                IdCategoria = productoDto.IdCategoria,
                IdMarca = productoDto.IdMarca
            };

            // Cargar dropdowns
            ViewBag.Estados = new SelectList(_estadoRepo.ObtenerEstados(), "Id", "NombreEstado", productoViewModel.IdEstado);
            ViewBag.Categorias = new SelectList(_categoriaRepo.ObtenerCategorias(), "Id", "NombreCategoria", productoViewModel.IdCategoria);
            ViewBag.Marcas = new SelectList(_marcaRepo.ObtenerMarcas(), "Id", "NombreMarca", productoViewModel.IdMarca);

            return View(productoViewModel);
        }
        [AutorizarRolUtilidad(1)]
        [HttpPost]
        public ActionResult Editar(ProductoViewModel productoViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var productoDto = new ProductoDto
                    {
                        Id = productoViewModel.Id,
                        Nombre = productoViewModel.Nombre,
                        Descripcion = productoViewModel.Descripcion,
                        StockMin = productoViewModel.StockMin,
                        StockActual = productoViewModel.StockActual,
                        PrecioVenta = productoViewModel.PrecioVenta,
                        IdEstado = productoViewModel.IdEstado,
                        IdCategoria = productoViewModel.IdCategoria,
                        IdMarca = productoViewModel.IdMarca
                    };

                    _productoServicio.EditarProducto(productoDto);

                    return RedirectToAction("Productos"); // Redirige si todo bien
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message); // Muestra error si hay
                }
            }

            // Si no es válido o hubo error, recargamos dropdowns
            ViewBag.Estados = new SelectList(_estadoRepo.ObtenerEstados(), "Id", "NombreEstado", productoViewModel.IdEstado);
            ViewBag.Categorias = new SelectList(_categoriaRepo.ObtenerCategorias(), "Id", "NombreCategoria", productoViewModel.IdCategoria);
            ViewBag.Marcas = new SelectList(_marcaRepo.ObtenerMarcas(), "Id", "NombreMarca", productoViewModel.IdMarca);

            return View(productoViewModel);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        [AutorizarRolUtilidad(1)]
        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            try
            {
                string usuarioActual = Session["Correo"]?.ToString(); // Asegúrate de que se esté guardando al iniciar sesión

                _productoServicio.EliminarProducto(id, usuarioActual);

                return RedirectToAction("Productos");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error al eliminar el producto: " + ex.Message);
                return RedirectToAction("Productos");
            }
        }
    }
}