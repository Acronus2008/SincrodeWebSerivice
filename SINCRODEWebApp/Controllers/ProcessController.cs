using Microsoft.AspNetCore.Mvc;

namespace SINCRODEWebApp.Controllers
{
    public class ProcessController : Controller
    {
        public IActionResult Index()
        {  
            return View();
        }

        public IActionResult ShowLogs(int id)
        {
            ViewData["ProccessId"] = id;
            return View();
        }
    }
}