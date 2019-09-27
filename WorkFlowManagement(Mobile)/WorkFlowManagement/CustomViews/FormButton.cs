using Android.Content;
using Android.Content.Res;
using Android.Widget;
using WorkFlowManagement.Model;
using Orientation = Android.Widget.Orientation;

namespace WorkFlowManagement.CustomViews
{
    public class FormButton : LinearLayout
    {
        private RelativeLayout theme;
        private Resources resource;
        private int OwnerID;
        private int VerifierID;

        public FormButton(Context context, ReportElement element, int ownerID, int verifiedID)
            : base(context)
        {
            resource = context.Resources;
            theme = new FormTheme(context, element.Title);
            VerifierID = verifiedID;

            Orientation = Orientation.Vertical;
            SetPadding(45, 10, 45, 20);

            Button button = new Button(context);
            button.Text = element.Title;
            button.Id = element.Id;
            button.Click += (sender, e) =>
            {
                //ToDo
            };
            AddView(theme);
            AddView(button);
        }
    }
}