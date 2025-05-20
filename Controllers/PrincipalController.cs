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
        [CacheUtilidad]
        public ActionResult Index()
        {
            if (Session["Usuario"] == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }
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