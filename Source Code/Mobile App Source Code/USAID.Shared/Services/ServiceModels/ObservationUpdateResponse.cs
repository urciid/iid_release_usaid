using System;

using System;
using System.Collections.Generic;
using System.Net;
using USAID.Models;
using USAID.ServiceModels;

namespace USAID.ServiceModels
{
	public class ObservationUpdateResponse : HttpResponse
	{

		public int observation_id { get; set; }

		public int indicator_id { get; set; }

		public int site_id { get; set; }

		public DateTime begin_date { get; set; }

		public DateTime end_date { get; set; }

		public string user_email { get; set; }
	}
}


