using System;
using System.Collections;
using Xamarin.Forms;
using System.Diagnostics;
using USAID.Interfaces;
using GA.Droid.Providers;

[assembly: Dependency(typeof(GALogger))]
namespace GA.Droid.Providers
{
	public class GALogger : IGALogger
	{
		bool enableHockeyApp = false;

		#region ILogger implementation

		public virtual void TrackPage(string page, string id = null)
		{
			Debug.WriteLine("Evolve Logger: TrackPage: " + page.ToString() + " Id: " + id ?? string.Empty);

			if (!enableHockeyApp)
				return;
			//HockeyApp.Android.Metrics.MetricsManager.TrackEvent($"{page}Page");

		}


		public virtual void Track(string trackIdentifier)
		{
			Debug.WriteLine("Evolve Logger: Track: " + trackIdentifier);

			if (!enableHockeyApp)
				return;
			//HockeyApp.Android.Metrics.MetricsManager.TrackEvent(trackIdentifier);
		}

		public virtual void Track(string trackIdentifier, string key, string value)
		{
			Debug.WriteLine("Evolve Logger: Track: " + trackIdentifier + " key: " + key + " value: " + value);

			if (!enableHockeyApp)
				return;

			trackIdentifier = $"{trackIdentifier}-{key}-{@value}";

			//HockeyApp.Android.Metrics.MetricsManager.TrackEvent(trackIdentifier);
		}

		public virtual void Report(Exception exception = null, Severity warningLevel = Severity.Warning)
		{
			Debug.WriteLine("Evolve Logger: Report: " + exception);

		}
		public virtual void Report(Exception exception, IDictionary extraData, Severity warningLevel = Severity.Warning)
		{
			Debug.WriteLine("Evolve Logger: Report: " + exception);
		}
		public virtual void Report(Exception exception, string key, string value, Severity warningLevel = Severity.Warning)
		{
			Debug.WriteLine("Evolve Logger: Report: " + exception + " key: " + key + " value: " + value);
		}
		#endregion
	}
}




