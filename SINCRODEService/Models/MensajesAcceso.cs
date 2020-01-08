using System.Collections.Generic;

namespace SINCRODEService.Models
{
    public partial class MensajesAcceso
    {
        public int Id { get; set; }
        public string FechaYhora { get; set; }
        public string FechaYhoraLlegada { get; set; }
        public string CodTarjeta { get; set; }
        public string CodEmpresa { get; set; }
        public string CodTarjetaPortatil { get; set; }
        public string Matricula { get; set; }
        public string NombrePersona { get; set; }
        public int? NumeroMensaje { get; set; }
        public string Mensaje { get; set; }
        public int? ValorPresencia { get; set; }
        public string TextoPresencia { get; set; }
        public int? TipoMensaje { get; set; }
        public string NombreLector { get; set; }
        public int? TipoDispositivo { get; set; }
        public string Oculto { get; set; }
        public int? FkTarjeta { get; set; }
        public int? FkTarjetaPortatil { get; set; }
        public int? FkPersona { get; set; }
        public int? FkLector { get; set; }
        public int? FkDispositivo { get; set; }
        public int? TipoMensajeBase { get; set; }
        public int? FkVisitante { get; set; }
        public int? FkVehiculo { get; set; }
        public string CodTarjetaLargo { get; set; }
        public int? TipoTarjeta { get; set; }
        public string Matricula2 { get; set; }
        public string FechaYhoraMatricula2 { get; set; }
        public string NombreEmpresa { get; set; }
        public int? CodAccesosProducido { get; set; }
        public int? FkSedeMensaje { get; set; }
        public int? Bonos { get; set; }
        public int? IncidenciaPresenciaV2 { get; set; }
        public string Opcional1 { get; set; }
        public int? IncidenciaSga { get; set; }
        public long? ExternId { get; set; }
        public int? TipoCausaMensaje { get; set; }

        public virtual ICollection<Tarjetas> Tarjetas { get; set; }
    }
}
