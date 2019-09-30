using System;
using System.Collections.Generic;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using WMP.Core.Data.SQLiteNet;

namespace USAID.Models
{
    public class RateCard
    {

		public List<AdvancePayment> AdvancePayments { get; set; }

        public double AvailablePoints { get; set; }

        public int CustomerID { get; set; }

        public string EquipmentType { get; set; }

        public string EquipmentTypeDescription { get; set; }

        public List<MaintenanceType> MaintenanceTypes { get; set; }

        public double MaximumAmount { get; set; }

        public double MinimumAmount { get; set; }

		public List<string> PurchaseOptions { get; set; }

        public int RateCardID { get; set; }

        public string RateCardName { get; set; }

		public IEnumerable<int> Terms { get; set; }

		public bool IsSelected { get; set;}

    }
}

