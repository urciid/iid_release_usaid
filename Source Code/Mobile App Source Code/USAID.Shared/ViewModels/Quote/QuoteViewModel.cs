using System.Threading.Tasks;
using WMP.Core.Data.SQLiteNet;
using USAID.Base;
using USAID.Interfaces;
using Xamarin.Forms;
using SQLite.Net;
using USAID.Services;
using USAID.Models;
using System.Collections.Generic;
using USAID.Pages;
using USAID.Builders;
using System.Linq;
using System;
using USAID.Repositories;
using USAID.Extensions;
using USAID.ApplicationObjects;
using Autofac;

namespace USAID.ViewModels
{
	public class QuoteViewModel : BaseViewModel
	{

		private readonly IQuoteBuilder _quoteBuilder;
		private readonly IQuoteRepository _quoteRepo;
		private readonly IEmailService _emailService;
		private readonly ISmsService _smsService;
		private readonly ICreditAppBuilder _creditAppBuilder;

		public QuoteViewModel (ISmsService smsService, IEmailService emailService, IGALogger logger, IQuoteBuilder quoteBuilder, IQuoteRepository quoteRepo)
		{
			_quoteBuilder = quoteBuilder;
			_quoteRepo = quoteRepo;
			_emailService = emailService;
			_smsService = smsService;
		}

		public string CompanyName {
			get { return GetValue<string> (); }
			set { SetValue (value); }
		}
		public string Created {
			get { return GetValue<string> (); }
			set { SetValue (value); }
		}

		#region Properties
		public List<MonthlyPayment> MonthlyPayments {
			get { return GetValue<List<MonthlyPayment>> (); }
			set { SetValue (value); }
		}
		#endregion

		private void Load ()
		{
			IsBusy = true;
			var quote = _quoteBuilder.GetQuote ();
			MonthlyPayments = quote.MonthlyPayments;
			CompanyName = quote.CompanyName;
			Created = quote.Created.ToString ("MM/dd/yyyy");
			IsBusy = false;
		}

		internal bool SubmitMonthlyPayments ()
		{
			var monthlyPayments = MonthlyPayments.Where (t => t.IsSelected);
			if (monthlyPayments.Count () > 1) {
				_quoteBuilder.SetMonthlyPayments (monthlyPayments.ToList ());
				return true;
			} else
				return false;
		}

		internal bool Save ()
		{
			var quote = _quoteBuilder.GetQuote ();
			_quoteRepo.Upsert (quote);
			return true;
		}

        internal void PopulateCreditApp()
        {
            var creditAppBuilder = AppContainer.Container.Resolve<ICreditAppBuilder>();
            var quote = _quoteBuilder.GetQuote();

            var maxTerm = quote.MonthlyPayments.Max(mp => mp.Term);
            var monthlyPaymentWithMaxTerm = quote.MonthlyPayments.FirstOrDefault(mp => Math.Abs(mp.Term - maxTerm) < double.Epsilon); //double comparison: http://stackoverflow.com/questions/6598179/the-right-way-to-compare-a-system-double-to-0-a-number-int
            creditAppBuilder.CreateCreditApp(new CreditApp
            {
                FromQuoteWorkflow = true,
                CompanyName = quote.CompanyName,
                TotalAmount = quote.EquipmentAmount,
                EquipmentDescription = quote.EquipmentDescription,
                RateCardId = monthlyPaymentWithMaxTerm.RateCardID,
                DesiredFinanceTerm = (int) monthlyPaymentWithMaxTerm.Term,
                Payment = monthlyPaymentWithMaxTerm.Payment,
                DesiredPurchaseOption = monthlyPaymentWithMaxTerm.PurchaseOption,
                MaintenanceFeeAmount = quote.PassThrough
            });
        }

		internal void SendQuoteEmail()
		{
			var app = _quoteBuilder.GetQuote();
			_emailService.CreateEmail(app.ToQuoteEmail());
		}

		internal void SendSmsMessage()
		{
			var app = _quoteBuilder.GetQuote();
			_smsService.CreateSms(app.ToQuoteSms());
		}

		Command loadCommand;

		public Command LoadCommand {
			get { return loadCommand ?? (loadCommand = new Command (ExecuteLoadCommand)); }
		}

		private void ExecuteLoadCommand (object option)
		{
			Load ();
		}

	}
}

