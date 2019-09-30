using System;
namespace USAID.ServiceModels
{
	public class ObservationChangeResponse : HttpResponse
	{
		public int observation_change_id { get; set; }

		public int observation_id { get; set; }

		public string description { get; set; }

		public string user_email { get; set; }
	}
}




