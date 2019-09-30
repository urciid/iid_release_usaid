using System;
namespace USAID.Models
{
    public class DealerDefaults
    {
        //TODO: confirm data types
        public decimal DollarLeasePurchase { get; set; }

        public string Account { get; set; }

        public string EmailCc { get; set; }

        public int ExpirationDays { get; set; }

        public int FinanceTermOnCreditApp { get; set; }

        public decimal MoneyDown { get; set; }

        public string Phone { get; set; }

        public string PurchaseOptionOnCreditApp { get; set; }

        public bool ShowInfoZoneLink { get; set; }

        public bool ShowInfoZoneLogo { get; set; }

        public bool ShowPassThruOnCreditApp { get; set; }

        public bool ShowPassThruOnQuote { get; set; }

        public bool ShowPersonalGuarantor { get; set; }

        public bool ShowPoints { get; set; }

        public string TeamEmail { get; set; }
    }
}

