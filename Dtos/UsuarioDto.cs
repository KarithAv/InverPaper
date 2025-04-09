using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InverPaper.Dtos
{
    public class UsuarioDto
    {
           public int Id { get; set; }
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string Correo { get; set; }
            public string Contraseña { get; set; }
            public int IdRol { get; set; }
            public int IdEstado { get; set; }
    }
}