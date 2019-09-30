using System;
namespace USAID.ServiceModels
{
	public class ObservationCommentRequest
	{
		public int observation_comment_id { get; set; }

		public int observation_id { get; set; }

		public string comment { get; set;}

		public string user_email { get; set; }
	}
}


