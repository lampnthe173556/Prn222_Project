﻿using System;
using System.Collections.Generic;

namespace EcormerProjectPRN222.Models;

public partial class Cart
{
    public int CartId { get; set; }

    public int UserId { get; set; }

    public int VariantId { get; set; }

    public int Quantity { get; set; }

    public DateTime? AddedAd { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual Variant Variant { get; set; } = null!;
}
