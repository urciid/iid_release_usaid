using System;
using System.Collections.Generic;
using USAID.Base;
using USAID.Common;
using USAID.ViewModels;
using Xamarin.Forms;

namespace USAID.Pages
{
	public class QuotePageBase : ViewPage<QuoteViewModel>
	{
	}

	public partial class QuotePage : QuotePageBase
	{
		public QuotePage ()
		{
			InitializeComponent ();
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
			this.ViewModel.LoadCommand.Execute (null);

			//lstMonthlyPayments.ItemAppearing += (sender, e) => {
			//	var list = ViewModel.MonthlyPayments;
			//	lstMonthlyPayments.HeightRequest = (lstMonthlyPayments.RowHeight * list.Count) + 20;
			//};

		}

		async void OnNextButtonClicked (object sender, EventArgs args)
		{

			var payments = ViewModel.SubmitMonthlyPayments ();
			if (payments) {
				await Navigation.PushAsync (new MonthlyPaymentsPage ());
			} else {
				await DisplayAlert ("Monthly Payments", "Please select a monthly payment.", "OK");
			}
		}

		async void OnSaveButtonClicked (object sender, EventArgs args)
		{

			var saved = ViewModel.Save ();
			if (saved) {
				await DisplayAlert ("Quote", "Quote Saved Successfully.", "OK");
			} 
		}

		void OnCreditAppButtonClicked (object sender, EventArgs args)
		{
            //go to credit app, should have populated _quotebuilder.getQuote
            ViewModel.PopulateCreditApp();
            App.TabPageRoot.CurrentPage = App.TabPageRoot.Children[Constants.CreditAppTab];
		}

		void OnMailButtonClicked (object sender, EventArgs args)
		{
			// go to credit app, should have populated _quotebuilder.getQuote
		    // await DisplayAlert ("Quote", "launch email with quote info", "OK");
			ViewModel.SendQuoteEmail();
		}

		void OnMessageButtonClicked (object sender, EventArgs args)
		{
			// go to credit app, should have populated _quotebuilder.getQuote
			// await DisplayAlert ("Quote", "launch text with quote info", "OK");
			ViewModel.SendSmsMessage();
		}
	}
}

