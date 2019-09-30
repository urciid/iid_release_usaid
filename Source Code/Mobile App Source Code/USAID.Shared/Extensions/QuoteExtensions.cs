using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using USAID.ApplicationObjects;
using USAID.Models;
using USAID.Repositories;
using USAID.Utilities;
using System.Linq;

namespace USAID.Extensions
{
	public static class QuoteExtensions
	{

		#region Email	
		public static Email ToQuoteEmail(this Quote quote)
		{
			StringBuilder sb = new StringBuilder();

			EmailBuilder.AddBoldedHeader(sb, "Thank you for calculating a monthly payment for a " + quote.EquipmentDescription + "  with requested financing of " + String.Format("{0:C}", Convert.ToInt32(quote.EquipmentAmount)) + ".");
			EmailBuilder.AddNewline(sb);
			EmailBuilder.AddRegularText(sb, "Here are your monthly payment options available, subject to credit approval.");

			foreach (var purchaseOption in quote.PurchaseOptions)
			{
				AddPaymentOptions(sb, purchaseOption, quote.MonthlyPayments);
			}

			var profileInfoRepository = AppContainer.Container.Resolve<IProfileInfoRepository>();
			var profileInfo = profileInfoRepository.GetDealerProfile();

			EmailBuilder.AddNewline(sb);
			EmailBuilder.AddItalicizedText(sb, "Applicable taxes not included.");
			EmailBuilder.AddNewline(sb);
			EmailBuilder.AddRegularText(sb, "Thank you!");

			var email = new Email
			{
				ToAddresses = new List<string> { profileInfo.DealerContactEmail },
				Subject = "My SnappShot Quote",
				Body = sb.ToString(),
				IsHtml = true
			};
			if (!string.IsNullOrWhiteSpace(profileInfo.AutoCcEmail))
			{
				email.CcAddresses = new List<string> { profileInfo.AutoCcEmail };
			}
			return email;
		}

		private static void AddPaymentOptions(StringBuilder sb, PurchaseOption purchaseOpt, List<MonthlyPayment> monthlyPayments) 
		{
			var paymentOptions = monthlyPayments.Where(mp => mp.PurchaseOption.Equals(purchaseOpt.PurchaseOptionDesc)); 

			EmailBuilder.AddNewline(sb);
			foreach (var paymentOption in paymentOptions)
			{
				// logic to determine singular or plural verbiage for advanced payment(s)
				string ap = (paymentOption.AdvancementPayment == 1 ? "Advanced Payment" : "Advanced Payments");

				EmailBuilder.AddBoldedHeader(sb, purchaseOpt.PurchaseOptionDesc);
				EmailBuilder.AddRegularText(sb, paymentOption.Term + " months at " + String.Format("{0:C}", Convert.ToInt32(paymentOption.Payment)) + " with " + paymentOption.AdvancementPayment + " " + ap);

				EmailBuilder.AddNewline(sb);
			}
		}
		#endregion



		#region Sms
		public static Sms ToQuoteSms(this Quote quote)
		{
			var sms = new Sms();
			StringBuilder sb = new StringBuilder();
			sb.Append("Pymt Options for " + String.Format("{0:C}", quote.EquipmentAmount) + " " + quote.EquipmentDescription + ": ");
			foreach (var purchaseOption in quote.PurchaseOptions)
			{
				AddSms(quote, sb, purchaseOption, quote.MonthlyPayments);
			}

			var profileInfoRepository = AppContainer.Container.Resolve<IProfileInfoRepository>();
			var profileInfo = profileInfoRepository.GetDealerProfile();

			sms.Body = sb.ToString();
			sms.Recipients = new List<string>(new string[] { profileInfo.DealerContactPhone });

			return sms;
		}

		private static void AddSms(Quote quote, StringBuilder sb, PurchaseOption purchaseOpt, List<MonthlyPayment> monthlyPayments)
		{
			var paymentOptions = monthlyPayments.Where(mp => mp.PurchaseOption.Equals(purchaseOpt.PurchaseOptionDesc));

			for (int x = 0; x < paymentOptions.Count(); x++)
			{
				var paymentOption = paymentOptions.ElementAt(x);

				// logic to determine singular or plural verbiage for advanced payment(s)
				string ap = (paymentOption.AdvancementPayment == 1 ? "Adv Pymt" : "Adv Pymts");

				string comma = (x < paymentOptions.Count() - 1 ? ", " : "");

				sb.Append(purchaseOpt.PurchaseOptionDesc + " - " + paymentOption.Term + " mos at " + String.Format("{0:C}", paymentOption.Payment) + " with " + paymentOption.AdvancementPayment + " " + ap + comma);
			}
		}
		#endregion

	}
}

