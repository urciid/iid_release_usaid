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

namespace USAID.ViewModels
{
	public class MonthlyPaymentsViewModel : BaseViewModel
	{
		
		private readonly IQuoteBuilder _quoteBuilder;

		public MonthlyPaymentsViewModel (IGALogger logger, IQuoteBuilder quoteBuilder)
		{
			_quoteBuilder = quoteBuilder;
		}

		#region Properties
		public List<MonthlyPayment> MonthlyPayments {
			get { return GetValue<List<MonthlyPayment>> (); }
			set { SetValue (value); }
		}
		#endregion

		private void LoadMonthlyPayments ()
		{
			IsBusy = true;
			var quote = _quoteBuilder.GetQuote ();
			MonthlyPayments = quote.MonthlyPayments;
			IsBusy = false;
		}

		internal bool SubmitMonthlyPayments ()
		{
			var monthlyPayments = MonthlyPayments.Where (t => t.IsSelected);
			if (monthlyPayments.Count () > 0) {
				_quoteBuilder.SetMonthlyPayments (monthlyPayments.ToList ());
				return true;
			} else
				return false;
		}


		Command loadCommand;

		public Command LoadCommand {
			get { return loadCommand ?? (loadCommand = new Command (ExecuteLoadCommand)); }
		}

		private void ExecuteLoadCommand (object option)
		{
			LoadMonthlyPayments ();
		}

	}
}

