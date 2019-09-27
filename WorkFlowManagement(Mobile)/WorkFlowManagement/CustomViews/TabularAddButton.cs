using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace WorkFlowManagement.CustomViews
{
    public class TabularAddButton : RelativeLayout
    {
        private RelativeLayout buttonHolder;

        public TabularAddButton(Context context) : base(context)
        {

            RelativeLayout.LayoutParams paramsForHolder = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.WrapContent);

            LayoutParameters = paramsForHolder;

            TextView AddText = new TextView(context);
            ImageButton Addbutton = new ImageButton(context);

            AddText.SetPadding(10, 10, 10, 10);
            AddText.Text = "Add";
            AddText.SetTextAppearance(context, Resource.Style.TextAppearance_AppCompat_Medium);
            AddText.SetTextColor(Color.ParseColor("#27AE60"));

            Addbutton.SetBackgroundResource(Resource.Drawable.addbutton);

            RelativeLayout.LayoutParams paramsForAddText = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
            paramsForAddText.AddRule(LayoutRules.AlignParentTop);
            paramsForAddText.AddRule(LayoutRules.AlignParentStart);
            AddText.LayoutParameters = paramsForAddText; //causes layout update

            RelativeLayout.LayoutParams paramsForAddButton = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
            paramsForAddButton.AddRule(LayoutRules.CenterVertical);
            paramsForAddButton.AddRule(LayoutRules.AlignParentEnd);
            Addbutton.LayoutParameters = paramsForAddButton; //causes layout update

            AddView(AddText);
            AddView(Addbutton);
        }
    }
}