using System;
using System.Linq;
using System.Web.Mvc;

using IID.BusinessLayer.Domain;
using IID.BusinessLayer.Globalization;
using common = IID.BusinessLayer.Globalization.Common.Resource;
using IID.WebSite.Models;
using IID.WebSite.Helpers;

namespace IID.WebSite.Controllers
{
    [NoCache]
    public class AimController : BaseController
    {
        [HttpPost()]
        [ValidateAntiForgeryToken]
        public JsonResult Add(Aim model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var aim = SetCoreFields(model);
                    return GetJsonResult(aim);
                }
                catch (Exception ex)
                {
                    return GetJsonResult(false, "An error occurred: " + ex.ToString());
                }
            }
            else
            {
                return GetJsonResult(false, "Model not valid.");
            }
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(Aim model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var aim = SetCoreFields(model);
                    return GetJsonResult(aim);
                }
                catch (Exception ex)
                {
                    return GetJsonResult(false, "An error occurred: " + ex.ToString());
                }
            }
            else
            {
                return GetJsonResult(false, "Model not valid.");
            }
        }

        private t_aim SetCoreFields(Aim model)
        {
            using (Entity context = new Entity())
            {
                t_aim aim = null;
                if (model.AimId.HasValue)
                {
                    aim = context.t_aim.Find(model.AimId.Value);
                    if (aim == null)
                        throw new ArgumentException("Invalid aimId: " + model.AimId.ToString());
                    aim.updatedby_userid = CurrentUser.Id;
                    aim.updated_date = DateTime.Now;
                }
                else
                {
                    aim = new t_aim();
                    aim.createdby_userid = CurrentUser.Id;
                    aim.created_date = DateTime.Now;

                    byte maxSort = 0;
                    var activity = context.t_activity.Find(model.ActivityId);
                    if (activity.aims.Count > 0)
                        maxSort = activity.aims.Max(e => e.sort);
                    aim.sort = maxSort += 1;

                    context.t_aim.Add(aim);
                }

                var language = IidCulture.CurrentLanguage;

                aim.activity_id = model.ActivityId;
                if (language == Language.English || !model.AimId.HasValue)
                    aim.name = model.Name;
                aim.active = model.Active;
                context.SaveChanges();

                if (language != Language.English)
                {
                    var languageId = IidCulture.CurrentLanguageId;
                    var userId = CurrentUser.Id;
                    aim.set_name_translated(languageId, model.Name, userId);
                }

                return aim;
            }
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            using (Entity context = new Entity())
            {
                try
                {
                    t_aim aim = context.t_aim.Find(id);
                    if (aim == null)
                        throw new ArgumentException("Invalid aimId: " + id.ToString());

                    context.t_aim.Remove(aim);
                    context.SaveChanges();

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return GetJsonResult(ex);
                }
            }
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public JsonResult Disable(int id)
        {
            using (Entity context = new Entity())
            {
                try
                {
                    t_aim aim = context.t_aim.Find(id);
                    if (aim == null)
                        throw new ArgumentException("Invalid aimId: " + id.ToString());

                    aim.active = false;
                    aim.updatedby_userid = CurrentUser.Id;
                    aim.updated_date = DateTime.Now;
                    context.SaveChanges();

                    return Json(new { success = true, Active = false, Status = common.InactiveStatus });
                }
                catch (Exception ex)
                {
                    return GetJsonResult(ex);
                }
            }
        }

        private JsonResult GetJsonResult(t_aim aim)
        {
            return Json(new
            {
                success = true,
                ActivityId = aim.activity_id,
                AimId = aim.aim_id,
                Name = aim.get_name_translated(IidCulture.CurrentLanguageId),
                Active = aim.active,
                Status = aim.active.HasValue ? (aim.active.Value ? common.ActiveStatus : common.InactiveStatus) : common.PendingStatus
            });
        }

        public JsonResult SortUp(int id)
        {
            using (Entity context = new Entity())
            {
                var thisAim = context.t_aim.Find(id);
                var prevAim =
                    thisAim.activity.aims
                        .OrderByDescending(e => e.sort)
                        .Where(e => e.sort < thisAim.sort)
                        .FirstOrDefault();

                if (prevAim != null)
                {
                    thisAim.sort = prevAim.sort;
                    prevAim.sort = Convert.ToByte(thisAim.sort + 1);
                    context.SaveChanges();
                }

                return GetSortResult(thisAim, prevAim);
            }
        }

        public JsonResult SortDown(int id)
        {
            using (Entity context = new Entity())
            {
                var thisAim = context.t_aim.Find(id);
                var nextAim =
                    thisAim.activity.aims
                        .OrderBy(e => e.sort)
                        .Where(e => e.sort > thisAim.sort)
                        .FirstOrDefault();

                if (nextAim != null)
                {
                    nextAim.sort = thisAim.sort;
                    thisAim.sort += 1;
                    context.SaveChanges();
                }

                return GetSortResult(nextAim, thisAim);
            }
        }

        private JsonResult GetSortResult(t_aim aim1, t_aim aim2)
        {
            return Json(new
            {
                Aim1 = new { Id = aim1.aim_id, Sort = aim1.sort },
                Aim2 = new { Id = aim2?.aim_id, Sort = aim2?.sort }
            });
        }
    }
}