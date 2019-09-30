using System;
using System.IO;
using System.Windows.Input;
using Autofac;
using USAID.ApplicationObjects;
using USAID.Builders;
using USAID.Interfaces;
using USAID.Models;
using USAID.Utilities;

namespace USAID.ViewModels
{
    public class CustomerInfoViewModel : BaseViewModel
    {
        private readonly ICreditAppBuilder _creditAppBuilder;
        
        #region Properties

        public string PhotoFileName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public bool PhotoAttached
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        //fields are disabled if user takes photo of business card
        public bool FieldsEnabled
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public string CompanyName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string MailingAddress
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string City
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string State
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string PostalCode
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string PhoneNumber
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string DBA
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        //Contact Info

        public string ContactName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string ContactEmail
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string ContactPhone
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        #endregion

        public CustomerInfoViewModel(ICreditAppBuilder creditAppBuilder)
        {
            _creditAppBuilder = creditAppBuilder;
        }

        internal void PopulateFields()
        {
            var creditApp = _creditAppBuilder.GetCreditApp();
            if (creditApp != null)
            {
                CompanyName = creditApp.CompanyName;
                MailingAddress = creditApp.MailingAddress;
                City = creditApp.City;
                State = creditApp.State;
                PostalCode = creditApp.PostalCode;
                PhoneNumber = creditApp.PhoneNumber;
                DBA = creditApp.DBA;
                ContactName = creditApp.ContactName;
                ContactEmail = creditApp.ContactEmail;
                ContactPhone = creditApp.ContactPhone;
            }
        }

        internal void SetCustomerInfoOnCreditApp()
        {
            _creditAppBuilder.SetCustomerInfo(new CustomerInfo
            {
                CompanyName = CompanyName,
                MailingAddress = MailingAddress,
                City = City,
                State = State,
                PostalCode = PostalCode,
                PhoneNumber = PhoneNumber,
                DBA = DBA,
                ContactName = ContactName,
                ContactEmail = ContactEmail,
                ContactPhone = ContactPhone
            });
        }

        internal void SetPhotoFilePathOnCreditApp(string path)
        {
            _creditAppBuilder.SetPhotoFilePath(path);

            var directoryPath = Path.GetDirectoryName(path);
            var userInfoService = AppContainer.Container.Resolve<IUserInfoService>();
            userInfoService.SavePhotoFilePath(directoryPath);

            PhotoFileName = Path.GetFileName(path);
            PhotoAttached = true;
            FieldsEnabled = false;
        }

        internal void RemovePhoto()
        {
            _creditAppBuilder.RemovePhoto();
            PhotoFileName = string.Empty;
            PhotoAttached = false;
            FieldsEnabled = true;
        }
    }
}

