using System;
using System.IO;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Util;
using Android.Views.InputMethods;
using Android.Widget;
using Newtonsoft.Json;
using WorkFlowManagement.Common;
using WorkFlowManagement.Enum;
using WorkFlowManagement.Services;
using WorkFlowManagement.Services.Dto;
using WorkFlowManagement.Services.Interfaces;
using Environment = Android.OS.Environment;
using File = Java.IO.File;

namespace WorkFlowManagement.Activities
{
    [Activity(Label = "Login Activity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class LoginActivity : Activity
    {
        private readonly ILoginService loginService;
        private IProjectService projectService;
        private ISharedPreferences sharedPreferences;
        private ISharedPreferencesEditor sharedPreferencesEditor;
        private Button buttonLogin;
        private EditText textPassword;
        private EditText textEmail;

        public LoginActivity()
        {
            loginService = new LoginService();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_login);

            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);
            sharedPreferencesEditor = sharedPreferences.Edit();
            sharedPreferencesEditor.Clear().Apply();

            MakeDirectoryForTheApp();
            InitializeElements();
        }

        private void InitializeElements()
        {
            buttonLogin = FindViewById<Button>(Resource.Id.btnLogin);
            textPassword = FindViewById<EditText>(Resource.Id.txtPassword);
            textEmail = FindViewById<EditText>(Resource.Id.txtEmail);
            buttonLogin.Click += ButtonLoginClick;
        }

        private async void ButtonLoginClick(object sender, EventArgs e)
        {
            InputMethodManager inputMethodManager = (InputMethodManager)GetSystemService(InputMethodService);
            inputMethodManager.HideSoftInputFromWindow(textPassword.WindowToken, 0);
            ((InputMethodManager)GetSystemService(InputMethodService)).HideSoftInputFromInputMethod(CurrentFocus.WindowToken, HideSoftInputFlags.NotAlways);

            if (string.IsNullOrEmpty(textEmail.Text) || string.IsNullOrEmpty(textPassword.Text))
            {
                Utility.DisplayToast(this, Resources.GetString(Resource.String.FillFields));
                return;
            }

            if (!IsValidEmail(textEmail.Text))
            {
                Utility.DisplayToast(this, Resources.GetString(Resource.String.InvalidEmail));
                return;
            }

            if (Utility.IsInternetAvailable(this))
            {
                var progress = new ProgressDialog(this) {Indeterminate = true};
                progress.SetProgressStyle(ProgressDialogStyle.Spinner);
                progress.SetMessage(Resources.GetString(Resource.String.LoadingMessage));
                progress.SetCancelable(false);
                progress.Show();

                try
                {
                    var userSession = await loginService.GetValidUserAsync(new LoginDto
                    {
                        Email = textEmail.Text,
                        Password = textPassword.Text,
                        DeviceToken = Utility.GetDeviceId(this),
                        DeviceType = DeviceType.Android,
                        AppType = AppType.Forms
                    });

                    if (userSession != null)
                    {
                        projectService = new ProjectService(userSession.AccessToken);
                        var projectList = await projectService.GetProjectList(userSession.UserId);
                        sharedPreferencesEditor.PutString(Resources.GetString(Resource.String.user_session), JsonConvert.SerializeObject(userSession));
                        sharedPreferencesEditor.PutString(Resources.GetString(Resource.String.current_project), JsonConvert.SerializeObject(projectList[0]));
                        sharedPreferencesEditor.Commit();
                        GoToMainActivity();
                    }
                    else
                    {
                        Utility.DisplayToast(this, Resources.GetString(Resource.String.LoginFailed));
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug(ComponentName.ClassName, ex.ToString());
                }
                progress.Dismiss();
            }
            else
            {
                Utility.DisplayToast(this, Resources.GetString(Resource.String.NoInternet));
            }
        }

        private void GoToMainActivity()
        {
            Finish();
            var mainActivity = new Intent(this, typeof(MainActivity));
            StartActivity(mainActivity);
        }

        private bool IsValidEmail(string email)
        {
            var emailpattern = new Regex(
                @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))"
                + @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$");
            return emailpattern.IsMatch(email.Trim());
        }

        private void MakeDirectoryForTheApp()
        {
            var sdCardPath = Environment.ExternalStorageDirectory.AbsolutePath;
            var filePath = Path.Combine(sdCardPath, "WFMS");

            var file = new File(filePath);
            if (!file.Exists())
            {
                file.Mkdir();
            }
        }
    }
}