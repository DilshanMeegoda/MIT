using System;
using System.Collections.Generic;
using WorkFlowManagement.Enum;
using WorkFlowManagement.Services.Interfaces;

namespace WorkFlowManagement.Services.Dto
{
    public class ItemDto : IItem
    {
        public int ItemId { get; set; }
        public int OldAnnotationId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Priority Priority { get; set; }
        public ItemStatus Status { get; set; }
        public int? AssignedTo { get; set; }
        public int AnnotationNumber { get; set; }
        public string Position { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public int? DrawingId { get; set; }
        public int ProjectId { get; set; }
        public List<string> Images { get; set; }
    }
}
