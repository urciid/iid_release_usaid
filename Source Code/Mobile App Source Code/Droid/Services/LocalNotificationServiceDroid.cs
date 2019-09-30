using System;
using Android.App;
using Android.Content;
using Android.Support.V4.App;
using USAID.Services;

namespace GA.Droid.Services
{
	public class LocalNotificationServiceDroid : ILocalNotificationService
	{
		public LocalNotificationServiceDroid()
		{

		}

		public void Notify(string title, string body, int id)
		{
			var builder = new NotificationCompat.Builder(Application.Context);
			builder.SetContentTitle(title);
			builder.SetContentText(body);
			builder.SetAutoCancel(true);


			builder.SetSmallIcon(Resource.Drawable.sales);

			var resultIntent = GetLauncherActivity();
			resultIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);

			var stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(Application.Context);
			stackBuilder.AddNextIntent(resultIntent);
		
			var resultPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);
			builder.SetContentIntent(resultPendingIntent);

			var notificationManager = NotificationManagerCompat.From(Application.Context);
			notificationManager.Notify(id, builder.Build());
		}


		public static Intent GetLauncherActivity()
		{
			var packageName = Application.Context.PackageName;
			return Application.Context.PackageManager.GetLaunchIntentForPackage(packageName);
		}
	}
}

