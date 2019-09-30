using System;
using System.Collections.Generic;
using System.Linq;

using IID.BusinessLayer.Domain;
using IID.BusinessLayer.Helpers;
using IID.WebSite.Helpers;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace IID.WebSite.Models
{
    public class CountryDirectorReport
    {
        public CountryDirectorReport(int countryId)
        {

            using (Entity context = new Entity())
            {
                var country = context.t_country.Find(countryId);

                CountryName = country.name;
                //var site = context.t_site.Find(siteId);
                //if (site == null)
                //    throw new ArgumentException("Invalid siteId: " + siteId.ToString());

                //var activity = context.t_activity.Find(activityId);
                //if (activity == null)
                //    throw new ArgumentException("Invalid activityId: " + activityId.ToString());

                //ActivityId = activityId;
                //ActivityName = activity.name;

                //SiteId = siteId;
                //SiteName = site.name;
                //var adm = site.administrative_division;
                //do
                //{
                //    SiteName += (", " + adm.name);
                //    adm = adm.parent_division;
                //} while (adm.parent_administrative_division_id.HasValue);
                //SiteName += (", " + adm.country.name);

                // NOTE: Don't use UserAssignedObjects here. Only show active aims/indicators.
                var indicators =
                    country.activities.Where(e => e.country_id == countryId)
                        .SelectMany(e => e.aims.Where(f => f.active == true).OrderBy(g => g.sort))
                        .SelectMany(e => e.indicators.Where(f => f.active == true).OrderBy(g => g.sort))
                        .Where(e => Enumerations.Parse<IndicatorType>(e.indicator_type.value) != IndicatorType.YesNo);
                Indicators = indicators.Select(e => new CountryDirectorReportIndicator(countryId, e)).ToList();
                //Changes =
                //    site.observations.Where(e => indicators.Contains(e.indicator))
                //    .SelectMany(e => e.changes.Where(f => f.approved))
                //    .OrderBy(e => e.start_date)
                //    .Select(g => new CountryDirectorReportChange(g)).ToList();
            }
        }
        
        public string CountryName { get; set; }
        

        public ICollection<CountryDirectorReportIndicator> Indicators { get; set; }
        //public ICollection<CountryDirectorReportChange> Changes { get; set; }
    }

    public class CountryDirectorReportIndicator
    {
        public CountryDirectorReportIndicator(int countryId, t_indicator indicator)
        {
            byte languageId = IidCulture.CurrentLanguageId;
            
            //SiteId = siteId;
            AimId = indicator.aim_id;
            AimName = indicator.aim.name;
            ActivityName = indicator.aim.activity.name;
            CountryName = indicator.aim.activity.country.name;
            IndicatorId = indicator.indicator_id;
            IndicatorName = indicator.name;
            IndicatorDefinition = indicator.definition;
            IndicatorType = Enumerations.Parse<IndicatorType>(indicator.indicator_type.value);
            DataCollectionFrequencyId = indicator.data_collection_frequency_fieldid;
            Observations = new List<CountryDirectorReportObservation>();

            if (IndicatorType == IndicatorType.Ratio)
                RatioValue = indicator.rate_per.value;

            //var results = (from i in indicator.observations
            //               select new CountryDirectorReportObservation()
            //               {
            //                   Date = i.begin_date,
            //                   Value = i.entries.Average(k => k.indicator_numeric_value)

            //               })
            //              .OrderBy(o => o.Date)
            //              .ToList();

            //if (Enumerations.Parse<DataCollectionFrequency>(indicator.data_collection_frequency.value) == DataCollectionFrequency.Daily)
            //{
            //    foreach (CountryDirectorReportObservation o in results)
            //    {
            //        int diff = (7 + (o.Date.DayOfWeek - DayOfWeek.Sunday)) % 7;
            //        o.Date = o.Date.AddDays(-1 * diff).Date;
            //    }
            //}

            //Observations = results.GroupBy(item => item.Date)
            //        .Select(g => new CountryDirectorReportObservation()
            //        {
            //            Date = g.Key,
            //            Value = g.Average(s => s.Value)
            //        })
            //        .OrderBy(o => o.Date)
            //        .ToList();

            using (Entity context = new Entity())
            {
                //CountryStats = new List<CountryDashboardDetails>();

                context.Database.Initialize(false);

                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.p_get_country_director_report";
                cmd.Parameters.Add(new SqlParameter("@user_id", 0));
                cmd.Parameters.Add(new SqlParameter("@indicator_id", IndicatorId));

                try
                {
                    context.Database.Connection.Open();
                    var reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            CountryDirectorReportObservation item = new CountryDirectorReportObservation();
                            var test = reader[1].ToString();
                            item.Date = (DateTime)reader[0];
                            if (reader[1] != DBNull.Value)
                                item.Value = (Double)reader.GetDecimal(1);
                            Observations.Add(item);
                        }
                    }
                }

                finally
                {
                    context.Database.Connection.Close();
                }
            }



        }

        public int SiteId { get; set; }
        public int AimId { get; set; }
        public string ActivityName { get; set; }
        public string CountryName { get; set; }
        public string AimName { get; set; }
        public int IndicatorId { get; set; }
        public string IndicatorName { get; set; }
        public string IndicatorDefinition { get; set; }
        public IndicatorType IndicatorType { get; set; }
        public string DataCollectionFrequencyId { get; set; }
        public string RatioValue { get; set; }
        public ICollection<CountryDirectorReportObservation> Observations { get; set; }
    }

    public class CountryAllIndicatorObservations
    {
        public DateTime Date { get; set; }
        public double? Value { get; set; }
    }


    public class CountryDirectorReportObservation
    {      
        public DateTime Date { get; set; } 
        public double? Value { get; set; }
        public int? FirstChangeId { get; set; }
        public string ChangeDescriptions { get; set; }
    }

    //public class CountryDirectorReportChange
    //{
    //    public CountryDirectorReportChange(t_observation_change change)
    //    {
    //        Date = change.start_date;
    //        IndicatorId = change.observation.indicator_id;
    //        IndicatorName = change.observation.indicator.name;
    //        Change = change.description;
    //        CreatedBy = change.createdby.full_name;
    //    }

    //    public DateTime Date { get; set; }
    //    public int IndicatorId { get; set; }
    //    public string IndicatorName { get; set; }
    //    public string Change { get; set; }
    //    public string CreatedBy { get; set; }
    //}
}