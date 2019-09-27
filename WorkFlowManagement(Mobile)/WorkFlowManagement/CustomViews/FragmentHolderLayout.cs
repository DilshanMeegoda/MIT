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
    public class FragmentHolderLayout : LinearLayout
    {
        private Activities.FormActivity formActivity;
        private AbsListView.LayoutParams lpView;

        public FragmentHolderLayout(Context context) : base(context)
        {
            DescendantFocusability = DescendantFocusability.BeforeDescendants;
            FocusableInTouchMode = true;
            SetBackgroundColor(Color.White);
            Orientation = Orientation.Vertical;
            SetPadding(10, 0, 10, 0);
        }

        //public FragmentHolderLayout(Context context, AbsListView.LayoutParams linLayoutParam)
        //    : base(context)
        //{
        //    //this.formActivity = formActivity;
        //    //this.lpView = lpView;
        //    DescendantFocusability = DescendantFocusability.BeforeDescendants;
        //    FocusableInTouchMode = true;
        //    SetBackgroundColor(Color.White);
        //    Orientation = Orientation.Vertical;
        //    SetPadding(10, 0, 10, 0);
        //    LayoutParameters = linLayoutParam;
        //}
    }
}