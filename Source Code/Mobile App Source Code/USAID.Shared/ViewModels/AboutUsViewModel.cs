using USAID.Interfaces;
using USAID.Base;
using USAID.Utilities;
using System.Windows.Input;
using Xamarin.Forms;
using USAID.Resx;
using System;

namespace USAID.ViewModels
{
	public class AboutUsViewModel : BaseViewModel
	{
		private readonly IUserInfoService _userInfoService;
		private readonly IHUDProvider _hudProvider;
		private readonly IDeviceSettings _deviceSettings;

		public AboutUsViewModel(IGALogger logger, IUserInfoService userInfoService,
		                        IHUDProvider hudProvider, IDeviceSettings deviceSettings)
		{
			_userInfoService = userInfoService;
			_hudProvider = hudProvider;
			_deviceSettings = deviceSettings;

			PageTitle = AppResources.AboutMenuText;
			_deviceSettings.LoadInfo();
			VersionLabel = string.Format("{0}:{1}", AppResources.VersionLabel, _deviceSettings.AppVersion);
			AboutUsDetailText = AppResources.AboutUsDetailText;

		}

		public string AboutUsDetailText
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public string VersionLabel
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public ICommand LoginCommand { get { return new SimpleCommand(Login); } }

		private async void Login()
		{


			_hudProvider.DisplayProgress("Authenticating");
			_hudProvider.Dismiss();
		}
	}
}
