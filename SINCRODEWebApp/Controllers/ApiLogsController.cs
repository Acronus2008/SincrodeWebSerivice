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
    public class ApiLogsController : Controller
    {
        private readonly DataBaseService.DataBaseService dataBase = new DataBaseService.DataBaseService();
        // POST api/<controller>

        [HttpPost]
        public IActionResult Post(int id)
        {
            //Get form data from client side
            var requestFormData = Request.Form;
            List<LogsModel> lstItems = GetLogs(requestFormData["search[value]"], id);

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

        private List<LogsModel> GetLogs(string searchCriteria, int idProcess = 0)
        {
            var model = new List<LogsModel>();

            try
            {
                var logs = dataBase.QueryTblProcesoslog().Where(m => m.IdPro == idProcess).ToList();

                foreach (var log in logs)
                {
                    var employed = GetEmployedFromProcess(log.IdEmp);
                    model.Add(new LogsModel() { Employed = employed, FechaInicioPro = log.FechaIniPro, DescProlog = log.DescProlog, ExcProlog = log.ExcProlog });
                }

                if (!string.IsNullOrEmpty(searchCriteria))
                {
                    model = model.Where(m => m.Employed.Contains(searchCriteria.ToUpper())).ToList();
                }
            }
            catch (Exception)
            {

                return new List<LogsModel>();
            }

            return model;
        }

        private string GetEmployedFromProcess(int EmployeId)
        {
            var dataEmployed = dataBase.GetTblEmpleadosByEmpleadoId(EmployeId);
            return string.Format("{0} {1}", dataEmployed.NombreEmp, dataEmployed.ApellidosEmp);
        }

        private List<LogsModel> ProcessCollection(List<LogsModel> lstElements, IFormCollection requestFormData)
        {
            if (lstElements == null || lstElements.Count() == 0)
            {
                return new List<LogsModel>();
            }

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
                        var prop = GetProperty(columName);
                        if (sortDirection == "asc")
                        {
                            return lstElements.OrderBy(prop.GetValue).Skip(skip).Take(pageSize).ToList();
                        }
                        else
                            return lstElements.OrderByDescending(prop.GetValue).Skip(skip).Take(pageSize).ToList();
                    }
                    else
                        return lstElements;
                }
            }
            return null;
        }
        private PropertyInfo GetProperty(string name)
        {
            var properties = typeof(LogsModel).GetProperties();
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
