using System;

namespace SINCRODEService.Models
{
    public class AusenciaEmpleado
    {
        public string DNI { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string CodAusencia { get; set; }
    }
}
