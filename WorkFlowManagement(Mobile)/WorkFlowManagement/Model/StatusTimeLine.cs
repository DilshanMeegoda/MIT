using System;
using WorkFlowManagement.Enum;

namespace WorkFlowManagement.Model
{
    public class StatusTimeLine
    {
        public int StatusTimeLineId { get; set; }
        public int? SentToUserId { get; set; }
        public string SentToUserName { get; set; }
        public int SentByUserId { get; set; }
        public string SentByUserName { get; set; }
        public ReportStatus ReportStatus { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public string Signature { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int ReportDataId { get; set; }
    }
}