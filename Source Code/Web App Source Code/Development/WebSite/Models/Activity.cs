using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using IID.BusinessLayer.Domain;
using activity = IID.BusinessLayer.Globalization.Activity.Resource;
using common = IID.BusinessLayer.Globalization.Common.Resource;
using error = IID.BusinessLayer.Globalization.Error.Resource;
using IID.BusinessLayer.Helpers;

namespace IID.WebSite.Models
{
    public class Activity : Base
    {
        public Activity() { }

        public Activity(t_activity activity)
        {
            SetProperties(activity, false, false);
        }

        public Activity(int activityId, bool loadChildren)
        {
            t_activity activity = Context.t_activity.FirstOrDefault(a => a.activity_id == activityId);
            if (activity == null)
                throw new ArgumentException("Invalid activityId: " + activityId.ToString());

            SetProperties(activity, loadChildren, true);
        }

        private void SetProperties(t_activity activity, bool loadChildren, bool translate)
        {
            ActivityId = activity.activity_id;
            ProjectId = activity.project_id;
            CountryId = activity.country_id;
            StartDate = activity.start_date;
            EndDate = activity.end_date;
            PrimaryManagerUserId = activity.primary_manager_userid;
            PrimaryManagerName = activity.primary_manager?.full_name;
            FunderFieldId = activity.funder_fieldid;
            FunderValue = activity.funder?.value;
            TechnicalAreaFieldId = activity.technical_area_fieldid;
            TechnicalAreaValue = activity.technical_area?.value;
            AdditionalManagerUserIds = activity.additional_managers.Select(m => m.user_id.ToString()).ToList();
            AdditionalManagerNames = activity.additional_managers.Select(m => m.full_name).ToList();
            TechnicalAreaSubtypeFieldIds = activity.technical_area_subtypes.Select(tast => tast.fieldid).ToList();
            TechnicalAreaSubtypeValues = activity.technical_area_subtypes.Select(tast => tast.value).ToList();
            Active = activity.active;

            if (translate)
            {
                Name = activity.get_name_translated(CurrentLanguageId);
                OtherKeyInformation = activity.get_other_key_information_translated(CurrentLanguageId);
            }
            else
            {
                Name = activity.name;
                OtherKeyInformation = activity.other_key_information;
            }

            if (loadChildren)
            {
                Aims = UserAssignedObjects.GetAims(activity.aims, CurrentUser).Select(a => new Aim(a, loadChildren, translate)).ToList();
                Notes = activity.notes.Select(n => new Note(n)).ToList();
                IsFavorite = activity.user_favorites.Select(e => e.user_id).Contains(CurrentUser.Id);
                HasObservations = activity.aims?.SelectMany(a => a.indicators?.SelectMany(i => i.observations))?.Any() ?? false;
            }
        }

        public int? ActivityId { get; set; }

        [AllowHtml]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        [StringLength(100)]
        [Display(Name = "Name", ResourceType = typeof(common))]
        public string Name { get; set; }

        [Display(Name = "Project", ResourceType = typeof(common))]
        [Required(ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        public int ProjectId { get; set; }

        [ScriptIgnore]
        private Project _project;
        [ScriptIgnore]
        public Project Project
        {
            get
            {
                if (_project == null)
                    _project = new Project(ProjectId);
                return _project;
            }
        }

        [ScriptIgnore]
        public SelectList Projects
        {
            get
            {
                return SelectLists.GetUserProjects(CurrentUser.Id, CurrentLanguageId, true);
            }
        }

        [Display(Name = "Country", ResourceType = typeof(common))]
        [Required(ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        public int CountryId { get; set; }

        [ScriptIgnore]
        private Country _country;
        [ScriptIgnore]
        public Country Country
        {
            get
            {
                if (_country == null)
                    _country = new Country(CountryId);
                return _country;
            }
        }

        [DataType(DataType.Date)]
        [Display(Name = "StartDate", ResourceType = typeof(activity))]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? StartDate { get; set; }

        [ScriptIgnore]
        public bool HasObservations { get; private set; }

        [DataType(DataType.Date)]
        [Display(Name = "EndDate", ResourceType = typeof(activity))]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        public int? PrimaryManagerUserId { get; set; }

        [Display(Name = "PrimaryManager", ResourceType = typeof(activity))]
        [ScriptIgnore]
        public string PrimaryManagerName { get; set; }

        [StringLength(6)]
        public string FunderFieldId { get; set; }

        [Display(Name = "Funder", ResourceType = typeof(activity))]
        [ScriptIgnore]
        public string FunderValue { get; set; }

        [AllowHtml]
        [Display(Name = "OtherKeyInformation", ResourceType = typeof(activity))]
        public string OtherKeyInformation { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        [StringLength(6)]
        public string TechnicalAreaFieldId { get; set; }

        [Display(Name = "TechnicalArea", ResourceType = typeof(activity))]
        [ScriptIgnore]
        public string TechnicalAreaValue { get; set; }

        [UIHint("Active")]
        [Display(Name = "Status", ResourceType = typeof(common))]
        public bool? Active { get; set; }

        [ScriptIgnore]
        public ICollection<string> AdditionalManagerUserIds { get; set; }

        [Display(Name = "AdditionalManagers", ResourceType = typeof(activity))]
        [ScriptIgnore]
        public ICollection<string> AdditionalManagerNames { get; set; }

        [ScriptIgnore]
        public ICollection<string> TechnicalAreaSubtypeFieldIds { get; set; }

        [Display(Name = "TechnicalAreaSubtypes", ResourceType = typeof(activity))]
        [ScriptIgnore]
        public ICollection<string> TechnicalAreaSubtypeValues { get; set; }

        [Display(Name = "Aims", ResourceType = typeof(common))]
        [ScriptIgnore]
        public ICollection<Aim> Aims { get; set; }

        [Display(Name = "Notes", ResourceType = typeof(common))]
        [ScriptIgnore]
        public ICollection<Note> Notes { get; set; }

        [ScriptIgnore]
        public bool IsFavorite { get; set; }

        [ScriptIgnore]
        public SelectList CountriesList
        {
            get
            {
                return SelectLists.GetCountries(CurrentUser.Id, CurrentLanguageId, true);
            }
        }

        [ScriptIgnore]
        public SelectList _users = SelectLists.GetUsers(new int[] { 2, 3 });
        [ScriptIgnore]
        public SelectList Users { get { return _users; } }

        [ScriptIgnore]
        public SelectList TechnicalAreas { get { return SelectLists.GetFieldIdSelectList(FieldIdParentTypes.TechnicalArea, null); } }

        [ScriptIgnore]
        public SelectList TechnicalAreaSubtypes { get { return SelectLists.GetFieldIdSelectList(FieldIdParentTypes.TechnicalAreaSubtype, null); } }

        [ScriptIgnore]
        public SelectList Funders
        {
            get
            {
                return SelectLists.GetFromDatabaseModel<t_fieldid>(
                    "value", "fieldid", e => e.parent_fieldid == FieldIdParentTypes.Funder, e => e.value);
            }
        }
    }
}