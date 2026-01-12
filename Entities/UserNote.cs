using System;
using System.Collections.Generic;

namespace Sales_CRM.Entities;

public partial class UserNote
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Note { get; set; } = null!;

    public string? AddedFrom { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
