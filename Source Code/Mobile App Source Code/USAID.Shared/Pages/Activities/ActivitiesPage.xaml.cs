using System;
using System.Collections.Generic;
using USAID.Base;
using USAID.Models;
using USAID.ViewModels;
using Xamarin.Forms;

namespace USAID.Pages
{
	public class ActivitiesPageBase : ViewPage<ActivitiesViewModel>
	{
	}
	public partial class ActivitiesPage : ActivitiesPageBase
	{
		
		public ActivitiesPage(Site site)
		{
			InitializeComponent();
			ViewModel.Site = site;

		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			ViewModel.LoadIndicators();
		}

		public async void OnItemTapped(object sender, ItemTappedEventArgs e)
		{
			//base.OnItemTapped(sender, e);

			IndicatorVM indicator = (IndicatorVM)e.Item;
			//ViewModel.SetQuote(quote);
			await Navigation.PushAsync(new ObservationPage(indicator, ViewModel.Site));
		}

		public async void OnObservation (object sender, EventArgs e)
		{
			var mi = ((Xamarin.Forms.StackLayout)sender);
			var indicator = (IndicatorVM)(mi.BindingContext);
			if (indicator != null)
			{
				await Navigation.PushAsync(new ObservationPage(indicator, ViewModel.Site));
			}
		}

		public async void OnChart(object sender, EventArgs e)
		{
			var mi = ((Xamarin.Forms.StackLayout)sender);
			var indicator = (IndicatorVM)(mi.BindingContext);
			if (indicator != null)
			{
				await Navigation.PushAsync(new ChartPage(indicator, ViewModel.Site.SiteId ));
			}
		}

		public async void OnDetails(object sender, EventArgs e)
		{
			var mi = ((Xamarin.Forms.StackLayout)sender);
			var indicator = (IndicatorVM)(mi.BindingContext);
			if (indicator != null)
			{
				await Navigation.PushAsync(new ActivityDetailPage(indicator.Activity));
			}
		}

		void Handle_BindingContextChanged(object sender, System.EventArgs e)
		{
			base.OnBindingContextChanged();

			if (BindingContext == null)
				return;

			ViewCell theViewCell = ((ViewCell)sender);
			var item = theViewCell.BindingContext as IndicatorVM;

			if (item != null)
			{
				System.Diagnostics.Debug.WriteLine(item.Name + " - " + item.Name.Length.ToString() );
				////check for height
				if (item.Name.Length >= 170)
				{
					theViewCell.Height = 170;
				}
				else if (item.Name.Length >= 160)
				{
					theViewCell.Height = 160;
				}
				else if (item.Name.Length >= 140)
				{
					theViewCell.Height = 160;
				}
				else if (item.Name.Length >= 130)
				{
					theViewCell.Height = 160;
				}
				else {
					theViewCell.Height = 140;
				}
			}
		}
	}
}

