using System;
using System.Collections.Generic;

namespace EcormerProjectPRN222.Models;

public partial class Discount
{
    public int DiscountId { get; set; }

    public string Code { get; set; } = null!;

    public string DiscountType { get; set; } = null!;

    public double Value { get; set; }

    public double MinOrderValue { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int Status { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
