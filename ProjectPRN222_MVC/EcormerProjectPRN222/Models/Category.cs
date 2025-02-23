using System;
using System.Collections.Generic;

namespace EcormerProjectPRN222.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }

    public string? Img { get; set; }

    public int Status { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
