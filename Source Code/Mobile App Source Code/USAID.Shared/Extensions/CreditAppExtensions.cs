using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using USAID.ApplicationObjects;
using USAID.Models;
using USAID.Repositories;
using USAID.Utilities;

namespace USAID.Extensions
{
    public static class CreditAppExtensions
    {
        // This is for submitting to GreatAmerica when a photo is attached for the customer info (rather than hitting the API)
        public static Email ToSubmissionEmail(this CreditApp app)
        {
            StringBuilder sb = new StringBuilder();

            EmailBuilder.AddItalicizedText(sb, "The business card of the applicant is attached to this email.");

            EmailBuilder.AddNewline(sb);
            AddSubmittedBySection(sb);
            EmailBuilder.AddNewline(sb);
            AddRequestDetailsSection(sb, app);

            return new Email
            {
                ToAddresses = new List<string> { "cmaier@westmonroepartners.com" },
                Subject = "SnappShot Credit App Submission",
                Body = sb.ToString(),
                IsHtml = true,
                AttachmentFileLocation = app.PhotoFilePath
            };
        }

        // This is for the confirmation email that users can send after submission
        public static Email ToConfirmationEmail(this CreditApp app)
        {
            StringBuilder sb = new StringBuilder();

            AddAppIdSection(sb, app.AppId);
            EmailBuilder.AddNewline(sb);
            AddSubmittedBySection(sb);
            EmailBuilder.AddNewline(sb);
            if (string.IsNullOrWhiteSpace(app.PhotoFilePath))
            {
                //do not have this info if took photo for customer info
                AddApplicantDetailsSection(sb, app);
                EmailBuilder.AddNewline(sb);
            }
            AddRequestDetailsSection(sb, app);

            var profileInfoRepository = AppContainer.Container.Resolve<IProfileInfoRepository>();
            var profileInfo = profileInfoRepository.GetDealerProfile();

            var email = new Email
            {
                ToAddresses = new List<string> { profileInfo.DealerContactEmail },
                Subject = "SnappShot Credit App Submission Confirmation",
                Body = sb.ToString(),
                IsHtml = true,
                AttachmentFileLocation = app.PhotoFilePath
            };
            if (!string.IsNullOrWhiteSpace(profileInfo.AutoCcEmail))
            {
                email.CcAddresses = new List<string> { profileInfo.AutoCcEmail };
            }
            return email;
        }

        private static void AddAppIdSection(StringBuilder sb, int appId)
        {
            EmailBuilder.AddRegularText(sb, "Application ID: " + appId);
        }

        //Submitted By
        private static void AddSubmittedBySection(StringBuilder sb)
        {
            EmailBuilder.AddBoldedHeader(sb, "Submitted By");
            var profileInfo = AppContainer.Container.Resolve<IProfileInfoRepository>().GetDealerProfile();
            EmailBuilder.AddIndentedText(sb, "Dealer Name: " + profileInfo.DealerName);
            EmailBuilder.AddIndentedText(sb, "Dealer Contact Name: " + profileInfo.DealerContactName);
            EmailBuilder.AddIndentedText(sb, "Dealer Contact Email: " + profileInfo.DealerContactEmail);
            EmailBuilder.AddIndentedText(sb, "Dealer Contact Phone: " + profileInfo.DealerContactPhone);
        }

        //Applicant Details
        private static void AddApplicantDetailsSection(StringBuilder sb, CreditApp app)
        {
            EmailBuilder.AddBoldedHeader(sb, "Applicant Details");
            EmailBuilder.AddIndentedText(sb, "Legal Name: " + app.CompanyName);
            EmailBuilder.AddIndentedText(sb, "DBA: " + app.DBA);
            EmailBuilder.AddIndentedText(sb, "Address1: " + app.MailingAddress);
            EmailBuilder.AddIndentedText(sb, "City: " + app.City);
            EmailBuilder.AddIndentedText(sb, "State/Province: " + app.State);
            EmailBuilder.AddIndentedText(sb, "Zip: " + app.PostalCode);
            EmailBuilder.AddIndentedText(sb, "Phone: " + app.PhoneNumber);
            EmailBuilder.AddIndentedText(sb, "Contact: " + app.ContactName);
            EmailBuilder.AddIndentedText(sb, "Contact Phone: " + app.ContactPhone);
            EmailBuilder.AddIndentedText(sb, "Contact Email: " + app.ContactEmail);
        }

        //Request Details
        private static void AddRequestDetailsSection(StringBuilder sb, CreditApp app)
        {
            EmailBuilder.AddBoldedHeader(sb, "Request Details");
            EmailBuilder.AddIndentedText(sb, "Equipment Description: " + app.EquipmentDescription);
            EmailBuilder.AddIndentedText(sb, "Equipment Amount: " + app.TotalAmount.ToString());
            EmailBuilder.AddIndentedText(sb, "Term: " + app.DesiredFinanceTerm.ToString());
            EmailBuilder.AddIndentedText(sb, "Purchase Option: " + app.DesiredPurchaseOption);
            EmailBuilder.AddIndentedText(sb, "Maintenance Fee Amount: " + app.MaintenanceFeeAmount.ToString());
            EmailBuilder.AddIndentedText(sb, "Payment: " + app.Payment.ToString());
            EmailBuilder.AddIndentedText(sb, "Notes: " + app.Comments);
        }
    }
}

