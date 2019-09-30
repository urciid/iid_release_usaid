using System;
using System.Web;
using System.Web.Mvc;
using IID.WebSite.Models;
using System.Net;

namespace IID.WebSite.Controllers
{
    [NoCache]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View(new HomeViewModel());
        }
        public ActionResult NoAccess()
        {
            return View();
        }
        public ActionResult CountryDirectorReport(int id)
        {
            return View(new CountryDirectorReport(id));
        }

        public ActionResult UserStatistics()
        {
            return View(new UserStatistics(Helpers.Identity.CurrentUser.Id));
        }

        public ActionResult CountryDashboard(int id)
        {
            return View(new CountryDashboard(Helpers.Identity.CurrentUser.Id, id));
        }

        [AllowAnonymous]
        public ActionResult SetCulture(string culture)
        {
            // Validate the culture, or fall back to a safe value.
            culture = Helpers.IidCulture.GetImplementedCulture(culture);

            // Save the culture to a cookie.
            HttpCookie cookie = this.Request.Cookies[CultureCookieName];
            if (cookie != null)
            {
                cookie.Value = culture;
            }
            else
            {
                cookie = new HttpCookie(CultureCookieName);
                cookie.Value = culture;
                cookie.Expires = DateTime.MaxValue;
            }
            Response.Cookies.Add(cookie);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpPost]
        public ActionResult OnlineSync(string changes)
        {
            var logs = Models.OnlineSync.SyncChanges(changes);
            return View(logs);
        }

        [HttpGet]
        public ActionResult FileNotFound()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ServerError()
        {
            return View();
        }
    }
}