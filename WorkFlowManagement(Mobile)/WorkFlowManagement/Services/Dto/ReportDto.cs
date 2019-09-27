using System;
using System.Collections.Generic;
using WorkFlowManagement.Model;

namespace WorkFlowManagement.Services.Dto
{
    public class ReportDto
    {
        public int ReportId { get; set; }
        public string ReportName { get; set; }
        public List<ReportElement> HeaderTemplate { get; set; }
        public List<ReportElement> DetailTemplate { get; set; }
        public string HeaderTemplateJson { get; set; }
        public string DetailTemplateJson { get; set; }
        public bool IsActive { get; set; }
        public bool IsStandard { get; set; }
        public int LastElementId { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public bool IsDeleted { get; set; }
        public int? ReportCategoryId { get; set; }
        public string ReportCategory { get; set; }
    }
}