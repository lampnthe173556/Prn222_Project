using EcormerProjectPRN222.Dao.AccountDAO;
using EcormerProjectPRN222.Models;
using EcormerProjectPRN222.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcormerProjectPRN222.Controllers
{
    public class ForgetPassController : Controller
    {
        private readonly EmailService _emailService;
        private static Random random = new Random();
        public ForgetPassController(EmailService emailService)
        {
            _emailService = emailService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> IndexAsync(String? email)
        {
            Account account = AccountDAO.GetAccountByEmail(email);
            if(account != null)
            {
                String message = GenerateOTP();
                //create session to store otp
                HttpContext.Session.SetString("otp", message);
                HttpContext.Session.SetString("email", email);
                String subject = "OTP to reset password";
                await _emailService.SendEmailAsync(email, subject, message);
                return RedirectToAction("Otp", "ForgetPass");
            }
            else
            {
                ViewBag.MessageError = "Email is not exist";
                return View();
            }           
        }
        public IActionResult Otp()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Otp(String otp)
        {
            String sessionOTP = HttpContext.Session.GetString("otp");
            if(otp.Equals(sessionOTP))
            {
                return RedirectToAction("ResetPass", "ForgetPass");
            }
            else
            {
                ViewBag.MessageError = "OTP is not correct";
                return View();
            }
        }
        public IActionResult ResetPass()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ResetPass(String newpassword, String againPass)
        {
            if(newpassword.Equals(againPass))
            {
                Account account = AccountDAO.GetAccountByEmail(HttpContext.Session.GetString("email"));
                account.Password = newpassword;
                AccountDAO.UpdateAccount(account);
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.MessageError = "Password is not match";
                return View();
            }
            
        }
        public static string GenerateOTP(int length = 5)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
