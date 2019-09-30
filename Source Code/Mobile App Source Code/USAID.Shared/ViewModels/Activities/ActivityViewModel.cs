using USAID.Interfaces;
using USAID.Base;
using USAID.Utilities;
using System.Windows.Input;
using Xamarin.Forms;
using USAID.Resx;
using System;
using System.Collections.Generic;
using USAID.Models;
using System.Linq;

namespace USAID.ViewModels
{
	public class ActivityViewModel : BaseViewModel
	{
		private readonly IUserInfoService _userInfoService;
		private readonly IHUDProvider _hudProvider;

		public ActivityModel Activity {
			get { return GetValue<ActivityModel> (); }
			set { SetValue (value); }
		}

		public string ActivityInformationLabel {
			get { return GetValue<string> (); }
			set { SetValue (value); }
		}

		public string OrganizationLabel {
			get { return GetValue<string> (); }
			set { SetValue (value); }
		}

		public string ProjectLabel {
			get { return GetValue<string> (); }
			set { SetValue (value); }
		}

		public string ActivityNameLabel {
			get { return GetValue<string> (); }
			set { SetValue (value); }
		}

		public string CountryLabel {
			get { return GetValue<string> (); }
			set { SetValue (value); }
		}

		public string ActivityManagerLabel {
			get { return GetValue<string> (); }
			set { SetValue (value); }
		}
	

		public bool NoDataLabelVisible {
			get { return GetValue<bool> (); }
			set { SetValue (value); }
		}

		public ActivityViewModel (IGALogger logger, IUserInfoService userInfoService,
			IHUDProvider hudProvider)
		{
			_userInfoService = userInfoService;
			_hudProvider = hudProvider;

			ActivityInformationLabel = AppResources.ActivityInformationLabel;
			ActivityManagerLabel = AppResources.ActivityManagerLabel;
			CountryLabel = AppResources.CountryLabel;
			ActivityNameLabel = AppResources.ActivityNameLabel;
			ProjectLabel = AppResources.ProjectLabel;
			OrganizationLabel = AppResources.OrganizationLabel;
			PageTitle = AppResources.ActivityDetailPageTitle;

		}

		public ICommand TestCommand { get { return new SimpleCommand (Test); } }

		private void Test ()
		{
			_hudProvider.DisplayProgress ("Authenticating");

			_hudProvider.Dismiss ();
		}
	}
}
