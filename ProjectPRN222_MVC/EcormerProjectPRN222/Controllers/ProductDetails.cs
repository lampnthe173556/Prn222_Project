using EcormerProjectPRN222.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EcormerProjectPRN222.Controllers
{
    public class ProductDetails : Controller
    {

		private readonly MyProjectClothingContext _context;

		public ProductDetails(MyProjectClothingContext context)
		{
			_context = context;
		}
		public IActionResult Index(int productId)
        {

			//take to account
			var user = HttpContext.Session.GetString("user");
			if (user != null)
			{
				Account account = JsonSerializer.Deserialize<Account>(user);
				ViewBag.User = account;
			}

			var context = new MyProjectClothingContext();
            Product product = context.Products.FirstOrDefault(x => x.ProductId == productId);
            ViewBag.product = product;

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
