using System;
using System.Collections.Generic;
using USAID.Base;
using USAID.Models;
using USAID.ViewModels;
using Xamarin.Forms;

namespace USAID.Pages
{
	public class RateOptionsPageBase : ViewPage<RateOptionsViewModel>
	{
	}

	public partial class RateOptionsPage : RateOptionsPageBase
	{
		public RateOptionsPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

            var result = await ViewModel.LoadRateOptions();

            if (!result) //rate cards failed to load
            {
                await DisplayAlert("Authorization Error", "Please contact your Sales Rep for assistance.", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                if (!ViewModel.ShowPoints)
                {
                    pointsField.IsVisible = false;
                }

                if (!ViewModel.ShowPassThru)
                {
                    passThruField.IsVisible = false;
                }

                lstRateCards.ItemAppearing += (sender, e) =>
                {
                    var list = ViewModel.RateCards;
                    lstRateCards.HeightRequest = (lstRateCards.RowHeight * list.Count) + 20;
                };

                lstPurchaseOptions.ItemAppearing += (sender, e) =>
                {
                    var list = ViewModel.PurchaseOptions;
                    lstPurchaseOptions.HeightRequest = (lstPurchaseOptions.RowHeight * list.Count) + 20;
                };

                lstTerms.ItemAppearing += (sender, e) =>
                {
                    var list = ViewModel.Terms;
                    lstTerms.HeightRequest = (lstTerms.RowHeight * list.Count) + 20;
                };
                lstMaintenanceTypes.ItemAppearing += (sender, e) =>
    			{
    				var list = ViewModel.MaintenanceTypes;
    				lstMaintenanceTypes.HeightRequest = (lstMaintenanceTypes.RowHeight * list.Count) + 20;
    			};
				lstAdvancePayments.ItemAppearing += (sender, e) =>
			   {
					var list = ViewModel.AdvancePayments;
					lstAdvancePayments.HeightRequest = (lstAdvancePayments.RowHeight * list.Count) + 20;
			   };

    			mainScrollView.IsVisible = true;
            }
		}


		async void OnNextButtonClicked(object sender, EventArgs args)
		{
            var result = await ViewModel.SubmitRateOptions();
            switch (result)
            {
                case Enumerations.RateOptionsSubmissionResult.UnableToRetrieveData:
                    await DisplayAlert("No Monthly Payments Available", "Unable to calculate payments within the parameters selected.  Please contact your Sales Rep for assistance.", "OK");
                    break;
                case Enumerations.RateOptionsSubmissionResult.Unauthorized:
                    await DisplayAlert("Authorization Error", "Please contact your Sales Rep for assistance.", "OK");
                    break;
                case Enumerations.RateOptionsSubmissionResult.Failure:
                    await DisplayAlert("No Monthly Payments Available", "Not able to return monthly payment options. Please select different parameters and try again.", "OK");
                    break;
                case Enumerations.RateOptionsSubmissionResult.Success:
                    await Navigation.PushAsync(new MonthlyPaymentsPage());
                    break;
                case Enumerations.RateOptionsSubmissionResult.InvalidEquipmentAmount:
                    await DisplayAlert("Invalid Equipment Amount", "The input equipment amount must be between " + ViewModel.MinEquipmentAmount + " and " + ViewModel.MaxEquipmentAmount + ".", "OK");
                    break;
				case Enumerations.RateOptionsSubmissionResult.InvalidPointAmount:
					await DisplayAlert("Invalid Points Amount", "The input points amount can not be greater than " + ViewModel.MaxPoints + ".", "OK");
					break;
            }
		}
	}
}

