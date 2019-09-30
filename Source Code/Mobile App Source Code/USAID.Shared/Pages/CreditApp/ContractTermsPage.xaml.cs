using System;
using System.Collections.Generic;
using USAID.Base;
using USAID.Common;
using USAID.ViewModels;
using Xamarin.Forms;
using USAID.Extensions;
using USAID.Enumerations;

namespace USAID.Pages
{
    public class ContractTermsPageBase : ViewPage<ContractTermsViewModel>
    {
		
    }

    public partial class ContractTermsPage : ContractTermsPageBase
    {
        public ContractTermsPage()
        {
            InitializeComponent();

            NavigationPage.SetBackButtonTitle(this, string.Empty);

            if (!ViewModel.ShowPassThru)
            {
                passThruField.IsVisible = false;
            }

            if (!ViewModel.ShowGuarantorPage)
            {
                nextButton.Text = "Submit";
            }

            termPicker.Items.Add(string.Empty); //empty for first field
            foreach (var term in Constants.Terms)
            {
                termPicker.Items.Add(term.ToString());
            }

            purchaseOptionPicker.Items.Add(string.Empty); //empty for first field
            foreach (var option in Constants.PurchaseOptions)
            {
                purchaseOptionPicker.Items.Add(option);
            }
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.PopulateFields(); 

            //set initial picker values (has existing value if coming from quote workflow)
            termPicker.SelectedIndex = termPicker.Items.IndexOf(ViewModel.DesiredFinanceTerm.ToString());
            purchaseOptionPicker.SelectedIndex = purchaseOptionPicker.Items.IndexOf(ViewModel.DesiredPurchaseOption);
        }

        void TermPickerValueChanged(object sender, System.EventArgs e)
        {
            ViewModel.DesiredFinanceTerm = termPicker.Items[termPicker.SelectedIndex].AsInt();
        }

        void PurchaseOptionPickerValueChanged(object sender, System.EventArgs e)
        {
            ViewModel.DesiredPurchaseOption = purchaseOptionPicker.Items[purchaseOptionPicker.SelectedIndex];
        }

        async void OnNextButtonClicked(object sender, EventArgs args)
        {
            ViewModel.SetContractTermsOnCreditApp();

            //this will be the last page before submission unless the Personal Guarantor section is turned on (dealer defaults)
            var guarantorSectionOn = ViewModel.ShowGuarantorPage;

            if (guarantorSectionOn)
            {
                await Navigation.PushAsync(new GuarantorPage());
            }
            else
            {
                var result = await ViewModel.SubmitCreditApp(guarantorSectionOn);
                switch (result)
                {
                    case CreditAppSubmissionResult.Unauthorized:
                        DisplayAlert("Authorization Error", "Please contact your Sales Rep for assistance.", "OK");
                        break;
                    case CreditAppSubmissionResult.Failure:
                        DisplayAlert("Submission Error", "There was an error submitting your credit application.", "OK");
                        break;
                    default:
                        await Navigation.PushAsync(new SubmissionConfirmationPage());
                        break;
                }
            }
        }
    }
}

