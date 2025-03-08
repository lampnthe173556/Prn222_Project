using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcormerProjectPRN222.Models;
using System.Linq;

namespace EcormerProjectPRN222.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly MyProjectClothingContext _context;
        public DashboardController(MyProjectClothingContext context) 
        {
            _context = context;
        }

        private string GetOrderStatus(int status)
        {
            return status switch
            {
                -1 => "Failed",
                0 => "Pending",
                1 => "Success",
                
            };
        }

        public async Task<IActionResult> Index()
        {
            // Get statistics for dashboard
            var totalProducts = await _context.Products.CountAsync();
            var totalOrders = await _context.Orders.CountAsync();
            var totalCustomers = await _context.Accounts.Where(a => a.RoleId != 1).CountAsync(); // Assuming RoleId 1 is admin
            var totalRevenue = await _context.Orders.Where(o => o.Status == 1).SumAsync(o => o.TotalAmount ?? 0); // Assuming Status 2 is completed

            // Get recent orders
            var recentOrders = await _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .Select(o => new
                {
                    Id = o.OrderId,
                    CustomerName = o.User.FullName,
                    StatusCode = o.Status,
                    Total = o.TotalAmount
                })
                .ToListAsync();

            var recentOrdersWithStatus = recentOrders.Select(o => new
            {
                o.Id,
                o.CustomerName,
                Status = GetOrderStatus(o.StatusCode),
                o.Total
            }).ToList();

            // Get top selling products
            var orderDetails = await _context.Set<OrderDetail>()
                .Include(od => od.Product)
                .Include(od => od.Order)
                .Where(od => od.Order.Status == 1) // Only completed orders
                .GroupBy(od => new { od.ProductId, od.Product.ProductName, od.Product.Img })
                .Select(g => new
                {
                    Id = g.Key.ProductId,
                    Name = g.Key.ProductName,
                    ImageUrl = g.Key.Img,
                    TotalSales = g.Sum(od => od.Quanity * (od.Order.TotalAmount ?? 0))
                })
                .OrderByDescending(x => x.TotalSales)
                .Take(5)
                .ToListAsync();

            // Get sales data for chart
            var salesData = await _context.Orders
                .Where(o => o.Status == 1)
                .GroupBy(o => o.OrderDate)
                .OrderBy(g => g.Key)
                .Take(7)
                .Select(g => new
                {
                    Date = g.Key.ToString(),
                    Total = g.Sum(o => o.TotalAmount ?? 0)
                })
                .ToListAsync();

            // Get category data for chart
            var categoryData = await _context.Categories
                .Select(c => new
                {
                    Name = c.CategoryName,
                    c.Products.Count
                })
                .ToListAsync();

            ViewBag.TotalProducts = totalProducts;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalCustomers = totalCustomers;
            ViewBag.Revenue = totalRevenue;
            ViewBag.RecentOrders = recentOrdersWithStatus;
            ViewBag.TopProducts = orderDetails;
            ViewBag.SalesData = new
            {
                labels = salesData.Select(x => x.Date),
                data = salesData.Select(x => x.Total)
            };
            ViewBag.CategoryData = new
            {
                labels = categoryData.Select(x => x.Name),
                data = categoryData.Select(x => x.Count)
            };

            return View();
        }
    }
}