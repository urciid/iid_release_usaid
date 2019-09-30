using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

using IID.BusinessLayer.Domain;

namespace IID.BusinessLayer.Helpers
{
    public static class SelectLists
    {
        public static SelectList GetUserProjects(int userId, byte languageId, bool addBlank)
        {
            var projects =
                UserAssignedObjects.GetProjects(userId, languageId).OrderBy(p => p.name)
                    .Select(x => new { project_id = x.project_id.ToString(), x.name }).ToList();
            if (addBlank)
                projects.Insert(0, new { project_id = "", name = "" });
            return new SelectList(projects, "project_id", "name");
        }

        public static SelectList GetCountries(int userId, byte languageId, bool addBlank)
        {
            var countries =
                UserAssignedObjects.GetCountries(userId, languageId).OrderBy(c => c.name)
                    .Select(x => new { country_id = x.country_id.ToString(), x.name }).ToList();
            if (addBlank)
                countries.Insert(0, new { country_id = "", name = "" });
            return new SelectList(countries, "country_id", "name");
        }

        public static SelectList GetUsers(ICollection<int> statuses)
        {
            using (Entity db = new Entity())
            {
                var users = db.t_user.AsQueryable();
                if (statuses != null)
                    users = users.Where(u => u.user_status_id.HasValue && statuses.Contains(u.user_status_id.Value));

                return new SelectList(users.ToList()
                                        .OrderBy(u => u.first_name)
                                        .ThenBy(u => u.last_name)
                                        .ToList(),
                                      "user_id", "full_name");
            }
        }

        public static SelectList GetFieldIdSelectList(string parentFieldId, bool? active)
        {
            using (Entity db = new Entity())
            {
                var results = db.t_fieldid.Where(e => e.parent_fieldid == parentFieldId);
                if (active.HasValue)
                    results = results.Where(ta => ta.active == active);
                // Order by sort_key (numeric) if present, or fall back to value (alphabetical).
                var list = results.OrderBy(e => e.sort_key).ThenBy(e => e.value).ToList();
                return new SelectList(list, "fieldid", "value");
            }
        }

        public static SelectList GetFieldIdSelectListRequestUserRole(string parentFieldId, bool? active, int CurrentUserId)
        {
            using (Entity db = new Entity())
            {
                // Only current logical way in here is either Sysadmin, Country Director, or Activity Leader
                // If Activity leader, can only request Site Manager role
                // This will need to be modified if other roles can request users
                var userActivityLeader = db.t_user_role.Where(e => e.user_role_fieldid == "rolatl" && e.user_id == CurrentUserId).ToList();

                if (userActivityLeader.Count > 0)
                {                                       
                    var results = db.t_fieldid.Where(e => e.parent_fieldid == parentFieldId && e.fieldid == "rolmgr");
                    if (active.HasValue)
                        results = results.Where(ta => ta.active == active);
                    // Order by sort_key (numeric) if present, or fall back to value (alphabetical).
                    var list = results.OrderBy(e => e.sort_key).ThenBy(e => e.value).ToList();
                    return new SelectList(list, "fieldid", "value");
                }
                else
                {
                    var resultsadmin = db.t_fieldid.Where(e => e.parent_fieldid == parentFieldId);
                    if (active.HasValue)
                        resultsadmin = resultsadmin.Where(ta => ta.active == active);
                    // Order by sort_key (numeric) if present, or fall back to value (alphabetical).
                    var listadmin = resultsadmin.OrderBy(e => e.sort_key).ThenBy(e => e.value).ToList();
                    return new SelectList(listadmin, "fieldid", "value");


                }
 
            }
        }

        public static SelectList GetSites(Expression<Func<v_site, bool>> predicate, bool addEmptyOption)
        {
            using (Entity db = new Entity())
            {
                var sites = db.Set<v_site>().AsEnumerable();
                if (predicate != null)
                    sites = sites.Where(predicate.Compile());

                var items = sites.Select(e => new
                {
                    SiteId = (int?)e.site_id,
                    SiteName = String.Concat(
                                        e.country_name, " / ",
                                        e.administrative_division_name_1, " / ",
                                        e.administrative_division_name_2 ?? "null", " / ",
                                        e.administrative_division_name_3 ?? "null", " / ",
                                        e.administrative_division_name_4 ?? "null", " / ",
                                        e.name).Replace("null / ", "")
                })
                    .OrderBy(e => e.SiteName)
                    .ToList();
                if (addEmptyOption)
                    items.Insert(0, new { SiteId = (int?)null, SiteName = String.Empty });

                return new SelectList(items, "SiteId", "SiteName");
            }
        }

        public static SelectList GetFromDatabaseModel<TEntity>(
            string textField, string valueField, Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> keySelector, bool addEmptyOption = false) where TEntity : class
        {
            using (Entity db = new Entity())
            {
                var entity = db.Set<TEntity>().AsEnumerable();
                if (predicate != null)
                    entity = entity.Where(predicate.Compile());
                if (keySelector != null)
                    entity = entity.OrderBy(keySelector.Compile());

                var items = entity.ToList();
                if (addEmptyOption)
                    items.Insert(0, default(TEntity));

                return new SelectList(items, valueField, textField);
            }
        }
    }
}