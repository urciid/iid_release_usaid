using System;
using System.Collections.Generic;
using System.Net;
using USAID.Models;
using USAID.ServiceModels;

namespace USAID.ServiceModels
{
	public class ObservationSaveResponse : HttpResponse
	{
		public Observation observation { get; set;}
	}
}



