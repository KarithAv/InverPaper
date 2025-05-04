using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InverPaper.Models
{
    public class RecuperarContraseñaViewModel
    {
        [Required, EmailAddress]
        public string Correo { get; set; }
    }
}