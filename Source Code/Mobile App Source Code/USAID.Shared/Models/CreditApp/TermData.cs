using System;
namespace USAID.Models
{
    public class TermData
    {
        public double Payment { get; set; }

        public double SecurityDeposit { get; set; }

        public int RatecardID { get; set; }

        public int Term { get; set; }

        public string LeaseType { get; set; }

        public string PurchaseOption { get; set; }
    }
}

