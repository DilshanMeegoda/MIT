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
using WorkFlowManagement.Enum;

namespace WorkFlowManagement.Services.Interfaces
{
    public interface IItem
    {
        int ItemId { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        Priority Priority { get; set; }
        ItemStatus Status { get; set; }
        int? AssignedTo { get; set; }
        int AnnotationNumber { get; set; }
        string Position { get; set; }
        DateTime? DueDate { get; set; }
        DateTime CreatedDateTime { get; set; }
        DateTime ModifiedDateTime { get; set; }
        int? DrawingId { get; set; }
        int ProjectId { get; set; }
    }
}