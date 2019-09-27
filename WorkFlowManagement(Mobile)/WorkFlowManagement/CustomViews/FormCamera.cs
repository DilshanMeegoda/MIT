using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Provider;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Assist;
using WorkFlowManagement.CustomClasses;
using WorkFlowManagement.Enum;
using WorkFlowManagement.Model;
using Environment = Android.OS.Environment;
using File = Java.IO.File;
using Object = Java.Lang.Object;
using Orientation = Android.Widget.Orientation;
using Path = System.IO.Path;
using Uri = Android.Net.Uri;

namespace WorkFlowManagement.CustomViews
{
    public class FormCamera : LinearLayout
    {
        private RelativeLayout theme;
        private Resources resource;
        private EditText dateDisplay;
        private TextView weekDisplay;
        private Button clearDateButton;
        private DatePickerDialog dateDialog;
        private RelativeLayout dateFrame;
        private ISharedPreferences sharedPreferences;
        private ISharedPreferencesEditor sharedPreferencesEditor;
        private DateTime date;
        private bool isArcheived;
        private InformationPopup Popup;
        private int OwnerID;
        private int VerifierID;
        private RelativeLayout.LayoutParams paramsDate;
        private ReportStatus reportStatus;
        private List<String> imageDownloadArray;
        private Context context;
        // TODO get rid of public static variables
        public static List<GridView> cameraPreviewView;
        public static List<ImageView> cameraIndicatorView;
        public static ImageLoader imageLoader;
        public static ImageFile imageJPGFile;

        public const int IMAGE_INDICATOR_ID = 111198;
        public const int IMAGE_PREVIEW_Header_ID = 134198;
        public const int IMAGE_PREVIEW_Info_ID = 1348765;

        public FormCamera(Context contxt, ReportElement element, int userID, int ownerID, int verifiedID, ReportStatus reportStatus,
            string typeFlag, string type, List<ReportElement> elementList)
            : base(contxt)
        {
            imageDownloadArray = new List<string>();
            if (cameraPreviewView == null)
            {
                cameraPreviewView = new List<GridView>();
            }
            else
            {
                for (int i = 0; i < cameraPreviewView.Count; i++)
                {
                    if (IMAGE_PREVIEW_Header_ID + element.Id == cameraPreviewView[i].Id || IMAGE_PREVIEW_Info_ID + element.Id == cameraPreviewView[i].Id)
                    {
                        cameraPreviewView.RemoveAt(i);
                    }
                }
            }

            if (cameraIndicatorView == null)
            {
                cameraIndicatorView = new List<ImageView>();
            }
            else
            {
                for (int i = 0; i < cameraIndicatorView.Count; i++)
                {
                    if (IMAGE_INDICATOR_ID + element.Id == cameraIndicatorView[i].Id)
                    {
                        cameraIndicatorView.RemoveAt(i);
                    }
                }
            }

            imageJPGFile = new ImageFile();

            context = contxt;
            resource = context.Resources;

            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(context);
            sharedPreferencesEditor = sharedPreferences.Edit();

            InitImageLoader();

            LinearLayout imageLay = new LinearLayout(context);
            imageLay.Orientation = Orientation.Vertical;

            var addImageButton = new ImageButton(context);
            addImageButton.Id = element.Id;
            addImageButton.SetPadding(20, 5, 5, 5);

            RelativeLayout.LayoutParams paramsForImageButton = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
            paramsForImageButton.AddRule(LayoutRules.AlignParentLeft);
            addImageButton.LayoutParameters = paramsForImageButton;

            GridView gridGallery = new ExpandingGrid(context);

            RelativeLayout.LayoutParams paramsForgridGallery = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.WrapContent);
            paramsForgridGallery.AddRule(LayoutRules.Above, addImageButton.Id);
            gridGallery.LayoutParameters = paramsForgridGallery;
            gridGallery.SetNumColumns(6);

            List<CustomGallery> dataT = new List<CustomGallery>();
            string sdCardPath2 = Environment.ExternalStorageDirectory.AbsolutePath;

            foreach (var VARIABLE in element.Values)
            {
                CustomGallery item = new CustomGallery();
                item.SdCardPath = "/storage/emulated/0/Checkd/" + VARIABLE.Value;
                dataT.Add(item);

                String fileExist = Path.Combine(sdCardPath2, "Checkd/" + VARIABLE.Value);
                File existFile = new File(fileExist);
                if (!existFile.Exists())
                {
                    imageDownloadArray.Add(VARIABLE.Value);
                }
            }

            MultipleImageDownloader(imageDownloadArray);

            MiniGallerAdapter adapter = new MiniGallerAdapter(Application.Context, imageLoader);
            if (dataT.Count != 0)
            {
                adapter.AddAll(dataT);
            }
            gridGallery.Adapter = adapter;

            if (typeFlag == "Info")
            {
                int artificial_Preview_ID = IMAGE_PREVIEW_Info_ID + element.Id;
                gridGallery.Id = artificial_Preview_ID;
                cameraPreviewView.Add(gridGallery);
            }
            else
            {
                int artificial_header_Preview_ID = IMAGE_PREVIEW_Header_ID + element.Id;
                gridGallery.Id = artificial_header_Preview_ID;
                cameraPreviewView.Add(gridGallery);
            }

            gridGallery.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs args)
            {
                List<CustomGallery> items = new List<CustomGallery>();
                MiniGallerAdapter adapter3 = (MiniGallerAdapter)gridGallery.Adapter;
                items = adapter3.getList();
                Object obj = items[args.Position].SdCardPath;
                String value = obj.ToString().Substring(obj.ToString().LastIndexOf('/') + 1);
                GalleryImagePreview(value);
            };

            imageLay.AddView(gridGallery);
            imageLay.AddView(addImageButton);

            RelativeLayout theme = new FormTheme(context, element.Title);
            LinearLayout btnLayer = new LinearLayout(context);
            btnLayer.Orientation = Orientation.Vertical;

            String img = element.Value;

            String sdCardPath = Environment.ExternalStorageDirectory.AbsolutePath;
            String filePath = Path.Combine(sdCardPath, "Checkd/" + img);
            File file = new File(filePath);

            addImageButton.SetImageResource(Resource.Drawable.android_camera_grey);
            addImageButton.SetBackgroundResource(0);

            ImageView indicatorImage = (ImageView)theme.GetChildAt(1);
            activateElementInfo(element, theme);
            int artificial_ID = IMAGE_INDICATOR_ID + element.Id;
            indicatorImage.Id = artificial_ID;
            cameraIndicatorView.Add(indicatorImage);

            addImageButton.Click += (sender2, e) => ImageSelectionChoiceDialog(sender2, e, element.Id + "", type);

            if (!element.Value.Equals(""))
            {
                indicatorImage.SetImageResource(Resource.Drawable.checked_forms_create_project_medium);
            }

            if (ownerID == 0 || ownerID == userID)
            {
                if (verifiedID != 0)
                {
                    addImageButton.Enabled = false;
                    addImageButton.Clickable = false;

                    if (reportStatus == ReportStatus.Rejected)
                    {
                        addImageButton.Enabled = true;
                        addImageButton.Clickable = true;
                    }
                }

                else
                {
                    addImageButton.Enabled = true;
                    addImageButton.Clickable = true;
                }
            }
            else
            {
                addImageButton.Enabled = false;
                addImageButton.Clickable = false;
            }

            isArcheived = sharedPreferences.GetBoolean(Resources.GetString(Resource.String.is_archived), false);

            if (isArcheived)
            {
                addImageButton.Clickable = false;
                addImageButton.Enabled = false;
                addImageButton.Click += null;
            }

            btnLayer.AddView(theme);
            btnLayer.AddView(imageLay);
            btnLayer.SetPadding(45, 10, 45, 20);
            AddView(btnLayer);
        }

        public async Task DownLoadSingleImage(String image)
        {
            //HttpResponseMessage message;
            //try
            //{
            //    message = await reportDataService.GetImage(image);

            //    var bitmap = await BitmapFactory.DecodeStreamAsync(await message.Content.ReadAsStreamAsync());
            //    String sdCardPath = Environment.ExternalStorageDirectory.AbsolutePath;
            //    String filePath = Path.Combine(sdCardPath, "Checkd/" + image);

            //    FileStream out2 = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
            //    bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, out2);
            //    out2.Close();
            //}
            //catch (Exception e)
            //{
            //    Log.Debug("Exception", e.Message);
            //}
        }

        private async Task MultipleImageDownloader(List<String> images)
        {
            for (int l = 0; l < images.Count; l++)
            {
                await DownLoadSingleImage(images[l]);
            }
        }

        private void GalleryImagePreview(string name)
        {
            switch (name)
            {
                case "":
                    Toast.MakeText(context, "Please Wait while Image is being Synced", ToastLength.Short).Show();
                    break;
                default:
                    {
                        String sdCardPath = Environment.ExternalStorageDirectory.AbsolutePath;
                        String filePath = Path.Combine(sdCardPath, "Checkd/" + name);

                        File file = new File(filePath);
                        if (file.Exists())
                        {
                            Uri pdfPath = Uri.FromFile(new File(filePath));
                            Intent intent = new Intent(Intent.ActionView);
                            intent.SetDataAndType(pdfPath, "image/*");
                            intent.SetFlags(ActivityFlags.NewTask);
                            context.StartActivity(intent);
                        }
                        else
                        {
                            Toast.MakeText(context, "There is no photo available to show", ToastLength.Short).Show();
                        }
                    }
                    break;
            }
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
                info.Click += (sender2, e) => showInfo(sender2, e, element.Info);
            }
        }

        private void showInfo(object sender, EventArgs eventArgs, string information)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(context);
            builder.SetMessage(information);

            builder.SetPositiveButton("Ok", (senderAlert, args) =>
            {
                builder.Dispose();
            });
            builder.Show();
        }

        private void ImageSelectionChoiceDialog(object sender, EventArgs eventArgs, string id, string type)
        {
            sharedPreferencesEditor.PutString("ImageButtonID", id);
            sharedPreferencesEditor.PutString("ImageButtonType", type);
            sharedPreferencesEditor.Commit();

            var builder = new Android.Support.V7.App.AlertDialog.Builder(context);
            builder.SetMessage(resource.GetString(Resource.String.PhotoOption));
            builder.SetCancelable(true);
            builder.SetPositiveButton(resource.GetString(Resource.String.Camera),
                (sender2, e) => GetImageFromCamera(sender2, e, id, type));
            builder.SetNegativeButton(resource.GetString(Resource.String.Galery),
                (sender2, e) => GetImageFromGallery());
            builder.Show();
        }

        private void GetImageFromCamera(object sender, EventArgs eventArgs, string id, string type)
        {
            sharedPreferencesEditor.PutInt("PhotoOption", 1).Commit();
            if (IsThereAnAppToTakePictures())
            {
                String sdCardPath = Environment.ExternalStorageDirectory.AbsolutePath;
                CreateDirectoryForPictures();

                String filePath = Path.Combine(sdCardPath, "Checkd/checkd_" + Guid.NewGuid() + ".jpg");
                Intent intent = new Intent(MediaStore.ActionImageCapture);
                imageJPGFile._file = new File(filePath);

                intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(imageJPGFile._file));
                ((Activity)context).StartActivityForResult(intent, 1);
            }
        }

        private void GetImageFromGallery()
        {
            sharedPreferencesEditor.PutInt("PhotoOption", 0).Commit();
            Intent intent = new Intent("luminous.ACTION_MULTIPLE_PICK");
            ((Activity)context).StartActivityForResult(intent, 1);
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities = context.PackageManager.QueryIntentActivities(intent,
                PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

        private void CreateDirectoryForPictures()
        {
            imageJPGFile._dir = new File(Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures), resource.GetString(Resource.String.ImageFolder));
            if (!imageJPGFile._dir.Exists())
            {
                imageJPGFile._dir.Mkdirs();
            }
        }

        private void InitImageLoader()
        {
            DisplayImageOptions defaultOptions =
                new DisplayImageOptions.
                    Builder().CacheOnDisk(true).
                    ImageScaleType(ImageScaleType.ExactlyStretched).
                    BitmapConfig(Bitmap.Config.Rgb565).
                    Build();

            ImageLoaderConfiguration.Builder builder =
                new ImageLoaderConfiguration.Builder(Application.Context).DefaultDisplayImageOptions(defaultOptions);

            ImageLoaderConfiguration config = builder.Build();
            imageLoader = ImageLoader.Instance;
            imageLoader.Init(config);
        }
    }
}