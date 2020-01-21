﻿using System;
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

//using System.Threading;

namespace SINCRODEService
{
    class SincroDEService : ServiceBase
    {
        private const string LogFileLocation = @"C:\temp\servicelog.txt";
        private List<CamposLOGA> _listaLoga;

        Timer timer = new Timer();

        //private void Log(string logMessage)
        //{
        //    Directory.CreateDirectory(Path.GetDirectoryName(LogFileLocation));
        //    File.AppendAllText(LogFileLocation, DateTime.Now.ToString() + " : " + logMessage + Environment.NewLine);
        //}

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
            DownloadFile(servidorftp, usuario, password, carpetaDestino);

            for (int i = 1; i <= NumberOfRetries; ++i)
            {
                try
                {
                    _listaLoga = ProcessWorkbook(carpetaDestino + "\\" + fileName);
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
            #endregion

            #region Salvar empleado en la base de datos si no existe
            try
            {
                bool employeeFound;
                int encontrados = 0;
                int creados = 0;
                using (var context = new SINCRODEDBContext())
                {
                    int maxidEmp = context.TblEmpleados.Any() ? context.TblEmpleados.Max(e => e.IdEmp) : 0;

                    Log("Salvando empleados en la BD SincroDE");
                    foreach (CamposLOGA campo in _listaLoga)
                    {
                        employeeFound = context.TblEmpleados.Where(s => s.DniEmp == campo.NifDni).Any();

                        //Si no encuentra el empleado lo inserta
                        if (employeeFound)
                        {
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
                                CojornadaEmp = campo.CoJornadaEmp
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
                            string employee;
                            employee = WebServiceRest.GetEmployee(wsEvalosMethod, userEvalos, passwordEvalos, empleado.DniEmp);
                            //Log("Se obtuvo el employee del WS: " + employee);
                            if (employee == null || employee == string.Empty || employee == "null")
                            {
                                string codigoKiosko = "000";
                                if (context.TblEmpleados.Any(e => e.DniSuperior == empleado.DniEmp))
                                {
                                    codigoKiosko = ObtenerCodigoSupervisor( empleado.DniEmp );
                                }
                                //Mando a crear el empleado en Evalos
                                //Creo el json con los datos q debo enviarle al ws
                                var employeeData = new Employee
                                {
                                    Code = empleado.DniEmp,
                                    Description = empleado.NombreEmp + " " + empleado.ApellidosEmp,
                                    CodeArea = empleado.UbicenEmp,
                                    CodeDepartment = empleado.CoddepEmp,
                                    CodeCompany = empleado.CodnegocioEmp,
                                    CodeSection = empleado.CodsubnegocioEmp,
                                    CodeSchedule = empleado.CojornadaEmp.ToString(),
                                    CustomFields = new CustomField
                                    {
                                        EM_IDORACLE = empleado.IdoracleEmp,
                                        EM_NUMPERSO = empleado.NumeroEmp,
                                        EM_TIPOCONTRATO = empleado.TipocontratoEmp.ToString().PadLeft(3, '0'),
                                        EM_REDUCCION = Math.Truncate(empleado.PorcenjornadaEmp ?? 0).ToString().PadLeft(3, '0')
                                    },
                                    Observations = empleado.DniSuperior,
                                    CodeWorkflow = ((empleado.CodcontratoEmp == "TT") ||
                                                    (empleado.CodcontratoEmp == "CW") ||
                                                    (empleado.CodcontratoEmp == "FT") ||
                                                    (empleado.CodcontratoEmp == "FC") ? "200" : "100"),
                                    CodeKiosk = codigoKiosko,
                                    CodePatternCalendar = "1ES",
                                    /*
                                    ((empleado.CodcontratoEmp == "FW") ||
                                    (empleado.CodcontratoEmp == "FT") ||
                                    (empleado.CodcontratoEmp == "FC") ? "1FW" : "1ES")*/
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
                                        Log("Respuesta erronea del POST " + httpWebResponse.StatusCode + " => " + httpWebResponse.StatusDescription);
                                    }
                                }
                            }
                            else
                            {
                                encontrados++;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log("Excepción en el consumo del WS de Evalos " + tracews + ": " + ex.ToString());
                        }
                        #endregion
                    }
                    Log("Total de empleados en la BD SincroDE: " + context.TblEmpleados.Count().ToString() + "  Empleados encontrados en Evalos: " + encontrados + "  Empleados enviados: " + creados);
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
            if ((paramFecini == null || paramFecini == "") || (paraFecFin == null || paraFecFin == ""))
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
        }
    }
}
