using Microsoft.AspNetCore.Http;
using SINCRODEWeb.Models;
using SINCRODEWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SINCRODEWebApp.Services
{
    public interface IProcessService
    {
        List<ProcessModel> GetProcess(string searchCriteria);

        List<ProcessModel> ProcessCollection(List<ProcessModel> lstElements, IFormCollection requestFormData);

        List<LogsModel> ProcessCollection(List<LogsModel> lstElements, IFormCollection requestFormData);

    }
}
