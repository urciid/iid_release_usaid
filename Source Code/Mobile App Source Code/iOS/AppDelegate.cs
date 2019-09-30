using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;

using USAID;
using Google.Analytics;
using UIKit;
using System.Runtime.InteropServices;
using USAID.Common;
using USAID.ApplicationObjects;
using System.Threading;
using Xuni.iOS;

namespace USAID.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		#region Fields

		private NSDictionary _launchOptions;
		#endregion
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init();
			_launchOptions = options;
			Xuni.Forms.FlexChart.Platform.iOS.Forms.Init();
			// Code for starting up the Xamarin Test Cloud Agent
#if ENABLE_TEST_CLOUD
			//Xamarin.Calabash.Start();
#endif

			//InitializeHockeyApp();
			//AllowLocalNotifications();

			// grab your app token from https://app.testfairy.com/settings/ 
			//#if DEBUG
			TestFairyLib.TestFairy.Begin ("b13a46d6a2e460bf6137d7160b6768b06d136e3e");
			//#endif

			LoadApplication(new App(new Setup(app)));

			#region Localization debug info
			foreach (var s in NSLocale.PreferredLanguages)
			{
				Console.WriteLine("pref:" + s);
			}

			var iosLocaleAuto = NSLocale.AutoUpdatingCurrentLocale.LocaleIdentifier;
			var iosLanguageAuto = NSLocale.AutoUpdatingCurrentLocale.LanguageCode;
			Console.WriteLine("nslocaleid:" + iosLocaleAuto);
			Console.WriteLine("nslanguage:" + iosLanguageAuto);


			var iosLocale = NSLocale.CurrentLocale.LocaleIdentifier;
			var iosLanguage = NSLocale.CurrentLocale.LanguageCode;
			var netLocale = iosLocale.Replace("_", "-");
			var netLanguage = iosLanguage.Replace("_", "-");
			Console.WriteLine("ios:" + iosLanguage + " " + iosLocale);
			Console.WriteLine("net:" + netLanguage + " " + netLocale);

			Console.WriteLine("culture:" + Thread.CurrentThread.CurrentCulture);
			Console.WriteLine("uiculture:" + Thread.CurrentThread.CurrentUICulture);
			#endregion



			return base.FinishedLaunching(app, options);
		}

		//private void AllowLocalNotifications()
		//{
		//	//The below code is to allow local notifications to be run on the applications
		//	var settings = UIUserNotificationSettings.GetSettingsForTypes(
		//	UIUserNotificationType.Alert
		//	| UIUserNotificationType.Badge
		//	| UIUserNotificationType.Sound,
		//	new NSSet());
		//	UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
		//}
		public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{
			if (url == null)
			{
				return false;
			}

			//if (url.Scheme == "usaid")
			//{
			//	//Need to get parameters
			//	NSUrlComponents comp = new NSUrlComponents(url.ToString());
			//	var items = comp.QueryItems;
			//	var email = items[0].Value;
			//	var password = items[1].Value;
			//	Providers.UserInfoService userService = new Providers.UserInfoService();
			//	userService.SaveUserInfo(new USAID.Models.UserInfoModel { Email = email, Password = password, LastDownload =  });
			//}

			return true;
		}
		//private void InitializeHockeyApp()
		//{
		//	try
		//	{
		//		if (!string.IsNullOrWhiteSpace(Keys.HockeyAppiOS))
		//		{

		//			var manager = BITHockeyManager.SharedHockeyManager;
		//			manager.Configure(Keys.HockeyAppiOS);

		//			//Disable update manager
		//			manager.DisableUpdateManager = true;

		//			manager.StartManager();
		//			//manager.Authenticator.AuthenticateInstallation();

		//		}
		//	}
		//	catch(Exception err)
		//	{
		//		//log
		//		var x = err.ToString();
		//	}
		//}
		//#region PUSH
		//[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		//public static extern IntPtr IntPtr_objc_msgSend(IntPtr receiver, IntPtr selector);

		//// this is the callback for remote notification registration, it must be in AppDelegate
		//// the actual registration call is made from within login, settings or where we decide
		//public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
		//{
		//	// process the received token
		//	var str = (NSString)Runtime.GetNSObject(IntPtr_objc_msgSend(deviceToken.Handle, new Selector("description").Handle));

		//	string deviceTokenString = str.ToString().Replace("<", "").Replace(">", string.Empty).Replace(" ", string.Empty);
		//	_pushNotificationManager.SaveDeviceToken(deviceTokenString);

		//}

		//public override void DidRegisterUserNotificationSettings(UIApplication application, UIUserNotificationSettings notificationSettings)
		//{
		//	application.RegisterForRemoteNotifications();
		//}

		//public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
		//{
		//	//Logger.Error ("FailedToRegisterForRemoteNotifications: {0}, {1}, {2}", error.Domain, error.Code, error.LocalizedDescription);   
		//	//var x = error;
		//}
		//public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
		//{
		//	//NSDictionary aps = (NSDictionary)userInfo.ObjectForKey(new NSString("aps"));
		//	//string alert = aps.ObjectForKey(new NSString("alert")).ToString();
		//	//InvokeOnMainThread(delegate
		//	//{
		//	//	(new UIAlertView("Alert", alert, null, "Ok", null)).Show();
		//	//});

		//	//UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
		//}

		//#endregion
	}
}

