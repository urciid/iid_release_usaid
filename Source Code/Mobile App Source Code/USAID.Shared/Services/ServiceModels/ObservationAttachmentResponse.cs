using System;
using System.Collections.Generic;
using System.Net;
using USAID.Models;
using USAID.ServiceModels;

namespace USAID.ServiceModels
{
	public class ObservationAttachmentResponse : HttpResponse
	{

		public int observation_attachment_id { get; set; }

		public int observation_id { get; set; }

		public string attachment_file_name { get; set; }

		public string attachment { get; set; }

		public string user_email { get; set; }

		public System.IO.Stream FileStream { get; set; }

	}
}


