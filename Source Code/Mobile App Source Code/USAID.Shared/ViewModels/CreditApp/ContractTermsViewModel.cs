using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using USAID.Builders;
using USAID.Interfaces;
using USAID.Utilities;
using USAID.Models;
using USAID.Enumerations;

namespace USAID.ViewModels
{
    public class ContractTermsViewModel : BaseViewModel
    {
        private readonly ICreditAppBuilder _creditAppBuilder;
        private readonly ICreditAppService _creditAppService;
        private readonly IHUDProvider _hudProvider;
        private readonly IDealerDefaultsManager _dealerDefaultsManager;
        
        #region Properties

        public string EquipmentDescription
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string TotalAmount
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string TotalFinancedAmount
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string MaintenanceFeeAmount
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public int DesiredFinanceTerm
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public string DesiredPurchaseOption
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string Comments
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public bool ShowPassThru
        {
            get
            {
                return _dealerDefaultsManager.DealerDefaults.ShowPassThruOnCreditApp;
            }
        }

        public bool ShowGuarantorPage
        {
            get
            {
                return _dealerDefaultsManager.DealerDefaults.ShowPersonalGuarantor;
            }
        }

        #endregion

        public ContractTermsViewModel(ICreditAppBuilder creditAppBuilder, ICreditAppService creditAppService,
             IHUDProvider hudProvider, IDealerDefaultsManager dealerDefaultsManager)
        {
            _creditAppBuilder = creditAppBuilder;
            _creditAppService = creditAppService;
            _hudProvider = hudProvider;
            _dealerDefaultsManager = dealerDefaultsManager;
        }

        internal void PopulateFields()
        {
            //populate with info originating from quote work flow
            var creditApp = _creditAppBuilder.GetCreditApp();
            if (creditApp != null && creditApp.FromQuoteWorkflow)
            {
                EquipmentDescription = creditApp.EquipmentDescription;
                TotalAmount = creditApp.TotalAmount.ToString(); //only populate if came from credit app
                MaintenanceFeeAmount = creditApp.MaintenanceFeeAmount.ToString();
                DesiredFinanceTerm = creditApp.DesiredFinanceTerm;
                DesiredPurchaseOption = creditApp.DesiredPurchaseOption;
            }
        }

        internal void SetContractTermsOnCreditApp()
        {
            _creditAppBuilder.SetContractTerms(new ContractTerms
            {
                EquipmentDescription = EquipmentDescription,
                TotalAmount = TotalAmount,
                TotalFinancedAmount = TotalFinancedAmount,
                MaintenanceFeeAmount = MaintenanceFeeAmount,
                DesiredFinanceTerm = DesiredFinanceTerm,
                DesiredPurchaseOption = DesiredPurchaseOption,
                Comments = Comments
            });
        }

        internal async Task<CreditAppSubmissionResult> SubmitCreditApp(bool guarantorSectionOn = false)
        {
            _hudProvider.DisplayProgress("Submitting Credit App");

            if (!guarantorSectionOn)
            {
                _creditAppBuilder.SetGuarantor(new Guarantor()); //do this so that the guarantor fields aren't null (causes service error)
            }

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

