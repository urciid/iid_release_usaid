using System;
namespace USAID.Models
{
    public class ContractTerms
    {
        public string EquipmentDescription { get; set; }

        public string TotalAmount { get; set; }

        public string TotalFinancedAmount { get; set; }

        public string MaintenanceFeeAmount { get; set; } //this is the same as pass through

        public int DesiredFinanceTerm { get; set; }

        public string DesiredPurchaseOption { get; set; }

        public string Comments { get; set; }
    }
}

