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
    public class TabularItemButton : RelativeLayout
    {
        public TabularItemButton(Context context) : base(context)
        {
            RelativeLayout.LayoutParams paramsForHolder = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.WrapContent);

            LayoutParameters = paramsForHolder;

            TextView NameText = new TextView(context);
            TextView numberText = new TextView(context);

            NameText.SetTextAppearance(context, Resource.Style.TextAppearance_AppCompat_Medium);
            NameText.SetTextColor(Color.ParseColor("#000000"));
            NameText.SetPadding(10, 10, 10, 10);

            RelativeLayout.LayoutParams paramsForNameText = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
            paramsForNameText.AddRule(LayoutRules.AlignParentTop);
            paramsForNameText.AddRule(LayoutRules.AlignParentStart);
            NameText.LayoutParameters = paramsForNameText; //causes layout update


            numberText.SetTextAppearance(context, Resource.Style.TextAppearance_AppCompat_Medium);
            numberText.SetTextColor(Color.ParseColor("#ffffff"));
            numberText.SetBackgroundColor(Color.ParseColor("#8f8e94"));
            numberText.SetBackgroundResource(Resource.Drawable.tabularcount);
            numberText.SetPadding(7, 0, 7, 0);
            numberText.Gravity = GravityFlags.Center;

            RelativeLayout.LayoutParams paramsFornumberText = new RelativeLayout.LayoutParams(70, RelativeLayout.LayoutParams.WrapContent);
            paramsFornumberText.AddRule(LayoutRules.AlignParentEnd);
            paramsFornumberText.AddRule(LayoutRules.CenterVertical);
            paramsFornumberText.SetMargins(0, 0, 30, 0);
            numberText.LayoutParameters = paramsFornumberText; //causes layout update

            AddView(NameText);
            AddView(numberText);

        }
    }
}