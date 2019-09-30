using System;
using System.Collections.Generic;
using USAID.Base;
using USAID.ViewModels;
using Xamarin.Forms;
using System.Reflection;
using USAID.Resx;
using USAID.Common;

namespace USAID.Pages
{
	public class LoginPageBase : ViewPage<LoginViewModel>
	{
	}
	public partial class LoginPage : LoginPageBase
	{
        private bool _keysInvalidated;

		public LoginPage(bool keysInvalidated = false)
		{
			InitializeComponent();
            _keysInvalidated = keysInvalidated;
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_keysInvalidated)
            {
                ShowLoginAlert();
                ViewModel.SetKeyFields();
            }

            MessagingCenter.Subscribe<LoginViewModel>(this, "InvalidLogin", (s) =>
            {
                ShowLoginAlert();
            });

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            MessagingCenter.Unsubscribe<LoginViewModel>(this, "InvalidLogin");
        }

        private void ShowLoginAlert()
        {
			DisplayAlert(AppResources.ErrorText, AppResources.LoginErrorText, AppResources.OKButton);
        }

		void LoginHelpButton_Clicked(object sender, EventArgs e)
		{
			//DisplayAlert("Help", "To activate, please contact your Sales Rep and request access to SnappShot.", "OK");
			var email = string.Format("mailto:{0}", Constants.CreateProfileEmailAddress);
			Device.OpenUri(new Uri(email));
		}
	}
}

