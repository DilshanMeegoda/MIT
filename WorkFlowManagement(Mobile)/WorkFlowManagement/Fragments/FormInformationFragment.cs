using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace WorkFlowManagement.Fragments
{
    public class FormInformationFragment : Fragment
    {
        private RelativeLayout layout;

        public FormInformationFragment(RelativeLayout layoutView)
        {
            layout = layoutView;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = layout;
            return view;
        }
    }
}