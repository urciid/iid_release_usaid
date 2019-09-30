using System.Collections.Generic;

using IID.BusinessLayer.Domain;
using IID.WebSite.Helpers;

namespace IID.WebSite.Models
{
    public class ChartModel
    {
        public ChartModel() { }

        public ChartModel(ChartParameters parameters, string ReportClassFieldId = "")
        {
            Parameters = parameters;

            using (Entity context = new Entity())
            {
                var indicator = context.t_indicator.Find(parameters.IndicatorId);
                AddMedian = parameters.AddMedian;
                Indicator = new Indicator(indicator, false, true);
                Activity = new Activity(indicator.aim.activity);
                if (ReportClassFieldId == null)
                {
                    ReportClassFieldId = "";
                }
                if (ReportClassFieldId.Length > 0)
                {
                    ChartFilters = new ChartFilters(parameters.IndicatorId, Identity.CurrentUser.Id, IidCulture.CurrentLanguageId, Indicator.ReportClassFieldId);
                }
                else
                {
                    ChartFilters = new ChartFilters(parameters.IndicatorId, Identity.CurrentUser.Id, IidCulture.CurrentLanguageId);
                }


                if (parameters != null && parameters.Criterias != null)
                    ChartResult = ChartResult.GetData(parameters, ReportClassFieldId);
            }
        }

        private ChartParameters Parameters { get; set; }
        public ICollection<ChartCriteria> ChartCriteria { get { return Parameters?.Criterias; } }
        public int ActiveCriteria { get { return Parameters?.ActiveCriteria ?? 0; } }

        public ChartResult ChartResult { get; set; }

        public Activity Activity { get; set; }

        public Indicator Indicator { get; set; }

        public ChartFilters ChartFilters { get; set; }

        public string ChartName { get; set; }

        public bool AddMedian { get; set; }

        public int? RefererSiteId { get; set; }
    }
}
