using System;
using System.Collections.Generic;
using System.Net;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Util;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using WorkFlowManagement.Activities;
using WorkFlowManagement.Common;
using WorkFlowManagement.CustomClasses;
using WorkFlowManagement.Enum;
using WorkFlowManagement.Model;
using WorkFlowManagement.Services;
using WorkFlowManagement.Services.Dto;
using WorkFlowManagement.Services.Interfaces;
using Environment = Android.OS.Environment;
using File = Java.IO.File;
using glAndroid = Android;
using Path = System.IO.Path;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WorkFlowManagement.Fragments
{
    public class VerifyReportFragment : DialogFragment
    {
        private IReportService reportService;
        private TextView AcceptHeader;
        private UserSession currentSession;
        private TextView DateView;
        private TextView Description;
        private string filName;
        private Report m_reportData;
        private TextView NameView;
        private Button ProceedButton;
        private EditText ReasonView;
        private string sdCardPath;
        private ISharedPreferences sharedPreferences;
        private ISharedPreferencesEditor sharedPreferencesEditor;
        private bool signatureFlag;
        private string type;
        private UserSession userSession;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            const int theme = glAndroid.Resource.Style.ThemeDeviceDefault;
            const DialogFragmentStyle style = (DialogFragmentStyle) glAndroid.Resource.Style.ThemeMaterialLight;
            SetStyle(style, theme);

            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            sharedPreferencesEditor = sharedPreferences.Edit();

            var userSessionJsonString =
                sharedPreferences.GetString(Resources.GetString(Resource.String.user_session), string.Empty);
            userSession = JsonConvert.DeserializeObject<UserSession>(userSessionJsonString);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            signatureFlag = false;
            sdCardPath = Environment.ExternalStorageDirectory.AbsolutePath;
            filName = "verifier_signature_" + Guid.NewGuid() + ".jpg";

            var view = inflater.Inflate(Resource.Layout.fragment_verify_report, container, false);

            var toolbar = view.FindViewById<Toolbar>(Resource.Id.my_toolbar_verify);
            toolbar.Title = "Description";
            toolbar.SetTitleTextColor(Color.ParseColor("#000000"));

            ProceedButton = view.FindViewById<Button>(Resource.Id.buttonProceed);
            NameView = view.FindViewById<TextView>(Resource.Id.textViewVerifierName);
            DateView = view.FindViewById<TextView>(Resource.Id.textViewVerifieddate);
            Description = view.FindViewById<TextView>(Resource.Id.textViewVerifydescription);
            ReasonView = view.FindViewById<EditText>(Resource.Id.editTextVerifyReason);
            AcceptHeader = view.FindViewById<TextView>(Resource.Id.textViewAcceptHeader);

            ProceedButton.Click += ProceedConfirmation;

            type = Arguments.GetString("type");
            var jsonObject = Arguments.GetString("JasonObject");
            m_reportData = JsonConvert.DeserializeObject<Report>(jsonObject);

            switch (type)
            {
                case "accept":
                    ProceedButton.Click += AcceptConfirmation;
                    ProceedButton.Text = "Accept as Verified";
                    ProceedButton.SetBackgroundColor(Color.ParseColor("#23c064"));
                    AcceptHeader.Text = "Accept the report";
                    break;
                case "decline":
                    ProceedButton.Click += DeclineConfirmation;
                    ProceedButton.Text = "Send Report Back";
                    ProceedButton.SetBackgroundColor(Color.ParseColor("#fc6042"));
                    AcceptHeader.Text = "Decline the report with reason";
                    break;
                case "proceed":
                    ProceedButton.Click += ProceedConfirmation;
                    ProceedButton.Text = "Proceed to Report";
                    ProceedButton.SetBackgroundColor(Color.ParseColor("#18a9f0"));
                    AcceptHeader.Text = "";
                    break;
            }

            InitializeElements();
            return view;
        }

        private void AcceptConfirmation(object sender, EventArgs e)
        {
            SaveReport(ReportStatus.ArchivedAsAccepted, "");
        }

        private void DeclineConfirmation(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ReasonView.Text))
            {
                Utility.DisplayToast(Application.Context, "Please Add Reason Text");
            }
            else
            {
                SaveReport(ReportStatus.Rejected, ReasonView.Text);
            }
        }

        private void ProceedConfirmation(object sender, EventArgs e)
        {
            Dialog.Dismiss();
        }

        private void InitializeElements()
        {
            StatusTimeLineDto stats = null;
            if (m_reportData.StatusTimeLines != null)
                stats = m_reportData.StatusTimeLines[m_reportData.StatusTimeLines.Count - 1];

            DateView.Text = DateTime.Now.ToShortDateString();

            switch (type)
            {
                case "accept":
                    Description.Visibility = ViewStates.Gone;
                    ReasonView.Visibility = ViewStates.Gone;
                    NameView.Text = userSession.FullName;
                    break;
                case "decline":
                    NameView.Text = userSession.FullName;
                    break;
                case "proceed":
                {
                    ReasonView.Enabled = false;

                    switch (m_reportData.ReportStatus)
                    {
                        case ReportStatus.Rejected:
                            AcceptHeader.Text = "Report Status : " + "Rejected";
                            AcceptHeader.SetTextColor(Color.ParseColor("#fc6042"));
                            break;
                        case ReportStatus.ArchivedAsAccepted:
                            AcceptHeader.Text = "Report Status : " + "Verified";
                            break;
                    }

                    Description.Text = "Reason Given";
                    if (stats != null)
                    {
                        NameView.Text = stats.SentByUserName;
                        ReasonView.Text = stats.Note;
                    }
                    break;
                }
            }
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var dialog = base.OnCreateDialog(savedInstanceState);
            dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            return dialog;
        }

        private async void SaveReport(ReportStatus status, string note)
        {
            ReportDataDto reportDataDto = GetReportDataDto(m_reportData);
            var statusObject = TimeLineObjectCreator(status, note);
            reportDataDto.ItemDtos = reportDataDto.ItemDtos ?? new List<ItemDto>();
            reportDataDto.StatusTimeLines = new List<StatusTimeLineDto> {statusObject};

            if (Utility.IsInternetAvailable(Application.Context))
            {
                reportDataDto.NotSynced = false;

                if (status == ReportStatus.ArchivedAsAccepted)
                {
                    reportDataDto.ReportStatus = status;
                    reportDataDto.IsArchived = true;
                }
                else if (status == ReportStatus.Rejected)
                {
                    reportDataDto.ReportStatus = status;
                }

                try
                {
                    reportService = new ReportService(userSession.AccessToken);
                    await reportService.PostReport(reportDataDto);

                    if (type == "accept")
                    {
                        Utility.DisplayToast(Application.Context, "Report has been Accepted");
                    }
                    else if (type == "decline")
                    {
                        Utility.DisplayToast(Application.Context, "Report has been Declined");
                    }
                    sharedPreferences.Edit().PutBoolean(Resources.GetString(Resource.String.IsReportUpdate), false).Commit();
                    sharedPreferencesEditor.PutBoolean("BackPressed", false);
                    sharedPreferencesEditor.Commit();
                    Activity.Finish();
                }
                catch (CustomHttpResponseException customHttpResponseException)
                {
                    if (customHttpResponseException.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        Log.Error("Authentication Exception - VerifyReportFragment", customHttpResponseException.ToString());
                        Activity.Finish();
                        Activity.StartActivity(typeof(LoginActivity));
                    }
                    else
                    {
                        Log.Error(customHttpResponseException.StatusCode + " - VerifyReportFragment", customHttpResponseException.ToString());
                        Utility.DisplayToast(Application.Context, "Error : " + customHttpResponseException.StatusCode + "\nAn error occurred while connecting to the application server");
                    }
                }
            }
            else
            {
                Utility.DisplayToast(Application.Context, Resources.GetString(Resource.String.no_internet_message));
            }
        }

        private ReportDataDto GetReportDataDto(Report mReportData)
        {
            return  new ReportDataDto
            {
                CreatedUserId = mReportData.CreatedUserId,
                OwnerUserId =  mReportData.OwnerUserId,
                ReportDataId = mReportData.ReportDataId,
                VerifierUserId = mReportData.VerifierUserId,
                Number = mReportData.Number,
                ProjectId = mReportData.ProjectId,
                ReportName = mReportData.ReportName,
                DetailTemplateData =  mReportData.DetailTemplateData,
                AssignUsers = mReportData.AssignUsers,
                CreatedDateTime = mReportData.CreatedDateTime,
                Description = mReportData.Description,
                DisplayUserImageUrl = mReportData.DisplayUserImageUrl,
                HeaderTemplateData = mReportData.HeaderTemplateData,
                ImageList = mReportData.ImageList,
                IsArchived = mReportData.IsArchived,
                ItemDtos = mReportData.ItemDtos,
                ModifiedDateTime = mReportData.ModifiedDateTime,
                ModifiedUserId = mReportData.ModifiedUserId,
                NoOfElements = mReportData.NoOfElements,
                NoOfFilledElements = mReportData.NoOfFilledElements,
                NotSynced = mReportData.NotSynced,
                QrCode = mReportData.QrCode,
                ReportId = mReportData.ReportId,
                ReportStatus = mReportData.ReportStatus
            };
        }

        private StatusTimeLineDto TimeLineObjectCreator(ReportStatus status, string note)
        {
            var timelineObject = new StatusTimeLineDto
            {
                CreatedDateTime = DateTime.Now,
                ReportDataId = m_reportData.ReportDataId,
                SentByUserId = (int) m_reportData.VerifierUserId,
                SentToUserId = m_reportData.OwnerUserId
            };

            switch (status)
            {
                case ReportStatus.ArchivedAsAccepted:
                    timelineObject.ReportStatus = status;
                    timelineObject.Note = "";
                    break;
                case ReportStatus.Rejected:
                    timelineObject.ReportStatus = status;
                    timelineObject.Note = note;
                    break;
            }

            timelineObject.Signature = string.Empty;

            return timelineObject;
        }
    }
}