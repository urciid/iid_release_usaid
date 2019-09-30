using System.Web;

using Microsoft.AspNet.Identity.Owin;

using IID.BusinessLayer.Helpers;
using IID.BusinessLayer.Identity;

namespace IID.WebSite.Helpers
{
    public static class Identity
    {
        public static ApplicationUserManager GetUserManager()
        {
            return HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
        }

        public static ApplicationSignInManager GetSignInManager()
        {
            return HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
        }

        public static IidUser CurrentUser
        {
            get
            {
                if (HttpContext.Current.Request.IsAuthenticated)
                    return GetUserManager().FindByIdAsync(HttpContext.Current.User.Id()).Result;

                return null;
            }
        }
    }
}