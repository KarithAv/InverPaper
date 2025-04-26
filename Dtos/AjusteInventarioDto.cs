using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InverPaper.Dtos
{
    public class AjusteInventarioDto
    {
        public int Id { get; set; }
        public int IdProducto { get; set; }
        public int CantidadAjustada { get; set; }
        public DateTime FechaAjuste { get; set; }
        public int IdMotivo { get; set; }
        public int IdUsuario { get; set; }
        public Boolean Accion { get; set; }
        public string Comentarios { get; set; } = String.Empty;
        public string Mensaje { get; set; } = String.Empty;
        public int Respuesta { get; set; }
    }
}