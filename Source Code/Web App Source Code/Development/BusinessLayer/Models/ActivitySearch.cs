using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using activity = IID.BusinessLayer.Globalization.Activity.Resource;
using common = IID.BusinessLayer.Globalization.Common.Resource;
using IID.BusinessLayer.Helpers;

namespace IID.BusinessLayer.Models
{
    public class ActivitySearchCriteria
    {
        public ActivitySearchCriteria() { }

        public int UserId { get; set; }
        public byte LanguageId { get; set; }

        [Display(Name = "Countries", ResourceType = typeof(common))]
        public int[] CountryIds { get; set; }

        [Display(Name = "TechnicalAreas", ResourceType = typeof(activity))]
        public string[] TechnicalAreaFieldIds { get; set; }

        [Display(Name = "TechnicalAreaSubtypes", ResourceType = typeof(activity))]
        public string[] TechnicalAreaSubtypeFieldIds { get; set; }

        [Display(Name = "Status", ResourceType = typeof(common))]
        public string Status { get; set; }

        [ScriptIgnore]
        public SelectList Countries { get { return SelectLists.GetCountries(UserId, LanguageId, false); } }

        [ScriptIgnore]
        public SelectList Statuses
        {
            get
            {
                Dictionary<string, string> items = new Dictionary<string, string>();
                items.Add("All", ("[" + activity.StatusAll + "]"));
                items.Add("Active", activity.StatusActive);
                items.Add("Completed", activity.StatusCompleted);
                return new SelectList(items, "Key", "Value");
            }
        }
    }

    public class ActivitySearchResult
    {
        public int ActivityId { get; set; }
        [Display(Name = "Activity", ResourceType = typeof(common))]
        public string ActivityName { get; set; }

        public int CountryId { get; set; }
        [Display(Name = "Country", ResourceType = typeof(common))]
        public string CountryName { get; set; }

        public int ProjectId { get; set; }
        [Display(Name = "Project", ResourceType = typeof(common))]
        public string ProjectName { get; set; }

        [Display(Name = "TechnicalAreaValue", ResourceType = typeof(activity))]
        public string TechnicalAreaValue { get; set; }

        [Display(Name = "TechnicalAreaSubtypeValues", ResourceType = typeof(activity))]
        public ICollection<string> TechnicalAreaSubtypeValues { get; set; }

        [Display(Name = "StartDate", ResourceType = typeof(activity))]
        [DataType(DataType.Date)]
        public string StartDate { get; set; }

        [Display(Name = "EndDate", ResourceType = typeof(activity))]
        [DataType(DataType.Date)]
        public string EndDate { get; set; }
    }
}