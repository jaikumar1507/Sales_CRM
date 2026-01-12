using System;
using System.Collections.Generic;

namespace Sales_CRM.Entities;

public partial class Timesheet
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? Task { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int? WorkedMinutes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
