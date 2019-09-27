using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
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
using ListFragment = Android.Support.V4.App.ListFragment;

namespace WorkFlowManagement.Fragments
{
    public class ArchivedListFragment : ListFragment
    {
        private ISharedPreferences _sharedPreferences;
        private ISharedPreferencesEditor sharedPreferencesEditor;
        private IReportService reportService;
        private int projectId;
        private ImageView emptyView;
        //private ListView archivedlistView;
        private List<Report> headerandItemsList;
        private ReportListArchiveAdapter listAdapter;
        private UserSession userSession;
        private SwipeRefreshLayout swipeRefresh;

        public static ArchivedListFragment NewInstance()
        {
            return new ArchivedListFragment();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_archived_list, container, false);
            initializeViews(view);
            return view;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RetainInstance = true;
            _sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            sharedPreferencesEditor = _sharedPreferences.Edit();

            var currentProjectJsonString = _sharedPreferences.GetString(Resources.GetString(Resource.String.current_project), string.Empty);
            var currentProject = JsonConvert.DeserializeObject<Project>(currentProjectJsonString);

            var jsonString = _sharedPreferences.GetString(Resources.GetString(Resource.String.user_session), string.Empty);
            userSession = JsonConvert.DeserializeObject<UserSession>(jsonString);

            projectId = currentProject.ProjectId;
        }

        private void initializeViews(View view)
        {
            swipeRefresh = view.FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
            swipeRefresh.Refresh += delegate
            {
                LoadData();
                swipeRefresh.Refreshing = false;
            };

            emptyView = view.FindViewById<ImageView>(Resource.Id.imageViewArchivePlaceHolder);
            emptyView.Visibility = ViewStates.Gone;
        }

        private async void LoadData()
        {
            if (projectId != 0)
            {
                if (Utility.IsInternetAvailable(Application.Context))
                {
                    try
                    {
                        reportService = new ReportService(userSession.AccessToken);
                        var unOrderedReportList = await reportService.GetArchivedReportList(projectId);

                        var listBeforFilter = unOrderedReportList.Where(x => x.IsArchived).OrderByDescending(a => a.CreatedDateTime.Date).ThenByDescending(a => a.CreatedDateTime.TimeOfDay);
                        var resultList = listBeforFilter.ToList();

                        emptyView.Visibility = resultList.Count == 0 ? ViewStates.Visible : ViewStates.Gone;

                        headerandItemsList = new List<Report>();
                        TreeSet sectionHeaders = new TreeSet();

                        var todayheader = false;
                        var yesterday = false;
                        var lastweek = false;
                        var other = false;

                        for (int i = 0; i < resultList.Count; i++)
                        {
                            if (resultList[i].CreatedDateTime.Date >= DateTime.Today)
                            {
                                if (todayheader == false)
                                {
                                    todayheader = true;
                                    Report reportDataDataDto = new Report();
                                    reportDataDataDto.ReportName = Application.Context.Resources.GetString(Resource.String.today);
                                    headerandItemsList.Add(reportDataDataDto);
                                    sectionHeaders.Add(headerandItemsList.Count - 1);
                                }

                                headerandItemsList.Add(resultList[i]);
                            }
                            else if (resultList[i].CreatedDateTime.Date == DateTime.Today.AddDays(-1))
                            {
                                if (!yesterday)
                                {
                                    yesterday = true;
                                    Report reportDataDataDto = new Report();
                                    reportDataDataDto.ReportName = Application.Context.Resources.GetString(Resource.String.yesterday);
                                    headerandItemsList.Add(reportDataDataDto);
                                    sectionHeaders.Add(headerandItemsList.Count - 1);
                                }
                                headerandItemsList.Add(resultList[i]);
                            }


                            else if (resultList[i].CreatedDateTime.Date < DateTime.Today.AddDays(-2) && resultList[i].CreatedDateTime.Date > DateTime.Today.AddDays(-7))
                            {
                                if (!lastweek)
                                {
                                    lastweek = true;
                                    Report reportDataDataDto = new Report();
                                    reportDataDataDto.ReportName = Application.Context.Resources.GetString(Resource.String.last_week);
                                    headerandItemsList.Add(reportDataDataDto);
                                    sectionHeaders.Add(headerandItemsList.Count - 1);
                                }
                                headerandItemsList.Add(resultList[i]);
                            }
                            else
                            {
                                if (!other)
                                {
                                    other = true;
                                    Report reportDataDataDto = new Report();
                                    reportDataDataDto.ReportName = Application.Context.Resources.GetString(Resource.String.other);
                                    headerandItemsList.Add(reportDataDataDto);
                                    sectionHeaders.Add(headerandItemsList.Count - 1);
                                }

                                headerandItemsList.Add(resultList[i]);
                            }
                        }

                        listAdapter = new ReportListArchiveAdapter(Application.Context, headerandItemsList, sectionHeaders);
                        ListView.Adapter = listAdapter;
                        listAdapter.NotifyDataSetChanged();
                    }

                    catch (Exception)
                    {
                        Log.Debug("Exception", "Archeive List");
                    }
                }
            }
            else
            {
                emptyView.Visibility = ViewStates.Visible;
            }
        }

        public override void OnListItemClick(ListView l, View v, int position, long id)
        {
            if (listAdapter.GetItemViewType(position) != 1)
            {
                if (Utility.IsInternetAvailable(Application.Context))
                {
                    Report reportDataX = headerandItemsList[position];
                    int idReportData = reportDataX.ReportDataId;

                    sharedPreferencesEditor.PutInt("ReportID", idReportData);
                    sharedPreferencesEditor.PutString("ReportType", "archive");
                    sharedPreferencesEditor.PutString("ReportName", headerandItemsList[position].ReportName);
                    sharedPreferencesEditor.Commit();

                    Activity.StartActivity(typeof(FormActivity));
                }
                else
                {
                    Toast.MakeText(Application.Context, Resources.GetString(Resource.String.internettoreadreport), ToastLength.Long).Show();
                }
            }
        }
    }
}