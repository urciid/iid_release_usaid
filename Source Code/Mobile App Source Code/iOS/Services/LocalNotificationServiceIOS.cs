using System;
using Foundation;
using UIKit;
using USAID.Services;

namespace WMP.iOS.Services
{
	public class LocalNotificationServiceIOS : ILocalNotificationService
	{
		public LocalNotificationServiceIOS(UIApplication app)
		{
			_application = app;
		}

		UIApplication _application;

		public void Notify(string title, string body, int id)
		{
			UILocalNotification loggedOutNotification = new UILocalNotification();
			loggedOutNotification.AlertBody = body;
			loggedOutNotification.AlertAction = title;
			loggedOutNotification.TimeZone = NSTimeZone.DefaultTimeZone;
			loggedOutNotification.RepeatInterval = 0;//default "The default value is 0, which means that the system fires the notification once and then discards it."
			loggedOutNotification.FireDate = (NSDate)DateTime.Now;

			_application.ScheduleLocalNotification(loggedOutNotification);

		}
	}
}

