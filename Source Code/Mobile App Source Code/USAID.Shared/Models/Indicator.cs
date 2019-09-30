using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SQLite.Net.Attributes;
using WMP.Core.Data.SQLiteNet;

namespace USAID.Models
{
	public class Indicator : IModel
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public DateTime Created { get; set; }
		public DateTime Modified { get; set; }
		public bool ModifiedLocally { get; set; }
		public bool Disaggregate_by_Sex { get; set;}
		public string IndicatorId { get; set; }
		public string IndicatorType { get; set; }
		public string Name { get; set; }
		public string Definition { get; set; }
		public string Frequency { get; set; }
		public string Aim { get; set; }
		public string NumeratorName { get; set; }
		public string NumeratorDefinition { get; set; }
		public string DenominatorName { get; set; }
		public string DenominatorDefinition { get; set; }
		public bool DisaggregateBySex { get; set;}
		public bool DisaggregateByAge { get; set; }
	}
}
