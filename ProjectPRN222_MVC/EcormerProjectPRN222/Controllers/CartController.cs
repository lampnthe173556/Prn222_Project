using EcormerProjectPRN222.Controllers;
using EcormerProjectPRN222.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

public class CartController : UserBaseController
{
    private readonly MyProjectClothingContext _context;

    public CartController(MyProjectClothingContext context)
    {
        _context = context;
    }

    // chỉ lấy UserId khi người dùng đăng nhập
    public int? GetUserSession()
    {
        var userJson = HttpContext.Session.GetString("user");
        if (string.IsNullOrEmpty(userJson)) return null;

        var user = JsonSerializer.Deserialize<Account>(userJson);
        return user?.UserId; // Trả về UserId hoặc null nếu không có
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
        if (userId == null)
        {
            TempData["ErrorMessage"] = "Bạn chưa đăng nhập!";
            return RedirectToAction("Index", "Login");
        } // ko có ID thì về trang Login

        var cartItems = _context.CartItems.Where(c => c.UserId == userId).ToList();
        ViewBag.cartItems = cartItems;
        return View(cartItems);
    }

    [HttpPost]
    public IActionResult AddToCart(int productId, string productName, string img, decimal price, int quantity)
    {

        int? userId = GetUserSession();
        if (userId == null) return RedirectToAction("Index", "Login");

        if (quantity <= 0) return BadRequest("Số lượng sản phẩm không hợp lệ");

        var cartItem = _context.CartItems.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

        if (cartItem == null)
        {
            _context.CartItems.Add(new CartItem
            {
                UserId = (int)userId,
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

        TempData["SuccessMessage"] = "Thêm vào giỏ hàng thành công!";
        return Redirect(Request.Headers["Referer"].ToString());
    }
    [HttpPost]
    public IActionResult UpdateCart(int productId, int quantity)
    {
        int? userId = GetUserSession();
        if (userId == null)
        {
            return Json(new { success = false, message = "Bạn chưa đăng nhập!" });
        }

        if (quantity < 0)
        {
            return Json(new { success = false, message = "Số lượng không hợp lệ!" });
        }

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

            // Tính tổng tiền của sản phẩm và tổng giỏ hàng
            decimal itemTotal = cartItem.Quantity * cartItem.Price;
            decimal cartTotal = _context.CartItems.Where(c => c.UserId == userId).Sum(c => c.Quantity * c.Price);

            return Json(new
            {
                success = true,
                itemTotal = itemTotal,
                cartTotal = cartTotal
            });
        }

        return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ hàng!" });
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
        var userJson = HttpContext.Session.GetString("user");
        if (userJson == null)
        {
            return RedirectToAction("Index", "Login");
        }

        var user = JsonSerializer.Deserialize<Account>(userJson);
        ViewBag.User = user;

        int? userId = GetUserSession();
        if (userId == null) return RedirectToAction("Index", "Login");

        var cartItems = _context.CartItems.Where(c => c.UserId == userId).ToList();
        var model = new CheckoutViewModel
        {
            CartItems = cartItems
        };
        return View(model);
    }

    [HttpPost]
    public IActionResult Checkout(CheckoutViewModel model)
    {
        if (ModelState.IsValid)
        {
            var userJson = HttpContext.Session.GetString("user");
            if (userJson == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var user = JsonSerializer.Deserialize<Account>(userJson);
            int userId = user.UserId;

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateOnly.FromDateTime(DateTime.Now),
                LocationOrder = model.Address,
                FullName = model.CustomerName,
                PhoneNumber = model.Phone,
                Status = 1, // Assuming 1 is the status for a new order
                PayId = model.TypePayment,
                TotalAmount = (double?)model.CartItems.Sum(item => item.Price * item.Quantity)
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            foreach (var item in model.CartItems)
            {
                var orderDetail = new OrderDetail
                {
                    OderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quanity = item.Quantity,
                    Price = item.Price
                };
                _context.OrderDetails.Add(orderDetail);
            }

            _context.SaveChanges();

            // Clear the cart after checkout
            var cartItems = _context.CartItems.Where(c => c.UserId == userId).ToList();
            _context.CartItems.RemoveRange(cartItems);
            _context.SaveChanges();

            return RedirectToAction("PaymentSuccess");
        }

        return View(model);
    }

    public IActionResult PaymentSuccess()
    {
        return View();
    }
}
