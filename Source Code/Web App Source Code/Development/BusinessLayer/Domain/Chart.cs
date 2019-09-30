using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

using IID.BusinessLayer.Models;
using IID.BusinessLayer.Helpers;
using chart = IID.BusinessLayer.Globalization.Chart.Resource;

namespace IID.BusinessLayer.Domain
{
    public class ChartCriteria
    {
        public ChartCriteria() { }

        public int CountryId { get; set; }
        public string CountryIds { get; set; }
        public string ReportClassFieldId { get; set; }
        public int? AdministrativeDivisionId1 { get; set; }
        public int? AdministrativeDivisionId2 { get; set; }
        public int? AdministrativeDivisionId3 { get; set; }
        public int? AdministrativeDivisionId4 { get; set; }
        public string PopulationDensityFieldId { get; set; }
        public string SiteTypeFieldIds { get; set; }
        public string WaveFieldId { get; set; }
        public string SiteIds { get; set; }
        public string ChangeIds { get; set; }
        public int? AgeRangeId { get; set; }
        private string _sexCode;
        public string SexCode
        {
            get { return _sexCode; }
            set { _sexCode = value == String.Empty ? null : value; }
        }

        public DateTime? BeginDate { get; set; }
        public string BeginDateString { get { return BeginDate.HasValue ? BeginDate.Value.ToString("yyyy-MM-dd") : null; } }

        public DateTime? EndDate { get; set; }
        public string EndDateString { get { return EndDate.HasValue ? EndDate.Value.ToString("yyyy-MM-dd") : null; } }

        public int ColorId { get; set; }
    }

    public sealed class ChartResult
    {
        private ChartResult() { }

        public static ChartResult GetData(ChartParameters request, string ReportClassFieldID = "")
        {
            ChartResult result = new ChartResult();

            string strCriteria;
            XmlSerializer serializer = new XmlSerializer(request.Criterias.GetType());
            using (StringWriter sw = new StringWriter())
            {
                using (var xw = XmlWriter.Create(sw, new XmlWriterSettings() { OmitXmlDeclaration = true }))
                {
                    serializer.Serialize(xw, request.Criterias);
                    strCriteria = sw.ToString();
                }
            }

            result.Series = new List<ChartSeries>();
            result.Changes = new List<ChartChange>();

            using (Entity db = new Entity())
            {
                db.Database.Initialize(false);

                var cmd = db.Database.Connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.p_get_chart_data";
                cmd.Parameters.Add(new SqlParameter("@indicator_id", request.IndicatorId));
                cmd.Parameters.Add(new SqlParameter("@criteria_xml", strCriteria));
                cmd.Parameters.Add(new SqlParameter("@report_class_fieldid", ReportClassFieldID));

                try
                {
                    db.Database.Connection.Open();
                    var reader = cmd.ExecuteReader();

                    var dbSeries = ((IObjectContextAdapter)db).ObjectContext.Translate<ChartSeries>(reader);
                    foreach (var item in dbSeries)
                        result.Series.Add(item);

                    reader.NextResult();
                    var dbResults = ((IObjectContextAdapter)db).ObjectContext.Translate<TimeSeriesPoint>(reader).ToArray();
                    foreach (var s in result.Series)
                        s.Points = dbResults.Where(x => x.SeriesId == s.SeriesId).ToArray();

                    reader.NextResult();
                    var dbChanges = ((IObjectContextAdapter)db).ObjectContext.Translate<ChartChange>(reader).ToArray();
                    foreach (var c in dbChanges)
                        result.Changes.Add(c);
                }
                finally
                {
                    db.Database.Connection.Close();
                }
            }

            for (int i = 0; i < result.Series.Count; i++)
                result.Series.ElementAt(i).Color = new Color(request.Criterias[i].ColorId);

            if (request.AddMedian && result.Series.Count == 1)
                result.RulesResult = new ChartRulesResult<DateTime>(
                    (ICollection<IPoint<DateTime>>)result.Series.ElementAt(0).Points);

            return result;
        }

        public ICollection<ChartSeries> Series { get; private set; }

        public ICollection<ChartChange> Changes { get; private set; }

        public ChartRulesResult<DateTime> RulesResult { get; private set; }
    }

    public sealed class ChartSeries
    {
        private int series_id { get; set; }
        public int SeriesId { get { return series_id; } }

        private string series_name { get; set; }
        public string SeriesName { get { return series_name; } }

        private string site_names { get; set; }
        public string SiteNames { get { return site_names; } }

        private string site_ids { get; set; }
        public string SiteIds { get { return site_ids; } }

        private int? site_id { get; set; }
        public int? SiteId { get { return site_id; } }

        private int? age_range_id { get; set; }
        public int? AgeRangeId { get { return age_range_id; } }

        private string sex_code { get; set; }
        public string SexCode { get { return sex_code; } }

        private DateTime? begin_date { get; set; }
        public DateTime? BeginDate { get { return begin_date; } }

        private DateTime? end_date { get; set; }
        public DateTime? EndDate { get { return end_date; } }

        private string administrative_division_name { get; set; }
        public string AdministrativeDivisionName { get { return administrative_division_name; } }

        public ICollection<TimeSeriesPoint> Points { get; set; }

        public Color Color { get; set; }
    }

    #region Points

    public interface IPoint<T>
    {
        T XValue { get; }
        decimal? YValue { get; }
    }

    public sealed class TimeSeriesPoint : IPoint<DateTime>
    {
        private int series_id { get; set; }
        public int SeriesId { get { return series_id; } }

        private DateTime begin_date { get; set; }
        public DateTime XValue { get { return begin_date; } }

        private string date_label { get; set; }
        public string Label
        {
            get { return date_label; }
            private set { date_label = value; }
        }

        private decimal? numerator { get; set; }

        private int? denominator { get; set; }
        public int? Denominator { get { return denominator; } }

        private decimal? value { get; set; }
        public decimal? YValue { get { return value; } }

        private bool has_changes { get; set; }
        public bool HasChanges { get { return has_changes; } }
    }

    public class MedianPoint<T> : IPoint<T>
    {
        public MedianPoint(T xValue, decimal? yValue)
        {
            XValue = xValue;
            YValue = yValue;
        }

        public T XValue { get; private set; }
        public decimal? YValue { get; private set; }
    }

    #endregion

    public sealed class ChartChange
    {
        private int series_id { get; set; }
        public int SeriesId { get { return series_id; } }

        private DateTime begin_date { get; set; }
        public DateTime BeginDate { get { return begin_date; } }

        private int observation_change_id { get; set; }
        public int ObservationChangeId { get { return observation_change_id; } }

        private DateTime start_date { get; set; }
        public DateTime StartDate {  get { return start_date; } }

        private string description { get; set; }
        public string Description {  get { return description; } }
    }

    public sealed class ChartFilters
    {
        private ChartFilters() { }

        public ChartFilters(int indicatorId, int userId, int languageId, string ReportClassFieldId = "")
        {
            using (Entity db = new Entity())
            {
                db.Database.Initialize(false);

                var cmd = db.Database.Connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.p_get_chart_filters";
                cmd.Parameters.Add(new SqlParameter("@indicator_id", indicatorId));
                cmd.Parameters.Add(new SqlParameter("@user_id", userId));
                cmd.Parameters.Add(new SqlParameter("@language_id", languageId));
                cmd.Parameters.Add(new SqlParameter("@report_class_fieldid", ReportClassFieldId));

                try
                {
                    db.Database.Connection.Open();
                    var reader = cmd.ExecuteReader();

                    Countries = new List<Country>();
                    var countries = ((IObjectContextAdapter)db).ObjectContext.Translate<Country>(reader);
                    foreach (var item in countries)
                        Countries.Add(item);

                    AdministrativeDivisionTypes = new List<AdministrativeDivisionType>();
                    reader.NextResult();
                    var administrativeDivisionTypes = ((IObjectContextAdapter)db).ObjectContext.Translate<AdministrativeDivisionType>(reader);
                    foreach (var item in administrativeDivisionTypes)
                        AdministrativeDivisionTypes.Add(item);

                    AdministrativeDivisions = new List<AdministrativeDivision>();
                    reader.NextResult();
                    var administrativeDivisions = ((IObjectContextAdapter)db).ObjectContext.Translate<AdministrativeDivision>(reader);
                    foreach (var item in administrativeDivisions)
                        AdministrativeDivisions.Add(item);

                    Waves = new List<Wave>();
                    reader.NextResult();
                    var waves = ((IObjectContextAdapter)db).ObjectContext.Translate<Wave>(reader);
                    foreach (var item in waves)
                        Waves.Add(item);

                    SiteTypes = new List<SiteType>();
                    reader.NextResult();
                    var siteTypes = ((IObjectContextAdapter)db).ObjectContext.Translate<SiteType>(reader);
                    foreach (var item in siteTypes)
                        SiteTypes.Add(item);

                    PopulationDensities = new List<PopulationDensity>();
                    reader.NextResult();
                    var populationDensities = ((IObjectContextAdapter)db).ObjectContext.Translate<PopulationDensity>(reader);
                    foreach (var item in populationDensities)
                        PopulationDensities.Add(item);

                    Sites = new List<Site>();
                    reader.NextResult();
                    var sites = ((IObjectContextAdapter)db).ObjectContext.Translate<Site>(reader);
                    foreach (var item in sites)
                        Sites.Add(item);

                    Changes = new List<Change>();
                    reader.NextResult();
                    var changes = ((IObjectContextAdapter)db).ObjectContext.Translate<Change>(reader);
                    foreach (var item in changes)
                        Changes.Add(item);

                    AgeRanges = new List<AgeRange>();
                    reader.NextResult();
                    var ageRanges = ((IObjectContextAdapter)db).ObjectContext.Translate<AgeRange>(reader);
                    foreach (var item in ageRanges)
                        AgeRanges.Add(item);

                    Sexes = new List<Sex>();
                    reader.NextResult();
                    var sexes = ((IObjectContextAdapter)db).ObjectContext.Translate<Sex>(reader);
                    foreach (var item in sexes)
                        Sexes.Add(item);

                    Colors = new List<Color>();
                    reader.NextResult();
                    var colors = ((IObjectContextAdapter)db).ObjectContext.Translate<Color>(reader);
                    foreach (var item in colors)
                        Colors.Add(item);

                    ReportClasses = new List<ReportClass>();
                    reader.NextResult();
                    var reportclasses = ((IObjectContextAdapter)db).ObjectContext.Translate<ReportClass>(reader);
                    foreach (var item in reportclasses)
                        ReportClasses.Add(item);
                }
                finally
                {
                    db.Database.Connection.Close();
                }
            }
        }

        public ICollection<Country> Countries;
        public ICollection<AdministrativeDivisionType> AdministrativeDivisionTypes;
        public ICollection<AdministrativeDivision> AdministrativeDivisions;
        public ICollection<SiteType> SiteTypes;
        public ICollection<Wave> Waves;
        public ICollection<PopulationDensity> PopulationDensities;
        public ICollection<Site> Sites;
        public ICollection<Change> Changes;
        public ICollection<AgeRange> AgeRanges;
        public ICollection<Sex> Sexes;
        public ICollection<Color> Colors;
        public ICollection<ReportClass> ReportClasses;

        public sealed class Country
        {
            private int country_id { get; set; }
            public int CountryId { get { return country_id; } }

            private string country_name { get; set; }
            public string CountryName { get { return country_name; } }
        }

        public sealed class AdministrativeDivisionType
        {
            private int country_id { get; set; }
            public int CountryId { get { return country_id; } }

            private string administrative_division_1_type { get; set; }
            public string AdministrativeDivisionType1 { get { return administrative_division_1_type; } }

            private string administrative_division_2_type { get; set; }
            public string AdministrativeDivisionType2 { get { return administrative_division_2_type; } }

            private string administrative_division_3_type { get; set; }
            public string AdministrativeDivisionType3 { get { return administrative_division_3_type; } }

            private string administrative_division_4_type { get; set; }
            public string AdministrativeDivisionType4 { get { return administrative_division_4_type; } }
        }

        public sealed class AdministrativeDivision
        {
            private int country_id { get; set; }
            public int CountryId { get { return country_id; } }

            private int administrative_division_id { get; set; }
            public int AdministrativeDivisionId { get { return administrative_division_id; } }

            private string administrative_division_name { get; set; }
            public string AdministrativeDivisionName { get { return administrative_division_name; } }

            private int? administrative_division_parent_id { get; set; }
            public int? AdministrativeDivisionParentId { get { return administrative_division_parent_id; } }
        }

        public sealed class Wave
        {
            private string wave_fieldid { get; set; }
            public string FieldId { get { return wave_fieldid; } }

            private string wave_value { get; set; }
            public string Value { get { return wave_value; } }
        }

        public sealed class SiteType
        {
            private int country_id { get; set; }
            public int CountryId { get { return country_id; } }

            private string site_Type_fieldid { get; set; }
            public string FieldId { get { return site_Type_fieldid; } }

            private string site_type_value { get; set; }
            public string Value { get { return site_type_value; } }
        }

        public sealed class PopulationDensity
        {
            private string rural_urban_fieldid { get; set; }
            public string FieldId { get { return rural_urban_fieldid; } }

            private string rural_urban_value { get; set; }
            public string Value { get { return rural_urban_value; } }
        }

        public sealed class Site
        {
            private int country_id { get; set; }
            public int CountryId { get { return country_id; } }

            private int site_id { get; set; }
            public int SiteId { get { return site_id; } }

            private string site_name { get; set; }
            public string SiteName { get { return site_name; } }

            private int administrative_division_1_id { get; set; }
            public int AdministrativeDivisionId1 { get { return administrative_division_1_id; } }

            private int? administrative_division_2_id { get; set; }
            public int? AdministrativeDivisionId2 { get { return administrative_division_2_id; } }

            private int? administrative_division_3_id { get; set; }
            public int? AdministrativeDivisionId3 { get { return administrative_division_3_id; } }

            private int? administrative_division_4_id { get; set; }
            public int? AdministrativeDivisionId4 { get { return administrative_division_4_id; } }

            private string wave_fieldid { get; set; }
            public string Wave { get { return wave_fieldid; } }

            private string site_Type_fieldid { get; set; }
            public string SiteType { get { return site_Type_fieldid; } }

            private string rural_urban_fieldid { get; set; }
            public string PopulationDensity { get { return rural_urban_fieldid; } }
        }

        public sealed class Change
        {
            private int observation_change_id { get; set; }
            public int ObservationChangeId { get { return observation_change_id; } }

            private string description { get; set; }
            public string Description { get { return description; } }

            private int country_id { get; set; }
            public int CountryId { get { return country_id; } }

            private int site_id { get; set; }
            public int SiteId { get { return site_id; } }
        }

        public sealed class AgeRange
        {
            private int indicator_age_range_id { get; set; }
            public int Id { get { return indicator_age_range_id; } }

            private string age_range { get; set; }
            public string Name { get { return age_range; } }
        }

        public sealed class Sex
        {
            private string sex_code { get; set; }
            public string Code { get { return sex_code; } }

            private string sex_description { get; set; }
            public string Description
            {
                get { return sex_description; }
            }
        }

        public sealed class Color
        {
            private int color_id { get; set; }
            public int ColorId { get { return color_id; } }

            private string name { get; set; }
            public string Name { get { return name; } }

            private string hexadecimal { get; set; }
            public string Hexadecimal { get { return hexadecimal; } }
        }

        public sealed class ReportClass
        {           
            private string report_class_fieldid { get; set; }
            public string FieldId { get { return report_class_fieldid; } }

            private string report_class_value { get; set; }
            public string Value { get { return report_class_value; } }
        }
    }

    public sealed class ChartParameters
    {
        private ChartParameters() { }

        public ChartParameters(int activityId)
        {
            using (Entity context = new Entity())
            {
                var activity = context.t_activity.Find(activityId);
                if (activity == null)
                    throw new ArgumentException("Activity not found! activityId: " + activityId.ToString());

                //  var indicator = activity.aims?.Where(e => e.active == true)?.FirstOrDefault()?.indicators?.Where(e => e.active == true)?.FirstOrDefault();
                List<int> aims = context.t_aim.Where(a => a.active == true && a.activity_id == activityId).Select(a => a.aim_id).ToList();                
                var indicator = (from p in context.t_indicator where p.active == true && aims.Contains(p.aim_id) select p).FirstOrDefault();                
                if (indicator == null)
                    throw new ArgumentException("No active indicators found! activityId: " + activityId.ToString());

                IndicatorId = indicator.indicator_id;
            }
        }

        public ChartParameters(int indicatorId, ChartCriteria[] criterias, bool addMedians, int activeCriteria, string reportClassFieldId = "")
        {
            IndicatorId = indicatorId;
            Criterias = criterias;
            AddMedian = addMedians;
            ActiveCriteria = activeCriteria;
            ReportClassFieldId = reportClassFieldId;
        }

        public int IndicatorId { get; set; }
        public string ReportClassFieldId { get; set; }

        public ChartCriteria[] Criterias { get; set; }

        public bool AddMedian { get; set; }

        public int ActiveCriteria { get; set; }

        public static string SerializeToXml(ChartParameters request)
        {
            using (StringWriter writer = new StringWriter())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ChartParameters));
                serializer.Serialize(writer, request);
                return writer.ToString();
            }
        }

        public static ChartParameters DeserializeXml(t_user_favorite_chart userChart)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ChartParameters));
            using (TextReader reader = new StringReader(userChart.chart_parameters))
            {
                return (ChartParameters)serializer.Deserialize(reader);
            }
        }
    }

    public sealed class ChartRulesResult<T>
    {
        private ChartRulesResult() { }

        /// <summary>
        /// Keys => X-axis; Values = Y-axis.
        /// </summary>
        /// <param name="points"></param>
        public ChartRulesResult(ICollection<IPoint<T>> points)
        {
            Points = new List<IPoint<T>>(points);
            _medianValues = new List<MedianPoint<T>[]>();
            _rulesTriggered = new List<RuleTrigger<T>>();

            SetMedians();
        }

        private IList<IPoint<T>> Points { get; set; }

        private List<MedianPoint<T>[]> _medianValues { get; set; }
        public ICollection<MedianPoint<T>[]> MedianValues { get { return _medianValues; } }

        private List<RuleTrigger<T>> _rulesTriggered { get; set; }
        public ICollection<RuleTrigger<T>> RulesTriggered { get { return _rulesTriggered; } }

        private void SetMedians()
        {
            const int minPointsForRules = 10;
            const int shiftThreshold = 6;
            const int trendThreshold = 5;

            // In case there are no points with values for some reason...
            var last = Points.LastOrDefault(x => x.YValue.HasValue);

            if (last != null)
            {
                int n = last == null ? 0 : Points.IndexOf(last);
                if (n < minPointsForRules)
                {
                    // Not enough points to attempt rules. Just compute the median for the entire set.
                    decimal median = CalculateMedian(0, n);
                    SetMedian(0, n, median);
                    SetRuleTriggered(n, chart.RuleNotEnoughPoints);
                }
                else
                {
                    int phaseFirstPoint = 0, shiftStreak = 0, trendStreak = 0, medianCrosses = 1;
                    int? lastValueComparedToOverallMedian = null;
                    bool shiftIsAboveMedian = false, trendIsAscending = false;
                    decimal overallMedian = CalculateMedian(0, n), runningMedian = 0M, lastValue = Decimal.MinValue;

                    for (int i = 0; i < Points.Count; i++)
                    {
                        if (i == phaseFirstPoint)
                        {
                            // Reset counters and flags.
                            shiftStreak = 0;
                            trendStreak = 0;
                            shiftIsAboveMedian = false;
                            trendIsAscending = false;
                        }

                        // NOTE: First iteration should not count toward streaks.
                        var p = Points[i];
                        if (p.YValue.HasValue && i > 0)
                        {
                            // Compute the median up to this point.
                            runningMedian = CalculateMedian(phaseFirstPoint, i);

                            // Rule 1: Shift above/below media.
                            var compareValueToMedian = Decimal.Compare(p.YValue.Value, runningMedian);
                            if ((compareValueToMedian > 0) == shiftIsAboveMedian)
                            {
                                shiftStreak += 1;
                                if (shiftStreak == shiftThreshold)
                                {
                                    SetMedian(phaseFirstPoint, i, runningMedian);
                                    SetRuleTriggered(i, String.Format("{0} {1}", shiftThreshold, chart.RuleShift));
                                    phaseFirstPoint = i + 1;
                                    continue;
                                }
                            }
                            else if (compareValueToMedian != 0)
                            {
                                shiftStreak = 1;
                                shiftIsAboveMedian = compareValueToMedian > 0;
                            }

                            // Rule 2: Ascending or descending trend.
                            var compareValueToLast = Decimal.Compare(p.YValue.Value, lastValue);
                            if ((compareValueToLast > 0) == trendIsAscending)
                            {
                                trendStreak += 1;
                                if (trendStreak == trendThreshold)
                                {
                                    SetMedian(phaseFirstPoint, i, runningMedian);
                                    SetRuleTriggered(i, String.Format("{0} {1}", trendThreshold, chart.RuleTrend));
                                    phaseFirstPoint = i + 1;
                                    continue;
                                }
                            }
                            else if (compareValueToLast != 0)
                            {
                                trendStreak = 1;
                                trendIsAscending = compareValueToLast > 0;
                            }

                            // Rule 3: Runs (crossing the median).
                            int thisValueComparedToOverallMedian = Decimal.Compare(p.YValue.Value, overallMedian);
                            if (lastValueComparedToOverallMedian == null)
                            {
                                // Always count the first data point.
                                medianCrosses += 1;
                                lastValueComparedToOverallMedian = thisValueComparedToOverallMedian;
                            }
                            else if (thisValueComparedToOverallMedian != 0)
                            {
                                // Only count subsequent points that cross the median (not on the median).
                                if (thisValueComparedToOverallMedian != lastValueComparedToOverallMedian)
                                {
                                    medianCrosses += 1;
                                    lastValueComparedToOverallMedian = thisValueComparedToOverallMedian;
                                }
                            }

                            lastValue = p.YValue.Value;
                        }
                    }

                    // Rule 3: Runs (crossing the median).
                    int pointsNotOnMedian = Points.Where(x => x.YValue.HasValue && x.YValue != overallMedian).Count();
                    t_chart_acceptable_number_runs acceptable = null;
                    using (Entity db = new Entity())
                        acceptable = db.t_chart_acceptable_number_runs.Find(pointsNotOnMedian);
                    // If a match was not found in the database, fake it with these approximations.
                    if (acceptable == null)
                        acceptable = new t_chart_acceptable_number_runs()
                        {
                            points_not_on_median = Convert.ToByte(pointsNotOnMedian),
                            runs_lower_limit = Convert.ToByte(pointsNotOnMedian / 2.5),
                            runs_upper_limit = Convert.ToByte(pointsNotOnMedian / 1.5)
                        };
                    if (medianCrosses < acceptable.runs_lower_limit || medianCrosses > acceptable.runs_upper_limit)
                    {
                        // NOTE: Too few or too many runs. Remove all phases and use overall median.
                        _medianValues.Clear();
                        RulesTriggered.Clear();
                        SetMedian(0, n, overallMedian);
                        SetRuleTriggered(n, String.Format("{0} ({1})", chart.RuleRuns, medianCrosses));
                    }
                    else if (phaseFirstPoint < n)
                    {
                        // Set the median for the last phase.
                        SetMedian(phaseFirstPoint, n, runningMedian);
                    }
                }
            }
        }

        private decimal CalculateMedian(int from, int to)
        {
            if (from < 0)
            {
                throw new ArgumentException("From index must be greater than or equal to zero.");
            }
            else if (from > to)
            {
                throw new ArgumentException("From index must be less than or equal to To index.");
            }
            else if (to > 0 && to >= Points.Count)
            {
                throw new ArgumentException("Index must be less than Points.Count.");
            }
            else if (to == 0)
            {
                return 0M;
            }
            else if (to > 0 && from == to)
            {
                return Points[from].YValue ?? 0;
            }
            else
            {
                // Order the not-null values.
                var values = Points.Skip(from).Take(to - from + 1).Where(x => x.YValue.HasValue).Select(x => x.YValue.Value).OrderBy(x => x).ToArray();

                if (values.Length == 0)
                {
                    // No values. Return zero.
                    return 0m;
                }
                else if (values.Length == 2)
                {
                    // Find the midpoint of the two values.
                    return values[0] + (values[1] - values[0]) / 2;
                }
                else if (values.Length % 2 == 0)
                {
                    // Find the midpoint of the two middle values.
                    int midIndex = values.Length / 2;
                    return values[midIndex - 1] + ((values[midIndex] - values[midIndex - 1]) / 2);
                }
                else
                {
                    // Return the middle value.
                    return values[values.Length / 2];
                }
            }
        }

        private void SetMedian(int from, int to, decimal median)
        {
            MedianPoint<T>[] points = new MedianPoint<T>[Points.Count];
            for (int i = 0; i < Points.Count; i++)
            {
                decimal? yValue = null;
                if (i >= from && i <= to)
                    yValue = median;
                points[i] = new MedianPoint<T>(Points[i].XValue, yValue);
            }
            _medianValues.Add(points);
        }

        private void SetRuleTriggered(int index, string rule)
        {
            RulesTriggered.Add(new RuleTrigger<T>(Points[index].XValue, rule));
        }
    }

    public sealed class RuleTrigger<T>
    {
        public RuleTrigger(T label, string rule)
        {
            XValue = label;
            Rule = rule;
        }

        public T XValue { get; private set; }
        public string Rule { get; private set; }
    }
}