using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InverPaper.Repositories;

namespace InverPaper.Controllers
{
    public class ProductoController : Controller
    {
        private ProductoRepository _productoRepo = new ProductoRepository();
        // GET: Producto
        public ActionResult Productos()
        {

            var lista = _productoRepo.ListarProductos(); // o usuarioService.ObtenerUsuarios()
            return View(lista);
        }
    }
}