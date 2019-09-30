using System;
using System.Collections.Generic;
using USAID.Base;
using USAID.Common;
using USAID.Enumerations;
using USAID.ViewModels;
using Xamarin.Forms;

namespace USAID.Pages
{
    public class GuarantorPageBase : ViewPage<GuarantorViewModel>
    {

    }

    public partial class GuarantorPage : GuarantorPageBase
    {

        public GuarantorPage()
        {
            InitializeComponent();

            NavigationPage.SetBackButtonTitle(this, string.Empty);

            statePicker.Items.Add(string.Empty);
            foreach (var state in Constants.States)
            {
                statePicker.Items.Add(state);
            }
        }

        void StatePickerValueChanged(object sender, System.EventArgs e)
        {
            ViewModel.State = statePicker.Items[statePicker.SelectedIndex];
        }

        async void OnNextButtonClicked(object sender, EventArgs args)
        {
            //assuming this will always be the last page before submission
            var result = await ViewModel.SubmitCreditApp();
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

