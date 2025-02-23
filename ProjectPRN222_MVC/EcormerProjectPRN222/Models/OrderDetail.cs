using System;
using System.Collections.Generic;

namespace EcormerProjectPRN222.Models;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public int OrderId { get; set; }

    public int VariantId { get; set; }

    public int Quantity { get; set; }

    public double Price { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Variant Variant { get; set; } = null!;
}
