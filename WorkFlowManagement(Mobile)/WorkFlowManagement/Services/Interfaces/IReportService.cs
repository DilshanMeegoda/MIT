using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WorkFlowManagement.Model;
using WorkFlowManagement.Services.Dto;

namespace WorkFlowManagement.Services.Interfaces
{
    public interface IReportService
    {
        Task<Report> GetReport(int reportId);
        Task<IEnumerable<Report>> GetReportList(int userId, int projectId);
        Task<IEnumerable<Report>> GetArchivedReportList(int projectId);
        Task<HttpResponseMessage> PostReport(ReportDataDto reportDataDto);
    }
}