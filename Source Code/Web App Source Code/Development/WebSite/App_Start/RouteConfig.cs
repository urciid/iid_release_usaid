using System.Web.Mvc;
using System.Web.Routing;

namespace IID.WebSite
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "AddIndicator",
                url: "Indicator/Add/{activityId}",
                namespaces: new string[] { "IID.WebSite.Controllers" },
                defaults: new { controller = "Indicator", action = "Add" }
            );

            routes.MapRoute(
                name: "CoachReport",
                url: "Site/CoachReport/{activityId}/{siteId}",
                namespaces: new string[] { "IID.WebSite.Controllers" },
                defaults: new { controller = "Site", action = "CoachReport" }
            );

            routes.MapRoute(
                name: "RankByImprovement",
                url: "Activity/RankByImprovement/{activityId}/{indicatorId}",
                namespaces: new string[] { "IID.WebSite.Controllers" },
                defaults: new { controller = "Activity", action = "RankByImprovement", indicatorId = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ViewObservation",
                url: "Observation/View/{indicatorId}/{siteId}",
                namespaces: new string[] { "IID.WebSite.Controllers" },
                defaults: new { controller = "Observation", action = "View" }
            );

            routes.MapRoute(
                name: "RecordObservation",
                url: "Observation/Record/{indicatorId}/{siteId}/{beginDate}",
                namespaces: new string[] { "IID.WebSite.Controllers" },
                defaults: new { controller = "Observation", action = "Record", beginDate = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ChartIndicator",
                url: "Chart/Indicator/{id}",
                namespaces: new string[] { "IID.WebSite.Controllers" },
                defaults: new { controller = "Chart", action = "Indicator" }
            );

            routes.MapRoute(
                name: "ChartActivity",
                url: "Chart/Activity/{id}",
                namespaces: new string[] { "IID.WebSite.Controllers" },
                defaults: new { controller = "Chart", action = "Activity" }
            );

            routes.MapRoute(
                name: "ChartObservation",
                url: "Chart/Observation/{indicatorId}/{siteId}",
                namespaces: new string[] { "IID.WebSite.Controllers" },
                defaults: new { controller = "Chart", action = "Observation" }
            );

            routes.MapRoute(
                name: "ChartFromDatabase",
                url: "Chart/Load/{userId}/{chartName}",
                namespaces: new string[] { "IID.WebSite.Controllers" },
                defaults: new { controller = "Chart", action = "Load" }
            );

            routes.MapRoute(
                name: "ViewRequests",
                url: "Request/View/{flags}",
                namespaces: new string[] { "IID.WebSite.Controllers" },
                defaults: new { controller = "Request", action = "View", flags = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "RequestsAdmin",
                url: "Request/Admin/{flags}",
                namespaces: new string[] { "IID.WebSite.Controllers" },
                defaults: new { controller = "Request", action = "Admin", flags = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                namespaces: new string[] { "IID.WebSite.Controllers" },
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        protected void Application_Start()
        {
            RegisterRoutes(RouteTable.Routes);
        }
    }
}
