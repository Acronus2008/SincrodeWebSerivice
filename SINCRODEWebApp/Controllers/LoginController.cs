using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SINCRODEWebApp.DAHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;

namespace SINCRODEWebApp.Controllers
{
    public class LoginController : Controller
    {
        private bool AllowAnonimous;

        public LoginController()
        {
            this.AllowAnonimous = GetConfiguration().GetValue<Boolean>("AllowAnonimous");
        }

        public IActionResult Index()
        {
            ViewBag.AllowAnonimous = this.AllowAnonimous;
            return View();
        }

        public IActionResult DoLogin(IFormCollection form)
        {
            var _username = form["Username"].ToString();
            var _password = form["Password"].ToString();

            if (this.AllowAnonimous)
            {
                if (_username.Equals("anonimous") && _password.Equals("anonimous"))
                {
                    CreateLoggedCookie("Anonimous");

                    return RedirectToAction("Index", "Process");
                }                
            }

            ADAuthentication aDAuthentication = ADHelper.ActiveDirectoryLogin(_username, _password);

            if (!aDAuthentication.IsAuthenticated)
            {
                ViewData["Message"] = aDAuthentication.Message;
               
                return View(nameof(Index));
            }

            CreateLoggedCookie(_username);

            return RedirectToAction("Index", "Process");
        }

        public IActionResult DoLoginAnomino()
        {
            CreateLoggedCookie("Anonimo");

            return RedirectToAction("Index", "Process");
        }

        public IActionResult Denied()
        {
            return View();
        }

        public IActionResult SignOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }

        private void CreateLoggedCookie(string _username)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,_username),
                new Claim(ClaimTypes.Role, "User")
            };

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60),
                IsPersistent = true
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        }

        private static IConfigurationRoot GetConfiguration()
        {
            return new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
        }
    }
}