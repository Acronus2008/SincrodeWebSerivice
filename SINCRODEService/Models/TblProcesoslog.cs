using System;

namespace SINCRODEService.Models
{
    public partial class TblProcesoslog
    {
        public int IdProlog { get; set; }
        public int IdPro { get; set; }
        public int IdEmp { get; set; }
        public DateTime FechaIniPro { get; set; }
        public string DescProlog { get; set; }
        public string ExcProlog { get; set; }

        public virtual TblEmpleados IdEmpNavigation { get; set; }
        public virtual TblProcesos IdProNavigation { get; set; }
    }
}
