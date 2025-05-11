using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InverPaper.Utilidades;
using InverPaper.Repositorios;

namespace InverPaper.Controllers
{
    [AutorizarSesionUtilidad]
    [CacheUtilidad]
    public class PrincipalController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            var repo = new ProductoRepositorio();
            var productosBajoStock = repo.ObtenerProductosProximosAgotarse();
            ViewBag.ProductosBajoStock = productosBajoStock;

            return View();
        }
        public ActionResult AccesoDenegado()
        {
            return View();
        }
    }
}