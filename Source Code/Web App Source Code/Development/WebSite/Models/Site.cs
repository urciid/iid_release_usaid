using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.Mvc;

using IID.BusinessLayer.Domain;
using common = IID.BusinessLayer.Globalization.Common.Resource;
using error = IID.BusinessLayer.Globalization.Error.Resource;
using site = IID.BusinessLayer.Globalization.Site.Resource;
using IID.BusinessLayer.Helpers;
using IID.BusinessLayer.Identity;
using IID.WebSite.Helpers;

namespace IID.WebSite.Models
{
    public class Site
    {
        public Site() { }

        public Site(int siteId)
        {
            using (Entity context = new Entity())
            {
                v_site site = context.v_site.Where(e => e.site_id == siteId).FirstOrDefault();
                if (site == null)
                    throw new ArgumentException("Invalid siteId: " + siteId);

                SetProperties(site, true);
            }
        }

        public Site(v_site site)
        {
            SetProperties(site, false);
        }

        private void SetProperties(v_site site, bool loadChildren)
        {
            SiteId = site.site_id;
            SiteName = site.name;
            CountryId = site.country_id ?? 0;
            CountryName = site.country_name;
            SiteTypeFieldId = site.site_type_fieldid;
            SiteTypeValue = site.site_type_value;
            AdministrativeDivisionId1 = site.administrative_division_id_1;
            AdministrativeDivisionName1 = site.administrative_division_name_1;
            AdministrativeDivisionType1 = site.administrative_division_type_1;
            AdministrativeDivisionId2 = site.administrative_division_id_2;
            AdministrativeDivisionName2 = site.administrative_division_name_2;
            AdministrativeDivisionType2 = site.administrative_division_type_2;
            AdministrativeDivisionId3 = site.administrative_division_id_3;
            AdministrativeDivisionName3 = site.administrative_division_name_3;
            AdministrativeDivisionType3 = site.administrative_division_type_3;
            AdministrativeDivisionId4 = site.administrative_division_id_4;
            AdministrativeDivisionName4 = site.administrative_division_name_4;
            AdministrativeDivisionType4 = site.administrative_division_type_4;
            FundingTypeFieldId = site.funding_type_fieldid;
            FundingTypeValue = site.funding_type_value;
            Partner = site.partner;
            Longitude = site.longitude;
            Latitude = site.latitude;
            QIIndexScoreFieldId = site.qi_index_score_fieldid;
            QIIndexScoreValue = site.qi_index_score_value;
            PopulationDensityFieldId = site.rural_urban_fieldid;
            PopulationDensityValue = site.rural_urban_value;
            OtherKeyInformation = site.other_key_information;
            Active = site.active;

            if (loadChildren)
            {
                using (Entity context = new Entity())
                {
                    IsFavorite = context.t_user.Find(Identity.CurrentUser.Id).favorite_sites?.Select(e => e.site_id).Contains(site.site_id) ?? false;

                    t_site t_site = context.t_site.Find(site.site_id);

                    ICollection<t_note> notes = t_site.notes;
                    Notes = notes.Select(e => new Note(e)).ToArray();

                    ICollection<t_activity> activities =
                        t_site
                            .activities
                            .Select(e => e.activity)
                            .Where(e => (e.active.HasValue && e.active.Value) ||
                                        (e.createdby_userid == Identity.CurrentUser.Id) ||
                                        (Identity.CurrentUser.IsInRole(Role.SystemAdministrator)))
                            .ToArray();
                    Activities = activities.Select(e => new Activity(e)).ToArray();
                }
            }
        }

        public int? SiteId { get; set; }
        [AllowHtml]
        [StringLength(100)]
        [Required(ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        [Display(Name = "Name", ResourceType = typeof(common))]
        public string SiteName { get; set; }

        [Required(ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        public int CountryId { get; set; }
        [Display(Name = "Country", ResourceType = typeof(common))]
        public string CountryName { get; set; }

        [Required(ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        public string SiteTypeFieldId { get; set; }
        [Display(Name = "Type", ResourceType = typeof(site))]
        public string SiteTypeValue { get; set; }

        [Required(ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        public int? AdministrativeDivisionId1 { get; set; }
        public string AdministrativeDivisionName1 { get; set; }
        public string AdministrativeDivisionType1 { get; set; }

        [Required(ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        public int? AdministrativeDivisionId2 { get; set; }
        public string AdministrativeDivisionName2 { get; set; }
        public string AdministrativeDivisionType2 { get; set; }

        public int? AdministrativeDivisionId3 { get; set; }
        public string AdministrativeDivisionName3 { get; set; }
        public string AdministrativeDivisionType3 { get; set; }

        public int? AdministrativeDivisionId4 { get; set; }
        public string AdministrativeDivisionName4 { get; set; }
        public string AdministrativeDivisionType4 { get; set; }

        public string FundingTypeFieldId { get; set; }
        [Display(Name = "FundingType", ResourceType = typeof(site))]
        public string FundingTypeValue { get; set; }

        [Display(Name = "Partner", ResourceType = typeof(site))]
        public string Partner { get; set; }

        [Display(Name = "Longitude", ResourceType = typeof(site))]
        public string Longitude { get; set; }

        [Display(Name = "Latitude", ResourceType = typeof(site))]
        public string Latitude { get; set; }

        public string QIIndexScoreFieldId { get; set; }
        [Display(Name = "QIIndexScore", ResourceType = typeof(site))]
        public string QIIndexScoreValue { get; set; }

        public string PopulationDensityFieldId { get; set; }
        [Display(Name = "PopulationDensity", ResourceType = typeof(site))]
        public string PopulationDensityValue { get; set; }

        [Display(Name = "OtherKeyInformation", ResourceType = typeof(site))]
        public string OtherKeyInformation { get; set; }

        [UIHint("Active")]
        [Display(Name = "Status")]
        [ScriptIgnore]
        public bool? Active { get; set; }

        [Display(Name = "Activities", ResourceType = typeof(common))]
        [ScriptIgnore]
        public ICollection<Activity> Activities { get; set; }

        [Display(Name = "Notes", ResourceType = typeof(common))]
        [ScriptIgnore]
        public ICollection<Note> Notes { get; set; }

        [ScriptIgnore]
        public bool IsFavorite { get; set; }

        [ScriptIgnore]
        public SelectList Countries
        {
            get
            {
                return SelectLists.GetCountries(
                    Identity.CurrentUser.Id, IidCulture.CurrentLanguageId, true);
            }
        }

        [ScriptIgnore]
        public SelectList FundingTypes
        {
            get
            {
                return SelectLists.GetFromDatabaseModel<t_fieldid>(
                    "value", "fieldid", e => e.parent_fieldid == FieldIdParentTypes.FundingType, e => e.value, true);
            }
        }

        [ScriptIgnore]
        public SelectList QIIndexScores
        {
            get
            {
                return SelectLists.GetFromDatabaseModel<t_fieldid>(
                    "value", "fieldid", e => e.parent_fieldid == FieldIdParentTypes.QIIndexScore, e => e.value, true);
            }
        }

        [ScriptIgnore]
        public SelectList PopulationDensities
        {
            get
            {
                return SelectLists.GetFromDatabaseModel<t_fieldid>(
                    "value", "fieldid", e => e.parent_fieldid == FieldIdParentTypes.PopulationDensity, e => e.value, true);
            }
        }
    }
}