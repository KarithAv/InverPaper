using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InverPaper.Dtos
{
    public class UsuarioDto
    {
           public int Id { get; set; }
           public string Nombre { get; set; }= String.Empty;
           public string Apellido { get; set; } = String.Empty;
           public string Correo { get; set; } = String.Empty;
           public string Contraseña { get; set; } = String.Empty;
           public int IdRol { get; set; }
        public string NombreRol { get; set; } = String.Empty;
        public int IdEstado { get; set; }
        public string NombreEstado { get; set; } = String.Empty;
        public string Mensaje { get; set; } = String.Empty;
        public int Respuesta { get; set; }
    }
}