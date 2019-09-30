using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace IID.BusinessLayer.Helpers
{
    public static class SendGrid
    {
        public static async Task SendEmail(string recipient, string subject, string body, bool isHtml)
        {
            await SendEmail(new string[] { recipient }, subject, body, isHtml, null);
        }

        public static async Task SendEmail (string[] recipients, string subject, string body, bool isHtml)
        {
            await SendEmail(recipients, subject, body, isHtml, null);
        }

        public static async Task SendEmail(string recipient, string subject, string body, bool isHtml, Dictionary<string, MemoryStream> attachments)
        {
            await SendEmail(new string[] { recipient }, subject, body, isHtml, attachments);
        }

        public static async Task SendEmail(string[] recipients, string subject, string body, bool isHtml, Dictionary<string, MemoryStream> attachments)
        {
            string apiKey = Settings.SendGrid.ApiKey;
            var client = new SendGridClient(apiKey);

            var msg = new SendGridMessage();
            msg.From = new EmailAddress("noreply@" + Settings.Domain);
            msg.Subject = subject;
            if (isHtml)
                msg.HtmlContent = body;
            else
                msg.PlainTextContent = body;
            foreach (string recipient in recipients)
                msg.AddTo(new EmailAddress(recipient));

            if (attachments != null)
            {
                foreach (KeyValuePair<string, MemoryStream> kvp in attachments)
                    msg.AddAttachment(kvp.Key, Convert.ToBase64String(kvp.Value.ToArray()), disposition: "attachment");
            }

            await client.SendEmailAsync(msg);
        }

        public static async Task SendEmail(IdentityMessage message)
        {
            await SendEmail(message.Destination, message.Subject, message.Body.Replace(Environment.NewLine, "<br />"), true);
        }

        public static async Task SendExceptionEmail(Exception ex, string url, string userName, string language)
        {
            string subject = String.Format("IID Exception ({0})", Settings.Environment);
            string body = String.Format(
                "An error occurred on the <b>{0}</b> web site:<br><br>URL: {1}<br><br>User: {2}<br><br>Language: {3}<br><br><b>{4}</b><br>{5}",
                Settings.Environment, url, userName, language, ex.Message, ex.StackTrace);
            await SendEmail(Settings.ExceptionReportRecipients, subject, body, true);
        }
    }
}