using System;
using System.Collections.Generic;

namespace EcormerProjectPRN222.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public double? Price { get; set; }

    public string? Img { get; set; }

    public string? Description { get; set; }

    public int CategoryId { get; set; }

    public int? Status { get; set; }

    public virtual Category Category { get; set; } = null!;
}
