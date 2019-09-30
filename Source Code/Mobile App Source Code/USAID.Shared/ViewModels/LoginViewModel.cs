using USAID.Interfaces;
using USAID.Base;
using USAID.Utilities;
using System.Windows.Input;
using Xamarin.Forms;
using USAID.Resx;
using System;
using USAID.Common;


namespace USAID.ViewModels
{
    public class LoginViewModel : BaseViewModel
	{
        private readonly IAuthenticationManager _authManager;
        private readonly IUserInfoService _userInfoService;
        private readonly IHUDProvider _hudProvider;

		public string WelcomeLabel
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public string EmailHintText
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}
		public string PasswordHintText
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}
		public string LoginButtonText
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}
		public string CreateProfileLabel
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}
		public string CreateProfileEmailAddress
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}
		public string ForgotPasswordLabel
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}


        public string Email
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string Password
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }



        public LoginViewModel(IGALogger logger, IAuthenticationManager authManager, IUserInfoService userInfoService,
            IHUDProvider hudProvider)
		{
			_authManager = authManager;
            _userInfoService = userInfoService;
            _hudProvider = hudProvider;

            Email = string.Empty;
			Password = string.Empty;

			PageTitle = AppResources.LoginPageTitle;
			WelcomeLabel = AppResources.WelcomeLabel;
			EmailHintText = AppResources.EmailHint;
			PasswordHintText = AppResources.PassowrdHint;
			CreateProfileLabel = AppResources.ToCreatePofileLabel;
			CreateProfileEmailAddress = Constants.CreateProfileEmailAddress;
			ForgotPasswordLabel = AppResources.ForgotPasswordLink;


			LoginButtonText = AppResources.LoginButtonText;
		}

		public ICommand LoginCommand { get { return new SimpleCommand(Login); } }

		private async void Login()
		{
//#if DEBUG
//			//TODO: remove this (added to simplify testing
//			Email = "support@arseneauinc.com"; //Derek
//			Password = "S@r@hL3@h";
//#endif
			if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
			{
				MessagingCenter.Send(this, "InvalidLogin");
				return;
			}
			_userInfoService.SaveUserInfo(new Models.UserInfoModel
			{
				Email = Email,
				Password = Password,
				Token = "",
				LastDownload = ""
            });

			_hudProvider.DisplayProgress(AppResources.WorkingText);

			bool authenticated = await _authManager.Authenticate();
		
			if (authenticated)
			{
				App.ShowHomeScreen();
			}
			else
			{
               MessagingCenter.Send(this, "InvalidLogin");
            }

            _hudProvider.Dismiss();
		}

        internal void SetKeyFields()
        {
            var userProfile = _userInfoService.GetSavedInfo();
			Email = userProfile.Email;
			Password = userProfile.Password;
        }
	}
}
