using System;

namespace WorkFlowManagement.Model
{
    public class Template
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public string ReportName { get; set; }
        public string HeaderTemplate { get; set; }
        public string DetailTemplate { get; set; }
        public bool IsActive { get; set; }
        public bool IsStandard { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
    }
}