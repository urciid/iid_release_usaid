using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using USAID.Common;
using USAID.Interfaces;
using USAID.Models;
using USAID.Repositories;
using USAID.Services.ServiceModels;

namespace USAID.Services
{
    public class CreditAppService : ICreditAppService
    {
        private readonly IProfileInfoRepository _profileRepo;

        public CreditAppService(IProfileInfoRepository profileRepo)
        {
            _profileRepo = profileRepo;
        }

        public async Task<SubmitCreditAppResponse> SubmitCreditApp(CreditApp creditApp)
        {
            //=== test data ===
            //var submitCreditAppRequest = new SubmitCreditAppRequest
            //{
            //    Application = new CreditAppMetaData
            //    {
            //        EnteredBy = "",
            //        ApplicationType = "New",
            //        ApplicationFormat = "STD",
            //        TotalfinancedAmount = 0
            //    },
            //    Terms = new TermData
            //    {
            //        Payment = 0,
            //        SecurityDeposit = 0.0,
            //        RatecardID = 0,
            //        Term = 0,
            //        LeaseType = "",
            //        PurchaseOption = ""
            //    },
            //    Vendor = new Vendor
            //    {
            //        DealerContactName = "",
            //        DealerContactEmail = "",
            //        DealerContactphone = ""
            //    },
            //    Lessee = new Lessee
            //    {
            //        LegalName = "test",
            //        Address1 = "",
            //        City = "",
            //        State = "",
            //        Zip = "",
            //        Phone = "",
            //        Contact = ""
            //    },
            //    Assets = new List<Asset>()
            //    {
            //        new Asset
            //        {
            //            Quantity = 1,
            //            Description = "",
            //            SerialNumber = "",
            //            Model = "",
            //            Manufacturer = "",
            //            TotalAmount = 0,
            //            IsUsed = 0
            //        }
            //    }
            //};

            var dealerProfile = _profileRepo.GetDealerProfile();

            var submitCreditAppRequest = new SubmitCreditAppRequest
            {
                Application = new CreditAppMetaData
                {
                    EnteredBy = dealerProfile.DealerContactName,
                    ApplicationType = "New",
                    ApplicationFormat = "STD",
                    TotalFinancedAmount = creditApp.TotalFinancedAmount,
                    MaintenanceFeeAmount = creditApp.MaintenanceFeeAmount,
                    Notes = creditApp.Comments
                },
                Terms = new TermData
                {
                    Payment = creditApp.Payment, //from quote
                    SecurityDeposit = 0.0,
                    RatecardID = creditApp.RateCardId, //from quote
                    Term = creditApp.DesiredFinanceTerm,
                    LeaseType = "", //new field on screen, wait for Carol to get back to us
                    PurchaseOption = creditApp.DesiredPurchaseOption
                },
                Vendor = new Vendor
                {
                    DealerName = dealerProfile.DealerName,
                    DealerContactName = dealerProfile.DealerContactName,
                    DealerContactEmail = dealerProfile.DealerContactEmail,
                    DealerContactPhone = dealerProfile.DealerContactPhone,
                    DealerRelationship = "Booking"
                },
                Lessee = new Lessee
                {
                    LegalName = creditApp.CompanyName,
                    Address1 = creditApp.MailingAddress,
                    City = creditApp.City,
                    State = creditApp.State,
                    Zip = creditApp.PostalCode,
                    Phone = creditApp.PhoneNumber,
                    DBA = creditApp.DBA,
                    Contact = creditApp.ContactName,
                    ContactEmail = creditApp.ContactEmail,
                    ContactPhone = creditApp.ContactPhone
                },
                Assets = new List<Asset>()
                {
                    new Asset
                    {
                        //new fields on screen, wait for Carol to get back to us
                        Quantity = 1,
                        Description = creditApp.EquipmentDescription,
                        TotalAmount = creditApp.TotalAmount,
                        SerialNumber = "",
                        Model = "",
                        Manufacturer = "",
                        IsUsed = 0
                    }
                },
                Guarantors = new List<Guarantor>()
                {
                    new Guarantor
                    {
                        FirstName = creditApp.GuarantorFirstName,
                        MiddleInitial = creditApp.GuarantorMiddleInitial,
                        LastName = creditApp.GuarantorLastName,
                        Address = creditApp.GuarantorAddress,
                        City = creditApp.GuarantorCity,
                        State = creditApp.GuarantorState,
                        Zip = creditApp.GuarantorZip
                    }
                }
            };

            var response = await WebServiceClientBase.Post<SubmitCreditAppRequest, SubmitCreditAppResponse>(Constants.CreditAppUri, submitCreditAppRequest, true);
            return response;
        }
    }
}

