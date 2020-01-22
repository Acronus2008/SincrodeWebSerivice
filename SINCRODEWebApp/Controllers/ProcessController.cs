using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace SINCRODEWebApp.Controllers
{
    public class ProcessController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Marcajes = GetConfiguration().GetValue<Boolean>("Menu:Marcaje");

            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        public IActionResult ShowLogs(int id)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Login");
            }

            ViewData["ProccessId"] = id;
            return View();
        }

        private static IConfigurationRoot GetConfiguration()
        {
            return new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
        }
    }
}