using System;
using System.IO;
using System.ServiceProcess;
using SINCRODEService.Models;
using System.Linq;
using System.Collections.Generic;
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

        private void Log(string logMessage)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(LogFileLocation));
            File.AppendAllText(LogFileLocation, DateTime.Now.ToString() + " : " + logMessage + Environment.NewLine);
        }

        protected override void OnStart(string[] args)
        {
            Log("*****************************************************************************************************************");
            Log("Iniciando");
            base.OnStart(args);

            try
            {
                // Set up a timer that triggers.
                timer.Interval = Intervalo.SetNextIntervalo();
                Log("Primera fecha de ejecución: " + DateTime.Now.AddMilliseconds(timer.Interval));
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
            Log("Próxima fecha de ejecución " + DateTime.Now.AddMilliseconds(proximoIntervalo));
            timer.Interval = proximoIntervalo;
            timer.Start();
        }

        private string TruncateStr(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
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
                //listaLOGA.ForEach(l => Log(l.Nombre));
                int employeeCount;
                int encontrados = 0;
                int creados = 0;
                using (var context = new SINCRODEDBContext())
                {
                    int maxidEmp = context.TblEmpleados.Any() ? context.TblEmpleados.Max(e => e.IdEmp) : 0;

                    foreach (CamposLOGA campo in _listaLoga)
                    {
                        employeeCount = context.TblEmpleados
                                                      .Where(s => s.DniEmp == campo.NifDni)
                                                      .Count();

                        //Si no encuentra el empleado lo inserta
                        if (employeeCount == 0)
                        {
                            //Insertar un nuevo empleado
                            //Log("maxidEmp " + maxidEmp);
                            if (creados == 0)//solo pongo el log en la primera vuelta
                            {
                                Log("Salvando empleados en la BD SincroDE");
                            }
                           
                            var empl = new TblEmpleados()
                            {
                                IdEmp = ++maxidEmp,
                                NombreEmp = TruncateStr(campo.Nombre, 100),
                                ApellidosEmp = TruncateStr(campo.Apellidos, 100),
                                DniEmp = TruncateStr(campo.NifDni, 20),
                                NumeroEmp = TruncateStr(campo.NoPersonal, 20),
                                IdoracleEmp = TruncateStr(campo.IdOracle, 20),
                                CodcenEmp = TruncateStr(campo.CodigoCentro, 50),
                                UbicenEmp = TruncateStr(campo.UbicacionCentroTrabajo, 20),
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

                            #region Consumir el WebService de Evalos para cada empleado que se crea
                            var wsEvalosMethod = config["EvalosAccess"] + "employee/";
                            string userEvalos = config["EvalosUser"];
                            string passwordEvalos = config["EvalosPassword"];

                            var tracews = "Get Employee: URL: " + wsEvalosMethod + " DNI: " + campo.NifDni;
                            try
                            {
                                string employee;
                                employee = WebServiceRest.GetEmployee(wsEvalosMethod, userEvalos, passwordEvalos, campo.NifDni);
                                //Log("Se obtuvo el employee del WS: " + employee);
                                if (employee is null)
                                {
                                    //Mando a crear el empleado en Evalos
                                    //Creo el json con los datos q debo enviarle al ws
                                    var employeeData = new Employee
                                    {
                                        Code = campo.NifDni,
                                        Description = campo.Nombre + " " + campo.Apellidos,
                                        CodeArea = campo.UbicacionCentroTrabajo.Substring(0,15),
                                        CodeDepartment = campo.CodigoDepartamento,
                                        CodeCompany = campo.CodigoNegocio,
                                        CodeSection = campo.CodigoSubNegocio,
                                        CodeSchedule = campo.CoJornadaEmp.ToString()
                                    };
                                    tracews = "Set Employee: URL: " + wsEvalosMethod + " DNI: " + campo.NifDni;
                                    string employeejson = JsonConvert.SerializeObject(employeeData);
                                    var httpWebResponse = WebServiceRest.PutPostRequest(wsEvalosMethod, userEvalos, passwordEvalos, employeejson, "PUT", campo.NifDni);
                                    
                                    //Log("Respuesta del Post " + httpWebResponse.StatusCode + "" + httpWebResponse.StatusDescription);

                                    if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                                    {
                                        ;//Log("Respuesta del POST " + httpWebResponse.StatusCode +" => "+ httpWebResponse.StatusDescription);
                                    }
                                    else
                                    {
                                        if (config["ShowDetailsLog"].ToUpper() == "TRUE")
                                        {
                                            Log("Respuesta erronea del POST " + httpWebResponse.StatusCode + " => " + httpWebResponse.StatusDescription);
                                        }
                                    }
                                    ////Log("Respuesta del put "+jsonResponse);
                                    //string putResponse = jsonResponse.Substring(1, 3);
                                    //string putMessage = jsonResponse.Substring(4);
                                    //if (putResponse == 200.ToString())
                                    //{
                                    //    ;//Log("Respuesta satisfactoria del PUT "+ putMessage);
                                    //}
                                    //else
                                    //{
                                    //    Log("Respuesta incorrecta del PUT " + putMessage);
                                    //}
                                }
                            }
                            catch (Exception ex)
                            {
                                Log("Excepción en el consumo del WS de Evalos " + tracews + ": " + ex.ToString());
                            }
                            #endregion
                        }
                        else
                        {
                            encontrados++;
                            //var employee = context.TblEmpleados
                            //                              .Where(s => s.DniEmp == campo.NifDni)
                            //                              .FirstOrDefault();
                        }
                    }
                    context.SaveChanges();
                    Log("Total de empleados en el fichero LOGA: " + _listaLoga.Count + "   Empleados encontrados: " + encontrados + "  Empleados creados: " + creados);
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
                    fechaini = DateTime.ParseExact(paramFecini +" 00:00:00", "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
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
