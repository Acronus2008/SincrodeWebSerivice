using System;

namespace SINCRODEService.Models
{
    public partial class TblMarcajeprocesado
    {
        public int IdMar { get; set; }
        public int IdEmp { get; set; }
        public int IdPro { get; set; }
        public DateTime FechaMarcajeMar { get; set; }
        public string DniEmp { get; set; }
        public string CodTarjetaMar { get; set; }
        public string IdLectorMar { get; set; }
        public string NombreLectorMar { get; set; }
        //public string NumeroEmp { get; set; }
        //public int NumeroMensajeMar { get; set; }
        //public string TextoMensajeMar { get; set; }

        public virtual TblEmpleados IdEmpNavigation { get; set; }
        public virtual TblProcesos IdProNavigation { get; set; }
    }
}
