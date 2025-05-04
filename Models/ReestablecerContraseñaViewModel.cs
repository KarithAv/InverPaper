using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InverPaper.Models
{
    public class ReestablecerContraseñaViewModel
    {
            public string Token { get; set; }
            [Required(ErrorMessage = "La contraseña es obligatoria.")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
            [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).+$", ErrorMessage = "La contraseña debe contener letras y números.")]
            public string NuevaContraseña { get; set; }

           [Required(ErrorMessage = "Debe confirmar la contraseña.")]
           [DataType(DataType.Password)]
           [Display(Name = "Confirmar Contraseña")]
           [System.ComponentModel.DataAnnotations.Compare("NuevaContraseña", ErrorMessage = "Las contraseñas no coinciden.")]
           public string ConfirmarContraseña { get; set; }

    }
}