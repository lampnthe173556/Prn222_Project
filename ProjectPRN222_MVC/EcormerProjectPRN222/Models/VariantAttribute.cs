using System;
using System.Collections.Generic;

namespace EcormerProjectPRN222.Models;

public partial class VariantAttribute
{
    public int VariantId { get; set; }

    public int AttributeId { get; set; }

    public int ValueId { get; set; }

    public virtual Attribute Attribute { get; set; } = null!;

    public virtual AttributeValue Value { get; set; } = null!;

    public virtual Variant Variant { get; set; } = null!;
}
