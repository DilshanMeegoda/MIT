using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using WorkFlowManagement.Enum;
using WorkFlowManagement.Model;
using Orientation = Android.Widget.Orientation;

namespace WorkFlowManagement.CustomViews
{
    public class FormTime : LinearLayout
    {
        private RelativeLayout theme;
        private Resources resource;
        private EditText timeDisplay;
        private Button clearTimeButton;
        private TimePickerDialog timeDialog;
        private RelativeLayout timeFrame;
        private ISharedPreferences sharedPreferences;
        private ISharedPreferencesEditor sharedPreferencesEditor;
        private int OwnerID;
        private InformationPopup Popup;

        private int hour;
        private int minute;
        private bool isArcheived;
        private Context contextx;
        private int VerifierID;
        private ReportStatus reportStatus;

        public FormTime(Context context, ReportElement element, int userID, int ownerID, int verifiedID, ReportStatus Reportstatus)
            : base(context)
        {
            resource = context.Resources;
            contextx = context;
            OwnerID = ownerID;
            VerifierID = verifiedID;
            theme = new FormTheme(context, element.Title);
            Orientation = Orientation.Vertical;
            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(context);
            sharedPreferencesEditor = sharedPreferences.Edit();
            Popup = new InformationPopup(context);
            reportStatus = Reportstatus;

            isArcheived = sharedPreferences.GetBoolean(Resources.GetString(Resource.String.is_archived), false);

            timeFrame = new RelativeLayout(context);

            RelativeLayout.LayoutParams parms2 = new RelativeLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            timeFrame.LayoutParameters = parms2;

            ImageView indicatorImage = (ImageView)theme.GetChildAt(1);
            //activateElementInfo(element);
            Popup.activateElementInfo(theme, element);

            RelativeLayout.LayoutParams paramsOfTimeDisplay = new RelativeLayout.LayoutParams(500, RelativeLayout.LayoutParams.WrapContent);
            paramsOfTimeDisplay.AddRule(LayoutRules.CenterVertical);
            paramsOfTimeDisplay.AddRule(LayoutRules.AlignParentLeft);

            timeDisplay = new EditText(context);
            timeDisplay.Id = element.Id;
            timeDisplay.SetTextColor(Color.Black);
            timeDisplay.LayoutParameters = paramsOfTimeDisplay;
            timeDisplay.Focusable = false;
            timeDisplay.SetBackgroundResource(Resource.Drawable.custom_edit_text_color);
            timeDisplay.InputType = Android.Text.InputTypes.TextFlagNoSuggestions;

            RelativeLayout.LayoutParams paramsOfClearButton = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
            paramsOfClearButton.AddRule(LayoutRules.RightOf, timeDisplay.Id);

            timeDisplay.Touch += (s, e) =>
            {
                var handled = false;
                if (e.Event.Action == MotionEventActions.Down)
                {
                    createTimeDialog(context);
                    handled = true;
                }
                else if (e.Event.Action == MotionEventActions.Up)
                {
                    handled = true;
                }
                e.Handled = handled;
            };

            clearTimeButton = new Button(context);
            clearTimeButton.Text = "Clear";
            clearTimeButton.TextSize = 12;
            clearTimeButton.SetTextColor(Resources.GetColor(Resource.Color.theme_color));
            clearTimeButton.SetBackgroundResource(0);
            clearTimeButton.LayoutParameters = paramsOfClearButton;
            clearTimeButton.Click += delegate { clearTime(); };

            hour = DateTime.Now.Hour;
            minute = DateTime.Now.Minute;

            if (string.IsNullOrEmpty(element.Value))
            {
                timeDisplay.Text = "";
                indicatorImage.SetImageResource(0);
                clearTimeButton.Visibility = ViewStates.Gone;
            }
            else
            {
                indicatorImage.SetImageResource(Resource.Drawable.checked_forms_create_project_medium);
                timeDisplay.Text = element.Value;
            }

            timeDisplay.TextChanged += (sender, e) =>
            {
                if (!timeDisplay.Text.Equals(""))
                {
                    indicatorImage.SetImageResource(Resource.Drawable.checked_forms_create_project_medium);
                    clearTimeButton.Visibility = ViewStates.Visible;
                }
                else
                {
                    indicatorImage.SetImageResource(0);
                    clearTimeButton.Visibility = ViewStates.Gone;
                }

                sharedPreferencesEditor.PutBoolean("ReportEditFlag", true);
                sharedPreferencesEditor.Commit();

            };

            if (OwnerID == 0 || OwnerID == userID)
            {
                if (VerifierID != 0)
                {
                    clearTimeButton.Enabled = false;
                    clearTimeButton.Visibility = ViewStates.Gone;
                    timeDisplay.Enabled = false;
                    timeDisplay.SetTextColor(Resources.GetColor(Resource.Color.grey));

                    if (reportStatus == ReportStatus.Rejected)
                    {
                        clearTimeButton.Enabled = true;
                        clearTimeButton.Visibility = ViewStates.Gone;
                        timeDisplay.Enabled = true;
                        timeDisplay.SetTextColor(Resources.GetColor(Resource.Color.black));
                    }
                }

                else
                {
                    clearTimeButton.Enabled = true;
                    clearTimeButton.Visibility = ViewStates.Gone;
                    timeDisplay.Enabled = true;
                    timeDisplay.SetTextColor(Resources.GetColor(Resource.Color.black));
                }
            }
            else
            {
                clearTimeButton.Enabled = false;
                clearTimeButton.Visibility = ViewStates.Gone;
                timeDisplay.Enabled = false;
                timeDisplay.SetTextColor(Resources.GetColor(Resource.Color.grey));
            }


            if (isArcheived)
            {
                timeDisplay.Enabled = false;
                clearTimeButton.Visibility = ViewStates.Gone;
                timeDisplay.Enabled = false;
                timeDisplay.SetTextColor(Resources.GetColor(Resource.Color.grey));
            }

            timeFrame.AddView(timeDisplay);
            timeFrame.AddView(clearTimeButton);
            AddView(theme);
            AddView(timeFrame);
            SetPadding(45, 10, 45, 20);

        }

        private void createTimeDialog(Context context)
        {
            timeDialog = new TimePickerDialog(context, HandleTimeSet, hour, minute, true);
            timeDialog.Show();
        }

        void HandleTimeSet(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            hour = e.HourOfDay;
            minute = e.Minute;

            string time = string.Format("{0}:{1}", hour, minute.ToString().PadLeft(2, '0'));
            timeDisplay.Text = time;
        }

        void clearTime()
        {
            timeDisplay.Text = "";
        }

        private void activateElementInfo(ReportElement element)
        {
            LinearLayout descriptionHolder = (LinearLayout)theme.GetChildAt(0);
            ImageButton info = (ImageButton)descriptionHolder.GetChildAt(1);

            if (string.IsNullOrEmpty(element.Info))
            {
                info.Visibility = ViewStates.Invisible;
                //info.Visibility = ViewStates.Gone;
            }
            else
            {
                info.Click += (sender2, e) => showInfo(sender2, e, element.Info);
            }
        }

        private void showInfo(object sender, EventArgs eventArgs, string information)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(contextx);
            builder.SetMessage(information);

            builder.SetPositiveButton("Ok", (senderAlert, args) =>
            {
                builder.Dispose();
            });
            builder.Show();
        }
    }
}