using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcormerProjectPRN222.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EcormerProjectPRN222.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountsController : AdminBaseController
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
                .Where(a => a.RoleId !=1)
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
        public async Task<IActionResult> UpdateStatus(int id, int? status)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return Json(new { success = false, message = "Account not found" });
            }

            try
            {
                account.Status = status;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Account status updated successfully" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error updating account status" });
            }
        }
    }
}
