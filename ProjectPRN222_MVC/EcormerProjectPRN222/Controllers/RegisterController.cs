using EcormerProjectPRN222.Dao.AccountDAO;
using Microsoft.AspNetCore.Mvc;

namespace EcormerProjectPRN222.Controllers
{
    public class RegisterController : Controller
    {
        [HttpGet]
        public IActionResult RegisterIndex()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RegisterIndex(String userName, String password, String fullName, String email, String phone, String location)
        {
            if(AccountDAO.checkDuplicate(email))
            {
                ViewBag.MessageError = "Email already exists!";
                return View("RegisterIndex");
            }else if(AccountDAO.checkDuplicateUserName(userName))
            {
                ViewBag.MessageError = "Username already exists!";
                return View("RegisterIndex");
            }
            else
            {
                AccountDAO.RegisterAccount(userName, password, fullName, email, phone, location);
            }
            return RedirectToAction("Index", "Login");  
        }
    }
}
