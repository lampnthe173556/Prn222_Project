using EcormerProjectPRN222.Dao.AccountDAO;
using EcormerProjectPRN222.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EcormerProjectPRN222.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(string email, string password)
        {
            Account account = AccountDAO.GetAccount(email, password);

            if (account != null)
            {
                var userJson = JsonSerializer.Serialize(account);
                HttpContext.Session.SetString("user", userJson);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Error = "Invalid username or password";
                return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("user");
            return RedirectToAction("Index");
        }
    }
}
