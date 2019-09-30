using System;
using CoreTelephony;
using Foundation;
using USAID.iOS.Utilities;
using USAID.Interfaces;
using UIKit;
using Xamarin.Forms;
using USAID.iOS.Providers;

[assembly: Dependency(typeof(DeviceSettings))]
namespace USAID.iOS.Providers
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

		/// <summary>
		/// Loads the info.
		/// </summary>
		public void LoadInfo()
		{
			CTTelephonyNetworkInfo phoneNetworkInfo = new CTTelephonyNetworkInfo ();
			CarrierName = phoneNetworkInfo.SubscriberCellularProvider == null ? null : phoneNetworkInfo.SubscriberCellularProvider.CarrierName;
			CarrierName = CarrierName ?? "Simulator";
			PhoneModel = UIDevice.CurrentDevice.Model;
			PhonePlatform = "ios";
			PhoneOS = UIDevice.CurrentDevice.SystemVersion;
			AppVersion = NSBundle.MainBundle.ObjectForInfoDictionary ("CFBundleVersion").ToString ();
			HasSDCard = DeviceHasSDCard();
			HasCarrierDataNetwork = IsCarrierDataNetworkOn();
		}

		/// <summary>
		/// Determines if the device has external storage mounted
		/// Only used in Android
		/// </summary>
		/// <returns><c>true</c>, if has SD card was deviced, <c>false</c> otherwise.</returns>
		public static bool DeviceHasSDCard()
		{
			return false;
		}

		/// <summary>
		/// Gets the current UTC offset.
		/// </summary>
		/// <returns>The current UTC offset.</returns>
		public int GetCurrentUTCOffset()
		{
			return (int)NSTimeZone.LocalTimeZone.GetSecondsFromGMT / 3600;
		}

		/// <summary>
		/// Determines if is device connected to charger.
		/// </summary>
		/// <returns><c>true</c> if is device connected to charger; otherwise, <c>false</c>.</returns>
		public static bool IsDeviceConnectedToCharger()
		{
			bool returnValue = false;

			UIDevice.CurrentDevice.BatteryMonitoringEnabled = true;

			if (UIDevice.CurrentDevice.BatteryState == UIDeviceBatteryState.Charging || UIDevice.CurrentDevice.BatteryState == UIDeviceBatteryState.Full)
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

			var localWifiStatus = Reachability.LocalWifiConnectionStatus();
			if (localWifiStatus == NetworkStatus.ReachableViaWiFiNetwork)
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

			var internetStatus = Reachability.InternetConnectionStatus();
			if (internetStatus != NetworkStatus.NotReachable)
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

			var internetStatus = Reachability.InternetConnectionStatus ();
			if (internetStatus == NetworkStatus.ReachableViaCarrierDataNetwork) {
				returnValue = true;
			} 

			return returnValue;
		}
	}
}

