using System;

namespace SINCRODEService.Models
{
    public class MarcajeEmpleado
    {
        public int Id { get; set; }
        public DateTime FechayHora { get; set; }
        public string DNI { get; set; }
        public string CodTarjeta { get; set; }
        public int CodLector { get; set; }
        public string Nombre { get; set; }
    }
}
