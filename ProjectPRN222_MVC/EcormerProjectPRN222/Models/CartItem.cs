using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcormerProjectPRN222.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; } // khóa chính

        [Required]
        public int UserId { get; set; } //  giỏ hàng gắn với một người dùng

        [Required]
        public int ProductId { get; set; }

        [Required]
        public string ProductName { get; set; }

        public string Img { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int Price { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
