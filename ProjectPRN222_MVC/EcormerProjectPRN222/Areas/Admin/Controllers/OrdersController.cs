using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcormerProjectPRN222.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace EcormerProjectPRN222.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrdersController : AdminBaseController
    {
        private readonly MyProjectClothingContext _context;

        public OrdersController(MyProjectClothingContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Orders";
            ViewData["Payments"] = new SelectList(_context.Payments, "PayId", "PaymentName");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Pay)
                .Select(o => new {
                    o.OrderId,
                    o.OrderDate,
                    CustomerName = o.User != null ? o.User.FullName : "Unknown",
                    o.LocationOrder,
                    o.Status,
                    PaymentMethod = o.Pay != null ? o.Pay.PaymentName : "Unknown",
                    TotalAmount = o.TotalAmount ?? 0,
                    Comment = o.Comment ?? "",
                    payId = o.PayId
                })
                .OrderBy(o => o.OrderDate)
                .ToListAsync();

            return Json(new { data = orders });
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderDetails(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Pay)
                .Select(o => new {
                    o.OrderId,
                    o.OrderDate,
                    Customer = o.User != null ? new {
                        FullName = o.User.FullName ?? "Unknown",
                        Email = o.User.Email ?? "Unknown",
                        Phone = o.User.Phone ?? "Unknown"
                    } : new {
                        FullName = "Unknown",
                        Email = "Unknown",
                        Phone = "Unknown"
                    },
                    LocationOrder = o.LocationOrder ?? "Unknown",
                    o.Status,
                    Payment = o.Pay != null ? new {
                        PaymentName = o.Pay.PaymentName ?? "Unknown",
                        PaymentDes = o.Pay.PaymentDes ?? "Unknown"
                    } : new {
                        PaymentName = "Unknown",
                        PaymentDes = "Unknown"
                    },
                    TotalAmount = o.TotalAmount ?? 0,
                    Comment = o.Comment ?? ""
                })
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return Json(new { success = false, message = "Order not found" });
            }

            var orderDetails = await _context.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.OderId == id)
                .Select(od => new {
                    ProductName = od.Product != null ? od.Product.ProductName ?? "Unknown" : "Unknown",
                    Price = od.Product != null ? od.Product.Price : 0,
                    od.Quanity,
                    SubTotal = od.Quanity * (od.Product != null ? od.Product.Price : 0)
                })
                .ToListAsync();

            return Json(new { 
                success = true, 
                data = new { 
                    order,
                    items = orderDetails
                } 
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, int status)
        {
            try
            {
                // First check if order exists
                var exists = await _context.Orders.AnyAsync(o => o.OrderId == id);
                if (!exists)
                {
                    return Json(new { success = false, message = "Order not found" });
                }

                // Execute parameterized SQL update with full schema name
                var sql = @"
                    UPDATE dbo.[Order]
                    SET [status] = @p0
                    WHERE [OrderID] = @p1";

                var parameters = new[]
                {
                    new SqlParameter("@p0", System.Data.SqlDbType.Int) { Value = status },
                    new SqlParameter("@p1", System.Data.SqlDbType.Int) { Value = id }
                };

                var rowsAffected = await _context.Database.ExecuteSqlRawAsync(sql, parameters);

                if (rowsAffected > 0)
                {
                    return Json(new { success = true, message = "Order status updated successfully" });
                }

                return Json(new { success = false, message = "Update failed" });
            }
            catch (Exception ex)
            {
                // Log the actual error for debugging
                Console.WriteLine($"Error updating order status: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }

                return Json(new { success = false, message = $"Error updating order status: {ex.Message}" });
            }
        }

    }
}
