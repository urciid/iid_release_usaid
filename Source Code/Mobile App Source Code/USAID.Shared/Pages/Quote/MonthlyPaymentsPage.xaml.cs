using System;
using System.Collections.Generic;
using USAID.Base;
using USAID.ViewModels;
using Xamarin.Forms;

namespace USAID.Pages
{
	public class MonthlyPaymentsPageBase : ViewPage<MonthlyPaymentsViewModel>
	{
	}

	public partial class MonthlyPaymentsPage : MonthlyPaymentsPageBase
	{
		public MonthlyPaymentsPage ()
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

			var payments = ViewModel.SubmitMonthlyPayments();
			if (payments) {
				await Navigation.PushAsync (new QuotePage ());
			} else {
				await DisplayAlert ("Monthly Payments", "Please select a monthly payment.", "OK");
			}
		}
	}
}

