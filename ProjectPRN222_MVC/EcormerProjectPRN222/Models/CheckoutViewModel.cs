using System.ComponentModel.DataAnnotations;

namespace EcormerProjectPRN222.Models
{
    public class CheckoutViewModel
    {
        [Required]
        public string CustomerName { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public int TypePayment { get; set; }

        public List<CartItem> CartItems { get; set; }
    }
}
