using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Locations;
using Android.OS;
using Android.Preferences;
using Android.Provider;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Newtonsoft.Json;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Assist;
using WorkFlowManagement.Common;
using WorkFlowManagement.CustomClasses;
using WorkFlowManagement.CustomViews;
using WorkFlowManagement.Enum;
using WorkFlowManagement.Model;
using AlertDialog = Android.App.AlertDialog;
using Environment = Android.OS.Environment;
using Exception = System.Exception;
using File = Java.IO.File;
using Object = Java.Lang.Object;
using Orientation = Android.Widget.Orientation;
using Path = System.IO.Path;
using String = System.String;
using StringBuilder = System.Text.StringBuilder;
using Uri = Android.Net.Uri;

namespace WorkFlowManagement.Activities
{
    [Activity(Label = "TabularActivity", WindowSoftInputMode = SoftInput.AdjustPan, ScreenOrientation = ScreenOrientation.Portrait)]
    public class TabularActivity : AppCompatActivity, ILocationListener
    {
        private UserSession currentSession;
        private List<ReportElement> reportElementList;
        private List<String> ImageUploadArray;
        private List<String> ImageDownloadArray;
        private LinearLayout listOfTabuarElementList;
        private ISharedPreferences sharedPreferences;
        private ISharedPreferencesEditor sharedPreferencesEditor;
        private Context context;
        private bool tabularCheck;
        private LinearLayout mainView;
        private Resources resource;
        private int OwnerID;
        private int verifierID;
        private ReportStatus reportStatus;
        private int userID;
        private int MainID;
        private string type;
        private LocationManager _locationManager;
        private Location _currentLocation;
        private ImageLoader imageLoader;
        private bool isArcheived;
        private List<GridView> cameraPreviewView;
        private List<ImageView> cameraIndicatorView;
        private int photoOption;
        private string sdCardPath;
        private ImageFile imageJPGFile;
        private string activityName;
        private String _locationProvider;
        private ScrollView scroller;
        private LinearLayout.LayoutParams paramsScroller;

        public const int IMAGE_INDICATOR_ID = 111198;
        public const int SIGNATURE_INDICATOR_ID = 300007;
        public const int IMAGE_PREVIEW_ID = 134198;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            context = this;
            //currentSession = new SessionService(new SQLiteAndriod().GetConnection()).GetCurrentSession();
            mainView = new LinearLayout(context);
            mainView.SetBackgroundColor(Color.ParseColor("#ffffff"));
            scroller = new ScrollView(context);
            scroller.SetBackgroundColor(Color.ParseColor("#ffffff"));

            resource = context.Resources;
            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(context);
            sharedPreferencesEditor = sharedPreferences.Edit();
            listOfTabuarElementList = new LinearLayout(context);
            listOfTabuarElementList.Orientation = Orientation.Vertical;
            reportStatus = new ReportStatus();

            cameraPreviewView = new List<GridView>();
            cameraIndicatorView = new List<ImageView>();
            ImageUploadArray = new List<string>();
            ImageDownloadArray = new List<string>();

            sdCardPath = Environment.ExternalStorageDirectory.AbsolutePath;
            imageJPGFile = new ImageFile();

            string jsonObject = Intent.GetStringExtra("JasonTabularData") ?? "Data not available";
            string reportStats = Intent.GetStringExtra("ReportStatus") ?? "Data not available";

            OwnerID = Intent.GetIntExtra("OwnerID", 0);
            verifierID = Intent.GetIntExtra("VerifierID", 0);
            activityName = Intent.GetStringExtra("NameOfTheTabular") ?? "Tabular Activity";
            reportElementList = JsonConvert.DeserializeObject<List<ReportElement>>(jsonObject);
            reportStatus = JsonConvert.DeserializeObject<ReportStatus>(reportStats);

            //userID = currentSession.Id;
            userID = 1;
            string mainID = sharedPreferences.GetString("MainTabularID", "0");
            type = Intent.GetStringExtra("TabularType") ?? "form";

            Intent.ReplaceExtras(new Bundle());
            Intent.SetAction("");
            Intent.SetData(null);
            Intent.SetFlags(0);

            MainID = Integer.ParseInt(mainID);

            Title = activityName;

            InitializeLocationReceivers();

            mainView = InitializeView(reportElementList);
            scroller.AddView(mainView);
            SetContentView(scroller);
        }

        private void InitializeLocationReceivers()
        {
            _locationManager = (LocationManager)GetSystemService(LocationService);

            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };

            IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

            if (acceptableLocationProviders.Any())
            {
                _locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                _locationProvider = String.Empty;
            }
        }

        private LinearLayout InitializeView(List<ReportElement> reportElements)
        {

            for (int i = 0; i < reportElements.Count; i++)
            {
                LinearLayout newLayout = new LinearLayout(context);

                newLayout = createTabularItem(reportElements[i]);
                listOfTabuarElementList.AddView(newLayout);
                listOfTabuarElementList.AddView(ElementSplitLine());
            }

            return listOfTabuarElementList;
        }

        private LinearLayout createTabularItem(ReportElement elements)
        {
            LinearLayout tabElement = ParseAndCreateElement(elements, type, OwnerID);
            return tabElement;
        }

        public LinearLayout ParseAndCreateElement(ReportElement element, String type, int ownerID)
        {
            LinearLayout vW = new LinearLayout(context);

            switch (element.Type)
            {
                case "textfield":
                    vW = createTextBox(element, type, ownerID, verifierID);
                    break;
                case "textfieldint":
                    vW = createFieldInt(element, type, ownerID, verifierID);
                    break;
                case "slider":
                    vW = createSlider(element, type, ownerID, verifierID);
                    break;
                case "signature":
                    vW = createSignature(element, type, ownerID, verifierID);
                    break;
                case "yesno":
                    vW = createYesNo(element, type, ownerID, verifierID);
                    break;
                case "multilinetextfield":
                    vW = createFieldMultiLine(element, type, ownerID, verifierID);
                    break;
                case "date":
                    vW = createDate(element, type, ownerID, verifierID);
                    break;
                case "time":
                    vW = createTime(element, type, ownerID, verifierID);
                    break;
                case "camera":
                    vW = createCamera(element, type, ownerID, verifierID);
                    break;
                case "checkbox":
                    vW = createCheckBox(element, type, ownerID, verifierID);
                    break;
                case "dropdown":
                    vW = createDropDown(element, type, ownerID, verifierID);
                    break;
                case "mainandsubfield":
                    vW = createMultiTextView(element, type, ownerID, verifierID);
                    break;
                case "updown":
                    vW = createupDown(element, type, ownerID, verifierID);
                    break;
                case "gps":
                    vW = createGPS(element, type, ownerID, verifierID);
                    break;
                case "tabularform":
                    vW = createTabularForm(element, type, ownerID, verifierID);
                    break;
            }

            return vW;
        }

        #region Create Elements

        private View ElementSplitLine()
        {
            TextView line = new TextView(context);
            line.TextSize = 0.5f;
            line.SetBackgroundColor(Color.ParseColor(resource.GetString(Resource.Color.grey)));
            return line;
        }

        private LinearLayout createTextBox(ReportElement element, String type, int ownerID, int verifierID)
        {
            LinearLayout fromClass = new FormEditText(context, element, userID, ownerID, verifierID, reportStatus);
            return fromClass;
        }

        private LinearLayout createSlider(ReportElement element, String type, int ownerID, int verifierID)
        {
            LinearLayout fromClass = new FormSlider(context, element, userID, ownerID, verifierID, reportStatus);
            return fromClass;
        }

        private LinearLayout createupDown(ReportElement element, String type, int ownerID, int verifierID)
        {
            LinearLayout fromClass = new FormPlusMinusCounter(context, element, userID, ownerID, verifierID, reportStatus);
            return fromClass;
        }

        private LinearLayout createFieldInt(ReportElement element, String type, int ownerID, int verifierID)
        {
            LinearLayout fromClass = new FormIntEditText(context, element, userID, ownerID, verifierID, reportStatus);
            return fromClass;
        }

        private LinearLayout createYesNo(ReportElement element, String type, int ownerID, int verifierID)
        {
            LinearLayout fromClass = new FormSwitch(context, element, userID, ownerID, verifierID, "", reportStatus, type, null);
            return fromClass;
        }

        private LinearLayout createFieldMultiLine(ReportElement element, String type, int ownerID, int verifierID)
        {
            LinearLayout fromClass = new FormMultiLineEditText(context, element, userID, ownerID, verifierID, reportStatus);
            return fromClass;
        }

        private LinearLayout createDate(ReportElement element, String type, int ownerID, int verifierID)
        {
            LinearLayout fromClass = new FormDate(context, element, userID, ownerID, verifierID, reportStatus);
            return fromClass;
        }

        private LinearLayout createTime(ReportElement element, String type, int ownerID, int verifierID)
        {
            LinearLayout fromClass = new FormTime(context, element, userID, ownerID, verifierID, reportStatus);
            return fromClass;
        }

        private LinearLayout createCheckBox(ReportElement element, String type, int ownerID, int verifierID)
        {
            LinearLayout checkBoxLayout = new FormCheckBox(context, element, userID, ownerID, verifierID, reportStatus, "", type, null);
            checkBoxLayout.Id = element.Id;
            return checkBoxLayout;
        }

        private LinearLayout createDropDown(ReportElement element, String type, int ownerID, int verifierID)
        {
            LinearLayout dropDownLayer = new FormDropDown(context, element, userID, ownerID, verifierID, reportStatus);
            dropDownLayer.Id = element.Id;
            return dropDownLayer;
        }

        private LinearLayout createMultiTextView(ReportElement element, String type, int ownerID, int verifierID)
        {
            LinearLayout fromClass = new FormHeaderSubElement(context, element, userID, verifierID);
            fromClass.Id = element.Id;
            return fromClass;
        }

        private LinearLayout createTabularForm(ReportElement element, String type, int ownerID, int verifierID)
        {
            FormTabular fromClass = new FormTabular(context, element, ownerID, verifierID, type, reportStatus);
            fromClass.Id = element.Id;

            return fromClass;
        }

        private LinearLayout createGPS(ReportElement element, String type, int ownerID, int verifierID)
        {
            FormGPS fromClass = new FormGPS(context, element, ownerID, userID, verifierID, reportStatus);
            fromClass.Id = element.Id;

            RelativeLayout gpsHolder = new RelativeLayout(this);
            gpsHolder = (RelativeLayout)fromClass.GetChildAt(1);
            EditText gpsaddress = (EditText)gpsHolder.GetChildAt(0);
            ImageButton getGPS = (ImageButton)gpsHolder.GetChildAt(1);

            getGPS.Click += (senderGPS, e) => AddressButton_OnClick(gpsaddress, fromClass);

            return fromClass;

        }

        private async void AddressButton_OnClick(EditText addressGPS, FormGPS gpsObject)
        {
            if (Utility.IsGPSAvailable(_locationManager))
            {
                if (_currentLocation == null)
                {
                    addressGPS.Text = "GPS is stabilizing . Please try again in few seconds";
                    return;
                }

                try
                {
                    Geocoder geocoder = new Geocoder(this);
                    IList<Address> addressList =
                        await geocoder.GetFromLocationAsync(_currentLocation.Latitude, _currentLocation.Longitude, 10);
                    Address address = addressList.FirstOrDefault();

                    if (address != null)
                    {
                        StringBuilder deviceAddress = new StringBuilder();
                        for (int i = 0; i < address.MaxAddressLineIndex; i++)
                        {
                            deviceAddress.Append(address.GetAddressLine(i)).Append(",");
                        }
                        addressGPS.Text = deviceAddress.ToString();
                        gpsObject.setGeoCoOrdinates(_currentLocation.Longitude + "", _currentLocation.Latitude + "");
                    }
                    else
                    {
                        addressGPS.Text = "Address not available. You can manually type an Address here";
                    }
                }
                catch (Exception ex)
                {
                    addressGPS.Text = "Address not available at this time, Seems like Internet is turned Off";
                    Log.Debug("GPS Exception", ex.ToString());
                }
            }
        }

        private LinearLayout createCamera(ReportElement element, String type, int ownerID, int verifierID)
        {
            //FormCamera formCamera = new FormCamera(context, element, userID, ownerID, verifierID, reportStatus, typeFlag, reportDataService, type);
            //return formCamera;
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

            RelativeLayout.LayoutParams paramsForgridGallery =
                new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent,
                    RelativeLayout.LayoutParams.WrapContent);
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

                String fileExist = System.IO.Path.Combine(sdCardPath2, "Checkd/" + VARIABLE.Value);
                File existFile = new File(fileExist);
                if (!existFile.Exists())
                {
                    ImageDownloadArray.Add(VARIABLE.Value);
                }
            }

            MultipleImageDownloader(ImageDownloadArray);

            MiniGallerAdapter adapter = new MiniGallerAdapter(Application.Context, imageLoader);
            if (dataT.Count != 0)
            {
                adapter.AddAll(dataT);
            }

            gridGallery.Adapter = adapter;

            int artificial_Preview_ID = IMAGE_PREVIEW_ID + element.Id;
            gridGallery.Id = artificial_Preview_ID;
            cameraPreviewView.Add(gridGallery);

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
                if (verifierID != 0)
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

            return btnLayer;
        }


        private LinearLayout createSignature(ReportElement element, String type, int ownerID, int verifierID)
        {
            RelativeLayout theme = new FormTheme(context, element.Title);

            LinearLayout btnLayer = new LinearLayout(context);
            btnLayer.Orientation = Orientation.Vertical;

            var sign = new ImageButton(context);
            sign.Id = element.Id;

            var line = new ImageView(context);
            line.SetBackgroundResource(Resource.Drawable.dottedlines);

            LinearLayout.LayoutParams parmsofline = new LinearLayout.LayoutParams(
                LinearLayout.LayoutParams.MatchParent, 1);
            line.LayoutParameters = parmsofline;

            LinearLayout.LayoutParams parms = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, 300);
            sign.LayoutParameters = parms;

            String img = element.Value;

            String sdCardPath = Environment.ExternalStorageDirectory.AbsolutePath;
            String sigPath = Path.Combine(sdCardPath, "Checkd/" + img);
            File file = new File(sigPath);

            sign.SetBackgroundResource(Resource.Drawable.ic_signature);

            if (img == "empty" || string.IsNullOrEmpty(img))
            {
                sign.Tag = "";
            }
            else
            {
                sign.Tag = img;

                if (file.Exists())
                {
                    string path = "file:///" + sigPath;
                    Bitmap bitmap = MediaStore.Images.Media.GetBitmap(ContentResolver, Uri.Parse(path));
                    BitmapDrawable ob = new BitmapDrawable(Resources, bitmap);
                    sign.SetBackgroundDrawable(ob);
                }
                else
                {
                    DownLoadSingleImage(img);
                    string path = "file:///" + sigPath;
                    Bitmap bitmap = MediaStore.Images.Media.GetBitmap(ContentResolver, Uri.Parse(path));
                    BitmapDrawable ob = new BitmapDrawable(Resources, bitmap);
                    sign.SetBackgroundDrawable(ob);
                }

            }

            ImageView indicatorImage = (ImageView)theme.GetChildAt(1);
            activateElementInfo(element, theme);
            int artificial_ID = SIGNATURE_INDICATOR_ID + element.Id;
            indicatorImage.Id = artificial_ID;
            cameraIndicatorView.Add(indicatorImage);

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


                    if (reportStatus == ReportStatus.Rejected)
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
                sharedPreferencesEditor.PutString("ImageButtonType", type);
                sharedPreferencesEditor.Commit();

                String filePath = Path.Combine(sdCardPath, "Checkd/signature_" + Guid.NewGuid() + ".jpg");
                Intent intent = new Intent(context, typeof(SignatureActivity));
                intent.PutExtra("URI", filePath);
                StartActivityForResult(intent, 2);
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

            return btnLayer;
        }

        #endregion

        private List<ReportElement> parseSingleTabularForm(List<ReportElement> MainElementList, LinearLayout MainViewList)
        {
            List<ReportElement> dataToReturn = new List<ReportElement>();

            for (int i = 0; i < MainElementList.Count; i++)
            {
                SetFormElementData(MainElementList[i], MainViewList);
                dataToReturn.Add(MainElementList[i]);
            }
            return dataToReturn;
        }

        private void SetFormElementData(ReportElement element, View layout)
        {
            string type = element.Type;
            int vID = element.Id;
            String vName = element.Title;

            if (type == "checkbox" || type == "dropdown" || type == "mainandsubfield" || type == "camera" ||
                type == "gps" || type == "tabularform")
            {
                if (type == "mainandsubfield")
                {
                    FormHeaderSubElement infoDropdownElenent = layout.FindViewById<FormHeaderSubElement>(vID);
                    if (infoDropdownElenent != null)
                    {
                        element.Values = infoDropdownElenent.HeaderSubValues();
                    }
                }

                if (type == "dropdown")
                {
                    FormDropDown headerDropdownElement = layout.FindViewById<FormDropDown>(vID);
                    if (headerDropdownElement != null)
                    {
                        element.Values = headerDropdownElement.DropDownBoxValues();
                    }
                }
                if (type == "checkbox")
                {
                    FormCheckBox headercheckboxelement = layout.FindViewById<FormCheckBox>(vID);
                    if (headercheckboxelement != null)
                    {
                        element.Values = headercheckboxelement.CheckBoxValues();
                    }

                    if (element.IsMultiSelect)
                    {
                        int checkSum = 0;
                        for (int j = 0; j < element.Values.Count; j++)
                        {
                            if (element.Values[j].Value.Equals("true") && element.Values[j].Condition)
                            {
                                for (int k = 0; k < element.Values[j].Child[0].Count; k++)
                                {
                                    SetFormElementData(element.Values[j].Child[0][k], layout);
                                }
                                checkSum += element.Values[j].Child[0].Count;
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < element.Values.Count; j++)
                        {
                            if (element.Values[j].Value.Equals("true") && element.Values[j].Condition)
                            {
                                for (int k = 0; k < element.Values[j].Child[0].Count; k++)
                                {
                                    SetFormElementData(element.Values[j].Child[0][k], layout);
                                }
                            }
                        }
                    }
                }

                if (type == "gps")
                {
                    FormGPS headerGPSelement = layout.FindViewById<FormGPS>(vID);
                    if (headerGPSelement != null && _currentLocation != null)
                    {
                        headerGPSelement.setGeoCoOrdinates(_currentLocation.Longitude + "", _currentLocation.Latitude + "");
                        element.Values = headerGPSelement.getGPSData();
                    }
                }

                //if (type == "tabularform")
                //{
                //    FormTabular headerTabularelement = layout.FindViewById<FormTabular>(vID);
                //    element.Child = headerTabularelement.getTabularData();
                //}

                if (type == "camera")
                {
                    List<CustomGallery> cusGal = new List<CustomGallery>();
                    foreach (var grid in cameraPreviewView)
                    {
                        if (grid.Id == vID + IMAGE_PREVIEW_ID)
                        {
                            MiniGallerAdapter adapterheader = (MiniGallerAdapter)grid.Adapter;
                            cusGal = adapterheader.getList();
                        }
                    }

                    List<KeyValue> imageAddress = new List<KeyValue>();
                    foreach (var item in cusGal)
                    {
                        // imageValues.Value = item.SdCardPath;
                        KeyValue imageValues = new KeyValue();
                        imageValues.Value = item.SdCardPath.Substring(item.SdCardPath.LastIndexOf("/") + 1);
                        imageAddress.Add(imageValues);
                    }
                    element.Values = imageAddress;
                }
            }
            else
            {
                element.Value = GetProcessedDataFromForm(type, vID, layout);

                if (type.Equals("yesno") && element.Options.First(a => a.Code == "conditional").Value == "true")
                {
                    if (element.Value.Equals("true"))
                    {
                        for (int j = 0; j < element.Accepted[0].Count; j++)
                        {
                            SetFormElementData(element.Accepted[0][j], layout);
                        }
                    }
                    else
                    {
                        for (int j = 0; j < element.Rejected[0].Count; j++)
                        {
                            SetFormElementData(element.Rejected[0][j], layout);
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(element.Value))
            {
                if (string.IsNullOrEmpty(element.FilledBy))
                {
                    element.FilledBy = userID + "";
                }
            }
        }

        private String GetProcessedDataFromForm(String type, int id, View reportElementsList)
        {
            String results = "";

            switch (type)
            {
                case "Button":
                    results = "Button";
                    break;

                case "slider":
                    var et2 = new SeekBar(context);
                    et2 = reportElementsList.FindViewById<SeekBar>(id);
                    switch (et2.Progress)
                    {
                        case 0:
                            results = "";
                            break;
                        default:
                            results = et2.Progress + "";
                            break;
                    }
                    break;

                case "textfieldint":
                    var et3 = new EditText(context);
                    et3 = reportElementsList.FindViewById<EditText>(id);
                    results = et3.Text;
                    break;

                case "textfield":
                    var et4 = new EditText(context);
                    et4 = reportElementsList.FindViewById<EditText>(id);
                    results = et4.Text;
                    break;

                case "multilinetextfield":
                    var et5 = new EditText(context);
                    et5 = reportElementsList.FindViewById<EditText>(id);
                    results = et5.Text;
                    break;

                case "yesno":
                    var et6 = new Switch(context);
                    et6 = reportElementsList.FindViewById<Switch>(id);
                    results = et6.Text != "Set" ? (et6.Checked ? "true" : "false") : "";
                    break;

                case "date":
                    var et7 = new EditText(context);
                    et7 = reportElementsList.FindViewById<EditText>(id);
                    string date = et7.Text;

                    try
                    {
                        DateTime dt = Convert.ToDateTime(date);

                        string inputString = dt.ToString("dd.MMM.yyyy");
                        ;
                        DateTime dDate;

                        if (DateTime.TryParse(inputString, out dDate))
                        {
                            String.Format("{0:dd.MMM.yyyy}", dDate);
                            results = string.IsNullOrEmpty(date) ? "" : dt.ToString("dd.MMM.yyyy");
                        }
                        else
                        {
                            Console.WriteLine("Invalid"); // <-- Control flow goes here
                            results = "";
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Debug("Date Bug", ex.ToString());
                        results = "";
                    }

                    break;

                case "time":
                    var et8 = new EditText(context);
                    et8 = reportElementsList.FindViewById<EditText>(id);
                    results = et8.Text;
                    break;

                case "datetime":
                    var et9 = new TextView(context);
                    et9 = reportElementsList.FindViewById<TextView>(id);
                    results = et9.Text;
                    break;

                case "signature":
                    var et10 = new ImageButton(context);
                    et10 = reportElementsList.FindViewById<ImageButton>(id);

                    if (et10.Tag != null)
                    {
                        results = et10.Tag.ToString();
                        string val = results;
                    }
                    else
                    {
                        results = "empty";
                    }
                    break;

                case "camera":
                    results = "camera";
                    break;

                case "checkbox":
                    results = "checkbox";
                    break;

                case "dropdown":
                    results = "dropdown";
                    break;
                case "mainandsubfield":
                    results = "mainsubfield";
                    break;
                case "updown":
                    var et12 = new EditText(context);
                    et12 = reportElementsList.FindViewById<EditText>(id);
                    switch (Int32.Parse(et12.Text))
                    {
                        case 0:
                            results = "";
                            break;
                        default:
                            results = et12.Text + "";
                            break;
                    }
                    break;
            }
            return results;
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();

            List<ReportElement> ReportListElementwithData = parseSingleTabularForm(reportElementList, listOfTabuarElementList);
            String json = JsonConvert.SerializeObject(ReportListElementwithData);
            List<String> jsonImages = ImageUploadArray;

            //ImageUploadArray

            sharedPreferencesEditor.PutBoolean("FromTabular", true);
            sharedPreferencesEditor.PutString("MainTabularID", MainID + "");
            sharedPreferencesEditor.PutString("MainTabularData", json);
            sharedPreferencesEditor.PutStringSet("MainTabularImages", jsonImages);

            sharedPreferencesEditor.Commit();

            Finish();
        }

        protected override void OnResume()
        {
            base.OnResume();

            tabularCheck = sharedPreferences.GetBoolean("FromTabular", false);

            if (tabularCheck)
            {
                List<String> x = new List<string>();
                string id = sharedPreferences.GetString("MainTabularID", "");
                string data = sharedPreferences.GetString("MainTabularData", "");
                List<String> upImages = sharedPreferences.GetStringSet("MainTabularImages", new Collection<string>()).ToList();
                ImageUploadArray.AddRange(upImages);

                for (int i = 0; i < reportElementList.Count; i++)
                {
                    if (reportElementList[i].Type.Equals("tabularform") && reportElementList[i].Id + "" == id)
                    {
                        FormTabular tform = listOfTabuarElementList.FindViewById<FormTabular>(Integer.ParseInt(id));
                        tform.UpdateMainTabular(data);
                        //  tform.UpDateImagess(upImages);
                    }
                }
            }


            sharedPreferencesEditor.PutBoolean("FromTabular", true);
            sharedPreferencesEditor.PutString("MainTabularID", MainID + "");
            sharedPreferencesEditor.PutString("MainTabularData", "");

            sharedPreferencesEditor.Commit();

            try
            {
                _locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
            }
            catch (Exception)
            {

            }

        }


        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {

            base.OnActivityResult(requestCode, resultCode, data);

            switch (resultCode)
            {
                case Result.Ok:
                    {
                        switch (requestCode)
                        {
                            case 1:
                                {
                                    string id = sharedPreferences.GetString("ImageButtonID", "");
                                    string type = sharedPreferences.GetString("ImageButtonType", "");

                                    for (int i = 0; i < reportElementList.Count; i++)
                                    {
                                        var et9 = new GridView(this);
                                        var indicatorImage = new ImageView(this);

                                        if (reportElementList[i].Type.Equals("camera") &&
                                            reportElementList[i].Id + "" == id)
                                        {
                                            int artificialID = reportElementList[i].Id + IMAGE_INDICATOR_ID;

                                            for (int x = 0; x < cameraIndicatorView.Count; x++)
                                            {
                                                if (cameraIndicatorView[x].Id == artificialID)
                                                {
                                                    indicatorImage = cameraIndicatorView[x];
                                                    et9 = cameraPreviewView[x];
                                                    break;
                                                }
                                            }

                                            switch (photoOption)
                                            {
                                                case 0:
                                                    ProcessImageFromGallery(data, et9, indicatorImage);
                                                    break;
                                                default:
                                                    ProcessImageFromCamera(et9, indicatorImage);
                                                    break;
                                            }
                                        }
                                    }
                                    break;
                                }

                            case 2:
                                {

                                    string id = sharedPreferences.GetString("ImageButtonID", "");
                                    string type = sharedPreferences.GetString("ImageButtonType", "");

                                    for (int i = 0; i < reportElementList.Count; i++)
                                    {
                                        var et11 = new ImageButton(this);
                                        var indicatorImage = new ImageView(this);

                                        if (reportElementList[i].Type.Equals("signature") &&
                                            reportElementList[i].Id + "" == id)
                                        {
                                            et11 = listOfTabuarElementList.FindViewById<ImageButton>(reportElementList[i].Id);
                                            int artificialID = reportElementList[i].Id + SIGNATURE_INDICATOR_ID;

                                            int artificial_Preview_id = reportElementList[i].Id +
                                                                        IMAGE_PREVIEW_ID;

                                            for (int x = 0; x < cameraIndicatorView.Count; x++)
                                            {
                                                if (cameraIndicatorView[x].Id == artificialID)
                                                {
                                                    indicatorImage = cameraIndicatorView[x];
                                                    break;
                                                }
                                            }

                                            ProcessImageFromGallerytpSignature(data, et11, indicatorImage);
                                        }
                                    }
                                    break;
                                }
                        }
                    }
                    break;
            }
        }

        #region GPS Controlls

        public void OnLocationChanged(Location location)
        {
            _currentLocation = location;
            if (_currentLocation == null)
            {
                // _locationText.Text = "Unable to determine your location.";
                //Log.Debug("OnLocation Changed", "Unable to determine your location.");
            }
            else
            {
                // _locationText.Text = String.Format("{0},{1}", _currentLocation.Latitude, _currentLocation.Longitude);
                //Log.Debug("OnLocation Changed",String.Format("{0},{1}", _currentLocation.Latitude, _currentLocation.Longitude));
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            _locationManager.RemoveUpdates(this);
        }

        public void OnProviderDisabled(string provider)
        {
        }

        public void OnProviderEnabled(string provider)
        {
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
        }

        #endregion

        #region Camera Controlls

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

        private async Task MultipleImageDownloader(List<String> images)
        {
            for (int l = 0; l < images.Count; l++)
            {
                await DownLoadSingleImage(images[l]);
            }
        }

        public async Task DownLoadSingleImage(String image)
        {
            HttpResponseMessage message = new HttpResponseMessage();
            try
            {
                //message = await ReportDataService.GetImage(image);
                //var bitmap = await BitmapFactory.DecodeStreamAsync(await message.Content.ReadAsStreamAsync());

                //String sdCardPath = Environment.ExternalStorageDirectory.AbsolutePath;
                //String filePath = Path.Combine(sdCardPath, "Checkd/" + image);

                //FileStream out2 = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
                //bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, out2);
                //out2.Close();


                //message = await ReportDataService.GetImage(image);

                var bitmap = await BitmapFactory.DecodeStreamAsync(await message.Content.ReadAsStreamAsync());
                String sdCardPath = Environment.ExternalStorageDirectory.AbsolutePath;
                String filePath = Path.Combine(sdCardPath, "Checkd/" + image);

                FileStream out2 = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, out2);
                out2.Close();
            }
            catch (Exception e)
            {
                //throw new Java.Lang.Exception("Image Download Exception");
                Log.Debug("Exception", e.Message);

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
                (sender2, e) => GetImageFromGallery(sender2, e, id, type));
            builder.Show();
        }


        private void GetImageFromCamera(object sender, EventArgs eventArgs, string id, string type)
        {
            photoOption = 1;
            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();

                String filePath = Path.Combine(sdCardPath, "Checkd/checkd_" + Guid.NewGuid() + ".jpg");
                Intent intent = new Intent(MediaStore.ActionImageCapture);
                imageJPGFile._file = new File(filePath);

                intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(imageJPGFile._file));
                StartActivityForResult(intent, 1);
            }
        }

        private void GetImageFromGallery(object sender, EventArgs eventArgs, string id, string typer)
        {
            photoOption = 0;

            //Intent i = new Intent(MultiImageSelectorEngine.Action.ActionPickMultiple);
            //StartActivityForResult(i, 1);

        }

        private void CreateDirectoryForPictures()
        {
            imageJPGFile._dir = new File(
                Environment.GetExternalStoragePublicDirectory(
                    Environment.DirectoryPictures), resource.GetString(Resource.String.ImageFolder));
            if (!imageJPGFile._dir.Exists())
            {
                imageJPGFile._dir.Mkdirs();
            }
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities = PackageManager.QueryIntentActivities(intent,
                PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

        private void ProcessImageFromGallery(Intent data, GridView et9, ImageView indicator)
        {

            String[] all_path = data.GetStringArrayExtra("all_path");
            List<String> imagePaths = new List<string>();

            for (int i = 0; i < all_path.Length; i++)
            {
                Uri contentUri = Uri.Parse("file:///" + all_path[i]);
                try
                {
                    Bitmap bitmap = MediaStore.Images.Media.GetBitmap(ContentResolver, contentUri);
                    try
                    {
                        File imgFile = new File("file:///" + all_path[i]);
                        if (imgFile.Exists())
                        {

                        }
                        String filePath = Path.Combine(sdCardPath, "Checkd/checkd_" + Guid.NewGuid() + ".jpg");
                        imagePaths.Add(filePath);

                        FileStream out2 = new FileStream(filePath, FileMode.Create);
                        bitmap.Compress(Bitmap.CompressFormat.Jpeg, 50, out2);
                        out2.Close();

                    }
                    catch (Java.Lang.Exception e)
                    {
                        Log.Debug("Exception", "Process Image from Gallery");
                    }
                }
                catch (Exception)
                {
                    Log.Debug("Gallery Image Exception", "Image Not Found");
                }
            }

            AddImageNameTagToGalleryView(et9, imagePaths, data, indicator);
        }

        private void AddImageNameTagToGalleryView(GridView et10, List<String> imageArrayList, Intent contentUri,
    ImageView indicator)
        {
            String filename = "";

            List<CustomGallery> dataT = new List<CustomGallery>();
            List<CustomGallery> itemsFromGallery = new List<CustomGallery>();

            MiniGallerAdapter adapter3 = (MiniGallerAdapter)et10.Adapter;
            itemsFromGallery = adapter3.getList();

            dataT.AddRange(itemsFromGallery);

            foreach (string uri in imageArrayList)
            {
                CustomGallery item = new CustomGallery();
                item.SdCardPath = uri;
                dataT.Add(item);

                filename = uri.Substring(uri.LastIndexOf("/") + 1);
                if (!ImageUploadArray.Contains(filename))
                {
                    ImageUploadArray.Add(filename);
                }
            }

            List<CustomGallery> distinct = dataT.Distinct().ToList();

            MiniGallerAdapter adapter2 = new MiniGallerAdapter(this, imageLoader);
            adapter2.AddAll(distinct);
            et10.Adapter = adapter2;

            et10.Tag = "Test";

            sharedPreferencesEditor.PutBoolean("ReportEditFlag", true);
            sharedPreferencesEditor.Commit();

            if (filename == null || filename == "")
            {
                indicator.SetImageResource(0);
            }
            else
            {
                indicator.SetImageResource(Resource.Drawable.checked_forms_create_project_medium);
            }

            imageJPGFile.bitmap = null;

            GC.Collect();
        }

        private void ProcessImageFromCamera(GridView et10, ImageView indicator)
        {
            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            Uri contentUri = Uri.FromFile(imageJPGFile._file);


            Bitmap bitmap = MediaStore.Images.Media.GetBitmap(ContentResolver, contentUri);
            try
            {
                FileStream out2 = new FileStream(imageJPGFile._file.Path, FileMode.Create);
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 50, out2);
                out2.Close();

                var bitmapScalled = Bitmap.CreateScaledBitmap(bitmap, 120, 120, true);
                bitmap.Recycle();

                BitmapDrawable ob = new BitmapDrawable(Resources, bitmapScalled);
            }
            catch (Java.Lang.Exception e)
            {
                Log.Debug("Exception", "Process Image from Camera");
            }

            AddImageNameTagToView(et10, mediaScanIntent, contentUri, indicator);
        }

        private void AddImageNameTagToView(GridView et10, Intent mediaScanIntent, Uri contentUri, ImageView indicator)
        {
            mediaScanIntent.SetData(contentUri);
            SendBroadcast(mediaScanIntent);

            String path = imageJPGFile._file.AbsolutePath;
            String filename = path.Substring(path.LastIndexOf("/") + 1);

            String[] all_path = { path };

            List<CustomGallery> dataT = new List<CustomGallery>();
            List<CustomGallery> itemsFromGallery = new List<CustomGallery>();


            MiniGallerAdapter adapter3 = (MiniGallerAdapter)et10.Adapter;
            itemsFromGallery = adapter3.getList();

            dataT.AddRange(itemsFromGallery);

            foreach (string uri in all_path)
            {
                CustomGallery item = new CustomGallery();
                item.SdCardPath = uri;

                dataT.Add(item);
            }

            List<CustomGallery> distinct = dataT.Distinct().ToList();

            MiniGallerAdapter adapter2 = new MiniGallerAdapter(this, imageLoader);
            adapter2.AddAll(distinct);
            et10.Adapter = adapter2;

            et10.Tag = filename;
            ImageUploadArray.Add(filename);

            sharedPreferencesEditor.PutBoolean("ReportEditFlag", true);
            sharedPreferencesEditor.Commit();

            if (filename == null || filename == "")
            {
                indicator.SetImageResource(0);
            }
            else
            {
                indicator.SetImageResource(Resource.Drawable.checked_forms_create_project_medium);
            }

            imageJPGFile.bitmap = null;

            GC.Collect();
        }

        #endregion


        private void ProcessImageFromGallerytpSignature(Intent data, ImageButton et9, ImageView indicator)
        {
            Uri contentUri = data.Data;
            string path = "file:///" + data.Data.ToString();

            Bitmap bitmap = MediaStore.Images.Media.GetBitmap(ContentResolver, Uri.Parse(path));
            try
            {
                FileStream out2 = new FileStream(data.Data.ToString(), FileMode.Create);
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 50, out2);
                out2.Close();

                //var bitmapScalled = Bitmap.CreateScaledBitmap(bitmap, 140, 100, true);
                //bitmap.Recycle();

                BitmapDrawable ob = new BitmapDrawable(Resources, bitmap);
                et9.SetBackgroundDrawable(ob);
                //et9.SetImageResource(0);

            }
            catch (Java.Lang.Exception e)
            {
                Log.Debug("Exception", "Process Image from Gallery");
            }

            AddImageNameTagToSignatureView(et9, contentUri, indicator);
        }

        private void AddImageNameTagToSignatureView(ImageButton et10, Uri contentUri, ImageView indicator)
        {
            String path = contentUri.ToString();
            String filename = path.Substring(path.LastIndexOf("/") + 1);

            et10.Tag = filename;
            ImageUploadArray.Add(filename);

            sharedPreferencesEditor.PutBoolean("ReportEditFlag", true);
            sharedPreferencesEditor.Commit();

            if (filename == null || filename == "")
            {
                indicator.SetImageResource(0);
            }
            else
            {
                indicator.SetImageResource(Resource.Drawable.checked_forms_create_project_medium);
            }

            GC.Collect();
        }

    }
}