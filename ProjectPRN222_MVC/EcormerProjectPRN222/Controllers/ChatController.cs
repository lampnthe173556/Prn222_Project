﻿using EcormerProjectPRN222.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using EcormerProjectPRN222.Hubs;
using System.Text.Json;

namespace EcormerProjectPRN222.Controllers
{
    public class ChatController : Controller
    {
        private readonly MyProjectClothingContext _context;
        private readonly IHubContext<SignalRServer> _signalRHub;

        public ChatController(MyProjectClothingContext context, IHubContext<SignalRServer> signalRHub)
        {
            _context = context;
            _signalRHub = signalRHub;
        }

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

        public async Task<IActionResult> Index()
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
            }

            // Lấy giỏ hàng (tối ưu hóa bằng AsNoTracking)
            var cartItems = await _context.CartItems
                                          .AsNoTracking()
                                          .Where(c => c.UserId == userId)
                                          .ToListAsync();
            ViewBag.cartItems = cartItems;

            // Lấy danh sách tin nhắn (tối ưu hóa bằng AsNoTracking)
            List<Message> messages = await _context.Messages
                                           .AsNoTracking()
                                           .Include(m => m.User)
                                           .Where(m => m.User.Username == user.Username ||
                                                       m.Group.Users.Any(u => u.Username == user.Username))
                                           .OrderBy(m => m.SentAt)
                                           .ToListAsync();

            // Gửi tín hiệu cập nhật chat qua SignalR (chờ xử lý để tránh lỗi)
            await _signalRHub.Clients.All.SendAsync("LoadAllUser");

            return View(messages);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string message)
        {
            // Lấy user từ Session
            var userJson = HttpContext.Session.GetString("user");
            if (string.IsNullOrEmpty(userJson))
            {
                TempData["ErrorMessage"] = "Bạn chưa đăng nhập!";
                return RedirectToAction("Index", "Login");
            }

            var user = JsonSerializer.Deserialize<Account>(userJson);

            // Kiểm tra tài khoản hợp lệ
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản!";
                return RedirectToAction("Index");
            }

            // Kiểm tra tin nhắn có rỗng không
            if (string.IsNullOrWhiteSpace(message))
            {
                TempData["ErrorMessage"] = "Tin nhắn không được để trống!";
                return RedirectToAction("Index");
            }

            // Tạo tin nhắn mới
            var newMessage = new Message
            {
                UserId = user.UserId,
                Content = message,
                SentAt = DateTime.Now,
                GroupId = 1 // Giả sử nhóm mặc định
            };

            _context.Messages.Add(newMessage);
            _context.SaveChanges();

            // Gửi tin nhắn đến tất cả clients bằng SignalR
            try
            {
                await _signalRHub.Clients.All.SendAsync("LoadAll", user.Username, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi SignalR: {ex.Message}");
            }
            return RedirectToAction("Index");
        }
    }
}