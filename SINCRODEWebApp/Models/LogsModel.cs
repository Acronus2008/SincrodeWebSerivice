using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SINCRODEWeb.Models
{
    public class LogsModel
    {
        [Display(Name ="Nombre y apellidos")]
        public string Employed { get; set; }
        [Display(Name ="Inicio")]
        public DateTime FechaInicioPro { get; set; }
        [Display(Name ="Descripcion de log")]
        public string DescProlog { get; set; }
        [Display(Name ="Errores")]
        public string ExcProlog { get; set; }
    }
}
