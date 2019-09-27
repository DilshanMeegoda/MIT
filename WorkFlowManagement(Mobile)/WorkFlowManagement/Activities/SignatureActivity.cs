using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace WorkFlowManagement.Activities
{
    [Activity(Label = "SignatureActivity")]
    public class SignatureActivity : Activity
    {
        System.Drawing.PointF[] points;
        private Android.Content.Res.Resources resource;
        private Context context;
        private string imagePath;
        private string list;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.activity_signature_view);
        }
    }
}