using System;
using System.Collections.Generic;
using SQLite.Net.Attributes;
using WMP.Core.Data.SQLiteNet;
using SQLiteNetExtensions.Attributes;

namespace USAID.Models
{
	public class Period : IModel
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public DateTime Created { get; set; }
		public DateTime Modified { get; set; }
		public bool ModifiedLocally { get; set; }
		public int ObservationID { get; set;}

		[ForeignKey(typeof(Observation))]
		public int LocalObservationID { get; set; }


		public int PeriodId { get; set; }
		public DateTime? BeginDate { get; set; }
		public DateTime? EndDate { get; set;}
	}
}
