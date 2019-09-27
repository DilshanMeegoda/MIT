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
using Java.Lang;
using WorkFlowManagement.Enum;
using WorkFlowManagement.Model;
using Orientation = Android.Widget.Orientation;

namespace WorkFlowManagement.CustomViews
{
    public class FormSlider : LinearLayout
    {
        private RelativeLayout theme;
        private readonly Resources resource;
        private ISharedPreferences sharedPreferences;
        private ISharedPreferencesEditor sharedPreferencesEditor;
        private Context contextx;
        private int OwnerID;
        private int VerifierID;
        private bool isArcheived;
        private InformationPopup Popup;
        private ReportStatus reportStatus;

        public FormSlider(Context context, ReportElement element, int userID, int ownerID, int verifiedID, ReportStatus Reportstatus)
            : base(context)
        {
            resource = context.Resources;
            contextx = context;
            OwnerID = ownerID;
            VerifierID = verifiedID;
            Popup = new InformationPopup(context);
            reportStatus = Reportstatus;

            if (element.Value == "")
            {
                element.Value = "0";
            }

            theme = new FormTheme(context, element.Title);
            RelativeLayout countHolder = new RelativeLayout(context);

            Orientation = Orientation.Vertical;
            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(context);
            sharedPreferencesEditor = sharedPreferences.Edit();

            EditText counterEditText = new EditText(context);
            counterEditText.Text = element.Value;
            counterEditText.TextSize = 30;
            counterEditText.InputType = Android.Text.InputTypes.ClassNumber;
            counterEditText.SetTextColor(Color.ParseColor((resource.GetString(Resource.Color.green_primary))));
            counterEditText.Gravity = GravityFlags.CenterHorizontal;
            counterEditText.SetPadding(10, 0, 10, 0);
            counterEditText.SetBackgroundResource(Resource.Drawable.back);
            counterEditText.SetWidth(200);

            RelativeLayout.LayoutParams paramsForSliderCounter = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            paramsForSliderCounter.AddRule(LayoutRules.CenterHorizontal);
            counterEditText.LayoutParameters = paramsForSliderCounter; //causes layout update
            counterEditText.SetPadding(20, 5, 20, 5);

            countHolder.AddView(counterEditText);

            ImageView indicatorImageView = (ImageView)theme.GetChildAt(1);
            indicatorImageView.SetImageResource(0);
            //activateElementInfo(element);
            Popup.activateElementInfo(theme, element);

            SeekBar slider = new SeekBar(context);
            slider.Progress = Integer.ParseInt(element.Value);
            slider.SetPadding(45, 15, 45, 20);
            slider.Id = element.Id;
            slider.Max = 31;

            isArcheived = sharedPreferences.GetBoolean(Resources.GetString(Resource.String.is_archived), false);

            slider.ProgressChanged += (sender, e) =>
            {
                if (e.FromUser)
                {
                    counterEditText.Text = $"{e.Progress}";

                    if (counterEditText.Text.Equals("0"))
                    {
                        indicatorImageView.SetImageResource(0);
                    }
                    else
                    {
                        indicatorImageView.SetImageResource(Resource.Drawable.checked_forms_create_project_medium);
                        sharedPreferencesEditor.PutBoolean("ReportEditFlag", true);
                        sharedPreferencesEditor.Commit();
                    }
                }

            };

            counterEditText.TextChanged += (sender, e) =>
            {
                if (!counterEditText.Text.Equals("0"))
                {
                    indicatorImageView.SetImageResource(Resource.Drawable.checked_forms_create_project_medium);

                    if (Integer.ParseInt(counterEditText.Text) > 31 || Integer.ParseInt(counterEditText.Text) < 0)
                    {
                        sliderValuePopUp(context);
                        counterEditText.Text = "0";
                        slider.Progress = 0;
                    }
                    slider.Progress = Integer.ParseInt(counterEditText.Text);
                }
                else
                {
                    indicatorImageView.SetImageResource(0);
                }
            };

            //when opening a Draft or Archive
            if (!counterEditText.Text.Equals("0"))
            {
                indicatorImageView.SetImageResource(Resource.Drawable.checked_forms_create_project_medium);
            }

            if (OwnerID == 0 || OwnerID == userID)
            {
                if (VerifierID != 0)
                {
                    slider.Enabled = false;
                    counterEditText.Enabled = false;

                    if (reportStatus == ReportStatus.Rejected)
                    {
                        slider.Enabled = true;
                        counterEditText.Enabled = true;
                    }
                }

                else
                {
                    slider.Enabled = true;
                    counterEditText.Enabled = true;
                }
            }
            else
            {
                slider.Enabled = false;
                counterEditText.Enabled = false;
            }

            if (isArcheived)
            {
                slider.Enabled = false;
                counterEditText.Enabled = false;
            }

            AddView(theme);
            AddView(countHolder);
            AddView(slider);
            SetPadding(45, 10, 45, 20);

        }

        private void sliderValuePopUp(Context context)
        {
            var builder = new Android.Support.V7.App.AlertDialog.Builder(context);
            builder.SetTitle(resource.GetString(Resource.String.sliderValueErrorTitle));
            builder.SetMessage(resource.GetString(Resource.String.sliderValueErrorMessage));
            builder.SetCancelable(false);
            builder.SetPositiveButton(resource.GetString(Resource.String.ProjectTitleAlertOk), delegate { });
            builder.Show();
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