using System;
using System.Linq;
using System.Web.Mvc;

using IID.BusinessLayer.Domain;
using IID.BusinessLayer.Helpers;
using IID.WebSite.Helpers;

namespace IID.WebSite.Controllers
{
    [NoCache]
    public class UserAccessController : BaseController
    {
        [HttpPost()]
        [ValidateAntiForgeryToken]
        public JsonResult Add(v_user_access model)
        {
            if (ModelState.IsValid)
            {
                using (Entity db = new Entity())
                {
                    try
                    {
                        t_user_access ua = new t_user_access();
                        SetCoreFields(ref ua, model);
                        ua.createdby_userid = CurrentUser.Id;
                        ua.created_date = DateTime.Now;
                        db.t_user_access.Add(ua);
                        db.SaveChanges();

                        return Json(db.v_user_access.Find(ua.user_access_id));
                    }
                    catch (Exception ex)
                    {
                        return GetJsonResult(false, "An error occurred: " + ex.ToString());
                    }
                }
            }
            else
            {
                return GetJsonResult(false, "Model not valid.");
            }
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(v_user_access model)
        {
            if (ModelState.IsValid)
            {
                using (Entity db = new Entity())
                {
                    try
                    {
                        t_user_access ua = db.t_user_access.Find(model.user_access_id);
                        if (ua == null)
                            throw new ArgumentException("Invalid userAccessId: " + model.user_access_id.ToString());

                        SetCoreFields(ref ua, model);
                        ua.updatedby_userid = CurrentUser.Id;
                        ua.updated_date = DateTime.Now;
                        db.SaveChanges();

                        return Json(db.v_user_access.Find(ua.user_access_id));
                    }
                    catch (Exception ex)
                    {
                        return GetJsonResult(false, "An error occurred: " + ex.ToString());
                    }
                }
            }
            else
            {
                return GetJsonResult(false, "Model not valid.");
            }
        }

        private void SetCoreFields(ref t_user_access entity, v_user_access model)
        {
            entity.user_id = model.user_id;
            entity.activity_id = model.activity_id;
            entity.country_id = model.country_id;
            entity.administrative_division_id = model.administrative_division_id;
            entity.site_id = model.site_id;
            entity.view_access = model.view_access;
            entity.update_access = model.update_access;
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            using (Entity db = new Entity())
            {
                try
                {
                    t_user_access ua = db.t_user_access.Find(id);
                    db.t_user_access.Remove(ua);
                    db.SaveChanges();
                    return GetJsonResult(true, null);
                }
                catch (Exception ex)
                {
                    return GetJsonResult(false, "An error occurred: " + ex.ToString());
                }
            }
        }

        [HttpGet]
        public JsonResult GetUserCountries(int userId, AccessLevel accessLevel)
        {
            using (Entity db = new Entity())
            {
                var existing = UserAccess.GetExistingUserAccess(userId, AccessType.Country, accessLevel);
                bool include = accessLevel != AccessLevel.None;

                var countries = db.t_country
                    .Where(e => e.active == true && existing.Contains(e.country_id) == include)
                    .ToList()
                    .Select(e => new
                    {
                        e.country_id,
                        name = e.get_name_translated(IidCulture.CurrentLanguageId)
                    })
                    .OrderBy(x => x.name);

                return Json(countries, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetActivityCountry(int activityId)
        {
            using (Entity db = new Entity())
            {
                var existing = db.t_activity.Find(activityId);
                var country_id = existing.country_id;
                var countries = db.t_country.Where(e => e.active == true && e.country_id == country_id)
                    .ToList()
                    .Select(e => new
                    {
                        e.country_id,
                        name = e.get_name_translated(IidCulture.CurrentLanguageId)
                    });                   

                return Json(countries, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetUserActivities(int userId, int? countryId, AccessLevel accessLevel)
        {
            using (Entity db = new Entity())
            {
                var existing = UserAccess.GetExistingUserAccess(userId, AccessType.Activity, accessLevel);
                bool include = accessLevel != AccessLevel.None;

                var activities = db.t_activity
                    .Where(e => e.active == true && existing.Contains(e.activity_id) == include &&
                            (!countryId.HasValue || e.country_id == countryId.Value))
                    .ToList()
                    .Select(e => new
                    {
                        e.activity_id,
                        name = e.get_name_translated(IidCulture.CurrentLanguageId)
                    })
                    .OrderBy(x => x.name);

                return Json(activities, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetUserAdministrativeDivisions(int userId, int countryId, AccessLevel accessLevel)
        {
            using (Entity db = new Entity())
            {
                var existing = UserAccess.GetExistingUserAccess(userId, AccessType.AdministrativeDivision, accessLevel);
                bool include = accessLevel != AccessLevel.None;

                var divisions = db.v_administrative_division
                    .Where(e => existing.Contains(e.administrative_division_id) == include && e.country_id == countryId)
                    .Select(e => new
                    {
                        e.administrative_division_id,
                        e.name,
                        e.concatenated_name
                    })
                    .OrderBy(x => x.concatenated_name)
                    .ToList();

                return Json(divisions, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetUserSites(int userId, int countryId, AccessLevel accessLevel)
        {
            using (Entity db = new Entity())
            {
                var existing = UserAccess.GetExistingUserAccess(userId, AccessType.Site, accessLevel);
                bool include = accessLevel != AccessLevel.None;

                var sites = db.v_site
                    .Where(e => existing.Contains(e.site_id) == include && e.country_id == countryId)
                    .Select(e => new
                    {
                        e.site_id,
                        e.name,
                        e.concatenated_name
                    })
                    .OrderBy(x => x.concatenated_name)
                    .ToList();

                return Json(sites, JsonRequestBehavior.AllowGet);
            }
        }
    }
}