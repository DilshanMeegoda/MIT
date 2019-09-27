using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using WorkFlowManagement.Activities;
using WorkFlowManagement.Adapters;
using WorkFlowManagement.Common;
using WorkFlowManagement.Model;
using WorkFlowManagement.Services;
using WorkFlowManagement.Services.Interfaces;
using glAndroid = Android;
using String = System.String;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WorkFlowManagement.Fragments
{
    public class ReportTempleteListDialogFragment : DialogFragment
    {
        private ITemplateService templateService;
        private ISharedPreferencesEditor sharedPreferencesEditor;
        private ListView _listReportTemplete;
        private FloatingActionButton _closeFloatingActionButton;
        private EditText _filterText;
        private List<Template> templateList;
        private int _projectId;
        private ImageView _emptyState;
        private UserSession userSession;
        private Toolbar _toolbar;
        private ProgressBar progressBar;
        private SwipeRefreshLayout swipeRefresh;
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            const int theme = glAndroid.Resource.Style.ThemeDeviceDefault;
            const DialogFragmentStyle style = (DialogFragmentStyle)glAndroid.Resource.Style.ThemeMaterialLight;
            SetStyle(style, theme);

            ISharedPreferences sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            sharedPreferencesEditor = sharedPreferences.Edit();

            var jsonString = sharedPreferences.GetString(Resources.GetString(Resource.String.user_session), string.Empty);
            userSession = JsonConvert.DeserializeObject<UserSession>(jsonString);

            var currentProjectJsonString = sharedPreferences.GetString(Resources.GetString(Resource.String.current_project), string.Empty);
            var currentProject = JsonConvert.DeserializeObject<Project>(currentProjectJsonString);

            _projectId = currentProject.ProjectId;

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.fragment_dialog_report_templete_list, container, false);

            _toolbar = view.FindViewById<Toolbar>(Resource.Id.my_toolbar);
            _toolbar.Title = Resources.GetString(Resource.String.fragDialgReprtTempListTitle);
            _toolbar.SetTitleTextColor(Color.ParseColor("#ffffff"));

            InitializeElements(view);

            return view;
        }

        private async void InitializeElements(View view)
        {
            view.Focusable = true;
            view.FocusableInTouchMode = true;

            progressBar = view.FindViewById<ProgressBar>(Resource.Id.projectTemplateListProgressBar);
            swipeRefresh = view.FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
            swipeRefresh.Refresh += async delegate
            {
                await LoadTemplateList();
                swipeRefresh.Refreshing = false;
            };
            _filterText = view.FindViewById<EditText>(Resource.Id.search);
            _filterText.SetTextColor(Color.Black);
            _filterText.TextChanged += FilterTextTextChanged;

            _closeFloatingActionButton = view.FindViewById<FloatingActionButton>(Resource.Id.fab);
            _closeFloatingActionButton.Click += CloseFloatingActionButtonClick;

            _listReportTemplete = view.FindViewById<ListView>(Resource.Id.list);
            _listReportTemplete.ItemClick += (sender, e) =>
            {
                sharedPreferencesEditor.PutInt(Resources.GetString(Resource.String.report_id), templateList[e.Position].ReportId);
                sharedPreferencesEditor.PutString(Resources.GetString(Resource.String.report_type), "template");
                sharedPreferencesEditor.PutString(Resources.GetString(Resource.String.report_name), templateList[e.Position].ReportName);
                sharedPreferencesEditor.Commit();
                Dialog.Dismiss();
                Activity.StartActivity(typeof(FormActivity));
            };

            _emptyState = view.FindViewById<ImageView>(Resource.Id.imageViewTemplatePlaceHolder);

            await LoadTemplateList();
        }

        private async Task LoadTemplateList()
        {
            progressBar.Visibility = ViewStates.Visible;
            if (Utility.IsInternetAvailable(Application.Context))
            {
                templateService = new TemplateService(userSession.AccessToken);
                templateList = (await templateService.GetTemplateList(_projectId)).OrderBy(x => x.ReportName).Where(y => y.IsStandard).ToList();
            }
            else
            {
                Utility.DisplayToast(Application.Context, Resources.GetString(Resource.String.NoInternet));
            }
            
            _listReportTemplete.Adapter = new ReportTemplateListAdapter(Application.Context, templateList);
            _emptyState.Visibility = templateList.Count == 0 ? ViewStates.Visible : ViewStates.Gone;
            progressBar.Visibility = ViewStates.Gone;
        }

        void FilterTextTextChanged(object sender, glAndroid.Text.TextChangedEventArgs e)
        {
            var searchTerm = _filterText.Text;

            if (String.IsNullOrEmpty(searchTerm))
            {
                ((ReportTemplateListAdapter)_listReportTemplete.Adapter).ResetSearch();
            }
            else
            {
                Search(searchTerm);
                ((ReportTemplateListAdapter)_listReportTemplete.Adapter).Filter.InvokeFilter(searchTerm);
            }
        }

        void CloseFloatingActionButtonClick(object sender, EventArgs e)
        {
            Dialog.Dismiss();
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            Dialog dialog = base.OnCreateDialog(savedInstanceState);
            dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            return dialog;
        }

        private void Search(string searchTerm)
        {
            templateList = templateList.Where(t => t.ReportName.ToLower().Contains(searchTerm.ToLower())).ToList();
        }

        private void NavigateToRequestReport()
        {
            //var chatIntent = new Intent(Application.Context, typeof(NewReportRequestActivity));
            //StartActivity(new Intent(chatIntent));
        }
    }
}