using System.ComponentModel.DataAnnotations;

namespace EcormerProjectPRN222.Models
{
    public class CheckoutViewModel
    {
        [Required]
        public string CustomerName { get; set; }
        
        [Required]
        public string Phone { get; set; }

        [Required]
        public string Address { get; set; }
        [Required]
        public string Comment { get; set; }

        [Required]
        public int TypePayment { get; set; } // hoặc đổi thành string nếu cần

        public List<CartItem> CartItems { get; set; } = new List<CartItem>(); // Khởi tạo tránh lỗi null
    }
}
