using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IID.BusinessLayer.Helpers;
using IID.WebSite.Helpers;
using IID.BusinessLayer.Domain;

namespace IID.WebSite.Controllers
{
    [NoCache]
    public class GeographyController : Controller
    {
        [HttpGet]
        public JsonResult GetActiveOrSelectedCountries(int? countryId = null)
        {
            using (Entity db = new Entity())
            {
                var countries = db.t_country
                    .Where(e => e.country_id == countryId || e.active == true)
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
    }
}