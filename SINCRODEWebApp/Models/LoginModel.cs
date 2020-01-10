using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SINCRODEWebApp.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage ="El usuario es requerido")]
        [Display(Name ="Usuario")]
        public string Username { get; set; }

        [Required(ErrorMessage ="La contraseña es requerida")]
        [Display(Name ="Contraseña")]
        public string Password { get; set; }
    }
}
