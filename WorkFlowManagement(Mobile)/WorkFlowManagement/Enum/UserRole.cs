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

namespace WorkFlowManagement.Enum
{
    public enum UserRole
    {
        SuperAdmin = 1,
        Admin = 2,
        Normal = 3,
        CompanyAdmin = 4,
        ProjectManager = 5,
        InvitedUser = 6
    }
}