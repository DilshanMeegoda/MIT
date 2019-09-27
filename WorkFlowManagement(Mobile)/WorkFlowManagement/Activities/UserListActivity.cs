using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Preferences;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Newtonsoft.Json;
using WorkFlowManagement.Adapters;
using WorkFlowManagement.Common;
using WorkFlowManagement.Enum;
using WorkFlowManagement.Model;
using WorkFlowManagement.Services;
using WorkFlowManagement.Services.Interfaces;
using AlertDialog = Android.App.AlertDialog;

namespace WorkFlowManagement.Activities
{
    [Activity(Label = "Users List")]
    public class UserListActivity : AppCompatActivity
    {
        private IUserService userService;
        private ISharedPreferences sharedPreferences;
        private UserSession currentSession;
        private UserListAdapter userListAdapter;
        private List<UserCompany> adapterUserList;
        private IList<Integer> assignUserList;
        private ExpandableListView expandableListView;
        private LinearLayout emptyListLayout;
        private TextView emptyListMessage;
        private Button confirm;
        private const string FindUserSearchTerm = "";
        private List<User> userList;
        private Drawable upArrow;
        private SearchView searchView;
        private ToggleButton assign;
        private List<UserCompany> userCompanies;
        private IList<Integer> selectedUserList;
        private string userListType;
        private Resources resources;
        private Activity activity;
        private IMenu menu;
        private bool isAddUser;
        private Project currentProject;

        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            resources = Resources;
            activity = this;
            selectedUserList = Intent.GetIntegerArrayListExtra(Resources.GetString(Resource.String.assign_user_id_list)) ?? new List<Integer>();
            userListType = Intent.GetStringExtra(Resources.GetString(Resource.String.assign_user_list_type));

            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);
            var userSessionJsonString = sharedPreferences.GetString(Resources.GetString(Resource.String.user_session), string.Empty);
            currentSession = JsonConvert.DeserializeObject<UserSession>(userSessionJsonString);

            var currentProjectJsonString = sharedPreferences.GetString(Resources.GetString(Resource.String.current_project), string.Empty);
            currentProject = JsonConvert.DeserializeObject<Project>(currentProjectJsonString);

            InitializeElements();
            await GetUserFromServer();
        }

        private void InitializeElements()
        {
            SetContentView(Resource.Layout.activity_user_list);

            expandableListView = (ExpandableListView)FindViewById(Resource.Id.userList);

            emptyListLayout = (LinearLayout)FindViewById(Resource.Id.viewEmptyUsers);
            emptyListMessage = (TextView)FindViewById(Resource.Id.txtEmptyUsers);
            
            confirm = (Button)FindViewById(Resource.Id.btnCollaboratorsConfirm);
            confirm.Click += ConfirmSelectedCollaborators;

            assignUserList = new List<Integer>();

            if (userListType == resources.GetString(Resource.String.add_collaborators))
            {
                confirm.Visibility = ViewStates.Visible;
            }
        }

        private void ConfirmSelectedCollaborators(object sender, EventArgs e)
        {
            if (assignUserList.Count > 0)
            {
                AlertDialog.Builder alertBuilder = new AlertDialog.Builder(this);
                alertBuilder.SetCancelable(false);
                alertBuilder.SetMessage("Do you want to share this report with this selected users?");
                alertBuilder.SetTitle("Share Report");
                alertBuilder.SetPositiveButton("Yes", (sender1, args) =>
                {
                    Intent formActivity = new Intent(this, typeof(FormActivity));
                    formActivity.PutExtra(Resources.GetString(Resource.String.assign_user_list_type), Resources.GetString(Resource.String.add_collaborators));
                    formActivity.PutIntegerArrayListExtra(Resources.GetString(Resource.String.assign_user_id_list), assignUserList);
                    SetResult(Result.Ok, formActivity);
                    Finish();
                });
                alertBuilder.SetNegativeButton("No", (sender1, args) =>
                {
                    assignUserList.Clear();
                    alertBuilder.Dispose();
                    userListAdapter = new UserListAdapter(this, userCompanies, selectedUserList, userListType);
                    userListAdapter.ButtonClickedOnAssignInvite += ButtonClickDelegate;
                    expandableListView.SetAdapter(userListAdapter);
                });
                alertBuilder.Show();
            }
            else
            {
                Utility.DisplayToast(this, "Please select a user to share report");
            }
        }

        private void FindUserClick(object sender, EventArgs e)
        {
            isAddUser = true;
            IMenuItem addMenuItem = menu.FindItem(Resource.Id.action_add_user);
            SetUserList(new List<User>());
            addMenuItem.SetVisible(false);
        }

        private async Task GetUserFromServer()
        {
            if (Utility.IsInternetAvailable(this))
            {
                ProgressDialog progress = new ProgressDialog(this);
                try
                {
                    progress.Indeterminate = true;
                    progress.SetProgressStyle(ProgressDialogStyle.Spinner);
                    progress.SetMessage(Resources.GetString(Resource.String.LoadingMessage));
                    progress.SetCancelable(false);
                    progress.Show();

                    userService = new UserService(currentSession.AccessToken);
                    userList = (await userService.SyncUsers(currentProject.ProjectId)).ToList();
                    SetUserList(RemoveLogedUserFromlist(userList));
                    progress.Dismiss();
                }
                catch (CustomHttpResponseException customHttpResponseException)
                {
                    if (customHttpResponseException.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        Log.Error("Authentication Exception - UserListActivity", customHttpResponseException.ToString());
                        Finish();
                        StartActivity(typeof(LoginActivity));
                    }
                    else
                    {
                        Log.Error(customHttpResponseException.StatusCode + " - UserListActivity", customHttpResponseException.ToString());
                        Utility.DisplayToast(this, "Error : " + customHttpResponseException.StatusCode + "\nAn error occurred while connecting to the application server");
                    }
                    progress.Dismiss();
                }
            }
            else
            {
                Utility.DisplayToast(this, Resources.GetString(Resource.String.NoInternet));
            }
        }

        private List<User> RemoveLogedUserFromlist(List<User> list)
        {
            return list.Where(user => user.UserId != currentSession.UserId).ToList();
        }

        private IList<UserCompany> GetUserCompanyList(List<User> users)
        {
            userCompanies = new List<UserCompany>();

            foreach (var company in users.GroupBy(a => a.CompanyName))
            {
                if (!string.IsNullOrEmpty(company.Key))
                {
                    var usercompany = new UserCompany();
                    usercompany.CompanyName = company.Key;
                    usercompany.Users = users.Where(a => a.CompanyName == company.Key).ToList();
                    userCompanies.Add(usercompany);
                }

                if (string.IsNullOrEmpty(company.Key) && users.Count(a => a.CompanyName == company.Key & a.UserRole != UserRole.InvitedUser.ToString()) != 0)
                {
                    var usercompanyOther = new UserCompany();
                    usercompanyOther.CompanyName = "Other";
                    usercompanyOther.Users = users.Where(a => a.CompanyName == company.Key & a.UserRole != UserRole.InvitedUser.ToString()).ToList();
                    userCompanies.Add(usercompanyOther);
                }

                if (string.IsNullOrEmpty(company.Key) && users.Count(a => a.UserRole == UserRole.InvitedUser.ToString()) != 0)
                {
                    var usercompanyInvited = new UserCompany();
                    usercompanyInvited.CompanyName = "Invited Users";
                    usercompanyInvited.Users = users.Where(a => a.CompanyName == company.Key & a.UserRole == UserRole.InvitedUser.ToString()).ToList();
                    userCompanies.Add(usercompanyInvited);
                }
            }
            return userCompanies;
        }

        private void ButtonClickDelegate(User user, bool isCheckd)
        {
                if (isCheckd)
                {
                    assignUserList.Add((Integer)user.UserId);

                    if (userListType == resources.GetString(Resource.String.verify))
                    {
                        AlertDialog.Builder alertBuilder = new AlertDialog.Builder(this);
                        alertBuilder.SetCancelable(false);
                        alertBuilder.SetMessage("Do you want to send this report to " + user.FullName + " for verification?");
                        alertBuilder.SetTitle("Send for Verification");
                        alertBuilder.SetPositiveButton("Yes", (sender, args) =>
                        {
                            Intent formActivity = new Intent(Application.Context, typeof(FormActivity));
                            formActivity.PutExtra(resources.GetString(Resource.String.assign_user_list_type),
                                resources.GetString(Resource.String.verify));
                            formActivity.PutIntegerArrayListExtra(
                                resources.GetString(Resource.String.assign_user_id_list), assignUserList);
                            activity.SetResult(Result.Ok, formActivity);
                            activity.Finish();
                        });
                        alertBuilder.SetNegativeButton("No", (sender, args) =>
                        {
                            RemoveSelectedItem(user.UserId);
                            alertBuilder.Dispose();
                            userListAdapter = new UserListAdapter(this, userCompanies, selectedUserList, userListType);
                            userListAdapter.ButtonClickedOnAssignInvite += ButtonClickDelegate;
                            expandableListView.SetAdapter(userListAdapter);
                        });
                        alertBuilder.Show();
                    }
                    else if (userListType == resources.GetString(Resource.String.change_ownership))
                    {
                        AlertDialog.Builder alertBuilder = new AlertDialog.Builder(this);
                        alertBuilder.SetCancelable(false);
                        alertBuilder.SetMessage("Do you want to change ownership of this report to " +
                                                user.FullName + "?");
                        alertBuilder.SetTitle("Change Ownership");
                        alertBuilder.SetPositiveButton("Yes", (sender, args) =>
                        {
                            Intent formActivity = new Intent(Application.Context, typeof(FormActivity));
                            formActivity.PutExtra(resources.GetString(Resource.String.assign_user_list_type),
                                resources.GetString(Resource.String.change_ownership));
                            formActivity.PutIntegerArrayListExtra(
                                resources.GetString(Resource.String.assign_user_id_list), assignUserList);
                            activity.SetResult(Result.Ok, formActivity);
                            activity.Finish();
                        });
                        alertBuilder.SetNegativeButton("No", (sender, args) =>
                        {
                            RemoveSelectedItem(user.UserId);
                            alertBuilder.Dispose();
                            userListAdapter = new UserListAdapter(this, userCompanies, selectedUserList, userListType);
                            userListAdapter.ButtonClickedOnAssignInvite += ButtonClickDelegate;
                            expandableListView.SetAdapter(userListAdapter);
                        });
                        alertBuilder.Show();
                    }
                    else
                    {
                        Utility.DisplayToast(Application.Context, user.FullName + " has been added to report");
                    }
                    sharedPreferences.Edit().PutBoolean("ReportEditFlag", true).Commit();
                }
                else
                {
                    RemoveSelectedItem(user.UserId);
                    Utility.DisplayToast(Application.Context, "Removed " + user.FullName + " from report");
                    sharedPreferences.Edit().PutBoolean("ReportEditFlag", false).Commit();
                }
        }

        private void RemoveSelectedItem(int userId)
        {
            int index = 0;
            foreach (var item in assignUserList.Where(item => item.IntValue() == userId))
            {
                index = assignUserList.IndexOf(item);
            }
            assignUserList.RemoveAt(index);
        }

        private void SetUserList(List<User> userList)
        {
            if (userList.Count == 0)
            {
                if (isAddUser)
                {
                    if (IsValidEmail(FindUserSearchTerm))
                    {
                        emptyListLayout.Visibility = ViewStates.Gone;
                        userList = new List<User>() { new User() { FullName = FindUserSearchTerm, ProjectUserRole = "Not found in system, send invite", CompanyName = "New User" } };
                    }
                    else
                    {
                        emptyListLayout.Visibility = ViewStates.Visible;
                        emptyListMessage.Text = "You can find person by type whole e-mail of the person you want to assign this report. If the e-mail is not registered in the CHECKD system, you can send them an invite";
                    }
                }
                else
                {
                    emptyListLayout.Visibility = ViewStates.Visible;
                    emptyListMessage.Text = " There are no any users on this project";
                }
            }
            else
            {
                emptyListLayout.Visibility = ViewStates.Gone;
            }

            userListAdapter = new UserListAdapter(this, (List<UserCompany>)GetUserCompanyList(userList), selectedUserList, userListType);
            userListAdapter.ButtonClickedOnAssignInvite += ButtonClickDelegate;
            expandableListView.SetAdapter(userListAdapter);

        }

        private static bool IsValidEmail(string email)
        {
            var emailPattern = new Regex(
                 @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))"
                 + @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$");
            return emailPattern.IsMatch(email.Trim());
        }
    }
}