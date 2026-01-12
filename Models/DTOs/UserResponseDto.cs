using System.Collections.Generic;

namespace Sales_CRM.Models.DTOs
{
    public class UserResponseDto
    {
        // Profile
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ProfileImage { get; set; }

        public decimal? HourlyRate { get; set; }
        public decimal? SalesTarget { get; set; }

        public string Facebook { get; set; }
        public string Linkedin { get; set; }
        public string Skype { get; set; }
        public string EmailSignature { get; set; }
        public string Direction { get; set; }

        public string TwilioPhone { get; set; }
        public bool TwilioWhatsappEnabled { get; set; }

        public bool IsStaff { get; set; }
        public bool IsAdmin { get; set; }

        public List<int> DepartmentIds { get; set; }
    }
}
