using System;
using System.Collections.Generic;
using System.Linq;

using IID.BusinessLayer.Domain;

namespace IID.WebSite.Models
{
    public enum Mode { RankByImprovement, Observation, Chart }

    public class ActivityTree : Base
    {
        public ActivityTree(int activityId, int selectedIndicatorId, int? siteId, Mode mode)
        {
            t_activity activity = Context.t_activity.Find(activityId);
            if (activity == null)
                throw new ArgumentException("Invalid activityId: " + activityId.ToString());

            // NOTE: Don't use UserAssignedObjects here. Only show active aims/indicators.
            Activity = new Activity(activityId, true);
            Entities = activity.aims.Where(e => e.active == true).OrderBy(e => e.sort).Select(a => new ActivityTreeEntity()
            {
                EntityType = "Aim",
                EntityId = a.aim_id,
                EntityName = a.get_name_translated(CurrentLanguageId),
                Children = a.indicators.OrderBy(e => e.sort).Where(e => e.active == true).Select(i => new ActivityTreeEntity()
                {
                    EntityType = "Indicator",
                    EntityId = i.indicator_id,
                    EntityName = i.name
                }).ToList()
            })
            .OrderBy(x => x.EntityName)
            .ToList();
            SelectedIndicatorId = selectedIndicatorId;
            SiteId = siteId;

            Mode = mode;
        }

        public ActivityTree(int activityId, ICollection<ActivityTreeEntity> entities, int selectedIndicatorId, int? siteId, Mode mode)
        {
            Activity = new Activity(activityId, true);
            Entities = entities;
            SelectedIndicatorId = selectedIndicatorId;
            SiteId = siteId;
            Mode = mode;
        }

        public Activity Activity { get; private set; }

        public ICollection<ActivityTreeEntity> Entities { get; set; }

        public int SelectedIndicatorId { get; set; }

        public int? SiteId { get; set; }

        public Mode Mode { get; set; }
    }

    public class ActivityTreeEntity
    {
        public string EntityType { get; set; }

        public int EntityId { get; set; }

        public string EntityName { get; set; }

        public ICollection<ActivityTreeEntity> Children { get; set; }
    }
}