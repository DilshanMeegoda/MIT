using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WorkFlowManagement.Common;
using WorkFlowManagement.Model;
using WorkFlowManagement.Services.Dto;
using WorkFlowManagement.Services.Interfaces;
using WorkFlowManagement.Services.Mapper;

namespace WorkFlowManagement.Services
{
    public class ProjectService : BaseService, IProjectService
    {
        private const string Url = Configuration.BaseUrl + "Project/";

        public ProjectService(string token)
            : base(token)
        {
            Client.BaseAddress = new Uri(Url);
        }

        public async Task<List<Project>> GetProjectList(int userId)
        {
            var response = await Client.GetAsync($"GetAllProjects?userId={userId}");
            if (response.IsSuccessStatusCode)
            {
                var projectDtoList = await response.Content.ReadAsAsync<IEnumerable<ProjectDto>>();
                return projectDtoList.ToProjectList().ToList();
            }
            
            return new List<Project>();
        }
    }
}
