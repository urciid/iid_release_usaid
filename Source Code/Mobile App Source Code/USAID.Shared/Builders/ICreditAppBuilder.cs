using System;
using System.Threading.Tasks;
using USAID;
using USAID.Enumerations;
using USAID.Models;

namespace USAID.Builders
{
    public interface ICreditAppBuilder
    {
        CreditApp GetCreditApp();

        void CreateCreditApp(CreditApp creditApp = null);

        void SetCustomerInfo(CustomerInfo customerInfo);

        void SetPhotoFilePath(string path);

        void RemovePhoto();

        void SetContractTerms(ContractTerms contractTerms);

        void SetGuarantor(Guarantor guarantor);

        Task<CreditAppSubmissionResult> SubmitCreditApp();
    }
}

