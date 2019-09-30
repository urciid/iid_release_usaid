using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using USAID.Common;
using USAID.Interfaces;
using USAID.Models;
using USAID.Services.ServiceModels;
using USAID.Services;
using Xamarin.Forms;
using System.Linq;

namespace USAID.Services
{
    public class MonthlyPaymentService : IMonthlyPaymentService
    {
        public async Task<MonthlyPaymentResponse> GetMonthlyPayments(Quote quote)
        {
			//loop through quote rate cards and maintenancy types to create list of PaymentCalculationParameterSets
			//terms, purchase options,
			IEnumerable<int> terms = quote.Terms.Select(t => t.Term);
			IEnumerable<string> maintenanceTypes = quote.MaintenanceTypes.Select (m => m.MaintenanceTypeDescription);
			IEnumerable<string> purchaseOptions = quote.PurchaseOptions.Select (p => p.PurchaseOptionDesc);

			var paymentParamList = new List<PaymentCalculationParameterSet> ();

			foreach (RateCardLocal card in quote.RateCards)
			{
				if (maintenanceTypes.Count () > 0) {
					foreach (string mtype in maintenanceTypes) {
						//need to create a set for each rate card/maint type combo
						//var paymentSet = new PaymentCalculationParameterSet () {
						//	RateCardID = card.RateCardID,
						//	EquipmentCost = quote.EquipmentAmount,
						//	NumberOfPoints = quote.Points,
						//	MaintenanceType = mtype,
						//	MaintenanceAmount = 0,
						//	IncludeInvalidPayments = 0,
						//	AdvancePayments = new List<AdvancePayment> { new AdvancePayment { AdvancePaymentValue = quote.PassThrough } },
						//	PurchaseOptions = purchaseOptions.ToList(),
						//	Terms = terms.ToList()
						//};
						//paymentParamList.Add (paymentSet);



						var payment = new PaymentCalculationParameterSet () {
							RateCardID = card.RateCardID,
							EquipmentCost = quote.EquipmentAmount,
							NumberOfPoints = quote.Points,
							MaintenanceType = mtype.ToString (),
							MaintenanceAmount = quote.PassThrough,
							IncludeInvalidPayments = 0,
							AdvancePayments = quote.AdvancePayments,
							//AdvancePayments = new List<AdvancePayment> { new AdvancePayment { AdvancePaymentDescription = "Pass Through", AdvancePaymentValue = quote.PassThrough } },
							PurchaseOptions = purchaseOptions.ToList(),
							Terms = terms.ToList()
						};
						paymentParamList.Add (payment);
					}

				}
				else
				{
					var paymentSet = new PaymentCalculationParameterSet () {
						RateCardID = card.RateCardID,
						EquipmentCost = quote.EquipmentAmount,
						NumberOfPoints = quote.Points,
						MaintenanceType = "",
						MaintenanceAmount = quote.PassThrough,
						IncludeInvalidPayments = 0,
						AdvancePayments = new List<AdvancePayment> { new AdvancePayment { AdvancePaymentDescription="Pass Through", AdvancePaymentValue = quote.PassThrough } },
						PurchaseOptions = purchaseOptions.ToList (),
						Terms = terms.ToList ()
					};
					paymentParamList.Add (paymentSet);
				}
			}
			var monthlyPaymentRequest = new MonthlyPaymentRequest () {
				CalculatePayments = paymentParamList
			};

			//TODO: mocked this out for testing, replace with user inputs
			//var paymentParamList = new List<PaymentCalculationParameterSet>()
			//{
			//    new PaymentCalculationParameterSet()
			//    {
			//        RateCardID = 8509,
			//        EquipmentCost = 50000,
			//        NumberOfPoints = 3,
			//        MaintenanceType = "TPM",
			//        MaintenanceAmount = 1000,
			//        IncludeInvalidPayments = 0,
			//        AdvancePayments = new List<AdvancePayment> { new AdvancePayment { AdvancePaymentValue = 0 } },
			//        PurchaseOptions = new List<string> { "FMV Lease", "FMV Rental" },
			//        Terms = new List<int> { 36, 39, 48, 60 }
			//    }
			//};
			//var monthlyPaymentRequest = new MonthlyPaymentRequest()
			//{
			//    CalculatePayments = paymentParamList
			//};

			var response = await WebServiceClientBase.Post<MonthlyPaymentRequest, MonthlyPaymentResponse> (Constants.MonthlyPaymentUri, monthlyPaymentRequest, true);

			////TODO: mocked this out for testing, replace with user inputs
			//var paymentParamList = new List<PaymentCalculationParameterSet>()
			//{
			//    new PaymentCalculationParameterSet()
			//    {
			//        RateCardID = 9816,
			//        EquipmentCost = 50000,
			//        NumberOfPoints = 3,
			//        MaintenanceType = "TPM",
			//        MaintenanceAmount = 1000,
			//        IncludeInvalidPayments = 0,
			//        AdvancePayments = new List<AdvancePayment> { new AdvancePayment { AdvancePaymentValue = 0 } },
			//        PurchaseOptions = new List<string> { "FMV Lease", "FMV Rental" },
			//        Terms = new List<int> { 36, 39, 48, 60 }
			//    }
			//};
			//var monthlyPaymentRequest = new MonthlyPaymentRequest()
			//{
			//    CalculatePayments = paymentParamList
			//};


			return response;
        }
    }
}

