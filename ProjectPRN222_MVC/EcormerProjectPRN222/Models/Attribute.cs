using System;
using System.Collections.Generic;

namespace EcormerProjectPRN222.Models;

public partial class Attribute
{
    public int AttributeId { get; set; }

    public string AttributeName { get; set; } = null!;

    public int Status { get; set; }

    public virtual ICollection<AttributeValue> AttributeValues { get; set; } = new List<AttributeValue>();
}
