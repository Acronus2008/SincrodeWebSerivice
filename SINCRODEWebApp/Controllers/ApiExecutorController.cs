using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SINCRODEWebApp.Controllers
{
    [Route("api/[controller]")]
    public class ApiExecutorController : Controller
    {
        /// <summary>
        /// Ejecuta el proceso ProcesaMarcajesRango
        /// Retorna un 102 Procesando en caso de lanzarce correctamente el proceso
        /// Retorna un 400 en caso de ocurrir un error en el proceso
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status102Processing)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post()
        {
            var requestFormData = Request.Form;
            var fechaInicio = Convert.ToDateTime(requestFormData["FechaInicio"].ToString());
            var fechaFin = Convert.ToDateTime(requestFormData["FechaFin"].ToString());

            if (fechaInicio.CompareTo(fechaFin) > 0)
            {
                return RedirectToAction("Index", "Process");
            }

            var isExecuted = ExecuteProccess(fechaInicio, fechaFin);
            ViewData["ExecutedProcess"] = isExecuted ?
                string.Format("Proceso ejecutado con fecha inicial: {0} fecha final: {1}", requestFormData["FechaInicio"].ToString(), requestFormData["FechaFin"].ToString())
                : "Ocurrio un error al ejecutar el proceso";
            
            return RedirectToAction("Index", "Process");
        }

        private static bool ExecuteProccess(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                SINCRODEService.MarcajesDassnet.ProcesaMarcajesRango(fechaInicio, fechaFin, false);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
