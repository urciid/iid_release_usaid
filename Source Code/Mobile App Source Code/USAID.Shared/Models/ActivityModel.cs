using System;
using SQLite.Net.Attributes;
using WMP.Core.Data.SQLiteNet;

namespace USAID.Models
{
	public class ActivityModel : IModel
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public DateTime Created { get; set; }
		public DateTime Modified { get; set; }
		public bool ModifiedLocally { get; set; }

		public int ActivityId { get; set; }
		public string Activity { get; set; }
		public string Project { get; set;}
		public string Organization { get; set;}
		public string Country { get; set;}
		public string ActivityManager { get; set; }

	}
}




