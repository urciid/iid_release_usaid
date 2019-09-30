using System;
using System.Collections.Generic;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using WMP.Core.Data.SQLiteNet;

namespace USAID.Models
{
	public class ObservationComment : IModel
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		[ForeignKey(typeof(Observation))]
		public int LocalObservationID { get; set; }

		public DateTime Created { get; set; }
		public DateTime Modified { get; set; }
		public bool ModifiedLocally { get; set; }
		public int CommentId { get; set; }

		public int Observation_Id { get; set; }

		public string Comment { get; set; }
		public DateTime Created_Date { get; set; }
	}
}
