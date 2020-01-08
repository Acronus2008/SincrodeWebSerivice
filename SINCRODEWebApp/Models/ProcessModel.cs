using SINCRODEWeb.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SINCRODEWebApp.Models
{
    public class ProcessModel
    {
        public int IdProcess { get; set; }

        [Display(Name ="Inicio")]
        public DateTime FechaInicio { get; set; }

        [Display(Name ="Fin")]
        public DateTime FechaFin { get; set; }

        [Display(Name ="Cantidad de registros")]
        public int Registros { get; set; }

        [Display(Name = "Cantidad de Empleados")]
        public int Empleados { get; set; }

        [Display(Name ="Cantidad de Errores")]
        public int Errores { get; set; }

        [Display(Name ="Proceso automatico")]
        public bool Auto { get; set; }

        public List<LogsModel> Logs { get; set; }
    }
}
