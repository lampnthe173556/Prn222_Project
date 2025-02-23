using System;
using System.Collections.Generic;

namespace EcormerProjectPRN222.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public string PaymentName { get; set; } = null!;

    public string? Description { get; set; }

    public int Status { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
