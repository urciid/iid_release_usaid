using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Foundation;
using ObjCRuntime;
using UIKit;
using USAID.Interfaces;

namespace USAID.iOS
{
	public delegate void RegisterDeviceDelegate(string deviceToken);

	public class PushNotificationManager
	{
		public bool DeviceIsRegisteredForPush
		{
			get
			{
				NSUserDefaults defaults = NSUserDefaults.StandardUserDefaults;
				if (defaults.ValueForKey(new NSString("HasPush")) == null)
					return false;

				return defaults.BoolForKey("HasPush");
			}
			set
			{
				NSUserDefaults defaults = NSUserDefaults.StandardUserDefaults;
				defaults.SetBool(value, "HasPush");
				defaults.Synchronize();
			}
		}

		public void RegisterApplication()
		{
			var currentVersion = new Version(UIDevice.CurrentDevice.SystemVersion);
			Version changeVersion = new Version(8, 0);
			if (currentVersion >= changeVersion)
			{
				UIUserNotificationSettings settings = UIUserNotificationSettings.GetSettingsForTypes(
					UIUserNotificationType.Alert | UIUserNotificationType.Badge, new NSSet());

				UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
			}
			else {
				// Code to support earlier iOS versions
				UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge);
			}
		}

		public void UnregisterApplication()
		{
			UIApplication.SharedApplication.UnregisterForRemoteNotifications();
		}

		public string LoadDeviceToken()
		{
			var token = NSUserDefaults.StandardUserDefaults.StringForKey("DeviceToken");
			return token;
		}

		public void SaveDeviceToken(string token)
		{
			NSUserDefaults.StandardUserDefaults.SetString(token, "DeviceToken");
			NSUserDefaults.StandardUserDefaults.Synchronize();
		}
	}
}



