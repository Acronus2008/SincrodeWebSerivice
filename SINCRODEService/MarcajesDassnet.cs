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

namespace SINCRODEService
{
    public static class MarcajesDassnet
    {
        private const string LogFileLocation = @"C:\temp\servicelog.txt";

        private static void Log(string logMessage)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(LogFileLocation));
            File.AppendAllText(LogFileLocation, DateTime.Now.ToString() + " : " + logMessage + Environment.NewLine);
        }


        //Método llamado cuando el sincrfode está en producción de forma automática
        public static void ProcesaMarcajes()
        {
            IConfiguration config = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
             .Build();

            //Recorro todos empleados de TBL_EMPLEADOS para consultar su marcaje
            try
            {
                int maxidProceso;
                DateTime fechIniProceso = DateTime.Now;

                using (var context = new SINCRODEDBContext())
                {
                    var employees = context.TblEmpleados.ToList();

                    //Tomo los datos necesarios para iniciar el proceso y salvarlo al final de procesar todos los marcajes
                    DateTime lastdateprocess = context.TblProcesos.Any()
                        ? context.TblProcesos.Max(p => p.FechaIniPro)
                        : DateTime.Now.AddMonths(-3).Date;
                    maxidProceso = context.TblProcesos.Any() ? context.TblProcesos.Max(p => p.IdPro) : 0;

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
                                                         Convert.ToInt32(m.FechaYhora.Substring(0, 4)) >= lastdateprocess.Year &&
                                                         Convert.ToInt32(m.FechaYhora.Substring(4, 2)) >= lastdateprocess.Month &&
                                                         Convert.ToInt32(m.FechaYhora.Substring(6, 2)) >= lastdateprocess.Day &&
                                                         Convert.ToInt32(m.FechaYhora.Substring(8, 2)) >= lastdateprocess.Hour &&
                                                         Convert.ToInt32(m.FechaYhora.Substring(10, 2)) >= lastdateprocess.Minute &&
                                                         Convert.ToInt32(m.FechaYhora.Substring(12, 2)) >= lastdateprocess.Second
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

        public static void ProcesaMarcajesRango(DateTime fechaini, DateTime fechafin, bool AutoPro = true)
        {
            IConfiguration config = ConfigHelper.GetConfiguration();

            //Recorro todos empleados de TBL_EMPLEADOS para consultar su marcaje
            try
            {
                int maxidProceso;
                int intfechaIni = Convert.ToInt32(string.Format("{0}{1}{2}", fechaini.Year, fechaini.Month, fechaini.Day));
                int intfechaFin = Convert.ToInt32(string.Format("{0}{1}{2}", fechafin.Year, fechafin.Month, fechafin.Day));
                using (var context = new SINCRODEDBContext())
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
            string wsEvalosMethod;

            int cantTotalRegistros = 0;
            int cantRegistroPro = 0;
            int cantErrores = 0;
            int maxidProlog;

            maxidProlog = sincrodecontext.TblProcesoslog.Any() ? sincrodecontext.TblProcesoslog.Max(l => l.IdProlog) : 0;
            TblProcesoslog procesoLog;

            //var empleadosXDistincta = sincrodecontext.TblMarcajeprocesado.Select(m => m.IdEmp).Distinct();
            var empleadosXDistinct = (from m in sincrodecontext.TblMarcajeprocesado select m.IdEmp).Distinct().ToList();

            //Log("Cantidad de marcajes con distintos empleados en la tabla "+empleadosXDistinct.Count());

            foreach (var empleado in empleadosXDistinct)
            {
                //var marcajesEmpleado = sincrodecontext.TblMarcajeprocesado.Where(m => m.IdEmp == empleado);
                var marcajesEmpleado = (from m1 in sincrodecontext.TblMarcajeprocesado select m1).Where(m => m.IdEmp == empleado).ToList();

                List<Attendance> attendances = new List<Attendance>();
                bool envioSuccefully;
                IQueryable<TblMarcajeprocesado> marcajesToDelete;
                foreach (var marcajeempleado in marcajesEmpleado)
                {
                    envioSuccefully = false;

                    var attendance = new Attendance()
                    {
                        CodeEmployee = marcajeempleado.DniEmp,
                        Date = marcajeempleado.FechaMarcajeMar.Date.ToString("yyyyMMdd",
                            CultureInfo.InvariantCulture),
                        Time =
                            marcajeempleado.FechaMarcajeMar.ToString("HHmmss", CultureInfo.InvariantCulture),
                        //Installation = "LOC",
                        //Clock = "",
                        Lector = marcajeempleado.IdLectorMar,
                        //"Incidence": "00",
                        Card = marcajeempleado.CodTarjetaMar,
                        //"Ip": "10.0.0.1"
                    };
                    attendances.Add(attendance);
                    cantTotalRegistros += 1;

                    //Si ya hay 100 marcajes se realiza el envío y se sigue iterando en el foreach
                    if (attendances.Count == 100)
                    {
                        //Envío de los datos de marcaje al WS de Evalos
                        jsonattendance = JsonConvert.SerializeObject(attendances.ToArray());
                        if (config["ShowDetailsLog"].ToUpper() == "TRUE")
                        {
                            Log("Json de 100 registros usado en el envío al WS de marcaje " + jsonattendance);
                        }
                        wsEvalosMethod = config["EvalosAccess"] + "booking/attendance/";
                        if (jsonattendance != "[]")
                        {
                            //jsonResponse = WebServiceRest.PostRequestJson(wsEvalosMethod, jsonattendance, "POST");
                            var httpWebResponse = WebServiceRest.PutPostRequest(wsEvalosMethod, jsonattendance, "POST");

                            //Log("Respuesta del Post " + httpWebResponse.StatusCode + "" + httpWebResponse.StatusDescription);
                            string messagelog;

                            if (httpWebResponse.StatusCode == HttpStatusCode.Created)
                            {
                                cantRegistroPro += attendances.Count;
                                envioSuccefully = true;
                                messagelog = httpWebResponse.StatusDescription;
                                //Log("Respuesta satisfactoria del POST " + messagelog);
                            }
                            else
                            {
                                cantErrores += attendances.Count;
                                envioSuccefully = false;
                                messagelog = httpWebResponse.StatusDescription;
                                //Log("Respuesta incorrecta del POST " + messagelog);
                            }

                            //Salvo el log de los procesos
                            procesoLog = new TblProcesoslog()
                            {
                                IdProlog = ++maxidProlog,
                                IdPro = idPro,
                                IdEmp = empleado,
                                FechaIniPro = fIniPro,
                                DescProlog = envioSuccefully
                                    ? messagelog
                                    : null,
                                ExcProlog = envioSuccefully
                                    ? null
                                    : messagelog
                            };
                            sincrodecontext.TblProcesoslog.Add(procesoLog);

                            //Aqui
                            //Si el envío fue satisfactorio borro todos los marcajes de ese proceso (IdPro) y ese empleado (IdEmp)
                            if (envioSuccefully)
                            {
                                marcajesToDelete = sincrodecontext.TblMarcajeprocesado.Where(m => m.IdPro == idPro && m.IdEmp == empleado);
                                //Log("Mensajes para borrar: " + marcajesToDelete.Count());
                                foreach (var marcajeprocesed in marcajesToDelete)
                                {
                                    sincrodecontext.TblMarcajeprocesado.Remove(marcajeprocesed);
                                }
                            }
                            sincrodecontext.SaveChanges();
                        }
                        attendances.Clear();
                    }
                }

                //Envío de los datos de marcaje al WS de Evalos
                jsonattendance = JsonConvert.SerializeObject(attendances.ToArray());
                wsEvalosMethod = config["EvalosAccess"] + "booking/attendance/";
                if (jsonattendance != "[]")
                {
                    if (config["ShowDetailsLog"].ToUpper() == "TRUE")
                    {
                        Log("Json usado en el envío al WS de marcaje " + jsonattendance);
                    }

                    //jsonResponse = WebServiceRest.PostRequestJson(wsEvalosMethod, jsonattendance, "POST");
                    var httpWebResponse = WebServiceRest.PutPostRequest(wsEvalosMethod, jsonattendance, "POST");
                    //Log("Respuesta del Post "+ httpWebResponse.StatusCode + " "+httpWebResponse.StatusDescription);

                    string messagelog;

                    if (httpWebResponse.StatusCode == HttpStatusCode.Created)
                    {
                        cantRegistroPro += attendances.Count;
                        envioSuccefully = true;
                        messagelog = httpWebResponse.StatusDescription;
                        //Log("Respuesta satisfactoria del POST " + messagelog);
                    }
                    else
                    {
                        cantErrores += attendances.Count;
                        envioSuccefully = false;
                        messagelog = httpWebResponse.StatusDescription;
                        //Log("Respuesta incorrecta del POST " + messagelog);
                    }

                    //Salvo el log de los procesos
                    procesoLog = new TblProcesoslog()
                    {
                        IdProlog = ++maxidProlog,
                        IdPro = idPro,
                        IdEmp = empleado,
                        FechaIniPro = fIniPro,
                        DescProlog = envioSuccefully
                            ? messagelog
                            : null,
                        ExcProlog = envioSuccefully
                            ? null
                            : messagelog
                    };
                    sincrodecontext.TblProcesoslog.Add(procesoLog);
                    sincrodecontext.SaveChanges();
                    //AQUI
                    var proceso = sincrodecontext.TblProcesos.Where(p => p.IdPro == idPro).FirstOrDefault();
                    proceso.RegistrosPro = cantRegistroPro;
                    proceso.ErroresPro = cantErrores;

                    //Si el envío fue satisfactorio borro todos los marcajes de ese proceso(IdPro) y ese empleado (IdEmp)
                    if (envioSuccefully)
                    {
                        marcajesToDelete = sincrodecontext.TblMarcajeprocesado.Where(m => m.IdPro == idPro && m.IdEmp == empleado);

                        foreach (var marcajeprocesed in marcajesToDelete)
                        {
                            sincrodecontext.TblMarcajeprocesado.Remove(marcajeprocesed);
                        }

                    }
                    sincrodecontext.SaveChanges();
                }
            }
        }
    }
}
