using System;
using System.Collections.Generic;

namespace EcormerProjectPRN222.Models;

public partial class ChatGroup
{
    public int GroupId { get; set; }

    public string GroupName { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Account> Users { get; set; } = new List<Account>();
}
