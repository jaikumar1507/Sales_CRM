using System;
using System.Collections.Generic;

namespace Sales_CRM.Entities;

public partial class User
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? ProfileImage { get; set; }

    public decimal? HourlyRate { get; set; }

    public decimal? SalesTarget { get; set; }

    public string? Facebook { get; set; }

    public string? Linkedin { get; set; }

    public string? Skype { get; set; }

    public string? EmailSignature { get; set; }

    public string? Direction { get; set; }

    public string? TwilioPhone { get; set; }

    public bool? TwilioWhatsappEnabled { get; set; }

    public bool? IsStaff { get; set; }

    public bool? IsAdmin { get; set; }

    public string PasswordHash { get; set; } = null!;

    public DateTime? PasswordChangedAt { get; set; }

    public TimeSpan? TotalLoggedTime { get; set; }

    public TimeSpan? LastMonthLoggedTime { get; set; }

    public TimeSpan? ThisMonthLoggedTime { get; set; }

    public TimeSpan? LastWeekLoggedTime { get; set; }

    public TimeSpan? ThisWeekLoggedTime { get; set; }

    public DateTime? LastActiveAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Timesheet> Timesheets { get; set; } = new List<Timesheet>();

    public virtual ICollection<UserNote> UserNotes { get; set; } = new List<UserNote>();

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();
}
