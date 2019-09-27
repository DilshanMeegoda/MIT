using System;

namespace WorkFlowManagement.Model
{
    public class User
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserRole { get; set; }
        public string ProjectUserRole { get; set; }
        public string CompanyName { get; set; }
        public string ImageName { get; set; }
        public bool IsActive { get; set; }
        public bool IsFavourite { get; set; }
        public DateTime ModifiedDateTime { get; set; }
    }
}