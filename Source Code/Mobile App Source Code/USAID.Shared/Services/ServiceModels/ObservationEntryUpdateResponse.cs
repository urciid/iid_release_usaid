using System;
using System.Collections.Generic;
using System.Net;
using USAID.Models;
using USAID.ServiceModels;

namespace USAID.ServiceModels
{
	public class ObservationEntryUpdateResponse : HttpResponse
	{

		public int? observation_entry_id { get; set; }
		public int? Observation_id { get; set; }
		public int? Indicator_Age_Range_Id { get; set; }
		public string Indicator_Gender { get; set; }
		public int? Numerator { get; set; }
		public int? Denominator { get; set; }
		public int? Count { get; set; }
		public double? Rate { get; set; }
		public bool? YesNo { get; set; }
	}
}



