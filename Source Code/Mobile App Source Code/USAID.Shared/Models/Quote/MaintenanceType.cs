using System;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using WMP.Core.Data.SQLiteNet;

namespace USAID.Models
{
    public class MaintenanceType : IModel
    {
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		[ForeignKey (typeof (RateCardLocal))]
		public int RateCardId { get; set; }

		[ManyToOne (CascadeOperations = CascadeOperation.CascadeRead)]
		public RateCardLocal RateCard { get; set; }

		[ForeignKey (typeof (Quote))]
		public int QuoteId { get; set; }

		[ManyToOne (CascadeOperations = CascadeOperation.CascadeRead)]
		public Quote Quote { get; set; }


		public DateTime Created { get; set; }

		public DateTime Modified { get; set; }

        public string MaintenanceTypeDescription { get; set; }

        public string MaintenanceTypeValue { get; set; }

		public bool IsSelected { get; set; }
    }
}

