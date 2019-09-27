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
    public class FormMultiLineEditText : LinearLayout
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

        public FormMultiLineEditText(Context context, ReportElement element, int userID, int ownerID, int verifiedID, ReportStatus Reportstatus)
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

            EditText editText = new EditText(context);
            editText.Id = element.Id;
            editText.Text = element.Value;
            editText.SetBackgroundResource(Resource.Drawable.custom_edit_text_color);
            editText.InputType = Android.Text.InputTypes.TextFlagCapSentences;
            //editText.SetSingleLine(true);

            ImageView indicatorImage = (ImageView)theme.GetChildAt(1);
            //activateElementInfo(element);
            Popup.activateElementInfo(theme, element);
            editText.TextChanged += (sender, e) =>
            {
                if (!editText.Text.Equals(""))
                {
                    indicatorImage.SetImageResource(Resource.Drawable.checked_forms_create_project_medium);
                }
                else
                {
                    indicatorImage.SetImageResource(0);
                }
                sharedPreferencesEditor.PutBoolean("ReportEditFlag", true);
                sharedPreferencesEditor.Commit();
            };

            //when opening a Draft or Archive
            if (!editText.Text.Equals(""))
            {
                indicatorImage.SetImageResource(Resource.Drawable.checked_forms_create_project_medium);
            }

            TextView elementSplitLine = new TextView(context);
            elementSplitLine.TextSize = 0.5f;
            elementSplitLine.SetBackgroundColor(Color.ParseColor(resource.GetString(Resource.Color.grey)));

            if (OwnerID == 0 || OwnerID == userID)
            {
                if (VerifierID != 0)
                {
                    editText.Enabled = false;
                    editText.SetTextColor(Resources.GetColor(Resource.Color.grey));

                    if (reportStatus == ReportStatus.Rejected)
                    {
                        editText.Enabled = true;
                        editText.SetTextColor(Resources.GetColor(Resource.Color.black));
                    }
                }

                else
                {
                    editText.Enabled = true;
                    editText.SetTextColor(Resources.GetColor(Resource.Color.black));
                }
            }
            else
            {
                editText.Enabled = false;
                editText.SetTextColor(Resources.GetColor(Resource.Color.grey));
            }

            if (isArcheived)
            {
                editText.Enabled = false;
                editText.SetTextColor(Resources.GetColor(Resource.Color.grey));
            }

            AddView(theme);
            AddView(editText);
            SetPadding(45, 10, 45, 20);

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