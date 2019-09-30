using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using USAID.ApplicationObjects;
using USAID.Enumerations;
using USAID.Extensions;
using USAID.Interfaces;
using USAID.Models;

namespace USAID.Builders.Impl
{
    public class CreditAppBuilder : ICreditAppBuilder
    {
        private CreditApp _creditApp;

        public CreditApp GetCreditApp()
        {
            return _creditApp;
        }

        public void CreateCreditApp(CreditApp creditApp = null)
        {
            //credit app passed in when coming from quote workflow
            _creditApp = creditApp ?? new CreditApp();
        }
        
        public void SetCustomerInfo(CustomerInfo customerInfo)
        {
            if (_creditApp == null)
            {
                CreateCreditApp();
            }

            _creditApp.CompanyName = customerInfo.CompanyName ?? string.Empty;
            _creditApp.MailingAddress = customerInfo.MailingAddress ?? string.Empty;
            _creditApp.City = customerInfo.City ?? string.Empty;
            _creditApp.State = customerInfo.State ?? string.Empty;
            _creditApp.PostalCode = customerInfo.PostalCode ?? string.Empty;
            _creditApp.PhoneNumber = customerInfo.PhoneNumber ?? string.Empty;
            _creditApp.DBA = customerInfo.DBA ?? string.Empty;
            _creditApp.ContactName = customerInfo.ContactName ?? string.Empty;
            _creditApp.ContactEmail = customerInfo.ContactEmail ?? string.Empty;
            _creditApp.ContactPhone = customerInfo.ContactPhone ?? string.Empty;
        }

        public void SetPhotoFilePath(string path)
        {
            _creditApp.PhotoFilePath = path;
        }

        public void RemovePhoto()
        {
            _creditApp.PhotoFilePath = null;
        }

        public void SetContractTerms(ContractTerms contractTerms)
        {
            if (_creditApp == null)
            {
                CreateCreditApp(); //this should never happen
            }

            _creditApp.EquipmentDescription = contractTerms.EquipmentDescription ?? string.Empty;
            _creditApp.TotalAmount = Convert.ToDouble(contractTerms.TotalAmount);
            _creditApp.TotalFinancedAmount = Convert.ToDouble(contractTerms.TotalFinancedAmount);
            _creditApp.MaintenanceFeeAmount = Convert.ToDouble(contractTerms.MaintenanceFeeAmount);
            _creditApp.DesiredFinanceTerm = contractTerms.DesiredFinanceTerm;
            _creditApp.DesiredPurchaseOption = contractTerms.DesiredPurchaseOption ?? string.Empty;
            _creditApp.Comments = contractTerms.Comments ?? string.Empty;
        }

        public void SetGuarantor(Guarantor guarantor)
        {
            if (_creditApp == null)
            {
                CreateCreditApp(); //this should never happen
            }

            _creditApp.GuarantorFirstName = guarantor.FirstName ?? string.Empty;
            _creditApp.GuarantorMiddleInitial = guarantor.MiddleInitial ?? string.Empty;
            _creditApp.GuarantorLastName = guarantor.LastName ?? string.Empty;
            _creditApp.GuarantorAddress = guarantor.Address ?? string.Empty;
            _creditApp.GuarantorCity = guarantor.City ?? string.Empty;
            _creditApp.GuarantorState = guarantor.State ?? string.Empty;
            _creditApp.GuarantorZip = guarantor.Zip ?? string.Empty;
        }

        public async Task<CreditAppSubmissionResult> SubmitCreditApp()
        {
            //must send email if photo taken for customer info
            if (!string.IsNullOrWhiteSpace(_creditApp.PhotoFilePath))
            {
                IEmailService _emailService = AppContainer.Container.Resolve<IEmailService>();
                _emailService.CreateEmail(_creditApp.ToSubmissionEmail());
                return CreditAppSubmissionResult.Success;
            }

            //if all data manually entered (no photo), send to API
            ICreditAppService _creditAppService = AppContainer.Container.Resolve<ICreditAppService>();
            var response = await _creditAppService.SubmitCreditApp(_creditApp);
            if (response != null)
            {
                if (response.ApplicationID != 0)
                {
                    _creditApp.AppId = response.ApplicationID;
                    return CreditAppSubmissionResult.Success;
                }
                else if (response.ErrorStatusCode == 401)
                {
                    return CreditAppSubmissionResult.Unauthorized;
                }
                else
                {
                    return CreditAppSubmissionResult.Failure;
                }
            }
            else
            {
                return CreditAppSubmissionResult.Failure;
            }
        }
    }
}

