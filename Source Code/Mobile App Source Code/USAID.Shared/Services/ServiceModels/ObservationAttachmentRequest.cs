using System;
namespace USAID.ServiceModels
{
	public class ObservationAttachmentRequest
	{
		public int observation_attachment_id { get; set; }

		public int observation_id { get; set; }

		public string attachment_file_name { get; set; }

		public string attachment { get; set;}

		public string user_email { get; set; }

		public System.IO.Stream FileStream { get; set; }
	}
}
