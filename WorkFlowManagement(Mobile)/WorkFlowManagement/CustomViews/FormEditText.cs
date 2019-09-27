using Android.Content;
using Android.Graphics;
using Android.Preferences;
using Android.Widget;
using WorkFlowManagement.Enum;
using WorkFlowManagement.Model;

namespace WorkFlowManagement.CustomViews
{
    public class FormEditText : LinearLayout
    {
        public FormEditText(Context context, ReportElement element, int userId, int ownerId, int verifiedId, ReportStatus reportStatus)
               : base(context)
        {
            RelativeLayout theme = new FormTheme(context, element.Title);
            Orientation = Orientation.Vertical;
            ISharedPreferences sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(context);
            ISharedPreferencesEditor sharedPreferencesEditor = sharedPreferences.Edit();

            EditText editText = new EditText(context);
            editText.Id = element.Id;
            editText.Text = element.Value;
            editText.SetSingleLine(true);
            editText.SetBackgroundResource(Resource.Drawable.custom_edit_text_color);
            editText.InputType = Android.Text.InputTypes.TextFlagCapSentences;

            ImageView indicatorImage = (ImageView)theme.GetChildAt(1);

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

            if (ownerId == 0 || ownerId == userId)
            {
                if (verifiedId != 0)
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

            TextView elementSplitLine = new TextView(context);
            elementSplitLine.TextSize = 0.5f;
            elementSplitLine.SetBackgroundColor(Color.ParseColor(context.Resources.GetString(Resource.Color.grey)));


            if (sharedPreferences.GetBoolean(Resources.GetString(Resource.String.is_archived), false))
            {
                editText.Enabled = false;
                editText.SetTextColor(Resources.GetColor(Resource.Color.grey));
            }

            AddView(theme);
            AddView(editText);
            SetPadding(45, 10, 45, 20);
        }
    }
}