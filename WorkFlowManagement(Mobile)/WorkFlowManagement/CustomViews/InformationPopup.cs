using System;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using WorkFlowManagement.Model;

namespace WorkFlowManagement.CustomViews
{
    public class InformationPopup
    {
        private Context contextx;

        public InformationPopup(Context context)
        {
            contextx = context;
        }

        public void activateElementInfo(RelativeLayout headerTheme, ReportElement element)
        {
            LinearLayout descriptionHolder = (LinearLayout)headerTheme.GetChildAt(0);
            ImageButton info = (ImageButton)descriptionHolder.GetChildAt(1);

            if (string.IsNullOrEmpty(element.Info))
            {
                info.Visibility = ViewStates.Invisible;
            }
            else
            {
                info.Click += (sender2, e) => showInfo(sender2, e, "Information", element.Info);
            }
        }

        public void showInfo(object sender, EventArgs eventArgs, string title, string information)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(contextx);
            builder.SetTitle(title);
            builder.SetMessage(information);

            builder.SetPositiveButton("Ok", (senderAlert, args) =>
            {
                builder.Dispose();
            });
            builder.Show();
        }
    }
}