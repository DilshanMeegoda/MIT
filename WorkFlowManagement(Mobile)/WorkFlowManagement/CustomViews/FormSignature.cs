using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Preferences;
using Android.Provider;
using Android.Util;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using WorkFlowManagement.Activities;
using WorkFlowManagement.Enum;
using WorkFlowManagement.Model;
using Orientation = Android.Widget.Orientation;
using Path = System.IO.Path;
using Uri = Android.Net.Uri;
using Environment = Android.OS.Environment;
using File = Java.IO.File;

namespace WorkFlowManagement.CustomViews
{
    public class FormSignature : LinearLayout
    {
        private Context contextx;
        public const int SIGNATURE_INDICATOR_ID = 300007;
        // TODO get rid of public static variables
        public static List<ImageView> cameraIndicatorView;
        public static List<GridView> cameraPreviewView;
        private bool isArcheived;
        //private ReportDataService reportDataService;

        public FormSignature(Context context, ReportElement element, string section, int userID, int ownerID, int verifierID, string formType,
            ReportStatus statusReport, List<ReportElement> elementList)
            : base(context)
        {
            contextx = context;
            ISharedPreferences sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(context);
            ISharedPreferencesEditor sharedPreferencesEditor = sharedPreferences.Edit();
            cameraIndicatorView = new List<ImageView>();
            cameraPreviewView = new List<GridView>();
            //reportDataService = rds;

            RelativeLayout theme = new FormTheme(context, element.Title);

            LinearLayout btnLayer = new LinearLayout(context);
            btnLayer.Orientation = Orientation.Vertical;

            var sign = new ImageButton(context);
            sign.Id = element.Id;

            var line = new ImageView(context);
            line.SetBackgroundResource(Resource.Drawable.dottedlines);

            LayoutParams parmsofline = new LayoutParams(
                ViewGroup.LayoutParams.MatchParent, 1);
            line.LayoutParameters = parmsofline;

            LayoutParams parms = new LayoutParams(ViewGroup.LayoutParams.WrapContent, 300);
            sign.LayoutParameters = parms;

            string img = element.Value;

            string sdCardPath = Environment.ExternalStorageDirectory.AbsolutePath;
            string sigPath = Path.Combine(sdCardPath, "Checkd/" + img);
            File file = new File(sigPath);

            sign.SetBackgroundResource(Resource.Drawable.ic_signature);
            //sign.SetBackgroundResource(0);

            if (img == "empty" || string.IsNullOrEmpty(img))
            {
                sign.Tag = "";
            }
            else
            {
                sign.Tag = img;
                if (file.Exists())
                {
                    SetImage(sigPath, sign);
                }
                else
                {
                    DownloadImage(img, sigPath, sign);
                }
            }

            ImageView indicatorImage = (ImageView)theme.GetChildAt(1);
            activateElementInfo(element, theme);
            int artificial_ID = SIGNATURE_INDICATOR_ID + element.Id;
            indicatorImage.Id = artificial_ID;
            cameraIndicatorView.Add(indicatorImage);
            cameraPreviewView.Add(new GridView(contextx));

            if (!element.Value.Equals(""))
            {
                indicatorImage.SetImageResource(Resource.Drawable.checked_forms_create_project_medium);
            }

            if (ownerID == 0 || ownerID == userID)
            {
                if (verifierID != 0)
                {
                    sign.Enabled = false;
                    sign.Clickable = false;

                    if (statusReport == ReportStatus.Rejected)
                    {
                        sign.Enabled = true;
                        sign.Clickable = true;
                    }
                }

                else
                {
                    sign.Enabled = true;
                    sign.Clickable = true;
                }
            }
            else
            {
                sign.Enabled = false;
                sign.Clickable = false;
            }

            sign.Click += (sender, e) =>
            {
                sharedPreferencesEditor.PutString("ImageButtonID", element.Id + "");
                sharedPreferencesEditor.PutString("ImageButtonType", section);
                sharedPreferencesEditor.Commit();

                string filePath = Path.Combine(sdCardPath, "Checkd/signature_" + Guid.NewGuid() + ".jpg");
                Intent intent = new Intent(context, typeof(SignatureActivity));
                intent.PutExtra("URI", filePath);
                intent.PutExtra("ElementList", JsonConvert.SerializeObject(elementList));
                ((Activity)contextx).StartActivityForResult(intent, 2);
            };


            isArcheived = sharedPreferences.GetBoolean(Resources.GetString(Resource.String.is_archived), false);

            if (isArcheived)
            {
                sign.Clickable = false;
            }

            btnLayer.AddView(theme);
            btnLayer.AddView(sign);
            btnLayer.AddView(line);
            btnLayer.SetPadding(45, 10, 45, 20);

            AddView(btnLayer);
        }

        private async void DownloadImage(string img, string imgPath, ImageButton button)
        {
            await DownLoadSingleImage(img);
            SetImage(imgPath, button);
        }

        private void SetImage(string imgPath, ImageButton button)
        {
            Bitmap bitmap = MediaStore.Images.Media.GetBitmap(contextx.ContentResolver, Uri.Parse("file:///" + imgPath));
            BitmapDrawable bitmapDrawable = new BitmapDrawable(Resources, bitmap);
            button.SetBackgroundDrawable(bitmapDrawable);
        }

        private void activateElementInfo(ReportElement element, RelativeLayout theme)
        {
            LinearLayout descriptionHolder = (LinearLayout)theme.GetChildAt(0);
            ImageButton info = (ImageButton)descriptionHolder.GetChildAt(1);

            if (string.IsNullOrEmpty(element.Info))
            {
                info.Visibility = ViewStates.Invisible;
                //info.Visibility = ViewStates.Gone;
            }
            else
            {
                info.Click += (sender2, e) => ShowInfo(element.Info);
            }
        }

        private void ShowInfo(string information)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(Context);
            builder.SetMessage(information);

            builder.SetPositiveButton("Ok", (senderAlert, args) =>
            {
                builder.Dispose();
            });
            builder.Show();
        }

        public async Task DownLoadSingleImage(string image)
        {
            //HttpResponseMessage message;
            //try
            //{
            //    message = await reportDataService.GetImage(image);

            //    var bitmap = await BitmapFactory.DecodeStreamAsync(await message.Content.ReadAsStreamAsync());
            //    string sdCardPath = Environment.ExternalStorageDirectory.AbsolutePath;
            //    string filePath = Path.Combine(sdCardPath, "Checkd/" + image);

            //    FileStream out2 = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
            //    bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, out2);
            //    out2.Close();
            //}
            //catch (Exception e)
            //{
            //    Log.Debug("Exception", e.Message);
            //}
        }
    }
}