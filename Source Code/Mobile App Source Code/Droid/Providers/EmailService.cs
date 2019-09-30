using Xamarin.Forms;
using GA.Droid.Providers;
using Android.Content;
using Android.Content.PM;
using System.Collections.Generic;
using USAID.Interfaces;
using USAID.Models;
using Android.OS;
using System.IO;

[assembly: Dependency(typeof(EmailService))]
namespace GA.Droid.Providers
{
	public class EmailService : IEmailService
	{
		/// <summary>
		/// Used for registration with dependency service
		/// </summary>
		public static void Init ()
		{
		}

        /// <summary>
        /// Opens up the Android Email Sender with pre-populated values
        /// </summary>
        /// <param name="email">Email.</param>
        public void CreateEmail(Email email)
        {
            //Note: currently unable to attach files when using ActionSendMultiple
            var emailIntent = new Intent(Android.Content.Intent.ActionSend);
            emailIntent.SetType("message/rfc822");
            emailIntent.PutExtra(Android.Content.Intent.ExtraEmail, email.ToAddresses.ToArray());
            if (email.CcAddresses != null && email.CcAddresses.Count > 0 && !string.IsNullOrWhiteSpace(email.CcAddresses[0]))
            {
                //currently assuming there will only be 1 cc address, TODO: confirm this
                emailIntent.PutExtra(Android.Content.Intent.ExtraCc, email.CcAddresses.ToArray());
            }
            emailIntent.PutExtra(Android.Content.Intent.ExtraSubject, email.Subject);

            if (email.IsHtml)
            {
                emailIntent.SetType("text/html");
                emailIntent.PutExtra(Intent.ExtraText, Android.Text.Html.FromHtml(email.Body));
            }
            else
            {
                emailIntent.SetType("text/plain");
                emailIntent.PutExtra(Intent.ExtraText, email.Body);
            }

            if (!string.IsNullOrWhiteSpace(email.AttachmentFileLocation))
            {
                var file = new Java.IO.File(email.AttachmentFileLocation);
                var uri = Android.Net.Uri.FromFile(file);

                file.SetReadable(true, false);
                emailIntent.PutExtra(Intent.ExtraStream, uri);
            }

            //Needed to add the "New Task" flag here or it was crashing in the emulator
            var chooserIntent = Intent.CreateChooser(emailIntent, "Send mail...");
            chooserIntent.AddFlags(ActivityFlags.NewTask);

            Android.App.Application.Context.StartActivity(chooserIntent);
        }

        public bool CanSendEmail()
        {
			return true; //TODO: actual logic?
		}
	}
}



