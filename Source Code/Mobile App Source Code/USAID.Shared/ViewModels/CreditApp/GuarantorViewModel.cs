using System;
using System.Threading.Tasks;
using System.Windows.Input;
using USAID.Builders;
using USAID.Enumerations;
using USAID.Interfaces;
using USAID.Models;
using USAID.Utilities;

namespace USAID.ViewModels
{
    public class GuarantorViewModel : BaseViewModel
    {
        private readonly IHUDProvider _hudProvider;
        private readonly ICreditAppBuilder _creditAppBuilder;
        private readonly ICreditAppService _creditAppService;

        #region Properties

        public string FirstName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string MiddleInitial
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string LastName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string Address
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

        public string Zip
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        #endregion

        public GuarantorViewModel(ICreditAppBuilder creditAppBuilder, ICreditAppService creditAppService,
            IHUDProvider hudProvider)
        {
            _creditAppBuilder = creditAppBuilder;
            _creditAppService = creditAppService;
            _hudProvider = hudProvider;
        }

        internal async Task<CreditAppSubmissionResult> SubmitCreditApp()
        {
            _hudProvider.DisplayProgress("Submitting Credit App");

            _creditAppBuilder.SetGuarantor(new Guarantor
            {
                FirstName = FirstName,
                MiddleInitial = MiddleInitial,
                LastName = LastName,
                Address = Address,
                City = City,
                State = State,
                Zip = Zip
            });

            try
            {
                return await _creditAppBuilder.SubmitCreditApp();
            }
            catch (Exception ex)
            {
                return CreditAppSubmissionResult.Failure; //tell view to pop alert
            }
            finally
            {
                _hudProvider.Dismiss();
            }
        }
    }
}

