using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InverPaper.Dtos
{
    public class MetodoPagoDto
    {
        public int Id { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public int Response { get; set; }
        public string Mensaje { get; set; } = String.Empty;
    }
}