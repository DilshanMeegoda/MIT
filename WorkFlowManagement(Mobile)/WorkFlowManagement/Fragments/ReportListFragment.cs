using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Util;
using Newtonsoft.Json;
using WorkFlowManagement.Activities;
using WorkFlowManagement.Adapters;
using WorkFlowManagement.Common;
using WorkFlowManagement.Model;
using WorkFlowManagement.Services;
using WorkFlowManagement.Services.Interfaces;
using Fragment = Android.Support.V4.App.Fragment;

namespace WorkFlowManagement.Fragments
{
    public class ReportListFragment : Fragment
    {
        private Button allFilter;
        private ImageView emptyView;
        private string filterOption;
        private int projectID;
        private Button receivedFilter;
        private bool scrollTracker;
        private bool searchFlag;
        private TreeSet sectionHeader;
        private Button sentFilter;
        private ISharedPreferences sharedPreferences;
        private ISharedPreferencesEditor sharedPreferencesEditor;
        private SwipeRefreshLayout swipeRefresh;
        private int userID;
        private UserSession userSession;
        private View view;
        private IEnumerable<Report> unOrderedReportList;
        private IEnumerable<Report> orderedReportList;
        private List<Report> headerandItemsList;
        private ListView reportListView;
        private ReportListArchiveAdapter listAdapter;
        private IReportService reportService;

        public static ReportListFragment NewInstance()
        {
            return new ReportListFragment();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_report_list, container, false);
            InitializeViews(view);
            SetUpButtons();
            LoadData(filterOption, false);
            return view;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            searchFlag = false;
            RetainInstance = true;

            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            sharedPreferencesEditor = sharedPreferences.Edit();

            var currentProjectJsonString = sharedPreferences.GetString(Resources.GetString(Resource.String.current_project), string.Empty);
            var currentProject = JsonConvert.DeserializeObject<Project>(currentProjectJsonString);

            var jsonString = sharedPreferences.GetString(Resources.GetString(Resource.String.user_session), string.Empty);
            userSession = JsonConvert.DeserializeObject<UserSession>(jsonString);

            userID = userSession.UserId;
            projectID = currentProject.ProjectId;
            filterOption = sharedPreferences.GetString(Resources.GetString(Resource.String.report_filter_option),
                Resources.GetString(Resource.String.filter_option_all));
        }

        private void InitializeViews(View view)
        {
            scrollTracker = true;

            reportListView = view.FindViewById<ListView>(Resource.Id.reportlist);
            emptyView = view.FindViewById<ImageView>(Resource.Id.imageViewReportPlaceHolder);
            emptyView.Visibility = ViewStates.Gone;

            allFilter = view.FindViewById<Button>(Resource.Id.myReportAllButtton);
            receivedFilter = view.FindViewById<Button>(Resource.Id.myReportReceivedButton);
            sentFilter = view.FindViewById<Button>(Resource.Id.myReportSentButton);

            swipeRefresh = view.FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
            swipeRefresh.Refresh += async delegate
            {
                await LoadData(filterOption, false);
                swipeRefresh.Refreshing = false;
            };

            allFilter.Click += (proceedsender1, e) =>
                VerifyConfirmation(Resources.GetString(Resource.String.filter_option_all));
            receivedFilter.Click += (proceedsender2, e) =>
                VerifyConfirmation(Resources.GetString(Resource.String.filter_option_received));
            sentFilter.Click += (proceedsender3, e) =>
                VerifyConfirmation(Resources.GetString(Resource.String.filter_option_sent));
        }

        private async void VerifyConfirmation(string filterType)
        {
            sharedPreferencesEditor.PutString(Resources.GetString(Resource.String.report_filter_option), filterType);
            sharedPreferencesEditor.Commit();

            filterOption = filterType;

            SetUpButtons();

            await LoadData(filterOption, true);
        }

        private void SetUpButtons()
        {
            if (filterOption.Equals(Resources.GetString(Resource.String.filter_option_all)))
            {
                allFilter.SetBackgroundResource(Resource.Drawable.button_active);
                receivedFilter.SetBackgroundResource(Resource.Drawable.button_inactive);
                sentFilter.SetBackgroundResource(Resource.Drawable.button_inactive);

                allFilter.SetTextColor(Color.ParseColor("#ffffff"));
                receivedFilter.SetTextColor(Color.ParseColor("#27AE60"));
                sentFilter.SetTextColor(Color.ParseColor("#27AE60"));
            }
            else if (filterOption.Equals(Resources.GetString(Resource.String.filter_option_received)))
            {
                allFilter.SetBackgroundResource(Resource.Drawable.button_inactive);
                receivedFilter.SetBackgroundResource(Resource.Drawable.button_active);
                sentFilter.SetBackgroundResource(Resource.Drawable.button_inactive);

                allFilter.SetTextColor(Color.ParseColor("#27AE60"));
                receivedFilter.SetTextColor(Color.ParseColor("#ffffff"));
                sentFilter.SetTextColor(Color.ParseColor("#27AE60"));
            }
            else if (filterOption.Equals(Resources.GetString(Resource.String.filter_option_sent)))
            {
                allFilter.SetBackgroundResource(Resource.Drawable.button_inactive);
                receivedFilter.SetBackgroundResource(Resource.Drawable.button_inactive);
                sentFilter.SetBackgroundResource(Resource.Drawable.button_active);

                allFilter.SetTextColor(Color.ParseColor("#27AE60"));
                receivedFilter.SetTextColor(Color.ParseColor("#27AE60"));
                sentFilter.SetTextColor(Color.ParseColor("#ffffff"));
            }
        }

        private async Task LoadData(string filterType, bool isFilter)
        {
            reportListView.ItemClick -= ReportList_ItemClick;
            if (projectID != 0 )
            {
                if (Utility.IsInternetAvailable(Application.Context))
                {
                    var dialog = new CustomDialogFragment();
                    dialog.Show(FragmentManager, "dialog");
                    try
                    {
                        if (!isFilter)
                        {
                            reportService = new ReportService(userSession.AccessToken);
                            unOrderedReportList = await reportService.GetReportList(userSession.UserId, projectID);
                            orderedReportList = unOrderedReportList.OrderByDescending(a => a.CreatedDateTime.Date)
                                .ThenByDescending(a => a.CreatedDateTime.TimeOfDay);
                            orderedReportList = FilterOptionList(orderedReportList, filterType);
                        }
                        else
                        {
                            orderedReportList = FilterOptionList(unOrderedReportList, filterType);
                        }

                        var reportList = orderedReportList.ToList();
                        emptyView.Visibility = reportList.Count > 0 ? ViewStates.Gone : ViewStates.Visible;

                        headerandItemsList = new List<Report>();
                        sectionHeader = new TreeSet();

                        var todayheader = false;
                        var yesterday = false;
                        var lastweek = false;
                        var other = false;

                        for (var i = 0; i < reportList.Count; i++)
                            if (reportList[i].CreatedDateTime.Date >= DateTime.Today)
                            {
                                if (todayheader == false)
                                {
                                    todayheader = true;
                                    Report temp = new Report();
                                    temp.ReportName = Resources.GetString(Resource.String.today);
                                    headerandItemsList.Add(temp);
                                    sectionHeader.Add(headerandItemsList.Count - 1);
                                }

                                headerandItemsList.Add(reportList[i]);
                            }
                            else if (reportList[i].CreatedDateTime.Date == DateTime.Today.AddDays(-1))
                            {
                                if (!yesterday)
                                {
                                    yesterday = true;
                                    Report temp2 = new Report();
                                    temp2.ReportName = Resources.GetString(Resource.String.yesterday);
                                    headerandItemsList.Add(temp2);
                                    sectionHeader.Add(headerandItemsList.Count - 1);
                                }

                                headerandItemsList.Add(reportList[i]);
                            }
                            else if (reportList[i].CreatedDateTime.Date < DateTime.Today.AddDays(-2) &&
                                     reportList[i].CreatedDateTime.Date > DateTime.Today.AddDays(-7))
                            {
                                if (!lastweek)
                                {
                                    lastweek = true;
                                    Report temp3 = new Report();
                                    temp3.ReportName = Resources.GetString(Resource.String.last_week);
                                    headerandItemsList.Add(temp3);
                                    sectionHeader.Add(headerandItemsList.Count - 1);
                                }

                                headerandItemsList.Add(reportList[i]);
                            }
                            else
                            {
                                if (!other)
                                {
                                    other = true;
                                    Report temp3 = new Report();
                                    temp3.ReportName = Resources.GetString(Resource.String.other);
                                    headerandItemsList.Add(temp3);
                                    sectionHeader.Add(headerandItemsList.Count - 1);
                                }

                                headerandItemsList.Add(reportList[i]);
                            }
                        
                        listAdapter = new ReportListArchiveAdapter(Application.Context, headerandItemsList, sectionHeader);
                        reportListView.Adapter = listAdapter;
                        reportListView.ItemClick += ReportList_ItemClick;
                        listAdapter.NotifyDataSetChanged();
                        dialog.Dismiss();
                    }
                    catch (Exception ex)
                    {
                        Log.Debug("Exception", ex.ToString());
                    }
                }
                else
                {
                    Toast.MakeText(Application.Context, Resources.GetString(Resource.String.NoInternet),
                        ToastLength.Short).Show();
                }
            }
            else
            {
                emptyView.Visibility = ViewStates.Visible;
            }
        }

        private void ReportList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (listAdapter.GetItemViewType(e.Position) != 1)
            {
                var reportDataData = headerandItemsList[e.Position];
                    if (Utility.IsInternetAvailable(Application.Context))
                    {
                        sharedPreferencesEditor.PutInt(Resources.GetString(Resource.String.report_id), reportDataData.ReportDataId);
                        sharedPreferencesEditor.PutString(Resources.GetString(Resource.String.report_name), reportDataData.ReportName);
                        sharedPreferencesEditor.PutString(Resources.GetString(Resource.String.report_type), !reportDataData.IsArchived ? "draft" : "archive");
                        sharedPreferencesEditor.Commit();

                        Activity.StartActivity(typeof(FormActivity));
                    }
                    else
                    {
                        Utility.DisplayToast(Application.Context, Application.Context.Resources.GetString(Resource.String.internettoreadreport));
                    }
            }
        }

        private IEnumerable<Report> FilterOptionList(IEnumerable<Report> incomingList, string filterOption)
        {
            var result = new List<Report>();
            var enumerable = incomingList.ToList();
            var tempList = enumerable.ToList();

            if (filterOption.Equals(Resources.GetString(Resource.String.filter_option_all)))
            {
                result = tempList;
            }
            else if (filterOption.Equals(Resources.GetString(Resource.String.filter_option_received)))
            {
                for (var i = 0; i < enumerable.Count; i++)
                {
                    if (!tempList[i].CreatedUserId.Equals(userID))
                    {
                        result.Add(tempList[i]);
                    }
                }
            }
            else if (filterOption.Equals(Resources.GetString(Resource.String.filter_option_sent)))
            {
                for (var i = 0; i < enumerable.Count; i++)
                {
                    if (tempList[i].CreatedUserId.Equals(userID) && tempList[i].AssignUsers.Count > 0)
                    {
                        result.Add(tempList[i]);
                    }
                }
            }

            return result;
        }
    }
}