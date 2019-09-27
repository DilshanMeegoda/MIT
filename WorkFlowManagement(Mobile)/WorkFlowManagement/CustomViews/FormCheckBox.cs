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
using UniversalImageLoader.Core;
using WorkFlowManagement.Enum;
using WorkFlowManagement.Model;
using Orientation = Android.Widget.Orientation;

namespace WorkFlowManagement.CustomViews
{
    public class FormCheckBox : LinearLayout
    {
        private RelativeLayout theme;
        private RelativeLayout timeFrame;
        private ISharedPreferences sharedPreferences;
        private ISharedPreferencesEditor sharedPreferencesEditor;
        private LinearLayout checkboxFrame;
        private LinearLayout parent;
        private FlowLayout checkHoriFrame;
        private bool isArcheived;
        private List<KeyValue> checkBoxValues;
        private Context contextx;
        private int layoutID;
        private ReportElement myElement;
        private int OwnerID;
        private int UserID;
        private int VerifierID;
        private int pos;
        private InformationPopup Popup;
        private ReportStatus reportStatus;
        private String formType;
        private String section;
        private ImageLoader imageLoader;

        public FormCheckBox(Context context, ReportElement element, int userID, int ownerID, int verifiedID, ReportStatus Reportstatus, string type, string sectionType, ImageLoader _imageLoader)
            : base(context)
        {
            contextx = context;
            myElement = element;
            UserID = userID;
            OwnerID = ownerID;
            VerifierID = verifiedID;
            formType = type;
            section = sectionType;
            imageLoader = _imageLoader;

            theme = new FormTheme(context, element.Title);
            Orientation = Orientation.Vertical;
            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(context);
            sharedPreferencesEditor = sharedPreferences.Edit();
            layoutID = Id * 1000;
            Popup = new InformationPopup(context);
            reportStatus = Reportstatus;

            isArcheived = sharedPreferences.GetBoolean(Resources.GetString(Resource.String.is_archived), false);

            checkboxFrame = new LinearLayout(context);
            checkHoriFrame = new FlowLayout(context);

            if (element.IsVertical)
            {
                checkboxFrame.Orientation = Orientation.Vertical;
            }

            ImageView indicatorImage = (ImageView)theme.GetChildAt(1);
            activateElementInfo(element);
            Popup.activateElementInfo(theme, element);

            checkBoxValues = element.Values;

            //CheckBox
            if (element.IsMultiSelect)
            {
                CreateCheckBox(context, indicatorImage);
            }
            //RadioButton
            else
            {
                CreateRadio(context, indicatorImage);
            }
            
            AddView(theme);

            if (element.IsVertical)
            {
                AddView(checkboxFrame);
            }
            else
            {
                AddView(checkHoriFrame);
            }
            SetPadding(45, 10, 45, 20);

            if (element.Values.Count > 0)
            {
                for (int i = 0; i < element.Values.Count; i++)
                {
                    if (element.Values[i].Condition && element.Values[i].Value.Equals("true"))
                    {
                        for (int j = 0; j < element.Values[i].Child[0].Count; j++)
                        {
                            AddView(getView(element.Values[i].Child[0][j], element.Values[i].Child[0]));
                        }
                    }
                }
            }
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
            }
            else
            {
                info.Click += (sender2, e) => showInfo(sender2, e, element.Info);
            }
        }

        private void CreateCheckBox(Context context, ImageView indicatorImage)
        {
            for (int i = 0; i < checkBoxValues.Count; i++)
            {
                CheckBox x = new CheckBox(context) { Text = checkBoxValues[i].Name };
                x.TextSize = 16;

                if (checkBoxValues[i].Value == "true")
                {
                    x.Checked = true;
                }
                else
                {
                    x.Checked = false;
                }

                x.SetPadding(15, 5, 10, 5);

                int i1 = i;
                x.CheckedChange += (o, e) =>
                {
                    x.RequestFocusFromTouch();
                    indicatorImage.SetImageResource(Resource.Drawable.checked_forms_create_project_medium);
                    sharedPreferencesEditor.PutBoolean("ReportEditFlag", true);
                    sharedPreferencesEditor.Commit();

                    if (x.Checked)
                    {
                        checkBoxValues[i1].Value = "true";
                        if (myElement.Values[i1].Condition)
                        {
                            for (int j = 0; j < myElement.Values[i1].Child[0].Count; j++)
                            {
                                AddView(getView(myElement.Values[i1].Child[0][j], myElement.Values[i1].Child[0]));
                            }
                        }
                    }
                    else
                    {
                        checkBoxValues[i1].Value = "false";
                        if (ChildCount > 2)
                        {
                            RemoveViews(2, ChildCount - 2);
                        }
                        for (int j = 0; j < myElement.Values.Count; j++)
                        {
                            if (checkBoxValues[j].Value.Equals("true") && checkBoxValues[j].Condition)
                            {
                                for (int k = 0; k < myElement.Values[j].Child[0].Count; k++)
                                {
                                    AddView(getView(myElement.Values[j].Child[0][k], myElement.Values[j].Child[0]));
                                }
                            }

                        }
                    }
                };

                if (isArcheived)
                {
                    x.Enabled = false;
                }


                if (OwnerID == 0 || OwnerID == UserID)
                {
                    if (VerifierID != 0)
                    {
                        x.Enabled = false;

                        if (reportStatus == ReportStatus.Rejected)
                        {
                            x.Enabled = true;
                        }
                    }

                    else
                    {
                        x.Enabled = true;
                    }
                }
                else
                {
                    x.Enabled = false;
                }


                if (myElement.IsVertical)
                {
                    checkboxFrame.AddView(x);
                }
                else
                {
                    checkHoriFrame.AddView(x);
                }
            }
        }

        private void CreateRadio(Context context, ImageView indicatorImage)
        {
            RadioGroup radioContainer = new RadioGroup(context);
            //int randomID = Guid.NewGuid().GetHashCode();
            if (myElement.IsVertical)
            {
                radioContainer.Orientation = Orientation.Vertical;
            }
            else
            {
                //radioContainer.Orientation = Orientation.Horizontal;
                radioContainer.Orientation = Orientation.Vertical;
            }

            int magicNumber = 867;

            for (int k = 0; k < checkBoxValues.Count; k++)
            {
                RadioButton r = new RadioButton(context) { Text = checkBoxValues[k].Name };
                r.Id = magicNumber + k;
                r.SetPadding(15, 5, 10, 5);
                r.TextSize = 16;

                int i2 = k;

                if (checkBoxValues[k].Value == "true")
                {
                    r.Checked = true;
                }

                r.CheckedChange += (o, e) =>
                {
                    r.RequestFocusFromTouch();
                    indicatorImage.SetImageResource(Resource.Drawable.checked_forms_create_project_medium);
                    sharedPreferencesEditor.PutBoolean("ReportEditFlag", true);
                    sharedPreferencesEditor.Commit();

                    if (r.Checked)
                    {
                        for (int j = 0; j < checkBoxValues.Count; j++)
                        {
                            checkBoxValues[j].Value = "false";
                        }
                        checkBoxValues[i2].Value = "true";

                        if (ChildCount > 2)
                        {
                            RemoveViews(2, ChildCount - 2);
                        }

                        if (myElement.Values[i2].Condition)
                        {
                            for (int j = 0; j < myElement.Values[i2].Child[0].Count; j++)
                            {
                                AddView(getView(myElement.Values[i2].Child[0][j],
                                    myElement.Values[i2].Child[0]));
                            }
                        }
                    }
                };

                if (isArcheived)
                {
                    r.Enabled = false;
                }

                if (OwnerID == 0 || OwnerID == UserID)
                {
                    if (VerifierID != 0)
                    {
                        r.Enabled = false;

                        if (reportStatus == ReportStatus.Rejected)
                        {
                            r.Enabled = true;
                        }
                    }

                    else
                    {
                        r.Enabled = true;
                    }
                }
                else
                {
                    r.Enabled = false;
                }

                radioContainer.AddView(r);
            }


            if (myElement.IsVertical)
            {
                checkboxFrame.AddView(radioContainer);
            }
            else
            {
                checkHoriFrame.AddView(radioContainer);
            }

        }

        public List<KeyValue> CheckBoxValues()
        {
            return checkBoxValues;
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