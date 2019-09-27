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
using WorkFlowManagement.Model;
using Orientation = Android.Widget.Orientation;

namespace WorkFlowManagement.CustomViews
{
    public class FormHeaderSubElement : LinearLayout
    {
        private RelativeLayout theme;
        private Resources resource;
        private RelativeLayout timeFrame;
        private ISharedPreferences sharedPreferences;
        private ISharedPreferencesEditor sharedPreferencesEditor;
        private LinearLayout checkboxFrame;
        private FlowLayout checkHoriFrame;
        private bool isArcheived;
        private List<KeyValue> checkBoxValues;
        private Context contextx;
        private ReportElement thisElement;
        private int VerifierID;

        //mainandsubfield

        public FormHeaderSubElement(Context context, ReportElement element, int userID, int verifiedID)
            : base(context)
        {
            contextx = context;
            resource = context.Resources;
            thisElement = element;
            VerifierID = verifiedID;
            theme = new FormTheme(context, "");
            Orientation = Orientation.Vertical;
            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(context);
            sharedPreferencesEditor = sharedPreferences.Edit();

            RelativeLayout.LayoutParams paramsForThemeLayout = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, 0);
            theme.LayoutParameters = paramsForThemeLayout; //causes layout update

            isArcheived = sharedPreferences.GetBoolean(Resources.GetString(Resource.String.is_archived), false);

            checkboxFrame = new LinearLayout(context);
            checkboxFrame.Orientation = Orientation.Vertical;

            ImageView indicatorImage = (ImageView)theme.GetChildAt(1);
            activateElementInfo(element);

            checkBoxValues = element.Values;

            CreateCheckBox(context, indicatorImage, element);

            if (string.IsNullOrEmpty(element.FilledBy))
            {
                //do nothing
            }
            else if (element.FilledBy == userID + "")
            {
                //do nothing
            }
            else
            {
                checkboxFrame.Enabled = false;
            }

            int id = Id;
            AddView(theme);
            AddView(checkboxFrame);
            SetPadding(45, 10, 45, 20);
        }

        private void activateElementInfo(ReportElement element)
        {
            LinearLayout descriptionHolder = (LinearLayout)theme.GetChildAt(0);
            ImageButton info = (ImageButton)descriptionHolder.GetChildAt(1);
            info.Visibility = ViewStates.Invisible;
        }

        private void CreateCheckBox(Context context, ImageView indicatorImage, ReportElement element)
        {
            TextView headerField = new TextView(context);
            TextView subField = new TextView(context);

            headerField.Text = element.Title;
            headerField.Typeface = Typeface.DefaultBold;
            headerField.TextSize = 20;
            headerField.SetTextColor(Resources.GetColor(Resource.Color.dark_black));

            subField.Text = element.Info;
            subField.SetPadding(0, 10, 0, 0);
            headerField.SetTextColor(Resources.GetColor(Resource.Color.dark_black));

            checkboxFrame.AddView(headerField);
            checkboxFrame.AddView(subField);
        }

        public List<KeyValue> HeaderSubValues()
        {
            checkBoxValues[0].Name = "Header";
            checkBoxValues[0].Value = thisElement.Title;

            checkBoxValues[1].Name = "Subject";
            checkBoxValues[1].Value = thisElement.Info;

            return checkBoxValues;
        }

    }
}