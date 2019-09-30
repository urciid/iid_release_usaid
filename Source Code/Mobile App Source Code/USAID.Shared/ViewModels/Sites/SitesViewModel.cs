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
using USAID.Repositories;

namespace USAID.ViewModels
{
	public class SitesViewModel : BaseViewModel
	{
		private readonly IUserInfoService _userInfoService;
		private readonly IHUDProvider _hudProvider;
		private readonly ISiteRepository _siteRepository;

		public bool NoSitesLabelVisible
		{
			get { return GetValue<bool>(); }
			set { SetValue(value); }
		}
		public string NoSitesText
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}
		public string DetailsText
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}
		public List<Site> Sites
		{
			get { return GetValue<List<Site>>(); }
			set { SetValue(value); }
		}

		public SitesViewModel(IGALogger logger, IUserInfoService userInfoService,
		                      IHUDProvider hudProvider, ISiteRepository siteRepository)
		{
			_userInfoService = userInfoService;
			_hudProvider = hudProvider;
			_siteRepository = siteRepository;

			PageTitle = AppResources.SitesMenuText;
			NoSitesText = AppResources.NoSitesText;

			//need to get sites

		}

		public void LoadSites()
		{
			IsBusy = true;
			Sites = _siteRepository.All().ToList().OrderBy(m=>m.SiteName).ToList();
			//Sites = GetMockedSites();
			NoSitesLabelVisible = !Sites.Any();
			IsBusy = false;
		}
	}
}
