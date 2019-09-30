using System;
using System.Threading;
using System.Web;
using System.Web.Mvc;

using IID.BusinessLayer.Identity;

using IID.WebSite.Helpers;
using System.Linq;
using IID.WebSite.Models;

namespace IID.WebSite.Controllers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class NoCacheAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();

            base.OnResultExecuting(filterContext);
        }
    }

    public class CustomExceptionFilter : HandleErrorAttribute
    {
        //protected static readonly ILog Logger = LogManager.GetLogger(typeof(CustomExceptionFilter));

        public override void OnException(ExceptionContext filterContext)
        {
            var controller = filterContext.Controller as Controller;
            controller.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            controller.Response.TrySkipIisCustomErrors = true;
            controller.Response.AddHeader("x-chromium-appcache-fallback-override", "disallow-fallback");
            filterContext.ExceptionHandled = true;

            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var actionName = (string)filterContext.RouteData.Values["action"];
            var exception = filterContext.Exception;
            //need a model to pass exception data to error view
            var model = new HandleErrorInfo(exception, controllerName, actionName);

            var view = new ViewResult();
            view.ViewName = "ServerError";
            view.ViewData = new ViewDataDictionary();
            view.ViewData.Model = model;

            //copy any view data from original control over to error view
            //so they can be accessible.
            var viewData = controller.ViewData;
            if (viewData != null && viewData.Count > 0)
            {
                viewData.ToList().ForEach(view.ViewData.Add);
            }

            string uri = HttpContext.Current.Request.Url.AbsoluteUri;
            string errorMessage = String.Format("Error in controller {0}, action {1} ({2})", controllerName, actionName, uri);
            //Logger.Error(errorMessage, exception);
            BusinessLayer.Helpers.SendGrid.SendExceptionEmail(
                exception, uri, Identity.CurrentUser.UserName,
                IidCulture.CurrentLanguage.ToString());

            view.ExecuteResult(filterContext);
        }
    }

    [Authorize]
    [CustomExceptionFilter]
    public abstract class BaseController : Controller
    {
        //protected static readonly ILog Logger = LogManager.GetLogger(typeof(BaseController));

        protected const string CultureCookieName = "CultureCookie";

        public UserSecurityAccess AccessPass;

        public BaseController() { }

        private ApplicationSignInManager _signInManager;
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? Helpers.Identity.GetSignInManager();
            }
            private set
            {
                _signInManager = value;
            }
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Helpers.Identity.GetUserManager();
            }
            private set
            {
                _userManager = value;
            }
        }

        private BusinessLayer.Identity.IidUser _currentUser;
        public IidUser CurrentUser
        {
            get
            {
                if (_currentUser == null)
                    _currentUser = Helpers.Identity.CurrentUser;

                return _currentUser;
            }
        }

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            //Logger.Info(Request.Url.AbsoluteUri);

            string[] urlSegments = Request.Url.Segments;
            ViewBag.IsOffline = (urlSegments.Length > 1 && urlSegments[1] == "Offline/");

            string cultureName = null;

            HttpCookie cookie = this.Request.Cookies[CultureCookieName];
            string[] languages = this.Request.UserLanguages;

            if (cookie != null)
                cultureName = cookie.Value;
            else if (languages != null && languages.Length > 0)
                cultureName = languages[0];

            cultureName = IidCulture.GetImplementedCulture(cultureName);

            var culture = new System.Globalization.CultureInfo(cultureName);
            if (Thread.CurrentCulture.LCID != culture.LCID)
            {
                Thread.CurrentCulture = culture;
                Thread.CurrentUICulture = culture;
            }

            return base.BeginExecuteCore(callback, state);
        }

        private Thread Thread { get { return Thread.CurrentThread; } }

        protected JsonResult GetJsonResult(bool success, string message)
        {
            return Json(new { success = success, responseText = message });
        }

        protected JsonResult GetJsonResult(Exception ex)
        {
            return Json(new { success = false, responseText = ex.Message });
        }

        protected UserSecurityAccess SecurityGuard(IidUser user, int CountryId, int ActivityId, int SiteId)
        {
            // This function will pass determine if a given user has update or view access to a given entity
            // It will return a 2 if update access, 1 if view only access, and a 0 if no access is allowed.

            // First check user role - System Administrators have access to everything and we can return
            if (user.IsInRole(Role.SystemAdministrator))
            {
                return UserSecurityAccess.Update;
            }

            // default to no access allowed
            UserSecurityAccess returnAccess = UserSecurityAccess.NoAccess;

            // default these roles for URC to have view access
            if (user.IsInRole(Role.ActivityLeader)
                || user.IsInRole(Role.Coach)
                || user.IsInRole(Role.CountryDirector)
                || user.IsInRole(Role.OtherStaff))
            {
                returnAccess = UserSecurityAccess.ViewOnly;
            }

            int Country = 0;
            int AdminLevel1 = 0;
            int AdminLevel2 = 0;
            int AdminLevel3 = 0;
            int AdminLevel4 = 0;

            // If SiteId is passed in, we need to look up administrative divisions
            if (SiteId != 0)
            {
                Site site = new Site(SiteId);
                Country = site.CountryId;
                AdminLevel1 = site.AdministrativeDivisionId1 ?? 0; 
                AdminLevel2 = site.AdministrativeDivisionId1 ?? 0;
                AdminLevel3 = site.AdministrativeDivisionId1 ?? 0;
                AdminLevel4 = site.AdministrativeDivisionId1 ?? 0;         
            }

            // Get Activity Country if Activity is passed in and Country is not
            if (ActivityId != 0 && CountryId == 0)
            {
                Activity act = new Activity(ActivityId, false);
                CountryId = act.CountryId;
            }
            
            // Loop thru all user permissions until an update permission if matched, or until all entries have been examined
            foreach (Permission perm in user.Permissions)
            {
                switch (perm.PermissionType)
                {
                    case PermissionType.Country:
                        if (CountryId == perm.ObjectId)
                        {
                            if (perm.UpdateAccess)
                            {
                                return UserSecurityAccess.Update;  //Done we found an update
                            }
                            else
                            {
                                returnAccess = UserSecurityAccess.ViewOnly;
                            }
                        }
                        break;
                    case PermissionType.Activity:
                        if (ActivityId == perm.ObjectId)
                        {
                            if (perm.UpdateAccess)
                            {
                                return UserSecurityAccess.Update;  //Done we found an update
                            }
                            else
                            {
                                returnAccess = UserSecurityAccess.ViewOnly;
                            }
                        }
                        break;
                    case PermissionType.AdministrativeDivision:
                        // if (AdminLevel1 == perm.ObjectId || AdminLevel2 == perm.ObjectId || AdminLevel3 == perm.ObjectId || AdminLevel4 = perm.ObjectId)
                        if (AdminLevel1 == perm.ObjectId
                            || AdminLevel2 == perm.ObjectId
                            || AdminLevel3 == perm.ObjectId
                            || AdminLevel4 == perm.ObjectId)
                        {
                            if (perm.UpdateAccess)
                            {
                                return UserSecurityAccess.Update;  //Done we found an update
                            }
                            else
                            {
                                returnAccess = UserSecurityAccess.ViewOnly;
                            }
                        }
                        break;
                    case PermissionType.Site:
                        if (SiteId == perm.ObjectId)
                        {
                            if (perm.UpdateAccess)
                            {
                                return UserSecurityAccess.Update;  //Done we found an update
                            }
                            else
                            {
                                returnAccess = UserSecurityAccess.ViewOnly;
                            }
                        }
                        break;
                }                             
            }

            return returnAccess;
        }
    }
}