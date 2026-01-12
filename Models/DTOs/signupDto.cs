using System.Collections.Generic;

namespace Sales_CRM.Models.DTOs
{
    public class SignupDto
    {
        // =========================
        // PROFILE
        // =========================
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ProfileImage { get; set; }

        // =========================
        // RATES & SALES
        // =========================
        public decimal? HourlyRate { get; set; }
        public decimal? SalesTarget { get; set; }

        // =========================
        // SOCIAL
        // =========================
        public string Facebook { get; set; }
        public string Linkedin { get; set; }
        public string Skype { get; set; }
        public string EmailSignature { get; set; }
        public string Direction { get; set; }

        // =========================
        // TWILIO
        // =========================
        public string TwilioPhone { get; set; }
        public bool TwilioWhatsappEnabled { get; set; }

        // =========================
        // FLAGS
        // =========================
        public bool IsStaff { get; set; }
        public bool IsAdmin { get; set; }

        // =========================
        // AUTH
        // =========================
        public string Password { get; set; }

        // =========================
        // DEPARTMENTS (CHECKBOXES)
        // =========================
        public List<int> DepartmentIds { get; set; }
    }
}
