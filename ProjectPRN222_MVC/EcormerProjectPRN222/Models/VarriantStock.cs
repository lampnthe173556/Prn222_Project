using System;
using System.Collections.Generic;

namespace EcormerProjectPRN222.Models;

public partial class VarriantStock
{
    public int StockId { get; set; }

    public int VariantId { get; set; }

    public int Quantity { get; set; }

    public DateTime UpdateAt { get; set; }

    public virtual Variant Variant { get; set; } = null!;
}
