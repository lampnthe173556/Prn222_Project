using EcormerProjectPRN222.Models;
using EcormerProjectPRN222.Models.Vnpay;
using EcormerProjectPRN222.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EcormerProjectPRN222.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly MyProjectClothingContext _context;

        // Thanh toán VNPay
        public PaymentController(MyProjectClothingContext context, IVnPayService vnPayService)
        {

            _vnPayService = vnPayService;
            _context = context;
        }


        [HttpPost]
        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Redirect(url);
        }

        // Thanh Toán COD
        //[HttpPost]
        //public IActionResult Checkout(CheckoutViewModel model)
        //{
        //    var userJson = HttpContext.Session.GetString("user");
        //    if (userJson == null)
        //    {
        //        return RedirectToAction("Index", "Login");
        //    }


        //    if (ModelState.IsValid)
        //    {
        //        userJson = HttpContext.Session.GetString("user");
        //        if (userJson == null)
        //        {
        //            return RedirectToAction("Index", "Login");
        //        }

        //        var user = JsonSerializer.Deserialize<Account>(userJson);
        //        int userId = user.UserId;

        //        var order = new Order
        //        {
        //            UserId = userId,
        //            OrderDate = DateOnly.FromDateTime(DateTime.Now),
        //            LocationOrder = model.Address,
        //            FullName = model.CustomerName,
        //            PhoneNumber = model.Phone,
        //            Status = 1, // Assuming 1 is the status for a new order
        //            PayId = model.TypePayment,
        //            TotalAmount = (int?)model.CartItems.Sum(item => item.Price * item.Quantity)
        //        };

        //        _context.Orders.Add(order);
        //        _context.SaveChanges();

        //        foreach (var item in model.CartItems)
        //        {
        //            var orderDetail = new OrderDetail
        //            {
        //                OderId = order.OrderId,
        //                ProductId = item.ProductId,
        //                Quanity = item.Quantity,
        //                Price = item.Price
        //            };
        //            _context.OrderDetails.Add(orderDetail);
        //        }

        //        _context.SaveChanges();

        //        // Clear the cart after checkout
        //        var cartItems = _context.CartItems.Where(c => c.UserId == userId).ToList();
        //        _context.CartItems.RemoveRange(cartItems);
        //        _context.SaveChanges();

        //        if (model.TypePayment == 1) // COD
        //        {
        //            return RedirectToAction("Index", "Bill");
        //        }
        //        else if (model.TypePayment == 2) // VNPay
        //        {
        //            //var paymentUrl = _vnPayService.CreatePaymentUrl(new PaymentInformationModel
        //            //{
        //            //    Name = model.CustomerName,
        //            //    Amount = model.CartItems.Sum(item => item.Price * item.Quantity),
        //            //    OrderDescription = "Thanh toán qua VNPay",
        //            //    OrderType = "other"
        //            //}, HttpContext);

        //            //return Redirect(paymentUrl);
        //        }
        //    }

        //    return View(model);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult PaymentOrder(CheckoutViewModel model)
        //{
        //    // Kiểm tra dữ liệu từ form
        //    if (!ModelState.IsValid)
        //    {
        //        // Nếu có lỗi, hiển thị lại form với model hiện tại
        //        return View("~/Views/Cart/Checkout.cshtml", model);
        //    }

        //    // Lấy thông tin user từ session (đảm bảo user đã đăng nhập)
        //    var userJson = HttpContext.Session.GetString("user");
        //    if (string.IsNullOrEmpty(userJson))
        //    {
        //        return RedirectToAction("Index", "Login");
        //    }
        //    var user = JsonSerializer.Deserialize<Account>(userJson);
        //    int userId = user.UserId;

        //    // Lấy giỏ hàng của user từ cơ sở dữ liệu
        //    var cartItems = _context.CartItems.Where(c => c.UserId == userId).ToList();
        //    if (cartItems == null || !cartItems.Any())
        //    {
        //        ModelState.AddModelError("", "Giỏ hàng trống.");
        //        return View("~/Views/Cart/Checkout.cshtml", model);
        //    }
        //    // Gán lại giỏ hàng cho model (để đảm bảo khi render lại view dữ liệu đầy đủ)
        //    model.CartItems = cartItems;

        //    // Tính tổng tiền
        //    int totalAmount = cartItems.Sum(item => item.Price * item.Quantity);

        //    // Tạo đơn hàng mới
        //    var order = new Order
        //    {
        //        UserId = userId,
        //        OrderDate = DateOnly.FromDateTime(DateTime.Now),
        //        LocationOrder = model.Address,
        //        FullName = model.CustomerName,
        //        PhoneNumber = model.Phone,
        //        Status = 1, // 1: đơn hàng mới, tùy theo định nghĩa của bạn
        //        PayId = model.TypePayment,
        //        TotalAmount = totalAmount
        //    };

        //    _context.Orders.Add(order);
        //    _context.SaveChanges();

        //    // Tạo các chi tiết đơn hàng dựa trên giỏ hàng
        //    foreach (var item in cartItems)
        //    {
        //        var orderDetail = new OrderDetail
        //        {
        //            OderId = order.OrderId,  // Lưu ý: Kiểm tra tên thuộc tính ở OrderDetail (OderId hay OrderId)
        //            ProductId = item.ProductId,
        //            Quanity = item.Quantity,
        //            Price = item.Price
        //        };
        //        _context.OrderDetails.Add(orderDetail);
        //    }
        //    _context.SaveChanges();

        //    // Xóa giỏ hàng của người dùng sau khi đặt đơn hàng thành công
        //    _context.CartItems.RemoveRange(cartItems);
        //    _context.SaveChanges();

        //    // Xử lý chuyển hướng theo phương thức thanh toán
        //    if (model.TypePayment == 1)
        //    {
        //        // COD: chuyển đến trang PaymentSuccess
        //        return RedirectToAction("PaymentSuccess");
        //    }
        //    else if (model.TypePayment == 2)
        //    {
        //        // VNPay: Thực hiện tạo URL thanh toán VNPay (giả sử có service _vnPayService)
        //        // Ví dụ:
        //        // var paymentUrl = _vnPayService.CreatePaymentUrl(new PaymentInformationModel
        //        // {
        //        //     Name = model.CustomerName,
        //        //     Amount = totalAmount,
        //        //     OrderDescription = "Thanh toán qua VNPay",
        //        //     OrderType = "other"
        //        // }, HttpContext);
        //        // return Redirect(paymentUrl);

        //        // Nếu chưa triển khai VNPay, bạn có thể chuyển hướng tạm thời:
        //        return RedirectToAction("PaymentSuccess");
        //    }

        //    return RedirectToAction("Index", "Home");
        //}
    }
}
