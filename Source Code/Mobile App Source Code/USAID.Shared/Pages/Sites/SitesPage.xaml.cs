using System;
using System.Collections.Generic;
using USAID.Base;
using USAID.Models;
using USAID.Resx;
using USAID.ViewModels;
using Xamarin.Forms;

namespace USAID.Pages
{
	public class SitesPageBase : ViewPage<SitesViewModel>
	{
	}
	public partial class SitesPage : SitesPageBase
	{
		void Handle_BindingContextChanged(object sender, System.EventArgs e)
		{
			base.OnBindingContextChanged();

			if (BindingContext == null)
				return;

			ViewCell theViewCell = ((ViewCell)sender);
			if (theViewCell.ContextActions.Count> 0)
				theViewCell.ContextActions[0].Text = AppResources.DetailsText;
		}

		public SitesPage()
		{
			InitializeComponent();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			ViewModel.LoadSites();
		}

		public async void OnItemTapped(object sender, ItemTappedEventArgs e)
		{
			//base.OnItemTapped(sender, e);

			Site site = (Site)e.Item;

			//ViewModel.SetQuote(quote);
			await Navigation.PushAsync(new ActivitiesPage(site));
		}



		public async void OnDetails(object sender, EventArgs e)
		{
			var mi = ((MenuItem)sender);
			Site site = (Site)mi.CommandParameter;
			await Navigation.PushAsync(new SitePage(site));
		}
	}
}

