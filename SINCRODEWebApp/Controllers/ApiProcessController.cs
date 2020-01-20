using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SINCRODEService.Models;
using SINCRODEWeb.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SINCRODEWebApp.Controllers
{
    [Route("api/[controller]")]
    public class ApiProcessController : Controller
    {
        private readonly DataBaseService.DataBaseService dataBase = new DataBaseService.DataBaseService();
        // POST api/<controller>

        [HttpPost]
        public IActionResult Post()
        {
            //Get form data from client side
            var requestFormData = Request.Form;
            List<Models.ProcessModel> lstItems = GetProcess(requestFormData["search[value]"]);

            var listItems = ProcessCollection(lstItems, requestFormData);

            // Custom response to bind information in client side
            dynamic response = new
            {
                Data = listItems,
                Draw = requestFormData["draw"],
                RecordsFiltered = lstItems.Count,
                RecordsTotal = lstItems.Count
            };
            return Ok(response);
        }

        private List<Models.ProcessModel> GetProcess(string searchCriteria)
        {
            var model = new List<Models.ProcessModel>();

            var process = dataBase.QueryTblProcesos();
            foreach (var proc in process)
            {
                var logs = new List<LogsModel>();
                model.Add(new Models.ProcessModel() { IdProcess = proc.IdPro, FechaInicio = proc.FechaIniPro, FechaFin = proc.FechaFinPro, Registros = proc.RegistrosPro, Empleados = proc.EmpleadosPro, Errores = proc.ErroresPro, Auto = proc.AutoPro ?? false, Logs = logs });
            }

            if (!string.IsNullOrEmpty(searchCriteria))
            {
                model = model.Where(m => m.FechaInicio.GetDateTimeFormats().Contains(searchCriteria)).ToList();
            }

            return model;
        }

        private List<LogsModel> GetLogs(int idProcess = 0)
        {
            var model = new List<LogsModel>();

            var logs = dataBase.QueryTblProcesoslog().Where(m => m.IdPro == idProcess).ToList();
            foreach (var log in logs)
            {
                var employed = GetEmployedFromProcess(log.IdEmp);
                model.Add(new LogsModel() { Employed = employed, FechaInicioPro = log.FechaIniPro, DescProlog = log.DescProlog, ExcProlog = log.ExcProlog });
            }

            return model;
        }

        private string GetEmployedFromProcess(int EmployeId)
        {
            var dataEmployed = dataBase.GetTblEmpleadosByEmpleadoId(EmployeId);
            return string.Format("{0} {1}", dataEmployed.NombreEmp, dataEmployed.ApellidosEmp);
        }

        private List<Models.ProcessModel> ProcessCollection(List<Models.ProcessModel> lstElements, IFormCollection requestFormData)
        {
            var skip = Convert.ToInt32(requestFormData["start"].ToString());
            var pageSize = Convert.ToInt32(requestFormData["length"].ToString());
            Microsoft.Extensions.Primitives.StringValues tempOrder = new[] { "" };

            if (requestFormData.TryGetValue("order[0][column]", out tempOrder))
            {
                var columnIndex = requestFormData["order[0][column]"].ToString();
                var sortDirection = requestFormData["order[0][dir]"].ToString();
                tempOrder = new[] { "" };
                if (requestFormData.TryGetValue($"columns[{columnIndex}][data]", out tempOrder))
                {
                    var columName = requestFormData[$"columns[{columnIndex}][data]"].ToString();

                    if (pageSize > 0)
                    {
                        var prop = getProperty(columName);
                        if (sortDirection == "asc")
                        {
                            return lstElements.OrderBy(prop.GetValue).Skip(skip).Take(pageSize).ToList();
                        }

                        return lstElements.OrderByDescending(prop.GetValue).Skip(skip).Take(pageSize).ToList();
                    }

                    return lstElements;
                }
            }
            return null;
        }

        private List<LogsModel> ProcessCollection(List<LogsModel> lstElements, IFormCollection requestFormData)
        {
            var skip = Convert.ToInt32(requestFormData["start"].ToString());
            var pageSize = Convert.ToInt32(requestFormData["length"].ToString());
            Microsoft.Extensions.Primitives.StringValues tempOrder = new[] { "" };

            if (requestFormData.TryGetValue("order[0][column]", out tempOrder))
            {
                var columnIndex = requestFormData["order[0][column]"].ToString();
                var sortDirection = requestFormData["order[0][dir]"].ToString();
                tempOrder = new[] { "" };
                if (requestFormData.TryGetValue($"columns[{columnIndex}][data]", out tempOrder))
                {
                    var columName = requestFormData[$"columns[{columnIndex}][data]"].ToString();

                    if (pageSize > 0)
                    {
                        var prop = getProperty(columName);
                        if (sortDirection == "asc")
                        {
                            return lstElements.OrderBy(prop.GetValue).Skip(skip).Take(pageSize).ToList();
                        }

                        return lstElements.OrderByDescending(prop.GetValue).Skip(skip).Take(pageSize).ToList();
                    }

                    return lstElements;
                }
            }
            return null;
        }
        private PropertyInfo getProperty(string name)
        {
            var properties = typeof(Models.ProcessModel).GetProperties();
            PropertyInfo prop = null;
            foreach (var item in properties)
            {
                if (item.Name.ToLower().Equals(name.ToLower()))
                {
                    prop = item;
                    break;
                }
            }
            return prop;
        }
    }
}
