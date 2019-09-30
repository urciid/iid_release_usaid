using System;
using USAID.Interfaces;
using USAID.Models;
using MessageUI;
using UIKit;

namespace USAID.iOS.Providers
{
    public class SmsService : ISmsService
    {
        public void CreateSms(Sms sms)
        {
            if (CanSendSms())
            {
                var smsController = new MFMessageComposeViewController();
                smsController.Body = sms.Body;
                smsController.Recipients = sms.Recipients.ToArray();
                smsController.Finished += (s, args) => args.Controller.DismissViewController(true, null);
                var rootViewController = UIApplication.SharedApplication.KeyWindow.RootViewController;
                rootViewController.PresentViewController(smsController, true, null);
            }
            else
            {
                App.CurrentApp.MainPage.DisplayAlert("No SMS App Available", "Unable to send SMS.", "OK");
            }
        }

        public bool CanSendSms()
        {
            return MFMessageComposeViewController.CanSendText;
        }
    }
}

