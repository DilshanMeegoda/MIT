using System;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Widget;
using WorkFlowManagement.Model;
using Orientation = Android.Widget.Orientation;

namespace WorkFlowManagement.CustomViews
{
    public class FormDateTime : LinearLayout
    {
        private RelativeLayout theme;
        private Resources resource;
        private TextView dateTimeDisplay;
        private Button pickDate;
        private Button pickTime;
        private DatePickerDialog dateDialog;
        private TimePickerDialog timeDialog;

        private int hour;
        private int minute;
        private string time;
        private DateTime date;
        private int OwnerID;
        private int VerifierID;


        public FormDateTime(Context context, ReportElement element, int ownerID, int verifiedID)
            : base(context)
        {
            resource = context.Resources;
            theme = new FormTheme(context, element.Title);
            Orientation = Orientation.Vertical;
            OwnerID = ownerID;
            VerifierID = verifiedID;

            dateTimeDisplay = new TextView(context);
            dateTimeDisplay.Text = element.Value;
            date = DateTime.Today;
            pickDate = new Button(context);
            pickDate.Text = resource.GetString(Resource.String.setdate);
            pickDate.Click += delegate { createDateDialog(context); };

            hour = DateTime.Now.Hour;
            minute = DateTime.Now.Minute;
            time = string.Format("{0}:{1}", hour, minute.ToString().PadLeft(2, '0'));

            pickTime = new Button(context);
            pickTime.Text = resource.GetString(Resource.String.settime);
            pickTime.Click += delegate { createTimeDialog(context); };

            AddView(theme);
            AddView(dateTimeDisplay);
            AddView(pickDate);
            AddView(pickTime);
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
            dateTimeDisplay.Text = date.ToString("D") + " : " + time;
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
            dateTimeDisplay.Text = date + time;
        }

    }
}