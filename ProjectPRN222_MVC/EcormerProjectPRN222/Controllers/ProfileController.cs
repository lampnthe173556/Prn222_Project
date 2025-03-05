using EcormerProjectPRN222.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EcormerProjectPRN222.Controllers
{
    public class ProfileController : Controller
    {
        private readonly MyProjectClothingContext _context;

        public ProfileController(MyProjectClothingContext context)
        {
            _context = context;
        }

        // GET: Profile
        public IActionResult Index()
        {
            var userJson = HttpContext.Session.GetString("user");
            if (userJson == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var user = JsonSerializer.Deserialize<Account>(userJson);
            ViewBag.User = user;
            var account = _context.Accounts.Find(user.UserId); // Lấy từ database
            if (account == null)
            {
                HttpContext.Session.Remove("user");
                return RedirectToAction("Index", "Login");
            }

            return View(account);
        }

        // GET: Profile/Edit
        public IActionResult Edit()
        {
            var userJson = HttpContext.Session.GetString("user");
            if (userJson == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var user = JsonSerializer.Deserialize<Account>(userJson);
            ViewBag.User = user;
            var account = _context.Accounts.Find(user.UserId);
            if (account == null)
            {
                HttpContext.Session.Remove("user");
                return RedirectToAction("Index", "Login");
            }

            return View(account);
        }

        // POST: Profile/Edit
        [HttpPost]
        public IActionResult Edit(Account account)
        {

            try
            {
                var existingAccount = _context.Accounts.FirstOrDefault(a => a.UserId == account.UserId);
                if (existingAccount == null)
                {
                    ViewBag.UpdateStatus = "Fail: Account not found";
                    return View(account);
                }

                // Cập nhật giá trị mới vào tài khoản đang tracking

                existingAccount.FullName = account.FullName;
                existingAccount.Email = account.Email;
                existingAccount.Phone = account.Phone;
                existingAccount.Location = account.Location;
                _context.Accounts.Update(existingAccount);

                _context.SaveChanges();

                // Cập nhật session
                var userJson = JsonSerializer.Serialize(existingAccount);
                HttpContext.Session.SetString("user", userJson);
                var user = JsonSerializer.Deserialize<Account>(userJson);
                ViewBag.User = user;

                ViewBag.UpdateStatus = "Success";
            }
            catch (DbUpdateException ex)
            {
                // Log the error (uncomment ex variable name and write a log.)
                ViewBag.UpdateStatus = "Fail: " + ex.InnerException?.Message;
            }
            catch (Exception ex)
            {
                ViewBag.UpdateStatus = "Fail: " + ex.Message;
            }

            return View(account);
        }

        // GET: Profile/Delete
        public IActionResult Delete()
        {
            var userJson = HttpContext.Session.GetString("user");
            if (userJson == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var user = JsonSerializer.Deserialize<Account>(userJson);
            ViewBag.User = user;

            var account = _context.Accounts.Find(user.UserId);
            if (account == null)
            {
                HttpContext.Session.Remove("user");
                return RedirectToAction("Index", "Login");
            }

            return View(account);
        }

        // POST: Profile/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed()
        {
            var userJson = HttpContext.Session.GetString("user");
            if (userJson != null)
            {
                var user = JsonSerializer.Deserialize<Account>(userJson);
                ViewBag.User = user;

                var account = _context.Accounts.Find(user.UserId);

                if (account != null)
                {
                    _context.Accounts.Remove(account);
                    _context.SaveChanges();
                    HttpContext.Session.Remove("user");
                }
            }

            return RedirectToAction("Index", "Home");
        }
        //GET: change pass
        public IActionResult ChangePassword()
        {
            var userJson = HttpContext.Session.GetString("user");
            if (userJson == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var user = JsonSerializer.Deserialize<Account>(userJson);
            ViewBag.User = user;

            var account = _context.Accounts.Find(user.UserId);
            if (account == null)
            {
                HttpContext.Session.Remove("user");
                return RedirectToAction("Index", "Login");
            }
            var userJsonUpdated = JsonSerializer.Serialize(account);
            HttpContext.Session.SetString("user", userJsonUpdated);

            return View(account);
        }

        // POST: Profile/Edit
        [HttpPost]
        public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {

            try
            {
                if (currentPassword.Equals(newPassword) )
                {
                    //ghi user vao session
                    var userJson = HttpContext.Session.GetString("user");
                    var user = JsonSerializer.Deserialize<Account>(userJson);
                    ViewBag.User = user;

                    var account = _context.Accounts.Find(user.UserId);
                    ViewBag.UpdateStatus = "The new password same as old password";
                    var userJsonUpdated = JsonSerializer.Serialize(account);
                    HttpContext.Session.SetString("user", userJsonUpdated);

                }
                if(newPassword.Equals(confirmPassword)) 
                {
                    var userJson = HttpContext.Session.GetString("user");
                    var user = JsonSerializer.Deserialize<Account>(userJson);
                    ViewBag.User = user;

                    var account = _context.Accounts.Find(user.UserId);
                    ViewBag.UpdateStatus = "The confirm password isn't same as new password";
                    var userJsonUpdated = JsonSerializer.Serialize(account);
                    HttpContext.Session.SetString("user", userJsonUpdated);
                }
                else
                {
                    var userJson = HttpContext.Session.GetString("user");
                    var user = JsonSerializer.Deserialize<Account>(userJson);


                    var existingAccount = _context.Accounts.FirstOrDefault(a => a.UserId == user.UserId);
                    if (existingAccount == null)
                    {
                        ViewBag.UpdateStatus = "Fail: Account not found";
                        return View(user);
                    }

                    // Cập nhật giá trị mới vào tài khoản đang tracking

                    existingAccount.Password = newPassword;

                    _context.Accounts.Update(existingAccount);

                    _context.SaveChanges();

                    // Cập nhật session
                    userJson = JsonSerializer.Serialize(existingAccount);
                    HttpContext.Session.SetString("user", userJson);
                    user = JsonSerializer.Deserialize<Account>(userJson);

                    ViewBag.User = user;

                }

            }
            catch (DbUpdateException ex)
            {
                ViewBag.UpdateStatus = "Fail: " + ex.InnerException?.Message;
            }
            catch (Exception ex)
            {
                ViewBag.UpdateStatus = "Fail: " + ex.Message;
            }

            return View();
        }
    }
}
