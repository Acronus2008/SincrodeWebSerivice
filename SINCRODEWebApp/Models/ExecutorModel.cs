using System;
using System.ComponentModel.DataAnnotations;

namespace SINCRODEWebApp.Models
{
    public class ExecutorModel
    {
        [Display(Name = "Fecha inicio")]
        [Required(ErrorMessage = "La fecha inicio es requerida")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [Display(Name = "Fecha fin")]
        [Required(ErrorMessage = "La fecha final es requerida")]
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }

        public bool Executed { get; set; }

        [Display(Name = "Proceso de absentismo")]
        public bool Absentismo { get; set; }

    }
}
