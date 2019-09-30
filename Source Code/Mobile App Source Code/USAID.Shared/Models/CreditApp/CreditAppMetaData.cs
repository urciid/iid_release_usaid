using System;
namespace USAID.Models
{
    public class CreditAppMetaData
    {
        public string EnteredBy { get; set; }

        public string ApplicationType { get; set; }

        public string ApplicationFormat { get; set; }

        public double TotalFinancedAmount { get; set; } //Note: the service expects a double for this

        public double MaintenanceFeeAmount { get; set; }

        public string Notes { get; set; }
    }
}

