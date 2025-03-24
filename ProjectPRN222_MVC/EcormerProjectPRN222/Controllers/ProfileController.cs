using EcormerProjectPRN222.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EcormerProjectPRN222.Controllers
{
    public class ProfileController : UserBaseController
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
                existingAccount.Username = account.Username;
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

        // POST: Profile/Delete
        [HttpPost, ActionName("Delete")]
        public IActionResult Delete()
        {
            try
            {
                var userJson = HttpContext.Session.GetString("user");
                if (userJson == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                var user = JsonSerializer.Deserialize<Account>(userJson);
                var account = _context.Accounts.Find(user.UserId);

                // Cập nhật giá trị mới vào tài khoản đang tracking

                account.Status = 0;
                _context.Accounts.Update(account);

                _context.SaveChanges();

                // Cập nhật session

                HttpContext.Session.Remove("user");
                return RedirectToAction("Index");
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

            return View();
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
                var userJson = HttpContext.Session.GetString("user");
                if (userJson == null)
                {
                    ViewBag.UpdateStatus = "Fail: User not logged in";
                    return View();
                }

                var user = JsonSerializer.Deserialize<Account>(userJson);
                var existingAccount = _context.Accounts.FirstOrDefault(a => a.UserId == user.UserId);

                if (existingAccount == null)
                {
                    ViewBag.UpdateStatus = "Fail: Account not found";

                    return View();
                }

                // Kiểm tra mật khẩu hiện tại có đúng không
                if (!existingAccount.Password.Equals(currentPassword))
                {
                    ViewBag.UpdateStatus = "Fail: Current password is incorrect";
                    var usera = JsonSerializer.Deserialize<Account>(userJson);
                    ViewBag.User = usera;
                    return View();
                }

                // Kiểm tra nếu mật khẩu mới giống mật khẩu cũ
                if (currentPassword.Equals(newPassword))
                {
                    ViewBag.UpdateStatus = "Fail: The new password is the same as the old password";
                    var usera = JsonSerializer.Deserialize<Account>(userJson);
                    ViewBag.User = usera;
                    return View();
                }

                // Kiểm tra nếu mật khẩu xác nhận không khớp
                if (!newPassword.Equals(confirmPassword))
                {
                    ViewBag.UpdateStatus = "Fail: The confirm password doesn't match the new password";
                    var usera = JsonSerializer.Deserialize<Account>(userJson);
                    ViewBag.User = usera;
                    return View();
                }

                // Cập nhật mật khẩu mới
                existingAccount.Password = newPassword;
                _context.Accounts.Update(existingAccount);
                _context.SaveChanges();

                // Cập nhật session
                userJson = JsonSerializer.Serialize(existingAccount);
                HttpContext.Session.SetString("user", userJson);
                ViewBag.UpdateStatus = "Success";

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
        public IActionResult HistoryOrder()
        {
            var userJson = HttpContext.Session.GetString("user");
            if (userJson == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var user = JsonSerializer.Deserialize<Account>(userJson);
            ViewBag.User = user;

            // Lấy danh sách đơn hàng của user
            var orders = _context.Orders
                .Include(o => o.Pay) // Load thông tin thanh toán
                .Where(o => o.UserId == user.UserId) // Lọc theo UserId
                .OrderByDescending(o => o.OrderDate) // Sort theo ngày đặt hàng mới nhất
                .ToList();

            return View(orders);
        }
        public IActionResult GetOrderDetails(int orderId)
        {
            var orderDetails = _context.OrderDetails
                .Include(od => od.Product) // Load thông tin sản phẩm
                .Where(od => od.OderId == orderId) // Lọc theo OrderId
                .Select(od => new
                {
                    ProductName = od.Product.ProductName,
                    Quantity = od.Quanity,
                    Price = od.Product.Price,
                    Total = od.Quanity * od.Product.Price
                }).ToList();

            return Json(orderDetails);
        }
    }
}
