using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcormerProjectPRN222.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EcormerProjectPRN222.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrdersController : Controller
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
                    CustomerName = o.User.FullName,
                    o.LocationOrder,
                    o.Status,
                    PaymentMethod = o.Pay.PaymentName,
                    o.TotalAmount,
                    o.Comment,
                    payId = o.PayId
                })
                .OrderByDescending(o => o.OrderDate)
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
                    Customer = new {
                        o.User.FullName,
                        o.User.Email,
                        o.User.Phone
                    },
                    o.LocationOrder,
                    o.Status,
                    Payment = new {
                        o.Pay.PaymentName,
                        o.Pay.PaymentDes
                    },
                    o.TotalAmount,
                    o.Comment
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
                    od.Product.ProductName,
                    od.Product.Price,
                    od.Quanity,
                    SubTotal = od.Quanity * od.Product.Price
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
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return Json(new { success = false, message = "Order not found" });
            }

            try
            {
                order.Status = status;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Order status updated successfully" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error updating order status" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrder(int id, [FromForm] Order order)
        {
            if (id != order.OrderId)
            {
                return Json(new { success = false, message = "Invalid order ID" });
            }

            try
            {
                var existingOrder = await _context.Orders.FindAsync(id);
                if (existingOrder == null)
                {
                    return Json(new { success = false, message = "Order not found" });
                }

                existingOrder.LocationOrder = order.LocationOrder;
                existingOrder.Status = order.Status;
                existingOrder.Comment = order.Comment;
                existingOrder.PayId = order.PayId;

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Order updated successfully" });
            }
            catch (DbUpdateConcurrencyException)
            {
                return Json(new { success = false, message = "Error updating order" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return Json(new { success = false, message = "Order not found" });
            }

            try
            {
                // Delete related order details first
                var orderDetails = await _context.OrderDetails.Where(od => od.OderId == id).ToListAsync();
                _context.OrderDetails.RemoveRange(orderDetails);

                // Then delete the order
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Order deleted successfully" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error deleting order" });
            }
        }
    }
}
