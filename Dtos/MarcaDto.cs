using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InverPaper.Dtos
{
    public class MarcaDto
    {
        public int Id { get; set; }
        public string NombreMarca { get; set; } = string.Empty;
        public int IdEstado { get; set; }
        public string NombreEstado { get; set; } = string.Empty;
        public int Response { get; set; }
        public string Mensaje { get; set; } = String.Empty;
    }
}