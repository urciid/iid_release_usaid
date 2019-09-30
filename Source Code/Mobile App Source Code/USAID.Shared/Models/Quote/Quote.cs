using System;
using System.Collections.Generic;
using SQLite.Net.Attributes;
using WMP.Core.Data.SQLiteNet;
using SQLiteNetExtensions.Attributes;

namespace USAID.Models
{
    public class Quote : IModel
    {
		[PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public string Name { get; set; }

		public string CompanyName { get; set; }

		public double EquipmentAmount { get; set; }

		public string EquipmentDescription { get; set; }

		public int Points { get; set; }

		public int PassThrough { get; set; }

		[OneToMany (CascadeOperations = CascadeOperation.All)]
		public List<RateCardLocal> RateCards { get; set; }

		[OneToMany(CascadeOperations = CascadeOperation.All)]
		public List<TermItem> Terms { get; set; } 

		[OneToMany (CascadeOperations = CascadeOperation.All)]
		public List<MaintenanceType> MaintenanceTypes { get; set; }

		[OneToMany (CascadeOperations = CascadeOperation.All)]
		public List<PurchaseOption> PurchaseOptions { get; set; }

		[OneToMany (CascadeOperations = CascadeOperation.All)]
		public List<MonthlyPayment> MonthlyPayments { get; set; }

		[OneToMany(CascadeOperations = CascadeOperation.All)]
		public List<AdvancePayment> AdvancePayments { get; set; }
    }
}

