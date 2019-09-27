using System;
using System.Collections.Generic;
using WorkFlowManagement.Enum;
using WorkFlowManagement.Services.Dto;

namespace WorkFlowManagement.Model
{
    public class Report
    {
        public int ReportId { get; set; }
        public int ReportDataId { get; set; }
        public int Number { get; set; }
        public string QrCode { get; set; }
        public string ReportName { get; set; }
        public string Description { get; set; }
        public string HeaderTemplateData { get; set; }
        public string DetailTemplateData { get; set; }
        public List<int> AssignUsers { get; set; }
        public ReportStatus ReportStatus { get; set; }
        public int OwnerUserId { get; set; }
        public int? VerifierUserId { get; set; }
        public bool NotSynced { get; set; }
        public int ProjectId { get; set; }
        public int NoOfElements { get; set; }
        public int NoOfFilledElements { get; set; }
        public bool IsArchived { get; set; }
        public string ImageList { get; set; }
        public string DisplayUserImageUrl { get; set; }
        public int CreatedUserId { get; set; }
        public int ModifiedUserId { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public virtual List<StatusTimeLineDto> StatusTimeLines { get; set; }
        public virtual List<ItemDto> ItemDtos { get; set; }
    }
}
