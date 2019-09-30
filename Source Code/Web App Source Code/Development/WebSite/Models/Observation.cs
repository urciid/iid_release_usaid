using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using Newtonsoft.Json;

using IID.BusinessLayer.Domain;
using IID.BusinessLayer.Helpers;
using IID.WebSite.Helpers;

namespace IID.WebSite.Models
{
    public class IndicatorSiteObservations
    {
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }
        public int IndicatorId { get; set; }
        [ScriptIgnore]
        public IndicatorType IndicatorType { get; set; }
        [JsonProperty(PropertyName = "IndicatorType")]
        public string IndicatorTypeFieldId { get; set; }
        public int SiteId { get; set; }
        public string IndicatorName { get; set; }
        public string IndicatorDefinition { get; set; }
        public string NumeratorName { get; set; }
        public string NumeratorDefinition { get; set; }
        public string DenominatorName { get; set; }
        public string DenominatorDefinition { get; set; }
        public bool DisaggregateByAge { get; set; }
        public bool DisaggregateBySex { get; set; }
        public DataTable Observations
        {
            get
            {
                return StoredProcedures.GetObservations(IndicatorId, SiteId, null);
            }
        }
    }

    public class Observation : Base
    {
        public Observation(t_indicator indicator, t_site site, DateTime? beginDate)
        {
            t_observation observation =
                indicator.observations.Where(e => e.begin_date == beginDate && e.site_id == site.site_id).FirstOrDefault();

            if (observation == null)
            {
                if (beginDate.HasValue)
                {
                    BeginDate = beginDate.Value;
                    ObjectParameter endDate = new ObjectParameter("end_date", typeof(DateTime));
                    Context.p_get_period_end_date(beginDate, indicator.data_collection_frequency_fieldid, endDate);
                    EndDate = (DateTime)endDate.Value;
                }
                else
                {
                    DateTime lastDay = beginDate ?? indicator.aim.activity.end_date ?? DateTime.Now;
                    p_get_latest_date_period_Result result =
                        Context.p_get_latest_date_period(lastDay, indicator.data_collection_frequency_fieldid).FirstOrDefault();
                    BeginDate = result.begin_date.Value;
                    EndDate = result.end_date.Value;
                }

                IsAgeDisaggregated = indicator.disaggregate_by_age;
                IsSexDisaggregated = indicator.disaggregate_by_sex;
            }
            else
            {
                ObservationId = observation.observation_id;
                BeginDate = observation.begin_date;
                EndDate = observation.end_date;
                IsAgeDisaggregated = observation.is_age_disaggregated;
                IsSexDisaggregated = observation.is_sex_disaggregated;
            }

            SetProperties(indicator.indicator_id, site.site_id, observation, true);
      }

        public Observation(t_observation observation)
        {
            ObservationId = observation.observation_id;
            BeginDate = observation.begin_date;
            EndDate = observation.end_date;
            IsAgeDisaggregated = observation.is_age_disaggregated;
            IsSexDisaggregated = observation.is_sex_disaggregated;

            SetProperties(observation.indicator_id, observation.site_id, observation, false);
        }

        private void SetProperties(int indicatorId, int siteId, t_observation observation, bool loadChildren)
        {
            IndicatorId = indicatorId;
            SiteId = siteId;
            t_observation_entry firstEntry = observation?.entries?.FirstOrDefault();
            CreatedBy = firstEntry?.createdby?.full_name;
            CreatedOn = firstEntry?.created_date;
            UpdatedBy = firstEntry?.updateby?.full_name;
            UpdatedOn = firstEntry?.updated_date;

            // Observation entries.
            Entries = StoredProcedures.GetObservations(IndicatorId, SiteId, BeginDate);

            if (loadChildren)
            {
                // Aim and Indicator
                Indicator = new Indicator(IndicatorId, false);
                Aim = new Aim(Indicator.AimId, false);

                // Other Aim Indicators
                // NOTE: Don't use UserAssignedObjects here. Only show active aims/indicators.
                var aims =
                    Context.t_indicator.Find(indicatorId).aim.activity.aims
                        .Where(e => e.active == true).OrderBy(e => e.sort);
                AimsAndIndicators = new List<Tuple<string, int, string>>();
                foreach (var aim in aims)
                {
                    AimsAndIndicators.Add(new Tuple<string, int, string>("Aim", aim.aim_id, aim.get_name_translated(CurrentLanguageId)));
                    foreach (var ind in aim.indicators.Where(e => e.active == true).OrderBy(e => e.sort))
                        AimsAndIndicators.Add(new Tuple<string, int, string>("Indicator", ind.indicator_id, ind.get_name_translated(CurrentLanguageId)));
                }

                // Populate date period selector's initial set of date periods.
                DatePeriods =
                    ObservationDatePeriod.GetDatePeriods(IndicatorId, siteId, BeginDate, null)
                        .ToDictionary(k => k.BeginDate.ToString("yyyy-MM-dd"), v => v);

                // One-time population of min/max tolerance dictionary.

                MinMaxTolerance = GetMinMaxTolerance();

                // Child collections.
                Changes = observation?.changes?.OrderBy(e => e.start_date).Select(e => new ObservationChange(e)).ToList();
                Attachments = observation?.attachments?.Select(e => new ObservationAttachment(e)).Where(a => (a.Active == true) 
                                                            || (a.CreatedByUserId == CurrentUserId)
                                                            || (CurrentUser.IsInRole(BusinessLayer.Identity.Role.SystemAdministrator))).ToList();
                Comments = observation?.comments?.OrderBy(e => e.created_date).Select(e => new ObservationComment(e)).ToList();
                History = observation?.change_history?.OrderBy(e => e.change_history_id).Select(e => new ObservationHistory(e)).ToList();

                using (Entity context = new Entity())
                    Sexes = context.t_sex.OrderBy(e => e.sex_id).ToDictionary(e => e.sex_code, e => e.sex_description);
            }
        }

        private Dictionary<string, int> GetMinMaxTolerance()
        {
            var dictMinMax = new Dictionary<string, int>();
            Entity context = new Entity();

            int? variance = null;

            t_observation previous_observation = context.t_observation.Where(e => (e.indicator_id == IndicatorId) && (e.site_id == SiteId) && (e.begin_date < BeginDate)).OrderByDescending(d => d.begin_date).FirstOrDefault();

            if (previous_observation == null)
            {
                return dictMinMax;
            }

            ICollection<t_observation_entry> previous_entries = context.t_observation_entry.Where(e => e.observation_id == previous_observation.observation_id).ToList();

            try  //attempt to use indicator change variance 
            {
                if (!String.IsNullOrEmpty(Indicator.ChangeVariable))
                    variance = Convert.ToInt16(Indicator.ChangeVariable); 
            }
            catch(Exception)
            {
                variance = null;
            }

            

            decimal? variance_percentage = (variance ?? Convert.ToDecimal(Settings.ObservationDefaultTolerancePercentage)) * .01m;
            decimal? variance_ratio = (variance ?? Convert.ToDecimal(Settings.ObservationDefaultToleranceRatio)) * .01m;
            decimal? variance_count = (variance ?? Convert.ToDecimal(Settings.ObservationDefaultToleranceCount)) * .01m;
            decimal? variance_average = (variance ?? Convert.ToDecimal(Settings.ObservationDefaultToleranceAverage)) * .01m;


            foreach (t_observation_entry entry in previous_entries)
            {
                string item;
                int? range_id;
                item = entry.indicator_gender ?? " ";
                range_id = entry.indicator_age_range_id ?? 0;
                int? minAmount = 0;
                int? maxAmount = 0;

                int? current_data_percent = 0;
                int? current_data_ratio = 0;
                decimal current_data_average = 0;

                if (entry.denominator != 0 && entry.denominator != null)
                {
                    current_data_percent = Convert.ToInt16((Convert.ToDecimal(entry.numerator) / Convert.ToDecimal(entry.denominator) * 100));
                    current_data_ratio = Convert.ToInt16((Convert.ToDecimal(entry.numerator) / Convert.ToDecimal(entry.denominator) * Indicator.RatioPer));
                    current_data_average = Convert.ToDecimal((entry.numerator) / Convert.ToDecimal(entry.denominator) * 100m);
                }


                switch (Indicator.Type)
                {
                    case IndicatorType.Count:
                        if (entry.count < 10)
                        {
                            minAmount = 0;
                            maxAmount = 99999;
                        }
                        else if (entry.count < 20)
                        {
                            variance = Convert.ToInt16(1.5m * variance_count * entry.count);
                            minAmount = entry.count - variance;
                            maxAmount = entry.count + variance;
                        }
                        else
                        {
                            variance = Convert.ToInt16(variance_count * entry.count);
                            minAmount = entry.count - variance;
                            maxAmount = entry.count + variance;
                        }
                        break;

                    case IndicatorType.Percentage:
                        if (entry.denominator < 10)
                        {
                            minAmount = 0;
                            maxAmount = 99999;
                        }
                        else if (entry.denominator < 20)
                        {
                            variance = Convert.ToInt16(1.5m * variance_percentage * 100m);
                            minAmount = current_data_percent - variance;
                            maxAmount = current_data_percent + variance;
                        }
                        else
                        {
                            variance = Convert.ToInt16(variance_percentage * 100m);
                            minAmount = current_data_percent - variance;
                            maxAmount = current_data_percent + variance;
                        }
                        break;

                    case IndicatorType.Ratio:
                        if (entry.denominator < 10)
                        {
                            minAmount = 0;
                            maxAmount = 99999;
                        }
                        else if (entry.denominator < 20)
                        {
                            variance = Convert.ToInt16(1.5m * variance_ratio * current_data_ratio);
                            minAmount = current_data_ratio - variance;
                            maxAmount = current_data_ratio + variance;
                        }
                        else
                        {
                            variance = Convert.ToInt16(variance_ratio * current_data_ratio);
                            minAmount = current_data_ratio - variance;
                            maxAmount = current_data_ratio + variance;
                        }
                        break;

                    case IndicatorType.Average:
                        if (entry.denominator < 10)
                        {
                            minAmount = 0;
                            maxAmount = 99999;
                        }
                        else if (entry.denominator < 20)
                        {
                            variance = Convert.ToInt16(1.5m * variance_average * current_data_average);
                            minAmount = current_data_percent - variance;
                            maxAmount = current_data_percent + variance;
                        }
                        else
                        {
                            variance = Convert.ToInt16(variance_average * current_data_average);
                            minAmount = current_data_percent - variance;
                            maxAmount = current_data_percent + variance;
                        }
                        break;
                }

                dictMinMax.Add("MIN" + item + range_id.ToString(), minAmount ?? 0);
                dictMinMax.Add("MAX" + item + range_id.ToString(), maxAmount ?? 99999);

            }

            return dictMinMax;
        }

        public int? ObservationId { get; set; }
        public int IndicatorId { get; set; }
        public int SiteId { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsAgeDisaggregated { get; set; }
        public bool IsSexDisaggregated { get; set; }

        [ScriptIgnore]
        public Indicator Indicator { get; set; }

        [ScriptIgnore]
        public ICollection<Tuple<string, int, string>> AimsAndIndicators { get; set; }

        [ScriptIgnore]
        public Aim Aim { get; set; }

        [ScriptIgnore]
        public Dictionary<string, int> MinMaxTolerance { get; set; }

        [ScriptIgnore]
        public Dictionary<string, ObservationDatePeriod> DatePeriods { get; set; }

        [ScriptIgnore]
        public DataTable Entries { get; set; }
        public Dictionary<string, Dictionary<string, object>> EntriesCollection
        {
            get
            {
                // Make sure there is some data. It is possible for a table to be totally empty.
                if (Entries != null && Entries.Columns.Count > 0 && Entries.Rows.Count > 0)
                {
                    if (!Entries.Columns.Contains("key"))
                        Entries.Columns.Add("key", typeof(string),
                            "(IsNull(age_range_id, '') + '|' + IsNull(sex_code, '') + '|' + type)");

                    return Entries.ToDictionaryOfDictionaries(x => (string)x["key"]);
                }
                else
                {
                    return null;
                }
            }
        }

        [ScriptIgnore]
        public ICollection<ObservationChange> Changes { get; set; }

        [ScriptIgnore]
        public ICollection<ObservationAttachment> Attachments { get; set; }

        [ScriptIgnore]
        public ICollection<ObservationComment> Comments { get; set; }

        [ScriptIgnore]
        public ICollection<ObservationHistory> History { get; set; }

        [ScriptIgnore]
        public Dictionary<string, string> Sexes { get; set; }

        [ScriptIgnore]
        public SelectList YesNoOptions
        {
            get
            {
                var options = new Dictionary<string, int>();
                options.Add("Yes", 1);
                options.Add("No", 0);
                return new SelectList(options, "Value", "Key");
            }
        }
    }

    public class ObservationDatePeriod
    {
        public static List<ObservationDatePeriod> GetDatePeriods(int indicatorId, int siteId, DateTime date, int? periods)
        {
            using (Entity context = new Entity())
            {
                var list = new List<ObservationDatePeriod>();

                var result = context.p_get_observation_date_periods(indicatorId, siteId, date, periods);
                foreach (var r in result)
                {
                    // Set the times as UTC. Important for JSON serialization!
                    var odp = new ObservationDatePeriod() { ObservationId = r.observation_id };
                    if (r.begin_date.HasValue)
                        odp.BeginDate = DateTime.SpecifyKind(r.begin_date.Value, DateTimeKind.Local);
                    if (r.end_date.HasValue)
                        odp.EndDate = DateTime.SpecifyKind(r.end_date.Value, DateTimeKind.Local);
                    odp.HasChangeCommentAttachment = r.has_changecommentattachment;
                    odp.ObservationId = r.observation_id;
                    list.Add(odp);
                }

                return list;
            }
        }

        public ObservationDatePeriod() { }

        public ObservationDatePeriod(v_observation_date_period entity)
        {
            BeginDate = entity.begin_date;
            EndDate = entity.end_date;
            ObservationId = entity.observation_id;
            HasChangeCommentAttachment = entity.has_changecommentattachment;
        }

        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? ObservationId { get; set; }
        public bool HasChangeCommentAttachment { get; set; }
    }

    public class ObservationEntry
    {
        public int? AgeRangeId { get; set; }
        public string SexCode { get; set; }
        public int? Numerator { get; set; }
        public int? Denominator { get; set; }
        public int? Count { get; set; }
        public decimal? Ratio { get; set; }
        public bool? YesNo { get; set; }
    }

    public class ObservationEntryCollection
    {
        public int IndicatorId { get; set; }
        public int SiteId { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsAgeDisaggregated { get; set; }
        public bool IsSexDisaggregated { get; set; }

        public ICollection<ObservationEntry> Entries { get; set; }
    }

    public class ObservationChange
    {
        public ObservationChange() { }

        public ObservationChange(t_observation_change change)
        {
            ChangeId = change.observation_change_id;
            ObservationId = change.observation_id;
            Description = change.description;
            StartDate = change.start_date;
            CreatedByUserId = change.createdby_userid;
            CreatedOn = change.created_date;
            UpdatedByUserId = change.updatedby_userid;
            UpdatedOn = change.updated_date;
            Approved = change.approved;
        }

        public int ChangeId { get; set; }
        public int ObservationId { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public int CreatedByUserId { get; set; }
        public string CreatedBy { get { return UserName.Get(CreatedByUserId); } }
        public DateTime CreatedOn { get; set; }
        public int? UpdatedByUserId { get; set; }
        public string UpdatedBy { get { return UpdatedByUserId.HasValue ? UserName.Get(UpdatedByUserId.Value) : null; } }
        public DateTime? UpdatedOn { get; set; }
        public bool Approved { get; set; }
    }

    public class ObservationAttachment
    {
        public ObservationAttachment() { }

        public ObservationAttachment(t_observation_attachment attachment)
        {
            AttachmentId = attachment.observation_attachment_id;
            ObservationId = attachment.observation_id;
            FileName = attachment.attachment_file_name;
            FileSize = attachment.attachment.Length;
            Active = attachment.active;
            CreatedByUserId = attachment.createdby_userid;
            CreatedOn = attachment.created_date;
            UpdatedByUserId = attachment.updatedby_userid;
            UpdatedOn = attachment.updated_date;
        }

        public int AttachmentId { get; set; }
        public int ObservationId { get; set; }
        public string FileName { get; set; }
        public byte[] FileBytes { get; set; }
        public long FileSize { get; set; }
        public bool? Active { get; set; }
        public int CreatedByUserId { get; set; }
        public string CreatedBy { get { return UserName.Get(CreatedByUserId); } }
        public DateTime CreatedOn { get; set; }
        public int? UpdatedByUserId { get; set; }
        public string UpdatedBy { get { return UpdatedByUserId.HasValue ? UserName.Get(UpdatedByUserId.Value) : null; } }
        public DateTime? UpdatedOn { get; set; }
    }

    public class ObservationComment
    {
        public ObservationComment() { }

        public ObservationComment(t_observation_comment comment)
        {
            CommentId = comment.observation_comment_id;
            ObservationId = comment.observation_id;
            Comment = comment.comment;
            CreatedByUserId = comment.createdby_userid;
            CreatedOn = comment.created_date;
            UpdatedByUserId = comment.updatedby_userid;
            UpdatedOn = comment.updated_date;
        }

        public int CommentId { get; set; }
        public int ObservationId { get; set; }
        public string Comment { get; set; }
        public int CreatedByUserId { get; set; }
        public string CreatedBy { get { return UserName.Get(CreatedByUserId); } }
        public DateTime CreatedOn { get; set; }
        public int? UpdatedByUserId { get; set; }
        public string UpdatedBy { get { return UpdatedByUserId.HasValue ? UserName.Get(UpdatedByUserId.Value) : null; } }
        public DateTime? UpdatedOn { get; set; }
    }

    public class ObservationHistory
    {
        public ObservationHistory() { }

        public ObservationHistory(t_observation_entry_change_history history)
        {
            Type = history.change_type;
            Date = history.change_date;
            UserId = history.change_userid;
            UserName = history.change_user.full_name;
            Label = history.change_label;
            Description = history.change_description;
        }

        public string Type { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
    }
}