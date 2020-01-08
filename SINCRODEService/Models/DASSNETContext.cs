using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SINCRODEService.Config;
using System.IO;

namespace SINCRODEService.Models
{
    public partial class DASSNETContext : DbContext
    {
        public DASSNETContext()
        {
        }

        public DASSNETContext(DbContextOptions<DASSNETContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Lectores> Lectores { get; set; }
        public virtual DbSet<MensajesAcceso> MensajesAcceso { get; set; }
        public virtual DbSet<PersonasT> PersonasT { get; set; }
        public virtual DbSet<Tarjetas> Tarjetas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfiguration config = ConfigHelper.GetConfiguration();

            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseSqlServer("Server=yenety-laptop;Initial Catalog=DASSNET;User ID=sincrouser;Password=sincrouser@123;");
                optionsBuilder.UseSqlServer(config["DassnetAccess"]);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Lectores>(entity =>
            {
                entity.Property(e => e.AccConfirmado)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.AccInhibido)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.AccesoDual)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.AlarmaFueraHorarioAcceso)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.AlarmaNipincorrecto)
                    .HasColumnName("AlarmaNIPIncorrecto")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.AlarmaTarjInvalida)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.AlarmaTarjetaCaducada)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.AlarmaViolacionApb)
                    .HasColumnName("AlarmaViolacionAPB")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.BateriaBaja)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Biestable)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.BiometricoConModos)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.CamaraEnlazadaMensajeAccesos)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Coaccion)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Codigo)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.DirVideoIp)
                    .HasColumnName("DirVideoIP")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.EntradasParoPorArmado)
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.EntradasParoPorBiestable)
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.EnviarTodasLasHuellas)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.EstadoHorario)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.EstadoLectorRemoto)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.LevSinPapeles)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.MensAcceso)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.MercPeligrosas)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.ModoLectorMatriculas)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.MsgDisplay)
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Observaciones)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Ocultar)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.OcultarLectorMatriculas)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.OcultarPorIndependientes)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Pass)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.PermisoAccesoListaNegra)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PermitirAperturaRemota)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PermitirImpulsoRemoto)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PrioridadFe).HasColumnName("PrioridadFE");

                entity.Property(e => e.PrioridadFha).HasColumnName("PrioridadFHA");

                entity.Property(e => e.PrioridadNi).HasColumnName("PrioridadNI");

                entity.Property(e => e.PrioridadTc).HasColumnName("PrioridadTC");

                entity.Property(e => e.PrioridadTi).HasColumnName("PrioridadTI");

                entity.Property(e => e.PrioridadVa).HasColumnName("PrioridadVA");

                entity.Property(e => e.PuntoSincronizacion)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.RecintoLsp)
                    .HasColumnName("RecintoLSP")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.TesteoPestillo)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.TipoDispositivoAutonomo)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.VideoIp)
                    .HasColumnName("VideoIP")
                    .HasMaxLength(1)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<MensajesAcceso>(entity =>
            {
                entity.Property(e => e.CodEmpresa)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CodTarjeta)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CodTarjetaLargo)
                    .HasMaxLength(26)
                    .IsUnicode(false);

                entity.Property(e => e.CodTarjetaPortatil)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.FechaYhora)
                    .HasColumnName("FechaYHora")
                    .HasMaxLength(14)
                    .IsUnicode(false);

                entity.Property(e => e.FechaYhoraLlegada)
                    .HasColumnName("FechaYHoraLlegada")
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.FechaYhoraMatricula2)
                    .HasColumnName("FechaYHoraMatricula2")
                    .HasMaxLength(14)
                    .IsUnicode(false);

                entity.Property(e => e.IncidenciaSga).HasColumnName("IncidenciaSGA");

                entity.Property(e => e.Matricula)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Matricula2)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Mensaje)
                    .HasMaxLength(180)
                    .IsUnicode(false);

                entity.Property(e => e.NombreEmpresa)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.NombreLector)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.NombrePersona)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.Oculto)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Opcional1)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.TextoPresencia)
                    .HasMaxLength(50)
                    .IsUnicode(false);

               entity.Property(e => e.Bonos).IsRequired(false);
                    
            });

            modelBuilder.Entity<PersonasT>(entity =>
            {
                entity.Property(e => e.Apellido1)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Apellido2)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Apellidos)
                    .HasMaxLength(80)
                    .IsUnicode(false);

                entity.Property(e => e.Autorizada)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Cargo)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Categoria)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.CodPostal)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.CodigoPersona)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.ColorCoche)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ColorCoche2)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ColorCoche3)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ColorCoche4)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ColorCoche5)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Contrata)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Destino)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.Direccion)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Dni)
                    .HasColumnName("DNI")
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.DocumentosOk)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Edificio)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.EsSupervisorPresencia)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.EstadoActual)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.EstadoCivil)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Extension)
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.FechaAlta)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.FechaBaja)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.FechaExpedicion)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.FechaFinListaNegra)
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.FechaInicioListaNegra)
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.FechaListaNegra)
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.FkImgDni1).HasColumnName("FkImgDNI1");

                entity.Property(e => e.FkImgDni2).HasColumnName("FkImgDNI2");

                entity.Property(e => e.Fnacimiento)
                    .HasColumnName("FNacimiento")
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.LugarNacimiento)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.MarcaCoche)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.MarcaCoche2)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.MarcaCoche3)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.MarcaCoche4)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.MarcaCoche5)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Matricula)
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.MatriculaCoche)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.MatriculaCoche2)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.MatriculaCoche3)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.MatriculaCoche4)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.MatriculaCoche5).HasMaxLength(10);

                entity.Property(e => e.ModeloCoche)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ModeloCoche2)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ModeloCoche3)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ModeloCoche4)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ModeloCoche5).HasMaxLength(20);

                entity.Property(e => e.MotivoListaNegra)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Movil)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Nacionalidad)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Nss)
                    .HasColumnName("NSS")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.Observaciones)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Pais)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ParametrosIntegracion)
                    .HasMaxLength(512)
                    .IsUnicode(false);

                entity.Property(e => e.PasarAnomina)
                    .HasColumnName("PasarANomina")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Pass)
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.Pendiente)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Planta)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Poblacion)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Provincia)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.PuestoListaNegra)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.RecibeVisitas)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Sexo)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.SigRevision)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Telefono)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.UltRevision)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.UsuarioAd)
                    .HasColumnName("UsuarioAD")
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.UsuarioListaNegra)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.Vip)
                    .HasColumnName("VIP")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Zona)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Tarjetas>(entity =>
            {
                entity.Property(e => e.AccesoApista)
                    .HasColumnName("AccesoAPista")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Actualizada)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.ArmadoDesarmado)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Aviso)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.BiestableAutonomos)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.BorradoPendiente)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.ChequeoPestillo)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.CodAlternativo)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.CodTarjeta)
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.CodTarjetaLargo)
                    .HasMaxLength(26)
                    .IsUnicode(false);

                entity.Property(e => e.EnviarAviso)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.EnviarHuella)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Estado)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.EstadoTarjetaDvc).HasColumnName("EstadoTarjetaDVC");

                entity.Property(e => e.FechaCambioEstado)
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.FechaEmision)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.FechaFin)
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.FechaInicio)
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.FechaLectorValidacion)
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.Fhmovimiento)
                    .HasColumnName("FHMovimiento")
                    .HasMaxLength(14)
                    .IsUnicode(false);

                entity.Property(e => e.FkHaccesos).HasColumnName("FkHAccesos");

                entity.Property(e => e.Grabada)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.GuidRegistroDvc)
                    .HasColumnName("GuidRegistroDVC")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Herramienta)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.HorarioLectoresDoc)
                    .HasColumnName("HorarioLectoresDOC")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Matricula)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Matricula2)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Matricula3)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Matricula4)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Matricula5)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ModoAccesoHuella)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.ModoAccesoMatricula)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Nip)
                    .HasColumnName("NIP")
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.NoDocumentosOk)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.NoSincIntercentros)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Observaciones)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.OcultarMovimientos)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Provisional)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.TarjetaAccesoParaVisita)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.TarjetaEspecial)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.TarjetaPortatil)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.TarjetaRonda)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.TarjetaVehiculo)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.TarjetaVisitaPermanente)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Telefono)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.TipoTarjeta)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.UltimaImpresion)
                    .HasMaxLength(14)
                    .IsUnicode(false);

                entity.Property(e => e.ValidacionPorPasoLector)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.VipenComedor)
                    .HasColumnName("VIPEnComedor")
                    .HasMaxLength(1)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
