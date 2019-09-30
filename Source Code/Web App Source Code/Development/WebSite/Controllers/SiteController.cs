using System;
using System.Linq;
using System.Web.Mvc;

using IID.BusinessLayer.Domain;
using IID.BusinessLayer.Globalization;
using common = IID.BusinessLayer.Globalization.Common.Resource;
using IID.BusinessLayer.Models;
using IID.WebSite.Models;
using IID.WebSite.Helpers;

namespace IID.WebSite.Controllers
{
    [NoCache]
    public class SiteController : BaseController
    {
        public ActionResult Add()
        {
            return View(new Site());
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Site model)
        {
            var site = SetCoreFields(model);
            return RedirectToAction("View", new { id = site.site_id });
        }

        public ActionResult View(int id)
        {
            return View(new Site(id));
        }

        public ActionResult Edit(int id)
        {
            return View(new Site(id));
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Site model)
        {
            SetCoreFields(model);
            return RedirectToAction("View", new { id = model.SiteId.Value });
        }

        private t_site SetCoreFields(Site model)
        {
            using (Entity context = new Entity())
            {
                t_site site = null;
                if (model.SiteId.HasValue)
                {
                    site = context.t_site.Find(model.SiteId.Value);
                    if (site == null)
                        throw new ArgumentException("Invalid siteId: " + model.SiteId.ToString());
                    site.updatedby_userid = CurrentUser.Id;
                    site.updated_date = DateTime.Now;
                }
                else
                {
                    site = new t_site();
                    site.createdby_userid = CurrentUser.Id;
                    site.created_date = DateTime.Now;
                    context.t_site.Add(site);
                }

                var language = IidCulture.CurrentLanguage;

                site.country_id = model.CountryId;
                site.site_type_fieldid = model.SiteTypeFieldId;
                site.administrative_division_id =
                    model.AdministrativeDivisionId4 ?? model.AdministrativeDivisionId3 ??
                    model.AdministrativeDivisionId2 ?? model.AdministrativeDivisionId1 ?? -1;
                site.latitude = model.Latitude;
                site.longitude = model.Longitude;
                site.qi_index_score_fieldid = model.QIIndexScoreFieldId;
                site.rural_urban_fieldid = model.PopulationDensityFieldId;
                if (language == Language.English || !model.SiteId.HasValue)
                    site.other_key_information = model.OtherKeyInformation;
                site.name = model.SiteName;
                site.funding_type_fieldid = model.FundingTypeFieldId;
                site.partner = model.Partner;
                site.active = model.Active;

                context.SaveChanges();

                if (language != Language.English)
                {
                    var languageId = IidCulture.CurrentLanguageId;
                    var userId = CurrentUser.Id;
                    site.set_other_key_information_translated(languageId, model.OtherKeyInformation, userId);
                }

                return site;
            }
        }

        public ActionResult Search()
        {
            return View(new SiteSearchCriteria());
        }

        public ActionResult CoachReport(int activityId, int siteId)
        {
            return View(new CoachReport(activityId, siteId));
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public JsonResult AddToFavorites(int id)
        {
            try
            {
                using (Entity context = new Entity())
                {
                    var site = context.t_site.Find(id);
                    if (site == null)
                        throw new ArgumentException("Invalid siteId: " + id.ToString());

                    var user = context.t_user.Find(CurrentUser.Id);

                    if (!site.user_favorite_sites.Select(e => e.user_id).Contains(user.user_id))
                    {
                        site.user_favorite_sites.Add(user);
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
                    var site = context.t_site.Find(id);
                    if (site == null)
                        throw new ArgumentException("Invalid siteId: " + id.ToString());

                    var user = context.t_user.Find(CurrentUser.Id);

                    if (site.user_favorite_sites.Select(e => e.user_id).Contains(user.user_id))
                    {
                        site.user_favorite_sites.Remove(user);
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

        public ActionResult AddDivision()
        {
            return View();
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public JsonResult AddAdministrativeDivision(int countryId, int? parentAdministrativeDivisionId, string name)
        {
            try
            {
                using (Entity context = new Entity())
                {
                    var administrativeDivision = new t_administrative_division()
                    {
                        country_id = countryId,
                        createdby_userid = CurrentUser.Id,
                        created_date = DateTime.Now,
                        name = name,
                        parent_administrative_division_id = parentAdministrativeDivisionId
                    };
                    context.t_administrative_division.Add(administrativeDivision);
                    context.SaveChanges();
                    return GetJsonResult(true, null);
                }
            }
            catch(Exception ex)
            {
                return GetJsonResult(ex);
            }
        }
    }
}