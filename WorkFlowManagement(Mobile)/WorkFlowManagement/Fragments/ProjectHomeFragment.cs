using System;
using System.Net;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Util;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using WorkFlowManagement.Common;
using WorkFlowManagement.Model;
using WorkFlowManagement.Services;
using Fragment = Android.Support.V4.App.Fragment;

namespace WorkFlowManagement.Fragments
{
    public class ProjectHomeFragment : Fragment
    {
        private const int DescriptionTextLength = 140;
        private static Project _project;
        private TextView _descriptionText;
        private ImageView _homeMap;
        private string _imageName;
        private TextView _location;
        private TextView _locationText;
        private Uri _mImageCaptureUri;
        private TextView _ownerName;
        private TextView _phoneNumber;
        private string _profileImageUriBase;
        private int _projectId;
        private ImageButton _projectLocation;
        private TextView _projectName;
        private Button _readMore;
        private bool _readMoreFlag;
        private ISharedPreferences _sharedPreferences;

        public static ProjectHomeFragment NewInstance()
        {
            return new ProjectHomeFragment();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_project_home, container, false);
            InitializeViews(view);

            _sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            var jsonString = _sharedPreferences.GetString(Resources.GetString(Resource.String.current_project), string.Empty);
            _project = JsonConvert.DeserializeObject<Project>(jsonString);

            SetProjectDetail();
            return view;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
        }

        private void InitializeViews(View view)
        {
            _projectName = view.FindViewById<TextView>(Resource.Id.textViewProjectNameLarge);
            _locationText = view.FindViewById<TextView>(Resource.Id.textViewSubtitle);
            _descriptionText = view.FindViewById<TextView>(Resource.Id.textViewDescription);
            _ownerName = view.FindViewById<TextView>(Resource.Id.textViewProjectOwner);
            _phoneNumber = view.FindViewById<TextView>(Resource.Id.textViewProjectPhoneNumber);
            _location = view.FindViewById<TextView>(Resource.Id.textViewProjectLocation);
            _readMore = view.FindViewById<Button>(Resource.Id.imagebutton_read_more);
            _homeMap = view.FindViewById<ImageView>(Resource.Id.imageHomeMap);
            _homeMap.SetScaleType(ImageView.ScaleType.CenterCrop);
            _projectLocation = view.FindViewById<ImageButton>(Resource.Id.imageViewProjectLocIcon);

            _homeMap.Click += ProjectLocation_Click;
            _readMore.Click += ReadMoreButton_Click;
            _projectLocation.Click += ProjectLocation_Click;
        }

        private void SetProjectDetail()
        {
            _projectName.Text = _project.Title;
            _locationText.Text = _project.Location;

            if (_project.Description.Length < DescriptionTextLength)
            {
                _descriptionText.Text = _project.Description;
                _readMore.Visibility = ViewStates.Gone;
            }
            else
            {
                _readMore.Visibility = ViewStates.Visible;
                _readMore.Text = Resources.GetString(Resource.String.read_more);
                _descriptionText.Text = _project.Description.Substring(0, DescriptionTextLength) + "...";
            }

            _ownerName.Text = _project.OwnerUserName;
            _phoneNumber.Text = _project.OwnerContactNo;
            _location.Text = string.IsNullOrEmpty(_project.Address) ? _project.Location : _project.Address;

            //if (Utility.IsInternetAvailable(Application.Context)) GetMapSnapShot();
        }

        private void ProjectLocation_Click(object sender, EventArgs e)
        {
            var lat = _project.Latitude;
            var log = _project.Longitude;

            if (string.IsNullOrEmpty(lat))
            {
                var mapString2 = "geo:0,0?q=" + _project.Address; // + "(" + project .Title+ ")";

                if (string.IsNullOrEmpty(_project.Address))
                {
                    var mapString3 = "geo:0,0?q=" + _project.Location; // + "(" + project.Title + ")";

                    var geoUri = Android.Net.Uri.Parse(mapString3);
                    var mapIntent = new Intent(Intent.ActionView, geoUri);
                    StartActivity(mapIntent);

                    if (string.IsNullOrEmpty(_project.Location))
                        Toast.MakeText(Application.Context, "There is no Location Data available", ToastLength.Short)
                            .Show();
                }
                else
                {
                    var geoUri = Android.Net.Uri.Parse(mapString2);
                    var mapIntent = new Intent(Intent.ActionView, geoUri);
                    StartActivity(mapIntent);
                }
            }
            else
            {
                var mapString = "geo:" + lat + "," + log; // + "(" + project.Title + ")";
                var geoUri = Android.Net.Uri.Parse(mapString);
                var mapIntent = new Intent(Intent.ActionView, geoUri);
                StartActivity(mapIntent);
            }
        }

        private void ReadMoreButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (_project.Description.Length > DescriptionTextLength)
                {
                    if (!_readMoreFlag)
                    {
                        _readMore.Text = Resources.GetString(Resource.String.show_less);
                        _descriptionText.Text = _project.Description;
                        _readMoreFlag = true;
                    }
                    else
                    {
                        _readMore.Text = Resources.GetString(Resource.String.read_more);
                        _descriptionText.Text = _project.Description.Substring(0, DescriptionTextLength) + "...";
                        _readMoreFlag = false;
                    }
                }
            }
            catch (Exception)
            {
                Log.Debug("Read More", "Data not loaded");
            }
        }

        private void GetMapSnapShot()
        {
            var lat = _project.Latitude;
            var log = _project.Longitude;

            var imageBitmap =
                GetImageBitmapFromUrl(
                    "http://maps.google.com/maps/api/staticmap?markers=color:red|%f,%f&%@&sensor=true,%20" + lat +
                    ",%20" + log + ",%20zoom=12&size=770x240");

            if (string.IsNullOrEmpty(lat))
            {
                imageBitmap = GetImageBitmapFromUrl("http://maps.googleapis.com/maps/api/staticmap?center=" +
                                                    _project.Address + "&zoom=12,&size=770x240");

                if (string.IsNullOrEmpty(_project.Address))
                    imageBitmap = GetImageBitmapFromUrl("http://maps.googleapis.com/maps/api/staticmap?center=" +
                                                        _project.Location + "&zoom=12,&size=770x240");
            }

            _homeMap.SetImageBitmap(imageBitmap);
        }

        private static Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
            }

            return imageBitmap;
        }
    }
}