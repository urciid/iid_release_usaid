using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using IID.BusinessLayer.Domain;
using IID.WebSite.Helpers;

namespace IID.WebSite.Models
{
    public enum UserAccessType
    {
        Activity, Country, AdministratitveDivision, Site
    }

    public enum UserAccessLevel
    {
        None, View, Update
    }

    public class UserAccess
    {
        #region Constructors

        public UserAccess() { }

        public UserAccess(int userAccessId)
        {
            using (Entity db = new Entity())
            {
                t_user_access entity = db.t_user_access.Find(userAccessId);
                if (entity == null)
                    throw new ArgumentException("Invalid userAccessId: " + userAccessId.ToString());
                SetProperties(entity);
            }
        }

        public UserAccess(t_user_access entity)
        {
            SetProperties(entity);
        }

        #endregion

        #region Properties

        public int? UserAccessId { get; set; }

        public int UserId { get; set; }
        public string UserFullName { get; set; }

        public UserAccessType AccessType { get; set; }

        public int? Id { get; set; }
        public string Name { get; set; }

        public UserAccessLevel AccessLevel { get; set; }

        #endregion
 
        #region Private Methods
        
        private void SetProperties(t_user_access entity )
        {
            byte languageId = IidCulture.CurrentLanguageId;

            UserAccessId = entity.user_access_id;
            UserId = entity.user_id;
            UserFullName = entity.user.full_name;

            if (entity.activity_id.HasValue)
            {
                AccessType = UserAccessType.Activity;
                Id = entity.activity_id;
                Name = entity.t_activity.get_name_translated(languageId);
            }
            else if (entity.country_id.HasValue)
            {
                AccessType = UserAccessType.Country;
                Id = entity.country_id;
                Name = entity.t_country.get_name_translated(languageId);
            }
            else if (entity.administrative_division_id.HasValue)
            {
                AccessType = UserAccessType.AdministratitveDivision;
                Id = entity.administrative_division_id;
                Name = entity.t_administrative_division.get_name_translated(languageId);
            }
            else if (entity.site_id.HasValue)
            {
                AccessType = UserAccessType.Site;
                Id = entity.site_id;
                Name = entity.t_site.name;
            }

            AccessLevel = entity.update_access ? UserAccessLevel.Update :
                          entity.view_access ?   UserAccessLevel.View :
                                                 UserAccessLevel.None;
        }

        #endregion
    }
}