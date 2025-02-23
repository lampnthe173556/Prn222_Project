using System;
using System.Collections.Generic;

namespace EcormerProjectPRN222.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public DateTime? DateBy { get; set; }

    public string? Note { get; set; }

    public double TotalAmount { get; set; }

    public int Status { get; set; }

    public int PaymentId { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Payment Payment { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();
}
