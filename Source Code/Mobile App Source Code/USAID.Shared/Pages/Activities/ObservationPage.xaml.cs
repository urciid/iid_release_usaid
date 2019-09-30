using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Plugin.Media;
using Plugin.Media.Abstractions;
using USAID.Base;
using USAID.Interfaces;
using USAID.Models;
using USAID.ViewModels;
using Xamarin.Forms;

namespace USAID.Pages
{
	public class ObservationPageBase : ViewPage<ObservationViewModel>
	{
	}
	public partial class ObservationPage : ObservationPageBase
	{
		bool inPageLoad = false;
		void Handle_BindingContextChanged(object sender, System.EventArgs e)
		{
			base.OnBindingContextChanged();

			if (BindingContext == null)
				return;

			ViewCell theViewCell = ((ViewCell)sender);
			var item = theViewCell.BindingContext as ObsViewModel;

			if (item != null)
			{

				//check for height
				if (item.IsIndicatorYesNo)
				{
					theViewCell.Height = 100;
				}
				else if (item.IsIndicatorNormal)
				{
					theViewCell.Height = 220;
				}
				else if (item.IsIndicatorCount)
				{
					theViewCell.Height = 100;
				}
				else 
				{
					theViewCell.Height = 250;
				}
			}
		}
		public ObservationPage(IndicatorVM indicator, Site site)
		{
			inPageLoad = true;
			ViewModel.Site = site;
			ViewModel._Navigation = Navigation;
			ViewModel.Indicator = indicator;
			ViewModel.Indicator.ObservationsTemplate = ViewModel.GetTemplates();
			ViewModel.ImageVisible = false;


			//need to load observations from Indicator - get observations by indicatorid and siteid
			//load last 6 months of periods
			ViewModel.Indicator.Periods = ViewModel.LoadPeriodsForNoObservations();

			if (indicator.Observations == null || indicator.Observations.Count == 0)
			{
				//then need to load observations from observation templates
				indicator.ObservationEntrys = indicator.ObservationsTemplate;
			}
			else {
				//load periods from observations
				ViewModel.LoadPeriodsWithObservations();
			}

			InitializeComponent();
			foreach (var option in ViewModel.Periods)
			{
				DateTime last = option.BeginDate == null ? new DateTime(1900, 12, 1) : (DateTime)option.BeginDate;
				var ci = DependencyService.Get<ILocale>().GetCurrentCultureInfo();
				var date = string.Format(ci, "{0:M/d/yyyy}", last);
				if (last.Date >= ViewModel.Indicator.StartDate.Date)
				{
					periodPicker.Items.Add(date.ToString());
				}

			}
			if (ViewModel.SelectedPeriod != null)
			{
				var last = ViewModel.SelectedPeriod.BeginDate;
				var ci = DependencyService.Get<ILocale>().GetCurrentCultureInfo();
				var date = string.Format(ci, "{0:M/d/yyyy}", last);
				periodPicker.SelectedIndex = periodPicker.Items.IndexOf(date.ToString());
			}
			else {
				periodPicker.SelectedIndex = 0;
				ViewModel.SelectedPeriod = ViewModel.Periods[periodPicker.SelectedIndex];

			}
			//if (ViewModel.IndicatorTypeIsNormal)
			//{
			//	listObservationsNormal.Footer = this
			//listObservationsNormal.Footer = new DataTemplate(typeof(Label))
			//	{
			//		Bindings = {
			//			{ Label.TextProperty, new Binding("IndicatorNumCount") }
			//		}    };

			inPageLoad = false;
		}
		private void changeListHeights()
		{
			
			int changeCount = ViewModel.Changes == null ? 1 : ViewModel.Changes.Count;
			int commentCount = ViewModel.Comments == null ? 1 : ViewModel.Comments.Count;
			int attachCount = ViewModel.Attachments == null ? 1 : ViewModel.Attachments.Count;
			listChanges.HeightRequest = (listChanges.RowHeight * changeCount)+10;
			listComments.HeightRequest = (listComments.RowHeight * commentCount) + 10;
			listAttachments.HeightRequest = (listAttachments.RowHeight * attachCount) + 10;
			grdMain.RowDefinitions[6].Height = new GridLength(this.ViewModel.GetObservationRowHeight());
			grdMain.RowDefinitions[8].Height = new GridLength(this.ViewModel.GetChangesRowHeight());
			grdMain.RowDefinitions[10].Height = new GridLength(this.ViewModel.GetCommentsRowHeight());
			grdMain.RowDefinitions[12].Height = new GridLength(this.ViewModel.GetAttachmentsRowHeight());

		}
		protected override void OnAppearing()
		{
			base.OnAppearing();
			DisaggregateStackLayout.IsVisible = ViewModel.DisaggregatedAvailable;
			DisaggregateStackLayoutBottom.IsVisible = ViewModel.DisaggregatedAvailable;
			changeListHeights();
		}

		public void PeriodPickerValueChanged(object sender, System.EventArgs e)
		{
			//if (App.isDirty)
			//{
			//	var answer = await DisplayAlert("Unsaved Changes", "Are you sure you want to leave without saving?", "Yes", "No");
			//	if (!answer)
			//	{
			//		periodPicker.Unfocus();
			//		return;
			//	}

			//}

			var x = periodPicker.SelectedIndex;

			if (periodPicker.SelectedIndex >= 0 && periodPicker.SelectedIndex < ViewModel.Periods.Count)
			{
				ViewModel.SelectedPeriod = ViewModel.Periods[x];
			}
			periodPicker.Unfocus();
			changeListHeights();
		}

		void DisaggregatedSelected_Toggled(object sender, Xamarin.Forms.ToggledEventArgs e)
		{
			
			var mi = e.Value;
			this.ViewModel.DisaggregateCommand.Execute(mi);
			var x = this.ViewModel.Obs.Count;
			var item = BindingContext as ObservationViewModel;
			changeListHeights();


			//grdMain.RowDefinitions[6].Height = new GridLength(this.ViewModel.GetObservationRowHeight());

		}

		//public async void OnDetails(object sender, EventArgs e)
		//{
		//	//var mi = ((MenuItem)sender);
		//	//Indicator indicator = (Indicator)mi.CommandParameter;
		//	//await Navigation.PushAsync(new ActivityDetailPage(indicator.Activity));
		//}

		public async void OnObservationItemTapped(object sender, EventArgs e)
		{
			if (sender == null) return;
			// do something with e.SelectedItem
			var x = ((ListView)sender);

			 ((ListView)sender).SelectedItem = null;
		}

		public async void OnChangeItemTapped(object sender, ItemTappedEventArgs e)
		{
			ObservationChange change = (ObservationChange)e.Item;
			//await Navigation.PushAsync(new ObservationPage(indicator, _site));
			await Navigation.PushModalAsync(new ChangePage(change));
		}

		public async void OnCommentsItemTapped(object sender, ItemTappedEventArgs e)
		{
			ObservationComment comment = (ObservationComment)e.Item;
			//await Navigation.PushAsync(new ObservationPage(indicator, _site));
			await Navigation.PushModalAsync(new CommentPage(comment));
		}

		public async void OnAttachmentsItemTapped(object sender, ItemTappedEventArgs e)
		{
			ObservationAttachment attach = (ObservationAttachment)e.Item;
			////await Navigation.PushAsync(new ObservationPage(indicator, _site));
			await Navigation.PushModalAsync(new AttachmentPage(attach));
		}

		async void CameraButtonClicked(object sender, System.EventArgs e)
		{
			//taken from https://github.com/jamesmontemagno/MediaPlugin
			if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
			{
				await App.CurrentApp.MainPage.DisplayAlert("No Camera", "No camera available.", "OK");
				return;
			}
			var filename = DateTime.Now.ToString("yyyy-dd-M--HH-MM-ss") + ".jpg";
			var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
			{
				Directory = "USAID",
				Name = filename,
				PhotoSize = PhotoSize.Small
			});
			if (file == null)
				return;
			if (ViewModel.Attachments == null)
				ViewModel.Attachments = new ObservableCollection<ObservationAttachment>();
			
			//var image = new Image();

			//image.Source = ImageSource.FromStream(() =>
			//{
			//	var stream = file.GetStream();
			//	file.Dispose();
			//	return stream;
			//});

			System.IO.Stream stream = file.GetStream();
			stream.Position = 0;
			byte[] buffer = new byte[stream.Length];
			for (int totalBytesCopied = 0; totalBytesCopied < stream.Length;)
				totalBytesCopied += stream.Read(buffer, totalBytesCopied, Convert.ToInt32(stream.Length) - totalBytesCopied);

			var attachment = new ObservationAttachment {Bytes = buffer, Attachment_File_Name = filename, Created = DateTime.Now, Created_Date = DateTime.Now };
			//MessagingCenter.Send(attachment, "AttachmentScreen");
			if (ViewModel.Attachments == null)
			{
				ViewModel.Attachments = new ObservableCollection<ObservationAttachment>();
			}
			var item = ViewModel.Attachments.Where(m => m == attachment).FirstOrDefault();
			if (item != null)
			{
				ViewModel.Attachments.Remove(item);
				ViewModel.Attachments.Add(item);
			}
			else
			{
				ViewModel.Attachments.Add(attachment);
			}
			changeListHeights();
		}

		//void Handle_BindingContextChanged(object sender, System.EventArgs e)
		//{
		//	base.OnBindingContextChanged();

		//	if (BindingContext == null)
		//		return;

		//	Grid theList = ((Grid)sender);
		//	var item = theList.BindingContext as ObservationViewModel;


		//	if (item != null)
		//	{
		//		//theList.HeightRequest = 300;
		//	}

		//}

		//onappearing 
		//int height = 320;
		//		//if (IndicatorTypeIsYesNo)
		//		//{
		//		//	height = 80 * Obs.Count();
		//		//}
		//		//else {
		//		//	height = 160 * Obs.Count();
		//		//}

		//		return height;
	}
}

