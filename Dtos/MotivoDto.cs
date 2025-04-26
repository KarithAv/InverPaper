using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InverPaper.Dtos
{
    public class MotivoDto
    {
        public int Id { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public int Response { get; set; }
        public string Mensaje { get; set; } = String.Empty;
    }
}