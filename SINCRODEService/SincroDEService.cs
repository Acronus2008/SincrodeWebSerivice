using System;
using System.IO;
using System.ServiceProcess;
using SINCRODEService.Models;
using System.Linq;
using System.Collections.Generic;
using static SINCRODEService.Program;
using static SINCRODEService.LogaFile;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Timers;
using System.Net;
using SINCRODEService.Config;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SINCRODEService
{
    class SincroDEService : ServiceBase
    {
        private const string LogFileLocation = @"C:\temp\servicelog.txt";
        private List<CamposLOGA> _listaLoga;
        public static List<CamposAbsentismo> _listaAbsentismo;

        Timer timer = new Timer();

        protected override void OnStart(string[] args)
        {
            Log("*****************************************************************************************************************");
            Log("Iniciando");
            base.OnStart(args);

            try
            {
                // Set up a timer that triggers.
                timer.Interval = Intervalo.SetNextIntervalo();
                Log("Primera fecha de ejecución: " + DateTime.Now.AddMilliseconds(timer.Interval).ToString("dd/MM/yyyy hh:mm:ss tt"));
                timer.Elapsed += OnTimer;
                timer.Start();
            }
            catch (Exception ex)
            {
                Log("Error iniciando el timer: " + ex.ToString());
            }

            //Ejecutar();

            Log("Iniciado");
        }

        protected override void OnStop()
        {
            Log("Deteniendo");
            base.OnStop();
            Log("Detenido");
            Log("*****************************************************************************************************************");
        }

        protected override void OnPause()
        {
            Log("Pausando");
            base.OnPause();
        }


        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            timer.Stop();

            Log("Ejecutando el Timer");

            Ejecutar();

            double proximoIntervalo = Intervalo.SetNextIntervalo();
            Log("Próxima fecha de ejecución " + DateTime.Now.AddMilliseconds(proximoIntervalo).ToString("dd/MM/yyyy hh:mm:ss tt"));
            timer.Interval = proximoIntervalo;
            timer.Start();
        }

        private string TruncateStr(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        //Obtiene el codigo de kiosko asociado a un empleado Supervisor
        private string ObtenerCodigoSupervisor( string codigoEmpleado )
        {
            using (var context = new SINCRODEDBContext())
            {
                //Si busca si existe un codigo para el supervisor 
                var super = context.TblSuperiorKiosko.FirstOrDefault( e => e.DniSup.Equals(codigoEmpleado) );
                if ( super == null )
                {
                    //Si no existe se inserta
                    var maxCodKiosko = context.TblSuperiorKiosko.Any() ? context.TblSuperiorKiosko.Max( e => e.CodKiosko ) : 0;
                    super = new TblSuperiorKiosko()
                    {
                        DniSup = codigoEmpleado,
                        CodKiosko = maxCodKiosko + 1
                    };
                    context.TblSuperiorKiosko.Add( super );
                    context.SaveChanges();
                }
                return super.CodKiosko.ToString().PadLeft(3, '0');
            }
        }

        public static void EliminarMarcajePendientes(int idProceso)
        {
            var context = new SINCRODEDBContext();
            {
                var marcajesPendientes = context.TblMarcajeprocesado.Where(mp => mp.IdPro == idProceso);
                foreach (var marcaje in marcajesPendientes)
                {
                    context.TblMarcajeprocesado.Remove(marcaje);
                }
                context.SaveChanges();
            }
        }

        //Método llamado cuando el sincrode está en producción de forma automática
        private void ProcesaAusencias()
        {
            var context = new SINCRODEDBContext();
            {
                //Tomo los datos necesarios para iniciar el proceso y salvarlo al final de procesar todas las ausencias
                DateTime lastdateprocess = context.TblProcesos.Any(p => !p.TipoPro)
                    ? context.TblProcesos.Where(p => !p.TipoPro).Max(p => p.FechaIniPro)
                    : new DateTime(2019, 10, 1);

                Log("Se procesa a partir de la última fecha procesada: " + lastdateprocess);

                ProcesaAusencias(lastdateprocess, DateTime.Now, true);
            }
        }

        //Método llamado desde la aplicacion web
        public static void ProcesaAusencias(DateTime fechaini, DateTime fechafin, bool AutoPro = true)
        {
            IConfiguration config = ConfigHelper.GetConfiguration();

            if (!AutoPro)
            {
                string carpetaDestino = config["RutaFileTemp"];
                string remoteFile = config["FileAbsentismos"];
                string fileName = remoteFile.Substring(remoteFile.LastIndexOf("/") + 1);

                _listaAbsentismo = ProcessAbsentismosWorkbook(carpetaDestino + "\\" + fileName);
            }


            //Recorro todos empleados de TBL_EMPLEADOS para consultar sus ausencias
            try
            {
                int maxidProceso;
                DateTime fechIniProceso = DateTime.Now;

                var context = new SINCRODEDBContext();
                {
                    var employees = context.TblEmpleados.ToList();

                    maxidProceso = context.TblProcesos.Any() ? context.TblProcesos.Max(p => p.IdPro) : 0;

                    //Inserto el proceso
                    var proceso = new TblProcesos()
                    {
                        IdPro = ++maxidProceso,
                        FechaIniPro = fechIniProceso,
                        FechaFinPro = DateTime.Now,
                        EmpleadosPro = employees.Count,
                        ErroresPro = 0,
                        AutoPro = AutoPro,
                        RegistrosPro = 0,
                        TipoPro = false
                    };
                    context.TblProcesos.Add(proceso);

                    int maxIdAbs = context.TblAbsentismoProcesado.Any()
                        ? context.TblAbsentismoProcesado.Max(p => p.IdAbs)
                        : 0;
                    int ausenciasCount = 0;
                    foreach (var employee in employees)
                    {
                        {
                            #region Obtener Ausencias

                            var ausenciasEmpleado = from a in _listaAbsentismo
                                                    where a.NifDni == employee.DniEmp && a.FechaFin.HasValue &&
                                                          ((a.FechaInicio >= fechaini && a.FechaInicio <= fechafin)||
                                                           (a.FechaFin.Value >= fechaini && a.FechaFin.Value <= fechafin))
                                                    select new AusenciaEmpleado
                                                    {
                                                        DNI = a.NifDni,
                                                        FechaInicio = a.FechaInicio,
                                                        FechaFin = a.FechaFin,
                                                        CodAusencia = a.CodAusencia
                                                    };
                            #endregion

                            #region Salvar Ausencias en la tabla TBL_ABSENTISMOPROCESADO
                            foreach (var ausencia in ausenciasEmpleado)
                            {
                                //TODO depende de la base de datos Dassnet
                                //Para cada empleado inserto el marcaje
                                var absentismoProcesado = new TblAbsentismoProcesado()
                                {
                                    IdAbs = ++maxIdAbs,
                                    IdEmp = employee.IdEmp,
                                    DniEmp = employee.DniEmp,
                                    IdPro = maxidProceso,
                                    FechaInicio = ausencia.FechaInicio,
                                    FechaFin = ausencia.FechaFin,
                                    CodAusencia = ausencia.CodAusencia
                                };
                                context.TblAbsentismoProcesado.Add(absentismoProcesado);
                                ausenciasCount++;
                            }
                        }
                    }

                    Log("Salvando los datos de absentismos");
                    context.SaveChanges();
                    Log("Salvados " + ausenciasCount + " ausencias");

                    //Llamar al ws de Evalos para enviar las ausencias
                    SendAusenciaToWS(context, maxidProceso, fechIniProceso);
                }
                #endregion
            }
            catch (Exception ex)
            {
                Log("#region Salvando en la tabla de ausencias: " + ex.ToString());
            }
        }

        public static void SendAusenciaToWS(SINCRODEDBContext sincrodecontext, int idPro, DateTime fIniPro)
        {
            IConfiguration config = ConfigHelper.GetConfiguration();

            Log("Comenzando el envio de ausencias al WS");
            //Mando a crear la ausencia en Evalos
            //Creo el json con los datos q debo enviarle al ws
            string jsonabsence;
            string wsEvalosMethod = config["EvalosAccess"] + "absence/";
            string username = config["EvalosUser"];
            string password = config["EvalosPassword"];

            int cantTotalRegistros = 0;
            int cantRegistroPro = 0;
            int cantErrores = 0;
            int maxidProlog;

            void EnviarAusenciaWS(int idEmpleado, int idAusencia, Absence absence)
            {
                TblProcesoslog procesoLog;

                jsonabsence = JsonConvert.SerializeObject(absence);
                if (jsonabsence != "[]")
                {
                    if (config["ShowDetailsLog"].ToUpper() == "TRUE")
                    {
                        Log("Json usado en el envío al WS de ausencia " + jsonabsence);
                    }

                    try
                    {
                        var httpWebResponse = WebServiceRest.PutPostRequest(wsEvalosMethod, username, password, jsonabsence, "PUT");

                        string messageLog;
                        string messageError = string.Empty;

                        StreamReader reader = new StreamReader(httpWebResponse.GetResponseStream());
                        string body = reader.ReadToEnd();

                        if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                        {
                            cantRegistroPro++;
                            //Si la ausencia se envió satisfactoriamente se borra de la tabla de absentismos procesados
                            var ausenciaToDelete = sincrodecontext.TblAbsentismoProcesado.FirstOrDefault(m => m.IdAbs == idAusencia);
                            if (ausenciaToDelete != null)
                            {
                                sincrodecontext.TblAbsentismoProcesado.Remove(ausenciaToDelete);
                            }
                            else
                            {
                                Log(string.Format("No fue posible encontrar registro con Id {0} en TblAbsentismoProcesado", idAusencia));
                            }

                            messageLog = httpWebResponse.StatusDescription;
                        }
                        else
                        {
                            Log("Respuesta de POST de Ausencia, empleado " + absence.CodeEmployee + " => " + body);

                            cantErrores++;
                            messageLog = httpWebResponse.StatusDescription;
                            messageError = httpWebResponse.StatusDescription;
                        }

                        //Salvo el log de los procesos
                        procesoLog = new TblProcesoslog()
                        {
                            IdProlog = ++maxidProlog,
                            IdPro = idPro,
                            IdEmp = idEmpleado,
                            FechaIniPro = fIniPro,
                            DescProlog = messageLog,
                            ExcProlog = messageError
                        };
                        sincrodecontext.TblProcesoslog.Add(procesoLog);
                        sincrodecontext.SaveChanges();

                    }
                    catch (Exception e)
                    {
                        Log("Error en el envio de POST de Ausencia: " + e.Message);
                    }
                }
            }

            maxidProlog = sincrodecontext.TblProcesoslog.Any() ? sincrodecontext.TblProcesoslog.Max(l => l.IdProlog) : 0;

            var empleadosXDistinct = (from m in sincrodecontext.TblAbsentismoProcesado select m.IdEmp).Distinct().ToList();

            foreach (var empleado in empleadosXDistinct)
            {
                var ausenciasEmpleado = (from m1 in sincrodecontext.TblAbsentismoProcesado select m1).Where(m => m.IdEmp == empleado).ToList();

                foreach (var ausenciaEmpleado in ausenciasEmpleado)
                {
                    if (ausenciaEmpleado.FechaFin.HasValue)
                    {
                        var idAusencia = sincrodecontext.TblCodigosAusencias.FirstOrDefault(a => a.CodAusencia == ausenciaEmpleado.CodAusencia).IdAus;
                        var absence = new Absence()
                        {
                            CodeEmployee = ausenciaEmpleado.DniEmp,
                            StartDate = ausenciaEmpleado.FechaInicio.Date.ToString("yyyyMMdd", CultureInfo.InvariantCulture),
                            EndDate = ausenciaEmpleado.FechaFin.Value.Date.ToString("yyyyMMdd", CultureInfo.InvariantCulture),
                            Incidence = idAusencia,
                        };
                        cantTotalRegistros += 1;

                        EnviarAusenciaWS(empleado, ausenciaEmpleado.IdAbs, absence);
                    }
                }

                var proceso = sincrodecontext.TblProcesos.Where(p => p.IdPro == idPro).FirstOrDefault();
                proceso.RegistrosPro = cantRegistroPro;
                proceso.ErroresPro = cantErrores;
                sincrodecontext.SaveChanges();
            }
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        private void Ejecutar()
        {
            IConfiguration config = ConfigHelper.GetConfiguration();

            #region Descargar y guardar fichero del FTP
            //string servidorftp = "ftp://gentalia.ddns.net/SincronizadorDE/DATOSLOGA_V2.xls";
            string servidorftp = config["FTPLoga"];
            string usuario = config["FTPuser"];
            string password = config["FTPPassword"];
            string carpetaDestino = config["RutaFileTemp"];// "D:\\InOut";
            string remoteFile = config["FileLoga"];

            string fileName = remoteFile.Substring(remoteFile.LastIndexOf("/") + 1);

            int NumberOfRetries = Convert.ToInt16(config["CantDeReintentos"]);
            int DelayOnRetry = 1000;
            DownloadFile(servidorftp, remoteFile, usuario, password, carpetaDestino);

            for (int i = 1; i <= NumberOfRetries; ++i)
            {
                try
                {
                    _listaLoga = ProcessLOGAWorkbook(carpetaDestino + "\\" + fileName);
                    Log("Fichero LOGA procesado");
                    break; // When done we can break loop
                }
                catch (IOException ex) when (i <= NumberOfRetries)
                {
                    // You may check error code to filter some exceptions, not every error
                    // can be recovered.
                    System.Threading.Thread.Sleep(DelayOnRetry);
                    if (i == NumberOfRetries)
                    {
                        Log("Error procesando el fichero LOGA: " + ex.ToString());
                    }
                }
            }

            if (Intervalo.nextTimeToExecute == 1)
            {
                //Si es la primera hora del día se intenta descargar el archivo de Absentismos
                remoteFile = config["FileAbsentismos"];
                fileName = remoteFile.Substring(remoteFile.LastIndexOf("/") + 1);

                DownloadFile(servidorftp, remoteFile, usuario, password, carpetaDestino);

                for (int i = 1; i <= NumberOfRetries; ++i)
                {
                    try
                    {
                        _listaAbsentismo = ProcessAbsentismosWorkbook(carpetaDestino + "\\" + fileName);
                        Log("Fichero de Absentismos procesado");
                        break; // When done we can break loop
                    }
                    catch (IOException ex) when (i <= NumberOfRetries)
                    {
                        // You may check error code to filter some exceptions, not every error
                        // can be recovered.
                        System.Threading.Thread.Sleep(DelayOnRetry);
                        if (i == NumberOfRetries)
                        {
                            Log("Error procesando el fichero de Absentismos: " + ex.ToString());
                        }
                    }
                }

            }
            #endregion

            #region Salvar empleado en la base de datos si no existe
            try
            {
                TblEmpleados employeeFound;
                int encontrados = 0;
                int creados = 0;
                using (var context = new SINCRODEDBContext())
                {
                    int maxidEmp = context.TblEmpleados.Any() ? context.TblEmpleados.Max(e => e.IdEmp) : 0;

                    Log("Salvando empleados en la BD SincroDE");
                    foreach (CamposLOGA campo in _listaLoga)
                    {
                        employeeFound = context.TblEmpleados.FirstOrDefault(s => s.DniEmp == campo.NifDni);

                        //Si no encuentra el empleado lo inserta
                        if (employeeFound != null)
                        {
                            employeeFound.NombreEmp = TruncateStr(campo.Nombre, 100);
                            employeeFound.ApellidosEmp = TruncateStr(campo.Apellidos, 100);
                            employeeFound.NumeroEmp = TruncateStr(campo.NoPersonal, 20);
                            employeeFound.IdoracleEmp = TruncateStr(campo.IdOracle, 20);
                            employeeFound.DniSuperior = TruncateStr(campo.DniSuperior, 20);
                            employeeFound.CodcenEmp = TruncateStr(campo.CodigoCentro, 50);
                            employeeFound.UbicenEmp = TruncateStr(campo.UbicacionCentroTrabajo, 30);
                            employeeFound.CoddepEmp = TruncateStr(campo.CodigoDepartamento, 50);
                            employeeFound.DescdepEmp = TruncateStr(campo.DescripcionCentroTrabajo, 50);
                            employeeFound.PNRSupEmp = TruncateStr(campo.PNRSupEmp, 20);
                            employeeFound.NombresupEmp = TruncateStr(campo.NombreResponsable, 50);
                            employeeFound.ApellidossupEmp = TruncateStr(campo.ApellidosResponsable, 50);
                            employeeFound.CodnegocioEmp = TruncateStr(campo.CodigoNegocio, 20);
                            employeeFound.CodsociedadEmp = TruncateStr(campo.CodigoSociedad, 20);
                            employeeFound.CodsubnegocioEmp = TruncateStr(campo.CodigoSubNegocio, 20);
                            employeeFound.DesccentrabajoEmp = TruncateStr(campo.DescripcionCentroTrabajo, 50);
                            employeeFound.Descnegocio = TruncateStr(campo.DescripcionNegocio, 50);
                            employeeFound.DescsociedadEmp = TruncateStr(campo.DescripcionSociedad, 50);
                            employeeFound.DescsubnegocioEmp = TruncateStr(campo.DescripcionSubNegocio, 50);
                            employeeFound.JornlaboralDomingo = campo.JornadaLaboralDomingo;
                            employeeFound.JornlaboralLunes = campo.JornadaLaboralLunes;
                            employeeFound.JornlaboralMartes = campo.JornadaLaboralMartes;
                            employeeFound.JornlaboralMiercoles = campo.JornadaLaboralMiercoles;
                            employeeFound.JornlaboralJueves = campo.JornadaLaboralJueves;
                            employeeFound.JornlaboralViernes = campo.JornadaLaboralViernes;
                            employeeFound.JornlaboralSabado = campo.JornadaLaboralSabado;
                            employeeFound.JornlaboralFestivo = campo.JornadaLaboralFestiva;
                            employeeFound.PorcenjornadaEmp = campo.PorcentajeReduccionJornada;
                            employeeFound.TipocontratoEmp = campo.TipoContrato;
                            employeeFound.CodcontratoEmp = campo.CodContratoEmp;
                            employeeFound.Ad = TruncateStr(campo.Ad, 50);
                            employeeFound.CojornadaEmp = campo.CoJornadaEmp;
                            employeeFound.EmailEmp = campo.EmailEmp;

                            context.TblEmpleados.Update(employeeFound);
                            encontrados++;
                        }
                        else
                        {
                            var empl = new TblEmpleados()
                            {
                                IdEmp = ++maxidEmp,
                                NombreEmp = TruncateStr(campo.Nombre, 100),
                                ApellidosEmp = TruncateStr(campo.Apellidos, 100),
                                DniEmp = TruncateStr(campo.NifDni, 20),
                                NumeroEmp = TruncateStr(campo.NoPersonal, 20),
                                IdoracleEmp = TruncateStr(campo.IdOracle, 20),
                                DniSuperior = TruncateStr(campo.DniSuperior, 20),
                                CodcenEmp = TruncateStr(campo.CodigoCentro, 50),
                                UbicenEmp = TruncateStr(campo.UbicacionCentroTrabajo, 30),
                                CoddepEmp = TruncateStr(campo.CodigoDepartamento, 50),
                                DescdepEmp = TruncateStr(campo.DescripcionCentroTrabajo, 50),
                                PNRSupEmp = TruncateStr(campo.PNRSupEmp, 20),
                                NombresupEmp = TruncateStr(campo.NombreResponsable, 50),
                                ApellidossupEmp = TruncateStr(campo.ApellidosResponsable, 50),
                                CodnegocioEmp = TruncateStr(campo.CodigoNegocio, 20),
                                CodsociedadEmp = TruncateStr(campo.CodigoSociedad, 20),
                                CodsubnegocioEmp = TruncateStr(campo.CodigoSubNegocio, 20),
                                DesccentrabajoEmp = TruncateStr(campo.DescripcionCentroTrabajo, 50),
                                Descnegocio = TruncateStr(campo.DescripcionNegocio, 50),
                                DescsociedadEmp = TruncateStr(campo.DescripcionSociedad, 50),
                                DescsubnegocioEmp = TruncateStr(campo.DescripcionSubNegocio, 50),
                                JornlaboralDomingo = campo.JornadaLaboralDomingo, //TODO
                                JornlaboralLunes = campo.JornadaLaboralLunes, //TODO
                                JornlaboralMartes = campo.JornadaLaboralMartes, //TODO
                                JornlaboralMiercoles = campo.JornadaLaboralMiercoles, //TODO
                                JornlaboralJueves = campo.JornadaLaboralJueves, //TODO
                                JornlaboralViernes = campo.JornadaLaboralViernes, //TODO
                                JornlaboralSabado = campo.JornadaLaboralSabado, //TODO
                                JornlaboralFestivo = campo.JornadaLaboralFestiva, //TODO
                                PorcenjornadaEmp = campo.PorcentajeReduccionJornada,
                                TipocontratoEmp = campo.TipoContrato,
                                CodcontratoEmp = campo.CodContratoEmp,
                                Ad = TruncateStr(campo.Ad, 50),
                                CojornadaEmp = campo.CoJornadaEmp,
                                EmailEmp = campo.EmailEmp
                            };
                            context.TblEmpleados.Add(empl);
                            creados++;
                        }
                    }
                    context.SaveChanges();
                    Log("Total de empleados en el fichero LOGA: " + _listaLoga.Count + "  Empleados encontrados: " + encontrados + "  Empleados creados: " + creados);


                    Log("Enviando empleados al WS de Evalos");
                    creados = 0;
                    encontrados = 0;
                    foreach (var empleado in context.TblEmpleados)
                    {
                        #region Consumir el WebService de Evalos para cada empleado que se crea
                        var wsEvalosMethod = config["EvalosAccess"] + "employee";
                        string userEvalos = config["EvalosUser"];
                        string passwordEvalos = config["EvalosPassword"];

                        var tracews = "Get Employee: URL: " + wsEvalosMethod + " DNI: " + empleado.DniEmp;
                        try
                        {
                            //string employee;
                            //employee = WebServiceRest.GetEmployee(wsEvalosMethod, userEvalos, passwordEvalos, empleado.DniEmp);
                            //Log("Se obtuvo el employee del WS: " + employee);
                            //if (employee == null || employee == string.Empty || employee == "null")
                            //{
                                string codigoKiosko = "000";
                                if (context.TblEmpleados.Any(e => e.DniSuperior == empleado.DniEmp))
                                {
                                    codigoKiosko = ObtenerCodigoSupervisor( empleado.DniEmp );
                                }
                            string emailEmp = string.IsNullOrEmpty(empleado.EmailEmp) || !IsValidEmail(empleado.EmailEmp) ? string.Empty : empleado.EmailEmp;
                                //Mando a crear el empleado en Evalos
                                //Creo el json con los datos q debo enviarle al ws
                            var employeeData = new Employee
                                {
                                    Code = empleado.DniEmp,
                                    Description = empleado.NombreEmp + " " + empleado.ApellidosEmp,
                                    CodeArea = empleado.UbicenEmp,
                                    CodeDepartment = empleado.CodnegocioEmp,
                                    CodeCompany = empleado.CodsociedadEmp,
                                    CodeSection = empleado.CodsubnegocioEmp,
                                    DateAdd = "20190901",
                                    CodeAccess = "999",
                                    CodeCorrection = "100",
                                    CodeSchedule = "1ES",
                                    CustomFields = new CustomField
                                    {
                                        EM_IDORACLE = empleado.IdoracleEmp,
                                        EM_NUMPERSO = empleado.NumeroEmp,
                                        EM_TIPOCONTRATO = empleado.TipocontratoEmp.ToString().PadLeft(3, '0'),
                                        EM_REDUCCION = Math.Truncate(empleado.PorcenjornadaEmp ?? 0).ToString().PadLeft(3, '0'),
                                        EM_NIEJERARQUIA = empleado.DniSuperior
                                    },
                                    Observations = string.Empty,
                                    CodeWorkflow = ((empleado.CodcontratoEmp == "TT") ||
                                                    (empleado.CodcontratoEmp == "CW") ||
                                                    (empleado.CodcontratoEmp == "FT") ||
                                                    (empleado.CodcontratoEmp == "FC") ? "200" : "100"),
                                    CodeKiosk = codigoKiosko,
                                    Email = emailEmp,
                                    CodePatternCalendar = ((empleado.CodcontratoEmp == "FW") ||
                                                           (empleado.CodcontratoEmp == "FT") ||
                                                           (empleado.CodcontratoEmp == "FC") ? "1FW" : "1ES"),
                                    PatternCalendarData = new PatternCalendar
                                    {
                                        StartDate = "20190101",
                                        EndDate = "20991231",
                                        Replace = true
                                    }
                                };
                                string employeejson = JsonConvert.SerializeObject(employeeData);
                                Log("Enviado al PUT de empleado " + employeejson);
                                tracews = "Put Employee: URL: " + wsEvalosMethod + " json: " + employeejson;
                                var httpWebResponse = WebServiceRest.PutPostRequest(wsEvalosMethod, userEvalos, passwordEvalos, employeejson, "PUT");

                                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                                {
                                    creados++;
                                }
                                else
                                {
                                    if (config["ShowDetailsLog"].ToUpper() == "TRUE")
                                    {
                                        Log("Respuesta erronea del PUT " + httpWebResponse.StatusCode + " => " + httpWebResponse.StatusDescription);
                                    }
                                }
                            //}
                            //else
                            //{
                            //  encontrados++;
                            //}
                        }
                        catch (Exception ex)
                        {
                            Log("Excepción en el consumo del WS de Evalos " + tracews + ": " + ex.ToString());
                        }
                        #endregion
                    }
                    //Log("Total de empleados en la BD SincroDE: " + context.TblEmpleados.Count().ToString() + "  Empleados encontrados en Evalos: " + encontrados + "  Empleados enviados: " + creados);
                    Log("Total de empleados en la BD SincroDE: " + context.TblEmpleados.Count().ToString() + "  Empleados enviados: " + creados);
                }
            }
            catch (Exception ex)
            {
                Log("Error salvando empleados en la BD Sincrode: " + ex.ToString());
            }
            #endregion

            #region Procesando marcajes

            Log("Comienzo del proceso de marcaje");
            //Llamando al métodos para procesar los marcajes
            string paramFecini = config["FechaInicialPrueba"];
            string paraFecFin = config["FechaFinalPrueba"];

            DateTime fechaini = new DateTime();
            DateTime fechafin = new DateTime();
            bool setfechas = true;
            //si no se logra obtener una fecha válida se asume q no estan bien puestos los parámetros
            if (string.IsNullOrEmpty(paramFecini) || string.IsNullOrEmpty(paraFecFin))
            {
                MarcajesDassnet.ProcesaMarcajes();
            }
            else
            {
                try
                {
                    fechaini = DateTime.ParseExact(paramFecini + " 00:00:00", "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    fechafin = DateTime.ParseExact(paraFecFin + " 23:59:59", "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    setfechas = false;
                }

                if (setfechas)
                {
                    Log("Se procesa desde " + fechaini + " hasta " + fechafin);
                    MarcajesDassnet.ProcesaMarcajesRango(fechaini, fechafin);
                }
                else
                {
                    Log("Fechas de parámetro incorrectas. Se procesa a partir de la última fecha procesada");
                    MarcajesDassnet.ProcesaMarcajes();
                }
            }

            #endregion

            #region Salvar ausencias en la base de datos 

            if (Intervalo.nextTimeToExecute == 1)
            {
                Log("Se comienzan a procesar las ausencias en la base de datos");

                //si no se logra obtener una fecha válida se asume q no estan bien puestos los parámetros
                if (string.IsNullOrEmpty(paramFecini) || string.IsNullOrEmpty(paraFecFin))
                {
                    ProcesaAusencias();
                }
                else
                {
                    try
                    {
                        fechaini = DateTime.ParseExact(paramFecini + " 00:00:00", "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        fechafin = DateTime.ParseExact(paraFecFin + " 23:59:59", "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        setfechas = false;
                    }

                    if (setfechas)
                    {
                        Log("Se procesa desde " + fechaini + " hasta " + fechafin);
                        ProcesaAusencias(fechaini, fechafin);
                    }
                    else
                    {
                        Log("Fechas de parámetro incorrectas. Se procesa a partir de la última fecha procesada");
                        ProcesaAusencias();
                    }
                }
            }
                

            #endregion
        }
    }
}
