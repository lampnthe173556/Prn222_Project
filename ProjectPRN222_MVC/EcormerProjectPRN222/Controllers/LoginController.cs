﻿using EcormerProjectPRN222.Dao.AccountDAO;
using EcormerProjectPRN222.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EcormerProjectPRN222.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            var rememberPass = Request.Cookies["rememberPass"];
            if (rememberPass != null)
            {
                ViewBag.RememberPass = rememberPass;
                var email = Request.Cookies["email"];
                var password = Request.Cookies["password"];
                ViewBag.Email = email;
                ViewBag.Password = password;
            }
            return View();
        }
        [HttpPost]
        public IActionResult Index(string email, string password, string remember_me, string returnUrl)
        {
            Account account = AccountDAO.GetAccount(email, password);
            if (remember_me != null)
            {
                Response.Cookies.Append("email", email, new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddDays(7),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax
                });
                Response.Cookies.Append("password", password, new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddDays(7),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax
                });
                Response.Cookies.Append("rememberPass", remember_me, new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddDays(7),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax
                });
            }
            else
            {
                Response.Cookies.Delete("email");
                Response.Cookies.Delete("password");
                Response.Cookies.Delete("rememberPass");
            }
            if (account != null)
            {
                var userJson = JsonSerializer.Serialize(account);
                HttpContext.Session.SetString("user", userJson);

                // If there's a returnUrl and it's a local URL, redirect there
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                // Otherwise redirect based on role
                if (account.RoleId == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }

            }
            else
            {
                ViewBag.Error = "Invalid username or password";
                return View();
            }
        }

        public IActionResult Logout()
        {
            // Clear session
            HttpContext.Session.Remove("user");
            
            // Clear authentication cookies
            Response.Cookies.Delete("email");
            Response.Cookies.Delete("password");
            Response.Cookies.Delete("rememberPass");
            
            // Clear all session data
            HttpContext.Session.Clear();
            
            return RedirectToAction("Index");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
