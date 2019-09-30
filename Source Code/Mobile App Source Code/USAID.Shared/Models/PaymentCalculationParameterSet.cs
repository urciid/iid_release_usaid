using System;
using System.Collections.Generic;

namespace USAID.Models
{
    public class PaymentCalculationParameterSet
    {
        public int RateCardID { get; set; }

        public double EquipmentCost { get; set; }

        public int NumberOfPoints { get; set; }

        public string MaintenanceType { get; set; }

        public int MaintenanceAmount { get; set; }

        public int IncludeInvalidPayments { get; set; }

        public IEnumerable<AdvancePayment> AdvancePayments { get; set; }

        public IEnumerable<string> PurchaseOptions { get; set; }

        public IEnumerable<int> Terms { get; set; }
    }
}

