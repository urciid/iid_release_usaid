using System;
using System.Collections.Generic;
using USAID.Base;
using USAID.Models;
using USAID.ViewModels;
using Xamarin.Forms;

namespace USAID.Pages
{
	public class QuotesPageBase : ViewPage<QuotesViewModel>
	{
	}

	public partial class QuotesPage : QuotesPageBase
	{
		public QuotesPage()
		{
			InitializeComponent();
		}

		public async void OnItemTapped(object sender, ItemTappedEventArgs e)
		{
			Quote quote = (Quote)e.Item;

			this.ViewModel.QuoteCommand.Execute (quote);
			await Navigation.PushAsync (new QuotePage());
			((ListView)sender).SelectedItem = null; // de-select the row
		}

		public async void OnDelete(object sender, EventArgs e)
		{
			
			var answer = await DisplayAlert("Quote", "Are you sure you want to delete quote?", "Yes", "No");
			if (answer)
			{
				var mi = ((MenuItem)sender);
				this.ViewModel.DeleteCommand.Execute(mi.CommandParameter);
			}


		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			this.ViewModel.LoadCommand.Execute(null);

		}

		async void AddQuoteClicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new RateOptionsPage() { Title = USAID.Common.Constants.RateOptionsTitle });
		}

		void HelpButton_Clicked(object sender, EventArgs e)
		{
			Navigation.PushAsync(new HelpPage());
		}
	}
}

