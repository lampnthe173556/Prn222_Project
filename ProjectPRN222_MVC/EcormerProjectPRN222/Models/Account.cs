﻿using System;
using System.Collections.Generic;

namespace EcormerProjectPRN222.Models;

public partial class Account
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Location { get; set; } = null!;

    public int RoleId { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Role Role { get; set; } = null!;
}
