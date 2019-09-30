using System;
using System.Collections.Generic;
using USAID.Base;
using USAID.ViewModels;
using Xamarin.Forms;

namespace USAID.Pages
{
    public class ProfilePageBase : ViewPage<ProfileViewModel>
    {
        
    }

    public partial class ProfilePage : ProfilePageBase
    {
        private Action _modalDismissedCallback;

        public ProfilePage(Action modalDismissedCallback = null, bool missingInformation = false)
        {
            InitializeComponent();
            if (modalDismissedCallback != null)
            {
                _modalDismissedCallback = modalDismissedCallback;
            }

            if (missingInformation) // must enter missing information to continue
            {
                cancelButton.IsVisible = false;
            }
            else
            {
                cancelButton.IsVisible = true;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.PopulateFields();
        }

        async void OnSaveButtonClicked(object sender, EventArgs args)
        {
            var valid = ViewModel.SaveInfo();

            if (!valid)
            {
               await DisplayAlert("Missing Fields", "Please fill in all fields before saving.", "OK");
            }
            else
            {
                if (_modalDismissedCallback != null)
                {
                    _modalDismissedCallback.Invoke();
                }
                await Navigation.PopModalAsync();
            }
        }

        async void OnCancelButtonClicked(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync();
        }

		void HelpButton_Clicked(object sender, EventArgs e)
		{
			Navigation.PushAsync(new HelpPage());
		}
    }
}

