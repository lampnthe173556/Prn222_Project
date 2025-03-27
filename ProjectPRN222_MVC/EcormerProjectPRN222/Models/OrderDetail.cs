using System;
using System.Collections.Generic;

namespace EcormerProjectPRN222.Models;

public partial class OrderDetail
{
    public int OderId { get; set; }

    public int ProductId { get; set; }

    public int Quanity { get; set; }

    public int Price { get; set; }

    public virtual Order Oder { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;


}
