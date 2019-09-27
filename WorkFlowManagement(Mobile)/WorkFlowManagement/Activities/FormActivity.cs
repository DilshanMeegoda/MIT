using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Preferences;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Lang;
using Java.Lang.Reflect;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UniversalImageLoader.Core;
using WorkFlowManagement.Common;
using WorkFlowManagement.CustomClasses;
using WorkFlowManagement.CustomViews;
using WorkFlowManagement.Enum;
using WorkFlowManagement.Fragments;
using WorkFlowManagement.Model;
using WorkFlowManagement.Services;
using WorkFlowManagement.Services.Dto;
using WorkFlowManagement.Services.Interfaces;
using AlertDialog = Android.App.AlertDialog;
using Exception = System.Exception;
using String = System.String;

namespace WorkFlowManagement.Activities
{
    [Activity(Label = "Form Activity", WindowSoftInputMode = SoftInput.AdjustPan, ScreenOrientation = ScreenOrientation.Portrait)]
    public class FormActivity : AppCompatActivity
    {
        private ISharedPreferences sharedPreferences;
        private ISharedPreferencesEditor sharedPreferencesEditor;
        private IReportService reportService;
        private ITemplateService templateService;

        private ReportDataDto reportDataWithData;

        //Constants to Define 
        private const int MainframeId = 206;
        private const int ScrollViewId = 406;
        private const int CompleteButtonId = 306;
        private const int VerifyButtonId = 307;
        private const int ScrollHeaderId = 407;
        private const int GreenHeaderId = 606;
        private const int ProceedId = 607;

        //Constant to Identify ImageIndicator
        public const int IMAGE_PREVIEW_Header_ID = 134198;
        public const int IMAGE_PREVIEW_Info_ID = 1348765;
        
        private ReportStatus statusReport;
        private List<ReportElement> informationElementList;
        private List<ReportElement> headerElementList;

        private Report report;
        private Template template;
        private ReportElement informationReportElement;
        private ReportElement headerReportElement;
        
        public static Location _currentLocation;
        public static LocationManager _locationManager;
        private string latit;
        private string longtit;

        private int createdUser;
        private int assigneeID;
        private string assignedUserName;
        private string verifierName;
        private string ownerName;
        private int verifierID;
        private int OwnerID;
        private int? reportAssigneeID;
        private int userID;
        private int projectID;
        private int photoOption;
        private bool reportEditFlag;
        private static int reportID;
        private int reportnumber;
        private int? companyID;
        private string companyName;
        private string formName;
        private string formType;
        private string formFormat;
        private string headerFormat;
        private Bitmap bitmapImage;
        private File imageFile;
        public string typeFlag;
        private ImageLoader imageLoader;

        //ElementCounter
        private int totalHeaderElements;
        private int filledHeaderElements;
        private int filledInfoElements;
        private string totalInfoElements;

        private Fragment currentFragment;
        private FormHeaderFragment headerFragment;
        private FormInformationFragment informationFragment;

        private RelativeLayout mainFrameLayout;
        private LinearLayout dynamiclayout;
        private LinearLayout dynamiclayoutHeader;
        private LinearLayout themeStruct;
        private ScrollView scroller;
        private ScrollView scrollerHeader;
        private RelativeLayout mainHeaderLayout;
        private LinearLayout verifyButtonsHolder;
        private ProgressDialog progressDialog;
        private ImageButton leftButton;
        private ImageButton rightButton;
        private TextView elementCounter;
        private TextView headerGreenLine;
        private TextView elementSplitLine;
        private TextView viewName;
        private TextView viewPageName;
        private Button completeButton;
        private Button proceedButton;
        private Button declineButton;
        private Button acceptButton;

        private RelativeLayout.LayoutParams paramsHeaderText;
        private RelativeLayout.LayoutParams paramsScroller;
        private RelativeLayout.LayoutParams paramsScrollerHeader;
        private RelativeLayout.LayoutParams paramsCompleteAuditButton;
        private RelativeLayout.LayoutParams paramsVerifyButtons;
        private RelativeLayout.LayoutParams ProceedButton;

        private List<int> assignedUserIDs = new List<int>();
        private Project currentProject;
        private UserSession userSession;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_form);
            statusReport = new ReportStatus();
            InitializeElements();
        }

        private async void InitializeElements()
        {
            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);
            sharedPreferencesEditor = sharedPreferences.Edit();

            var userSessionJsonString = sharedPreferences.GetString(Resources.GetString(Resource.String.user_session), string.Empty);
            userSession = JsonConvert.DeserializeObject<UserSession>(userSessionJsonString);

            var currentProjectJsonString = sharedPreferences.GetString(Resources.GetString(Resource.String.current_project), string.Empty);
            currentProject = JsonConvert.DeserializeObject<Project>(currentProjectJsonString);
            
            userID = userSession.UserId;

            reportID = sharedPreferences.GetInt(Resources.GetString(Resource.String.report_id), 0);
            formName = sharedPreferences.GetString(Resources.GetString(Resource.String.report_name), "Form");
            formType = sharedPreferences.GetString(Resources.GetString(Resource.String.report_type), "draft");
            projectID = currentProject.ProjectId;

            Title = formName;
            informationReportElement = new ReportElement();
            headerReportElement = new ReportElement();

            await GetReportData();

            Title = report != null ? formName + " " + report.Number : formName;

            SetUpFormMainStructure();
            SetupHeaderFormElements();
            SetUpFormElements();
            GetElementLists();

            try
            {
                CreateCustomInformationViewLoop(informationElementList);
                CreateCustomHeaderViewLoop(headerElementList);
                CountOfFieldsWithValue(headerElementList, informationElementList);
                InitializeFragments();

                GetOverFlowMenu();

                if (report.ReportStatus == ReportStatus.ArchivedAsAccepted || report.ReportStatus == ReportStatus.Rejected)
                {
                    VerifyConfirmationAtStart("proceed");
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Get Report", ex.ToString());
            }
        }

        private void VerifyConfirmationAtStart(string type)
        {
            var bundle = new Bundle();
            bundle.PutString("type", type);

            var json = JsonConvert.SerializeObject(report);
            bundle.PutString("JasonObject", json);

            var fragmentTransaction = FragmentManager.BeginTransaction();
            var verifyDialogFragment = new VerifyReportFragment {Arguments = bundle};

            try
            {
                verifyDialogFragment.Show(fragmentTransaction, Resources.GetString(Resource.String.DialogFragment));
            }
            catch (Exception ex)
            {
                Log.Error("Fragment Error", ex.ToString());
                Toast.MakeText(this, "Something went wrong, please try reloading the report", ToastLength.Short).Show();
            }
        }

        private void GetOverFlowMenu()
        {
            try
            {
                var config = ViewConfiguration.Get(this);
                var menuKeyField = config.Class.GetDeclaredField("sHasPermanentMenuKey");

                if (menuKeyField == null) return;
                menuKeyField.Accessible = true;
                menuKeyField.SetBoolean(config, false);
            }
            catch (Java.Lang.Exception ex)
            {
                Log.Debug("Exception", "GetOverFlow Menu");
            }
        }

        private void GetElementLists()
        {
            try
            {
                switch (formType)
                {
                    case "template":
                        formFormat = template.DetailTemplate;
                        headerFormat = template.HeaderTemplate;
                        break;
                    case "archive":
                    case "draft":
                        formFormat = report.DetailTemplateData;
                        headerFormat = report.HeaderTemplateData;
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Empty Template", ex.ToString());
                Toast.MakeText(this, "Error loading the report, please try reloading the report", ToastLength.Short).Show();
            }
            
            try
            {
                if (formFormat != null)
                    informationElementList =
                        (List<ReportElement>)JsonConvert.DeserializeObject(formFormat, typeof(List<ReportElement>));

                if (headerFormat != null)
                    headerElementList =
                        (List<ReportElement>)JsonConvert.DeserializeObject(headerFormat, typeof(List<ReportElement>));
            }
            catch (Java.Lang.Exception e)
            {
                e.PrintStackTrace();
                Log.Debug("Exception", "Report Element Serialization");
            }
        }

        private async void CreateCustomInformationViewLoop(IReadOnlyList<ReportElement> informationList)
        {
            typeFlag = "Info";

            if (report != null && report.IsArchived)
            {
                sharedPreferencesEditor.PutBoolean(Resources.GetString(Resource.String.is_archived), true);
                sharedPreferencesEditor.Commit();
            }

            for (var i = 0; i < informationList.Count; i++)
            {
                informationReportElement = informationList[i];
                CreteCustomView(informationReportElement, "form", OwnerID, i + 1);
            }

            if (report != null)
            {
                paramsScroller = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);

                if (report.ReportStatus == ReportStatus.ArchivedAsAccepted ||
                    report.ReportStatus == ReportStatus.Rejected)
                {
                    dynamiclayout.AddView(proceedButton);
                }
                else if (userID == verifierID)
                {
                    dynamiclayout.AddView(verifyButtonsHolder);
                }
                else
                {
                    dynamiclayout.AddView(completeButton);
                }
            }
            else
            {
                dynamiclayout.AddView(completeButton);
            }

            scroller.AddView(dynamiclayout);
            mainFrameLayout.AddView(scroller);
        }

        private async void CreateCustomHeaderViewLoop(IReadOnlyList<ReportElement> headerList)
        {
            typeFlag = "Head";

            for (int i = 0; i < headerList.Count; i++)
            {
                headerReportElement = headerList[i];
                CreteCustomView(headerReportElement, "header", OwnerID, i + 1);
            }

            scrollerHeader.AddView(dynamiclayoutHeader);
            mainHeaderLayout.AddView(headerGreenLine);
            mainHeaderLayout.AddView(scrollerHeader);

            if (report != null && report.IsArchived)
            {
                completeButton.Enabled = false;
            }

            sharedPreferencesEditor.PutBoolean(Resources.GetString(Resource.String.is_archived), false);
            sharedPreferencesEditor.Commit();
        }

        public void CreteCustomView(ReportElement element, String type, int ownerID, int position)
        {
            while (true)
            {
                switch (element.Type)
                {
                    case "textfield":
                        LinearLayout formEditText = new FormEditText(this, element, userID, ownerID, verifierID, statusReport);
                        SaveView(formEditText, ElementSplitLine(), type);
                        break;
                    case "textfieldint":
                        LinearLayout formIntEditText = new FormIntEditText(this, element, userID, ownerID, verifierID, statusReport);
                        SaveView(formIntEditText, ElementSplitLine(), type);
                        break;
                    case "slider":
                        LinearLayout formSlider = new FormSlider(this, element, userID, ownerID, verifierID, statusReport);
                        SaveView(formSlider, ElementSplitLine(), type);
                        break;
                    case "signature":
                        LinearLayout formSignature = new FormSignature(this, element, type, userID, ownerID, verifierID, formType, statusReport, null);
                        SaveView(formSignature, ElementSplitLine(), type);
                        break;
                    case "Button":
                        LinearLayout formButton = new FormButton(this, element, ownerID, verifierID);
                        SaveView(formButton, ElementSplitLine(), type);
                        break;
                    case "yesno":
                        LinearLayout formSwitch = new FormSwitch(this, element, userID, ownerID, verifierID, formType, statusReport, type, imageLoader);
                        SaveView(formSwitch, ElementSplitLine(), type);
                        break;
                    case "multilinetextfield":
                        LinearLayout formMultiLineEditText = new FormMultiLineEditText(this, element, userID, ownerID, verifierID, statusReport);
                        SaveView(formMultiLineEditText, ElementSplitLine(), type);
                        break;
                    case "datetime":
                        LinearLayout formDateTime = new FormDateTime(this, element, ownerID, verifierID);
                        SaveView(formDateTime, ElementSplitLine(), type);
                        break;
                    case "date":
                        LinearLayout formDate = new FormDate(this, element, userID, ownerID, verifierID, statusReport);
                        SaveView(formDate, ElementSplitLine(), type);
                        break;
                    case "time":
                        LinearLayout formTime = new FormTime(this, element, userID, ownerID, verifierID, statusReport);
                        SaveView(formTime, ElementSplitLine(), type);
                        break;
                    case "camera":
                        LinearLayout formCamera = new FormCamera(this, element, userID, ownerID, verifierID, statusReport, typeFlag, type, null);
                        SaveView(formCamera, ElementSplitLine(), type);
                        break;
                    case "checkbox":
                        LinearLayout checkBoxLayout = new FormCheckBox(this, element, userID, ownerID, verifierID, statusReport, formType, type, imageLoader);
                        checkBoxLayout.Id = element.Id;
                        SaveView(checkBoxLayout, ElementSplitLine(), type);
                        break;
                    case "dropdown":
                        LinearLayout formDropDown = new FormDropDown(this, element, userID, ownerID, verifierID, statusReport);
                        formDropDown.Id = element.Id;
                        SaveView(formDropDown, ElementSplitLine(), type);
                        break;
                    case "mainandsubfield":
                        LinearLayout formHeaderSubElement = new FormHeaderSubElement(this, element, userID, verifierID);
                        formHeaderSubElement.Id = element.Id;
                        SaveView(formHeaderSubElement, ElementSplitLine(), type);
                        break;
                    case "updown":
                        LinearLayout formPlusMinusCounter = new FormPlusMinusCounter(this, element, userID, ownerID, verifierID, statusReport);
                        SaveView(formPlusMinusCounter, ElementSplitLine(), type);
                        break;
                    case "gps":
                        FormGPS fromClass = new FormGPS(this, element, ownerID, userID, verifierID, statusReport);
                        SaveView(fromClass, ElementSplitLine(), type);
                        break;
                    case "tabularform":
                        FormTabular formTabular = new FormTabular(this, element, ownerID, verifierID, type, statusReport);
                        formTabular.Id = element.Id;
                        SaveView(formTabular, ElementSplitLine(), type);
                        break;
                }
                break;
            }
        }

        private View ElementSplitLine()
        {
            elementSplitLine = new TextView(this) {TextSize = 0.5f};
            elementSplitLine.SetBackgroundColor(Color.ParseColor(Resources.GetString(Resource.Color.grey)));

            return elementSplitLine;
        }

        private void SaveView(View v1, View v2, String type)
        {
            switch (type)
            {
                case "form":
                    dynamiclayout.AddView(v1);
                    dynamiclayout.AddView(v2);
                    break;
                case "header":
                    dynamiclayoutHeader.AddView(v1);
                    dynamiclayoutHeader.AddView(v2);
                    break;
            }
        }

        private void CountOfFieldsWithValue(IReadOnlyCollection<ReportElement> headerList, IReadOnlyCollection<ReportElement> infoList)
        {
            try
            {
                var countWithValuesDetail = headerList.Count(x => !string.IsNullOrEmpty(x.Value));
                var countWithValuesHeader = infoList.Count(x => !string.IsNullOrEmpty(x.Value));
                var countWithValues = countWithValuesDetail + countWithValuesHeader;
                var allElements = headerList.Count + infoList.Count;

                totalInfoElements = countWithValues + "/" + allElements;
                elementCounter.Text = totalInfoElements;
            }
            catch (Exception ex)
            {
                Log.Debug("Page Count Exception", ex.ToString());
            }

        }

        private void InitializeFragments()
        {

            headerFragment = new FormHeaderFragment(mainHeaderLayout);
            informationFragment = new FormInformationFragment(mainFrameLayout);

            var trans = FragmentManager.BeginTransaction();

            trans.Add(Resource.Id.frameforfrags, informationFragment, "Info");
            trans.Hide(informationFragment);
            trans.Add(Resource.Id.frameforfrags, headerFragment, "Header");

            trans.Commit();
            currentFragment = headerFragment;

            viewName.Text = "Header";
            viewPageName.Text = "Section 1 of 2";

            leftButton.Enabled = false;
            rightButton.Enabled = true;

        }

        private async Task GetReportData()
        {
            try
            {
                switch (formType)
                {
                    case "template":
                        templateService = new TemplateService(userSession.AccessToken);
                        template = await templateService.GetTemplate(reportID);
                        OwnerID = userID;
                        verifierID = 0;
                        break;
                    case "draft":
                    case "archive":
                    {
                        reportService = new ReportService(userSession.AccessToken);
                        report = await reportService.GetReport(reportID);
                        OwnerID = report.OwnerUserId;
                        if (report.VerifierUserId != null) verifierID = (int) report.VerifierUserId;
                        break;
                    }
                }
            }
            catch (CustomHttpResponseException customHttpResponseException)
            {
                if (customHttpResponseException.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Log.Error("Authentication Exception - FormActivity", customHttpResponseException.ToString());
                    Finish();
                    StartActivity(typeof(LoginActivity));
                }
                else
                {
                    Log.Error(customHttpResponseException.StatusCode + " - FormActivity", customHttpResponseException.ToString());
                    Utility.DisplayToast(this, "Error : " + customHttpResponseException.StatusCode + "\nAn error occurred while connecting to the application server");
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Exception", ex.ToString());
                Toast.MakeText(this, "Error loading the report, please try reloading the report", ToastLength.Short).Show();
            }
        }

        private void SetUpFormMainStructure()
        {
            mainFrameLayout = new RelativeLayout(this) {Id = MainframeId};
            mainFrameLayout.SetBackgroundColor(Color.ParseColor("#f4f4f4"));

            mainHeaderLayout = new RelativeLayout(this);

            viewName = FindViewById<TextView>(Resource.Id.viewNametextView);
            viewPageName = FindViewById<TextView>(Resource.Id.viewpagenumber);

            rightButton = FindViewById<ImageButton>(Resource.Id.toChangeInfo);
            leftButton = FindViewById<ImageButton>(Resource.Id.toHeader);

            elementCounter = FindViewById<TextView>(Resource.Id.textViewElementCounter);

            leftButton.Click += LeftBtn_Click;
            rightButton.Click += RightBtn_Click;
        }

        private void LeftBtn_Click(object sender, EventArgs e)
        {
            ShowFragment(headerFragment);
            leftButton.Enabled = false;
            rightButton.Enabled = true;

            viewName.Text = "Header";
            viewPageName.Text = "Section 1 of 2";
        }

        private void RightBtn_Click(object sender, EventArgs e)
        {
            ShowFragment(informationFragment);

            leftButton.Enabled = true;
            rightButton.Enabled = false;

            viewName.Text = "Change-Information";
            viewPageName.Text = "Section 2 of 2";
        }

        private void ShowFragment(Fragment fragment)
        {
            var trans = FragmentManager.BeginTransaction();
            trans.Hide(currentFragment);
            trans.Show(fragment);
            trans.AddToBackStack(null);
            trans.Commit();
            currentFragment = fragment;
        }

        private void SetupHeaderFormElements()
        {
            dynamiclayoutHeader = new FragmentHolderLayout(this);

            scrollerHeader = new ScrollView(this) {Id = ScrollHeaderId};

            headerGreenLine = new TextView(this) {Id = GreenHeaderId, TextSize = 3};
            headerGreenLine.SetBackgroundColor(Color.ParseColor(GetString(Resource.Color.green_primary)));

            paramsHeaderText = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.WrapContent);
            paramsHeaderText.AddRule(LayoutRules.AlignParentTop);
            headerGreenLine.LayoutParameters = paramsHeaderText;

            paramsScrollerHeader = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent);
            scrollerHeader.LayoutParameters = paramsScrollerHeader;

        }

        private void SetUpFormElements()
        {
            dynamiclayout = new FragmentHolderLayout(this);
            verifyButtonsHolder = new LinearLayout(this);

            scroller = new ScrollView(this) {Id = ScrollViewId};

            verifyButtonsHolder = CreateVerifyButtons();
            verifyButtonsHolder.SetPadding(20, 5, 20, 5);
            verifyButtonsHolder.Id = VerifyButtonId;

            completeButton = CreateCompleteAuditBtn();
            completeButton.Id = CompleteButtonId;
            completeButton.Click += CompleteBtn_Click;

            proceedButton = CreateProceedBtn();
            proceedButton.Id = ProceedId;
            proceedButton.Click += (proceedSender, e) => VerifyConfirmation(proceedSender, e, "proceed");

            if (report != null)
            {
                if (userID == OwnerID)
                {
                    if (report.ReportStatus == ReportStatus.ArchivedAsAccepted ||
                        report.ReportStatus == ReportStatus.Archived ||
                        report.ReportStatus == ReportStatus.SendToVerification)
                    {
                        completeButton.Enabled = false;
                    }
                    else
                    {
                        completeButton.Enabled = true;
                    }
                }
                else
                {
                    completeButton.Enabled = false;
                }
            }

            paramsCompleteAuditButton = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.WrapContent);
            paramsCompleteAuditButton.AddRule(LayoutRules.AlignParentBottom);

            ProceedButton = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.WrapContent);
            ProceedButton.AddRule(LayoutRules.AlignParentBottom);

            paramsVerifyButtons = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.WrapContent);
            paramsVerifyButtons.AddRule(LayoutRules.AlignParentBottom);

            paramsScroller = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent);
            paramsScroller.AddRule(LayoutRules.Above, proceedButton.Id);
            paramsScroller.AddRule(LayoutRules.Above, verifyButtonsHolder.Id);
            paramsScroller.AddRule(LayoutRules.Above, completeButton.Id);
            scroller.LayoutParameters = paramsScroller;

            completeButton.LayoutParameters = paramsVerifyButtons;
            proceedButton.LayoutParameters = ProceedButton;
            verifyButtonsHolder.LayoutParameters = paramsCompleteAuditButton;
        }

        private LinearLayout CreateVerifyButtons()
        {
            var verifyButtons = new LinearLayout(this);
            verifyButtons.SetPadding(20, 20, 20, 20);
            verifyButtons.Orientation = Orientation.Horizontal;
            var paramsForVerifyButtons = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            verifyButtons.LayoutParameters = paramsForVerifyButtons;

            declineButton = new Button(this) {Text = "Decline"};
            declineButton.SetTextColor(Color.White);
            declineButton.SetWidth(250);
            declineButton.SetBackgroundColor(Color.ParseColor("#fc6042"));
            declineButton.SetPadding(32, 5, 50, 5);
            var paramsForDeclineButton = new TableLayout.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.WrapContent, 1f);
            paramsForDeclineButton.SetMargins(5, 5, 5, 5);
            declineButton.LayoutParameters = paramsForDeclineButton;
            declineButton.Click += (sender7, e) => VerifyConfirmation(sender7, e, "decline");

            acceptButton = new Button(this) {Text = "Accept"};
            acceptButton.SetTextColor(Color.White);
            acceptButton.SetWidth(200);
            acceptButton.SetBackgroundColor(Color.ParseColor("#23c064"));
            declineButton.SetPadding(50, 5, 32, 5);
            var paramsForAcceptButton = new TableLayout.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.WrapContent, 1f);
            paramsForAcceptButton.SetMargins(5, 5, 5, 5);
            acceptButton.LayoutParameters = paramsForAcceptButton;
            acceptButton.Click += (sender8, e) => VerifyConfirmation(sender8, e, "accept");

            verifyButtons.AddView(declineButton);
            verifyButtons.AddView(acceptButton);

            return verifyButtons;
        }

        private Button CreateCompleteAuditBtn()
        {
            var completeBtn = new Button(this) {Text = GetString(Resource.String.CompleteAudit)};
            completeBtn.SetTextColor(Color.ParseColor(GetString(Resource.Color.green_primary)));
            completeBtn.SetPadding(15, 15, 15, 20);
            return completeBtn;
        }

        private void CompleteBtn_Click(object sender, EventArgs e)
        {
            var view = LayoutInflater.Inflate(Resource.Layout.alert_dialog_complete_report, null);
            var alert = new AlertDialog.Builder(this).Create();
            alert.SetView(view);
            alert.SetTitle("Audit Confirmation");
            alert.SetCanceledOnTouchOutside(false);

            var buttonSendToVerification = view.FindViewById<Button>(Resource.Id.btnSendToVerification);
            buttonSendToVerification.Click += delegate
            {
                AssignVerifier();
                alert.Dismiss();
            };

            var btnSave = view.FindViewById<Button>(Resource.Id.btnSave);
            btnSave.Click += delegate
            {
                SaveForm(false, ReportStatus.Modified);
                alert.Dismiss();
            };

            var buttonSaveArchiveReport = view.FindViewById<Button>(Resource.Id.btnSaveArchiveReport);
            buttonSaveArchiveReport.Click += delegate
            {
                SaveForm(true, ReportStatus.Archived);
                Finish();
                alert.Dismiss();
            };

            alert.Show();
        }

        private Button CreateProceedBtn()
        {
            var proceedBtn = new Button(this) {Text = "Confirmation Details"};
            proceedBtn.SetTextColor(Color.ParseColor(GetString(Resource.Color.light_blue)));
            proceedBtn.SetPadding(15, 15, 15, 20);
            return proceedBtn;
        }

        private void VerifyConfirmation(object sender, EventArgs e, string type)
        {
            var bundle = new Bundle();
            bundle.PutString("type", type);

            var json = JsonConvert.SerializeObject(report);
            bundle.PutString("JasonObject", json);

            var fragmentTransaction = FragmentManager.BeginTransaction();
            var verifyDialogFragment = new VerifyReportFragment {Arguments = bundle};

            verifyDialogFragment.Show(fragmentTransaction, GetString(Resource.String.DialogFragment));
        }

        private void AssignVerifier()
        {
            if (userID == OwnerID)
            {
                if (report != null)
                {
                    if (report.ReportStatus == ReportStatus.SendToVerification)
                    {
                        Utility.DisplayToast(this, "Report is Already sent for Verification");
                    }
                    else if (report.ReportStatus != ReportStatus.Archived &&
                             report.ReportStatus != ReportStatus.ArchivedAsAccepted)
                    {
                        NavigateToVerifyUserActivity("Verify");
                    }
                }
                else
                {
                    NavigateToVerifyUserActivity("Verify");
                }
            }
            else
            {
                Utility.DisplayToast(this, "Sorry you don't have the permission to assign a Verifier");
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            switch (resultCode)
            {
                case Result.Ok:
                    {
                        switch (requestCode)
                        {
                            case 1:
                                {
                                    if (data.GetStringExtra(Resources.GetString(Resource.String.assign_user_list_type)) == Resources.GetString(Resource.String.add_collaborators))
                                    {
                                        assignedUserIDs = GetIntArray(data.GetIntegerArrayListExtra(Resources.GetString(Resource.String.assign_user_id_list)));
                                        if (assignedUserIDs.Count > 0)
                                        {
                                            SaveForm(false, ReportStatus.Shared);
                                            Utility.DisplayToast(this, "Report has been shared successfully");
                                        }
                                    }
                                    else if (data.GetStringExtra(Resources.GetString(Resource.String.assign_user_list_type)) == Resources.GetString(Resource.String.verify))
                                    {
                                        verifierID = GetIntArray(data.GetIntegerArrayListExtra(Resources.GetString(Resource.String.assign_user_id_list)))[0];
                                        SaveForm(false, ReportStatus.SendToVerification);
                                        Utility.DisplayToast(this, "Report has been sent successfully");
                                    }
                                    else if (data.GetStringExtra(Resources.GetString(Resource.String.assign_user_list_type)) == Resources.GetString(Resource.String.change_ownership))
                                    {
                                        OwnerID = GetIntArray(data.GetIntegerArrayListExtra(Resources.GetString(Resource.String.assign_user_id_list)))[0];
                                        SaveForm(false, ReportStatus.HandOver);
                                        Utility.DisplayToast(this, "Report has been handed over successfully");
                                    }
                                    break;
                                }
                        }
                    }
                    break;
            }

        }

        private static List<int> GetIntArray(IEnumerable<Integer> getIntegerArrayListExtra)
        {
            return getIntegerArrayListExtra.Select(integer => integer.IntValue()).ToList();
        }

        private async void SaveForm(bool syncState, ReportStatus status)
        {
            var progress = new ProgressDialog(this) {Indeterminate = true};
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.SetMessage(Resources.GetString(Resource.String.LoadingMessage));
            progress.SetCancelable(false);
            progress.Show();

            reportDataWithData = new ReportDataDto {StatusTimeLines = new List<StatusTimeLineDto>()};

            if (verifierID == 0)
            {
                reportDataWithData.VerifierUserId = null;
            }
            else
            {
                reportDataWithData.VerifierUserId = verifierID;
            }

            var statusObject = TimeLineObjectCreator(status, assignedUserIDs);
            reportDataWithData.AssignUsers = new List<int>();
            reportDataWithData.StatusTimeLines.Add(statusObject);

            var reportElementsDetailList = new List<ReportElement>();
            var reportElementsHeaderList = new List<ReportElement>();

            foreach (var element in informationElementList)
            {
                SetFormElementData(element, dynamiclayout, "form");
                if (element.Options != null)
                {
                    var description = GetDescription(element);
                    if (!string.IsNullOrEmpty(description))
                    {
                        reportDataWithData.Description = description;
                    }
                }
                reportElementsDetailList.Add(element);
            }

            foreach (var element in headerElementList)
            {
                SetFormElementData(element, dynamiclayoutHeader, "header");
                if (element.Options != null)
                {
                    var description = GetDescription(element);
                    if (!string.IsNullOrEmpty(description))
                    {
                        reportDataWithData.Description = description;
                    }
                }
                reportElementsHeaderList.Add(element);
            }

            switch (formType)
            {
                case "template":
                    reportDataWithData.ReportId = template.ReportId;
                    reportDataWithData.ReportName = template.ReportName;
                    reportDataWithData.ReportDataId = 0;
                    reportDataWithData.Number = 0;
                    reportDataWithData.OwnerUserId = OwnerID;
                    reportDataWithData.CreatedUserId = userID;
                    reportDataWithData.ItemDtos = new List<ItemDto>();
                    break;
                case "draft":
                case "archive":
                    reportDataWithData.ReportId = report.ReportId;
                    reportDataWithData.ReportName = report.ReportName;
                    reportDataWithData.ReportDataId = reportID;
                    reportDataWithData.Number = report.Number;
                    reportDataWithData.OwnerUserId = report.OwnerUserId;
                    reportDataWithData.CreatedUserId = report.CreatedUserId;
                    reportDataWithData.ItemDtos = report.ItemDtos ?? new List<ItemDto>();
                    break;
            }

            reportDataWithData.DetailTemplateData = JsonConvert.SerializeObject(reportElementsDetailList,
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            reportDataWithData.HeaderTemplateData = JsonConvert.SerializeObject(reportElementsHeaderList,
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            reportDataWithData.IsArchived = syncState;
            reportDataWithData.ProjectId = projectID;
            reportDataWithData.ModifiedUserId = userID;
            reportDataWithData.CreatedDateTime = DateTime.Now;
            reportDataWithData.ModifiedDateTime = DateTime.Now;
            reportDataWithData.NoOfElements = headerElementList.Count + informationElementList.Count;
            reportDataWithData.NoOfFilledElements = headerElementList.Count(x => !string.IsNullOrEmpty(x.Value)) +
                                                informationElementList.Count(x => !string.IsNullOrEmpty(x.Value));

            reportDataWithData.NotSynced = false;

            switch (status)
            {
                case ReportStatus.Modified:
                    if (statusReport == ReportStatus.Rejected)
                    {
                        reportDataWithData.VerifierUserId = null;
                    }
                    reportDataWithData.ReportStatus = ReportStatus.InProgress;
                    break;
                case ReportStatus.Shared:
                    reportDataWithData.ReportStatus = statusReport;
                    break;
                default:
                    reportDataWithData.ReportStatus = status;
                    break;
            }

            try
            {
                reportService = new ReportService(userSession.AccessToken);
                await reportService.PostReport(reportDataWithData);
            }
            catch (CustomHttpResponseException customHttpResponseException)
            {
                if (customHttpResponseException.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Log.Error("Authentication Exception - FormActivity", customHttpResponseException.ToString());
                    Finish();
                    StartActivity(typeof(LoginActivity));
                }
                else
                {
                    Log.Error(customHttpResponseException.StatusCode + " - FormActivity", customHttpResponseException.ToString());
                    Utility.DisplayToast(this, "Error : " + customHttpResponseException.StatusCode + "\nAn error occurred while connecting to the application server");
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Delete Auto Save Back Save", ex.ToString());
            }

            if (syncState)
            {
                Utility.DisplayToast(this, "Report Saved and Archived");
            }
            else if (status == ReportStatus.Modified)
            {
                switch (formType)
                {
                    case "draft":
                        Utility.DisplayToast(this, "Draft Updated");
                        break;
                    default:
                        Utility.DisplayToast(this, "Report Saved to Draft");
                        break;
                }
            }
            
            progress.Dismiss();
            Finish();
        }

        public override void OnBackPressed()
        {
            Finish();
        }

        private void NavigateToVerifyUserActivity(string type)
        {
            if (Utility.IsInternetAvailable(this))
            {
                var userListActivity = new Intent(this, typeof(UserListActivity));

                switch (type)
                {
                    case "Verify":
                        sharedPreferencesEditor.PutInt("VerifiedUser", verifierID);
                        sharedPreferencesEditor.PutBoolean("FromVerifiedUser", true);
                        userListActivity.PutExtra(Resources.GetString(Resource.String.assign_user_list_type), Resources.GetString(Resource.String.verify));
                        break;
                    case "Owner":
                        sharedPreferencesEditor.PutInt("OwnerID", OwnerID);
                        sharedPreferencesEditor.PutBoolean("FromOwnerUser", true);
                        userListActivity.PutExtra(Resources.GetString(Resource.String.assign_user_list_type), Resources.GetString(Resource.String.change_ownership));
                        break;
                }

                sharedPreferencesEditor.Commit();
                if (report != null)
                {
                    userListActivity.PutExtra(Resources.GetString(Resource.String.report_number),
                        report.Number);
                    userListActivity.PutExtra(Resources.GetString(Resource.String.report_name),
                        report.ReportName);
                    userListActivity.PutIntegerArrayListExtra(Resources.GetString(Resource.String.assign_user_id_list),
                        GetIntegerArray(report.AssignUsers));
                }
                StartActivityForResult(userListActivity, 1);

            }
            else
            {
                Utility.DisplayToast(this, Resources.GetString(Resource.String.no_internet_message));
            }
        }

        private StatusTimeLineDto TimeLineObjectCreator(ReportStatus status, List<int> assignedUserIDs)
        {
            var timelineObject = new StatusTimeLineDto {CreatedDateTime = DateTime.Now, Note = ""};
            if (report == null)
            {
                timelineObject.ReportDataId = 0;
            }
            else
            {
                timelineObject.ReportDataId = report.ReportDataId;
            }

            timelineObject.ReportStatus = status;
            timelineObject.SentByUserId = userID;

            switch (status)
            {
                case ReportStatus.Shared:
                    timelineObject.SentToUserId = assignedUserIDs[assignedUserIDs.Count - 1];
                    foreach (var assignedUserID in assignedUserIDs)
                    {
                        timelineObject.Note += assignedUserID;
                        if (assignedUserID != timelineObject.SentToUserId)
                        {
                            timelineObject.Note += ",";
                        }
                    }
                    break;
                case ReportStatus.HandOver:
                    timelineObject.SentToUserId = OwnerID;
                    if (userID != OwnerID)
                    {
                        assignedUserIDs.Add(userID);
                    }
                    break;
                case ReportStatus.SendToVerification:
                    timelineObject.SentToUserId = verifierID;
                    break;
            }

            timelineObject.Signature = "";
            return timelineObject;
        }

        private void SetFormElementData(ReportElement element, View layout, string section)
        {
            var type = element.Type;
            var vID = element.Id;
            var vName = element.Title;

            if (type == "checkbox" || type == "dropdown" || type == "mainandsubfield" || type == "camera" || type == "gps" || type == "tabularform")
            {
                if (type == "mainandsubfield")
                {
                    var infoDropdownElement = layout.FindViewById<FormHeaderSubElement>(vID);
                    if (infoDropdownElement != null)
                    {
                        element.Values = infoDropdownElement.HeaderSubValues();
                    }
                }

                if (type == "dropdown")
                {
                    var headerDropdownElement = layout.FindViewById<FormDropDown>(vID);
                    if (headerDropdownElement != null)
                    {
                        element.Values = headerDropdownElement.DropDownBoxValues();
                    }
                }
                if (type == "checkbox")
                {
                    var headerCheckBoxElement = layout.FindViewById<FormCheckBox>(vID);
                    if (headerCheckBoxElement != null)
                    {
                        element.Values = headerCheckBoxElement.CheckBoxValues();
                    }

                    if (element.IsMultiSelect)
                    {
                        var checkSum = 0;
                        for (var j = 0; j < element.Values.Count; j++)
                        {
                            if (element.Values[j].Value.Equals("true") && element.Values[j].Condition)
                            {
                                for (var k = 0; k < element.Values[j].Child[0].Count; k++)
                                {
                                    SetFormElementData(element.Values[j].Child[0][k], layout, section);
                                }
                                checkSum += element.Values[j].Child[0].Count;
                            }
                        }
                    }
                    else
                    {
                        for (var j = 0; j < element.Values.Count; j++)
                        {
                            if (element.Values[j].Value.Equals("true") && element.Values[j].Condition)
                            {
                                for (var k = 0; k < element.Values[j].Child[0].Count; k++)
                                {
                                    SetFormElementData(element.Values[j].Child[0][k], layout, section);
                                }
                            }
                        }
                    }
                }

                if (type == "gps")
                {
                    var headerGpsElement = layout.FindViewById<FormGPS>(vID);
                    if (headerGpsElement != null)
                    {
                        if (longtit != null && latit != null)
                        {
                            headerGpsElement.setGeoCoOrdinates(longtit, latit);
                        }
                        element.Values = headerGpsElement.getGPSData();
                    }
                }

                if (type == "camera")
                {
                    List<CustomGallery> cusGal = new List<CustomGallery>();
                    foreach (var grid in FormCamera.cameraPreviewView)
                    {
                        if (grid.Id == vID + (section.Equals("header") ? IMAGE_PREVIEW_Header_ID : IMAGE_PREVIEW_Info_ID))
                        {
                            MiniGallerAdapter adapterheader = (MiniGallerAdapter)grid.Adapter;
                            cusGal = adapterheader.getList();
                        }
                    }

                    List<KeyValue> imageAddress = new List<KeyValue>();
                    foreach (var item in cusGal)
                    {
                        // imageValues.Value = item.SdCardPath;
                        KeyValue imageValues = new KeyValue();
                        imageValues.Value = item.SdCardPath.Substring(item.SdCardPath.LastIndexOf("/") + 1);
                        imageAddress.Add(imageValues);
                    }
                    element.Values = imageAddress;
                }
            }
            else
            {
                element.Value = GetProcessedDataFromForm(type, vID, section);

                if (type.Equals("yesno") && element.Options != null)
                {
                    var conditionalCode = element.Options.FirstOrDefault(a => a.Code == "conditional");
                    if (conditionalCode != null && Convert.ToBoolean(conditionalCode.Value))
                    {
                        if (element.Value.Equals("true"))
                        {
                            for (var j = 0; j < element.Accepted[0].Count; j++)
                            {
                                SetFormElementData(element.Accepted[0][j], layout, section);
                            }
                        }
                        else
                        {
                            for (var j = 0; j < element.Rejected[0].Count; j++)
                            {
                                SetFormElementData(element.Rejected[0][j], layout, section);
                            }
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(element.Value))
            {
                if (string.IsNullOrEmpty(element.FilledBy))
                {
                    element.FilledBy = userID + "";
                }
            }
        }

        private static string GetDescription(ReportElement element)
        {
            var isDescription = element.Options?.FirstOrDefault(a => a.Code == "isdescription");
            if (isDescription != null &&
                ((element.Type.Equals("textfield") || element.Type.Equals("multilinetextfield")) &&
                 Convert.ToBoolean(isDescription.Value)))
            {
                return element.Value;
            }
            return "";
        }

        private static List<Integer> GetIntegerArray(IEnumerable<int> getIntegerArrayListExtra)
        {
            return getIntegerArrayListExtra.Select(item => (Integer) item).ToList();
        }

        private string GetProcessedDataFromForm(string type, int id, string section)
        {
            var results = "";

            switch (type)
            {
                case "Button":
                    results = "";
                    break;

                case "slider":
                    var et2 = new SeekBar(this);
                    et2 = section == "form"
                        ? dynamiclayout.FindViewById<SeekBar>(id)
                        : dynamiclayoutHeader.FindViewById<SeekBar>(id);
                    switch (et2.Progress)
                    {
                        case 0:
                            results = "";
                            break;
                        default:
                            results = et2.Progress + "";
                            break;
                    }
                    break;

                case "textfieldint":
                    var et3 = new EditText(this);
                    et3 = section == "form"
                        ? dynamiclayout.FindViewById<EditText>(id)
                        : dynamiclayoutHeader.FindViewById<EditText>(id);
                    results = et3.Text;
                    break;

                case "textfield":
                    var et4 = new EditText(this);
                    et4 = section == "form"
                        ? dynamiclayout.FindViewById<EditText>(id)
                        : dynamiclayoutHeader.FindViewById<EditText>(id);
                    results = et4.Text;
                    break;

                case "multilinetextfield":
                    var et5 = new EditText(this);
                    et5 = section == "form"
                        ? dynamiclayout.FindViewById<EditText>(id)
                        : dynamiclayoutHeader.FindViewById<EditText>(id);
                    results = et5.Text;
                    break;

                case "yesno":
                    var et6 = new Switch(this);
                    et6 = section == "form"
                        ? dynamiclayout.FindViewById<Switch>(id)
                        : dynamiclayoutHeader.FindViewById<Switch>(id);
                    results = et6.Text != "Set" ? (et6.Checked ? "true" : "false") : "";
                    break;

                case "date":
                    var et7 = new EditText(this);
                    et7 = section == "form"
                        ? dynamiclayout.FindViewById<EditText>(id)
                        : dynamiclayoutHeader.FindViewById<EditText>(id);
                    var date = et7.Text;

                    try
                    {
                        var dt = Convert.ToDateTime(date);
                        var inputString = dt.ToString("dd.MMM.yyyy");
                        ;
                        DateTime dDate;

                        if (DateTime.TryParse(inputString, out dDate))
                        {
                            String.Format("{0:dd.MMM.yyyy}", dDate);
                            results = string.IsNullOrEmpty(date) ? "" : dt.ToString("dd.MMM.yyyy");
                        }
                        else
                        {
                            results = "";
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Debug("Date Bug", ex.ToString());
                        results = "";
                    }

                    break;

                case "time":
                    var et8 = new EditText(this);
                    et8 = section == "form"
                        ? dynamiclayout.FindViewById<EditText>(id)
                        : dynamiclayoutHeader.FindViewById<EditText>(id);
                    results = et8.Text;
                    break;

                case "datetime":
                    var et9 = new TextView(this);
                    et9 = section == "form"
                        ? dynamiclayout.FindViewById<TextView>(id)
                        : dynamiclayoutHeader.FindViewById<TextView>(id);
                    results = et9.Text;
                    break;

                case "signature":
                    var et10 = new ImageButton(this);
                    et10 = section == "form"
                        ? dynamiclayout.FindViewById<ImageButton>(id)
                        : dynamiclayoutHeader.FindViewById<ImageButton>(id);

                    if (et10.Tag != null)
                    {
                        results = et10.Tag.ToString();
                    }
                    else
                    {
                        results = "";
                    }
                    break;

                case "camera":
                    results = "";
                    break;

                case "checkbox":
                    results = "";
                    break;

                case "dropdown":
                    results = "";
                    break;
                case "mainandsubfield":
                    results = "";
                    break;
                case "updown":
                    try
                    {
                        var et12 = new EditText(this);
                        et12 = section == "form"
                        ? dynamiclayout.FindViewById<EditText>(id)
                        : dynamiclayoutHeader.FindViewById<EditText>(id);


                        switch (int.Parse(et12.Text))
                        {
                            case 0:
                                results = "";
                                break;
                            default:
                                results = et12.Text + "";
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Debug("UpDown crash", ex.ToString());
                    }
                    break;
            }
            return results;
        }
    }
}