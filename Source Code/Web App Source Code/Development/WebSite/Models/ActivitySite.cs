using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;

using IID.BusinessLayer.Domain;
using common = IID.BusinessLayer.Globalization.Common.Resource;
using site = IID.BusinessLayer.Globalization.Site.Resource;
using IID.WebSite.Helpers;
using System.Web.Script.Serialization;

namespace IID.WebSite.Models
{
    public class ActivitySite
    {
        public ActivitySite(t_activity_site entity)
        {
            ActivityId = entity.activity_id;
            SiteId = entity.site_id;
            SiteName = entity.site?.name;
            SiteTypeValue = entity.site?.site_type.value;
            CoachUserId = entity.coach_user_id;
            SupportStartDate = entity.support_start_date;
            SupportEndDate = entity.support_end_date;
            WaveFieldId = entity.wave_fieldid;
            WaveValue = entity.wave?.value;

            HasObservations = entity.activity?.aims?.SelectMany(a => a.indicators?.SelectMany(i => i.observations))?.Where(s => s.site_id == entity.site_id).Any() ?? false;
        }

        public string Key { get { return String.Concat(ActivityId, "_", SiteId); } }

        public int ActivityId { get; set; }

        public int SiteId { get; set; }
        [ScriptIgnore]
        public string SiteName { get; set; }

        [ScriptIgnore]
        public string SiteTypeValue { get; set; }

        public int? CoachUserId { get; set; }
        public string CoachUserName { get { return CoachUserId.HasValue ? UserName.Get(CoachUserId.Value) : null; } }

        public DateTime? SupportStartDate { get; set; }

        public DateTime? SupportEndDate { get; set; }

        public string WaveFieldId { get; set; }
        [ScriptIgnore]
        public string WaveValue { get; set; }

        [ScriptIgnore]
        public bool HasObservations { get; private set; }
    }

    public class ActivitySiteCollection
    {
        public ActivitySiteCollection(int activityId, int? indicatorId)
        {
            using (Entity context = new Entity())
            {
                t_activity activity = context.t_activity.Find(activityId);
                if (activity == null)
                    throw new ArgumentException("Invalid activityId:" + activityId.ToString());

                ActivityId = activityId;
                IndicatorId = indicatorId;
                Sites = activity.sites
                    .Where(e => e.activity_id == activityId)
                    .Select(e => new ActivitySite(e))
                    .OrderBy(e => e.SiteName)
                    .ToArray();
            }
        }

        public int ActivityId { get; set; }

        public int? IndicatorId { get; set; }

        public ICollection<ActivitySite> Sites { get; set; }
    }

    public class ActivitySiteSearchCriteria
    {
        public ActivitySiteSearchCriteria(int activityId)
        {
            using (Entity context = new Entity())
            {
                var activity = context.t_activity.Find(activityId);
                if (activity == null)
                    throw new ArgumentException(("Invalid activityId: " + activityId.ToString()));

                ActivityId = activityId;
                CountryId = activity.country_id;
            }
        }

        public int ActivityId { get; set; }

        public int? CountryId { get; set; }
        [Display(Name = "Country", ResourceType = typeof(common))]
        public string CountryName { get; set; }

        public string SiteTypeFieldId { get; set; }
        [Display(Name = "Type", ResourceType = typeof(site))]
        public string SiteTypeValue { get; set; }

        public bool Assigned = false; // This flag is only changed on the client side.

        public Dictionary<string, string> Waves
        {
            get
            {
                using (Entity context = new Entity())
                {
                    return context.t_fieldid.Where(e => e.parent_fieldid == FieldIdParentTypes.Wave)
                        .ToList()
                        .Select(e => new
                        {
                            e.fieldid,
                            value = e.get_value_translated(IidCulture.CurrentLanguageId),
                            e.sort_key
                        })
                        .OrderBy(e => e.sort_key)
                        .ToDictionary(e => e.fieldid, e => e.value);
                }
            }
        }
    }
}