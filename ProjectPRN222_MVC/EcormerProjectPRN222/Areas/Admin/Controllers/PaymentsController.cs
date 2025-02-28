using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcormerProjectPRN222.Models;

namespace EcormerProjectPRN222.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PaymentsController : Controller
    {
        private readonly MyProjectClothingContext _context;

        public PaymentsController(MyProjectClothingContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Payments";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPayments()
        {
            var payments = await _context.Payments
                .Select(p => new {
                    p.PayId,
                    p.PaymentName,
                    p.PaymentDes,
                    p.Status,
                    OrderCount = p.Orders.Count
                })
                .ToListAsync();

            return Json(new { data = payments });
        }

        [HttpGet]
        public async Task<IActionResult> GetPayment(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return Json(new { success = false, message = "Payment not found" });
            }
            return Json(new { success = true, data = payment });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaymentName,PaymentDes,Status")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(payment);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Payment method created successfully" });
                }
                catch (Exception)
                {
                    return Json(new { success = false, message = "Error creating payment method" });
                }
            }
            return Json(new { success = false, message = "Invalid data" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("PayId,PaymentName,PaymentDes,Status")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payment);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Payment method updated successfully" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentExists(payment.PayId))
                    {
                        return Json(new { success = false, message = "Payment method not found" });
                    }
                    throw;
                }
            }
            return Json(new { success = false, message = "Invalid data" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return Json(new { success = false, message = "Payment method not found" });
            }

            try
            {
                // Check if payment method is being used in orders
                if (await _context.Orders.AnyAsync(o => o.PayId == id))
                {
                    return Json(new { success = false, message = "Cannot delete payment method that is being used in orders" });
                }

                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Payment method deleted successfully" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error deleting payment method" });
            }
        }

        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.PayId == id);
        }
    }
}
