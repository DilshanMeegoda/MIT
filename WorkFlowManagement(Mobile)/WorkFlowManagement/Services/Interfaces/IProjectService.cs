using System.Collections.Generic;
using System.Threading.Tasks;
using WorkFlowManagement.Model;

namespace WorkFlowManagement.Services.Interfaces
{
    public interface IProjectService
    {
        Task<List<Project>> GetProjectList(int userId);
    }
}