using System;
using System.Collections.Generic;
using SQLite.Net.Attributes;
using WMP.Core.Data.SQLiteNet;

namespace USAID.Models
{
	public class IndicatorAgePeriod : IModel
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public DateTime Created { get; set; }
		public DateTime Modified { get; set; }
		public bool ModifiedLocally { get; set; }

		public int Indicator_Age_Range_ID { get; set;}
		public int Indicator_Id { get; set;}

		public string Age_Range { get; set; }


	}
}
