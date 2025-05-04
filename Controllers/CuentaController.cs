using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InverPaper.Servicios;
using InverPaper.Models;
using InverPaper.Repositorios;
using InverPaper.Utilidades;
using System.Diagnostics;

namespace InverPaper.Controllers
{
    public class CuentaController : Controller
    {
        TokenRepositorio _tokenRepo = new TokenRepositorio();
        UsuarioRepositorio _usuarioRepo = new UsuarioRepositorio();
        public ActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var service = new UsuarioServicio();
            var resultado = service.Login(model.Email, model.Contraseña);

            if (resultado.Respuesta == 1)
            {
                if (resultado.IdEstado != 1) // Validación del estado (1 = Activo)
                {
                    ModelState.AddModelError("", "Tu cuenta está inactiva. Contacta al administrador.");
                    return View(model);
                }
                Session["IdUsuario"] = resultado.Id;
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
        public ActionResult RecuperarContraseña()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RecuperarContraseña(string correo)
        {
            var usuario = _usuarioRepo.ObtenerUsuarioPorCorreo(correo);

            if (usuario == null)
            {
                ModelState.AddModelError("", "No se encontró una cuenta con ese correo.");
                return View();
            }

            // Crear token
            var token = Guid.NewGuid();
            var expiracion = DateTime.Now.AddMinutes(15);
            var tokenRepo = new TokenRepositorio();
            tokenRepo.CrearToken(usuario.Id, token, expiracion);

            // Generar URL
            var resetUrl = Url.Action("RestablecerContraseña", "Cuenta", new { token = token }, Request.Url.Scheme);

            string rutaPlantilla = Server.MapPath("~/Views/Cuenta/CuerpoCorreo.cshtml");
            string html = System.IO.File.ReadAllText(rutaPlantilla);

            // Reemplazar los marcadores de posición con los datos reales
            string mensaje = html
                .Replace("{NOMBRE}", usuario.Nombre)
                .Replace("{CORREO}", usuario.Correo)
                .Replace("{LINK}", resetUrl);

            // Enviar correo
            var correoUtil = new GestorCorreoUtilidad();
            correoUtil.EnviarCorreo(usuario.Correo, "Recuperación de Contraseña", mensaje, true);

            ViewBag.Mensaje = "Se ha enviado un enlace de recuperación a tu correo.";
            return View();
        }
        [HttpGet]
        public ActionResult RestablecerContraseña(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("RestablecerContraseña", "Cuenta");
            }

            // Verificar el token en la base de datos
            var tokenRecuperacion = _tokenRepo.ObtenerIdUsuarioPorToken(Guid.Parse(token));

            if (tokenRecuperacion == null || tokenRecuperacion.Usado || tokenRecuperacion.FechaExpiracion < DateTime.Now)
            {
                TempData["Error"] = "El enlace de recuperación ha expirado o ya fue utilizado.";
                return RedirectToAction("Login", "Cuenta");
            }

            // Si el token es válido, se muestra el formulario para ingresar la nueva contraseña
            var viewModel = new ReestablecerContraseñaViewModel
            {
                Token = token
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RestablecerContraseña(ReestablecerContraseñaViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Verificar que las contraseñas coincidan
                if (viewModel.NuevaContraseña != viewModel.ConfirmarContraseña)
                {
                    ModelState.AddModelError("ConfirmarContraseña", "Las contraseñas no coinciden.");
                    return View(viewModel);
                }

                // Validar que contenga letras y números (mínimo 6 caracteres)
                var regex = new System.Text.RegularExpressions.Regex("^(?=.*[a-zA-Z])(?=.*\\d).{6,}$");
                if (!regex.IsMatch(viewModel.NuevaContraseña))
                {
                    ModelState.AddModelError("NuevaContraseña", "La contraseña debe contener al menos una letra y un número, y tener mínimo 6 caracteres.");
                    return View(viewModel);
                }

                try
                {
                    var tokenRecuperacion = _tokenRepo.ObtenerIdUsuarioPorToken(Guid.Parse(viewModel.Token));

                    if (tokenRecuperacion == null || tokenRecuperacion.Usado || tokenRecuperacion.FechaExpiracion < DateTime.Now)
                    {
                        ModelState.AddModelError("", "El token ha expirado o ya ha sido utilizado.");
                        return View(viewModel);
                    }

                    var usuario = _usuarioRepo.ObtenerUsuarioPorId(tokenRecuperacion.IdUsuario);

                    if (usuario == null)
                    {
                        ModelState.AddModelError("", "No se encontró el usuario.");
                        return View(viewModel);
                    }

                    string nuevaContraseña = (viewModel.NuevaContraseña);
                    
                    _usuarioRepo.ActualizarContraseña(usuario.Id, nuevaContraseña);

                    _tokenRepo.MarcarTokenComoUsado(Guid.Parse(viewModel.Token));

                    return RedirectToAction("Login", "Cuenta");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(viewModel);
                }
            }

            return View(viewModel);
        }

    }
}
