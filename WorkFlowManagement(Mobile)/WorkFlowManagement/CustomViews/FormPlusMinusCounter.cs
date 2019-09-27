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
    public class FormPlusMinusCounter : LinearLayout
    {
        private RelativeLayout theme;
        private readonly Resources resource;
        private ISharedPreferences sharedPreferences;
        private ISharedPreferencesEditor sharedPreferencesEditor;
        private Context contextx;
        private EditText counterEditText;
        private bool isArcheived;
        private int OwnerID;
        private int VerifierID;
        private InformationPopup Popup;
        private ReportStatus reportStatus;

        public FormPlusMinusCounter(Context context, ReportElement element, int userID, int ownerID, int verifiedID, ReportStatus Reportstatus)
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

            RelativeLayout stepperlayout = new RelativeLayout(context);
            stepperlayout.FocusableInTouchMode = true;
            stepperlayout.Focusable = true;
            stepperlayout.SetPadding(0, 5, 0, 5);

            RelativeLayout.LayoutParams paramsForStepperLayout = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.WrapContent);
            paramsForStepperLayout.AddRule(LayoutRules.CenterHorizontal);
            stepperlayout.LayoutParameters = paramsForStepperLayout; //causes layout update

            ImageButton negativeButton = new ImageButton(context);
            negativeButton.SetImageResource(0);
            negativeButton.SetBackgroundResource(Resource.Drawable.decrement);

            ImageButton positiveButton = new ImageButton(context);
            positiveButton.SetImageResource(0);
            positiveButton.SetBackgroundResource(Resource.Drawable.increment);

            negativeButton.Click += (sender3, e) => decreaseCount(sender3, e, element.Info);
            positiveButton.Click += (sender4, e) => increaseCount(sender4, e, element.Info);

            counterEditText = new EditText(context);
            counterEditText.Id = element.Id;
            counterEditText.Text = element.Value;
            counterEditText.TextSize = 25;
            counterEditText.InputType = Android.Text.InputTypes.ClassNumber;
            counterEditText.SetTextColor(Color.ParseColor((resource.GetString(Resource.Color.green_primary))));

            counterEditText.Gravity = GravityFlags.Center;
            counterEditText.SetBackgroundResource(Resource.Drawable.back);
            counterEditText.SetPadding(10, 15, 10, 10);

            counterEditText.SetMaxWidth(350);
            counterEditText.SetMinWidth(220);
            counterEditText.SetMinimumWidth(220);

            counterEditText.SetMaxHeight(200);
            counterEditText.SetMinHeight(100);
            counterEditText.SetMinimumHeight(100);

            //counterEditText.Focusable = false;

            RelativeLayout.LayoutParams paramsForSliderCounter = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
            paramsForSliderCounter.AddRule(LayoutRules.CenterInParent);
            counterEditText.LayoutParameters = paramsForSliderCounter;

            RelativeLayout.LayoutParams paramsForNegative = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
            paramsForNegative.AddRule(LayoutRules.LeftOf, counterEditText.Id);
            paramsForNegative.AddRule(LayoutRules.AlignBottom, counterEditText.Id);
            paramsForNegative.AddRule(LayoutRules.AlignTop, counterEditText.Id);
            negativeButton.LayoutParameters = paramsForNegative;

            negativeButton.SetMinimumHeight(100);
            negativeButton.SetMaxHeight(150);
            negativeButton.SetMinimumWidth(200);
            negativeButton.SetMaxHeight(100);

            RelativeLayout.LayoutParams paramsForPositive = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
            paramsForPositive.AddRule(LayoutRules.RightOf, counterEditText.Id);
            paramsForPositive.AddRule(LayoutRules.AlignBottom, counterEditText.Id);
            paramsForPositive.AddRule(LayoutRules.AlignTop, counterEditText.Id);
            positiveButton.LayoutParameters = paramsForPositive;

            positiveButton.SetMinimumHeight(100);
            positiveButton.SetMaxHeight(150);
            positiveButton.SetMinimumWidth(200);
            positiveButton.SetMaxHeight(100);

            ImageView indicatorImageView = (ImageView)theme.GetChildAt(1);
            indicatorImageView.SetImageResource(0);
            Popup.activateElementInfo(theme, element);

            isArcheived = sharedPreferences.GetBoolean(Resources.GetString(Resource.String.is_archived), false);

            counterEditText.TextChanged += (sender, e) =>
            {
                if (!counterEditText.Text.Equals("0"))
                {
                    indicatorImageView.SetImageResource(Resource.Drawable.checked_forms_create_project_medium);
                    sharedPreferencesEditor.PutBoolean("ReportEditFlag", true);
                    sharedPreferencesEditor.Commit();
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
                    negativeButton.Enabled = false;
                    positiveButton.Enabled = false;
                    counterEditText.Enabled = false;

                    if (reportStatus == ReportStatus.Rejected)
                    {
                        negativeButton.Enabled = true;
                        positiveButton.Enabled = true;
                        counterEditText.Enabled = true;
                    }
                }

                else
                {
                    negativeButton.Enabled = true;
                    positiveButton.Enabled = true;
                    counterEditText.Enabled = true;
                }

            }
            else
            {
                negativeButton.Enabled = false;
                positiveButton.Enabled = false;
                counterEditText.Enabled = false;
            }

            if (isArcheived)
            {
                negativeButton.Enabled = false;
                positiveButton.Enabled = false;
                counterEditText.Enabled = false;
            }

            stepperlayout.AddView(negativeButton);
            stepperlayout.AddView(counterEditText);
            stepperlayout.AddView(positiveButton);

            AddView(theme);
            AddView(stepperlayout);
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

        private void increaseCount(object sender, EventArgs eventArgs, string information)
        {
            if (!string.IsNullOrEmpty(counterEditText.Text))
            {
                int value = Int32.Parse(counterEditText.Text);
                value = value + 1;
                counterEditText.Text = value + "";
            }
        }

        private void decreaseCount(object sender, EventArgs eventArgs, string information)
        {
            if (!string.IsNullOrEmpty(counterEditText.Text))
            {
                int value = Int32.Parse(counterEditText.Text);
                if (value != 0)
                {
                    value = value - 1;
                    counterEditText.Text = value + "";
                }

            }
        }
    }
}
