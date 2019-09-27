using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Orientation = Android.Widget.Orientation;

namespace WorkFlowManagement.CustomViews
{
    public class FormTheme : RelativeLayout
    {
        public FormTheme(Context context, string title) : base(context)
        {
            LayoutParams thisLayoutParameters = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            LayoutParameters = thisLayoutParameters;

            TextView descriptionText = new TextView(Application.Context);
            ImageView indicatorImage = new ImageView(Application.Context);
            ImageButton infoButton = new ImageButton(Application.Context);
            LinearLayout descriptionHolder = new LinearLayout(Application.Context);

            descriptionHolder.Orientation = Orientation.Horizontal;

            descriptionText.Text = title;
            descriptionText.SetPadding(0, 20, 0, 0);
            descriptionText.SetTextColor(Color.ParseColor(context.Resources.GetString(Resource.Color.green_primary)));

            infoButton.SetImageResource(Resource.Drawable.form_info);
            infoButton.SetBackgroundResource(0);

            LayoutParams helpParameters = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            infoButton.LayoutParameters = helpParameters; 

            descriptionHolder.AddView(descriptionText);
            descriptionHolder.AddView(infoButton);

            indicatorImage.SetPadding(50, 20, 0, 0);

            LayoutParams indicatorParameters = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            indicatorParameters.AddRule(LayoutRules.AlignParentRight);
            indicatorImage.LayoutParameters = indicatorParameters;

            AddView(descriptionHolder);
            AddView(indicatorImage);

        }
    }
}