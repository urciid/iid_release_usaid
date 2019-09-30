using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
//using Android.Gms.Common.Apis;
using GA.Droid;
using GA.Droid.Utilities;
using USAID.Common;
using GA.Droid.Providers;
using Com.Testfairy;

namespace USAID.Droid
{
	[Activity (Label = "Mobile Assist", Theme = "@style/GreatAmericaTheme", LaunchMode = LaunchMode.SingleTop, AlwaysRetainTaskState = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
	[IntentFilter(new[] { Intent.ActionCall, Intent.ActionView },
		Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
	              DataScheme = "USAID")]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		
		protected override void OnCreate (Bundle bundle)
		{
			//Xamarin.Insights.Initialize (global::WMP.Droid.XamarinInsights.ApiKey, this);
			base.OnCreate (bundle);
			this.ActionBar.SetIcon(Android.Resource.Color.Transparent);
			global::Xamarin.Forms.Forms.Init (this, bundle);
			//InitializeHockeyApp();
			TestFairy.Begin(this, "b13a46d6a2e460bf6137d7160b6768b06d136e3e");

			if (Intent.Data != null && Intent.Scheme.Equals("usaid"))
			{
				////coming from URL
				//var email = Intent.Data.GetQueryParameters("email")[0];
				//var password = Intent.Data.GetQueryParameters("password")[0];
				//if (dealerKey != null && partnerKey != null)
				//{
				//	UserInfoService userInfoService = new UserInfoService();
				//	userInfoService.SaveUserInfo(new Shared.Models.UserInfoModel { Email = email, PartnerKey = partnerKey });

				//}

			}

			LoadApplication (new App (new Setup()));

		}

		//void InitializeHockeyApp()
		//{
		//	try
		//	{
		//		if (string.IsNullOrWhiteSpace(Keys.HockeyAppAndroid))
		//			return;

		//		HockeyApp.Android.CrashManager.Register(this, Keys.HockeyAppAndroid);
		//		//HockeyApp.UpdateManager.Register(this, ApiKeys.HockeyAppAndroid);

		//		HockeyApp.Android.Metrics.MetricsManager.Register(this, Application, Keys.HockeyAppAndroid);

		//	}
		//	catch (Exception err)
		//	{
		//		//log error
		//		var x = err.ToString();
		//	}

		//}
	}
}
