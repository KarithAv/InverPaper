using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InverPaper.Services;
using InverPaper.Models;

namespace InverPaper.Controllers
{
    public class CuentaController : Controller
    {
        public ActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var service = new UsuarioService();
            var resultado = service.Login(model.Email, model.Contraseña);

            if (resultado.Respuesta == 1)
            {
                if (resultado.IdEstado != 1) // Validación del estado (1 = Activo)
                {
                    ModelState.AddModelError("", "Tu cuenta está inactiva. Contacta al administrador.");
                    return View(model);
                }

                Session["usuario"] = resultado.Nombre;
                Session["rol"] = resultado.IdRol;
                return RedirectToAction("Index", "Principal");
            }
            else
            {
                ModelState.AddModelError("", resultado.Mensaje);
                return View(model);
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
