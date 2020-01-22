using System.Collections.Generic;

namespace SINCRODEService.Models
{
    public partial class TblEmpleados
    {
        public TblEmpleados()
        {
            TblMarcajeprocesado = new HashSet<TblMarcajeprocesado>();
            TblProcesoslog = new HashSet<TblProcesoslog>();
        }

        public int IdEmp { get; set; }
        public string NombreEmp { get; set; }
        public string ApellidosEmp { get; set; }
        public string DniEmp { get; set; }
        public string NumeroEmp { get; set; }
        public string IdoracleEmp { get; set; }
        public string DniSuperior { get; set; }
        public string CodcenEmp { get; set; }
        public string UbicenEmp { get; set; }
        public string CoddepEmp { get; set; }
        public string DescdepEmp { get; set; }
        public string PNRSupEmp { get; set; }//todo
        public string NombresupEmp { get; set; }//todo
        public string ApellidossupEmp { get; set; }//todo
        public string CodnegocioEmp { get; set; }//todo
        public string CodsociedadEmp { get; set; }//todo
        public string CodsubnegocioEmp { get; set; }//todo
        public string DesccentrabajoEmp { get; set; }//todo
        public string Descnegocio { get; set; }//todo
        public string DescsociedadEmp { get; set; }//todo
        public string DescsubnegocioEmp { get; set; }//todo
        public double? PorcenjornadaEmp { get; set; }
        public int? JornlaboralFestivo { get; set; }
        public int? JornlaboralLunes { get; set; }//todo
        public int? JornlaboralMartes { get; set; }//todo
        public int? JornlaboralMiercoles { get; set; }//todo
        public int? JornlaboralJueves { get; set; }//todo
        public int? JornlaboralViernes { get; set; }//todo
        public int? JornlaboralSabado { get; set; }//todo
        public int? JornlaboralDomingo { get; set; }//todo
        public int? TipocontratoEmp { get; set; }
        public string Ad { get; set; }
        public string CodcontratoEmp { get; set; }
        public int? CojornadaEmp { get; set; }
        //Borradas
        public int? Coworking_EMP { get; set; }

        public virtual ICollection<TblMarcajeprocesado> TblMarcajeprocesado { get; set; }
        public virtual ICollection<TblProcesoslog> TblProcesoslog { get; set; }
    }
}
