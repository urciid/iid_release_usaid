using System;
using System.Linq;
using System.Web.Mvc;

using IID.BusinessLayer.Domain;
using IID.BusinessLayer.Helpers;
using IID.WebSite.Models;
using IID.WebSite.Helpers;
using IID.BusinessLayer.Globalization;

namespace IID.WebSite.Controllers
{
    [NoCache]
    public class IndicatorController : BaseController
    {
        public ActionResult Add(int activityId)
        {
            return View(new Indicator(activityId));
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Indicator model)
        {
            t_indicator entity = SetEntityFromModel(model);
            return RedirectToAction("View", new { id = entity.indicator_id });
        }

        public ActionResult View(int id)
        {
            Indicator model = new Indicator(id, true);
            AccessPass = SecurityGuard(CurrentUser, 0, model.ActivityId, 0);
            if (AccessPass == BusinessLayer.Identity.UserSecurityAccess.NoAccess)
            {
                return RedirectToAction("NoAccess", "Home");
            }
            ViewBag.AccessRights = AccessPass;

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            Indicator model = new Indicator(id, true);
            AccessPass = SecurityGuard(CurrentUser, 0, model.ActivityId, 0);
            if (AccessPass == BusinessLayer.Identity.UserSecurityAccess.Update)
            {
                return View(model);
            }
            {
                return RedirectToAction("NoAccess", "Home");
            }

        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Indicator model)
        {
            SetEntityFromModel(model);
            return RedirectToAction("View", new { id = model.IndicatorId.Value });
        }

        private t_indicator SetEntityFromModel(Indicator model)
        {
            using (Entity context = new Entity())
            {
                t_indicator indicator = null;
                if (model.IndicatorId.HasValue)
                {
                    indicator = context.t_indicator.Find(model.IndicatorId.Value);
                    if (indicator == null)
                        throw new ArgumentException("Invalid indicatorId: " + model.IndicatorId.ToString());
                    indicator.updatedby_userid = CurrentUser.Id;
                    indicator.updated_date = DateTime.Now;
                }
                else
                {
                    indicator = new t_indicator();
                    indicator.createdby_userid = CurrentUser.Id;
                    indicator.created_date = DateTime.Now;

                    byte maxSort = 0;
                    var aim = context.t_aim.Find(model.AimId);
                    if (aim.indicators.Count > 0)
                        maxSort = aim.indicators.Max(e => e.sort);
                    indicator.sort = maxSort += 1;

                    context.t_indicator.Add(indicator);
                }

                var language = IidCulture.CurrentLanguage;

                indicator.aim_id = model.AimId;

                // Only apply t_indicator.name if the language is English or this is 
                // a new indicator (in which case, we have to supply some value).
                if (language == Language.English || !model.IndicatorId.HasValue)
                {
                    indicator.name = model.Name;
                    indicator.definition = model.Definition;
                }

                t_fieldid indicatorTypeField = context.t_fieldid.Find(model.TypeFieldId);
                IndicatorType indicatorType = Enumerations.Parse<IndicatorType>(indicatorTypeField.value);
                switch (indicatorType)
                {
                    case IndicatorType.Percentage:
                    case IndicatorType.Average:
                    case IndicatorType.Ratio:
                        if (language == Language.English || !model.IndicatorId.HasValue)
                        {
                            indicator.numerator_name = model.NumeratorName;
                            indicator.numerator_definition = model.NumeratorDefinition;
                            indicator.numerator_source = model.NumeratorSource;
                            indicator.denominator_name = model.DenominatorName;
                            indicator.denominator_definition = model.DenominatorDefinition;
                            indicator.denominator_source = model.DenominatorSource;
                        }
                        break;

                    default:
                        indicator.numerator_name = null;
                        indicator.numerator_definition = null;
                        indicator.denominator_name = null;
                        indicator.denominator_definition = null;
                        indicator.denominator_source = null;
                        break;
                }
                indicator.numerator_source = model.NumeratorSource;
                indicator.indicator_type_fieldid = model.TypeFieldId;
                indicator.group_fieldid = model.GroupFieldId;
                indicator.data_collection_frequency_fieldid = model.DataCollectionFrequencyFieldId;
                indicator.sampling_fieldid = model.SamplingFieldId;
                indicator.report_class_fieldid = model.ReportClassFieldId;
                switch (indicatorType)
                {
                    case IndicatorType.Percentage:
                    case IndicatorType.Average:
                    case IndicatorType.Count:
                    case IndicatorType.Ratio:
                        indicator.change_variable = model.ChangeVariable;
                        break;

                    default:
                        indicator.change_variable = null;
                        break;
                }
                switch (indicatorType)
                {
                    case IndicatorType.Percentage:
                    case IndicatorType.Average:
                    case IndicatorType.Count:
                    case IndicatorType.Ratio:
                        indicator.disaggregate_by_sex = model.DisaggregateBySex;
                        indicator.disaggregate_by_age = model.DisaggregateByAge;
                        break;

                    default:
                        indicator.disaggregate_by_sex = false;
                        indicator.disaggregate_by_age = false;
                        break;
                }
                switch (indicatorType)
                {
                    case IndicatorType.Percentage:
                    case IndicatorType.Average:
                    case IndicatorType.Count:
                    case IndicatorType.Ratio:
                        indicator.target_performance = model.TargetPerformance;
                        indicator.threshold_good_performance = model.ThresholdGoodPerformance;
                        indicator.threshold_poor_performance = model.ThresholdPoorPerformance;
                        indicator.increase_is_good = model.IncreaseIsGood;
                        break;

                    default:
                        indicator.target_performance = null;
                        indicator.threshold_good_performance = null;
                        indicator.threshold_poor_performance = null;
                        indicator.increase_is_good = false;
                        break;
                }
                switch (indicatorType)
                {
                    case IndicatorType.Ratio:
                        indicator.rate_per_fieldid = model.RatePerFieldId;
                        break;

                    default:
                        indicator.rate_per_fieldid = null;
                        break;
                }
                indicator.active = model.Active;


                context.SaveChanges();

                // Defer translation logic until after the indicator has been saved to the db.
                // This is necessary because we depend on indicator_id, which may not be known sooner.
                if (language != Language.English)
                {
                    var languageId = IidCulture.CurrentLanguageId;
                    var userId = CurrentUser.Id;
                    indicator.set_name_translated(languageId, model.Name, userId);
                    indicator.set_definition_translated(languageId, model.Definition, userId);
                    indicator.set_numerator_name_translated(languageId, model.NumeratorName, userId);
                    indicator.set_numerator_definition_translated(languageId, model.NumeratorDefinition, userId);
                    indicator.set_numerator_source_translated(languageId, model.NumeratorSource, userId);
                    indicator.set_denominator_name_translated(languageId, model.DenominatorName, userId);
                    indicator.set_denominator_definition_translated(languageId, model.DenominatorDefinition, userId);
                    indicator.set_denominator_source_translated(languageId, model.DenominatorSource, userId);
                }

                // NOTE: Once observations have been recorded, age ranges cannot be modified,
                // as this would cause a primary key constraint violation.
                if (indicator.observations?.Count() == 0)
                {
                    context.t_indicator_age_range.RemoveRange(indicator.indicator_age_ranges);
                    if (model.DisaggregateByAge && model.AgeRangeNames != null)
                    {
                        foreach (string ageRange in model.AgeRangeNames)
                            indicator.indicator_age_ranges.Add(
                                new t_indicator_age_range()
                                {
                                    indicator_id = indicator.indicator_id, //entity.IndicatorId.Value,
                                age_range = ageRange
                                });

                        context.SaveChanges();
                    }
                }

                return indicator;
            }
        }

        public JsonResult SortUp(int id)
        {
            using (Entity context = new Entity())
            {
                var thisIndicator = context.t_indicator.Find(id);
                var prevIndicator =
                    thisIndicator.aim.indicators
                        .OrderByDescending(e => e.sort)
                        .Where(e => e.sort < thisIndicator.sort)
                        .FirstOrDefault();

                if (prevIndicator != null)
                {
                    thisIndicator.sort = prevIndicator.sort;
                    prevIndicator.sort = Convert.ToByte(thisIndicator.sort + 1);
                    context.SaveChanges();
                }

                return GetSortResult(thisIndicator, prevIndicator);
            }
        }

        public JsonResult SortDown(int id)
        {
            using (Entity context = new Entity())
            {
                var thisIndicator = context.t_indicator.Find(id);
                var nextIndicator =
                    thisIndicator.aim.indicators
                        .OrderBy(e => e.sort)
                        .Where(e => e.sort > thisIndicator.sort)
                        .FirstOrDefault();

                if (nextIndicator != null)
                {
                    nextIndicator.sort = thisIndicator.sort;
                    thisIndicator.sort += 1;
                    context.SaveChanges();
                }

                return GetSortResult(nextIndicator, thisIndicator);
            }
        }

        private JsonResult GetSortResult(t_indicator indicator1, t_indicator indicator2)
        {
            return Json(new
            {
                Indicator1 = new { Id = indicator1.indicator_id, Sort = indicator1.sort },
                Indicator2 = new { Id = indicator2?.indicator_id, Sort = indicator2?.sort }
            });
        }
    }
}