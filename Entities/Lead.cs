using System;
using System.Collections.Generic;

namespace Sales_CRM.Entities;

public partial class Lead
{
    public int Id { get; set; }

    public string? Status { get; set; }

    public string? Source { get; set; }

    public string? Assigned { get; set; }

    public string Name { get; set; } = null!;

    public string? Position { get; set; }

    public string? Email { get; set; }

    public string? Website { get; set; }

    public string? Phone { get; set; }

    public decimal? Deposits { get; set; }

    public string? Comments { get; set; }

    public string? Description { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Country { get; set; }

    public string? Zipcode { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Tags { get; set; }

    public bool? WhatsappEnable { get; set; }

    public DateTime? LastContact { get; set; }
}
