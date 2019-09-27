using System.Collections.Generic;
using System.Threading.Tasks;
using WorkFlowManagement.Model;

namespace WorkFlowManagement.Services.Interfaces
{
    public interface ITemplateService
    {
        Task<Template> GetTemplate(int reportId);
        Task<IEnumerable<Template>> GetTemplateList(int projectId);
    }
}