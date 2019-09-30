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
using USAID.Common;
using System.Threading.Tasks;
using USAID.Pages;
using System.Collections.ObjectModel;
using USAID.Repositories;
using SQLite.Net;
using System.IO;
using System.Text;

namespace USAID.ViewModels
{
	public class ObservationViewModel : BaseViewModel
	{
		private readonly IUserInfoService _userInfoService;
		private readonly IHUDProvider _hudProvider;
		private readonly IObservationRepository _observationRep;
		private readonly IObservationEntryRepository _observationEntryRepository;
		private readonly IObservationChangeRepository _observationChangeRepository;
		private readonly IObservationCommentRepository _observationCommentRepository;
		private readonly IObservationAttachmentRepository _observationAttachmentRepository;
		private readonly IObservationService _observationService;
		private readonly IDeviceSettings _deviceSettings;

		public ObservableCollection<ObsViewModel> _originalObs { get; set; }
		public string _firstTimeChange { get; set; }
		public INavigation _Navigation { get; set; }
		private SQLiteConnection _connection;
		public bool inLoad = false;
		public double GetObservationRowHeight()
		{
			//need to figure out height depending on how many records into Obs
			if (Obs == null)
			{
				return 100;
			}
			var h = Obs.Count();
			if (h > 0 && DisaggregatedSelected)
			{
				h = h + 1;
			}
			if (IndicatorTypeIsYesNo)
			{
				if (h == 1)
				{
					return 150.00;
				}
				else
				{
					return h * 100.00;
				}
			}
			else if (IndicatorTypeIsCount)
			{
				if (h == 1)
				{
					return 150.00;
				}
				else
				{
					return h * 100.00;
				}
			}
			else
			{
				if (h == 1)
				{
					return 250.00;
				}
				else
				{
					return h * 250.00;
				}
			}

		}

		public double IndicatorNumCount
		{
			get { if (Obs != null) { return Obs.Sum(x => x.NumeratorValue); } else { return 0; } }
		}

		public double IndicatorDenCount
		{
			get { if (Obs != null) { return Obs.Sum(x => x.DenominatorValue); } else { return 0; } }
		}

		public double IndicatorCount
		{
			get { if (Obs != null) { return Obs.Sum(x => x.IndicatorCount); } else { return 0; } }
		}

		public string IndicatorTitle
		{
			get { return "T";  }
		}

		public string IndicatorNumDefinition
		{
			get { return Indicator.NumeratorDefinition; }
		}

		public string IndicatorDenDefinition
		{
			get { return Indicator.DenominatorDefinition; }
		}

		public string IndicatorTotalDefinition
		{
			get { return Indicator.Name; }
		}



	

		public string IndicatorCountDisplay
		{

			get
			{
				var ret = "";
				var IndicatorType = Indicator.IndicatorType;
				var DenominatorValue = IndicatorDenCount;
				var NumeratorValue = IndicatorNumCount;
				if (IndicatorType == Constants.IndicatorTypePercentage)
				{
					if (DenominatorValue > 0)
					{
						double perc = ((double)NumeratorValue / (double)DenominatorValue) * 100;
						return string.Format("{0}%", Math.Round(perc, 0).ToString());
					}
				}
				if (IndicatorType == Constants.IndicatorTypeAverage)
				{
					if (DenominatorValue > 0)
					{
						double perc = ((double)NumeratorValue / (double)DenominatorValue);
						return string.Format("{0}", Math.Round(perc, 1).ToString());
					}
				}

				if (IndicatorType == Constants.IndicatorTypeRate)
				{
					if (DenominatorValue > 0 && NumeratorValue > 0)
					{

						double perc = ((double)NumeratorValue / (double)DenominatorValue) * 1000;
						return string.Format("{0} per {1}", Math.Round(perc, 0).ToString(), 1000);
					}
				}
				if (IndicatorType == Constants.IndicatorTypeCount)
				{
					return string.Format("{0}", Math.Round(IndicatorCount, 1).ToString());
				}

				return ret;

			}
		}


		public double GetChangesRowHeight()
		{
			//need to figure out height depending on how many records into Obs
			var h = Changes == null ? 1 : Changes.Count;
			return h * 80;
		}
		public double GetCommentsRowHeight()
		{
			//need to figure out height depending on how many records into Obs
			var h = Comments == null ? 1 : Comments.Count;
			return h * 80;
		}
		public double GetAttachmentsRowHeight()
		{
			//need to figure out height depending on how many records into Obs
			var h = Attachments == null ? 1 : Attachments.Count;
			return h * 80;
		}
		public bool DisaggregatedSelected
		{
			get { return GetValue<bool>(); }
			set { SetValue(value); }
		}

		public bool DisaggregatedEnabled
		{
			get { return GetValue<bool>(); }
			set { SetValue(value); }
		}



		public bool IndicatorTypeIsYesNo
		{
			get { return Indicator.IndicatorType == Constants.IndicatorTypeYesNo; }
		}

		public bool IndicatorTypeIsCount
		{
			get { return Indicator.IndicatorType == Constants.IndicatorTypeCount; }
		}


		public bool IndicatorTypeIsNormal
		{
			get
			{
				bool ret = false;
				if (Indicator.IndicatorType == Constants.IndicatorTypeAverage || Indicator.IndicatorType == Constants.IndicatorTypeRate || Indicator.IndicatorType == Constants.IndicatorTypePercentage)
				{
					ret = true;
				};
				return ret;
			}
		}

		public bool DisaggregatedAvailable
		{
			//get { return (string.IsNullOrEmpty(Indicator.Sex) && Indicator.Age != null && !Indicator.Age.Any()); }
			get { return (Indicator.DisaggregateBySex) || (Indicator.DisaggregateByAge); }
		}

		public IndicatorVM Indicator
		{
			get { return GetValue<IndicatorVM>(); }
			set { SetValue(value); }
		}

		public string AimTitle
		{
			get { return Indicator.Aim; }
		}

		public Site Site
		{
			get { return GetValue<Site>(); }
			set { SetValue(value); }
		}
		public Observation Observation
		{
			get { return GetValue<Observation>(); }
			set { SetValue(value); }
		}

		public Period SelectedPeriod
		{
			get { return GetValue<Period>(); }
			set
			{
				SetValue(value);
				LoadObservationDataForPeriod(value.LocalObservationID);
			}
		}

		public List<Period> Periods
		{
			get { return Indicator.Periods; }
			set { SetValue(value); }
		}

		public ObservableCollection<ObsViewModel> Obs
		{
			get { return GetValue<ObservableCollection<ObsViewModel>>(); }
			set { SetValue(value); }
		}

		public DateTime TestDate
		{
			get { return GetValue<DateTime>(); }
			set { SetValue(value); }
		}

		public ObservableCollection<ObservationChange> Changes
		{
			get { return GetValue<ObservableCollection<ObservationChange>>(); }
			set { SetValue(value); }
		}

		public ObservableCollection<ObservationAttachment> Attachments
		{
			get { return GetValue<ObservableCollection<ObservationAttachment>>(); }
			set { SetValue(value); }
		}

		public ObservableCollection<ObservationComment> Comments
		{
			get { return GetValue<ObservableCollection<ObservationComment>>(); }
			set { SetValue(value); }
		}

		public bool ImageVisible
		{
			get { return GetValue<bool>(); }
			set { SetValue(value); }
		}

		Command continueImageCommand;
		/// <summary>
		/// Gets the open power ops command.
		/// </summary>
		/// <value>The open power ops command.</value>
		public Command ContinueImageCommand
		{
			get { return continueImageCommand ?? (continueImageCommand = new Command(ExecuteContinueImageCommand)); }
		}

		private void ExecuteContinueImageCommand(object option)
		{
			//close image overlay
			ImageVisible = false;

		}
		public ObservationAttachment SelectedAttachment { get; set; }

		public ImageSource ImageSelected
		{
			get
			{
				if (SelectedAttachment != null)
				{
					var stream = new MemoryStream(SelectedAttachment.Bytes);
					return ImageSource.FromStream(() => stream);
				}
				return null;

			}
		}



		public void InitialSetup()
		{
			if (SelectedPeriod == null)
			{
				SelectedPeriod = Periods.First();
				//LoadObservationDataForPeriod();
			}
		}
		public void LoadObservationDataForPeriod(int Id)
		{
			inLoad = true;
			//Need to get Obs, Changes, attachments, and comments for obsId
			var ind = Indicator.Observations.Where(item => item.Id == Id && item.Site_id == Site.SiteId).FirstOrDefault();
			DisaggregatedEnabled = true;
			if (Id > 0 && ind != null && ind.ObservationEntries.Count > 0)
			{
				Observation = Indicator.Observations.Where(item => item.Id == Id).FirstOrDefault();
				Changes = Observation.Changes;
				Comments = Observation.Comments;
				Attachments = Observation.Attachments;

				var currentObs = Observation.ObservationEntries;

				if (currentObs.Count > 0)
				{
					DisaggregatedEnabled = false;
				}
				Obs = new ObservableCollection<ObsViewModel>();
				if (currentObs.Count > 0)
				{
					DisaggregatedSelected = currentObs.Count > 1;
					//if (Obs == null)
					//	Obs = new ObservableCollection<ObsViewModel>();
					foreach (var o in currentObs)
					{
						var indicatorAgeRangeId = o.Indicator_Age_Range_Id == null ? 0 : (int)o.Indicator_Age_Range_Id;
						var ageRangeTitle = indicatorAgeRangeId > 0 ? Indicator.AgePeriods.Where(m => m.Indicator_Age_Range_ID == indicatorAgeRangeId).FirstOrDefault().Age_Range : "";

						Obs.Add(new ObsViewModel
						{
							ObservationEntryId = o.ObservationEntryId == null ? 0 : (int)o.ObservationEntryId,
							IndicatorAgeRangeId = indicatorAgeRangeId,
							ObservationId = o.Observation_id == null ? 0 : (int)o.Observation_id,
							Rate = o.Rate == null ? 0 : (double)o.Rate,
							IndicatorGender = o.Indicator_Gender,
							Title = String.Format("{0} {1}", o.Title, ageRangeTitle),
							DenominatorDefinition = Indicator.DenominatorDefinition,
							DenominatorName = Indicator.DenominatorName,
							DenominatorValue = o.Denominator == null ? 0 : (int)o.Denominator,
							IndicatorName = Indicator.Name,
							IndicatorDefinition = Indicator.Name,
							IndicatorType = Indicator.IndicatorType,
							NumeratorDefinition = Indicator.NumeratorDefinition,
							NumeratorName = Indicator.NumeratorName,
							NumeratorValue = o.Numerator == null ? 0 : (int)o.Numerator,
							IndicatorYesNo = o.Yes_No == null ? false : (bool)o.Yes_No,
							IndicatorCount = o.Count == null ? 0 : (int)o.Count
						});


					}

					//if Indicator.DisaggregateBySex) || (Indicator.DisaggregateByAge) then remove obs that dont' have gender or title
					if (Indicator.DisaggregateBySex)
					{
						if (Obs.Count > 1)
						{
							var list = Obs.Where(m => !string.IsNullOrEmpty(m.IndicatorGender)).OrderByDescending(m => m.IndicatorGender);
							Obs = new ObservableCollection<ObsViewModel>(list);
						}
						if (Obs.Count == 1)
						{
							var list = Obs;
							Obs = new ObservableCollection<ObsViewModel>(list);
						}

					}
					_originalObs = Obs;
				}
                //RaisePropertyChanged("Obs");

			}
			else
			{
				//need observation because it is new
				Observation = new Observation
				{
					Begin_Date = SelectedPeriod.BeginDate,
					End_Date = SelectedPeriod.EndDate,
					Indicator_id = Int32.Parse(Indicator.IndicatorId),
					ObservationEntries = new ObservableCollection<ObservationEntry>(),
					Site_id = Site.SiteId
				};

				//Do not have data so get data from template observations
				//var obsEntry = _observationEntryRepository.GetObservationEntries(Id);
				if (Indicator.ObservationsTemplate.Count > 0)
				{
					//TODO Disaggregate by flags on Indicator
					DisaggregatedSelected = Indicator.ObservationsTemplate.Count > 1;
					var templateObs = Indicator.ObservationsTemplate.ToList();
					Obs = new ObservableCollection<ObsViewModel>();
					foreach (var o in templateObs)
					{

						var indicatorAgeRangeId = o.Indicator_Age_Range_Id == null ? 0 : (int)o.Indicator_Age_Range_Id;
						var ageRangeTitle = indicatorAgeRangeId > 0 ? Indicator.AgePeriods.Where(m => m.Indicator_Age_Range_ID == indicatorAgeRangeId).FirstOrDefault().Age_Range : "";

						Obs.Add(new ObsViewModel
						{
							ObservationEntryId = o.ObservationEntryId == null ? 0 : (int)o.ObservationEntryId,
							IndicatorAgeRangeId = o.Indicator_Age_Range_Id == null ? 0 : (int)o.Indicator_Age_Range_Id,
							ObservationId = o.Observation_id == null ? 0 : (int)o.Observation_id,
							Rate = o.Rate == null ? 0 : (double)o.Rate,
							IndicatorGender = o.Indicator_Gender,
							Title = String.Format("{0} {1}", o.Title, ageRangeTitle),
							DenominatorDefinition = Indicator.DenominatorDefinition,
							DenominatorName = Indicator.DenominatorName,
							DenominatorValue = o.Denominator == null ? 0 : (int)o.Denominator,
							IndicatorName = Indicator.Name,
							IndicatorDefinition = Indicator.Name,
							IndicatorType = Indicator.IndicatorType,
							NumeratorDefinition = Indicator.NumeratorDefinition,
							NumeratorName = Indicator.NumeratorName,
							NumeratorValue = o.Numerator == null ? 0 : (int)o.Numerator,
							IndicatorYesNo = o.Yes_No == null ? false : (bool)o.Yes_No,
							IndicatorCount = o.Count == null ? 0 : (int)o.Count
						});
					}
					if (Indicator.DisaggregateBySex)
					{
						var list = Obs.Where(m => !string.IsNullOrEmpty(m.IndicatorGender)).OrderByDescending(m => m.IndicatorGender);
						Obs = new ObservableCollection<ObsViewModel>(list);
					}
					_originalObs = Obs;
					Changes = new ObservableCollection<ObservationChange>();
					Attachments = new ObservableCollection<ObservationAttachment>();
					Comments = new ObservableCollection<ObservationComment>();
				}
			}
			inLoad = false;
		}


		public ICommand SaveCommand { get { return new SimpleCommand(Save); } }

		private async void Save()
		{
			_hudProvider.DisplayProgress("Saving");
			Observation.ModifiedLocally = true;
			Observation.Changes = Changes == null ? new ObservableCollection<ObservationChange>() : Changes;
			Observation.Comments = Comments == null ? new ObservableCollection<ObservationComment>() : Comments;
			Observation.Attachments = Attachments == null ? new ObservableCollection<ObservationAttachment>() : Attachments;

			//saving observation data
			var obsId = SelectedPeriod.ObservationID;
			Observation.Observation_id = SelectedPeriod.ObservationID;

			if (Observation.ObservationEntries == null)
				Observation.ObservationEntries = new ObservableCollection<ObservationEntry>();
			//checking to see if they toggled 
			//bool sameCount = Observation.ObservationEntries.Count == Obs.Count;

			if (obsId > 0)
			{
				//need to load up Observation with entries
				foreach (ObsViewModel ob in Obs)
				{
					//find entry for this vm
					ObservationEntry entry = Observation.ObservationEntries.Where(m => m.ObservationEntryId == ob.ObservationEntryId).First();
					if (entry != null)
					{
						entry.LocalObservationID = SelectedPeriod.LocalObservationID;
						entry.Observation_id = obsId;
						entry.ModifiedLocally = true;
						entry.Count = (int)ob.IndicatorCount;
						entry.Numerator = ob.NumeratorValue;
						entry.Denominator = ob.DenominatorValue;
						entry.Yes_No = ob.IndicatorYesNo;
						entry.Indicator_Gender = ob.IndicatorGender;
						entry.Indicator_Age_Range_Id = ob.IndicatorAgeRangeId;
					}
					else
					{
						//new entry
						ObservationEntry newEntry = new ObservationEntry();
						newEntry.Observation_id = obsId;
						newEntry.LocalObservationID = SelectedPeriod.LocalObservationID;
						newEntry.ModifiedLocally = true;
						newEntry.Count = (int)ob.IndicatorCount;
						newEntry.Numerator = ob.NumeratorValue;
						newEntry.Denominator = ob.DenominatorValue;
						newEntry.Yes_No = ob.IndicatorYesNo;
						entry.Indicator_Gender = ob.IndicatorGender;
						entry.Indicator_Age_Range_Id = ob.IndicatorAgeRangeId;
						Observation.ObservationEntries.Add(newEntry);

					}
				}
			}
			else
			{
				foreach (ObsViewModel ob in Obs)
				{
					//new entry
					ObservationEntry newEntry = new ObservationEntry();
					newEntry.Observation_id = obsId;
					newEntry.LocalObservationID = SelectedPeriod.LocalObservationID;
					newEntry.ModifiedLocally = true;
					newEntry.Count = (int)ob.IndicatorCount;
					newEntry.Numerator = ob.NumeratorValue;
					newEntry.Denominator = ob.DenominatorValue;
					newEntry.Yes_No = ob.IndicatorYesNo;
					newEntry.Indicator_Age_Range_Id = ob.IndicatorAgeRangeId;
					newEntry.Indicator_Gender = ob.IndicatorGender;

					Observation.ObservationEntries.Add(newEntry);
				}
			}
			//need to check if dissagregated then add total row
			if (DisaggregatedSelected)
			{
					ObservationEntry newEntry = new ObservationEntry();
					newEntry.Observation_id = obsId;
					newEntry.LocalObservationID = SelectedPeriod.LocalObservationID;
					newEntry.ModifiedLocally = true;
					newEntry.Count = (int)IndicatorCount;
					newEntry.Numerator = (int)IndicatorNumCount;
					newEntry.Denominator = (int)IndicatorDenCount;
					//newEntry.Yes_No = ob.IndicatorYesNo;
					//newEntry.Indicator_Age_Range_Id = ob.IndicatorAgeRangeId;
					//newEntry.Indicator_Gender = ob.IndicatorGender;

					Observation.ObservationEntries.Add(newEntry);
			}
			var userinfo = _userInfoService.GetSavedInfo();
			//saving locally
			_observationRep.Upsert(Observation);


			//if connected try to save up to server
			if (App.IsConnected)
			{
				Observation.Begin_Date = OffSetDateTimeZone((DateTime)Observation.Begin_Date);
				Observation.End_Date = OffSetDateTimeZone((DateTime)Observation.End_Date);
				var response = await _observationService.ObservationSave(userinfo.Email, Observation);
				_observationRep.Upsert(response.observation);

			}

			_hudProvider.Dismiss();
			await _Navigation.PopAsync();

		}

		public ICommand AddCommentCommand { get { return new SimpleCommand(AddComment); } }

		private void AddComment()
		{
			//await Navigation.PushModalAsync(new ChangePage(change));
			_Navigation.PushModalAsync(new CommentPage(null));
		}

		public ICommand AddChangeCommand { get { return new SimpleCommand(AddChange); } }

		private void AddChange()
		{
			//await Navigation.PushModalAsync(new ChangePage(change));
			_Navigation.PushModalAsync(new ChangePage(null));
		}

		public ICommand DisaggregateCommand { get { return new SimpleCommand(DisaggregateChange); } }

		private void DisaggregateChange()
		{
			_firstTimeChange = _firstTimeChange == null ? "firstTime" : "notfirsttime";
			_hudProvider.DisplayProgress("Loading");
			if (DisaggregatedSelected)
			{
				if (_firstTimeChange != "firstTime")
				{
					Obs = _originalObs;
				}
			}
			else
			{
				if (_firstTimeChange != "firstTime")
				{
					Obs = null;
					Obs = new ObservableCollection<ObsViewModel>();
					Obs.Add(GetDefaultObsViewModel());
				}


			}
			//DisaggregatedSelected = !DisaggregatedSelected;
			//Need to change obs to show all obs or just the one obs

			_hudProvider.Dismiss();



		}
		public ObsViewModel GetDefaultObsViewModel()
		{
			ObsViewModel obs = new ObsViewModel
			{
				DenominatorDefinition = Indicator.DenominatorDefinition,
				DenominatorName = Indicator.DenominatorName,
				DenominatorValue = 0,
				IndicatorName = Indicator.Name,
				IndicatorDefinition = Indicator.Name,
				IndicatorType = Indicator.IndicatorType,
				NumeratorDefinition = Indicator.NumeratorDefinition,
				NumeratorName = Indicator.NumeratorName,
				NumeratorValue = 0,
				Title = "",
				IndicatorCount = 0,
				IndicatorYesNo = false
			};
			return obs;

		}
		DateTime OffSetDateTimeZone(DateTime date)
		{
			var offset = _deviceSettings.GetCurrentUTCOffset();
			var newDate = date;
			if (offset > 0)
			{
				newDate = date.AddHours(offset);
			}

			//var newDate = TimeZoneInfo.ConvertTimeToUTC(date, TimeZoneInfo.Local);

			return newDate;
		}

		public void LoadPeriodsWithObservations()
		{

			var obs = Indicator.Observations.Where(m => m.Site_id == Site.SiteId).ToList();
			//Indicator.Periods = LoadPeriodsForNoObservations();


			foreach (var ob in obs)
			{
				var date = (DateTime)ob.Begin_Date;
				date = OffSetDateTimeZone(date);
				//Need to add datediff with UTC

				//loop through observations and look for begin date
				//var item = Indicator.Periods.Find(m => m.BeginDateDisplay == ob.Begin_Date.ToString());

				//TODO: need to figure out why observationid is not populating correctly
				foreach (var period in Indicator.Periods)
				{
					var begin = OffSetDateTimeZone((DateTime)period.BeginDate);
					if (begin.Year == date.Year && begin.Month == date.Month && begin.Day == date.Day)
					{
						//same date so update ob
						period.ObservationID = ob.Observation_id;
						period.LocalObservationID = ob.Id;

					}
				}

			}
		}

		public List<Period> LoadPeriodsForNoObservations()
		{
			//var currentBegin = DateTime.Today;
			var currentBegin = DateTime.Today;
			//currentBegin = OffSetDateTimeZone(currentBegin);
			var oldestDate = currentBegin.AddMonths(-Constants.PeriodBackAmount).Date;

			List<Period> list = new List<Period>();
			if (Indicator.Frequency.Trim() == Constants.IndicatorFrequencyDaily)
			{
				for (DateTime date = currentBegin; date.Date >= oldestDate.Date; date = date.AddDays(-1))
				{
					var newBegin = date;
					var newEnd = date;
					if (newEnd.Date <= DateTime.Now.Date)
					{
						list.Add(new Period { BeginDate = newBegin.Date, EndDate = newEnd.Date, LocalObservationID = 0 });
					}

				}
			}
			if (Indicator.Frequency.Trim() == Constants.IndicatorFrequencyWeekly)
			{
				double days = -(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday;
				currentBegin = currentBegin.AddDays(days).Date;

				for (DateTime date = currentBegin; date.Date >= oldestDate.Date; date = date.AddDays(-7).Date)
				{
					var newBegin = date;
					var newEnd = date.AddDays(6);
					if (newEnd.Date <= DateTime.Now.Date)
						list.Add(new Period { BeginDate = newBegin, EndDate = newEnd, ObservationID = 0 });
				}
			}
			if (Indicator.Frequency.Trim() == Constants.IndicatorFrequencyBiWeekly)
			{
				//var startDate = Indicator.StartDate.Date;

				//double days = -(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday;
				//currentBegin = currentBegin.AddDays(days).Date;
				currentBegin = Indicator.StartDate.Date;


				for (DateTime date = currentBegin; date.Date < DateTime.Now.Date; date = date.AddDays(14).Date)
				{
					var newBegin = date;
					var newEnd = date.AddDays(13);
					if (newEnd.Date <= DateTime.Now.Date)
						list.Add(new Period { BeginDate = newBegin, EndDate = newEnd, ObservationID = 0 });
				}
				//need to reverse list
				list.Reverse();
			}
			if (Indicator.Frequency.Trim() == Constants.IndicatorFrequencyMonthly)
			{

				for (DateTime date = currentBegin; date.Date >= oldestDate.Date; date = date.AddMonths(-1))
				{
					var newBegin = new DateTime(date.Year, date.Month, 1); ;
					var newEnd = newBegin.AddMonths(1).AddDays(-1);
					if (newEnd.Date <= DateTime.Now.Date)
						list.Add(new Period { BeginDate = newBegin, EndDate = newEnd, ObservationID = 0 });
				}
			}

			if (Indicator.Frequency.Trim() == Constants.IndicatorFrequencyQuarterly)
			{
				var oldest = DateTime.Now.AddMonths(-12);
				DateTime endOldestQuarter = GetQuarterEnd(oldest);
				DateTime beginOldestQuarter = endOldestQuarter.AddMonths(-3).AddDays(1);
				if (endOldestQuarter.Date <= DateTime.Now.Date)
					list.Add(new Period { BeginDate = beginOldestQuarter, EndDate = endOldestQuarter });


				DateTime nextEndQuarter = endOldestQuarter.AddMonths(3);
				DateTime nextBeginQuarter = nextEndQuarter.AddMonths(-3).AddDays(1);
				if (nextEndQuarter.Date <= DateTime.Now.Date)
					list.Add(new Period { BeginDate = nextBeginQuarter, EndDate = nextEndQuarter, ObservationID = 0 });

				nextEndQuarter = nextEndQuarter.AddMonths(3);
				nextBeginQuarter = nextEndQuarter.AddMonths(-3).AddDays(1);
				if (nextEndQuarter.Date <= DateTime.Now.Date)
					list.Add(new Period { BeginDate = nextBeginQuarter, EndDate = nextEndQuarter, ObservationID = 0 });

				nextEndQuarter = nextEndQuarter.AddMonths(3);
				nextBeginQuarter = nextEndQuarter.AddMonths(-3).AddDays(1);
				if (nextEndQuarter.Date <= DateTime.Now.Date)
					list.Add(new Period { BeginDate = nextBeginQuarter, EndDate = nextEndQuarter, ObservationID = 0 });


			}



			return list;

		}

		public DateTime GetQuarterEnd(DateTime dt)
		{
			int month = dt.Month;
			month = ((month + 2) / 3) * 3;
			var x = new DateTime(dt.Year, month, 1).AddMonths(1).AddDays(-1);
			return x;
		}

		public ObservableCollection<ObservationEntry> GetTemplates()
		{
			ObservableCollection<ObservationEntry> templates = new ObservableCollection<ObservationEntry>();

			//if age range and sex
			//need to have entry for each m/f for each age range
			if (Indicator.DisaggregateByAge && Indicator.DisaggregateBySex)
			{
				foreach (var age in Indicator.AgePeriods)
				{
					templates.Add(new ObservationEntry { Denominator = 0, Numerator = 0, Yes_No = false, Indicator_Age_Range_Id = age.Indicator_Age_Range_ID, Indicator_Gender = Constants.ObservationMale });
					templates.Add(new ObservationEntry { Denominator = 0, Numerator = 0, Yes_No = false, Indicator_Age_Range_Id = age.Indicator_Age_Range_ID, Indicator_Gender = Constants.ObservationFeMale });
				}
			}
			//if range
			//need to have entry for each age range
			else if (Indicator.DisaggregateByAge && !Indicator.DisaggregateBySex)
			{
				foreach (var age in Indicator.AgePeriods)
				{
					templates.Add(new ObservationEntry { Denominator = 0, Numerator = 0, Yes_No = false, Indicator_Age_Range_Id = age.Indicator_Age_Range_ID });
				}
			}
			//if sex
			//need to have entry for each sex
			else if (Indicator.DisaggregateBySex && !Indicator.DisaggregateByAge)
			{
				templates.Add(new ObservationEntry { Denominator = 0, Numerator = 0, Yes_No = false, Indicator_Gender = Constants.ObservationMale });
				templates.Add(new ObservationEntry { Denominator = 0, Numerator = 0, Yes_No = false, Indicator_Gender = Constants.ObservationFeMale });
			}
			//if not sex and not range the basic entry
			else
			{
				templates.Add(new ObservationEntry { Denominator = 0, Numerator = 0, Yes_No = false });
			}










			//if (IndicatorTypeIsCount)
			//{
			//	templates.Add(new ObservationEntry { Denominator = 0, Numerator = 0 });
			//}
			//else if (IndicatorTypeIsYesNo)
			//{
			//	templates.Add(new ObservationEntry {  });
			//}
			//else {
			//	//normal
			//	templates.Add(new ObservationEntry { Denominator = 0, Numerator = 0 });
			//}


			return templates;
		}
		public string TotalCountDisplay
		{
			//get display value while using num and dom depending on type
			get
			{
				var IndicatorType = Indicator.IndicatorType;
				var DenominatorValue = IndicatorDenCount;
				var NumeratorValue = IndicatorNumCount;

				var ret = "";
				if (IndicatorType == Constants.IndicatorTypePercentage)
				{
					if (DenominatorValue > 0)
					{
						double perc = ((double)NumeratorValue / (double)DenominatorValue) * 100;
						return string.Format("{0}%", Math.Round(perc, 0).ToString());
					}
				}
				if (IndicatorType == Constants.IndicatorTypeAverage)
				{
					if (DenominatorValue > 0)
					{
						double perc = ((double)NumeratorValue / (double)DenominatorValue);
						return string.Format("{0}", Math.Round(perc, 1).ToString());
					}
				}

				if (IndicatorType == Constants.IndicatorTypeRate)
				{
					if (DenominatorValue > 0 && NumeratorValue > 0)
					{

						double perc = ((double)NumeratorValue / (double)DenominatorValue) * 1000;
						return string.Format("{0} per {1}", Math.Round(perc, 0).ToString(), 1000);
					}
				}
				if (IndicatorType == Constants.IndicatorTypeCount)
				{
					return string.Format("{0}", Math.Round(IndicatorCount, 1).ToString());
				}

				return ret;
			}

		}

		public ObservationViewModel(IGALogger logger, IUserInfoService userInfoService, IObservationRepository observationRep,
									IHUDProvider hudProvider, IObservationEntryRepository observationEntryRepository,
									IObservationChangeRepository observationChangeRepository,
									IObservationCommentRepository observationCommentRepository,
									IObservationAttachmentRepository observationAttachmentRepository,
									IObservationService ObservationService, IDeviceSettings deviceSettings)
		{
			_userInfoService = userInfoService;
			_hudProvider = hudProvider;
			_observationRep = observationRep;
			_observationEntryRepository = observationEntryRepository;
			_observationChangeRepository = observationChangeRepository;
			_observationCommentRepository = observationCommentRepository;
			_observationAttachmentRepository = observationAttachmentRepository;
			_observationService = ObservationService;
			_deviceSettings = deviceSettings;

			TestDate = DateTime.Now;


			MessagingCenter.Subscribe<ObservationChange>(this, "ChangeScreen", (change) =>
			{
				if (Changes == null)
				{
					Changes = new ObservableCollection<ObservationChange>();
				}
				var item = Changes.Where(m => m == change).FirstOrDefault();
				if (item != null)
				{
					Changes.Remove(item);
					Changes.Add(item);
				}
				else
				{
					Changes.Add(change);
				}
				if (_Navigation.ModalStack.Count != 0)
					_Navigation.PopModalAsync();
			});

			MessagingCenter.Subscribe<ObservationAttachment>(this, "AttachmentScreen", (attach) =>
			{
				if (Attachments == null)
				{
					Attachments = new ObservableCollection<ObservationAttachment>();
				}
				var item = Attachments.Where(m => m == attach).FirstOrDefault();
				if (item != null)
				{
					Attachments.Remove(item);
					Attachments.Add(item);
				}
				else
				{
					Attachments.Add(attach);
				}
			});

			MessagingCenter.Subscribe<ObsViewModel>(this, "NumDemUpdate", (change) =>
			{
				RaisePropertyChanged("IndicatorNumCount");
				RaisePropertyChanged("IndicatorDenCount");
				RaisePropertyChanged("IndicatorCount");
                RaisePropertyChanged("IndicatorCountDisplay");
			});



			MessagingCenter.Subscribe<ObservationAttachment>(this, "AttachmentScreenCancel", (change) =>
			{
				if (_Navigation.ModalStack.Count != 0)
					_Navigation.PopModalAsync();
			});

			MessagingCenter.Subscribe<ObservationChange>(this, "ChangeScreenCancel", (change) =>
			{
				Changes = Changes;
				if (_Navigation.ModalStack.Count != 0)
					_Navigation.PopModalAsync();
			});

			MessagingCenter.Subscribe<ObservationComment>(this, "CommentScreen", (comment) =>
			{

				var item = Comments.Where(m => m == comment).FirstOrDefault();
				if (item != null)
				{
					Comments.Remove(item);
					Comments.Add(item);
				}
				else
				{
					var i = Comments.Count();
					Comments.Add(comment);
				}
				if (_Navigation.ModalStack.Count != 0)
					_Navigation.PopModalAsync();
			});

			MessagingCenter.Subscribe<ObservationComment>(this, "CommentScreenCancel", (change) =>
			{
				Comments = Comments;
				if (_Navigation.ModalStack.Count != 0)
					_Navigation.PopModalAsync();
			});

		}
	}
}
