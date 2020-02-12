using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SINCRODEWebApp.Models;
using SINCRODEWebApp.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SINCRODEWebApp.Controllers
{
    [Route("api/[controller]")]
    public class ApiAbsentismosController : Controller
    {
        private readonly DataBaseService.DataBaseService dataBase = new DataBaseService.DataBaseService();

        private readonly AbsentismosService absentismosService = new AbsentismosService();

        [HttpPost]
        public IActionResult Post()
        {
            //Get form data from client side
            var requestFormData = Request.Form;
            List<ProcessModel> processes = absentismosService.GetProcess(requestFormData["search[value]"]);
            List<ProcessModel> processCollection = absentismosService.ProcessCollection(processes, requestFormData);

            // Custom response to bind information in client side
            dynamic response = new
            {
                Data = processCollection,
                Draw = requestFormData["draw"],
                RecordsFiltered = processes.Count,
                RecordsTotal = processes.Count
            };
            return Ok(response);
        }

    }
}
