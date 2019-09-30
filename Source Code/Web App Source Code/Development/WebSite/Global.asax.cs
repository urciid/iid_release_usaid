using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using ExpressiveAnnotations.Attributes;
using ExpressiveAnnotations.MvcUnobtrusive.Validators;

namespace IID.WebSite
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            DataAnnotationsModelValidatorProvider.RegisterAdapter(
                typeof(RequiredIfAttribute), typeof(RequiredIfValidator));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(
                    typeof(AssertThatAttribute), typeof(AssertThatValidator));
        }

        protected void Application_EndRequest()
        {
            if (Context.Response.StatusCode == 404)
            {
                var exception = Server.GetLastError();
                var httpException = exception as HttpException;
                Response.Clear();
                Server.ClearError();
                var routeData = new RouteData();
                routeData.Values["controller"] = "Home";
                routeData.Values["action"] = "FileNotFound";
                routeData.Values["exception"] = exception;
                Response.StatusCode = 500;

                // Avoid IIS7 getting in the middle
                Response.TrySkipIisCustomErrors = true;
                IController controller = new Controllers.HomeController();
                HttpContextWrapper wrapper = new HttpContextWrapper(Context);
                var rc = new RequestContext(wrapper, routeData);
                controller.Execute(rc);
            }
        }
    }
}
