using System;
using System.Collections.Generic;

namespace EcormerProjectPRN222.Models;

public partial class Variant
{
    public int VarriantId { get; set; }

    public int ProductId { get; set; }

    public string? Sku { get; set; }

    public double Price { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<VarriantStock> VarriantStocks { get; set; } = new List<VarriantStock>();
}
