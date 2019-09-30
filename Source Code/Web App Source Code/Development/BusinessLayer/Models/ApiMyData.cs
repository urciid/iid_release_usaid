using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IID.BusinessLayer.Domain;

namespace IID.BusinessLayer.Models
{
    public class ApiMyData
    {
        public IEnumerable<p_api_get_mydata_sites_Result> Sites;

        public IEnumerable<p_api_get_mydata_indicators_new_Result> Indicators;

        public IEnumerable<p_api_get_mydata_activities_Result> Activities;

        public IEnumerable<p_api_get_mydata_site_indicators_Result> SiteIndicators;

        public IEnumerable<p_api_get_mydata_indicator_age_periods_Result> IndicatorAgePeriods;

        public IEnumerable<p_api_get_mydata_observations_Result> Observations;

        public IEnumerable<p_api_get_mydata_observation_entries_Result> ObservationEntries;

        public IEnumerable<p_api_get_mydata_observation_changes_Result> ObservationChanges;

        public IEnumerable<p_api_get_mydata_observation_comments_Result> ObservationComments;

        public IEnumerable<p_api_get_mydata_observation_attachments_Result> ObservationAttachments;
    }
}
