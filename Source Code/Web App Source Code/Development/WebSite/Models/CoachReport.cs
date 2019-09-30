using System;
using System.Collections.Generic;
using System.Linq;

using IID.BusinessLayer.Domain;
using IID.BusinessLayer.Helpers;
using IID.WebSite.Helpers;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace IID.WebSite.Models
{
    public class CoachReport
    {
        public CoachReport(int activityId, int siteId)
        {
            using (Entity context = new Entity())
            {
                var site = context.t_site.Find(siteId);
                if (site == null)
                    throw new ArgumentException("Invalid siteId: " + siteId.ToString());

                var activity = context.t_activity.Find(activityId);
                if (activity == null)
                    throw new ArgumentException("Invalid activityId: " + activityId.ToString());

                ActivityId = activityId;
                ActivityName = activity.name;

                SiteId = siteId;
                SiteName = site.name;
                var adm = site.administrative_division;
                if (adm.parent_administrative_division_id.HasValue)
                {
                    do
                    {
                        SiteName += (", " + adm.name);
                        adm = adm.parent_division;
                    } while (adm.parent_administrative_division_id.HasValue);
                }
                else
                {
                    SiteName += (", " + adm.name);
                }
              
                SiteName += (", " + adm.country.name);

                // NOTE: Don't use UserAssignedObjects here. Only show active aims/indicators.
                var indicators =
                    site.activities.Where(e=>e.activity_id == activityId)
                        .SelectMany(e => e.activity.aims.Where(f => f.active == true).OrderBy(g => g.sort))
                        .SelectMany(e => e.indicators.Where(f => f.active == true).OrderBy(g => g.sort))
                        .Where(e=> Enumerations.Parse<IndicatorType>(e.indicator_type.value) != IndicatorType.YesNo);
                Indicators = indicators.Select(e => new CoachReportIndicator(siteId, e)).ToList();
                Changes =
                    site.observations.Where(e => indicators.Contains(e.indicator))
                    .SelectMany(e => e.changes.Where(f => f.approved))
                    .OrderBy(e => e.start_date)
                    .Select(g => new CoachReportChange(g)).ToList();
            }
        }

        public int ActivityId { get; set; }
        public string ActivityName { get; set; }

        public int SiteId { get; set; }
        public string SiteName { get; set; }

        public ICollection<CoachReportIndicator> Indicators { get; set; }
        public ICollection<CoachReportChange> Changes { get; set; }
    }

    public class CoachReportIndicator
    {
        public CoachReportIndicator(int siteId, t_indicator indicator)
        {
            byte languageId = IidCulture.CurrentLanguageId;

            SiteId = siteId;
            AimId = indicator.aim_id;
            AimName = indicator.aim.name;
            IndicatorId = indicator.indicator_id;
            IndicatorName = indicator.name;
            IndicatorDefinition = indicator.definition;
            IndicatorType = Enumerations.Parse<IndicatorType>(indicator.indicator_type.value);
            DataCollectionFrequencyId = indicator.data_collection_frequency_fieldid;
            Observations = new List<CoachReportObservation>();

       
            if (IndicatorType == IndicatorType.Ratio)
                RatioValue = indicator.rate_per.value;

            var observations = indicator.observations.Where(e => e.site_id == siteId).OrderBy(e => e.begin_date);

            foreach (var observation in observations)
            {
                foreach (var entry in observation.entries.Where(e => e.indicator_age_range_id == null && e.indicator_gender == null))
                {
                    if (Enumerations.Parse<DataCollectionFrequency>(indicator.data_collection_frequency.value) == DataCollectionFrequency.Daily)
                    {
                        //observation.begin_date = Date.StartOfWeek(observation.begin_date);

                        int diff = (7 + (observation.begin_date.DayOfWeek - DayOfWeek.Sunday)) % 7;
                        observation.begin_date = observation.begin_date.AddDays(-1 * diff).Date;

                    }

                    var o = new CoachReportObservation(observation.begin_date, entry.indicator_numeric_value);

                    var changes =
                        observation.changes
                            .Where(e => e.approved)
                            .OrderBy(e => e.start_date)
                            .ThenBy(e => e.observation_change_id);
                    if (changes != null && changes.Any())
                    {
                        o.FirstChangeId = changes.First().observation_change_id;
                        o.ChangeDescriptions = String.Join("<br />", changes.Select(e => e.description));
                    }

                    Observations.Add(o);
                }
            }

            if (Enumerations.Parse<DataCollectionFrequency>(indicator.data_collection_frequency.value) == DataCollectionFrequency.Daily)
            {
                List<CoachReportObservation> ObservationsGrouped;
                ObservationsGrouped = new List<CoachReportObservation>();
                ObservationsGrouped = Observations.GroupBy(e => e.Date).Select(n => new CoachReportObservation(n.Key, null)
                {
                    Date = n.Key,
                    Value = n.Average(v => v.Value),
                    FirstChangeId = n.Min(f => f.FirstChangeId),
                    ChangeDescriptions = n.Min(c => c.ChangeDescriptions)
                }).ToList();

                Observations = ObservationsGrouped;

            }

                if (Observations.Any() && Observations.Count > 1)
            {
                // Fill in gaps. Wish we would have done this via stored proc instead of EF...
                DateTime firstDate = Observations.Min(o => o.Date);
                DateTime lastDate = Observations.Max(o => o.Date);
                Func<DateTime, DateTime> addInterval = null;
                switch (Enumerations.Parse<DataCollectionFrequency>(indicator.data_collection_frequency.value))
                {
                    case DataCollectionFrequency.Daily:
                        //addInterval = new Func<DateTime, DateTime>(x => x.AddDays(1));
                        addInterval = new Func<DateTime, DateTime>(x => x.AddDays(7));
                        break;

                    case DataCollectionFrequency.Weekly:
                        addInterval = new Func<DateTime, DateTime>(x => x.AddDays(7));
                        break;

                    case DataCollectionFrequency.BiWeekly:
                        addInterval = new Func<DateTime, DateTime>(x => x.AddDays(14));
                        break;

                    case DataCollectionFrequency.Monthly:
                        addInterval = new Func<DateTime, DateTime>(x => x.AddMonths(1));
                        break;

                    case DataCollectionFrequency.Quarterly:
                        addInterval = new Func<DateTime, DateTime>(x => x.AddMonths(3));
                        break;
                }
                var theDate = addInterval(firstDate);
                do
                {
                    // Daily gets summed as weekly
                    //if (Enumerations.Parse<DataCollectionFrequency>(indicator.data_collection_frequency.value)== DataCollectionFrequency.Daily)
                    //{
                    //    if (!Observations.Any(o => o.Date == theDate))
                    //        Observations.Add(new CoachReportObservation(theDate, null));
                    //    theDate = addInterval(theDate);

                    //}
                    //else 
                    if (!Observations.Any(o => o.Date == theDate))
                        Observations.Add(new CoachReportObservation(theDate, null));
                    theDate = addInterval(theDate);
                }
                while (theDate < lastDate);

                // Re-order by date.
                Observations = Observations.OrderBy(o => o.Date).ToList();
            }
        }

        public int SiteId { get; set; }
        public int AimId { get; set; }
        public string AimName { get; set; }
        public int IndicatorId { get; set; }
        public string IndicatorName { get; set; }
        public string IndicatorDefinition { get; set; }
        public IndicatorType IndicatorType { get; set; }
        public string DataCollectionFrequencyId { get; set; }
        public string RatioValue { get; set; }
        public ICollection<CoachReportObservation> Observations { get; set; }
    }

    public class CoachReportObservation
    {
        public CoachReportObservation(DateTime date, double? value)
        {
            Date = date;
            Value = value;
        }

        public DateTime Date { get; set; }
        public double? Value { get; set; }
        public int? FirstChangeId { get; set; }
        public string ChangeDescriptions { get; set; }
    }

    public class CoachReportChange
    {
        public CoachReportChange(t_observation_change change)
        {
            Date = change.start_date;
            IndicatorId = change.observation.indicator_id;
            IndicatorName = change.observation.indicator.name;
            Change = change.description;
            CreatedBy = change.createdby.full_name;
        }

        public DateTime Date { get; set; }
        public int IndicatorId { get; set; }
        public string IndicatorName { get; set; }
        public string Change { get; set; }
        public string CreatedBy { get; set; }
    }
}