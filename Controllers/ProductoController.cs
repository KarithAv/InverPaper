using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InverPaper.Repositorios;

namespace InverPaper.Controllers
{
    public class ProductoController : Controller
    {
        private ProductoRepositorio _productoRepo = new ProductoRepositorio();
        // GET: Producto
        public ActionResult Productos()
        {

            var lista = _productoRepo.ListarProductos(); // o usuarioService.ObtenerUsuarios()
            return View(lista);
        }
    }
}