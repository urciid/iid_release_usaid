using System;
using Android.Content;
using Android.Net;
using Android.OS;
using USAID.Interfaces;
using Xamarin.Forms;
using GA.Droid.Providers;

[assembly: Dependency(typeof(DeviceSettings))]
namespace GA.Droid.Providers
{
	public class DeviceSettings : IDeviceSettings
	{

		public string CarrierName { get; set; }
		public string PhoneModel { get; set; }
		public string PhonePlatform { get; set; }
		public string PhoneOS { get; set; }
		public string AppVersion { get; set; }
		public bool HasSDCard { get; set; }
		public bool HasCarrierDataNetwork { get; set; }

		public static void Init ()
		{
		}

		public void LoadInfo()
		{
			CarrierName = Android.OS.Build.Brand;
			PhoneModel = Android.OS.Build.Manufacturer + ":" + Android.OS.Build.Model;
			PhonePlatform = "android";
			PhoneOS = Android.OS.Build.VERSION.Release; 
			var packageInfo = Android.App.Application.Context.PackageManager.GetPackageInfo(Android.App.Application.Context.PackageName, 0);
			AppVersion = packageInfo.VersionName;
			HasSDCard = DeviceHasSDCard();
			HasCarrierDataNetwork = IsCarrierDataNetworkOn();
		}

		// <summary>
		/// Determines if the device has external storage mounted
		/// Only used in Android
		/// </summary>
		/// <returns><c>true</c>, if has SD card was deviced, <c>false</c> otherwise.</returns>
		public static bool DeviceHasSDCard()
		{
			bool returnValue = false;

			var externalStorageState = Android.OS.Environment.ExternalStorageState; // getExternalStorageState();

			if (externalStorageState == Android.OS.Environment.MediaMounted)
			{
				returnValue = true;
			}

			return returnValue;
		}

		/// <summary>
		/// Gets the current UTC offset.
		/// </summary>
		/// <returns>The current UTC offset.</returns>
		public int GetCurrentUTCOffset()
		{
			//return TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours;
			Java.Util.TimeZone timeZone = Java.Util.TimeZone.Default; 
			Java.Util.Date now = new Java.Util.Date();
			int offsetFromUtc = timeZone.GetOffset(now.Time) / 3600000;
			return offsetFromUtc;
		}

		/// <summary>
		/// Determines if is device connected to charger.
		/// </summary>
		/// <returns><c>true</c> if is device connected to charger; otherwise, <c>false</c>.</returns>
		public static bool IsDeviceConnectedToCharger()
		{
			bool returnValue = false;

			var filter  = new IntentFilter(Intent.ActionBatteryChanged);
			var batteryState = Android.App.Application.Context.RegisterReceiver(null, filter);

			var plugged = batteryState.GetIntExtra(BatteryManager.ExtraPlugged, -1);
			bool usbCharge = (plugged == Convert.ToInt32(Android.OS.BatteryPlugged.Usb));
			bool acCharge = (plugged == Convert.ToInt32(Android.OS.BatteryPlugged.Ac));

			if (usbCharge || acCharge)
			{
				returnValue = true;
			}

			return returnValue;
		}

		/// <summary>
		/// Determines if is wifi on.
		/// </summary>
		/// <returns><c>true</c> if is wifi on; otherwise, <c>false</c>.</returns>
		public static bool IsWifiOn()
		{
			bool returnValue = false;

			var connectivityManager = (ConnectivityManager)Android.App.Application.Context.GetSystemService("connectivity");
			var activeConnection = connectivityManager.ActiveNetworkInfo;

			var wifiState = connectivityManager.GetNetworkInfo(ConnectivityType.Wifi).GetState();
			if (wifiState == NetworkInfo.State.Connected)
			{
				returnValue = true;
			}

			return returnValue;
		}

		/// <summary>
		/// Determines if internet is connected.
		/// </summary>
		/// <returns><c>true</c> if is wifi on; otherwise, <c>false</c>.</returns>
		public static bool IsInternetOn()
		{
			bool returnValue = false;

			var connectivityManager = (ConnectivityManager)Android.App.Application.Context.GetSystemService("connectivity");
			var activeConnection = connectivityManager.ActiveNetworkInfo;

			if ((activeConnection != null) && activeConnection.IsConnected)
			{
				returnValue = true;
			}

			return returnValue;
		}

		/// <summary>
		/// Determines if data is connected.
		/// </summary>
		/// <returns><c>true</c> if is data on; otherwise, <c>false</c>.</returns>
		public static bool IsCarrierDataNetworkOn()
		{
			bool returnValue = false;

			var connectivityManager = (ConnectivityManager)Android.App.Application.Context.GetSystemService("connectivity");


			var activeConnection = connectivityManager.ActiveNetworkInfo;
			if (activeConnection != null)
			{
				if(activeConnection.Type == ConnectivityType.Mobile && activeConnection.Type != ConnectivityType.Wifi)
				{
					returnValue = true;
				} 
			}

			return returnValue;
		}
	}
}

