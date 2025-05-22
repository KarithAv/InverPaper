using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InverPaper.Repositorios;
using InverPaper.Dtos;
using InverPaper.Utilidades;
using InverPaper.Models;
using System.Diagnostics;
using InverPaper.Servicios;

namespace InverPaper.Controllers
{
    [AutorizarSesionUtilidad]
    [AutorizarRolUtilidad(1)]
    [CacheUtilidad]
    public class UsuarioController : Controller
    {
        private UsuarioRepositorio _usuarioRepo = new UsuarioRepositorio ();
        private UsuarioServicio _usuarioService = new UsuarioServicio();
        private RolRepositorio _rolRepo = new RolRepositorio();
        private EstadoRepositorio _estadoRepo = new EstadoRepositorio();
        // GET: Usuario
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Usuarios()
        {
            var lista = _usuarioRepo.ListarUsuarios(); // o usuarioService.ObtenerUsuarios()
            return View(lista);

        }


        // GET: Usuario/Crear
        public ActionResult Crear()
        {
            ViewBag.Roles = new SelectList(_rolRepo.ObtenerRoles(), "Id", "Rol");
            ViewBag.Estados = new SelectList(_estadoRepo.ObtenerEstados(), "Id", "NombreEstado");

            return View();
        }
        // POST: Usuario/Crear
        [HttpPost]
        public ActionResult Crear(CrearUsuarioViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string usuarioActual = Session["Correo"]?.ToString();
                    // ✅ Aquí se transforma el ViewModel a un DTO
                    var usuarioDto = new UsuarioDto
                    {
                        Nombre = viewModel.Nombre,
                        Apellido = viewModel.Apellido,
                        Correo = viewModel.Correo,
                        Contraseña = viewModel.Contraseña,
                        IdRol = viewModel.IdRol,
                        IdEstado = viewModel.IdEstado
                    };

                    // ✅ Aquí realmente se crea el usuario en la base de datos
                    _usuarioService.CrearUsuario(usuarioDto,usuarioActual);

                    return RedirectToAction("Usuarios");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            // Si falla, recargo los ViewBag
            ViewBag.Roles = new SelectList(_rolRepo.ObtenerRoles(), "Id", "Rol");
            ViewBag.Estados = new SelectList(_estadoRepo.ObtenerEstados(), "Id", "NombreEstado");

            return View(viewModel);
        }

//--------------------------------------------------------------------------------------------------------------

        public ActionResult Editar(int id)
{
    var usuario = _usuarioRepo.ObtenerUsuarioPorId(id);

    if (usuario == null)
    {
        return HttpNotFound();
    }

    // Preparamos el ViewModel con los datos del usuario
    var viewModel = new EditarUsuarioViewModel
    {
        Id = usuario.Id,
        Nombre = usuario.Nombre,
        Apellido = usuario.Apellido,
        Correo = usuario.Correo,
        IdRol = usuario.IdRol,
        IdEstado = usuario.IdEstado,

        Roles = _rolRepo.ObtenerRoles()
    .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Rol }),

        Estados = _estadoRepo.ObtenerEstados()
    .Select(e => new SelectListItem { Value = e.Id.ToString(), Text = e.NombreEstado })
    };

    return View(viewModel);
}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(EditarUsuarioViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string usuarioActual = Session["Correo"]?.ToString();
                    // Verificar si el correo está duplicado (solo si cambia el correo)
                    if (_usuarioRepo.UsuarioExisteEditar(viewModel.Correo, viewModel.Id))
                    {
                        ModelState.AddModelError("", "Ya existe un usuario con ese correo.");
                        viewModel.Roles = _rolRepo.ObtenerRoles()
                            .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Rol });
                        viewModel.Estados = _estadoRepo.ObtenerEstados()
                            .Select(e => new SelectListItem { Value = e.Id.ToString(), Text = e.NombreEstado });
                        return View(viewModel);
                    }

                    if (!string.IsNullOrEmpty(viewModel.Contraseña) && viewModel.Contraseña != viewModel.ConfirmarContraseña)
                    {
                        ModelState.AddModelError("", "La contraseña y la confirmación no coinciden.");
                        viewModel.Roles = _rolRepo.ObtenerRoles()
                            .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Rol });
                        viewModel.Estados = _estadoRepo.ObtenerEstados()
                            .Select(e => new SelectListItem { Value = e.Id.ToString(), Text = e.NombreEstado });
                        return View(viewModel);
                    }

                    string contraseñaFinal;

                    if (string.IsNullOrWhiteSpace(viewModel.Contraseña))
                    {
                        contraseñaFinal = _usuarioRepo.ObtenerUsuarioPorId(viewModel.Id).Contraseña; // ya encriptada
                    }
                    else
                    {
                        contraseñaFinal = viewModel.Contraseña; // la nueva sin encriptar aún
                    }

                    var usuarioDto = new UsuarioDto
                    {
                        Id = viewModel.Id,
                        Nombre = viewModel.Nombre,
                        Apellido = viewModel.Apellido,
                        Correo = viewModel.Correo,
                        IdRol = viewModel.IdRol,
                        IdEstado = viewModel.IdEstado,
                        Contraseña = contraseñaFinal
                    };


                    _usuarioService.EditarUsuario(usuarioDto,usuarioActual); // Actualizar usuario

                    return RedirectToAction("Usuarios");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            // Si hay errores de validación o excepción, volver a poblar los dropdowns
            viewModel.Roles = _rolRepo.ObtenerRoles()
                .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Rol });

            viewModel.Estados = _estadoRepo.ObtenerEstados()
                .Select(e => new SelectListItem { Value = e.Id.ToString(), Text = e.NombreEstado });

            return View(viewModel);
        }


        public ActionResult Eliminar(int id)
        {
            var usuario = _usuarioRepo.ObtenerUsuarioPorId (id); // este método debe existir
            if (usuario == null)
            {
                return RedirectToAction("Usuarios", new { mensaje = "Usuario no encontrado" });
            }
            return View(usuario);
        }

        // POST: Usuario/Eliminar
        [HttpPost, ActionName("Eliminar")]
        public ActionResult EliminarConfirmado(int id)
        {
            string usuarioActual = Session["Correo"]?.ToString();
            _usuarioRepo.EliminarUsuario(id, usuarioActual);
            return RedirectToAction("Usuarios");
        }

    }


}