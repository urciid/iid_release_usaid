using System;
using System.Text;
using Android.Content;
using USAID.Interfaces;
using USAID.Models;

namespace GA.Droid.Providers
{
    public class SmsService : ISmsService
    {
        public void CreateSms(Sms sms)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(sms.Recipients[0]);
            //how to send to multiple numbers: http://stackoverflow.com/questions/11176270/how-to-send-a-sms-to-many-recipients
            //unfortunately this doesn't seem to always work, not sure if there's a surefire way in Android to send to multiple recipients
            for (int i = 1; i < sms.Recipients.Count; i++)
            {
                sb.Append(";" + sms.Recipients[i]);
            }
            var smsUri = Android.Net.Uri.Parse("smsto:" + sb.ToString());
            var smsIntent = new Intent(Intent.ActionSendto, smsUri);
            smsIntent.AddFlags(ActivityFlags.NewTask);
            smsIntent.PutExtra("sms_body", sms.Body);
            Android.App.Application.Context.StartActivity(smsIntent);
        }

        public bool CanSendSms()
        {
            return true; //actual logic?
        }
    }
}

