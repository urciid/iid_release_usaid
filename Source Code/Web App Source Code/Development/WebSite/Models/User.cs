using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

using IID.BusinessLayer.Domain;
using error = IID.BusinessLayer.Globalization.Error.Resource;
using common = IID.BusinessLayer.Globalization.Common.Resource;
using IID.BusinessLayer.Helpers;
using IID.WebSite.Helpers;

namespace IID.WebSite.Models
{
    public class User
    {
        public User()
        {
            AllSites = SelectLists.GetSites(e => e.active == true, true);
        }

        public User(int userId)
        {
            using (Entity db = new Entity())
            {
                byte languageId = IidCulture.CurrentLanguageId;

                t_user user = db.t_user.Find(userId);

                if (user == null)
                    throw new ArgumentException("Invalid userId: " + UserId);

                v_user[] roles = db.v_user.Where(e => e.user_id == user.user_id).ToArray();

                v_site site = null;
                if (user.site_id.HasValue)
                    site = db.v_site.Find(user.site_id.Value);

                UserId = user.user_id;
                OrganizationId = user.organization_id;
                Email = user.email;
                UserStatusId = user.user_status_id;
                SiteId = user.site_id;
                SiteName = site?.get_name_translated(languageId);
                FirstName = user.first_name;
                LastName = user.last_name;
                FullName = user.first_name + " " + user.last_name + " (" + user.email + ")";
                Phone = user.phone;
                Title = user.title;
                LastLogin = user.last_login_activity;
                CreatedByUserId = user.createdby_userid;
                CreatedDate = user.created_date;
                UpdatedDate = user.updated_date;
                UpdatedByUserId = user.updatedby_userid;
                UserRoleFieldIds = roles.Select(e=>e.user_role_fieldid).ToArray();
                UserRoleValues = roles.Select(e => e.user_role_fieldid_value).ToArray();
                Active = user.active;

                var userAccess = db.v_user_access.Where(e => e.user_id == userId);
                AssignedActivities = userAccess.Where(e => e.access_level_type == "Activity").OrderBy(e => e.activity_name).ToArray();
                AssignedCountries = userAccess.Where(e => e.access_level_type == "Country").OrderBy(e => e.country).ToArray();
                AssignedRegions = userAccess.Where(e => e.access_level_type == "Regional").OrderBy(e => e.access_level_name).ToArray();
                AssignedSites = userAccess.Where(e => e.access_level_type == "Site").OrderBy(e => e.site_name).ToArray();

                // Get all active sites. If the user is assigned to an inactive site, include it, too!
                int inactiveSiteId = (site != null && site.active == false) ? site.site_id : 0;
                Expression<Func<v_site, bool>> predicate = e => e.active == true || e.site_id == inactiveSiteId;
                AllSites = SelectLists.GetSites(predicate, true);
            }
        }

        public int? UserId { get; set; }

        public int? OrganizationId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        [StringLength(250)]
        public string Email { get; set; }

        public int? UserStatusId { get; set; }

        [Display(Name = "Site")]
        public int? SiteId { get; set; }
        public string SiteName { get; set; }

        [Display(Name = "Role")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        public ICollection<string> UserRoleFieldIds { get; set; }
        public ICollection<string> UserRoleValues { get; set; }

        [Display(Name = "First Name")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        [StringLength(100)]
        public string LastName { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "Phone")]
        [StringLength(100)]
        public string Phone { get; set; }

        [Display(Name = "Title")]
        [StringLength(100)]
        public string Title { get; set; }

        [Display(Name = "Last Login")]
        public DateTime? LastLogin { get; set; }

        public int CreatedByUserId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? UpdatedByUserId { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [UIHint("Active")]
        [Display(Name = "Status")]
        public bool? Active { get; set; }

        [Display(Name = "Countries", ResourceType = typeof(common))]
        public ICollection<v_user_access> AssignedCountries { get; set; }

        [Display(Name = "Activities", ResourceType = typeof(common))]
        public ICollection<v_user_access> AssignedActivities { get; set; }

        [Display(Name = "AdministrativeDivisions", ResourceType = typeof(common))]
        public ICollection<v_user_access> AssignedRegions { get; set; }

        [Display(Name = "Sites", ResourceType = typeof(common))]
        public ICollection<v_user_access> AssignedSites { get; set; }

        public SelectList AllSites { get; set; }
    }
}