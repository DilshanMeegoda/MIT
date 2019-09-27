using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using WorkFlowManagement.Adapters;
using WorkFlowManagement.Fragments;
using WorkFlowManagement.Model;
using ActionBar = Android.Support.V7.App.ActionBar;
using ActionBarDrawerToggle = Android.Support.V4.App.ActionBarDrawerToggle;
using Environment = Android.OS.Environment;
using Path = System.IO.Path;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WorkFlowManagement.Activities
{
    [Activity(Label = "@string/app_name", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        private ISharedPreferences sharedPreferences;
        private UserSession userSession;
        private TabLayout tabLayout;
        private ViewPager pager;
        private TextView textCurrentProjectTitle;
        private MainPagerAdapter pagerAdapter;
        private string currentProjectName;
        private DrawerLayout mDrawerLayout;
        private ArrayAdapter mLeftDrawerAdapter;
        private ListView mLeftDrawer;
        private ActionBarDrawerToggle mDrawerToggle;
        private FloatingActionButton floatingActionButton;
        private Project currentProject;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);

            var userSessionJsonString = sharedPreferences.GetString(Resources.GetString(Resource.String.user_session), string.Empty);
            userSession = JsonConvert.DeserializeObject<UserSession>(userSessionJsonString);

            var currentProjectJsonString = sharedPreferences.GetString(Resources.GetString(Resource.String.current_project), string.Empty);
            currentProject = JsonConvert.DeserializeObject<Project>(currentProjectJsonString);

            if (currentProject != null)
            {
                currentProjectName = currentProject.Title;
            }

            if (userSession == null)
            {
                Finish();
                StartActivity(typeof(LoginActivity));
            }

            InitializeElements();
        }

        private void InitializeElements()
        {
            CreateActionBar();

            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.new_drawer_layout);
            mLeftDrawer = FindViewById<ListView>(Resource.Id.leftDrawerReport);

            var mleftDrawerItems = new List<string>();
            mleftDrawerItems.Add(Resources.GetString(Resource.String.logout));
            mDrawerToggle = new ActionBarDrawerToggle(this, mDrawerLayout, Resource.Drawable.menu_navbar_ham_new, Resource.String.open_drawer, Resource.String.close_drawer);
            mLeftDrawerAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, mleftDrawerItems);
            mLeftDrawer.Adapter = mLeftDrawerAdapter;
            mDrawerLayout.SetDrawerListener(mDrawerToggle);
            mLeftDrawer.ItemClick += ListView_ItemClick;

            pager = FindViewById<ViewPager>(Resource.Id.pager);
            pager.OffscreenPageLimit = 2;
            tabLayout = FindViewById<TabLayout>(Resource.Id.tabLayout);

            floatingActionButton = FindViewById<FloatingActionButton>(Resource.Id.fab);
            floatingActionButton.Click += FloatingActionButtonClick;

            InitializePagerAdapter();
        }

        private void InitializePagerAdapter()
        {
            pagerAdapter = new MainPagerAdapter(SupportFragmentManager, this);
            pager.Adapter = pagerAdapter;
            tabLayout.SetupWithViewPager(pager);
        }

        private void FloatingActionButtonClick(object sender, EventArgs e)
        {
            FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();
            new ReportTempleteListDialogFragment().Show(fragmentTransaction, Resources.GetString(Resource.String.DialogFragment));
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    mDrawerToggle.OnOptionsItemSelected(item);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void CreateActionBar()
        {
            ActionBar actionBar = SupportActionBar;
            Toolbar.LayoutParams layoutParams = new Toolbar.LayoutParams(ViewGroup.LayoutParams.WrapContent,
                ((actionBar.Height) / 2) - 5);
            actionBar.SetDisplayShowTitleEnabled(false);
            actionBar.SetDisplayShowCustomEnabled(true);
            actionBar.SetCustomView(SetCurrentProjectTitle(), layoutParams);
            actionBar.SetHomeAsUpIndicator(Resources.GetDrawable(Resource.Drawable.menu_navbar_ham_new));
            actionBar.SetDisplayHomeAsUpEnabled(true);
            actionBar.SetHomeButtonEnabled(true);
        }

        private View SetCurrentProjectTitle()
        {
            View currentProjectNameLayout =
                ((LayoutInflater)GetSystemService(LayoutInflaterService)).Inflate(Resource.Layout.current_project_name, null);
            textCurrentProjectTitle = currentProjectNameLayout.FindViewById<TextView>(Resource.Id.textViewTitle);
            textCurrentProjectTitle.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.projectselectionarrow, 0);
            textCurrentProjectTitle.SetTextColor(Color.White);
            textCurrentProjectTitle.Text = string.IsNullOrEmpty(currentProjectName) ? Resources.GetString(Resource.String.choose_project) : currentProjectName;
            textCurrentProjectTitle.Click += ProjectNameClick;
            return currentProjectNameLayout;
        }

        private void ProjectNameClick(object sender, EventArgs e)
        {
            var projectListActivity = new Intent(this, typeof(ProjectNameListActivity));
            StartActivityForResult(projectListActivity, 0);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode != Result.Ok) return;
            var jsonString = sharedPreferences.GetString(Resources.GetString(Resource.String.current_project), string.Empty);
            currentProject = JsonConvert.DeserializeObject<Project>(jsonString);
            textCurrentProjectTitle.Text = currentProject.Title;
            InitializePagerAdapter();
        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var item = mLeftDrawerAdapter.GetItem(e.Position);
            if (item.ToString() == "Logout")
            {
                Finish();
                StartActivity(typeof(LoginActivity));
            }
        }
    }
}