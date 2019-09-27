using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WorkFlowManagement.Common;
using WorkFlowManagement.Model;
using WorkFlowManagement.Services.Dto;
using WorkFlowManagement.Services.Interfaces;
using WorkFlowManagement.Services.Mapper;

namespace WorkFlowManagement.Services
{
    public class TemplateService : BaseService, ITemplateService
    {
        private const string Url = Configuration.BaseUrl + "Report/";

        public TemplateService(string token)
            : base(token)
        {
            Client.BaseAddress = new Uri(Url);
        }

        public async Task<Template> GetTemplate(int reportId)
        {
            var response = await Client.GetAsync($"GetReport?reportId={reportId}");

            if (response.IsSuccessStatusCode)
            {
                var templateData = await response.Content.ReadAsAsync<ReportDto>();
                return templateData.ToTemplate();
            }

            return new Template();
        }

        public async Task<IEnumerable<Template>> GetTemplateList(int projectId)
        {
            var response = await Client.GetAsync($"GetReportsByProject?projectId={projectId}");

            if (response.IsSuccessStatusCode)
            {
                var templateDataList = await response.Content.ReadAsAsync<IEnumerable<ReportDto>>();
                return templateDataList.ToTemplateList();
            }

            return new[] {new Template()};
        }
    }
}
