using System;
using System.Collections.Generic;
using USAID.Base;
using USAID.Common;
using USAID.ViewModels;
using Media.Plugin;
using Media.Plugin.Abstractions;
using Xamarin.Forms;

namespace USAID.Pages
{
    public class CustomerInfoPageBase : ViewPage<CustomerInfoViewModel>
    {
        
    }

    public partial class CustomerInfoPage : CustomerInfoPageBase
    {

        public CustomerInfoPage()
        {
            InitializeComponent();

			NavigationPage.SetBackButtonTitle(this, string.Empty);

            statePicker.Items.Add(string.Empty); //empty for first field
            foreach (var state in Constants.States)
            {
                statePicker.Items.Add(state);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ViewModel.FieldsEnabled = true;
            ViewModel.PhotoAttached = false;
            ViewModel.PopulateFields();
            //if ViewModel.State is null, the binding doesn't update the picker
            if (string.IsNullOrWhiteSpace(ViewModel.State))
            {
                statePicker.SelectedIndex = 0;
            }
        }

        async void CameraButtonClicked(object sender, System.EventArgs e)
        {
            //taken from https://github.com/jamesmontemagno/MediaPlugin
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                DisplayAlert("No Camera", "No camera available.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "CreditAppCustomerInfo",
                Name = DateTime.Now.ToString("G") + ".jpg"
            });

            if (file == null)
                return;

            ViewModel.SetPhotoFilePathOnCreditApp(file.Path);

            var image = new Image();

            image.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                file.Dispose();
                return stream;
            });
        }

        void StatePickerValueChanged(object sender, System.EventArgs e)
        {
            ViewModel.State = statePicker.Items[statePicker.SelectedIndex];
        }

        internal async void OnNextButtonClicked(object sender, EventArgs args)
        {
            ViewModel.SetCustomerInfoOnCreditApp();
            await Navigation.PushAsync(new ContractTermsPage());
        }

        internal void OnRemoveButtonClicked(object sender, EventArgs args)
        {
            ViewModel.RemovePhoto();
        }
    }
}

