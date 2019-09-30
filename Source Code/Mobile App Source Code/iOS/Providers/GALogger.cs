using System;
using System.Collections.Generic;
using Google.Analytics;
using Xamarin.Forms;
using USAID.iOS.Providers;
using System.Collections;
using System.Diagnostics;
using USAID.Interfaces;
//using HockeyApp;


[assembly: Dependency(typeof(GALogger))]
namespace USAID.iOS.Providers
{
	
	public class GALogger : IGALogger
	{
		bool enableHockeyApp = false;

		public virtual void TrackPage(string page, string id = null)
		{
			Debug.WriteLine("GA Logger: TrackPage: " + page.ToString() + " Id: " + id ?? string.Empty);

			if (!enableHockeyApp)
				return;
			//HockeyApp.iOS.BITHockeyManager.SharedHockeyManager?.MetricsManager?.TrackEvent($"{page}Page");
		}
		public virtual void Track(string trackIdentifier)
		{
			Debug.WriteLine("GA Logger: Track: " + trackIdentifier);

			if (!enableHockeyApp)
				return;
			//HockeyApp.iOS.BITHockeyManager.SharedHockeyManager?.MetricsManager?.TrackEvent(trackIdentifier);
		}

		public virtual void Track(string trackIdentifier, string key, string value)
		{
			Debug.WriteLine("GA Logger: Track: " + trackIdentifier + " key: " + key + " value: " + value);

			if (!enableHockeyApp)
				return;

			trackIdentifier = $"{trackIdentifier}-{key}-{@value}";
			//HockeyApp.iOS.BITHockeyManager.SharedHockeyManager?.MetricsManager?.TrackEvent(trackIdentifier);
		}

		public virtual void Report(Exception exception = null, Severity warningLevel = Severity.Warning)
		{
			Debug.WriteLine("GA Logger: Report: " + exception);

		}
		public virtual void Report(Exception exception, IDictionary extraData, Severity warningLevel = Severity.Warning)
		{
			Debug.WriteLine("GA Logger: Report: " + exception);
		}
		public virtual void Report(Exception exception, string key, string value, Severity warningLevel = Severity.Warning)
		{
			Debug.WriteLine("GA Logger: Report: " + exception + " key: " + key + " value: " + value);
		}
	}
}

