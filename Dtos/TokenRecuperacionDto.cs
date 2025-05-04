using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InverPaper.Dtos
{
    public class TokenRecuperacionDto
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public bool Usado { get; set; }
        public DateTime FechaExpiracion { get; set; }
    }
}