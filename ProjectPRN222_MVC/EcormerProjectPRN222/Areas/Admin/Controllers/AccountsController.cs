using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcormerProjectPRN222.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Cryptography;
using System.Text;

namespace EcormerProjectPRN222.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountsController : Controller
    {
        private readonly MyProjectClothingContext _context;

        public AccountsController(MyProjectClothingContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Accounts";
            ViewData["Roles"] = new SelectList(_context.Roles, "RoleId", "RoleName");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAccounts()
        {
            var accounts = await _context.Accounts
                .Include(a => a.Role)
                .Select(a => new {
                    a.UserId,
                    a.Username,
                    a.FullName,
                    a.Email,
                    a.Phone,
                    a.Location,
                    a.RoleId,
                    RoleName = a.Role.RoleName,
                    a.Status
                })
                .ToListAsync();

            return Json(new { data = accounts });
        }

        [HttpGet]
        public async Task<IActionResult> GetAccount(int id)
        {
            var account = await _context.Accounts
                .Select(a => new {
                    a.UserId,
                    a.Username,
                    a.FullName,
                    a.Email,
                    a.Phone,
                    a.Location,
                    a.RoleId,
                    a.Status
                })
                .FirstOrDefaultAsync(a => a.UserId == id);

            if (account == null)
            {
                return Json(new { success = false, message = "Account not found" });
            }

            return Json(new { success = true, data = account });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username,FullName,Email,Phone,Location,RoleId,Password,Status")] Account account)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if username already exists
                    if (await _context.Accounts.AnyAsync(a => a.Username == account.Username))
                    {
                        return Json(new { success = false, message = "Username already exists" });
                    }

                    // Hash password
                    //account.Password = HashPassword(account.Password);
                    
                    _context.Add(account);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Account created successfully" });
                }
                catch (Exception)
                {
                    return Json(new { success = false, message = "Error creating account" });
                }
            }
            return Json(new { success = false, message = "Invalid data" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] Account account, string? newPassword)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingAccount = await _context.Accounts.FindAsync(account.UserId);
                    if (existingAccount == null)
                    {
                        return Json(new { success = false, message = "Account not found" });
                    }

                    // Check username uniqueness if changed
                    if (existingAccount.Username != account.Username)
                    {
                        if (await _context.Accounts.AnyAsync(a => a.Username == account.Username))
                        {
                            return Json(new { success = false, message = "Username already exists" });
                        }
                    }

                    // Update password if provided
                    if (!string.IsNullOrEmpty(newPassword))
                    {
                        existingAccount.Password = HashPassword(newPassword);
                    }

                    existingAccount.Username = account.Username;
                    existingAccount.FullName = account.FullName;
                    existingAccount.Email = account.Email;
                    existingAccount.Phone = account.Phone;
                    existingAccount.Location = account.Location;
                    existingAccount.RoleId = account.RoleId;
                    existingAccount.Status = account.Status;

                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Account updated successfully" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Json(new { success = false, message = "Update failed" });
                }
            }
            return Json(new { success = false, message = "Invalid data" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return Json(new { success = false, message = "Account not found" });
            }

            try
            {
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Account deleted successfully" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error deleting account" });
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
