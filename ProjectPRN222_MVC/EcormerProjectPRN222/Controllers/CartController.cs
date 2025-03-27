using EcormerProjectPRN222.Models;
using EcormerProjectPRN222.Models.Vnpay;
using EcormerProjectPRN222.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;


public class CartController : Controller
{
    private readonly MyProjectClothingContext _context;
    private readonly IVnPayService _vnPayService;

    public CartController(MyProjectClothingContext context, IVnPayService vnPayService)
    {
        _context = context;
        _vnPayService = vnPayService;
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
    public IActionResult AddToCart(int productId, string productName, string img, int price, int quantity)
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
            decimal itemTotal = (decimal)(cartItem.Quantity * cartItem.Price);
            decimal cartTotal = (decimal)_context.CartItems.Where(c => c.UserId == userId).Sum(c => c.Quantity * c.Price);

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


        var cartItems = _context.CartItems
                .Where(c => c.UserId == userId)
                .Select(c => new CartItem
                {
                    UserId = (int)userId,
                    ProductName = c.ProductName,
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    Price = c.Price,
                })
                .ToList();
        ViewBag.cartItems = cartItems;

        var model = new CheckoutViewModel
        {
            CustomerName = user.FullName,
            Phone = user.Phone,
            Address = user.Location,
            CartItems = cartItems
        };
        return View(model);
    }

    [HttpPost]
    public IActionResult Checkout(CheckoutViewModel model)
    {
        var userJson = HttpContext.Session.GetString("user");
        if (userJson == null)
        {
            return RedirectToAction("Index", "Login");
        }

        var user = JsonSerializer.Deserialize<Account>(userJson);
        ViewBag.User = user;

        int userId = user.UserId;

        var order = new Order
        {
            UserId = userId,
            OrderDate = DateOnly.FromDateTime(DateTime.Now),
            LocationOrder = model.Address,
            FullName = model.CustomerName,
            PhoneNumber = model.Phone,
            Comment = model.Comment,
            Status = 1, // Assuming 1 is the status for a new order
            PayId = model.TypePayment,
            TotalAmount = (int?)model.CartItems.Sum(item => item.Price * item.Quantity)
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
                Price = (int)item.Price
            };
            _context.OrderDetails.Add(orderDetail);
        }

        _context.SaveChanges();

        // Clear the cart after checkout
        var cartItems = _context.CartItems.Where(c => c.UserId == userId).ToList();
        _context.CartItems.RemoveRange(cartItems);
        _context.SaveChanges();

        if (model.TypePayment == 1) // COD
        {
            return RedirectToAction("Index", "Bill");
        }
        else if (model.TypePayment == 2) // VNPay
        {
            //var paymentUrl = _vnPayService.CreatePaymentUrl(new PaymentInformationModel
            //{
            //    Name = model.CustomerName,
            //    Amount = model.CartItems.Sum(item => item.Price * item.Quantity),
            //    OrderDescription = "Thanh toán qua VNPay",
            //    OrderType = "other"
            //}, HttpContext);

            //return Redirect(paymentUrl);
        }


        return View(model);
    }

    [HttpGet]
    public IActionResult PaymentCallbackVnpay()
    {
        var response = _vnPayService.PaymentExecute(Request.Query);

        return Json(response);
    }

}

