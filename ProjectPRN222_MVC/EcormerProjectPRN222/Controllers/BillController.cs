using EcormerProjectPRN222.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EcormerProjectPRN222.Controllers
{
    public class BillController : Controller
    {
        private readonly MyProjectClothingContext _context;

        public BillController(MyProjectClothingContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userJson = HttpContext.Session.GetString("user");
            if (userJson == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var user = JsonSerializer.Deserialize<Account>(userJson);
            if (user == null)
            {
                return RedirectToAction("Index", "Login");
            }

            int userId = user.UserId;

            // Get the latest order for the logged-in user
            var order = _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .FirstOrDefault();

            if (order == null)
            {
                return View("NoOrders"); // Create a view to handle no orders scenario
            }

            var orderDetails = _context.OrderDetails
                .Where(od => od.OderId == order.OrderId)
                .ToList();

            var model = new BillViewModel
            {
                Order = order,
                OrderDetails = orderDetails
            };
            return View(model);

        }
    }
}
