using System;
using System.Linq;
using System.Web.Mvc;

using IID.BusinessLayer.Domain;
using IID.BusinessLayer.Globalization;
using common = IID.BusinessLayer.Globalization.Common.Resource;
using IID.BusinessLayer.Models;
using IID.WebSite.Helpers;
using IID.WebSite.Models;
using System.Data;
using ClosedXML.Excel;
using System.IO;

namespace IID.WebSite.Controllers
{
    [NoCache]
    public class ActivityController : BaseController
    {
        public ActionResult Add()
        {
            return View(new Activity());
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Activity model)
        {
            if (ModelState.IsValid)
            {
                var activity = SetCoreFields(model);

                return RedirectToAction("View", new { id = activity.activity_id });
            }
            else
            {
                return View(model);
            }
        }

        public ActionResult View(int id)
        {
            return View(new Activity(id, true));
        }

        public ActionResult Edit(int id)
        {
            return View(new Activity(id, true));
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Activity model)
        {
            if (ModelState.IsValid)
            {
                SetCoreFields(model);

                return RedirectToAction("View", new { id = model.ActivityId.Value });
            }
            else
            {
                return View(model);
            }
        }

        private t_activity SetCoreFields(Activity model)
        {
            using (Entity context = new Entity())
            {
                t_activity activity = null;
                if (model.ActivityId.HasValue)
                {
                    activity = context.t_activity.Find(model.ActivityId.Value);
                    if (activity == null)
                        throw new ArgumentException("Invalid activityId: " + model.ActivityId.ToString());
                    activity.updatedby_userid = CurrentUser.Id;
                    activity.updated_date = DateTime.Now;
                }
                else
                {
                    activity = new t_activity();
                    activity.createdby_userid = CurrentUser.Id;
                    activity.created_date = DateTime.Now;
                    context.t_activity.Add(activity);
                }

                var language = IidCulture.CurrentLanguage;

                if (language == Language.English || !model.ActivityId.HasValue)
                {
                    activity.name = model.Name;
                    activity.other_key_information = model.OtherKeyInformation;
                }
                activity.project_id = model.ProjectId;
                activity.country_id = model.CountryId;
                activity.funder_fieldid = model.FunderFieldId;
                activity.start_date = model.StartDate;
                activity.end_date = model.EndDate;
                activity.primary_manager_userid = model.PrimaryManagerUserId;
                activity.technical_area_fieldid = model.TechnicalAreaFieldId;
                activity.active = model.Active;

                if (model.AdditionalManagerUserIds != null)
                    foreach (string id in model.AdditionalManagerUserIds)
                        activity.additional_managers.Add(context.t_user.Find(Convert.ToInt32(id)));

                if (model.TechnicalAreaSubtypeFieldIds != null)
                    foreach (string fieldId in model.TechnicalAreaSubtypeFieldIds)
                        activity.technical_area_subtypes.Add(context.t_fieldid.Find(fieldId));

                context.SaveChanges();

                if (language != Language.English)
                {
                    var languageId = IidCulture.CurrentLanguageId;
                    var userId = CurrentUser.Id;
                    activity.set_name_translated(languageId, model.Name, userId);
                    activity.set_other_key_information_translated(languageId, model.OtherKeyInformation, userId);
                }

                return activity;
            }
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            Response.ContentType = "application/json; charset=utf-8";

            using (Entity context = new Entity())
            {
                try
                {
                    t_activity activity = context.t_activity.Find(id);
                    context.t_activity.Remove(activity);
                    context.SaveChanges();
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, responseText = "An error occurred: " + ex.ToString() });
                }
            }
        }

        public ActionResult RankByImprovement(int activityId, int? indicatorId)
        {
            return View("RankByImprovement", new RankByImprovementData(activityId, indicatorId, IidCulture.CurrentLanguageId));
        }

        public ActionResult Search()
        {
            return View(new ActivitySearchCriteria() { UserId = CurrentUser.Id, LanguageId = IidCulture.CurrentLanguageId });
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public JsonResult AddToFavorites(int id)
        {
            try
            {
                using (Entity context = new Entity())
                {
                    var activity = context.t_activity.Find(id);
                    if (activity == null)
                        throw new ArgumentException("Invalid activityId: " + id.ToString());

                    var user = context.t_user.Find(CurrentUser.Id);

                    if (!activity.user_favorites.Select(e => e.user_id).Contains(user.user_id))
                    {
                        activity.user_favorites.Add(user);
                        context.SaveChanges();
                    }
                }

                return ReturnFavoriteResult(true, common.RemoveFavorite, "/Images/icons/16/star_color.png");
            }
            catch (Exception ex)
            {
                return GetJsonResult(ex);
            }
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public JsonResult RemoveFromFavorites(int id)
        {
            try
            {
                using (Entity context = new Entity())
                {
                    var activity = context.t_activity.Find(id);
                    if (activity == null)
                        throw new ArgumentException("Invalid activityId: " + id.ToString());

                    var user = context.t_user.Find(CurrentUser.Id);

                    if (activity.user_favorites.Select(e => e.user_id).Contains(user.user_id))
                    {
                        activity.user_favorites.Remove(user);
                        context.SaveChanges();
                    }
                }

                return ReturnFavoriteResult(false, common.AddFavorite, "/Images/icons/16/star_gray.png");
            }
            catch (Exception ex)
            {
                return GetJsonResult(ex);
            }
        }

        private JsonResult ReturnFavoriteResult(bool isFavorite, string tooltip, string iconUrl)
        {
            return Json(new
            {
                success = true,
                IsFavorite = isFavorite,
                Tooltip = tooltip,
                IconUrl = iconUrl
            });
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public JsonResult AssignSite(int activityId, int siteId, string waveFieldId)
        {
            using (Entity context = new Entity())
            {
                var activity = context.t_activity.Find(activityId);
                if (activity == null)
                    return GetJsonResult(false, "Invalid activityId: " + activityId.ToString());

                var site = activity.sites.Where(e => e.site_id == siteId).FirstOrDefault();
                if (site != null)
                    return GetJsonResult(false, "Site already assigned! siteId:" + siteId.ToString());

                site = new t_activity_site() { activity_id = activityId, site_id = siteId, wave_fieldid = waveFieldId };
                context.t_activity_site.Add(site);
                context.SaveChanges();

                context.Entry(site).Reference(e => e.wave).Load();
                return Json(new
                {
                    success = true,
                    WaveFieldId = site.wave_fieldid,
                    WaveValue = site.wave.get_value_translated(IidCulture.CurrentLanguageId)
                });
            }
        }


        [HttpPost()]
        [ValidateAntiForgeryToken]
        public JsonResult RemoveSite(int id, int siteId)
        {
            using (Entity context = new Entity())
            {
                var activity = context.t_activity.Find(id);
                if (activity == null)
                    return GetJsonResult(false, "Invalid activityId: " + id.ToString());

                var site = activity.sites.Where(e => e.site_id == siteId).FirstOrDefault();

                var observation = context.t_observation.Where(o => o.site_id == siteId).FirstOrDefault();
                if (observation != null)
                {
                    return GetJsonResult(false, "Site has observations - cannot delete. ");
                }
                else
                {
                    context.t_activity_site.Remove(site);
                    context.SaveChanges();
                    return GetJsonResult(true, null);
                }
            }
        }


        [HttpGet]
        public JsonResult GetUserActivities(int userId, int? countryId, bool hasAccess)
        {
            using (Entity context = new Entity())
            {
                var existing = context.t_user_access.Where(e => e.user_id == userId).Select(e => e.activity_id);
                var activities = context.t_activity
                    .Where(e => e.active == true && existing.Contains(e.activity_id) == hasAccess &&
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetActivityUnsassignedSites(int activityId, int countryId, string siteTypeFieldId)
        {
            using (Entity context = new Entity())
            {
                var activityRecord = context.t_activity.Where(e => e.activity_id == activityId).FirstOrDefault();
                var userRoleAdmin = context.t_user_role.Where(e => e.user_role_fieldid == "roladm" && e.user_id == Identity.CurrentUser.Id).ToList();
                var userRoleCountryDirector = context.t_user_role.Where(e => e.user_role_fieldid == "rolcod" && e.user_id == Identity.CurrentUser.Id).ToList();
                var userRoleActivityLeader = context.t_user_role.Where(e => e.user_role_fieldid == "rolatl" && e.user_id == Identity.CurrentUser.Id).ToList();
                var userCountryAccess = context.t_user_access.Where(e => e.user_id == Identity.CurrentUser.Id).Select(e => e.country_id);
                var userActivityAccess = context.t_user_access.Where(e => e.user_id == Identity.CurrentUser.Id).Select(e => e.activity_id);

                var userAccess = context.t_user_access.Where(e => e.user_id == Identity.CurrentUser.Id).Select(e => e.activity_id);
                var activitySite = context.t_activity.Find(activityId).sites.Select(e => e.site_id);

                var sites = context.v_site
                    .Where(e => e.active == true
                    && e.country_id == countryId
                    && (siteTypeFieldId == "" || e.site_type_fieldid == siteTypeFieldId)
                    && (userAccess.Contains(e.site_id)
                            || userRoleAdmin.Count > 0
                            || ((userRoleCountryDirector.Count > 0 || userRoleActivityLeader.Count > 0)
                                    && (userCountryAccess.Contains(activityRecord.country_id) || userActivityAccess.Contains(activityId))))
                    && !activitySite.Contains(e.site_id))
                    .ToList()
                    .Select(e => new
                    {
                        ActivityId = activityId,
                        SiteId = e.site_id,
                        SiteName = e.name,
                        AdministrativeDivisionId1 = e.administrative_division_id_1,
                        AdministrativeDivisionName1 = e.administrative_division_name_1,
                        AdministrativeDivisionId2 = e.administrative_division_id_2,
                        AdministrativeDivisionName2 = e.administrative_division_name_2,
                        AdministrativeDivisionId3 = e.administrative_division_id_3,
                        AdministrativeDivisionName3 = e.administrative_division_name_3,
                        AdministrativeDivisionId14 = e.administrative_division_id_4,
                        AdministrativeDivisionName4 = e.administrative_division_name_4,
                        SiteType = e.site_type_value,
                        QIIndexScore = e.qi_index_score_value
                    })
                    .OrderBy(x => x.SiteName);
                return Json(sites, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateActivitySite(int activityId, int siteId, DateTime? supportStartDate, DateTime? supportEndDate, string waveFieldId, string wave, int? coachUserId, string coach)
        {
            using (Entity context = new Entity())
            {
                try
                {
                    var activitySite = context.t_activity_site.Find(new object[] { activityId, siteId });
                    activitySite.support_start_date = supportStartDate;
                    activitySite.support_end_date = supportEndDate;
                    activitySite.wave_fieldid = waveFieldId;
                    activitySite.coach_user_id = coachUserId;
                    context.SaveChanges();

                    return Json(new
                    {
                        success = true,
                        ActivityId = activitySite.activity_id,
                        SiteId = activitySite.site_id,
                        SupportStartDate = activitySite.support_start_date.HasValue ? activitySite.support_start_date.Value.ToString("d") : "",
                        SupportEndDate = activitySite.support_end_date.HasValue ? activitySite.support_end_date.Value.ToString("d") : "",
                        WaveFieldId = activitySite.wave_fieldid,
                        WaveValue = wave,
                        CoachUserId = activitySite.coach_user_id,
                        CoachUserName = coach
                    });
                }
                catch (Exception ex)
                {
                    return GetJsonResult(ex);
                }
            }
        }

        public void DataExport(int id, int SiteID = 0)
        {
            Export exp = new Export();
            DataSet ds = new DataSet();

            ds = exp.GetObservationData(Identity.CurrentUser.Id, id, 0, SiteID);

            using (XLWorkbook wb = new XLWorkbook())
            {
                foreach (DataTable dt in ds.Tables)
                {
                    if (dt.Rows.Count > 0)
                    {
                        string worksheetName = dt.Rows[0]["Site"].ToString();
                        worksheetName = worksheetName.Replace("/", "");
                        worksheetName = worksheetName.Replace("*", "");
                        worksheetName = worksheetName.Replace("[", "");
                        worksheetName = worksheetName.Replace("]", "");

                        // Complete temp hack to shorten some Site Names - need better solution
                        worksheetName = worksheetName.Replace("Centre Hospitalier Universitaire", "CHU");
                        int x = worksheetName.Length;
                        if (x > 30)
                        {
                            worksheetName = worksheetName.Substring(0, 30);
                        }
                        dt.TableName = worksheetName;
                        var ws = wb.Worksheets.Add(dt, dt.TableName);
                        ws.Tables.FirstOrDefault().ShowAutoFilter = false;
                    }
                }
                string myName = Server.UrlEncode("IID_export_" + id.ToString() + "_" + SiteID.ToString() + "_"
                    + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + ".xlsx");
                MemoryStream stream = GetStream(wb);
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=" + myName);
                Response.ContentType = "application/vnd.ms-excel";
                Response.BinaryWrite(stream.ToArray());
                Response.End();

            }
        }

        public void DataExportResearch(int id, int SiteID = 0)
        {
            Export exp = new Export();
            DataSet ds = new DataSet();

            ds = exp.GetObservationDataResearch(Identity.CurrentUser.Id, id, 0, SiteID);

            using (XLWorkbook wb = new XLWorkbook())
            {
                foreach (DataTable dt in ds.Tables)
                {
                    if (dt.Rows.Count > 0)
                    {
                        var ws = wb.Worksheets.Add(dt, "Research");
                        ws.Tables.FirstOrDefault().ShowAutoFilter = false;
                    }
                }
                string myName = Server.UrlEncode("IID_export_research_" + id.ToString() + "_"
                    + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + ".xlsx");
                MemoryStream stream = GetStream(wb);
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=" + myName);
                Response.ContentType = "application/vnd.ms-excel";
                Response.BinaryWrite(stream.ToArray());
                Response.End();

            }
        }

        private MemoryStream GetStream(XLWorkbook excelWorkbook)
        {
            MemoryStream fs = new MemoryStream();
            excelWorkbook.SaveAs(fs);
            fs.Position = 0;
            return fs;
        }
    }
}
