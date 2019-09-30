using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using common = IID.BusinessLayer.Globalization.Common.Resource;
using site = IID.BusinessLayer.Globalization.Site.Resource;

namespace IID.BusinessLayer.Models
{
    public class SiteSearchCriteria
    {
        public int? CountryId { get; set; }
        [Display(Name = "Country", ResourceType = typeof(common))]
        public string CountryName { get; set; }

        public int? AdministrativeDivisionId1 { get; set; }
        public string AdministrativeDivisionName1 { get; set; }

        public int? AdministrativeDivisionId2 { get; set; }
        public string AdministrativeDivisionName2 { get; set; }

        public int? AdministrativeDivisionId3 { get; set; }
        public string AdministrativeDivisionName3 { get; set; }

        public int? AdministrativeDivisionId4 { get; set; }
        public string AdministrativeDivisionName4 { get; set; }

        public string SiteTypeFieldId { get; set; }
        [Display(Name = "Type", ResourceType = typeof(site))]
        public string SiteTypeValue { get; set; }

        public byte LanguageId { get; set; }
    }

    public class SiteSearchResult
    {
        public int SiteId { get; set; }
        [Display(Name = "Site", ResourceType = typeof(common))]
        public string SiteName { get; set; }

        public int? CountryId { get; set; }
        [Display(Name = "Country", ResourceType = typeof(common))]
        public string CountryName { get; set; }

        public int? AdministrativeDivisionId1 { get; set; }
        public string AdministrativeDivisionName1 { get; set; }

        public int? AdministrativeDivisionId2 { get; set; }
        public string AdministrativeDivisionName2 { get; set; }

        public int? AdministrativeDivisionId3 { get; set; }
        public string AdministrativeDivisionName3 { get; set; }

        public int? AdministrativeDivisionId4 { get; set; }
        public string AdministrativeDivisionName4 { get; set; }

        [Display(Name = "Type", ResourceType = typeof(site))]
        public string SiteTypeValue { get; set; }

        public string QIIndexScore { get; set; }
    }
}