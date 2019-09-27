using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using WorkFlowManagement.Adapters;
using WorkFlowManagement.Common;
using WorkFlowManagement.Model;
using WorkFlowManagement.Services;
using WorkFlowManagement.Services.Interfaces;
using SearchView = Android.Support.V7.Widget.SearchView;

namespace WorkFlowManagement.Activities
{
    [Activity(Label = "Project List", ScreenOrientation = ScreenOrientation.Portrait)]
    public class ProjectNameListActivity : AppCompatActivity
    {
        private ISharedPreferences sharedPreferences;
        private ISharedPreferencesEditor sharedPreferencesEditor;
        private IProjectService projectService;
        private UserSession userSession;
        private ProjectNameListAdapter projectNameListAdapter;
        private ArrayAdapter aAdapter;
        private FloatingActionButton FabAddProjectActionButton;
        private List<Project> listFromServer;
        private Drawable upArrow;
        private int userID;
        private ListView listProjectName;
        private ProgressBar progressBar;
        private SwipeRefreshLayout swipeRefresh;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.activity_project_name_list);

            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);
            sharedPreferencesEditor = sharedPreferences.Edit();

            var jsonString = sharedPreferences.GetString(Resources.GetString(Resource.String.user_session), string.Empty);
            userSession = JsonConvert.DeserializeObject<UserSession>(jsonString);

            progressBar = FindViewById<ProgressBar>(Resource.Id.projectListProgressBar);
            swipeRefresh = FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
            swipeRefresh.Refresh += delegate
            {
                LoadProjectsAsync();
                swipeRefresh.Refreshing = false;
            };
            listProjectName = FindViewById<ListView>(Resource.Id.listProjectName);
            listProjectName.ItemClick += (sender, e) =>
            {
                sharedPreferencesEditor.PutString(Resources.GetString(Resource.String.current_project), JsonConvert.SerializeObject(listFromServer[e.Position]));
                sharedPreferencesEditor.Commit();
                var mainActivity = new Intent(this, typeof(MainActivity));
                SetResult(Result.Ok, mainActivity);
                Finish();
            };

            LoadProjectsAsync();
        }

        private async void LoadProjectsAsync()
        {
            progressBar.Visibility = ViewStates.Visible;

            if (Utility.IsInternetAvailable(this))
            {
                projectService = new ProjectService(userSession.AccessToken);
                listFromServer = (await projectService.GetProjectList(userSession.UserId)).ToList();
            }
            else
            {
                Utility.DisplayToast(this, Resources.GetString(Resource.String.NoInternet));
            }

            projectNameListAdapter = new ProjectNameListAdapter(Application.Context, listFromServer);
            listProjectName.Adapter = projectNameListAdapter;
            progressBar.Visibility = ViewStates.Gone;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_project_name, menu);
            var menuItem = menu.FindItem(Resource.Id.action_project_search);
            var searchView = MenuItemCompat.GetActionView(menuItem);

            SearchView sView = searchView.JavaCast<SearchView>();
            sView.QueryTextChange += SearchQueryTextChange;

            return true;
        }

        void SearchQueryTextChange(object sender, SearchView.QueryTextChangeEventArgs e)
        {
            var matchingValues = listFromServer.Where(stringToCheck => stringToCheck.Title.Contains(e.NewText));
            projectNameListAdapter = new ProjectNameListAdapter(Application.Context, matchingValues.ToList());
            listProjectName.Adapter = projectNameListAdapter;
        }
    }
}