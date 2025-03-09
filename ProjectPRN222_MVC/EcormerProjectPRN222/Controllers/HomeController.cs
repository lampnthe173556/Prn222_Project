using EcormerProjectPRN222.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EcormerProjectPRN222.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyProjectClothingContext _context;

        public HomeController(MyProjectClothingContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            //New products
            List<Product> top4ProductsMen = _context.Products.Where(p => p.CategoryId == 1).OrderByDescending(p => p.ProductId).Take(4).ToList();
            List<Product> top4ProductsWoman = _context.Products.Where(p => p.CategoryId == 2).OrderByDescending(p => p.ProductId).Take(4).ToList();
            List<Product> top3ProductsChildren = _context.Products.Where(p => p.CategoryId == 3).OrderByDescending(p => p.ProductId).Take(4).ToList();
            List<Product> top10ProductsNew = top4ProductsMen.Concat(top4ProductsWoman).Concat(top3ProductsChildren).ToList();
            //best saler
            List<Product> top5ProductsBestSaler = _context.Products.OrderBy(p => p.Price).Take(10).ToList();

            //take to account
            var user = HttpContext.Session.GetString("user");
            if (user != null)
            {
                Account account = JsonSerializer.Deserialize<Account>(user);
                ViewBag.User = account;
            }

            ViewBag.Top10ProductsNew = top10ProductsNew;
            ViewBag.Top10ProductsBestSaler = top5ProductsBestSaler;

            // update số lượng sản phẩm trong giỏ hàng
            int? userId = GetUserSession();
            if (userId == null) return RedirectToAction("Index", "Login"); // ko có ID thì về trang Login

            var cartItems = _context.CartItems.Where(c => c.UserId == Convert.ToInt32(userId)).ToList();
            ViewBag.cartItems = cartItems;
            
            return View();
        }
        public int GetUserSession()
        {
            var userJson = HttpContext.Session.GetString("user");
            if (userJson != null)
            {
                var user = JsonSerializer.Deserialize<Account>(userJson);
                return user.UserId; // Trả về UserId
            }
            return 0; 
        }
    }
}
