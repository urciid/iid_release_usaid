using USAID.Interfaces;
using USAID.Base;
using USAID.Utilities;
using System.Windows.Input;
using Xamarin.Forms;
using USAID.Resx;
using System;
using System.Threading.Tasks;
using USAID.Models;
using USAID.Repositories;
using System.Linq;
using System.Collections.ObjectModel;

namespace USAID.ViewModels
{
	public class LandingViewModel : BaseViewModel
	{
		private readonly IUserInfoService _userInfoService;
		private readonly IHUDProvider _hudProvider;
		private readonly ILocale _localeService;
		private readonly IObservationService _observationService;
		private readonly ISiteRepository _siteRepository;
		private readonly IActivityRepository _activityRepository;
		private readonly IIndicatorRepository _indicatorRepository;
		private readonly ISiteIndicatorRepository _siteIndicatorRepository;
		private readonly IObservationRepository _observationRepository;
		private readonly IObservationEntryRepository _observationEntryRepository;
		private readonly IObservationChangeRepository _observationChangeRepository;
		private readonly IObservationCommentRepository _observationCommentRepository;
		private readonly IObservationAttachmentRepository _observationAttachmentRepository;
		private readonly IIndicatorAgeRepository _indicatorAgeRepository;
		private readonly IDeviceSettings _deviceSettings;

		public int LanguageId
		{
			get { return GetValue<int>(); }
			set { SetValue(value); }
		}

		public string DownloadText
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public string LastInformationText
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public string LastInformationTextDate
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public string DownloadButtonText
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public DateTime LastDownloadDate
		{
			get { return GetValue<DateTime>(); }
			set { SetValue(value); }
		}

		public LandingViewModel(IGALogger logger, IUserInfoService userInfoService,
		                        IHUDProvider hudProvider, IObservationService observationService, ISiteRepository siteRepository, 
		                        IActivityRepository activityRepository, IIndicatorRepository indicatoryRepository,
		                        ISiteIndicatorRepository siteIndicatoryRepository, IObservationRepository observationRepository,
		                        IObservationEntryRepository observationEntryRepository, IObservationChangeRepository observationChangeRepository,
		                        IObservationCommentRepository observationCommentRepository, IObservationAttachmentRepository observationAttachmentRepository,
		                        IIndicatorAgeRepository indicatoryAgeRepository, IDeviceSettings deviceSettings)
		{
			_userInfoService = userInfoService;
			_hudProvider = hudProvider;
			_observationService = observationService;
			_siteRepository = siteRepository;
			_activityRepository = activityRepository;
			_indicatorRepository = indicatoryRepository;
			_siteIndicatorRepository = siteIndicatoryRepository;
			_observationRepository = observationRepository;
			_observationEntryRepository = observationEntryRepository;
			_observationChangeRepository = observationChangeRepository;
			_observationCommentRepository = observationCommentRepository;
			_observationAttachmentRepository = observationAttachmentRepository;
			_indicatorAgeRepository = indicatoryAgeRepository;
			_deviceSettings = deviceSettings;

			DownloadText = AppResources.DownloadText;
			LastInformationText = AppResources.LastInformationText;
			DownloadButtonText = AppResources.DownloadButtonText;

			var culture = DependencyService.Get<ILocale>().GetCurrentCultureInfo();
			if (culture.TwoLetterISOLanguageName == "fr")
			{
				LanguageId = 2;
			}
			else if (culture.TwoLetterISOLanguageName == "es")
			{
				LanguageId = 3;
			}
			else {
				LanguageId = 1;
			}


			//GetUserInfo
			var userInfo = _userInfoService.GetSavedInfo();
			if (!string.IsNullOrEmpty(userInfo.LastDownload))
			{
				try
				{
					var last = DateTime.Parse(userInfo.LastDownload);
					var ci = DependencyService.Get<ILocale>().GetCurrentCultureInfo();
					LastInformationTextDate = userInfo.LastDownload; //string.Format(ci, "{0}", last);
				}
				catch
				{
					LastInformationText = AppResources.NoDownloadText;
				}

			}
			else {
				LastInformationText = AppResources.NoDownloadText;
			}


		}

		public ICommand DownloadCommand { get { return new SimpleCommand(Download); } }
		DateTime OffSetDateTimeZone(DateTime date)
		{
			var offset = _deviceSettings.GetCurrentUTCOffset();
			var newDate = date.AddHours(offset);
			//var newDate = TimeZoneInfo.ConvertTimeToUTC(date, TimeZoneInfo.Local);

			return newDate;		
		}

		private async void Download()
		{
			try
			{
				//TODO: check if connectivity
				if (!App.IsConnected)
				{
					await App.CurrentApp.MainPage.DisplayAlert(AppResources.NoInternetText, "", AppResources.OKButton);
				}
				else {
					//TODO: if any locally saved data, then uplo
					_hudProvider.DisplayProgress(AppResources.WorkingText);
					var observations = _observationRepository.All(mn => mn.ModifiedLocally == true);
					if (observations != null && observations.Count > 0)
					{
						var userinfo = _userInfoService.GetSavedInfo();
						foreach (var ob in observations)
						{
							ob.Begin_Date = OffSetDateTimeZone((DateTime)ob.Begin_Date);
							ob.End_Date = OffSetDateTimeZone((DateTime)ob.End_Date);
							var obsResponse = await _observationService.ObservationSave(userinfo.Email, ob);
							_observationRepository.Upsert(obsResponse.observation);
						}

					}
					var user = _userInfoService.GetSavedInfo();
					var response = await _observationService.GetAllData(user.Email, LanguageId );
					if (response != null)
					{
						//Need to determine if observations need to be uploaded
						_siteRepository.DeleteAll();
						_activityRepository.DeleteAll();
						_indicatorRepository.DeleteAll();
						_siteIndicatorRepository.DeleteAll();
						_observationRepository.DeleteAll();
						_observationEntryRepository.DeleteAll();
						_observationChangeRepository.DeleteAll();
						_observationCommentRepository.DeleteAll();
						_observationAttachmentRepository.DeleteAll();
						_indicatorAgeRepository.DeleteAll();

						//have response object so need to insert into database
						if (response.Sites != null)
						{
							foreach (var item in response.Sites)
							{
								_siteRepository.Upsert(item);
							}
						}
						if (response.Activities != null)
						{
							foreach (var activity in response.Activities)
							{
								_activityRepository.Upsert(activity);
							}
						}
						if (response.Indicators != null)
						{
							foreach (var indicator in response.Indicators)
							{
								_indicatorRepository.Upsert(indicator);
							}
						}
						if (response.SiteIndicators != null)
						{
							foreach (var si in response.SiteIndicators)
							{
								_siteIndicatorRepository.Upsert(si);
							}
						}
						if (response.Observations != null)
						{
							foreach (var obs in response.Observations)
							{
								//changes
								var changes = response.ObservationChanges.ToList().Where(m => m.Observation_id == obs.Observation_id).ToList();
								obs.Changes = new ObservableCollection<ObservationChange>(changes);

								// comments
								var comments = response.ObservationComments.ToList().Where(m => m.Observation_Id == obs.Observation_id).ToList();
								obs.Comments = new ObservableCollection<ObservationComment>(comments);

								// attachments
								var attachments = response.ObservationAttachments.ToList().Where(m => m.Observation_Id == obs.Observation_id).ToList();
								obs.Attachments = new ObservableCollection<ObservationAttachment>(attachments);

								// Entries
								var entries = response.ObservationEntries.ToList().Where(m => m.Observation_id == obs.Observation_id).ToList();
								obs.ObservationEntries = new ObservableCollection<ObservationEntry>(entries);


								//Need to get entries, changes, comments, and attachments
								_observationRepository.Upsert(obs);
							}
						}

						if (response.IndicatorAgePeriods != null)
						{
							foreach (var age in response.IndicatorAgePeriods)
							{
								_indicatorAgeRepository.Upsert(age);
							}
						}
						////save to database
						//LastInformationText = AppResources.LastInformationText;
						//var ci = DependencyService.Get<ILocale>().GetCurrentCultureInfo();
						//var date = string.Format(ci, "{0}", DateTime.Now);
						//LastInformationText = String.Format(AppResources.LastInformationText, date);

						user.LastDownload = DateTime.Now.ToString();
						_userInfoService.SaveUserInfo(user);
						var last = DateTime.Parse(user.LastDownload);
						var ci = DependencyService.Get<ILocale>().GetCurrentCultureInfo();
						LastInformationTextDate = user.LastDownload; //string.Format(ci, "{0}", last);
						LastInformationText = AppResources.LastInformationText;
				}



						
						
					//}
				}
				//return RateOptionsSubmissionResult.Failure;
			}
			catch (Exception ex)
			{
				var x = ex.ToString();
				//return RateOptionsSubmissionResult.Failure; //tell view to pop alert
				//display error
			}
			finally
			{
				_hudProvider.Dismiss();
			}
		}
	}
}
