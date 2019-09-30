using System;
using System.Collections.Generic;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using WMP.Core.Data.SQLiteNet;

namespace USAID.Models
{
	public class TermItem : IModel
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

		public string TermDisplay { get; set; }

		public int Term { get; set; }

		public bool IsSelected { get; set;}
	}
}

