using System;
using System.Collections.Generic;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using WMP.Core.Data.SQLiteNet;

namespace USAID.Models
{
	public class RateCardLocal : IModel
	{

		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		[ForeignKey (typeof (Quote))]
		public int QuoteId { get; set; }

		[ManyToOne (CascadeOperations = CascadeOperation.CascadeRead)]
		public Quote Quote { get; set; }

		public DateTime Created { get; set; }

		public DateTime Modified { get; set; }

		[OneToMany (CascadeOperations = CascadeOperation.All)]
		public List<AdvancePayment> AdvancePayments { get; set; }

		public double AvailablePoints { get; set; }

		public int CustomerID { get; set; }

		public string EquipmentType { get; set; }

		public string EquipmentTypeDescription { get; set; }

		[OneToMany (CascadeOperations = CascadeOperation.All)]
		public List<MaintenanceType> MaintenanceTypes { get; set; }

		public double MaximumAmount { get; set; }

		public double MinimumAmount { get; set; }

		[OneToMany (CascadeOperations = CascadeOperation.All)]
		public List<PurchaseOption> PurchaseOptions { get; set; }

		public int RateCardID { get; set; }

		public string RateCardName { get; set; }

		[OneToMany (CascadeOperations = CascadeOperation.All)]
		public List<TermItem> Terms { get; set; }

		public bool IsSelected { get; set; }

	}
}

