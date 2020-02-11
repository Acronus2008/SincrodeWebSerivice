using System;
using System.Collections.Generic;
using System.Text;

namespace SINCRODEService.Models
{
    public partial class TblAbsentismoProcesado
    {
        public int IdAbs { get; set; }
        public int IdEmp { get; set; }
        public int IdPro { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string CodAusencia { get; set; }
        public string DniEmp { get; set; }

        public virtual TblEmpleados IdEmpNavigation { get; set; }
        public virtual TblProcesos IdProNavigation { get; set; }
    }
}
