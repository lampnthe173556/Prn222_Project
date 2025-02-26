using System;
using System.Collections.Generic;

namespace EcormerProjectPRN222.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public DateOnly OrderDate { get; set; }

    public int UserId { get; set; }

    public string LocationOrder { get; set; } = null!;

    public int Status { get; set; }

    public string? Comment { get; set; }

    public int PayId { get; set; }

    public double? TotalAmount { get; set; }

    public virtual Payment Pay { get; set; } = null!;

    public virtual Account User { get; set; } = null!;
}
