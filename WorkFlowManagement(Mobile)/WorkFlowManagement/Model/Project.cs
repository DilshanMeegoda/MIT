﻿using System;
using System.Collections.Generic;
using WorkFlowManagement.Services.Dto;

namespace WorkFlowManagement.Model
{
    public class Project 
    {
        public Project()
        {
            CreatedDateTime = DateTime.Now;
            ModifiedDateTime = DateTime.Now;
        }
        public int ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public int CompanyId { get; set; }
        public int? OwnerUserId { get; set; }
        public string OwnerUserName { get; set; }
        public string OwnerContactNo { get; set; }
        public int[] SelectedReports { get; set; }
        public int[] SelectedUsers { get; set; }
        public List<AutoCompleteCommonDto> ReportLiteDtoList { get; set; }
        public List<UserLiteDto> AssignUserList { get; set; }
        public bool IsArchived { get; set; }
        public DateTime ArchivedDateTime { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
    }
}