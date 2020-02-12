using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using SINCRODEWeb.Models;
using SINCRODEWebApp.Models;

namespace SINCRODEWebApp.Services
{

    public class MarcajesService : CollectionProperty, IProcessService
    {
        private readonly DataBaseService.DataBaseService dataBase = new DataBaseService.DataBaseService();

        public void ExecuteProcess(DateTime fechaInicio, DateTime fechaFin)
        {
            SINCRODEService.MarcajesDassnet.ProcesaMarcajesRango(fechaInicio, fechaFin, false);
        }

        public List<ProcessModel> GetProcess(string searchCriteria)
        {
            var model = new List<ProcessModel>();

            try
            {
                var process = dataBase.QueryTblProcesosMarcajes();
                foreach (var proc in process)
                {
                    var logs = new List<LogsModel>();
                    model.Add(new ProcessModel() { IdProcess = proc.IdPro, FechaInicio = proc.FechaIniPro, FechaFin = proc.FechaFinPro, Registros = proc.RegistrosPro, Empleados = proc.EmpleadosPro, Errores = proc.ErroresPro, Auto = proc.AutoPro, Logs = logs });
                }

                if (!string.IsNullOrEmpty(searchCriteria))
                {
                    model = model.Where(m => m.FechaInicio.GetDateTimeFormats().Contains(searchCriteria)).ToList();
                }
            }
            catch (Exception)
            {
                return new List<ProcessModel>();
            }

            return model;
        }

        public List<ProcessModel> ProcessCollection(List<ProcessModel> lstElements, IFormCollection requestFormData)
        {
            if (lstElements == null || lstElements.Count() == 0)
            {
                return new List<ProcessModel>();
            }

            return BuildCollectionFromListOfElement(lstElements, requestFormData);
        }

        public List<LogsModel> ProcessCollection(List<LogsModel> lstElements, IFormCollection requestFormData)
        {

            if (lstElements == null || lstElements.Count() == 0)
            {
                return new List<LogsModel>();
            }

            return BuildCollectionFromListOfElement(lstElements, requestFormData);
        }
    }
}
