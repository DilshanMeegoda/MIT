using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class FormDate : LinearLayout
    {
        private RelativeLayout theme;
        private Resources resource;
        private EditText dateDisplay;
        private TextView weekDisplay;
        private Button clearDateButton;
        private DatePickerDialog dateDialog;
        private RelativeLayout dateFrame;
        private ISharedPreferences sharedPreferences;
        private ISharedPreferencesEditor sharedPreferencesEditor;
        private Context contextx;
        private DateTime date;
        private bool isArcheived;
        private InformationPopup Popup;
        private int OwnerID;
        private int VerifierID;
        private RelativeLayout.LayoutParams paramsDate;
        private ReportStatus reportStatus;

        public FormDate(Context context, ReportElement element, int userID, int ownerID, int verifiedID, ReportStatus Reportstatus)
            : base(context)
        {
            resource = context.Resources;
            OwnerID = ownerID;
            VerifierID = verifiedID;
            theme = new FormTheme(context, element.Title);
            Orientation = Orientation.Vertical;
            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(context);
            sharedPreferencesEditor = sharedPreferences.Edit();
            Popup = new InformationPopup(context);
            contextx = context;
            isArcheived = sharedPreferences.GetBoolean(Resources.GetString(Resource.String.is_archived), false);
            reportStatus = Reportstatus;
            dateFrame = new RelativeLayout(context);

            dateDisplay = new EditText(context);
            dateDisplay.Id = element.Id;

            RelativeLayout.LayoutParams dateFrameParametere = new RelativeLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            dateFrame.LayoutParameters = dateFrameParametere;

            //datedisplay
            RelativeLayout.LayoutParams paramsOfDateDisplay = new RelativeLayout.LayoutParams(500, RelativeLayout.LayoutParams.WrapContent);
            paramsOfDateDisplay.AddRule(LayoutRules.AlignParentLeft);

            RelativeLayout.LayoutParams paramsOfWeekDisplay = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.WrapContent);
            paramsOfWeekDisplay.AddRule(LayoutRules.AlignParentBottom);
            paramsOfWeekDisplay.AddRule(LayoutRules.Below, dateDisplay.Id);

            ImageView indicatorImage = (ImageView)theme.GetChildAt(1);
            Popup.activateElementInfo(theme, element);

            dateDisplay = new EditText(context);
            dateDisplay.Id = element.Id;
            dateDisplay.SetTextColor(Color.Black);
            dateDisplay.LayoutParameters = paramsOfDateDisplay;
            dateDisplay.SetBackgroundResource(Resource.Drawable.custom_edit_text_color);
            dateDisplay.InputType = Android.Text.InputTypes.TextFlagNoSuggestions;
            dateDisplay.Touch += (s, e) => {
                var handled = false;
                if (e.Event.Action == MotionEventActions.Down)
                {
                    createDateDialog(context);
                    handled = true;
                }
                else if (e.Event.Action == MotionEventActions.Up)
                {
                    // do other stuff
                    handled = true;
                }
                e.Handled = handled;
            };

            //button
            RelativeLayout.LayoutParams paramsOfClearButton = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
            //paramsOfClearButton.AddRule(LayoutRules.CenterVertical);
            paramsOfClearButton.AddRule(LayoutRules.RightOf, dateDisplay.Id);

            clearDateButton = new Button(context);
            clearDateButton.Text = "Clear";
            clearDateButton.TextSize = 12;
            clearDateButton.SetTextColor(Resources.GetColor(Resource.Color.theme_color));
            clearDateButton.SetBackgroundResource(0);
            clearDateButton.LayoutParameters = paramsOfClearButton;
            //clearDateButton.SetBackgroundResource(Resource.Drawable.clear_button);
            clearDateButton.Click += delegate { clearDate(); };

            if (string.IsNullOrEmpty(element.Value))
            {
                dateDisplay.Text = "";
                indicatorImage.SetImageResource(0);
                clearDateButton.Visibility = ViewStates.Gone;
            }
            else
            {
                indicatorImage.SetImageResource(Resource.Drawable.checked_forms_create_project_medium);
                dateDisplay.Text = dateLogic(element.Value);
                //have to set week
            }

            dateDisplay.TextChanged += (sender, e) =>
            {
                if (!dateDisplay.Text.Equals(""))
                {
                    indicatorImage.SetImageResource(Resource.Drawable.checked_forms_create_project_medium);
                    clearDateButton.Visibility = ViewStates.Visible;
                }
                else
                {
                    indicatorImage.SetImageResource(0);
                    clearDateButton.Visibility = ViewStates.Gone;
                }
                sharedPreferencesEditor.PutBoolean("ReportEditFlag", true);
                sharedPreferencesEditor.Commit();
            };

            weekDisplay = new TextView(context);
            weekDisplay.SetWidth(80);
            weekDisplay.SetPadding(40, 0, 0, 0);

            if (element.Value == "")
            {
                //donothing
            }
            else
            {
                DateTime dateTime = Convert.ToDateTime(element.Value);
                weekDisplay.Text = resource.GetString(Resource.String.week) + getWeekNo(dateTime);
            }


            weekDisplay.SetTextColor(Color.Black);
            weekDisplay.LayoutParameters = paramsOfWeekDisplay;

            date = DateTime.Today;

            if (OwnerID == 0 || OwnerID == userID)
            {
                if (VerifierID != 0)
                {
                    clearDateButton.Enabled = false;
                    clearDateButton.Visibility = ViewStates.Gone;
                    dateDisplay.Enabled = false;
                    dateDisplay.SetTextColor(Resources.GetColor(Resource.Color.grey));
                    weekDisplay.SetTextColor(Resources.GetColor(Resource.Color.grey));

                    if (reportStatus == ReportStatus.Rejected)
                    {
                        clearDateButton.Enabled = true;
                        clearDateButton.Visibility = ViewStates.Gone;
                        dateDisplay.Enabled = true;
                        dateDisplay.SetTextColor(Resources.GetColor(Resource.Color.black));
                        weekDisplay.SetTextColor(Resources.GetColor(Resource.Color.black));
                    }
                }

                else
                {
                    clearDateButton.Enabled = true;
                    clearDateButton.Visibility = ViewStates.Gone;
                    dateDisplay.Enabled = true;
                    dateDisplay.SetTextColor(Resources.GetColor(Resource.Color.black));
                    weekDisplay.SetTextColor(Resources.GetColor(Resource.Color.black));
                }
            }
            else
            {
                clearDateButton.Enabled = false;
                clearDateButton.Visibility = ViewStates.Gone;
                dateDisplay.Enabled = false;
                dateDisplay.SetTextColor(Resources.GetColor(Resource.Color.grey));
                weekDisplay.SetTextColor(Resources.GetColor(Resource.Color.grey));
            }

            if (isArcheived)
            {
                clearDateButton.Enabled = false;
                clearDateButton.Visibility = ViewStates.Gone;
                dateDisplay.Enabled = false;
                dateDisplay.SetTextColor(Resources.GetColor(Resource.Color.grey));
            }

            dateFrame.AddView(dateDisplay);
            dateFrame.AddView(clearDateButton);
            dateFrame.AddView(weekDisplay);

            AddView(theme);
            AddView(dateFrame);
            SetPadding(45, 10, 45, 20);

        }

        private void createDateDialog(Context context)
        {
            dateDialog = new DatePickerDialog(context, HandleDateSet, date.Year, date.Month, date.Day);
            dateDialog.Show();
        }


        void HandleDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            date = e.Date;
            dateDisplay.Text = date.ToString("dd.MMM.yyyy");
            weekDisplay.Text = resource.GetString(Resource.String.week) + getWeekNo(date);
        }

        private int getWeekNo(DateTime date)
        {
            int weekNo;
            try
            {
                var currentCulture = CultureInfo.CurrentCulture;
                weekNo = currentCulture.Calendar.GetWeekOfYear(date,
                    currentCulture.DateTimeFormat.CalendarWeekRule,
                    currentCulture.DateTimeFormat.FirstDayOfWeek);
            }
            catch (Exception e)
            {
                weekNo = 0;
                return weekNo;
            }

            return weekNo;
        }

        private string dateLogic(string date)
        {

            //dateDisplay.Text = date.ToString("dd.MMM.yyyy");
            //weekDisplay.Text = "WEEK: " + getWeekNo(date);

            string resultDAte = "";
            char[] delimiterChars = { '-' };
            string[] words = date.Split(delimiterChars);

            for (int i = words.Length - 1; i > -1; i--)
            {
                if (i == 2)
                {
                    resultDAte = words[i];
                }
                else if (i == 1)
                {
                    string month;
                    month = words[i];
                    if (month == "01")
                    {
                        resultDAte = resultDAte + ".Jan";
                    }
                    else if (month == "02")
                    {
                        resultDAte = resultDAte + ".Feb";
                    }
                    else if (month == "03")
                    {
                        resultDAte = resultDAte + ".Mar";
                    }
                    else if (month == "04")
                    {
                        resultDAte = resultDAte + ".Apr";
                    }
                    else if (month == "05")
                    {
                        resultDAte = resultDAte + ".May";
                    }
                    else if (month == "06")
                    {
                        resultDAte = resultDAte + ".Jun";
                    }
                    else if (month == "07")
                    {
                        resultDAte = resultDAte + ".Jul";
                    }
                    else if (month == "08")
                    {
                        resultDAte = resultDAte + ".Aug";
                    }
                    else if (month == "09")
                    {
                        resultDAte = resultDAte + ".Sep";
                    }
                    else if (month == "10")
                    {
                        resultDAte = resultDAte + ".Oct";
                    }
                    else if (month == "11")
                    {
                        resultDAte = resultDAte + ".Nov";
                    }
                    else if (month == "12")
                    {
                        resultDAte = resultDAte + ".Dec";
                    }

                }

                else if (i == 0)
                {
                    resultDAte = resultDAte + "." + words[i];
                }
            }

            return resultDAte;
        }

        void clearDate()
        {
            dateDisplay.Text = "";
            weekDisplay.Text = "";
        }

    }
}