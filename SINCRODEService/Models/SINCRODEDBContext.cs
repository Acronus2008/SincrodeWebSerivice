using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SINCRODEService.Config;
using System.IO;

namespace SINCRODEService.Models
{
    public partial class SINCRODEDBContext : DbContext
    {
        public SINCRODEDBContext()
        {
        }

        public SINCRODEDBContext(DbContextOptions<SINCRODEDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TblEmpleados> TblEmpleados { get; set; }
        public virtual DbSet<TblMarcajeprocesado> TblMarcajeprocesado { get; set; }
        public virtual DbSet<TblProcesos> TblProcesos { get; set; }
        public virtual DbSet<TblProcesoslog> TblProcesoslog { get; set; }
        public virtual DbSet<TblSuperiorKiosko> TblSuperiorKiosko { get; set; }
        public virtual DbSet<TblAbsentismoProcesado> TblAbsentismoProcesado { get; set; }
        public virtual DbSet<TblCodigosAusencias> TblCodigosAusencias { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfiguration config = ConfigHelper.GetConfiguration();

            if (!optionsBuilder.IsConfigured)
            {
                //"Server=yenety-laptop;Initial Catalog=SINCRODEDB;User ID=sincrouser;Password=sincrouser@123;"
                optionsBuilder.UseSqlServer(config["SQLConexion"]);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:DefaultSchema", "db_datareader");

            #region TBL_EMPLEADOS
            modelBuilder.Entity<TblEmpleados>(entity =>
            {
                entity.HasKey(e => e.IdEmp);

                entity.ToTable("TBL_EMPLEADOS", "dbo");

                entity.Property(e => e.IdEmp)
                    .HasColumnName("ID_EMP")
                    .HasComment("Id del empleado. Se utilizará como identificativo único del registro")
                    .ValueGeneratedNever();

                entity.Property(e => e.NombreEmp)
                    .IsRequired()
                    .HasColumnName("Nombre_EMP")
                    .HasMaxLength(100)
                    .HasComment("Contiene el nombre del empleado");

                entity.Property(e => e.ApellidosEmp)
                    .IsRequired()
                    .HasColumnName("Apellidos_EMP")
                    .HasMaxLength(100)
                    .HasComment("Contiene el Apellido del empleado");

                entity.Property(e => e.DniEmp)
                    .IsRequired()
                    .HasColumnName("DNI_EMP")
                    .HasMaxLength(20)
                    .HasComment("Identificación fiscal del empleado");

                entity.Property(e => e.NumeroEmp)
                    .IsRequired()
                    .HasColumnName("Numero_EMP")
                    .HasMaxLength(20)
                    .HasComment("Número único de empleado");

                entity.Property(e => e.IdoracleEmp)
                    .IsRequired()
                    .HasColumnName("IDOracle_EMP")
                    .HasMaxLength(20)
                    .HasComment("Identificador del empleado en la BD de Oracle");

                entity.Property(e => e.DniSuperior)
                    .HasColumnName("DNI_Sup")
                    .HasMaxLength(20)
                    .HasComment("Identificación fiscal del supervisor");

                entity.Property(e => e.CodcenEmp)
                    .HasColumnName("CODCEN_EMP")
                    .HasMaxLength(50)
                    .HasComment("Código identificativo del Centro de Trabajo");

                entity.Property(e => e.PNRSupEmp)
                    .HasColumnName("PNRSup_EMP")
                    .HasMaxLength(20)
                    .HasComment("");

                entity.Property(e => e.NombresupEmp)
                    .HasColumnName("NombreSup_EMP")
                    .HasMaxLength(50)
                    .HasComment("");

                entity.Property(e => e.ApellidossupEmp)
                    .HasColumnName("ApellidosSup_EMP")
                    .HasMaxLength(50)
                    .HasComment("");

                entity.Property(e => e.UbicenEmp)
                    .HasColumnName("UbiCentrabajo_EMP")
                    .HasMaxLength(30)
                    .HasComment("Nomenclatura de la ubicación del Centro de trabajo");

                entity.Property(e => e.DesccentrabajoEmp)
                    .HasColumnName("DescCentrabajo_EMP")
                    .HasMaxLength(50)
                    .HasComment("");

                entity.Property(e => e.CoddepEmp)
                    .HasColumnName("CodDept_EMP")
                    .HasMaxLength(50)
                    .HasComment("Código del Departamento del empleado");

                entity.Property(e => e.DescdepEmp)
                    .HasColumnName("DescDpto_EMP")
                    .HasMaxLength(50)
                    .HasComment("Descripción o Nombre del Departamento del empleado");

                entity.Property(e => e.CodsociedadEmp)
                    .HasColumnName("CodSociedad_EMP")
                    .HasMaxLength(20)
                    .HasComment("");

                entity.Property(e => e.DescsociedadEmp)
                    .HasColumnName("DescSociedad_EMP")
                    .HasMaxLength(50)
                    .HasComment("");

                entity.Property(e => e.CodnegocioEmp)
                    .HasColumnName("CodNegocio_EMP")
                    .HasMaxLength(30)
                    .HasComment("");

                entity.Property(e => e.Descnegocio)
                    .HasColumnName("DescNegocio")
                    .HasMaxLength(50)
                    .HasComment("");

                entity.Property(e => e.CodsubnegocioEmp)
                    .HasColumnName("CodSubnegocio_EMP")
                    .HasMaxLength(30)
                    .HasComment("");

                entity.Property(e => e.DescsubnegocioEmp)
                    .HasColumnName("DescSubnegocio_EMP")
                    .HasMaxLength(50)
                    .HasComment("");

                entity.Property(e => e.PorcenjornadaEmp)
                    .HasColumnName("PorcenJornada_EMP")
                    .HasColumnType("double")
                    .HasComment("");

                entity.Property(e => e.JornlaboralFestivo)
                    .HasColumnName("JornLaboral_Festivo")
                    .HasColumnType("int")
                    .HasComment("Indica si la jornada laboral es festiva");

                entity.Property(e => e.JornlaboralLunes)
                    .HasColumnName("JornLaboralLunes")
                    .HasColumnType("int")
                    .HasComment("");

                entity.Property(e => e.JornlaboralMartes)
                    .HasColumnName("JornLaboralMartes")
                    .HasColumnType("int")
                    .HasComment("");

                entity.Property(e => e.JornlaboralMiercoles)
                    .HasColumnName("JornLaboralMiercoles")
                    .HasColumnType("int")
                    .HasComment("");

                entity.Property(e => e.JornlaboralJueves)
                    .HasColumnName("JornLaboralJueves")
                    .HasColumnType("int")
                    .HasComment("");

                entity.Property(e => e.JornlaboralViernes)
                    .HasColumnName("JornLaboralViernes")
                    .HasColumnType("int")
                    .HasComment("");

                entity.Property(e => e.JornlaboralSabado)
                    .HasColumnName("JornLaboralSabado")
                    .HasColumnType("int")
                    .HasComment("");

                entity.Property(e => e.JornlaboralDomingo)
                    .HasColumnName("JornLaboralDomingo")
                    .HasColumnType("int")
                    .HasComment("");

                entity.Property(e => e.Ad)
                    .HasColumnName("AD")
                    .HasMaxLength(50)
                    .HasComment("Columna sin nombre");

                entity.Property(e => e.TipocontratoEmp)
                    .HasColumnName("TipoContrato_EMP")
                    .HasColumnType("int")
                    .HasComment("");

                entity.Property(e => e.CodcontratoEmp)
                    .HasColumnName("CodContrato_EMP")
                    .HasColumnType("string")
                    .HasComment("");

                entity.Property(e => e.EmailEmp)
                    .HasColumnName("Email_EMP")
                    .HasColumnType("string")
                    .HasComment("");
                
                entity.Property(e => e.CojornadaEmp)
                    .HasColumnName("CoJornadaEsp_EMP")
                    .HasColumnType("int")
                    .HasComment("");
            });
            #endregion

            #region TBL_MARCAJEPROCESADO
            modelBuilder.Entity<TblMarcajeprocesado>(entity =>
            {
                entity.HasKey(e => e.IdMar);

                entity.ToTable("TBL_MARCAJEPROCESADO", "dbo");

                entity.Property(e => e.IdMar)
                    .HasColumnName("ID_MAR")
                    .HasComment("Id del marcaje registrado. Se utilizará como identificativo único del registro")
                    .ValueGeneratedNever();

                entity.Property(e => e.IdEmp)
                    .HasColumnName("ID_EMP")
                    .HasComment("Id del empleado. Se relaciona con el de la tabla TBL_EMPLEADOS");

                entity.Property(e => e.IdPro)
                    .HasColumnName("ID_PRO")
                    .HasComment("Identificador del Proceso realizado");

                entity.Property(e => e.FechaMarcajeMar)
                    .HasColumnName("FechaMarcaje_MAR")
                    .HasColumnType("datetime")
                    .HasComment("Fecha de registro de acceso DD/MM/YYYY HH:MM");

                entity.Property(e => e.DniEmp)
                    .IsRequired()
                    .HasColumnName("DNI_EMP")
                    .HasMaxLength(20)
                    .HasComment("DNI del empleado según PersonasT");

                entity.Property(e => e.CodTarjetaMar)
                    .IsRequired()
                    .HasColumnName("CodTarjeta_MAR")
                    .HasMaxLength(10)
                    .HasComment("Codigo de la tarjeta utilizada");

                entity.Property(e => e.IdLectorMar)
                    .IsRequired()
                    .HasColumnName("IdLector_MAR")
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasComment("Codigo del lector");

                entity.Property(e => e.NombreLectorMar)
                    .IsRequired()
                    .HasColumnName("NombreLector_MAR")
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasComment("Nombre del lector");

                //entity.Property(e => e.NumeroEmp)
                //    .IsRequired()
                //    .HasColumnName("Numero_EMP")
                //    .HasMaxLength(20)
                //    .HasComment("Numero de Empleado obtenido de la consulta a Dassnet");

                //entity.Property(e => e.NumeroMensajeMar)
                //    .HasColumnName("NumeroMensaje_MAR")
                //    .HasComment("Número del mensaje");

                //entity.Property(e => e.TextoMensajeMar)
                //    .IsRequired()
                //    .HasColumnName("TextoMensaje_MAR")
                //    .HasMaxLength(60)
                //    .IsUnicode(false)
                //    .HasComment("Texto del Mensaje");

                entity.HasOne(d => d.IdEmpNavigation)
                    .WithMany(p => p.TblMarcajeprocesado)
                    .HasForeignKey(d => d.IdEmp)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TBL_MARCAJEPROCESADO_TBL_EMPLEADOS");

                entity.HasOne(d => d.IdProNavigation)
                    .WithMany(p => p.TblMarcajeprocesado)
                    .HasForeignKey(d => d.IdPro)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TBL_MARCAJEPROCESADO_TBL_PROCESOS");
            });
            #endregion

            #region TBL_PROCESOS
            modelBuilder.Entity<TblProcesos>(entity =>
            {
                entity.HasKey(e => e.IdPro);

                entity.ToTable("TBL_PROCESOS", "dbo");

                entity.Property(e => e.IdPro)
                    .HasColumnName("ID_PRO")
                    .HasComment("Id del proceso realizado. Se utilizará como identificativo único del registro")
                    .ValueGeneratedNever();

                entity.Property(e => e.AutoPro)
                    .IsRequired()
                    .HasColumnName("AUTO_PRO")
                    .HasDefaultValueSql("((1))")
                    .HasComment("Si el proceso es automático o no, 1- SINCRONIZADORDE ó 2- VIEWERDE");

                entity.Property(e => e.EmpleadosPro)
                    .HasColumnName("Empleados_PRO")
                    .HasComment("Número de empleados procesados");

                entity.Property(e => e.ErroresPro)
                    .HasColumnName("Errores_PRO")
                    .HasComment("Numero de errores, 0 en caso de éxito");

                entity.Property(e => e.FechaFinPro)
                    .HasColumnName("FechaFin_PRO")
                    .HasColumnType("datetime")
                    .HasComment("Fecha y hora en que se finaliza el proceso DD/MM/YYYY HH:MM");

                entity.Property(e => e.FechaIniPro)
                    .HasColumnName("FechaIni_PRO")
                    .HasColumnType("datetime")
                    .HasComment("Fecha y hora en que se inicia el proceso DD/MM/YYYY HH:MM");

                entity.Property(e => e.RegistrosPro)
                    .HasColumnName("Registros_PRO")
                    .HasComment("Numero de registros procesados");

                entity.Property(e => e.TipoPro)
                    .IsRequired()
                    .HasColumnName("TIPO_PRO")
                    .HasDefaultValueSql("((1))")
                    .HasComment("Si el proceso es marcajes o no, 1- Marcajes ó 2- Absentismos");
            });
            #endregion

            #region TBL_PROCESOSLOG
            modelBuilder.Entity<TblProcesoslog>(entity =>
            {
                entity.HasKey(e => e.IdProlog);

                entity.ToTable("TBL_PROCESOSLOG", "dbo");

                entity.Property(e => e.IdProlog)
                    .HasColumnName("ID_PROLOG")
                    .HasComment("Id del log registrado tras la ejecución de un proceso. Se utilizará como identificativo único del registro")
                    .ValueGeneratedNever();

                entity.Property(e => e.DescProlog)
                    .HasColumnName("DESC_PROLOG")
                    .HasColumnType("text")
                    .HasComment("Descripción del Error en el empleado");

                entity.Property(e => e.ExcProlog)
                    .HasColumnName("EXC_PROLOG")
                    .HasColumnType("text")
                    .HasComment("Descripción de la Excepción del error en la operación realizada");

                entity.Property(e => e.FechaIniPro)
                    .HasColumnName("FechaIni_PRO")
                    .HasColumnType("datetime")
                    .HasComment("Fecha y hora en que se inicia el proceso");

                entity.Property(e => e.IdEmp)
                    .HasColumnName("ID_EMP")
                    .HasComment("Id del empleado según TBL_EMPLEADO");

                entity.Property(e => e.IdPro)
                    .HasColumnName("ID_PRO")
                    .HasComment("Identificador del Proceso realizado");

                entity.HasOne(d => d.IdEmpNavigation)
                    .WithMany(p => p.TblProcesoslog)
                    .HasForeignKey(d => d.IdEmp)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TBL_PROCESOSLOG_TBL_EMPLEADOS");

                entity.HasOne(d => d.IdProNavigation)
                    .WithMany(p => p.TblProcesoslog)
                    .HasForeignKey(d => d.IdPro)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TBL_PROCESOSLOG_TBL_PROCESOS");
            });
            #endregion

            #region TBL_SUPERIOR_KIOSKO
            modelBuilder.Entity<TblSuperiorKiosko>(entity =>
            {
                entity.HasKey(e => e.DniSup);

                entity.ToTable("TBL_SUPERIOR_KIOSKO", "dbo");

                entity.Property(e => e.DniSup)
                    .HasColumnName("DNI_Sup")
                    .HasMaxLength(20)
                    .HasComment("Dni del Superior");

                entity.Property(e => e.CodKiosko)
                    .HasColumnName("CodKiosko")
                    .HasColumnType("int")
                    .HasComment("Codigo de kiosko");
            });
            #endregion

            #region TBL_ABSENTISMOPROCESADO
            modelBuilder.Entity<TblAbsentismoProcesado>(entity =>
            {
                entity.HasKey(e => e.IdAbs);

                entity.ToTable("TBL_ABSENTISMOPROCESADO", "dbo");

                entity.Property(e => e.IdAbs)
                   .HasColumnName("ID_ABS")
                   .HasComment("Id del absentismo. Se utilizará como identificativo único del registro")
                   .ValueGeneratedNever();

                entity.Property(e => e.IdEmp)
                     .HasColumnName("ID_EMP")
                     .HasComment("Id del empleado según TBL_EMPLEADO");

                entity.Property(e => e.DniEmp)
                    .IsRequired()
                    .HasColumnName("DNI_EMP")
                    .HasMaxLength(20)
                    .HasComment("DNI del empleado según PersonasT");

                entity.Property(e => e.IdPro)
                    .HasColumnName("ID_PRO")
                    .HasComment("Identificador del Proceso realizado");

                entity.Property(e => e.FechaInicio)
                    .HasColumnName("FechaInicio")
                    .HasColumnType("datetime")
                    .HasComment("Fecha de inicio de la ausencia DD/MM/YYYY");

                entity.Property(e => e.FechaFin)
                    .HasColumnName("FechaFin")
                    .HasColumnType("datetime")
                    .HasComment("Fecha de fin de la ausencia DD/MM/YYYY");

                entity.Property(e => e.CodAusencia)
                    .HasColumnName("CodAusencia")
                    .HasMaxLength(10)
                    .HasComment("Codigo de la  ausencia");

                entity.HasOne(d => d.IdEmpNavigation)
                    .WithMany(p => p.TblAbsentismoProcesado)
                    .HasForeignKey(d => d.IdEmp)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TBL_PROCESOSLOG_TBL_EMPLEADOS");

                entity.HasOne(d => d.IdProNavigation)
                    .WithMany(p => p.TblAbsentismoProcesado)
                    .HasForeignKey(d => d.IdPro)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TBL_PROCESOSLOG_TBL_PROCESOS");
            });
            #endregion

            #region TBL_CODIGOS_AUSENCIAS
            modelBuilder.Entity<TblCodigosAusencias>(entity =>
            {
                entity.HasKey(e => e.IdAus);

                entity.ToTable("TBL_CODIGOS_AUSENCIAS", "dbo");

                entity.Property(e => e.IdAus)
                   .HasColumnName("ID_AUS")
                   .HasMaxLength(3)
                   .HasComment("Id del absentismo. Se utilizará como identificativo único del registro")
                   .ValueGeneratedNever();

                entity.Property(e => e.CodAusencia)
                    .HasColumnName("CodAusencia")
                    .HasMaxLength(10)
                    .HasComment("Codigo de la  ausencia");
            });
            #endregion

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
