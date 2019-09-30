using System;
using System.Collections.Generic;

namespace USAID.Models
{
	public class RateOptions
	{
		public string CompanyName { get; set; }

		public double EquipmentAmount { get; set; }

		public string EquipmentDescription { get; set; }

		public List<RateCardLocal> RateCards  { get; set; }

		public List<TermItem> Terms { get; set; }

		public List<MaintenanceType> MaintenanceTypes { get; set; }

		public List<PurchaseOption> PurchaseOptions { get; set; }

		public List<AdvancePayment> AdvancePayments { get; set; }

		public int Points { get; set; }

		public int PassThrough { get; set;}
	}
}

