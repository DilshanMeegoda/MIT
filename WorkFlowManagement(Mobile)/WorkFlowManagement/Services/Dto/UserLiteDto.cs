using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace WorkFlowManagement.Services.Dto
{
    public class UserLiteDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public string UserRole { get; set; }
        public string ProjectUserRole { get; set; }
        public string CompanyName { get; set; }
        public string ImageName { get; set; }
        public bool IsSelected { get; set; }
        public bool IsActive { get; set; }
        public DateTime ModifiedDateTime { get; set; }
    }
}