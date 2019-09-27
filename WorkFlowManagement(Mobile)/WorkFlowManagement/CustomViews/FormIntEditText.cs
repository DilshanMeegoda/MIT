using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
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
    public class FormIntEditText : LinearLayout
    {
        private RelativeLayout theme;
        private Resources resource;
        private ISharedPreferences sharedPreferences;
        private ISharedPreferencesEditor sharedPreferencesEditor;
        private Context contextx;
        private bool isArcheived;
        private int OwnerID;
        private int VerifierID;
        private InformationPopup Popup;
        private ReportStatus reportStatus;

        public FormIntEditText(Context context, ReportElement element, int userID, int ownerID, int verifiedID, ReportStatus Reportstatus)
            : base(context)
        {
            resource = context.Resources;
            contextx = context;
            OwnerID = ownerID;
            VerifierID = verifiedID;
            theme = new FormTheme(context, element.Title);
            Popup = new InformationPopup(context);
            reportStatus = Reportstatus;

            Orientation = Orientation.Vertical;
            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(context);
            sharedPreferencesEditor = sharedPreferences.Edit();

            isArcheived = sharedPreferences.GetBoolean(Resources.GetString(Resource.String.is_archived), false);

            SetPadding(45, 10, 45, 20);

            EditText intEditText = new EditText(context);
            intEditText.InputType = Android.Text.InputTypes.ClassNumber | Android.Text.InputTypes.TextFlagNoSuggestions;
            intEditText.Id = element.Id;
            intEditText.Text = element.Value;
            intEditText.SetBackgroundResource(Resource.Drawable.custom_edit_text_color);

            ImageView indicatorImageView = (ImageView)theme.GetChildAt(1);
            indicatorImageView.SetImageResource(0);
            //activateElementInfo(element);
            Popup.activateElementInfo(theme, element);

            intEditText.TextChanged += (sender, e) =>
            {
                if (!intEditText.Text.Equals(""))
                {

                    indicatorImageView.SetImageResource(Resource.Drawable.checked_forms_create_project_medium);
                }
                else
                {
                    indicatorImageView.SetImageResource(0);
                }
                sharedPreferencesEditor.PutBoolean("ReportEditFlag", true);
                sharedPreferencesEditor.Commit();
            };

            //when opening a Draft or Archive
            if (!intEditText.Text.Equals(""))
            {

                indicatorImageView.SetImageResource(Resource.Drawable.checked_forms_create_project_medium);
            }

            if (OwnerID == 0 || OwnerID == userID)
            {
                if (VerifierID != 0)
                {
                    intEditText.Enabled = false;
                    intEditText.SetTextColor(Resources.GetColor(Resource.Color.grey));

                    if (reportStatus == ReportStatus.Rejected)
                    {
                        intEditText.Enabled = true;
                        intEditText.SetTextColor(Resources.GetColor(Resource.Color.black));
                    }
                }

                else
                {
                    intEditText.Enabled = true;
                    intEditText.SetTextColor(Resources.GetColor(Resource.Color.black));
                }
            }
            else
            {
                intEditText.Enabled = false;
                intEditText.SetTextColor(Resources.GetColor(Resource.Color.grey));
            }

            if (isArcheived)
            {
                intEditText.Enabled = false;
                intEditText.SetTextColor(Resources.GetColor(Resource.Color.grey));
            }

            AddView(theme);
            AddView(intEditText);

        }

        private void activateElementInfo(ReportElement element)
        {
            LinearLayout descriptionHolder = (LinearLayout)theme.GetChildAt(0);
            ImageButton info = (ImageButton)descriptionHolder.GetChildAt(1);

            if (string.IsNullOrEmpty(element.Info))
            {
                info.Visibility = ViewStates.Invisible;
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