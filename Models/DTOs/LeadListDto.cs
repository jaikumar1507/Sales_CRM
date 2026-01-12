namespace Sales_CRM.Models.DTOs
{
    public class LeadListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public decimal? Deposits { get; set; }
        public string? Comments { get; set; }
        public string? Assigned { get; set; }
        public string? Status { get; set; }
        public string? Source { get; set; }
        public string? Tags { get; set; }
        public bool WhatsappEnable { get; set; }
        public DateTime? LastContact { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

