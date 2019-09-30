using System;
using System.Collections.Generic;
using USAID.Base;
using USAID.Models;
using USAID.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace USAID.Pages
{
	public class ChangePageBase : ViewPage<ChangeViewModel>
	{
	}
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ChangePage : ChangePageBase
	{
		

		private ObservationChange _change;
		public ChangePage(ObservationChange change = null)
		{
			InitializeComponent();
			if (Device.OS == TargetPlatform.Android)
			{
				BoxView.IsVisible = false;
			}
			buttonLayout.BackgroundColor = Color.FromHex("#034468");
			NavigationPage.SetBackButtonTitle(this, "Cancel");
			if (change == null)
			{
				_change = new ObservationChange { Start_Date = DateTime.Now};
			}
			else {
				_change = change;
			}
			ViewModel.Change = _change;
			ViewModel.OriginalChange = _change;
		}

		async void CancelClicked(object sender, System.EventArgs e)
		{
			var answer = await DisplayAlert("Unsaved Changes", "Are you sure you want to leave without saving?", "Yes", "No");
			if (answer)
			{
				await Navigation.PopAsync(true);
			}
		}
	}
}
