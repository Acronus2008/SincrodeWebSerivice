using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SINCRODEService.Config;
using SINCRODEService.Models;
using static SINCRODEService.Program;

namespace SINCRODEService
{
    class MarcajesResponse
    {
        public string Message { set; get; }
    }

    public static class MarcajesDassnet
    {
        //Método llamado cuando el sincrode está en producción de forma automática
        public static void ProcesaMarcajes()
        {
            IConfiguration config = ConfigHelper.GetConfiguration();

            //Recorro todos empleados de TBL_EMPLEADOS para consultar su marcaje
            try
            {
                int maxidProceso;
                DateTime fechIniProceso = DateTime.Now;

                using (var context = new SINCRODEDBContext())
                {
                    var employees = context.TblEmpleados.ToList();

                    //Tomo los datos necesarios para iniciar el proceso y salvarlo al final de procesar todos los marcajes
                    DateTime lastdateprocess = context.TblProcesos.Any(p => p.TipoPro)
                        ? context.TblProcesos.Where(p => p.TipoPro).Max(p => p.FechaIniPro)
                        : new DateTime(2019, 10, 1);
                    maxidProceso = context.TblProcesos.Any() ? context.TblProcesos.Max(p => p.IdPro) : 0;

                    string fechaIni = string.Format("{0}{1}{2}{3}{4}{5}", lastdateprocess.Year, lastdateprocess.Month.ToString().PadLeft(2, '0'), lastdateprocess.Day.ToString().PadLeft(2, '0'),
                                                                          lastdateprocess.Hour.ToString().PadLeft(2, '0'), lastdateprocess.Minute.ToString().PadLeft(2, '0'), lastdateprocess.Second.ToString().PadLeft(2, '0'));
                    string fechaFin = string.Format("{0}{1}{2}{3}{4}{5}", fechIniProceso.Year, fechIniProceso.Month.ToString().PadLeft(2, '0'), fechIniProceso.Day.ToString().PadLeft(2, '0'),
                                                                          fechIniProceso.Hour.ToString().PadLeft(2, '0'), fechIniProceso.Minute.ToString().PadLeft(2, '0'), fechIniProceso.Second.ToString().PadLeft(2, '0'));
                    long intfechaIni = Convert.ToInt64(fechaIni);
                    long intfechaFin = Convert.ToInt64(fechaFin);

                    //Inserto el proceso
                    var proceso = new TblProcesos()
                    {
                        IdPro = ++maxidProceso,
                        FechaIniPro = fechIniProceso,
                        FechaFinPro = DateTime.Now,
                        EmpleadosPro = employees.Count,
                        ErroresPro = 0,
                        AutoPro = true,
                        RegistrosPro = 0
                    };
                    context.TblProcesos.Add(proceso);

                    int maxidMarcaje = context.TblMarcajeprocesado.Any()
                        ? context.TblMarcajeprocesado.Max(p => p.IdMar)
                        : 0;
                    int marcajescount = 0;
                    Log("Se procesa a partir de la última fecha procesada: " + lastdateprocess);
                    foreach (var employee in employees)
                    {
                        using (var dassnetcontext = new DASSNETContext())
                        {
                            #region Obtener Marcaje de Dassnet

                            var marcajesEmpleado = from m in dassnetcontext.MensajesAcceso
                                                   join p in dassnetcontext.PersonasT on m.FkPersona equals p.Id
                                                   join l in dassnetcontext.Lectores on m.FkLector equals l.Id
                                                   join t in dassnetcontext.Tarjetas on m.FkTarjeta equals t.Id into tempJoin
                                                   from j in tempJoin.DefaultIfEmpty()
                                                   where p.Dni == employee.DniEmp &&
                                                         Convert.ToInt64(m.FechaYhora) >= intfechaIni &&
                                                         Convert.ToInt64(m.FechaYhora) <= intfechaFin
                                                   select new MarcajeEmpleado
                                                   {
                                                       Id = m.Id,
                                                       FechayHora = Utils.StrToDateTime(m.FechaYhora),
                                                       DNI = p.Dni,
                                                       CodTarjeta = j == null ? "" : j.CodTarjeta,
                                                       CodLector = l.Id,
                                                       Nombre = l.Nombre
                                                   };
                            #endregion

                            #region Salvar Marcajes en la tabla TBL_MARCAJEPROCESADO
                            foreach (var marcaje in marcajesEmpleado)
                            {
                                #region Obtener Marcaje de Dassnet

                                //TODO depende de la base de datos Dassnet
                                //Para cada empleado inserto el marcaje
                                var marcajeprocesado = new TblMarcajeprocesado()
                                {
                                    IdMar = ++maxidMarcaje,
                                    IdEmp = employee.IdEmp,
                                    IdPro = maxidProceso,
                                    FechaMarcajeMar = marcaje.FechayHora,
                                    DniEmp = employee.DniEmp,
                                    CodTarjetaMar = marcaje.CodTarjeta,
                                    IdLectorMar = marcaje.CodLector.ToString(),
                                    NombreLectorMar = marcaje.Nombre
                                };
                                context.TblMarcajeprocesado.Add(marcajeprocesado);
                                marcajescount++;

                                #endregion
                            }
                        }
                    }

                    Log("Salvando los datos de marcaje");
                    context.SaveChanges();
                    Log("Salvados " + marcajescount + " marcajes");

                    //Llamar al ws de Evalos para mandarle el arreglo con los marcajes
                    SendMarcajeToWS(context, maxidProceso, fechIniProceso);
                }
                #endregion
            }
            catch (Exception ex)
            {
                Log("#region Salvando en la tabla marcaje: " + ex.ToString());
            }
        }

        //Método llamado desde la aplicacion web
        public static void ProcesaMarcajesRango(DateTime fechaini, DateTime fechafin, bool AutoPro = true)
        {
            IConfiguration config = ConfigHelper.GetConfiguration();

            //Recorro todos empleados de TBL_EMPLEADOS para consultar su marcaje
            try
            {
                int maxidProceso;
                int intfechaIni = Convert.ToInt32(string.Format("{0}{1}{2}", fechaini.Year, fechaini.Month.ToString().PadLeft(2, '0'), fechaini.Day.ToString().PadLeft(2, '0')));
                int intfechaFin = Convert.ToInt32(string.Format("{0}{1}{2}", fechafin.Year, fechafin.Month.ToString().PadLeft(2, '0'), fechafin.Day.ToString().PadLeft(2, '0')));

                var context = new SINCRODEDBContext();
                {
                    var employees = context.TblEmpleados.ToList();

                    //Tomo los datos necesarios para iniciar el proceso y salvarlo al final de procesar todos los marcajes
                    DateTime lastdateprocess = context.TblProcesos.Any()
                        ? context.TblProcesos.Max(p => p.FechaIniPro)
                        : DateTime.Now.AddMonths(-3).Date;
                    maxidProceso = context.TblProcesos.Any() ? context.TblProcesos.Max(p => p.IdPro) + 1 : 1;
                    DateTime fechIniProceso = DateTime.Now;

                    //Inserto el proceso
                    var proceso = new TblProcesos()
                    {
                        IdPro = maxidProceso,
                        FechaIniPro = fechIniProceso,
                        FechaFinPro = DateTime.Now,
                        EmpleadosPro = employees.Count,
                        ErroresPro = 0,
                        AutoPro = AutoPro,
                        RegistrosPro = 0
                    };
                    var procesoTmp = context.TblProcesos.Add(proceso);

                    int maxidMarcaje = context.TblMarcajeprocesado.Any()
                        ? context.TblMarcajeprocesado.Max(p => p.IdMar)
                        : 0;
                    int marcajescount = 0;
                    foreach (var employee in employees)
                    {
                        using (var dassnetcontext = new DASSNETContext())
                        {
                            #region Obtener Marcaje de Dassnet

                            var marcajesEmpleado = from m in dassnetcontext.MensajesAcceso
                                                   join p in dassnetcontext.PersonasT on m.FkPersona equals p.Id
                                                   join l in dassnetcontext.Lectores on m.FkLector equals l.Id
                                                   join t in dassnetcontext.Tarjetas on m.FkTarjeta equals t.Id into tempJoin
                                                   from j in tempJoin.DefaultIfEmpty()
                                                   where p.Dni == employee.DniEmp &&
                                                         Convert.ToInt32(m.FechaYhora.Substring(0, 8)) >= intfechaIni &&
                                                         Convert.ToInt32(m.FechaYhora.Substring(0, 8)) <= intfechaFin
                                                   select new MarcajeEmpleado
                                                   {
                                                       Id = m.Id,
                                                       FechayHora = Utils.StrToDateTime(m.FechaYhora),
                                                       DNI = p.Dni,
                                                       CodTarjeta = j == null ? "" : j.CodTarjeta,
                                                       CodLector = l.Id,
                                                       Nombre = l.Nombre
                                                   };

                            #endregion

                            #region Salvar Marcajes en la tabla TBL_MARCAJEPROCESADO

                            foreach (var marcaje in marcajesEmpleado)
                            {
                                #region Obtener Marcaje de Dassnet

                                //TODO depende de la base de datos Dassnet
                                //Para cada empleado inserto el marcaje
                                var marcajeprocesado = new TblMarcajeprocesado()
                                {
                                    IdMar = ++maxidMarcaje,
                                    IdEmp = employee.IdEmp,
                                    IdPro = maxidProceso,
                                    FechaMarcajeMar = marcaje.FechayHora,
                                    DniEmp = employee.DniEmp,
                                    CodTarjetaMar = marcaje.CodTarjeta,
                                    IdLectorMar = marcaje.CodLector.ToString(),
                                    NombreLectorMar = marcaje.Nombre
                                };
                                context.TblMarcajeprocesado.Add(marcajeprocesado);
                                marcajescount++;

                                #endregion
                            }
                        }
                    }

                    procesoTmp.Entity.FechaFinPro = DateTime.Now;

                    Log("Salvando los datos de marcaje");
                    context.SaveChanges();
                    Log("Salvados " + marcajescount + " marcajes");

                    //Llamar al ws de Evalos para mandarle el arreglo con los marcajes
                    SendMarcajeToWS(context, maxidProceso, DateTime.Now);
                }
                #endregion
            }
            catch (Exception ex)
            {
                Log("#region Salvando en la tabla marcaje: " + ex.ToString());
            }
        }

        public static void SendMarcajeToWS(SINCRODEDBContext sincrodecontext, int idPro, DateTime fIniPro)
        {
            IConfiguration config = ConfigHelper.GetConfiguration();

            Log("Comenzando el envio de marcajes al WS");
            //Mando a crear el marcaje en Evalos
            //Creo el json con los datos q debo enviarle al ws
            string jsonattendance;
            string wsEvalosMethod = config["EvalosAccess"] + "booking/attendance/";
            string username = config["EvalosUser"];
            string password = config["EvalosPassword"];

            int cantTotalRegistros = 0;
            int cantRegistroPro = 0;
            int cantErrores = 0;
            int maxidProlog;

            List<Attendance> attendances = new List<Attendance>();
            List<int> attendanceIds = new List<int>();

            void EnviarMarcajesWS(int idEmpleado)
            {
                TblProcesoslog procesoLog;

                jsonattendance = JsonConvert.SerializeObject(attendances.ToArray());
                if (jsonattendance != "[]")
                {
                    if (config["ShowDetailsLog"].ToUpper() == "TRUE")
                    {
                        Log("Json usado en el envío al WS de marcaje " + jsonattendance);
                    }

                    try
                    {
                        var httpWebResponse = WebServiceRest.PutPostRequest(wsEvalosMethod, username, password, jsonattendance, "POST");

                        //Log("Respuesta del Post " + httpWebResponse.StatusCode + "" + httpWebResponse.StatusDescription);
                        string messageLog;
                        string messageError = string.Empty;

                        if (httpWebResponse.StatusCode == HttpStatusCode.Created)
                        {
                            StreamReader reader = new StreamReader(httpWebResponse.GetResponseStream());
                            string body = reader.ReadToEnd();
                            Log("Respuesta de POST de Marcajes, empleado " + attendances.Last<Attendance>().CodeEmployee + " => " + body);
                            var jsonBody = JsonConvert.DeserializeObject<List<MarcajesResponse>>(body);
                            for (int i = 0; i < attendances.Count; i++)
                            {
                                if (jsonBody[i].Message == "OK")
                                {
                                    cantRegistroPro++;
                                    //Si el marcaje se envió satisfactoriamente se borra de la tabla de marcajes procesados
                                    var marcajeToDelete = sincrodecontext.TblMarcajeprocesado.FirstOrDefault(m => m.IdMar == attendanceIds[i]);
                                    if (marcajeToDelete != null)
                                    {
                                        sincrodecontext.TblMarcajeprocesado.Remove(marcajeToDelete);
                                    }
                                    else
                                    {
                                        Log(string.Format("No se encontró en la tabla TBL_MARCAJEPROCESADO un registro con ID = {0}", attendanceIds[i]));
                                    }
                                }
                                else
                                {
                                    Log("Error en marcaje " + attendances[i].Date + " => " + jsonBody[i].Message);
                                    cantErrores++;
                                    messageError = body;
                                }
                            }

                            messageLog = httpWebResponse.StatusDescription;
                            //Log("Respuesta satisfactoria del POST " + messagelog);
                        }
                        else
                        {
                            cantErrores += attendances.Count;
                            messageLog = httpWebResponse.StatusDescription;
                            messageError = httpWebResponse.StatusDescription;
                            //Log("Respuesta incorrecta del POST " + messagelog);
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
                        Log("Error en el envio de POST de Marcajes: " + e.Message);
                    }
                }
                attendances.Clear();
                attendanceIds.Clear();
            }

            maxidProlog = sincrodecontext.TblProcesoslog.Any() ? sincrodecontext.TblProcesoslog.Max(l => l.IdProlog) : 0;

            //var empleadosXDistincta = sincrodecontext.TblMarcajeprocesado.Select(m => m.IdEmp).Distinct();
            var empleadosXDistinct = (from m in sincrodecontext.TblMarcajeprocesado select m.IdEmp).Distinct().ToList();

            //Log("Cantidad de marcajes con distintos empleados en la tabla "+empleadosXDistinct.Count());

            foreach (var empleado in empleadosXDistinct)
            {
                //var marcajesEmpleado = sincrodecontext.TblMarcajeprocesado.Where(m => m.IdEmp == empleado);
                var marcajesEmpleado = (from m1 in sincrodecontext.TblMarcajeprocesado select m1).Where(m => m.IdEmp == empleado).ToList();

                foreach (var marcajeempleado in marcajesEmpleado)
                {
                    var attendance = new Attendance()
                    {
                        CodeEmployee = marcajeempleado.DniEmp,
                        Date = marcajeempleado.FechaMarcajeMar.Date.ToString("yyyyMMdd", CultureInfo.InvariantCulture),
                        Time = marcajeempleado.FechaMarcajeMar.ToString("HHmmss", CultureInfo.InvariantCulture),
                        Installation = "LOC",
                        Clock = marcajeempleado.IdLectorMar.PadLeft(3, '0'),
                        Lector = "01",
                        Incidence = "00",
                        Card = marcajeempleado.CodTarjetaMar,
                        //"Ip": "10.0.0.1"
                    };
                    attendances.Add(attendance);
                    attendanceIds.Add(marcajeempleado.IdMar);
                    cantTotalRegistros += 1;

                    //Si ya hay 100 marcajes se realiza el envío y se sigue iterando en el foreach
                    if (attendances.Count == 100)
                    {
                        //Envío de los datos de marcaje al WS de Evalos
                        EnviarMarcajesWS(empleado);
                    }
                }

                //Envío de los datos de marcaje al WS de Evalos
                EnviarMarcajesWS(empleado);

                var proceso = sincrodecontext.TblProcesos.Where(p => p.IdPro == idPro).FirstOrDefault();
                proceso.RegistrosPro = cantRegistroPro;
                proceso.ErroresPro = cantErrores;
                sincrodecontext.SaveChanges();
            }
        }
    }
}
