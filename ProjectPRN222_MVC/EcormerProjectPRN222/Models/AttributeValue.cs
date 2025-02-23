using System;
using System.Collections.Generic;

namespace EcormerProjectPRN222.Models;

public partial class AttributeValue
{
    public int ValueId { get; set; }

    public int AttributeId { get; set; }

    public string ValueName { get; set; } = null!;

    public virtual Attribute Attribute { get; set; } = null!;
}
