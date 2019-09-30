using System;
using SQLite.Net.Attributes;
using WMP.Core.Data.SQLiteNet;

namespace USAID.Models
{
	public class SiteIndicator : IModel
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public DateTime Created { get; set; }
		public DateTime Modified { get; set; }
		public bool ModifiedLocally { get; set; }

		public int SiteId { get; set; }
		public int ActivityId { get; set; }
		public int IndicatorId { get; set; }

		public DateTime StartDate { get; set; }

	}
}




