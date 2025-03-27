using System;
using System.Collections.Generic;

namespace EcormerProjectPRN222.Models;

public partial class Message
{
    public int MessageId { get; set; }

    public int GroupId { get; set; }

    public int UserId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? SentAt { get; set; }

    public virtual ChatGroup Group { get; set; } = null!;

    public virtual Account User { get; set; } = null!;
}
