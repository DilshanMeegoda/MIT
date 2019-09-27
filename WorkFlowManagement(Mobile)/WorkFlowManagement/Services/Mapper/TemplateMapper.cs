using System.Collections.Generic;
using WorkFlowManagement.Model;
using WorkFlowManagement.Services.Dto;

namespace WorkFlowManagement.Services.Mapper
{
    public static class TemplateMapper
    {
        public static IEnumerable<Template> ToTemplateList(this IEnumerable<ReportDto> reportDtotList)
        {
            IList<Template> templateList = new List<Template>();
            foreach (var reportDto in reportDtotList)
            {
                var template = new Template
                {
                    ReportId = reportDto.ReportId,
                    ReportName = reportDto.ReportName,
                    HeaderTemplate = reportDto.HeaderTemplateJson,
                    DetailTemplate = reportDto.DetailTemplateJson,
                    IsActive = reportDto.IsActive,
                    IsStandard = reportDto.IsStandard,
                    CreatedDateTime = reportDto.CreatedDateTime,
                    ModifiedDateTime = reportDto.ModifiedDateTime
                };
                templateList.Add(template);
            }
            return templateList;
        }

        public static Template ToTemplate(this ReportDto reportDto)
        {
            var report = new Template
            {
                ReportId = reportDto.ReportId,
                ReportName = reportDto.ReportName,
                HeaderTemplate = reportDto.HeaderTemplateJson,
                DetailTemplate = reportDto.DetailTemplateJson,
                IsActive = reportDto.IsActive,
                IsStandard = reportDto.IsStandard,
                CreatedDateTime = reportDto.CreatedDateTime,
                ModifiedDateTime = reportDto.ModifiedDateTime
            };

            return report;
        }
    }
}