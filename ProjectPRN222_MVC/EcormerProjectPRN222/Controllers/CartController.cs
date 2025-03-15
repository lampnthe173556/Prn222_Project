using EcormerProjectPRN222.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

public class CartController : Controller
{
    private readonly MyProjectClothingContext _context;

    public CartController(MyProjectClothingContext context)
    {
        _context = context;
    }

    // chỉ lấy UserId khi người dùng đăng nhập
    public int GetUserSession()
    {
        var userJson = HttpContext.Session.GetString("user");
        if (userJson != null)
        {
            var user = JsonSerializer.Deserialize<Account>(userJson);
            return user.UserId; // trả về UserId 
        }
        return 0; // 
    }

    public int GetCartCount()
    {
        int? userId = GetUserSession();
        if (userId == null) return 0; //  chưa đăng nhập thì giỏ hàng trống
        return _context.CartItems.Count(c => c.UserId == userId);
    }

    public IActionResult Index()
    {
        var userJson = HttpContext.Session.GetString("user");
        if (userJson == null)
        {
            return RedirectToAction("Index", "Login");
        }

        var user = JsonSerializer.Deserialize<Account>(userJson);
        ViewBag.User = user;

        int? userId = GetUserSession();
        if (userId == null) return RedirectToAction("Index", "Login"); // ko có ID thì về trang Login

        var cartItems = _context.CartItems.Where(c => c.UserId == userId).ToList();
        ViewBag.cartItems = cartItems;
        return View(cartItems);
    }

    [HttpPost]
    public IActionResult AddToCart(int productId, string productName, string img, decimal price, int quantity)
    {
        int userId = GetUserSession();
        if (userId == null) return RedirectToAction("Index", "Login"); 

        if (quantity <= 0) return BadRequest("Số lượng sản phẩm không hợp lệ");

        var cartItem = _context.CartItems.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

        if (cartItem == null)
        {
            _context.CartItems.Add(new CartItem
            {
                UserId = userId,
                ProductId = productId,
                ProductName = productName,
                Img = img,
                Price = price,
                Quantity = quantity
            });
        }
        else
        {
            cartItem.Quantity += quantity;
        }

        _context.SaveChanges();
        return Redirect(Request.Headers["Referer"].ToString());
    }

    [HttpPost]
    public IActionResult UpdateCart(int productId, int quantity)
    {
        int? userId = GetUserSession();
        if (userId == null) return RedirectToAction("Index", "Login");

        if (quantity < 0) return BadRequest("Số lượng không hợp lệ");

        var cartItem = _context.CartItems.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

        if (cartItem != null)
        {
            if (quantity == 0)
            {
                _context.CartItems.Remove(cartItem);
            }
            else
            {
                cartItem.Quantity = quantity;
            }
            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult RemoveFromCart(int productId)
    {
        int? userId = GetUserSession();
        if (userId == null) return RedirectToAction("Index", "Login"); 

        var cartItem = _context.CartItems.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

        if (cartItem != null)
        {
            _context.CartItems.Remove(cartItem);
            _context.SaveChanges();
        }
        return RedirectToAction("Index");
    }
    //Checkout
    public IActionResult Checkout()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Checkout(Order model)
    {
        if (ModelState.IsValid)
        {
            // Save the order details to the database
            _context.Orders.Add(model);
            _context.SaveChanges();

            // Redirect to a success page
            return RedirectToAction("PaymentSuccess");
        }

        return View(model);
    }

    public IActionResult PaymentSuccess()
    {
        return View();
    }
}
