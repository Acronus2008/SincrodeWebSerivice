using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SINCRODEWebApp.DAHelper;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace SINCRODEWebApp.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult DoLogin(IFormCollection form)
        {
            var _username = form["Username"].ToString();
            var _password = form["Password"].ToString();

            bool IsUserLoggedWithActiveDirectory = ADHelper.ActiveDirectoryLogin(_username, _password);

            if (!IsUserLoggedWithActiveDirectory)
            {
                return View(nameof(Index));
            }

            CreateLoggedCookie(_username);

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
    }
}