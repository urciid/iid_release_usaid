using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

using IID.BusinessLayer.Domain;
using common = IID.BusinessLayer.Globalization.Common.Resource;
using error = IID.BusinessLayer.Globalization.Error.Resource;

namespace IID.WebSite.Models
{
    public class Aim : Base
    {
        public Aim() { }

        public Aim(int activityId)
        {
            t_activity activity = Context.t_activity.Find(activityId);
            if (activity == null)
                throw new ArgumentException("Invalid activityId: " + activityId.ToString());

            ActivityId = activityId;
            ActivityAims =
                UserAssignedObjects.GetAims(activity.aims, CurrentUser)
                    .ToDictionary(e => e.aim_id, e => e.get_name_translated(CurrentLanguageId));
        }

        public Aim(int aimId, bool loadChildren)
        {
            t_aim aim = Context.t_aim.Find(aimId);
            if (aim == null)
                throw new ArgumentException("Invalid aimId: " + aimId.ToString());

            SetProperties(aim, loadChildren, true);
        }

        public Aim(t_aim aim, bool loadChildren, bool translate)
        {
            SetProperties(aim, loadChildren, translate);
        }

        private void SetProperties(t_aim aim, bool loadChildren, bool translate)
        {
            ActivityId = aim.activity_id;
            AimId = aim.aim_id;
            Active = aim.active;
            Sort = aim.sort;

            if (translate)
            {
                Name = aim.get_name_translated(CurrentLanguageId);
            }
            else
            {
                Name = aim.name;
            }

            if (loadChildren)
            {
                ActivityAims = UserAssignedObjects.GetAims(aim.activity.aims, CurrentUser).ToDictionary(e => e.aim_id, e => e.get_name_translated(CurrentLanguageId));
                Indicators = UserAssignedObjects.GetIndicators(aim.indicators, CurrentUser).Select(i => new Indicator(i, false, true)).ToList();
            }
        }

        public int ActivityId { get; set; }

        public int? AimId { get; set; }

        [AllowHtml]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        [StringLength(250)]
        [Display(Name = "Name", ResourceType = typeof(common))]
        public string Name { get; set; }

        [UIHint("Active")]
        [Display(Name = "Status")]
        public bool? Active { get; set; }

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

        public Dictionary<int, string> ActivityAims { get; set; }

        public ICollection<Indicator> Indicators { get; set; }
    }

    public class AimCollection
    {
        public AimCollection(int activityId, ICollection<Aim> aims)
        {
            ActivityId = activityId;
            Aims = aims;
        }

        public int ActivityId { get; private set; }

        public ICollection<Aim> Aims { get; set; }
    }
}