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
	public class SiteViewModel : BaseViewModel
	{
		private readonly IUserInfoService _userInfoService;
		private readonly IHUDProvider _hudProvider;

		public Site Site
		{
			get { return GetValue<Site>(); }
			set { SetValue(value); }
		}

		public string SiteNameLabel
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public string SiteInformationLabel
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public string CountryLabel
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public string SiteTypeLabel
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public string RegionLabel
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public string DistrictLabel
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public string SubDistrictLabel
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public string CityTownVillageLabel
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public string FundingTypeLabel
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public string PartnerLabel
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public string LongitudeLabel
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public string LatitudeLabel
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public string QIIndexScoreLabel
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public string RuralUrbanLabel
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public string OtherKeyInformationLabel
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public ICommand TestCommand { get { return new SimpleCommand(Test); } }

		private void Test()
		{



			_hudProvider.DisplayProgress("Authenticating");


			_hudProvider.Dismiss();
		}

		public SiteViewModel(IGALogger logger, IUserInfoService userInfoService,
			IHUDProvider hudProvider)
		{
			_userInfoService = userInfoService;
			_hudProvider = hudProvider;

			SiteNameLabel = AppResources.SiteNameLabel;
			CountryLabel = AppResources.CountryNameLabel;
			SiteTypeLabel = AppResources.SiteTypeLabel;
			RegionLabel = AppResources.RegionLabel;
			DistrictLabel = AppResources.DistrictLabel;
			SubDistrictLabel = AppResources.SubDistrictLabel;
			CityTownVillageLabel = AppResources.CityTownVillageLabel;
			FundingTypeLabel = AppResources.FundingTypeLabel;
			PartnerLabel = AppResources.PartnerLabel;
			LongitudeLabel = AppResources.LongitudeLabel;
			LatitudeLabel = AppResources.LattitudeLabel;
			QIIndexScoreLabel = AppResources.QIIndexScoreLabel;
			RuralUrbanLabel = AppResources.RuralUrbanLabel;
			OtherKeyInformationLabel = AppResources.OtherKeyInformationLabel;
			SiteInformationLabel = AppResources.SiteInformationLabel;


		}
	}
}
