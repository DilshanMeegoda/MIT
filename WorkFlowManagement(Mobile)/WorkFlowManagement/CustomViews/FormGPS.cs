using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using WorkFlowManagement.Activities;
using WorkFlowManagement.Common;
using WorkFlowManagement.Enum;
using WorkFlowManagement.Model;
using Orientation = Android.Widget.Orientation;

namespace WorkFlowManagement.CustomViews
{
    public class FormGPS : LinearLayout
    {
        private RelativeLayout theme;
        private Resources resource;

        private EditText editText;
        private Context contextx;

        private string longtitude;
        private string latitude;
        private string curaddress;
        private bool isArcheived;
        private int OwnerID;
        private int VerifierID;
        private InformationPopup Popup;

        private ISharedPreferences sharedPreferences;
        private ISharedPreferencesEditor sharedPreferencesEditor;
        private ReportStatus reportStatus;

        public FormGPS(Context context, ReportElement element, int ownerID, int userID, int verifiedID, ReportStatus Reportstatus)
            : base(context)
        {
            resource = context.Resources;
            contextx = context;
            OwnerID = userID;
            VerifierID = verifiedID;
            theme = new FormTheme(context, element.Title);
            Orientation = Orientation.Vertical;
            RelativeLayout gpsHolder = new RelativeLayout(contextx);
            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(context);
            sharedPreferencesEditor = sharedPreferences.Edit();
            List<KeyValue> existingValue = element.Values;
            Popup = new InformationPopup(context);
            reportStatus = Reportstatus;

            editText = new EditText(context);
            Id = element.Id;

            isArcheived = sharedPreferences.GetBoolean(Resources.GetString(Resource.String.is_archived), false);

            if (existingValue != null)
            {
                foreach (var address in existingValue)
                {
                    if (address.Name == "address")
                    {
                        editText.Text = address.Value;
                        curaddress = address.Value;
                    }

                    if (address.Name == "longitude")
                    {
                        longtitude = address.Value;
                    }

                    if (address.Name == "latitude")
                    {
                        latitude = address.Value;
                    }
                }
            }

            editText.SetBackgroundResource(Resource.Drawable.custom_edit_text_color);

            RelativeLayout.LayoutParams paramsForGPSHolder = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.WrapContent);
            gpsHolder.LayoutParameters = paramsForGPSHolder;

            ImageButton locationBtn = new ImageButton(context);
            locationBtn.Id = 891;
            locationBtn.SetBackgroundResource(0);
            locationBtn.SetImageResource(Resource.Drawable.android_form_gps_icon);
            locationBtn.Click += (senderGPS, e) => AddressButton_OnClick(editText, this);

            RelativeLayout.LayoutParams paramsForlocationBtn = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
            paramsForlocationBtn.AddRule(LayoutRules.AlignParentTop);
            paramsForlocationBtn.AddRule(LayoutRules.AlignParentEnd);
            locationBtn.LayoutParameters = paramsForlocationBtn;

            RelativeLayout.LayoutParams paramsForEditText = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
            paramsForEditText.AddRule(LayoutRules.AlignParentTop);
            paramsForEditText.AddRule(LayoutRules.AlignParentStart);
            paramsForEditText.AddRule(LayoutRules.StartOf, locationBtn.Id);
            editText.LayoutParameters = paramsForEditText;
            //editText.Gravity = GravityFlags.CenterVertical;

            gpsHolder.AddView(editText);
            gpsHolder.AddView(locationBtn);

            ImageView indicatorImage = (ImageView)theme.GetChildAt(1);
            //activateElementInfo(element);
            Popup.activateElementInfo(theme, element);
            editText.TextChanged += (sender, e) =>
            {
                if (!editText.Text.Equals(""))
                {
                    indicatorImage.SetImageResource(Resource.Drawable.checked_forms_create_project_medium);
                    curaddress = editText.Text;
                }
                else
                {
                    indicatorImage.SetImageResource(0);
                }
            };

            //when opening a Draft or Archive
            if (!editText.Text.Equals(""))
            {
                indicatorImage.SetImageResource(Resource.Drawable.checked_forms_create_project_medium);
            }


            if (string.IsNullOrEmpty(element.FilledBy))
            {
                //do nothing
            }
            else if (element.FilledBy == userID + "")
            {
                //do nothing
            }
            else
            {
                editText.Enabled = false;
                editText.SetTextColor(Resources.GetColor(Resource.Color.grey));
            }

            TextView elementSplitLine = new TextView(context);
            elementSplitLine.TextSize = 0.5f;
            elementSplitLine.SetBackgroundColor(Color.ParseColor(resource.GetString(Resource.Color.grey)));


            if (OwnerID == 0 || OwnerID == userID)
            {
                if (VerifierID != 0)
                {
                    editText.Enabled = false;
                    editText.SetTextColor(Resources.GetColor(Resource.Color.grey));
                    locationBtn.Enabled = false;
                    locationBtn.Click += null;

                    if (reportStatus == ReportStatus.Rejected)
                    {
                        editText.Enabled = true;
                        editText.SetTextColor(Resources.GetColor(Resource.Color.black));
                        locationBtn.Enabled = true;
                    }
                }

                else
                {
                    editText.Enabled = true;
                    editText.SetTextColor(Resources.GetColor(Resource.Color.black));
                    locationBtn.Enabled = true;
                }
            }
            else
            {
                editText.Enabled = false;
                editText.SetTextColor(Resources.GetColor(Resource.Color.grey));
                locationBtn.Enabled = false;
                locationBtn.Click += null;
            }

            if (isArcheived)
            {
                editText.Enabled = false;
                editText.SetTextColor(Resources.GetColor(Resource.Color.grey));
                locationBtn.Enabled = false;
                locationBtn.Click += null;
            }


            AddView(theme);
            AddView(gpsHolder);
            SetPadding(45, 10, 45, 20);
        }

        private async void AddressButton_OnClick(EditText addressGPS, FormGPS gpsObject)
        {
            LocationManager _locationManager = FormActivity._locationManager;
            Location _currentLocation = FormActivity._currentLocation;

            if (Utility.IsGPSAvailable(_locationManager))
            {
                if (_currentLocation == null)
                {
                    addressGPS.Hint = "GPS is stabilizing . Please try again in few seconds";
                    return;
                }

                try
                {
                    Geocoder geocoder = new Geocoder(contextx);
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

                        sharedPreferencesEditor.PutBoolean("ReportEditFlag", true);
                        sharedPreferencesEditor.Commit();
                    }
                    else
                    {
                        addressGPS.Hint = "Address not available. You can manually type an Address here";
                    }
                }
                catch (Exception ex)
                {
                    addressGPS.Hint = "Address not available at this time, Seems like Internet is turned Off";
                    Log.Debug("GPS Exception", ex.ToString());
                }
            }
            else
            {
                Toast.MakeText(contextx, "Please turn on GPS to get location", ToastLength.Long).Show();
            }
        }

        private void activateElementInfo(ReportElement element)
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
            AlertDialog.Builder builder = new AlertDialog.Builder(contextx);
            builder.SetMessage(information);

            builder.SetPositiveButton("Ok", (senderAlert, args) =>
            {
                builder.Dispose();
            });
            builder.Show();
        }

        public void setGeoCoOrdinates(string longti, string lati)
        {
            longtitude = longti;
            latitude = lati;
            curaddress = editText.Text;
        }

        public List<KeyValue> getGPSData()
        {
            List<KeyValue> gpsdataBundle = new List<KeyValue>();
            KeyValue gpsLong = new KeyValue();
            gpsLong.Name = "longtitude";
            gpsLong.Value = longtitude;
            gpsdataBundle.Add(gpsLong);

            KeyValue gpsLati = new KeyValue();
            gpsLati.Name = "latitude";
            gpsLati.Value = latitude;
            gpsdataBundle.Add(gpsLati);

            KeyValue gpsAddress = new KeyValue();
            gpsAddress.Name = "address";
            gpsAddress.Value = curaddress;
            gpsdataBundle.Add(gpsAddress);

            return gpsdataBundle;
        }

    }
}