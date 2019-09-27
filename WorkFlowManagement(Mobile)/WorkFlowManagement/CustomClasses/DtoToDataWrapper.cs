using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using WorkFlowManagement.Model;
using WorkFlowManagement.Services.Dto;

namespace WorkFlowManagement.CustomClasses
{
    public class DtoToDataWrapper
    {
        private ReportDataDto guestObject;

        public DtoToDataWrapper(ReportDataDto reportWithData)
        {
            guestObject = reportWithData;
        }
        public Report WrapandSendData()
        {
            return new Report
            {
                ReportDataId = guestObject.ReportDataId,
                ReportName = guestObject.ReportName,
                HeaderTemplateData = guestObject.HeaderTemplateData,
                DetailTemplateData = guestObject.DetailTemplateData,
                OwnerUserId = guestObject.OwnerUserId,
                ProjectId = guestObject.ProjectId,
                CreatedUserId = guestObject.CreatedUserId,
                ModifiedUserId = guestObject.ModifiedUserId,
                CreatedDateTime = guestObject.CreatedDateTime,
                ModifiedDateTime = guestObject.ModifiedDateTime,
                Number = guestObject.Number,
                ReportId = guestObject.ReportId,
                Description = guestObject.Description,
                ReportStatus = guestObject.ReportStatus,
                IsArchived = guestObject.IsArchived,
                ImageList = guestObject.ImageList,
                NotSynced = true
            };
        }
    }
}