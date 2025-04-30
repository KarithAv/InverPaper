using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InverPaper.Utilidades
{
    public class AutorizarRolUtilidad : AuthorizeAttribute
    {
        private readonly int[] _rolesPermitidos;

        public AutorizarRolUtilidad(params int[] roles)
        {
            _rolesPermitidos = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase contexto)
        {
            var rolUsuario = contexto.Session["rol"] as int?;

            if (!rolUsuario.HasValue)
                return false;

            return _rolesPermitidos.Contains(rolUsuario.Value);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filtroContexto)
        {
            // Redirige a la vista de acceso denegado
            filtroContexto.Result = new RedirectResult("~/Principal/AccesoDenegado");
        }
    }
}