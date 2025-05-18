using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InverPaper.Utilidades
{
    public class AutorizarSesionUtilidad : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext contexto)
        {
            // Si no existe la sesión de usuario, redirige al login
            if (HttpContext.Current.Session["usuario"] == null)
            {
                //contexto.Controller.TempData["MensajeLogin"] = "Debe iniciar sesión primero.";
                contexto.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary
                    {
                        { "controller", "Cuenta" },
                        { "action", "Login" }
                    });
            }

            base.OnActionExecuting(contexto);
        }
    }
}