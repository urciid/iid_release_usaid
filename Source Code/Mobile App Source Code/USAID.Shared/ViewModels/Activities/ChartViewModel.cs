using USAID.Interfaces;
using USAID.Base;
using USAID.Utilities;
using System.Windows.Input;
using Xamarin.Forms;
using USAID.Resx;
using System;
using System.Collections.Generic;
using USAID.Repositories;
using USAID.Models;
using System.Linq;
using USAID.Common;

namespace USAID.ViewModels
{
	public class ChartViewModel : BaseViewModel
	{
		private readonly IUserInfoService _userInfoService;
		private readonly IHUDProvider _hudProvider;
		private readonly IDeviceSettings _deviceSettings;
		private readonly IIndicatorRepository _indicatorRepository;


		public ChartViewModel(IGALogger logger, IUserInfoService userInfoService,
								IHUDProvider hudProvider, IDeviceSettings deviceSettings,
		                      IIndicatorRepository indicatorRepository)
		{
			_userInfoService = userInfoService;
			_hudProvider = hudProvider;
			_deviceSettings = deviceSettings;
			_indicatorRepository = indicatorRepository;


			//PageTitle = AppResources.AboutUsPageTitle;
			//_deviceSettings.LoadInfo();
			//VersionLabel = string.Format("{0}:{1}", AppResources.VersionLabel, _deviceSettings.AppVersion);
			//AboutUsDetailText = AppResources.AboutUsDetailText;

		}

		public IndicatorVM Indicator
		{
			get { return GetValue<IndicatorVM>(); }
			set { SetValue(value); }
		}

		public int SiteId
		{
			get { return GetValue<int>(); }
			set { SetValue(value); }
		}

		public int YAxisMax
		{
			get { return GetValue<int>(); }
			set { SetValue(value); }
		}

		public double IndicatorCountDisplay (string IndicatorType, double DenominatorValue, double NumeratorValue, double CountValue)
		{
			//get display value while using num and dom depending on type
			if (IndicatorType == Constants.IndicatorTypePercentage)
			{
				if (DenominatorValue > 0)
				{
					var perc = ((double)NumeratorValue / (double)DenominatorValue) * 100;
					return  Math.Round(perc, 0);
				}
			}
			if (IndicatorType == Constants.IndicatorTypeAverage)
			{
				if (DenominatorValue > 0)
				{
					double avg = (NumeratorValue / DenominatorValue);
					if (avg > YAxisMax)
						YAxisMax = (int)avg;
					return Math.Round(avg, 1);
				}
			}

			if (IndicatorType == Constants.IndicatorTypeRate)
			{
				if (DenominatorValue > 0 && NumeratorValue > 0)
				{
					var rate = ((double)NumeratorValue / (double)DenominatorValue) * 1000;
					return Math.Round(rate, 0);
				}
			}
			if (IndicatorType == Constants.IndicatorTypeCount)
			{
				return CountValue;
			}
			return 0.0;

		}

		public IEnumerable<ReportEntity> CreateEntityList()
		{
			//need to get all observationentryies
			var indicatorId = int.Parse(Indicator.IndicatorId);
			var obs = Indicator.Observations.Where(m => m.Site_id == SiteId).OrderBy(m=>m.Begin_Date).ToList();

			List<ReportEntity> entityList = new List<ReportEntity>();

			for (int i = 0; i < obs.Count(); i++)
			{
				var ci = DependencyService.Get<ILocale>().GetCurrentCultureInfo();
				var date = string.Format(ci, "{0:MMM/yy}", obs[i].Begin_Date);
				double count = IndicatorCountDisplay(Indicator.IndicatorType, (double)obs[i].ObservationEntries.Sum(m=>m.Denominator), (double)obs[i].ObservationEntries.Sum(m => m.Numerator), (double)obs[i].ObservationEntries.Sum(m => m.Count));
				if (count > 0)
					entityList.Add(new ReportEntity { BeginDateDisplay = date, CountDisplay = count });
			}

			return entityList;
		}
	}
	public class ReportEntity
	{
		public string BeginDateDisplay { get; set; }
		public double CountDisplay { get; set; }

	}
	public class SalesExpensesDownloadsEntity
	{
		public string Name { get; set; }

		public double Sales { get; set; }

		public double Expenses { get; set; }

		public double Downloads { get; set; }

		public DateTime Date { get; set; }

		public SalesExpensesDownloadsEntity()
		{
			this.Name = string.Empty;
			this.Sales = 0;
			this.Expenses = 0;
			this.Downloads = 0;
			this.Date = DateTime.Now;
		}

		public SalesExpensesDownloadsEntity(string name, double sales, double expenses, double downloads, DateTime date)
		{
			this.Name = name;
			this.Sales = sales;
			this.Expenses = expenses;
			this.Downloads = downloads;
			this.Date = date;
		}
	}

}
