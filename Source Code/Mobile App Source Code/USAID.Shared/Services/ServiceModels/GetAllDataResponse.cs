using System;
using System.Collections.Generic;
using System.Net;
using USAID.Models;
using USAID.ServiceModels;

namespace USAID.ServiceModels
{
    public class GetAllDataResponse : HttpResponse
    {
        public string ErrorMessage { get; set; } //TODO: figure out possible values

		public IEnumerable<Site> Sites { get; set; }
		public IEnumerable<Indicator> Indicators { get; set; }
		public IEnumerable<ActivityModel> Activities { get; set;}
		public IEnumerable<SiteIndicator> SiteIndicators { get; set;}
		public IEnumerable<IndicatorAgePeriod> IndicatorAgePeriods { get; set;}
		public IEnumerable<Observation> Observations { get; set;}
		public IEnumerable<ObservationEntry> ObservationEntries { get; set;}
		public IEnumerable<ObservationChange> ObservationChanges { get; set; }
		public IEnumerable<ObservationComment> ObservationComments { get; set; }
		public IEnumerable<ObservationAttachment> ObservationAttachments { get; set;}

    }
}

