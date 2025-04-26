using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InverPaper.Models
{
    public class CrearUsuarioViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener al menos 3 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El apellido debe tener al menos 3 caracteres.")]
        public string Apellido { get; set; }

            [Required(ErrorMessage = "El correo es obligatorio.")]
            [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
            public string Correo { get; set; }

            [Required(ErrorMessage = "La contraseña es obligatoria.")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
            [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).+$", ErrorMessage = "La contraseña debe contener letras y números.")]
            public string Contraseña { get; set; }

            [Required(ErrorMessage = "El rol es obligatorio.")]
            public int IdRol { get; set; }

            [Required(ErrorMessage = "El estado es obligatorio.")]
            public int IdEstado { get; set; }

            // Para los dropdowns, necesitamos estas propiedades para poblar las listas
            public IEnumerable<SelectListItem> Roles { get; set; }
            public IEnumerable<SelectListItem> Estados { get; set; }
        }
}