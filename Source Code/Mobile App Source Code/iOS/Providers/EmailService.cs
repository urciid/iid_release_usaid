using USAID.Interfaces;
using USAID.Models;
using Xamarin.Forms;
using USAID.iOS.Providers;
using MessageUI;
using UIKit;
using System.IO;

[assembly: Dependency(typeof(EmailService))]
namespace USAID.iOS.Providers
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
		/// Opens up the ios Email Controller with pre-populated values
		/// </summary>
		/// <param name="email">Email.</param>
		public void CreateEmail(Email email)
		{
			if (CanSendEmail()) 
			{
				var mailController = new MFMailComposeViewController ();
                mailController.SetMessageBody (email.Body, email.IsHtml);
				mailController.SetSubject (email.Subject);
				mailController.SetToRecipients (email.ToAddresses.ToArray ());

                if (email.CcAddresses != null && email.CcAddresses.Count > 0 && !string.IsNullOrWhiteSpace(email.CcAddresses[0]))
                {
                    //currently assuming there will only be 1 cc address, TODO: confirm this
                    mailController.SetCcRecipients(email.CcAddresses.ToArray());
                }

                if (!string.IsNullOrWhiteSpace(email.AttachmentFileLocation))
                {
                    UIImage image = new UIImage(email.AttachmentFileLocation);
                    mailController.AddAttachmentData(image.AsJPEG(), "image/jpeg", Path.GetFileName(email.AttachmentFileLocation));
                }

				mailController.Finished += ( s, args) => args.Controller.DismissViewController (true, null);
				var rootViewController = UIApplication.SharedApplication.KeyWindow.RootViewController;
				rootViewController.PresentViewController (mailController, true, null);
			}
            else
            {
                App.CurrentApp.MainPage.DisplayAlert("No Email App Available", "Unable to send email.", "OK");
            }
		}

		public bool CanSendEmail()
        {
			return MFMailComposeViewController.CanSendMail;
		}
	}
}



