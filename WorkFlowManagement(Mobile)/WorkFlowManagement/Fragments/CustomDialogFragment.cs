using System;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Util;
using Android.Views;

namespace WorkFlowManagement.Fragments
{
    public class CustomDialogFragment : Android.Support.V4.App.DialogFragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Android 3.x+ still wants to show title: disable
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);

            // CHANGE TO YOUR DIALOG LAYOUT or VIEW CREATION CODE
            return inflater.Inflate(Resource.Layout.loading_view, container, true);
        }

        public override void OnResume()
        {
            try
            {
                // Auto size the dialog based on it's contents
                Dialog.Window.SetLayout(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

                // Make sure there is no background behind our view
                Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            }

            catch (Exception)
            {
                Log.Debug("CustomDialogFragment Crash on Resume", "CustomDialogFragment");
            }

            // Disable standard dialog styling/frame/theme: our custom view should create full UI
            SetStyle(StyleNoFrame, Android.Resource.Style.Theme);

            base.OnResume();
        }
    }
}