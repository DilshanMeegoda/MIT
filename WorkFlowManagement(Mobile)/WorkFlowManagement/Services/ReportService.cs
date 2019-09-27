using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WorkFlowManagement.Common;
using WorkFlowManagement.Model;
using WorkFlowManagement.Services.Dto;
using WorkFlowManagement.Services.Interfaces;
using WorkFlowManagement.Services.Mapper;

namespace WorkFlowManagement.Services
{
    public class ReportService : BaseService, IReportService
    {
        private const string Url = Configuration.BaseUrl + "ReportData/";

        public ReportService(string token)
            : base(token)
        {
            Client.BaseAddress = new Uri(Url);
        }

        public async Task<Report> GetReport(int reportId)
        {
            var response = await Client.GetAsync($"GetReportData?reportDataId={reportId}");

            if (response.IsSuccessStatusCode)
            {
                var reportDataDto = await response.Content.ReadAsAsync<ReportDataDto>();
                return reportDataDto.ToReport();
            }

            return new Report();
        }

        public async Task<IEnumerable<Report>> GetReportList(int userId, int projectId)
        {
            var response = await Client.GetAsync($"GetReportDataList?userId={userId}&projectId={projectId}");

            if (response.IsSuccessStatusCode)
            {
                var reportDataDtoList = await response.Content.ReadAsAsync<IEnumerable<ReportDataDto>>();
                return reportDataDtoList.ToReportList().ToList();
            }

            return new List<Report>();
        }

        public async Task<IEnumerable<Report>> GetArchivedReportList(int projectId)
        {
            var response = await Client.GetAsync($"GetArchivedList?projectId={projectId}");

            if (response.IsSuccessStatusCode)
            {
                var archivedReportDataDtoList = await response.Content.ReadAsAsync<IEnumerable<ReportDataDto>>();
                return archivedReportDataDtoList.ToReportList().ToList();
            }

            return new List<Report>();
        }

        public async Task<HttpResponseMessage> PostReport(ReportDataDto reportDataDto)
        {
            var json = JsonConvert.SerializeObject(reportDataDto, Formatting.Indented);

            var byteContent = new ByteArrayContent(Encoding.UTF8.GetBytes(json));

            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return Client.PostAsync("PostReportData", byteContent).Result;
        }
    }
}