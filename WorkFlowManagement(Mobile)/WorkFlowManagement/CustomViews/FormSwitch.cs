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
using UniversalImageLoader.Core;
using WorkFlowManagement.Enum;
using WorkFlowManagement.Model;
using Orientation = Android.Widget.Orientation;

namespace WorkFlowManagement.CustomViews
{
    public class FormSwitch : LinearLayout
    {
        private RelativeLayout theme;
        private Resources resource;
        private bool defaultState;
        private string switchState;
        private ISharedPreferences sharedPreferences;
        private ISharedPreferencesEditor sharedPreferencesEditor;
        private bool isArcheived;
        private Context contextx;
        private int OwnerID;
        private int UserID;
        private int VerifierID;
        private InformationPopup Popup;
        private ReportStatus reportStatus;
        private String formType;
        private String section;
        private ImageLoader imageLoader;

        public FormSwitch(Context context, ReportElement element, int userID, int ownerID, int verifiedID, String type, ReportStatus Reportstatus, string sectionType, ImageLoader imgLoader)
            : base(context)
        {
            resource = context.Resources;
            contextx = context;
            OwnerID = ownerID;
            UserID = userID;
            VerifierID = verifiedID;
            formType = type;
            section = sectionType;
            theme = new FormTheme(context, element.Title);
            Orientation = Orientation.Vertical;
            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(context);
            sharedPreferencesEditor = sharedPreferences.Edit();
            Popup = new InformationPopup(context);
            reportStatus = Reportstatus;
            imageLoader = imgLoader;

            isArcheived = sharedPreferences.GetBoolean(Resources.GetString(Resource.String.is_archived), false);

            Switch swch = new Switch(context);

            RelativeLayout.LayoutParams paramsOfSwitch = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
            paramsOfSwitch.AddRule(LayoutRules.AlignParentLeft);
            swch.LayoutParameters = paramsOfSwitch;

            swch.SetPadding(0, 30, 30, 30);
            swch.Id = element.Id;
            swch.SetTextColor(Color.White);

            switchState = element.Value;
            ImageView indicatorImage = (ImageView)theme.GetChildAt(1);
            //activateElementInfo(element);
            Popup.activateElementInfo(theme, element);

            if (switchState == "")
            {
                swch.Text = "1";
            }

            else if (switchState == "false")
            {
                indicatorImage.SetImageResource(Resource.Drawable.checked_forms_create_project_medium);
                swch.Checked = false;
                swch.Text = "";
            }
            else if (switchState == "true")
            {
                indicatorImage.SetImageResource(Resource.Drawable.checked_forms_create_project_medium);
                swch.Checked = true;
                swch.Text = "";
            }

            swch.CheckedChange += (sender, e) =>
            {
                swch.Text = "";
                indicatorImage.SetImageResource(Resource.Drawable.checked_forms_create_project_medium);

                //if (element.Options != null && element.Options.FirstOrDefault(a => a.Code == "conditional").Value == "true")
                //{
                //    if (swch.Checked)
                //    {
                //        switchState = "true";
                //        element.Value = "true";

                //        if (ChildCount > 2)
                //        {
                //            RemoveViews(2, element.Rejected[0].Count);
                //        }

                //        for (int j = 0; j < element.Accepted[0].Count; j++)
                //        {
                //            AddView(getView(element.Accepted[0][j], element.Accepted[0]));
                //        }
                //    }
                //    else
                //    {
                //        switchState = "false";
                //        element.Value = "false";
                //        if (ChildCount > 2)
                //        {
                //            RemoveViews(2, element.Accepted[0].Count);
                //        }

                //        for (int k = 0; k < element.Rejected[0].Count; k++)
                //        {
                //            AddView(getView(element.Rejected[0][k], element.Rejected[0]));
                //        }
                //    }
                //}

                sharedPreferencesEditor.PutBoolean("ReportEditFlag", true);
                sharedPreferencesEditor.Commit();
            };

            //when opening a Draft or Archive
            if (swch.Checked)
            {
                indicatorImage.SetImageResource(Resource.Drawable.checked_forms_create_project_medium);
                switchState = "true";
            }

            if (OwnerID == 0 || OwnerID == userID)
            {
                if (VerifierID != 0)
                {
                    swch.Enabled = false;

                    if (reportStatus == ReportStatus.Rejected)
                    {
                        swch.Enabled = true;
                    }
                }

                else
                {
                    swch.Enabled = true;
                }
            }
            else
            {
                swch.Enabled = false;
            }

            TextView elementSplitLine = new TextView(context);
            elementSplitLine.TextSize = 0.5f;
            elementSplitLine.SetBackgroundColor(Color.ParseColor(resource.GetString(Resource.Color.grey)));

            if (isArcheived)
            {
                swch.Enabled = false;
            }

            AddView(theme);
            AddView(swch);
            SetPadding(55, 10, 45, 20);

            //if (element.Options != null)
            //{
            //    var conditionalCode = element.Options.FirstOrDefault(a => a.Code == "conditional");
            //    if (conditionalCode == null || !Convert.ToBoolean(conditionalCode.Value)) return;
            //    if (switchState.Equals("true"))
            //    {
            //        for (int i = 0; i < element.Accepted[0].Count; i++)
            //        {
            //            AddView(getView(element.Accepted[0][i], element.Accepted[0]));
            //        }
            //    }
            //    else
            //    {
            //        for (int i = 0; i < element.Rejected[0].Count; i++)
            //        {
            //            AddView(getView(element.Rejected[0][i], element.Rejected[0]));
            //        }

            //    }
            //}
        }

        private View getView(ReportElement element, List<ReportElement> elementList)
        {
            LinearLayout view = new LinearLayout(contextx);
            switch (element.Type)
            {
                case "textfield":
                    view = new FormEditText(contextx, element, UserID, OwnerID, VerifierID, reportStatus);
                    break;
                case "textfieldint":
                    view = new FormIntEditText(contextx, element, UserID, OwnerID, VerifierID, reportStatus);
                    break;
                case "slider":
                    view = new FormSlider(contextx, element, UserID, OwnerID, VerifierID, reportStatus);
                    break;
                case "signature":
                    view = new FormSignature(contextx, element, section, UserID, OwnerID, VerifierID, formType, reportStatus, elementList);
                    break;
                case "Button":
                    view = new FormButton(contextx, element, OwnerID, VerifierID);
                    break;
                case "yesno":
                    view = new FormSwitch(contextx, element, UserID, OwnerID, VerifierID, formType, reportStatus, section, imageLoader);
                    break;
                case "multilinetextfield":
                    view = new FormMultiLineEditText(contextx, element, UserID, OwnerID, VerifierID, reportStatus);
                    break;
                case "datetime":
                    view = new FormDateTime(contextx, element, OwnerID, VerifierID);
                    break;
                case "date":
                    view = new FormDate(contextx, element, UserID, OwnerID, VerifierID, reportStatus);
                    break;
                case "time":
                    view = new FormTime(contextx, element, UserID, OwnerID, VerifierID, reportStatus);
                    break;
                case "camera":
                    view = new FormCamera(contextx, element, UserID, OwnerID, VerifierID, reportStatus, section, section, elementList);
                    break;
                case "checkbox":
                    view = new FormCheckBox(contextx, element, UserID, OwnerID, VerifierID, reportStatus, formType, section, imageLoader);
                    break;
                case "dropdown":
                    view = new FormDropDown(contextx, element, UserID, OwnerID, VerifierID, reportStatus);
                    break;
                case "mainandsubfield":
                    view = new FormHeaderSubElement(contextx, element, UserID, VerifierID);
                    break;
                case "updown":
                    view = new FormPlusMinusCounter(contextx, element, UserID, OwnerID, VerifierID, reportStatus);
                    break;
                case "gps":
                    view = new FormGPS(contextx, element, OwnerID, UserID, VerifierID, reportStatus);
                    break;
                case "tabularform":
                    view = new FormTabular(contextx, element, OwnerID, VerifierID, section, reportStatus);
                    break;
            }
            return view;
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