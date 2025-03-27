using System;
using System.Collections.Generic;

namespace EcormerProjectPRN222.Models;

public partial class CartItem
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? Img { get; set; }

    public int Price { get; set; }

    public int Quantity { get; set; }

    public virtual Account? User { get; set; }
}
