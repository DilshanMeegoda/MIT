using System;

namespace WorkFlowManagement.Services.Dto
{
    public class UserSessionDto
    {
        public int Id { get; set; }
        public int UserSessionId { get; set; }
        public string FullName { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public int? CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string UserImage { get; set; }
        public string CompanyImage { get; set; }
        public bool IsPaid { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
    }
}