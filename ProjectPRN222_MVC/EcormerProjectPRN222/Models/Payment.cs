using System;
using System.Collections.Generic;

namespace EcormerProjectPRN222.Models;

public partial class Payment
{
    public int PayId { get; set; }

    public string PaymentName { get; set; } = null!;

    public string? PaymentDes { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
