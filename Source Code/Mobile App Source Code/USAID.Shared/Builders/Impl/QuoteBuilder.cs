using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using USAID.ApplicationObjects;
using USAID.Extensions;
using USAID.Interfaces;
using USAID.Models;

namespace USAID.Builders.Impl
{
	public class QuoteBuilder : IQuoteBuilder
	{
		private Quote _quote;

		public Quote GetQuote()
		{
			return _quote;
		}

		public void SetQuote (Quote quote)
		{
			_quote = quote;
		}

		public void CreateQuote()
		{
			_quote = new Quote();
			_quote.Created = DateTime.Now;
		}

		public void SetRateOptions(RateOptions rateOptions)
		{
			_quote.CompanyName = rateOptions.CompanyName;
			_quote.EquipmentAmount = rateOptions.EquipmentAmount;
			_quote.EquipmentDescription = rateOptions.EquipmentDescription;
			_quote.Points = rateOptions.Points;
			_quote.PassThrough = rateOptions.PassThrough;
			//rate cards that are selected
			_quote.RateCards = rateOptions.RateCards;
			_quote.Terms = rateOptions.Terms;
			_quote.MaintenanceTypes = rateOptions.MaintenanceTypes;
			_quote.PurchaseOptions = rateOptions.PurchaseOptions;
			_quote.AdvancePayments = rateOptions.AdvancePayments;

		}

		public void SetMonthlyPayments(List<MonthlyPayment> monthlyPayments)
		{
			_quote.MonthlyPayments = monthlyPayments;
		}

		public bool EmailQuote()
		{
			IEmailService _emailService = AppContainer.Container.Resolve<IEmailService>();
			_emailService.CreateEmail(_quote.ToQuoteEmail());
			return true;
		}

		public bool SmsQuote()
		{
			ISmsService _smsService = AppContainer.Container.Resolve<ISmsService>();
			_smsService.CreateSms(_quote.ToQuoteSms());
			return true;
		}

	}
}

