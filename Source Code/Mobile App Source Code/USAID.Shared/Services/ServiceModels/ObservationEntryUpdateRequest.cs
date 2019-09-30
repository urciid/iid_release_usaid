using System;
using USAID.Models;

namespace USAID.ServiceModels
{
	public class ObservationEntryUpdateRequest
	{
		public int? observation_entry_id { get; set; }
		public int? observation_id { get; set; }
		public int? indicator_age_range_id { get; set; }
		public string indicator_gender { get; set; }
		public int? numerator { get; set; }
		public int? denominator { get; set; }
		public int? count { get; set; }
		public double? rate { get; set; }
		public string user_email { get; set; }
		public bool yes_no { get; set;}
	}
}
