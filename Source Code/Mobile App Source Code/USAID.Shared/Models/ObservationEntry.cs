using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using WMP.Core.Data.SQLiteNet;

namespace USAID.Models
{
	public class ObservationEntry  : IModel
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		[ForeignKey(typeof(Observation))]
		public int LocalObservationID { get; set; }

		public DateTime Created { get; set; }
		public DateTime Modified { get; set; }
		public DateTime begin_date { get; set; }
		public DateTime end_date { get; set;}
		public bool ModifiedLocally { get; set; }
		public string Age_Range { get; set;}
		public int? ObservationEntryId { get; set; }
		public int? Observation_id { get; set; }
		public int? Indicator_Age_Range_Id { get; set;}
		public string Indicator_Gender { get; set;}
		public int? Numerator { get; set; }
		public int? Denominator { get; set; }
		public int? Count { get; set; }
		public double? Rate { get; set; }
		public bool? Yes_No { get; set; }
		public string Title { get
			{
				var ret = Indicator_Gender;
				//if (Indicator_Age_Range_Id != null)
				//{
				//	ret = String.Format("{0} {1}", Indicator_Gender, Age_Range); 
				//}

				return ret;
			}
		}
	}
}
