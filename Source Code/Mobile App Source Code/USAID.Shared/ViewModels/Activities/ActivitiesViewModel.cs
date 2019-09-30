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
using USAID.Enumerations;
using USAID.Common;
using System.Collections.ObjectModel;
using USAID.Custom;
using USAID.Repositories;

namespace USAID.ViewModels
{
	public class ActivitiesViewModel : BaseViewModel
	{
		private readonly IUserInfoService _userInfoService;
		private readonly IHUDProvider _hudProvider;
		public ObservableCollection<IndicatorVM> IndicatorsG { get; set; }
		private readonly IIndicatorRepository _indicatorRepository;
		private readonly ISiteIndicatorRepository _siteIndicatoryRepository;
		private readonly IActivityRepository _activitiesRepository;
		private readonly IObservationRepository _observationRepository;
		private readonly IObservationEntryRepository _observationEntryRepository;
		private readonly IIndicatorAgeRepository _indicatorAgeRespository;




		public Site Site
		{
			get { return GetValue<Site>(); }
			set { SetValue(value); }
		}

		public ObservableCollection<Grouping<string, IndicatorVM>> IndicatorsGrouped
		{
			get { return GetValue<ObservableCollection<Grouping<string, IndicatorVM>>>(); }
			set { SetValue(value); }
		}

		public List<IndicatorVM> Indicators
		{
			get { return GetValue<List<IndicatorVM>>(); }
			set 
			{
				SetValue(value);
				IndicatorsG = new ObservableCollection<IndicatorVM>();
				foreach (var item in value)
				{
					IndicatorsG.Add(item);
				}
				var sorted = from ind in IndicatorsG
					orderby ind.Aim
				               group ind by ind.Aim into indGroup
				               select new Grouping<string, IndicatorVM>(indGroup.Key, indGroup);

				IndicatorsGrouped = new ObservableCollection<Grouping<string, IndicatorVM>>(sorted);
			}
		}

		public string SiteNameLabel
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public bool NoDataLabelVisible
		{
			get { return GetValue<bool>(); }
			set { SetValue(value); }
		}



		public ICommand TestCommand { get { return new SimpleCommand(Test); } }

		private void Test()
		{
			_hudProvider.DisplayProgress("Authenticating");

			_hudProvider.Dismiss();
		}

		public ActivitiesViewModel(IGALogger logger, IUserInfoService userInfoService,
		                           IHUDProvider hudProvider, IIndicatorRepository indicatoryRepository, ISiteIndicatorRepository siteIndicatoryRepository,
		                           IActivityRepository activityRepository, IObservationRepository observationRepository, IObservationEntryRepository observationEntryRepository,
		                           IIndicatorAgeRepository indicatorAgeRepository)
		{
			_userInfoService = userInfoService;
			_hudProvider = hudProvider;
			SiteNameLabel = AppResources.SiteNameLabel;
			PageTitle = AppResources.IndicatorsPageTitle;
			_indicatorRepository = indicatoryRepository;
			_siteIndicatoryRepository = siteIndicatoryRepository;
			_activitiesRepository = activityRepository;
			_observationRepository = observationRepository;
			_observationEntryRepository = observationEntryRepository;
			_indicatorAgeRespository = indicatorAgeRepository;

		}
		public void LoadIndicators()
		{
			IsBusy = true;
			//Indicators = GetMockedIndicators(); //GetIndicators();
			Indicators = GetIndicators();
			NoDataLabelVisible = !Indicators.Any();
			IsBusy = false;
		}


		private List<IndicatorVM> GetIndicators()
		{
			List<IndicatorVM> list = new List<IndicatorVM>();
			//site indicators
			var siteIndicators = _siteIndicatoryRepository.All(m => m.SiteId == Site.SiteId).ToList();
			//Loop through siteIndicators and load up IndicatorVM

			if (siteIndicators != null)
			{
				foreach (var siteIndicator in siteIndicators)
				{
					//Need to get indicator information and observations and observationsEntry
					//Indicator
					var indicator = _indicatorRepository.GetIndicatorModel(siteIndicator.IndicatorId);

					var activity = _activitiesRepository.GetActivityModel(siteIndicator.ActivityId);

					//Getting observations

					//Need to loop through periods and add ones that don't belong
					//if exists than create IndicatorVM
					IndicatorVM vm = new IndicatorVM();
					vm.Activity = activity == null ? new ActivityModel() : activity;
					vm.AgePeriods = _indicatorAgeRespository.GetAgePeriods(siteIndicator.IndicatorId);
					vm.IndicatorId = indicator.IndicatorId;
					vm.Aim = indicator.Aim;
					vm.Created = indicator.Created;
					vm.Definition = indicator.Definition;
					vm.DenominatorName = indicator.DenominatorName;
					vm.DenominatorDefinition = indicator.DenominatorDefinition;
					vm.DisaggregateByAge = indicator.DisaggregateByAge;
					vm.DisaggregateBySex = indicator.DisaggregateBySex;
					vm.Frequency = indicator.Frequency;
					vm.IndicatorType = indicator.IndicatorType;
					vm.Name = indicator.Name;
					vm.NumeratorName = indicator.NumeratorName;
					vm.NumeratorDefinition = indicator.NumeratorDefinition;
					vm.StartDate = siteIndicator.StartDate;


					//var ob3s = _observationRepository.GetObservations(Site.SiteId, siteIndicator.IndicatorId);
					var obs = _observationRepository.All(m => m.Indicator_id == siteIndicator.IndicatorId).ToList();
					vm.Observations = obs;
					list.Add(vm);

				}
			}


			//var activities = _activitiesRepository.All().ToList();

			//var allIndicators = _indicatorRepository.All().ToList();

			//if (allIndicators != null)
			//{
			//	foreach (var item in allIndicators)
			//	{
			//		//need to loop through all indicators and only take the ones that are in siteIndicators
			//		var siteI = siteIndicators.Find(m => (m.IndicatorId.ToString() == item.IndicatorId) && (m.SiteId == Site.SiteId));
			//		//(c => (c.ProductID == ProductID) && (c.ProductName == "ABS001"));

			//		var activity = activities.Find(m => m.ActivityId == siteI.ActivityId);
			//		if (siteI != null)
			//		{
			//			var obs = _observationRepository.GetObservations(Site.SiteId, Int32.Parse(item.IndicatorId));

			//			//create periods
			//			List<Period> periods = new List<Period>();
			//			foreach (var ob in obs)
			//			{
			//				periods.Add(new Period { BeginDate = ob.Begin_Date, EndDate = ob.End_Date });
			//			}

			//		}

			//	}
			//}


			return list;

		}
		private List<Period> GetPeriods(string frequency)
		{
			List<Period> periods = new List<Period>();


			//	for (int i = 1; i < 5; i++)
			//	{
			//		var bDate = DateTime.Now.AddDays(i);
			//		var eDate = DateTime.Now.AddDays(i + 1);
			//		periods.Add(new Period { BeginDate = bDate, EndDate = eDate, PeriodId = i });
			//	}


			return periods;
		}
		private Observation GetObservation(Int32 indicatorId)
		{
			var allObs = _observationRepository.All(mx => mx.Indicator_id == indicatorId && mx.Site_id == Site.SiteId);
				
			var obs = _observationRepository.All(mx => mx.Indicator_id == indicatorId && mx.Site_id == Site.SiteId).FirstOrDefault();

			return obs;
		}

	}
}
