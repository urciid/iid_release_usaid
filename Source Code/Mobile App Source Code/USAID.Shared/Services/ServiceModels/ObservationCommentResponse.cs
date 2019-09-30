using System;
using System.Collections.Generic;
using System.Net;
using USAID.Models;
using USAID.ServiceModels;

namespace USAID.ServiceModels
{
	public class ObservationCommentResponse : HttpResponse
	{

		public int observation_id { get; set; }

		public int observation_comment_id { get; set; }

		public string comment { get; set;}

		public string user_email { get; set; }
	}
}


