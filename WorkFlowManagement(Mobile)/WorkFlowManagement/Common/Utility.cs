using Android.Content;
using Android.Locations;
using Android.Net;
using Android.Telephony;
using Android.Widget;
using Java.Util;

namespace WorkFlowManagement.Common
{
    public class Utility
    {
        public static bool IsInternetAvailable(Context contxt)
        {
            Context context = contxt;
            ConnectivityManager connectivityManager = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
            var activeConnection = connectivityManager.ActiveNetworkInfo;

            if ((activeConnection != null) && activeConnection.IsConnected)
            {
                return true;
            }
            return false;
        }

        public static void DisplayToast(Context context, string message)
        {
            Toast.MakeText(context, message, ToastLength.Long).Show();
        }

        public static bool IsGPSAvailable(LocationManager locationManager)
        {
            if (locationManager.IsProviderEnabled(LocationManager.GpsProvider))
            {
                return true;
            }
            return false;
        }

        public static string GetDeviceId(Context context)
        {
            var telephonyDeviceID = string.Empty;
            var telephonySIMSerialNumber = string.Empty;
            TelephonyManager telephonyManager = (TelephonyManager)context.ApplicationContext.GetSystemService(Context.TelephonyService);
            if (telephonyManager != null)
            {
                if (!string.IsNullOrEmpty(telephonyManager.DeviceId))
                    telephonyDeviceID = telephonyManager.DeviceId;
                if (!string.IsNullOrEmpty(telephonyManager.SimSerialNumber))
                    telephonySIMSerialNumber = telephonyManager.SimSerialNumber;
            }
            var androidID = Android.Provider.Settings.Secure.GetString(context.ApplicationContext.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
            var deviceUuid = new UUID(androidID.GetHashCode(), ((long)telephonyDeviceID.GetHashCode() << 32) | telephonySIMSerialNumber.GetHashCode());
            return deviceUuid.ToString();
        }
    }
}