using System;
namespace USAID.Models
{
	public class CreditApp
	{
        public bool FromQuoteWorkflow { get; set; }

		public int AppId { get; set; } //used for confirmation screen/email

        public string AppStatus { get; set; } //currently not used in sending credit app

        public DateTime DateSubmitted { get; set; } //currently not used in sending credit app

        public decimal EquipmentCost { get; set; } //currently not used in sending credit app

        // Customer Info
        public string CompanyName { get; set; }

        public string MailingAddress { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }

        public string PhoneNumber { get; set; }

        public string DBA { get; set; }

        public string ContactName { get; set; }

        public string ContactEmail { get; set; }

        public string ContactPhone { get; set; }

        public string PhotoFilePath { get; set; }

        // Contract Terms
        public string EquipmentDescription { get; set; }

        public double TotalAmount { get; set; }

        public double TotalFinancedAmount { get; set; }

        public double MaintenanceFeeAmount { get; set; } //a.k.a. pass through

        public int DesiredFinanceTerm { get; set; }

        public string DesiredPurchaseOption { get; set; }

        public string Comments { get; set; }

        public double Payment { get; set; } //TODO: get from quote workflow

        public int RateCardId { get; set; }

        // Guarantor
        public string GuarantorFirstName { get; set; }

        public string GuarantorMiddleInitial { get; set; }

        public string GuarantorLastName { get; set; }

        public string GuarantorAddress { get; set; }

        public string GuarantorCity { get; set; }

        public string GuarantorState { get; set; }

        public string GuarantorZip { get; set; }
    }
}

