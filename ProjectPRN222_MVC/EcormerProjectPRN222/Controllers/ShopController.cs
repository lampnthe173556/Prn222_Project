using EcormerProjectPRN222.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EcormerProjectPRN222.Controllers
{
    public class ShopController : Controller
    {
        private readonly MyProjectClothingContext _context;

        public ShopController(MyProjectClothingContext context)
        {
            _context = context;
        }
        public IActionResult Index(int? idCate)
        {
            var categories = _context.Categories.ToList();
            if (idCate == null)
            {

                var indexCate = 1;
                var products = _context.Products.Where(p => p.CategoryId == indexCate).ToList();
                ViewBag.Products = products;
                ViewBag.IdCate = indexCate;
            }
            else
            {
                var indexCate = idCate;
                var products = _context.Products.Where(p => p.CategoryId == indexCate).ToList();
                ViewBag.Products = products;
                ViewBag.IdCate = indexCate;
            }
            //account
            var userJson = HttpContext.Session.GetString("user");
            if (userJson == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var user = JsonSerializer.Deserialize<Account>(userJson);
            ViewBag.User = user;
            // update số lượng sản phẩm trong giỏ hàng
            int? userId = GetUserSession();
            if (userId == null) return RedirectToAction("Index", "Login"); // ko có ID thì về trang Login

            var cartItems = _context.CartItems.Where(c => c.UserId == Convert.ToInt32(userId)).ToList();
            ViewBag.cartItems = cartItems;

            ViewBag.Categories = categories;
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
