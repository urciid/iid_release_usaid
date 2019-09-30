using USAID.Base;
using WMP.Core.Data.SQLiteNet;
using USAID.Models;
using Xamarin.Forms;
using USAID.ApplicationObjects;
using Autofac;
using USAID.Repositories.Impl;
using USAID.Repositories;
using USAID.Common;
using USAID.Interfaces;

namespace USAID.ViewModels
{
	public class ProfileViewModel : BaseViewModel
	{
        private readonly IProfileInfoRepository _profileRepo;

        #region Properties

        public string DealerName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string Company
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

        public string Email
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

        public string AutoCcEmail
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

		public string  PhoneNumber
        {
			get { return GetValue<string>(); }
			set { SetValue(value);}
		}


        #endregion

        public ProfileViewModel(IProfileInfoRepository profileRepo)
        {
            _profileRepo = profileRepo;
        }

        public void PopulateFields()
        {
            var info = _profileRepo.GetDealerProfile();
			if (info != null)
			{
                DealerName = info.DealerContactName;
				Company = info.DealerName;
				Email = info.DealerContactEmail;
                AutoCcEmail = info.AutoCcEmail;
				PhoneNumber = info.DealerContactPhone;
			}
            else
            {
                //populate fields from Salesforce dealer defaults if first login
                var dealerDefaults = AppContainer.Container.Resolve<IDealerDefaultsManager>().DealerDefaults;
                if (dealerDefaults != null)
                {
                    //TODO: company and dealer name?
                    Email = dealerDefaults.TeamEmail;
                    AutoCcEmail = dealerDefaults.EmailCc;
                    PhoneNumber = dealerDefaults.Phone;
                }
            }
        }

        // Return value specifies whether or not to allow user to continue.
        // We want to force users to enter all required data.
        public bool SaveInfo()
        {
            if (string.IsNullOrWhiteSpace(DealerName) || 
                string.IsNullOrWhiteSpace(Company))
            {
                //currently dealer name and dealer contact name are the only required fields
                return false;
            }

            var info = new ProfileInfo
            {
                Id = Constants.ProfileSqliteId,
                DealerContactName = DealerName,
                DealerName = Company,
                DealerContactEmail = Email,
                AutoCcEmail = AutoCcEmail,
                DealerContactPhone = PhoneNumber
            };

            _profileRepo.Upsert(info);
            return true;
        }
	}
}

