using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;

using common = IID.BusinessLayer.Globalization.Common.Resource;
using IID.BusinessLayer.Helpers;

namespace IID.BusinessLayer.Domain
{
    public sealed class RankByImprovementData
    {
        private RankByImprovementData() { }

        public RankByImprovementData(int activityId, int? indicatorId, byte languageId)
        {
            using (Entity context = new Entity())
            {
                context.Database.Initialize(false);

                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.p_get_rank_by_improvement_data";
                cmd.Parameters.Add(new SqlParameter("@activity_id", activityId));
                cmd.Parameters.Add(new SqlParameter("@indicator_id", (object)indicatorId ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@language_id", languageId));

                try
                {
                    context.Database.Connection.Open();
                    var reader = cmd.ExecuteReader();

                    Info = ((IObjectContextAdapter)context).ObjectContext.Translate<RankByImprovementInfo>(reader).First();

                    reader.NextResult();
                    var sites = ((IObjectContextAdapter)context).ObjectContext.Translate<RankByImprovementSite>(reader);
                    Sites = new List<RankByImprovementSite>(sites);

                    reader.NextResult();
                    var observations = ((IObjectContextAdapter)context).ObjectContext.Translate<RankByImprovementObservation>(reader);
                    SiteObservations = observations.GroupBy(g => g.SiteId).ToDictionary(k => k.Key, v => v.OrderBy(o => o.BeginDate).ToArray());
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }

            bool canCalculateImprovement = false;
            switch (Info.IndicatorType)
            {
                case IndicatorType.Percentage:
                    canCalculateImprovement = true;
                    Info.DisplayFormatString = "P0";
                    break;
                case IndicatorType.Average:
                    canCalculateImprovement = true;
                    Info.DisplayFormatString = "N1";
                    break;
                case IndicatorType.Ratio:
                    canCalculateImprovement = true;
                    Info.DisplayFormatString = "0";
                    break;
                case IndicatorType.Count:
                    canCalculateImprovement = true;
                    Info.DisplayFormatString = "N0";
                    break;
            }
            if (canCalculateImprovement)
            {
                var improvementMultiplier = Info.IncreaseIsGood ? 1 : -1;
                foreach (var site in Sites)
                    if (SiteObservations.ContainsKey(site.SiteId))
                        site.CalculateImprovement(SiteObservations[site.SiteId], improvementMultiplier, Info.IndicatorType);

                Sites = Sites.OrderByDescending(o => o.Improvement).ToList();
                for (int i = 0; i < Sites.Count(); i++)
                    Sites[i].Rank = i + 1;
            }
        }

        public RankByImprovementInfo Info { get; private set; }
        public List<RankByImprovementSite> Sites { get; private set; }
        public Dictionary<int, RankByImprovementObservation[]> SiteObservations { get; private set; }
    }

    public sealed class RankByImprovementInfo
    {
        private int activity_id { get; set; }
        public int ActivityId { get { return activity_id; } }

        private string activity_name { get; set; }
        public string ActivityName { get { return activity_name; } }

        private int indicator_id { get; set; }
        public int IndicatorId { get { return indicator_id; } }

        private string indicator_name { get; set; }
        public string IndicatorName { get { return indicator_name; } }

        private string indicator_definition { get; set; }
        public string IndicatorDefinition { get { return indicator_definition; } }

        private string indicator_type { get; set; }
        public IndicatorType IndicatorType { get { return Enumerations.Parse<IndicatorType>(indicator_type); } }

        private bool increase_is_good { get; set; }
        public bool IncreaseIsGood {  get { return increase_is_good; } }

        public string DisplayFormatString { get; set; }
    }

    public sealed class RankByImprovementSite
    {
        #region Database Members

        private int site_id { get; set; }
        public int SiteId {  get { return site_id; } }

        private string site_name { get; set; }
        public string SiteName { get { return site_name; } }

        private string site_type_value { get; set; }
        public string SiteTypeValue { get { return site_type_value; } }

        private string country_name { get; set; }
        public string CountryName { get { return country_name; } }

        private string region_name { get; set; }
        public string RegionName { get { return region_name; } }

        private string coach_user_name { get; set; }
        public string CoachUserName { get { return coach_user_name; } }

        private DateTime? support_start_date { get; set; }
        public DateTime? SupportStartDate { get { return support_start_date; } }

        private DateTime? support_end_date { get; set; }
        public DateTime? SupportEndDate { get { return support_end_date; } }

        #endregion

        #region Trend Members

        public int Rank { get; set; }

        public object StartValue { get; private set; }

        public object LatestValue { get; private set; }

        public double? Improvement { get; private set; }

        public void CalculateImprovement(RankByImprovementObservation[] observations, int improvementMultiplier, IndicatorType indicatorType)
        {
            if (observations != null && observations.Any())
            {
                int observationCount = observations.Length;

                if (indicatorType == IndicatorType.YesNo)
                {
                    int startStreak = 1;
                    bool firstValue = Convert.ToBoolean(observations.First().Value);
                    for (int i = 1; i < observationCount; i++)
                    {
                        if (observations[i].Value.Equals(firstValue))
                            startStreak += 1;
                        else
                            break;
                    }
                    StartValue = String.Format("{0} ({1:N0})", firstValue ? common.Yes : common.No, startStreak);

                    // NOTE: I am assuming it is ok to show the start streak as the most recent streak as well.
                    int endStreak = 1;
                    bool lastValue = Convert.ToBoolean(observations.Last().Value);
                    for (int i = observationCount - 2; i >= 0; i--)
                    {
                        if (observations[i].Value.Equals(lastValue))
                            endStreak += 1;
                        else
                            break;
                    }
                    LatestValue = String.Format("{0} ({1:N0})", lastValue ? common.Yes : common.No, endStreak);
                }
                else
                {
                    StartValue = observations.First().Value;

                    if (observationCount > 1)
                        LatestValue = observations.Last().Value;
                }
            }

            if (StartValue != null && LatestValue != null)
                Improvement = (Convert.ToDouble(LatestValue) - Convert.ToDouble(StartValue)) * improvementMultiplier;
        }

        #endregion
    }

    public sealed class RankByImprovementObservation
    {
        private int site_id { get; set; }
        public int SiteId { get { return site_id; } }

        private DateTime begin_date { get; set; }
        public DateTime BeginDate { get { return begin_date; } }

        private double value { get; set; }
        public double Value { get { return value; } }
    }
}
