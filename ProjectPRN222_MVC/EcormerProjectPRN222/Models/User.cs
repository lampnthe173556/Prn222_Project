using System;
using System.Collections.Generic;

namespace EcormerProjectPRN222.Models;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Address { get; set; }

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public int? Gender { get; set; }

    public string? Image { get; set; }

    public DateTime? Dob { get; set; }

    public int RoleId { get; set; }

    public int Status { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Role Role { get; set; } = null!;
}
