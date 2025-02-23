using System;
using System.Collections.Generic;

namespace EcormerProjectPRN222.Models;

public partial class Brand
{
    public int BrandId { get; set; }

    public string BrandName { get; set; } = null!;

    public string? Description { get; set; }

    public string? Img { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public int Status { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
