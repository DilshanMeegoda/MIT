using System;
using System.Collections.Generic;
using WorkFlowManagement.Model;
using WorkFlowManagement.Services.Dto;

namespace WorkFlowManagement.Services.Mapper
{
    public static class ProjectMapper
    {
        public static IEnumerable<Project> ToProjectList(this IEnumerable<ProjectDto> projectDtoList)
        {
            IList<Project> projectList = new List<Project>();
            foreach (var projectDto in projectDtoList)
            {
                var project = new Project();
                project.ProjectId = projectDto.ProjectId;
                project.Title = projectDto.Title;
                project.Description = projectDto.Description;
                project.IsActive = projectDto.IsActive;
                project.ModifiedDateTime = projectDto.ModifiedDateTime;
                project.Address = projectDto.Address;
                project.Location = projectDto.Location;
                project.OwnerUserName = GetOwnerUsername(projectDto);
                project.OwnerContactNo = GetOwnerContactNo(projectDto);
                projectList.Add(project);
            }
            return projectList;
        }

        private static string GetOwnerContactNo(ProjectDto projectDto)
        {
            if (!string.IsNullOrEmpty(projectDto.OwnerUserName)) return projectDto.OwnerUserName;
            var userContactNoList = new List<string> { "0714553452", "0776543325", "0742345634" };
            var idx = new Random().Next(userContactNoList.Count);
            return userContactNoList[idx];
        }

        private static string GetOwnerUsername(ProjectDto projectDto)
        {
            if (!string.IsNullOrEmpty(projectDto.OwnerUserName)) return projectDto.OwnerUserName;
            var usernameList = new List<string>{ "Amila Silva", "Nilanka Silva", "Chinthaka Waruna" };
            var idx = new Random().Next(usernameList.Count);
            return usernameList[idx];

        }
    }
}