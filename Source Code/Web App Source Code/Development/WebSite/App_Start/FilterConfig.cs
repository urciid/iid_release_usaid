using System;
using System.Web;
using System.Web.Mvc;

namespace IID.WebSite
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new AuthorizeAttribute(), 1);
            filters.Add(new HandleErrorAttribute(), 2);
        }
    }
}
