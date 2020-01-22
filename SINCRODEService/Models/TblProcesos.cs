using System;
using System.Collections.Generic;

namespace SINCRODEService.Models
{
    public partial class TblProcesos
    {
        public TblProcesos()
        {
            TblMarcajeprocesado = new HashSet<TblMarcajeprocesado>();
            TblProcesoslog = new HashSet<TblProcesoslog>();
        }

        public int IdPro { get; set; }
        public DateTime FechaIniPro { get; set; }
        public DateTime FechaFinPro { get; set; }
        public int RegistrosPro { get; set; }
        public int EmpleadosPro { get; set; }
        public int ErroresPro { get; set; }
        public bool? AutoPro { get; set; }

        public bool? AbsentismosPro { get; set; }

        public virtual ICollection<TblMarcajeprocesado> TblMarcajeprocesado { get; set; }
        public virtual ICollection<TblProcesoslog> TblProcesoslog { get; set; }
    }
}
