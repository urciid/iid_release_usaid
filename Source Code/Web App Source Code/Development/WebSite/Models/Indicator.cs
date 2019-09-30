using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using IID.BusinessLayer.Domain;
using common = IID.BusinessLayer.Globalization.Common.Resource;
using error = IID.BusinessLayer.Globalization.Error.Resource;
using indicator = IID.BusinessLayer.Globalization.Indicator.Resource;
using IID.BusinessLayer.Helpers;
using ExpressiveAnnotations.Attributes;
using System.Globalization;

namespace IID.WebSite.Models
{
    public class Indicator : Base
    {
        public Indicator() { }

        public Indicator(int activityId)
        {
            // A little bit different workflow because we are instantiating a new (empty) indicator.
            ActivityId = activityId;

            var activity = Context.t_activity.Find(activityId);
            var aims =
                UserAssignedObjects.GetAims(activity.aims, CurrentUser)
                    .Select(e => new Tuple<int?, string>(e.aim_id, e.get_name_translated(CurrentLanguageId)))
                    .ToList();
            aims.Insert(0, new Tuple<int?, string>(null, null));
            ActivityAims = aims;

            AgeRanges = new Dictionary<string, string>();
        }

        public Indicator(int indicatorId, bool loadChildren)
        {
            t_indicator entity = Context.t_indicator.Find(indicatorId);
            SetProperties(entity, loadChildren, loadChildren, true);
        }

        public Indicator(t_indicator entity, bool loadChildren, bool translate)
        {
            SetProperties(entity, loadChildren, loadChildren, translate);
        }

        public Indicator(t_indicator entity, bool loadChildren, bool loadParent, bool translate)
        {
            SetProperties(entity, loadChildren, loadParent, translate);
        }

        private void SetProperties(t_indicator entity, bool loadChildren, bool loadParent, bool translate)
        {
            ActivityId = entity.aim?.activity_id ?? -1; // Refactor as nullable int when possible.
            AimId = entity.aim_id;
            IndicatorId = entity.indicator_id;
            TypeFieldId = entity.indicator_type_fieldid;
            TypeValue = entity.indicator_type?.value;
            GroupFieldId = entity.group_fieldid;
            GroupValue = entity.indicator_group?.value;
            DataCollectionFrequencyFieldId = entity.data_collection_frequency_fieldid;
            DataCollectionFrequencyValue = entity.data_collection_frequency?.value;  
            SamplingFieldId = entity.sampling_fieldid;
            ReportClassFieldId = entity.report_class_fieldid;
            ReportClassValue = entity.report_class?.value;
            SamplingValue = entity.sampling?.value;
            ChangeVariable = entity.change_variable;
            DisaggregateBySex = entity.disaggregate_by_sex;
            DisaggregateByAge = entity.disaggregate_by_age;
            if (DisaggregateByAge)
            {
                AgeRanges =
                    entity.indicator_age_ranges
                        .OrderBy(e => e.indicator_age_range_id)
                        .ToDictionary(e => Convert.ToString(e.indicator_age_range_id), e => e.age_range);
            }
            else
            {
                AgeRanges = new Dictionary<string, string>();
            }
            TargetPerformance = entity.target_performance;
            ThresholdGoodPerformance = entity.threshold_good_performance;
            ThresholdPoorPerformance = entity.threshold_poor_performance;
            IncreaseIsGood = entity.increase_is_good;
            RatePerFieldId = entity.rate_per_fieldid;
            RatePerValue = entity.rate_per?.value;
            Active = entity.active;
            Sort = entity.sort;

            if (translate)
            {
                Definition = entity.get_definition_translated(CurrentLanguageId);
                Name = entity.get_name_translated(CurrentLanguageId);
                NumeratorName = entity.get_numerator_name_translated(CurrentLanguageId);
                NumeratorDefinition = entity.get_numerator_definition_translated(CurrentLanguageId);
                NumeratorSource = entity.get_numerator_source_translated(CurrentLanguageId);
                DenominatorName = entity.get_denominator_name_translated(CurrentLanguageId);
                DenominatorDefinition = entity.get_denominator_definition_translated(CurrentLanguageId);
                DenominatorSource = entity.get_denominator_source_translated(CurrentLanguageId);
                //

                DataCollectionFrequencyValue = entity.get_data_frequency_translated(CurrentLanguageId, DataCollectionFrequencyValue);
            }
            else
            {
                Definition = entity.definition;
                Name = entity.name;
                NumeratorName = entity.numerator_name;
                NumeratorDefinition = entity.numerator_definition;
                NumeratorSource = entity.numerator_source;
                DenominatorName = entity.denominator_name;
                DenominatorDefinition = entity.denominator_definition;
                DenominatorSource = entity.denominator_source;
            }

            if (loadParent)
            {
                Aim = new Aim(AimId, false);
            }

            if (loadChildren)
            {
                ActivityAims =
                    UserAssignedObjects.GetAims(entity.aim.activity.aims, CurrentUser)
                        .Select(e => new Tuple<int?, string>(e.aim_id, e.get_name_translated(CurrentLanguageId)))
                        .ToList();
                Sites =
                    entity.aim.activity.sites
                        .Select(e => new ActivitySite(e))
                        .ToList();
                HasObservations = entity.observations?.Any() ?? false;
                OtherIndicators =
                    UserAssignedObjects.GetIndicators(entity.aim.indicators, CurrentUser)
                        .Where(e => e.indicator_id != IndicatorId)
                        .Select(e => new Indicator(e, false, true))
                        .ToList();
            }
        }

        [ScriptIgnore]
        public int ActivityId { get; set; }

        [ScriptIgnore]
        public ICollection<Tuple<int?, string>> ActivityAims { get; set; }

        [Required(ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        [Display(Name = "Aim", ResourceType = typeof(common))]
        public int AimId { get; set; }

        [ScriptIgnore]
        public Aim Aim { get; private set; }

        public int? IndicatorId { get; set; }

        [Display(Name = "Definition", ResourceType = typeof(indicator))]
        public string Definition { get; set; }

        [Display(Name = "Type", ResourceType = typeof(common))]
        [Required(ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        public string TypeFieldId { get; set; }
        [ScriptIgnore]
        public string TypeValue { get; set; }
        [ScriptIgnore]
        public IndicatorType Type {  get { return Enumerations.Parse<IndicatorType>(TypeValue); } }

        [Required(ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        [Display(Name = "Group", ResourceType = typeof(indicator))]
        public string GroupFieldId { get; set; }
        [ScriptIgnore]
        public string GroupValue { get; set; }

        [AllowHtml]
        [Display(Name = "Name", ResourceType = typeof(common))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        public string Name { get; set; }

        [Display(Name = "Name", ResourceType = typeof(common))]
        [RequiredIf("TypeFieldId == 'indper' || TypeFieldId == 'indavg' || TypeFieldId == 'indrat'", AllowEmptyStrings = false, ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        public string NumeratorName { get; set; }

        [Display(Name = "Definition", ResourceType = typeof(indicator))]
        public string NumeratorDefinition { get; set; }

        [Display(Name = "Source", ResourceType = typeof(indicator))]
        public string NumeratorSource { get; set; }

        [Display(Name = "Name", ResourceType = typeof(common))]
        [RequiredIf("TypeFieldId == 'indper' || TypeFieldId == 'indavg' || TypeFieldId == 'indrat'", AllowEmptyStrings = false, ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        public string DenominatorName { get; set; }

        [Display(Name = "Definition", ResourceType = typeof(indicator))]
        public string DenominatorDefinition { get; set; }

        [Display(Name = "Source", ResourceType = typeof(indicator))]
        public string DenominatorSource { get; set; }

        [Display(Name = "DataCollectionFrequency", ResourceType = typeof(indicator))]
        [Required(ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        public string DataCollectionFrequencyFieldId { get; set; }
        [ScriptIgnore]
        public string DataCollectionFrequencyValue { get; set; }

        [Display(Name = "Sampling", ResourceType = typeof(indicator))]
        [Required(ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        public string SamplingFieldId { get; set; }
        [ScriptIgnore]
        public string SamplingValue { get; set; }

        [Display(Name = "ReportClass", ResourceType = typeof(indicator))]
        public string ReportClassFieldId { get; set; }
        [ScriptIgnore]
        public string ReportClassValue { get; set; }

        [Display(Name = "ChangeVariable", ResourceType = typeof(indicator))]
        public string ChangeVariable { get; set; }

        [UIHint("YesNo")]
        [Display(Name = "DisaggregateBySex", ResourceType = typeof(indicator))]
        [Required(ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        public bool DisaggregateBySex { get; set; }

        [UIHint("YesNo")]
        [Display(Name = "DisaggregateByAge", ResourceType = typeof(indicator))]
        [Required(ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        public bool DisaggregateByAge { get; set; }

        [Display(Name = "TargetPerformance", ResourceType = typeof(indicator))]
        public decimal? TargetPerformance { get; set; }

        [Display(Name = "ThresholdGoodPerformance", ResourceType = typeof(indicator))]
        public decimal? ThresholdGoodPerformance { get; set; }

        [Display(Name = "ThresholdPoorPerformance", ResourceType = typeof(indicator))]
        public decimal? ThresholdPoorPerformance { get; set; }

        [Required(ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        public bool IncreaseIsGood { get; set; }
        [Display(Name = "IncreaseIsGood", ResourceType = typeof(indicator))]
        public string IncreaseIsGoodText { get { return IncreaseIsGood ? "Increase" : "Decrease"; } }

        [Display(Name = "RatePer", ResourceType = typeof(indicator))]
        [RequiredIf("TypeFieldId == 'indrat'", AllowEmptyStrings = false, ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        public string RatePerFieldId { get; set; }
        [ScriptIgnore]
        public string RatePerValue { get; set; }
        [ScriptIgnore]
        public int? RatioPer
        {
            get
            {
                if (Type == IndicatorType.Ratio)
                {
                    int ratioPer = 0;
                    if (Int32.TryParse(RatePerValue.Replace(",",""), out ratioPer))
                        return ratioPer;
                }
                return null;
            }
        }

        [UIHint("Active")]
        [Display(Name = "Status")]
        public bool? Active { get; set; }

        // NOTE: This member receives strings posted from the view.
        [Display(Name = "AgeRanges", ResourceType = typeof(indicator))]
        [ScriptIgnore]
        public IEnumerable<string> AgeRangeNames { get; set; }

        // NOTE: This member holds the id and name pairs of database entries.
        [Display(Name = "AgeRanges", ResourceType = typeof(indicator))]
        [ScriptIgnore]
        public Dictionary<string, string> AgeRanges { get; set; }

        [Display(Name = "Disaggregation", ResourceType = typeof(indicator))]
        public string DisaggregationText
        {
            get
            {
                if (DisaggregateBySex && DisaggregateByAge)
                    return indicator.DisaggregateBySexAge;
                else if (DisaggregateBySex)
                    return indicator.DisaggregateBySex;
                else if (DisaggregateByAge)
                    return indicator.DisaggregateByAge;
                else
                    return indicator.DisaggregateNone;
            }
        }

        public string Status
        {
            get
            {
                switch (Active)
                {
                    case true:
                        return common.ActiveStatus;
                    case false:
                        return common.InactiveStatus;
                    default:
                        return common.PendingStatus;
                }
            }
        }

        public byte Sort { get; set; }

        [ScriptIgnore]
        [Display(Name = "Sites", ResourceType = typeof(common))]
        public ICollection<ActivitySite> Sites { get; set; }

        [ScriptIgnore]
        public SelectList IndicatorTypes
        {
            get
            {
                return SelectLists.GetFromDatabaseModel<t_fieldid>(
                    "value", "fieldid", e => e.parent_fieldid == FieldIdParentTypes.IndicatorType, e => e.value,
                    !IndicatorId.HasValue);
            }
        }

        [ScriptIgnore]
        public SelectList IndicatorGroups
        {
            get
            {
                return SelectLists.GetFromDatabaseModel<t_fieldid>(
                    "value", "fieldid", e => e.parent_fieldid == FieldIdParentTypes.IndicatorGroup, e => e.value,
                    !IndicatorId.HasValue);
            }
        }

        [ScriptIgnore]
        public SelectList DataCollectionFrequencies
        {
            get
            {
                return SelectLists.GetFromDatabaseModel<t_fieldid>(
                    "value", "fieldid", e => e.parent_fieldid == FieldIdParentTypes.IndicatorCollectionFrequency,
                    e => e.sort_key, !IndicatorId.HasValue);
            }
        }

        [ScriptIgnore]
        public SelectList Samplings
        {
            get
            {
                return SelectLists.GetFromDatabaseModel<t_fieldid>(
                    "value", "fieldid", e => e.parent_fieldid == FieldIdParentTypes.Sampling, e => e.value,
                    !IndicatorId.HasValue);
            }
        }

        [ScriptIgnore]
        public SelectList ReportClasses
        {
            get
            {
                return SelectLists.GetFromDatabaseModel<t_fieldid>(
                    "value", "fieldid", e => e.parent_fieldid == FieldIdParentTypes.ReportClass, e => e.value,
                    !IndicatorId.HasValue);
            }
        }

        [ScriptIgnore]
        public SelectList RatiosPer
        {
            get
            {
                return SelectLists.GetFromDatabaseModel<t_fieldid>(
                    "value", "fieldid", e => e.parent_fieldid == FieldIdParentTypes.RatioPer, e => e.sort_key,
                    !IndicatorId.HasValue);
            }
        }

        public bool HasObservations { get; private set; }

        [ScriptIgnore]
        public IEnumerable<Indicator> OtherIndicators { get; private set; }
    }
}