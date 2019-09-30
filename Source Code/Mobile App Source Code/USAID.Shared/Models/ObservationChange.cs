using System;
using System.Collections.Generic;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using WMP.Core.Data.SQLiteNet;

namespace USAID.Models
{
	public class ObservationChange : IModel
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		[ForeignKey(typeof(Observation))]
		public int LocalObservationID { get; set; }	

		public DateTime Created { get; set; }
		public DateTime Modified { get; set; }
		public bool ModifiedLocally { get; set; }

		public int Observation_id { get; set; }

		public int ChangeId { get; set;}
	
		public string Description { get; set;}
		public DateTime Start_Date { get; set;}

	}
}
