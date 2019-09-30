using System;
using System.Threading.Tasks;
using Android.Content;
using Android.App;
//using Android.Gms.Common;
using System.Threading;
using USAID.Interfaces;
//using Android.Gms.Gcm;
using USAID.Common;

namespace GA.Droid.Utilities
{
	public class PushNotificationManager
	{

		#region push notification

		public static void RegisterPush(Context context)
		{
			if (checkPlayServices(context))
			{
				ThreadPool.QueueUserWorkItem(o => GCMRun(context));
			}
		}

		public static void GCMRun(Context context)
		{
			try
			{
				//GoogleCloudMessaging gcm = GoogleCloudMessaging.GetInstance(context);
				string[] senderIds = { Keys.GooglePushKey };
				//string regid = gcm.Register(senderIds);
				//we will receive the token in the gcmintentservice
				//G.Verbose("regid: " + regid);

			}
			catch (Java.Lang.Exception ex)
			{
				//GALogge.Error("Failed to register to notifications", ex);
			}
		}

		void UnregisterPush(Context context)
		{
			Intent intent = new Intent("com.google.android.c2dm.intent.UNREGISTER");
			intent.PutExtra("app", PendingIntent.GetBroadcast(context, 0, new Intent(), 0));
			context.StartService(intent);
		}

		private static bool checkPlayServices(Context context)
		{
			//int resultCode = GooglePlayServicesUtil.IsGooglePlayServicesAvailable(context);
			//if (resultCode != ConnectionResult.Success)
			//{
			//	if (GooglePlayServicesUtil.IsUserRecoverableError(resultCode))
			//	{
			//		GooglePlayServicesUtil.GetErrorDialog(resultCode, (Activity)context, 0).Show();
			//	}
			//	else
			//	{
			//		Console.WriteLine("This device is not supported.");
			//		//Finish();
			//	}
			//	return false;
			//}
			return true;
		}
		#endregion
	}
}



