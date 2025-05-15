using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InverPaper.Repositorios;
using InverPaper.Dtos;
using InverPaper.Utilidades;
using System.Text.RegularExpressions;

namespace InverPaper.Servicios
{

    public class UsuarioServicio
    {
        private UsuarioRepositorio _repo = new UsuarioRepositorio ();
        private EncriptadorUtilidad _encriptador = new EncriptadorUtilidad();
        public UsuarioDto Login(string correo, string contraseña)
        {
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(contraseña))
            {
                return new UsuarioDto
                {
                    Mensaje = "Los campos no pueden estar vacíos.",
                    Respuesta = 0
                };
            }

            return _repo.Login(correo, contraseña);
        }
//-------------------------------------------------------------------------------------------------------------
        public void CrearUsuario(UsuarioDto usuarioDto, string usuarioActual)
        {
            // Validar campos
            if (string.IsNullOrWhiteSpace(usuarioDto.Nombre) || string.IsNullOrWhiteSpace(usuarioDto.Apellido))
            {
                throw new ArgumentException("El nombre y apellido son obligatorios.");
            }

            if (string.IsNullOrWhiteSpace(usuarioDto.Correo))
            {
                throw new ArgumentException("El correo es obligatorio.");
            }

            var correoRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$"; // Expresión regular para validar correo
            if (!Regex.IsMatch(usuarioDto.Correo, correoRegex))
            {
                throw new ArgumentException("El formato del correo no es válido.");
            }

            if (string.IsNullOrWhiteSpace(usuarioDto.Contraseña) || usuarioDto.Contraseña.Length < 6)
            {
                throw new ArgumentException("La contraseña debe tener al menos 6 caracteres.");
            }

            // Validar si ya existe un usuario con ese correo
            if (_repo.UsuarioExiste(usuarioDto.Correo))
            {
                throw new ArgumentException("Ya existe un usuario con ese correo.");
            }
            // Crear usuario
            _repo.CrearUsuario(usuarioDto, usuarioActual);
        }
 //--------------------------------------------------------------------------------------------------------------------------------------------
        public void EditarUsuario(UsuarioDto usuarioDto, string usuarioActual)
        {
            // Validar los campos antes de editar
            if (usuarioDto.Id <= 0)
            {
                throw new ArgumentException("El ID del usuario es inválido.");
            }

            if (string.IsNullOrWhiteSpace(usuarioDto.Nombre) || string.IsNullOrWhiteSpace(usuarioDto.Apellido))
            {
                throw new ArgumentException("El nombre y apellido son obligatorios.");
            }

            if (string.IsNullOrWhiteSpace(usuarioDto.Correo))
            {
                throw new ArgumentException("El correo es obligatorio.");
            }

            var correoRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$"; // Expresión regular para validar correo
            if (!Regex.IsMatch(usuarioDto.Correo, correoRegex))
            {
                throw new ArgumentException("El formato del correo no es válido.");
            }

            var usuarioExistente = _repo.UsuarioExisteEditar(usuarioDto.Correo, usuarioDto.Id);
            if (usuarioExistente)
            {
                throw new ArgumentException("Ya existe otro usuario con ese correo.");
            }


            // Solo encriptar si viene nueva (no encriptada)
            if (!usuarioDto.Contraseña.StartsWith("$2")) // bcrypt inicia con $2a o $2b
            {
                usuarioDto.Contraseña = EncriptadorUtilidad.EncriptarPassword(usuarioDto.Contraseña);
            }

            _repo.EditarUsuario(usuarioDto,usuarioActual);

        }

        public void EliminarUsuario(int id, string usuarioActual)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El ID del usuario es inválido.");
            }

            _repo.EliminarUsuario(id, usuarioActual);
        }
    }
}


