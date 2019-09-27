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
    public enum AppType
    {
        Floorplan = 1,
        Control = 2,
        Equipment = 3,
        Hours = 4,
        Forms = 5,
        PHPWebCustomer = 6,
        NETSuperAdmin = 7,
        PHPWebServices = 8,
        PHPWebSuperAdmin = 9,
        Other = 10
    }
}