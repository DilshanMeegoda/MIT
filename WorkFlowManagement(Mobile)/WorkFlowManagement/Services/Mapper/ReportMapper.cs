using System.Collections.Generic;
using WorkFlowManagement.Model;
using WorkFlowManagement.Services.Dto;

namespace WorkFlowManagement.Services.Mapper
{
    public static class ReportMapper
    {
        public static IEnumerable<Report> ToReportList(this IEnumerable<ReportDataDto> reportDataDtoList)
        {
            IList<Report> reportList = new List<Report>();
            foreach (var reportDto in reportDataDtoList)
            {
                var report = new Report();
                report.ReportId = reportDto.ReportId;
                report.ReportDataId = reportDto.ReportDataId;
                report.Number = reportDto.Number;
                report.QrCode = reportDto.QrCode;
                report.ReportName = reportDto.ReportName;
                report.Description = reportDto.Description;
                report.HeaderTemplateData = reportDto.HeaderTemplateData;
                report.DetailTemplateData = reportDto.DetailTemplateData;
                report.AssignUsers = reportDto.AssignUsers;
                report.ReportStatus = reportDto.ReportStatus;
                report.OwnerUserId = reportDto.OwnerUserId;
                report.VerifierUserId = reportDto.VerifierUserId;
                report.ProjectId = reportDto.ProjectId;
                report.NoOfElements = reportDto.NoOfElements;
                report.NoOfFilledElements = reportDto.NoOfFilledElements;
                report.CreatedDateTime = reportDto.CreatedDateTime;
                report.ModifiedDateTime = reportDto.ModifiedDateTime;
                report.IsArchived = reportDto.IsArchived;
                report.CreatedUserId = reportDto.CreatedUserId;
                report.ModifiedUserId = reportDto.ModifiedUserId;
                report.StatusTimeLines = reportDto.StatusTimeLines;
                reportList.Add(report);
            }
            return reportList;
        }

        public static Report ToReport(this ReportDataDto reportDataDto)
        {

            var report = new Report();
            report.ReportId = reportDataDto.ReportId;
            report.ReportDataId = reportDataDto.ReportDataId;
            report.Number = reportDataDto.Number;
            report.QrCode = reportDataDto.QrCode;
            report.ReportName = reportDataDto.ReportName;
            report.Description = reportDataDto.Description;
            report.HeaderTemplateData = reportDataDto.HeaderTemplateData;
            report.DetailTemplateData = reportDataDto.DetailTemplateData;
            report.AssignUsers = reportDataDto.AssignUsers;
            report.ReportStatus = reportDataDto.ReportStatus;
            report.OwnerUserId = reportDataDto.OwnerUserId;
            report.VerifierUserId = reportDataDto.VerifierUserId;
            report.ProjectId = reportDataDto.ProjectId;
            report.NoOfElements = reportDataDto.NoOfElements;
            report.NoOfFilledElements = reportDataDto.NoOfFilledElements;
            report.CreatedDateTime = reportDataDto.CreatedDateTime;
            report.ModifiedDateTime = reportDataDto.ModifiedDateTime;
            report.IsArchived = reportDataDto.IsArchived;
            report.CreatedUserId = reportDataDto.CreatedUserId;
            report.ModifiedUserId = reportDataDto.ModifiedUserId;
            report.StatusTimeLines = reportDataDto.StatusTimeLines;

            return report;
        }
    }
}